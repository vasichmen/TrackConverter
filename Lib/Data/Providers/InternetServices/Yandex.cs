using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// работа с сервисом яндекс
    /// https://tech.yandex.ru/maps/doc/geocoder/desc/concepts/input_params-docpage/
    /// 
    /// Работа с маршрутихатором организована опытным путем
    /// </summary>
    class Yandex : BaseConnection, IRouterProvider, IGeoсoderProvider
    {
        /// <summary>
        /// минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(100);
            }
        }

        /// <summary>
        /// проложить маршрут
        /// </summary>
        /// <param name="from">начальная точка</param>
        /// <param name="to">конечная точка</param>
        /// <param name="waypoints">промежуточные точки</param>
        /// <returns></returns>
        public TrackFile CreateRoute(Coordinate from, Coordinate to, TrackFile waypoints)
        {
            JObject jobj = GetRouteJSON(from, to, waypoints);
            return decodeJSONPath(jobj);
        }

        /// <summary>
        /// получить информацию о маршруте в формате JSON
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="waypoints"></param>
        /// <returns></returns>
        private JObject GetRouteJSON(Coordinate from, Coordinate to, TrackFile waypoints = null)
        {
            //последовательность запросов
            //https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoObjects,package.route&lang=ru-RU
            //из ответа берется токен и кладется сюда:
            //https://api-maps.yandex.ru/services/route/2.0/?callback=id_1231233466&lang=ru_RU&token=e32fd59b37713e94086b5f49c8c0abbf&rll=37.03%2C55.045~36.7537%2C54.46247&results=1&snap=rough&sign=769824

            JObject jobj = null;
            string json = null;

            //различия параметров в запросах:

            //пешком
            //&rtm=atm&&rtt=pd

            //автомобиль
            //&rtm=dtr&

            //на транспорте
            //&rtm=atm&&rtt=mt

            //запрос токена. Возвращается текст javascript , со структурой , в которой есть токен. 
            string urlGetToken = "https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoObjects,package.route&lang=ru-RU";
            string js = SendStringRequest(urlGetToken);

            //находим токен из javascript
            int start = js.IndexOf("project_data[\"token\"]=\"") + "project_data[\"token\"]=\"".Length;
            int end = js.IndexOf("\";", start);
            int length = end - start;

            //параметр token для запроса
            string token = js.Substring(start, length);

            //случайный id для запроса (возможно, это увеличит число доступных запросов)
            int id = new Random().Next(0, int.MaxValue);

            //параметр sign запроса
            int sign = new Random().Next(0, 999999999);

            //параметры запроса ( на автомобиле, пешком)
            string param = null;
            switch (Vars.Options.Services.PathRouteMode)
            {
                case PathRouteMode.Driving:
                    param = "rtm=dtr";
                    break;
                case PathRouteMode.Walk:
                    param = "rtm=atm&&rtt=pd";
                    break;
                default: throw new ArgumentException("Неподдерживаемый тип маршрута " + Vars.Options.Services.PathRouteMode);
            }

            //промежуточные точки
            string points = from.ToString("{lon}%2C{lat}", "00.000000");
            if (waypoints != null && waypoints.Count > 0)
                foreach (TrackPoint tt in waypoints)
                    points += "~" + tt.Coordinates.ToString("{lon}%2C{lat}", "00.000000");
            points += "~" + to.ToString("{lon}%2C{lat}", "00.000000");

            //запрос маршрута
            string urlGetRoute = string.Format(
                "https://api-maps.yandex.ru/services/route/2.0/?callback=id_{0}&lang=ru_RU&token={1}&rll={2}&{3}&results=1&snap=rough&sign={4}",
                id,
                token,
                points,
                param,
                sign
                );
            json = SendStringRequest(urlGetRoute);

            //обработка ответа от сервера
            json = json.Substring(json.IndexOf('{'));
            json = json.TrimEnd(new char[] { ';', ')' });
            try
            {
                jobj = JObject.Parse(json);
            }
            catch (Exception ex) { throw new ApplicationException("Ошибка в парсере JSON. Сервер вернул некорректный объект.", ex); }

            return jobj;
        }

        /// <summary>
        /// разбор ответа сервера и выбор маршрута
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private static TrackFile decodeJSONPath(JObject jobj)
        {
            try
            {
                JToken route = jobj.SelectToken("data.features[0].features", true);
                TrackFile res = new TrackFile();
                foreach (JToken jt in route)
                {
                    string coords = jt["properties"]["encodedCoordinates"].ToString();
                    TrackFile part = Yandex.DecodePolyline(coords);
                    res.Add(part);
                }
                return res;
            }
            catch (Exception)
            {
                throw new ApplicationException("Через заданные точки яндекс не смог построить маршрут.\r\n(путь нельзя проложить в выбранном районе)");
            }
        }

        /// <summary>
        /// декодирование ломаной быстрым способом
        /// </summary>
        /// <param name="encodedCoordinates"></param>
        /// <returns></returns>
        private static TrackFile DecodePolyline2(string encodedCoordinates)
        {
            throw new NotImplementedException();

            string key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=";

            //создание строки бит
            byte[] bytes = new byte[encodedCoordinates.Length];
            for (int i = 0; i < encodedCoordinates.Length;i++)
                bytes[i] = (byte)key.IndexOf(encodedCoordinates[i]);

            //создание списка . Чтение по 6 бит. Каждые 32 бита добавление в пару lon-lat


            //изменение порядка на обратный

            List<List<string>> list = null;
            //перевод в 10 СС с учетом знака
            List<List<int>> coords = new List<List<int>>();
            foreach (List<string> cd in list)
            {
                List<int> c = new List<int>();
                foreach (string s in cd)
                {
                    int r = Convert.ToInt32(s, 2);
                    c.Add(r);
                }
                coords.Add(c);
            }

            //сложение сдвигов 
            for (int i = 1; i < coords.Count; i++)
            {
                coords[i][0] = coords[i - 1][0] + coords[i][0];
                coords[i][1] = coords[i - 1][1] + coords[i][1];
            }

            //запись результата
            TrackFile res = new TrackFile();
            foreach (List<int> cd in coords)
                res.Add(new TrackPoint((double)(cd[1] / 1000000d), (double)(cd[0] / 1000000d)));

            return res;
        }

        /// <summary>
        /// декодиорвание ломаной.
        /// Алгоритм кодирования: 
        /// https://tech.yandex.ru/maps/doc/jsapi/1.x/dg/tasks/how-to-add-polyline-docpage/#encoding-polyline-points
        /// Для Python:
        /// https://yandex.ru/blog/mapsapi/16101
        /// </summary>
        /// <param name="encodedCoordinates"></param>
        /// <returns></returns>
        private static TrackFile DecodePolyline(string encodedCoordinates)
        {
            string key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=";
            encodedCoordinates = encodedCoordinates.Replace("=", "");
            //создание строки бит
            string bin = "";
            for (int i = 0; i < encodedCoordinates.Length; i++)
            {
                int ind = key.IndexOf(encodedCoordinates[i]); //поиск символа в ключе
                if (ind == -1)
                    throw new ArgumentException("В кодированной ломаной присутствуют недопустимые символы");
                //if (ind == 64) { bin += "000000";continue; }
                string n = Convert.ToString(ind, 2); //преобразование в bin
                string n2 = n.PadLeft(6, '0'); //выравнивание
                bin += n2;
                if (bin.Length % 6 != 0)
                    return null;
            }
            //добавление нулей справа для делимости на 64 байт
            bin = bin.PadRight((bin.Length / 64) * 64 + 64, '1');

            //создание списка 
            List<List<string>> list = new List<List<string>>();
            for (int i = 0; i < bin.Length; i += 64)
            {
                string lon = bin.Substring(i, 32);
                string lat = bin.Substring(i + 32, 32);
                list.Add(new List<string>() { lon, lat });
            }

            //изменение порядка на обратный
            foreach (List<string> cd in list)
            {
                string lon = cd[0];
                string nlon = "";
                nlon += lon.Substring(24, 8);
                nlon += lon.Substring(16, 8);
                nlon += lon.Substring(8, 8);
                nlon += lon.Substring(0, 8);

                string lat = cd[1];
                string nlat = "";
                nlat += lat.Substring(24, 8);
                nlat += lat.Substring(16, 8);
                nlat += lat.Substring(8, 8);
                nlat += lat.Substring(0, 8);

                cd[0] = nlon;
                cd[1] = nlat;
            }

            //перевод в 10 СС с учетом знака
            List<List<int>> coords = new List<List<int>>();
            foreach (List<string> cd in list)
            {
                List<int> c = new List<int>();
                foreach (string s in cd)
                {
                    int r = Convert.ToInt32(s, 2);
                    c.Add(r);
                }
                coords.Add(c);
            }

            //сложение сдвигов 
            for (int i = 1; i < coords.Count; i++)
            {
                coords[i][0] = coords[i - 1][0] + coords[i][0];
                coords[i][1] = coords[i - 1][1] + coords[i][1];
            }

            //запись результата
            TrackFile res = new TrackFile();
            foreach (List<int> cd in coords)
                res.Add(new TrackPoint((double)(cd[1] / 1000000d), (double)(cd[0] / 1000000d)));

            return res;
        }

        /// <summary>
        /// кодирование координат в base64 
        /// https://tech.yandex.ru/maps/doc/jsapi/1.x/dg/tasks/how-to-add-polyline-docpage/#encoding-polyline-points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private static string EncodePolyline(TrackFile points)
        {
            throw new NotImplementedException("Yandex.EncodePolyline не реализован");

            points = new TrackFile();
            points.Add(new TrackPoint("55.742186", "37.623521"));

            //координаты в целом виде
            List<List<int>> coords = new List<List<int>>();
            foreach (TrackPoint tt in points)
            {
                List<int> cd = new List<int>(); //lon lat
                cd.Add((int)(tt.Coordinates.Longitude.TotalDegrees * 1000000d));
                cd.Add((int)(tt.Coordinates.Latitude.TotalDegrees * 1000000d));
                coords.Add(cd);
            }

            //вычисление смещений координат (идем с конца массива)
            for (int i = coords.Count - 1; i > 1; i++)
            {
                int nlon = coords[i][0] - coords[i - 1][0];
                int nlat = coords[i][1] - coords[i - 1][1];
                coords[i][0] = nlon;
                coords[i][1] = nlat;
            }

            //перевод координат в двоичную систему
            List<List<string>> coordsBin = new List<List<string>>();
            foreach (List<int> cd in coords)
            {
                //долгота
                string lon = Convert.ToString(cd[0], 2); //перевод в двоичную СС
                lon = lon.PadLeft(32, '0'); //выравнивание до 32 символов

                //широта
                string lat = Convert.ToString(cd[1], 2); //перевод в двоичную СС
                lat = lat.PadLeft(32, '0'); //выравнивание до 32 символов

                List<string> cdBin = new List<string>();
                cdBin.Add(lon);
                cdBin.Add(lat);
                coordsBin.Add(cdBin);
            }

            //изменение порядка на обратный
            foreach (List<string> cd in coordsBin)
            {
                string lon = cd[0];
                string nlon = "";
                nlon += lon.Substring(24, 8);
                nlon += lon.Substring(16, 8);
                nlon += lon.Substring(8, 8);
                nlon += lon.Substring(0, 8);

                string lat = cd[1];
                string nlat = "";
                nlat += lat.Substring(24, 8);
                nlat += lat.Substring(16, 8);
                nlat += lat.Substring(8, 8);
                nlat += lat.Substring(0, 8);

                cd[0] = nlon;
                cd[1] = nlat;
            }

            //кодирование по 6 байт
            string key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=";
            string bin = "";
            foreach (List<string> cd in coordsBin) //перевод всех координат в одну строку
                bin += cd[0] + cd[1];

            //кодирование
            string res = "";
            for (int i = 0; i < bin.Length; i += 6)
            {
                string b = bin.Substring(i, 6);
                double pos = 0;
                pos = int.Parse(b[5].ToString()) * Math.Pow(2, 0);
                pos += int.Parse(b[4].ToString()) * Math.Pow(2, 1);
                pos += int.Parse(b[3].ToString()) * Math.Pow(2, 2);
                pos += int.Parse(b[2].ToString()) * Math.Pow(2, 3);
                pos += int.Parse(b[1].ToString()) * Math.Pow(2, 4);
                pos += int.Parse(b[0].ToString()) * Math.Pow(2, 5);

                res += key[(int)pos];
            }
            return res;
        }

        /// <summary>
        /// узнать адрес по координате
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            string url = string.Format(
               "https://geocode-maps.yandex.ru/1.x/?geocode={0}&results=1",
               coordinate.Longitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.') + "," + coordinate.Latitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.'));
            XmlDocument dc = SendXmlRequest(url);

            XmlNode n001 = dc["ymaps"];
            XmlNode n01 = n001["GeoObjectCollection"];
            XmlNode n1 = n01["featureMember"];
            XmlNode n2 = n1["GeoObject"];
            XmlNode n3 = n2["metaDataProperty"];
            XmlNode n4 = n3["GeocoderMetaData"];
            XmlNode n5 = n4["text"];

            return n5.InnerText;
        }

        /// <summary>
        /// получить координаты, подходящие под запрос. Адреса будут добавлены в словарь 
        /// </summary>
        /// <param name="query">часть адреса</param>
        /// <returns></returns>
        public Dictionary<string, Coordinate> GetAddresses(string query)
        {
            string url = string.Format("https://geocode-maps.yandex.ru/1.x/?geocode={0}", query);
            XmlDocument dc = SendXmlRequest(url);

            XmlNode root = dc["ymaps"];
            XmlNode collection = root["GeoObjectCollection"];

            XmlNodeList features = collection.ChildNodes;
            Dictionary<string, Coordinate> res = new Dictionary<string, Coordinate>();
            foreach (XmlNode feature in features)
            {
                if (feature.LocalName == "metaDataProperty") continue;
                if (feature.LocalName == "featureMember")
                {
                    XmlNode geoobj = feature["GeoObject"];
                    string description;
                    if (geoobj["description"] != null)
                        description = geoobj["description"].InnerText;
                    else
                        description = "";
                    string name = geoobj["name"].InnerText;
                    string title = name + ", " + description;
                    string coords = geoobj["Point"]["pos"].InnerText;
                    string lon = coords.Split(' ')[0];
                    string lat = coords.Split(' ')[1];
                    Coordinate crd = new Coordinate(lat, lon);
                    if (!res.ContainsKey(title))
                        res.Add(title, crd);
                }
            }
            return res;
        }

        /// <summary>
        /// получить координаты адреса
        /// </summary>
        /// <param name="address">адрес</param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {

            string url = string.Format("https://geocode-maps.yandex.ru/1.x/?geocode={0}", address);
            XmlDocument xml = SendXmlRequest(url);

            XmlNode cord = xml.GetElementsByTagName("featureMember")[0];
            if (cord==null)
                throw new ApplicationException("Не найден адрес: " + address);
            XmlNode nd = cord.ChildNodes[0]["Point"];

            string cd = nd["pos"].InnerText;

            string[] ar = cd.Split(' ');

            Coordinate res = new Coordinate(ar[1], ar[0]);
            return res;
        }

        /// <summary>
        /// построить все маршруты между точками. Используются разные потоки для получения данных о обработки ответов
        /// </summary>
        /// <param name="points">точки</param>
        /// <param name="callback">метод для вывода информации об операции</param>
        /// <returns></returns>
        public List<List<TrackFile>> CreateRoutes(TrackFile points, Action<string> callback)
        {
            ConcurrentQueue<JObject> queue = new ConcurrentQueue<JObject>(); //очередь на обработку
            ConcurrentBag<List<TrackFile>> tracks = new ConcurrentBag<List<TrackFile>>(); //результирующая матрица маршрутов
            bool isRequestComplete = false; //истина, если завершены все запросы к серверу

            //действие обработки очереди JSON ответов сервера
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
                        JObject jobj = null; //ждем добавления ответов в очередь
                        while (queue.IsEmpty || !queue.TryDequeue(out jobj))
                            Thread.Sleep(2000);

                        //если ответ null, то так и пишем
                        if (jobj == null)
                        {
                            row.Add(null);
                            pr++;
                            continue; ;
                        }

                        //обработка JSON
                        TrackFile res = decodeJSONPath(jobj);
                        pr++;
                        res.Distance = res.CalculateDistance(); //рассчет длины маршрута
                        row.Add(res);
                        if (callback != null && isRequestComplete)
                            callback.BeginInvoke("Построение оптимального маршрута: обработка результатов, завершено " + (pr / N * 100d).ToString("0.0") + "%", null, null);
                    }
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
                        JObject jo = GetRouteJSON(points[i].Coordinates, points[j].Coordinates);
                        jo.SelectToken("data.features[0].features[0].properties.encodedCoordinates", true);
                        queue.Enqueue(jo);
                        if (callback != null)
                            callback.BeginInvoke("Построение оптимального маршрута: получение расстояний, завершено " + (k / all * 100d).ToString("0.0") + "%" + ", путей в очереди: " + queue.Count, null, null);
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
