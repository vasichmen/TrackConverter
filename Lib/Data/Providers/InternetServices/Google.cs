using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Interfaces;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.IO;
using System.Windows.Forms;
using TrackConverter.Res.Properties;
using TrackConverter.Lib.Mathematic.Astronomy;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Работа с API Google
    /// https://developers.google.com/maps/documentation/geocoding/intro?hl=ru
    /// https://developers.google.com/maps/documentation/elevation/intro?hl=ru
    /// https://developers.google.com/maps/documentation/directions/intro?hl=ru
    /// </summary>
    class Google : BaseConnection, IRouterProvider, IGeoсoderProvider, IGeoInfoProvider
    {
        /// <summary>
        /// временная папка для маршрутов
        /// </summary>
        private string fold;

        /// <summary>
        ///  минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                //гугл, если с одинаковой частотой подавать запросы, кидает OVER_QUERY_LIMIT. 
                //для обхода используется рандомная задержка запросов
                //https://developers.google.com/maps/documentation/directions/usage-limits?hl=ru
                return TimeSpan.FromMilliseconds(new Random().Next(500) + 50);
            }
        }

        /// <summary>
        /// максимальное чилос попыток переподключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// если истина, то это локальный источник данных
        /// </summary>
        public bool isLocal
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// проложить с помощью гугла. Возвращает null, если произошла ошибка
        /// </summary>
        /// <param name="from">откуда</param>
        /// <param name="to">куда</param>
        /// <param name="waypoints">промежуточные точки</param>
        /// <returns></returns>
        public TrackFile CreateRoute(Coordinate from, Coordinate to, TrackFile waypoints)
        {
            if (waypoints != null && waypoints.Count > 23)
                throw new ArgumentOutOfRangeException("Промежуточных точек может быть не больше 23");

            XmlDocument xml = CreateRouteXML(from, to, waypoints);
            return decodeXMLPath(xml);
        }

        /// <summary>
        /// запрос к серверу и получение XML с маршрутом
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="waypoints"></param>
        /// <returns></returns>
        private XmlDocument CreateRouteXML(Coordinate from, Coordinate to, TrackFile waypoints = null)
        {
            //параметры
            //https://developers.google.com/maps/documentation/directions/intro#TravelModes
            string param = null;
            switch (Vars.Options.Services.PathRouteMode)
            {
                case PathRouteMode.Driving:
                    param = "driving";
                    break;
                case PathRouteMode.Walk:
                    param = "walking";
                    break;
                default: throw new ArgumentException("неподдерживаемый режим: " + Vars.Options.Services.PathRouteMode);
            }

            ////Фомируем запрос к API маршрутов Google.
            string url = string.Format("http://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&waypoints=(waypoints)&sensor=false&language=ru&mode={2}",
                from.ToString("{lat},{lon}", "00.000000"),
                to.ToString("{lat},{lon}", "00.000000"),
                param
                );

            //заполнение промежуточных точек
            //https://developers.google.com/maps/documentation/directions/intro#Waypoints
            if (waypoints != null)
            {
                string wps = "";
                foreach (TrackPoint tt in waypoints)
                    wps += "via:" + tt.Coordinates.ToString("{lat},{lon}", "00.000000") + "|";
                wps = wps.TrimEnd('|');
                url = url.Replace("(waypoints)", wps);
            }
            else url = url.Replace("&waypoints=(waypoints)", "");

            XmlDocument xml = SendXmlGetRequest(url);
            return xml;
        }

        /// <summary>
        /// преобразование пути XML в TrackFile
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static TrackFile decodeXMLPath(XmlDocument xml)
        {
            XmlNode status = xml.GetElementsByTagName("status")[0];
            if (status.InnerText != "OK")
                if (status.InnerText == "ZERO_RESULTS")
                    throw new ApplicationException("Через заданные точки невозможно проложить маршрут");
                else
                    throw new ApplicationException("Произошла ошибка: " + status.InnerText);

            XmlNode leg = xml.GetElementsByTagName("leg")[0];
            TrackFile res = new TrackFile();
            foreach (XmlNode step in leg.ChildNodes)
                if (step.LocalName == "step")
                {
                    string polyline = null; //информация о ломанной
                    //флажки информации о шаге
                    bool flp = false;
                    foreach (XmlNode lc in step.ChildNodes)
                    {
                        //обработка ломанной
                        if (lc.LocalName.ToLower() == "polyline")
                        {
                            XmlNode points = lc.ChildNodes[0];
                            polyline = points.InnerText;
                            flp = true;
                        }

                        //если найдены конец шага, начало шага и ломанная, то добавляем в маршрут
                        if (flp)
                        {
                            //преобразование ломанной в маршрут шага
                            TrackFile stepTrack = ConvertPolyline(polyline);
                            //прибавление шага к маршруту
                            res.Add(stepTrack);
                            flp = false;
                        }
                    }
                }
            return res;
        }

        /// <summary>
        /// преобразует ломаную в маршрут. 
        /// Источник: https://github.com/googlemaps/android-maps-utils/blob/master/library/src/com/google/maps/android/PolyUtil.java
        /// Описание кодирования: https://developers.google.com/maps/documentation/utilities/polylinealgorithm
        /// </summary>
        /// <param name="polyline">закодированное представление ломанной</param>
        /// <returns></returns>
        private static TrackFile ConvertPolyline(string polyline)
        {
            int len = polyline.Length;
            // For speed we preallocate to an upper bound on the final length, then
            // truncate the array before returning.
            TrackFile path = new TrackFile();
            int index = 0;
            int lat = 0;
            int lng = 0;

            while (index < len)
            {
                int result = 1;
                int shift = 0;
                int b;
                do
                {
                    b = polyline[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f);
                lat += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);

                result = 1;
                shift = 0;
                do
                {
                    b = polyline[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f);
                lng += (result & 1) != 0 ? ~(result >> 1) : (result >> 1);

                path.Add(new TrackPoint(lat * 1e-5, lng * 1e-5));
            }
            return path;
        }

        /// <summary>
        /// преобразование маршрута в кодированную линию для передачи Google
        /// Источник:
        /// https://github.com/googlemaps/android-maps-utils/blob/master/library/src/com/google/maps/android/PolyUtil.java
        /// </summary>
        /// <param name="trk">маршрут</param>
        /// <returns></returns>
        private static string ConvertPolyline(BaseTrack trk)
        {
            long lastLat = 0;
            long lastLng = 0;

            string result = "";

            foreach (TrackPoint point in trk)
            {
                long lat = (long)Math.Round(point.Coordinates.Latitude.TotalDegrees * 1e5);
                long lng = (long)Math.Round(point.Coordinates.Longitude.TotalDegrees * 1e5);
                long dLat = lat - lastLat;
                long dLng = lng - lastLng;
                encodeCoord(dLat, ref result);
                encodeCoord(dLng, ref result);
                lastLat = lat;
                lastLng = lng;
            }
            return result;
        }

        /// <summary>
        /// кодирование точки в ломаную
        /// </summary>
        /// <param name="coord">координата</param>
        /// <param name="result">результат</param>
        private static void encodeCoord(long coord, ref string result)
        {
            coord = coord < 0 ? ~(coord << 1) : coord << 1;
            while (coord >= 0x20)
            {
                result += (char.ConvertFromUtf32((int)((0x20 | (coord & 0x1f)) + 63)));
                coord >>= 5;
            }
            result += (char.ConvertFromUtf32((int)(coord + 63)));
        }

        /// <summary>
        /// узнать адрес по координатам.
        /// Документация:
        /// https://developers.google.com/maps/documentation/geocoding/intro?hl=ru
        /// </summary>
        /// <param name="coordinate">координаты места</param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            //https://maps.googleapis.com/maps/api/geocode/json?latlng=40.714224,-73.961452
            string url = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?latlng={0}&languange=ru-Ru",
                coordinate.ToString("{lat},{lon}", "00.000000"));
            XmlDocument xml = SendXmlGetRequest(url);

            XmlNode status = xml.GetElementsByTagName("status")[0];
            if (status.InnerText != "OK")
            {
                if (status.InnerText.ToLower() == "zero_results")
                    throw new ApplicationException("Превышен предел запросов Google");
                if (status.InnerText.ToLower() == "over_query_limit")
                    throw new ApplicationException("Превышен предел запросов Google");
            }

            //преобразование адреса к стандартному виду
            XmlNode result = xml.GetElementsByTagName("result")[0];
            XmlNodeList parts = result.ChildNodes;
            List<XmlNode> components = new List<XmlNode>(); //части адреса
            foreach (XmlNode nd in parts)
                if (nd.LocalName == "address_component")
                    components.Add(nd);

            string res = "";

            //порядок частей адреса в стандартном виде
            List<string> format = new List<string>() { "country", "administrative_area_level_1", "locality", "route", "street_number" };

            foreach (string part in format) //поочереди ищем все части адреса
                foreach (XmlNode nd in components) //перебор всех компонентов адреса 
                    if (nd.InnerText.Contains(part)) //если это та часть, то записавыем название
                    {
                        string lname = nd["long_name"].InnerText;
                        res += lname + ", ";
                    }
            res = res.Trim(new char[] { ' ', ',' });
            return res;
        }

        /// <summary>
        /// узнать координаты адреса
        /// </summary>
        /// <param name="address">адрес места</param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            string url = string.Format(
               "http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true_or_false&language=ru",
               Uri.EscapeDataString(address));

            XmlDocument dc = SendXmlGetRequest(url);

            if (dc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {

                //Получение широты и долготы.
                XmlNodeList nodes =
                    dc.SelectNodes("//location");

                //Переменные широты и долготы.
                double latitude = 0.0;
                double longitude = 0.0;

                //Получаем широту и долготу.
                foreach (XmlNode node in nodes)
                {
                    latitude =
                       XmlConvert.ToDouble(node.SelectSingleNode("lat").InnerText.ToString());
                    longitude =
                       XmlConvert.ToDouble(node.SelectSingleNode("lng").InnerText.ToString());
                }
                return new Coordinate(latitude, longitude);
            }
            else
                throw new ApplicationException("Некорректный ответ сервера");
        }

        /// <summary>
        /// узнать высоту в указанном месте в метрах над у. м.
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            //https://maps.googleapis.com/maps/api/elevation/json?locations=39.7391536,-104.9847034&api_key=

            string url = string.Format("https://maps.googleapis.com/maps/api/elevation/xml?locations={0},{1}&api_key={2}",
                coordinate.Latitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
                coordinate.Longitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
                Resources.google_elevation_api_key);

            XmlDocument xml = SendXmlGetRequest(url);
            XmlNode status = xml.GetElementsByTagName("status")[0];
            if (status.InnerText == "OK")
            {
                string el = xml.GetElementsByTagName("elevation")[0].InnerText;
                double ell = double.Parse(el.Replace('.', Vars.DecimalSeparator));
                return ell;
            }
            else
                throw new ApplicationException("Ошибка при обработке запроса. \r\nGoogle error: " + status.InnerText);
        }

        /// <summary>
        /// записать в трек высоты точек
        /// </summary>
        /// <param name="track">трек, в который надо записать высоты</param>
        /// <param name="callback">Действие, выполняемое при получении высот точек</param>
        /// <returns></returns>
        public BaseTrack GetElevations(BaseTrack track, Action<string> callback)
        {
            //пример запроса
            //https://maps.googleapis.com/maps/api/elevation/json?locations=40.714728,-73.998672|-34.397,150.644

            List<double> els = new List<double>();
            const int maxptperrequest = 512;
            for (int i = 0; i < track.Count; i += maxptperrequest)
            {
                double pers = (((i + 0.0) / (track.Count + 0.0)) * 100d);
                if (callback != null)
                    callback.Invoke("Получение высот точек маршрута  " + track.Name + ", завершено " + pers.ToString("0.0") + "%");
                //трек для текущего запроса
                int length = track.Count - i < maxptperrequest ? track.Count - i : maxptperrequest;
                BaseTrack trk = track.GetRange(i, length);

                //ПОПЫТКА ЗАГРУЗИТЬ ОТРЕЗОК ИЗ КЭША
                //если удалось загрузить все точки из кэша, то возвращаем результат
                bool succces = Vars.dataCache.TryGetElevations(ref track);
                if (succces)
                    return track;

                //ЕСЛИ НЕ ПОЛУЧИЛОСЬ ЗАГРУЗИТЬ ИЗ КЭША
                //передача точек кодированной линией. Этим способом можно отправлять 500 точек за один раз
                string pts = "enc:" + ConvertPolyline(trk);

                string url = string.Format("https://maps.googleapis.com/maps/api/elevation/json?locations={0}", pts);

                int attempt = 0;
                while (attempt < this.MaxAttempts)
                {
                    attempt++;
                    string json = SendStringGetRequest(url);
                    //разбор результата
                    JObject obj = JObject.Parse(json);
                    if (obj["status"].ToString() == "OK")
                    {
                        foreach (JObject j in obj["results"])
                        {
                            double e = Convert.ToDouble(j["elevation"]);
                            els.Add(e);
                        }
                        break;
                    }
                    //если сервер вернул ошибку
                    else
                    {
                        string status = obj["status"].ToString();
                        if (status != "OK")
                            switch (status)
                            {
                                case "OVER_QUERY_LIMIT":
                                    //попытка увеличить задержку и попробвать ещё раз
                                    int sleep = (int)this.MinQueryInterval.TotalMilliseconds * 50;
                                    callback.Invoke("Ошибка OVER_QUERY_LIMIT, попытка " + attempt + ", ожидание " + sleep / 1000 + " cекунд...");
                                    Thread.Sleep(sleep);
                                    continue;
                                default: throw new ApplicationException("Ошибка сервиса Google: " + status);
                            }
                        if (attempt == MaxAttempts)//если достигли максимального числа попыток, то выход с ошибкой
                            throw new ApplicationException("Ошибка сервиса Google: " + status);
                    }
                }
            }

            //если не все точки получены, то ошибка
            if (els.Count != track.Count)
                throw new ApplicationException("Что-то пошло не так: не удалось получить все высоты точек.");

            //заполнение высот
            for (int i = 0; i < track.Count; i++)
                if (double.IsNaN(track[i].MetrAltitude))
                    track[i].MetrAltitude = els[i];
            Vars.dataCache.Put(track, els, callback); //запись в кэш
            return track;
        }

        /// <summary>
        /// построить все маршруты мжду точками
        /// </summary>
        /// <param name="points"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public List<List<TrackFile>> CreateRoutes(BaseTrack points, Action<string> callback)
        {
            ConcurrentQueue<XmlDocument> queue = new ConcurrentQueue<XmlDocument>(); //очередь на обработку
            ConcurrentBag<List<TrackFile>> tracks = new ConcurrentBag<List<TrackFile>>(); //результирующая матрица маршрутов
            bool isRequestComplete = false; //истина, если завершены все запросы к серверу

            //действие обработки очереди XML ответов сервера
            Action polylinerAction = new Action(() =>
            {
                //Thread.Sleep(2000); //ждем, пока добавятся в очередь ответы сервера
                fold = "";
                if (Vars.Options.Services.UseFSCacheForCreatingRoutes)
                {
                    fold = Application.StartupPath + Resources.temp_directory + @"\" + Guid.NewGuid().ToString();
                    Directory.CreateDirectory(fold);
                }
                DateTime start1 = DateTime.Now;
                double N = points.Count * points.Count;
                double pr = 0;
                int i = 0;
                while (pr < N)
                {
                    //обработка ряда
                    List<TrackFile> row = new List<TrackFile>();
                    for (int j = 0; j < points.Count; j++)
                    {
                        XmlDocument xml = null; //ждем добавления ответов в очередь
                        while (queue.IsEmpty || !queue.TryDequeue(out xml))
                            Thread.Sleep(50);

                        //если ответ null, то так и пишем
                        if (xml == null)
                        {
                            row.Add(null);
                            pr++;
                        }
                        else //обработка  XML
                        {
                            if (Vars.Options.Services.UseFSCacheForCreatingRoutes) //если надо использовать кэш ФС
                            {
                                TrackFile res = new TrackFile();
                                pr++;

                                //получение длины маршрута
                                XmlNode r1 = xml["DirectionsResponse"]["route"]["leg"]["distance"]["value"];
                                string routel = r1.InnerText;
                                double distance = double.Parse(routel);
                                res.setDistance(distance);
                                row.Add(res);

                                //запись данных маршрута в файл
                                string textxml = xml.InnerXml;
                                StreamWriter sw = new StreamWriter(fold + @"\" + i.ToString() + "_" + j.ToString() + ".xml");
                                sw.Write(textxml);
                                sw.Close();
                            }
                            else
                            {
                                //обработка XML
                                TrackFile res = decodeXMLPath(xml);
                                pr++;
                                row.Add(res);
                            }
                            if (callback != null && isRequestComplete)
                                callback.BeginInvoke("Построение оптимального маршрута: обработка результатов, завершено " + (pr / N * 100d).ToString("0.0") + "%", null, null);
                        }
                    }
                    i++;
                    tracks.Add(row);
                }
                return;
            });
            Task polyliner = new Task(polylinerAction);
            polyliner.Start(); //начинаем обработку

            //отправка запросов серверу и добавление в очередь на обработку ответов
            DateTime start = DateTime.Now;
            double k = 0; //выполненные запросы
            double all = points.Count * points.Count; //всего запросов
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) //если начальная точка равна конечной, то пути нет
                        queue.Enqueue(null);
                    else
                    {
                        int attempt = 0;
                        while (attempt < this.MaxAttempts)
                        {
                            attempt++;
                            XmlDocument xml = CreateRouteXML(points[i].Coordinates, points[j].Coordinates);
                            //проверка ответа на ошибку OVER_QUERY_LIMIT
                            XmlNode status = xml["DirectionsResponse"]["status"];
                            if (status.InnerText != "OK")
                                switch (status.InnerText)
                                {
                                    case "ZERO_RESULTS":
                                        throw new ApplicationException(string.Format("Не удалось проложить маршрут между точками:\r\nНачальная точка: {0}\r\nКонечная точка: {1}", points[i].Name, points[j].Name));
                                    case "OVER_QUERY_LIMIT":
                                        //попытка увеличить задержку и попробвать ещё раз
                                        int sleep = (int)this.MinQueryInterval.TotalMilliseconds * 15;
                                        callback.Invoke("Ошибка OVER_QUERY_LIMIT, попытка " + attempt + ", ожидание " + sleep / 1000 + " cекунд...");
                                        Thread.Sleep(sleep);
                                        if (attempt == MaxAttempts) //если достигли максимального числа попыток, то выход с ошибкой
                                            throw new ApplicationException("Ошибка сервиса Google: " + status.InnerText);
                                        continue;
                                    default: throw new ApplicationException("Ошибка сервиса Google: " + status.InnerText);
                                }

                            queue.Enqueue(xml);
                            if (callback != null)
                                callback.BeginInvoke("Построение оптимального маршрута: получение расстояний, завершено " + (k / all * 100d).ToString("0.0") + "%" + ", путей в очереди: " + queue.Count, null, null);
                            if (polyliner.Exception != null)
                                throw polyliner.Exception.InnerException;
                            break;
                        }

                    }
                    k++;
                }

            isRequestComplete = true;
            if (callback != null)
                callback.Invoke("Построение оптимального маршрута: ожидание завершения обработки... ");
            polyliner.Wait(); //ожидание завершения обработки

            //обработка идет в обратном порядке, поэтому меняем порядок строк
            List<List<TrackFile>> trks = tracks.ToList();
            trks.Reverse();
            return trks;
        }

        /// <summary>
        /// возвращает маршрут из файлового кэша (вызов при построенни графа маршрутов)
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        public TrackFile GetRouteFromFSCache(int i, int j)
        {
            if (string.IsNullOrEmpty(fold))
                throw new ApplicationException("Отсутствует временная папка");

            string fname = fold + @"\" + i.ToString() + "_" + j.ToString() + ".xml";
            if (!File.Exists(fname))
                throw new ApplicationException("Файл " + fname + " не найден во временной папке");

            XmlDocument xml = new XmlDocument();
            xml.Load(fname);
            TrackFile res = decodeXMLPath(xml);
            return res;
        }

        /// <summary>
        /// получить информацию о временной зоне
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            //https://maps.googleapis.com/maps/api/timezone/xml?location=39.6034810,-119.6822510&timestamp=1331161200&key=YOUR_API_KEY
            int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string url = string.Format("https://maps.googleapis.com/maps/api/timezone/xml?location={0},{1}&timestamp={2}&language=ru&api_key={3}",
                coordinate.Latitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
                coordinate.Longitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
                timestamp,
                Resources.google_elevation_api_key);

            XmlDocument xml = SendXmlGetRequest(url);
            XmlNode status = xml.GetElementsByTagName("status")[0];
            if (status.InnerText == "OK")
            {
                string raw_offset = xml.GetElementsByTagName("raw_offset")[0].InnerText;
                double raw_offset2 = double.Parse(raw_offset.Replace('.', Vars.DecimalSeparator));
                string dst_offset = xml.GetElementsByTagName("dst_offset")[0].InnerText;
                double dst_offset2 = double.Parse(dst_offset.Replace('.', Vars.DecimalSeparator));
                string time_zone_id2 = xml.GetElementsByTagName("time_zone_id")[0].InnerText;
                string time_zone_name2 = xml.GetElementsByTagName("time_zone_name")[0].InnerText;

                if (Vars.Options.DataSources.UseSystemTimeZones)
                    return TimeZoneInfo.FindSystemTimeZoneById(time_zone_id2);
                else
                    return TimeZoneInfo.CreateCustomTimeZone(time_zone_id2, TimeSpan.FromSeconds(raw_offset2), time_zone_name2, time_zone_name2);
            }
            else
                throw new ApplicationException("Ошибка при обработке запроса: \r\n" + status.InnerText);
        }
    }
}
