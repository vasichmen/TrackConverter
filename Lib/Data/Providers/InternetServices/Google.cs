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
        ///  минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(250);
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
            if (waypoints.Count > 23)
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

            XmlDocument xml = SendXmlRequest(url);
            return xml;
        }

        /// <summary>
        /// преобразование пути XML в TrackFile
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static TrackFile decodeXMLPath(XmlDocument xml)
        {
            if (xml.GetElementsByTagName("status")[0].InnerText != "OK")
                throw new ApplicationException("Произошла ошибка: "+ xml.GetElementsByTagName("status")[0].InnerText+"\r\n"+ xml.GetElementsByTagName("error_message")[0].InnerText);

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
        private static string ConvertPolyline(TrackFile trk)
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
            XmlDocument xml = SendXmlRequest(url);

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

            XmlDocument dc = SendXmlRequest(url);

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

            string url = string.Format("https://maps.googleapis.com/maps/api/elevation/xml?locations={0},{1}",
                coordinate.Latitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.'),
                coordinate.Longitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.')
                );

            XmlDocument xml = SendXmlRequest(url);
            XmlNode status = xml.GetElementsByTagName("status")[0];
            if (status.InnerText == "OK")
            {
                string el = xml.GetElementsByTagName("elevation")[0].InnerText;
                double ell = double.Parse(el.Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                return ell;
            }
            else
                throw new ApplicationException("Ошибка при обработке запроса: \r\n" + status.InnerText);
        }

        /// <summary>
        /// записать в трек высоты точек
        /// </summary>
        /// <param name="track">трек, в который надо записать высоты</param>
        /// <param name="callback">Действие, выполняемое при получении высот точек</param>
        /// <returns></returns>
        public TrackFile GetElevations(TrackFile track, Action<string> callback)
        {
            //пример запроса
            //https://maps.googleapis.com/maps/api/elevation/json?locations=40.714728,-73.998672|-34.397,150.644

            List<double> els = new List<double>();
            const int maxptperrequest = 512;
            for (int i = 0; i < track.Count; i += maxptperrequest)
            {
                double pers = (((i + 0.0) / (track.Count + 0.0)) * 100d);
                if (callback != null)
                    callback.Invoke("Обработка " + track.Name + ", завершено " + pers.ToString("0.0") + "%");
                //трек для текущего запроса
                int length = track.Count - i < maxptperrequest ? track.Count - i : maxptperrequest;
                TrackFile trk = track.GetRange(i, length);

                //передача точек кодированной линией. Этим способом можно отправлять 500 точек за один раз
                string pts = "enc:" + ConvertPolyline(trk);

                string url = string.Format("https://maps.googleapis.com/maps/api/elevation/json?locations={0}", pts);
                string json = SendStringRequest(url);

                //разбор результата
                JObject obj = JObject.Parse(json);
                if (obj["status"].ToString() == "OK")
                {
                    foreach (JObject j in obj["results"])
                    {
                        double e = Convert.ToDouble(j["elevation"]);
                        els.Add(e);
                    }
                }
                //если сервер вернул ошибку
                else throw new ApplicationException(obj["status"].ToString());
            }

            //если не все точки получены, то ошибка
            if (els.Count != track.Count)
                throw new ApplicationException("Что-то пошло не так: не удалось получить все высоты точек.");

            //заполнение высот
            for (int i = 0; i < track.Count; i++)
                track[i].MetrAltitude = els[i];

            return track;
        }

        /// <summary>
        /// построить все маршруты мжду точками
        /// </summary>
        /// <param name="points"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public List<List<TrackFile>> CreateRoutes(TrackFile points, Action<string> callback)
        {
            ConcurrentQueue<XmlDocument> queue = new ConcurrentQueue<XmlDocument>(); //очередь на обработку
            ConcurrentBag<List<TrackFile>> tracks = new ConcurrentBag<List<TrackFile>>(); //результирующая матрица маршрутов
            bool isRequestComplete = false; //истина, если завершены все запросы к серверу

            //действие обработки очереди XML ответов сервера
            Action polylinerAction = new Action(() =>
            {
                Thread.Sleep(2000); //ждем, пока добавятся в очередь ответы сервера
                DateTime start1 = DateTime.Now;
                double N = points.Count * points.Count;
                double pr = 0;
                while (pr < N)
                {
                    //обработка ряда
                    List<TrackFile> row = new List<TrackFile>();
                    for (int i = 0; i < points.Count; i++)
                    {
                        XmlDocument xml = null; //ждем добавления ответов в очередь
                        while (queue.IsEmpty || !queue.TryDequeue(out xml))
                            Thread.Sleep(100);

                        //если ответ null, то так и пишем
                        if (xml == null)
                        {
                            row.Add(null);
                            pr++;
                            continue; ;
                        }

                        //обработка XML
                        TrackFile res = decodeXMLPath(xml);
                        pr++;
                        res.Distance = res.CalculateDistance(); //рассчет длины маршрута
                        row.Add(res);
                    }
                    tracks.Add(row);
                    if (isRequestComplete)
                        if (callback != null)
                            callback.Invoke("Построение оптимального маршрута: обработка результатов, завершено " + (pr / N * 100d).ToString("0.0") + "%");
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
                        XmlDocument jo = CreateRouteXML(points[i].Coordinates, points[j].Coordinates);
                        queue.Enqueue(jo);
                        callback.BeginInvoke("Построение оптимального маршрута: получение расстояний, завершено " + (k / all * 100d).ToString("0.0") + "%", null, null);
                    }
                    k++;
                }

            isRequestComplete = true;
            Thread.Sleep(100);
            if (callback != null)
                callback.Invoke("Построение оптимального маршрута: ожидание завершения обработки... ");
            polyliner.Wait(); //ожидание завершения обработки

            //обработка идет в обратном порядке, поэтому меняем порядок строк
            List<List<TrackFile>> trks = tracks.ToList();
            trks.Reverse();
            return trks;
        }
    }
}
