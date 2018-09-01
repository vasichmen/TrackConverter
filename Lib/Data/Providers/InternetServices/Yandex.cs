using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json.Linq;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using GMap.NET;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Mathematic.Geodesy.Projections.GMapImported;
using GMap.NET.MapProviders;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// работа с сервисом яндекс
    /// https://tech.yandex.ru/maps/doc/geocoder/desc/concepts/input_params-docpage/
    /// 
    /// Работа с маршрутихатором организована опытным путем
    /// </summary>
    public class Yandex : BaseConnection, IRouterProvider, IGeoсoderProvider, IRastrMapLayerProvider
    {
        #region вложенные классы

        /// <summary>
        /// карта спутника Яндекса
        /// </summary>
        public class SatelliteMap : BaseMapProvider
        {
            /// <summary>
            /// экземпляр провайдера
            /// </summary>
            public static readonly SatelliteMap Instance;

            static SatelliteMap()
            {
                Instance = new SatelliteMap();
            }

            /// <summary>
            /// id
            /// </summary>
            public override Guid Id
            {
                get
                {
                    return new Guid("82DC969D-0491-40F3-8C21-4D90C23F47EB");
                }
            }

            /// <summary>
            /// имя карты
            /// </summary>
            public override string Name
            {
                get
                {
                    return "YandexSatelliteMap";
                }
            }

            /// <summary>
            /// слои для отображения
            /// </summary>
            public override GMapProvider[] Overlays
            {
                get
                {
                    return new GMapProvider[1] { Instance };
                }
            }

            /// <summary>
            /// проекция карты
            /// </summary>
            public override PureProjection Projection
            {
                get
                {
                    return MercatorProjectionYandex.Instance;
                }
            }

            /// <summary>
            /// возвращает картинку по заданным координатам
            /// </summary>
            /// <param name="pos">тайловые координаты</param>
            /// <param name="zoom">масштаб</param>
            /// <returns></returns>
            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                //https://sat04.maps.yandex.net/tiles?l=sat&v=3.426.0&x=9912&y=5132&z=14&lang=ru_RU
                string url = "https://sat0{0}.maps.yandex.net/tiles?l=sat&v=3.426.0&x={1}&y={2}&z={3}&lang={4}";
                int server = new Random().Next(1, 5);
                string lang;
                switch (Vars.Options.Map.MapLanguange)
                {
                    case LanguageType.Russian:
                        lang = "ru_RU";
                        break;
                    default: throw new Exception("Этот язык не реализован");
                }

                url = string.Format(url, server, pos.X, pos.Y, zoom, lang);
                return GetGMapImage(url);
            }

        }

        /// <summary>
        /// карта яндекс.гибрид
        /// </summary>
        public class HybridMap : BaseMapProvider
        {
            /// <summary>
            /// экземпляр поставщика
            /// </summary>
            public static readonly HybridMap Instance;

            static HybridMap()
            {
                Instance = new HybridMap();
            }

            /// <summary>
            /// id
            /// </summary>
            public override Guid Id
            {
                get
                {
                    return new Guid("82D3869D-0491-40F3-8C21-4D90C23F47EB");
                }
            }

            /// <summary>
            /// название карты
            /// </summary>
            public override string Name
            {
                get
                {
                    return "YandexHybridMap";
                }
            }

            /// <summary>
            /// слои карты (спутник + слой карты гибрида)
            /// </summary>
            public override GMapProvider[] Overlays
            {
                get
                {
                    return new GMapProvider[2] { SatelliteMap.Instance, YandexHybridMapProvider.Instance };
                }
            }

            /// <summary>
            /// проекция 
            /// </summary>
            public override PureProjection Projection
            {
                get
                {
                    return MercatorProjectionYandex.Instance;
                }
            }

            /// <summary>
            /// возвращает картинку гибрида (без спутника!) по заданным координатам
            /// </summary>
            /// <param name="pos">тайловые координаты</param>
            /// <param name="zoom">масштаб</param>
            /// <returns></returns>
            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                return YandexHybridMapProvider.Instance.GetTileImage(pos, zoom);
            }
        }


        #endregion

        /// <summary>
        /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
        /// </summary>
        /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
        /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
        public Yandex(string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

        /// <summary>
        /// временная папка для маршрутов
        /// </summary>
        private string temp_fold = null;

        private string traffic_map = null;
        private DateTime lastUpdate_tm = DateTime.MinValue;

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
        /// максимальное число попыток подключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 5;
            }
        }

        #region IRouteProvider

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
        private JObject GetRouteJSON(Coordinate from, Coordinate to, TrackFile waypoints = null, string auth_token = null)
        {
            //последовательность запросов
            //https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoObjects,package.route&lang=ru-RU
            //из ответа берется токен и кладется сюда:
            //https://api-maps.yandex.ru/services/route/2.0/?callback=id_1231233466&lang=ru_RU&token=e32fd59b37713e94086b5f49c8c0abbf&rll=37.03%2C55.045~36.7537%2C54.46247&results=1&snap=rough&sign=769824

            string json = null;

            //различия параметров в запросах:

            //пешком
            //&rtm=atm&&rtt=pd

            //автомобиль
            //&rtm=dtr&

            //на транспорте
            //&rtm=atm&&rtt=mt


            //случайный id для запроса (возможно, это увеличит число доступных запросов)
            int id = new Random().Next(0, int.MaxValue);

            //параметр sign запроса
            int sign = new Random().Next(0, 999999999);

            //токен авторизации
            string token;
            if (string.IsNullOrWhiteSpace(auth_token))
                token = getRouterToken();
            else
                token = auth_token;

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
            json = SendStringGetRequest(urlGetRoute);

            //обработка ответа от сервера
            return SendJsonGetRequest(urlGetRoute);
        }

        /// <summary>
        /// возвращает токен для получения маршшрутов
        /// </summary>
        /// <returns></returns>
        string getRouterToken() {

            //запрос токена. Возвращается текст javascript , со структурой , в которой есть токен. 
            string urlGetToken = "https://api-maps.yandex.ru/2.0/?load=package.standard,package.geoObjects,package.route&lang=ru-RU";
            string js = SendStringGetRequest(urlGetToken);

            //находим токен из javascript
            int start = js.IndexOf("project_data[\"token\"]=\"") + "project_data[\"token\"]=\"".Length;
            int end = js.IndexOf("\";", start);
            int length = end - start;

            //параметр token для запроса
            string token = js.Substring(start, length);
            return token;
        }

        /// <summary>
        /// разбор ответа сервера и выбор маршрута
        /// </summary>
        /// <param name="jobj">объект JSON всей информации о маршруте (все, что прислал сервер)</param>
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
                    TrackFile part=null;
                    try
                    {
                        part = Yandex.DecodePolyline2(coords);
                    }
                    catch (OverflowException)
                    {
                        part = Yandex.DecodePolyline(coords);
                    }
                    finally
                    {
                        res.Add(part);
                    }
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
        public static TrackFile DecodePolyline2(string encodedCoordinates)
        {
            string key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=";
            int fgggg = encodedCoordinates.Length;
            //encodedCoordinates = encodedCoordinates.TrimEnd('=');
            //создание строки бит
            int[] bytes = new int[encodedCoordinates.Length];
            for (int i = 0; i < encodedCoordinates.Length; i++)
                bytes[i] = key.IndexOf(encodedCoordinates[i]);

            //создание списка . Чтение по 6 бит. Каждые 32 бита добавление в пару lon-lat
            List<int> pairs = new List<int>(); //список чисел(первые два - базовые координаты, дальше - приращения lon-lat)
            int pos = 0; //позиция, окуда начинается запись в новое число
            int a = 0x00000000; //новое число
            int c = 0; //добавляемые 6 бит из массива
            int lendob = 0; //длина добавляемой части к концу числа. (2,4,6 бит)
            int lenost = 0; //длина остатка (2,4,0 бит)
            for (int i = 0; i < bytes.Length; i++)
            {
                c = bytes[i]; //очередные 6 бит для записи в новое число
                if (pos < 32 - 6) // если в число еще влезает 6 бит, то прибавляем и переход к следующему числу
                {
                    int b = c << 32 - 6 - pos; // сдвиг на нужную позицию
                    a = a | b; //побитовое сложение
                    pos += 6; //прибавление начальной позиции
                    continue; //переход к следующему числу
                }
                else //если 6 новых бит надо разделить на 2 числа
                {
                    lendob = 32 - pos; //длина добавляемой части
                    lenost = 6 - lendob; //длина остатка

                    if (lendob != 4 && lendob != 2 && lendob != 6)
                        throw new Exception("lendob control");

                    //добавление к старому числу первой части
                    int b = c >> lenost; //сдвиг на нужную позицию
                    a = a | b; //сложение

                    int f = Perest(a); //инверсия порядка байт
                    pairs.Add(f); //добавление в список

                    //добавление остатка к новому числу
                    if (lenost != 0) //если длина остатка больше 0
                    {
                        a = 0x00000000; //новое число
                        b = c << 32 - lenost; // сдвиг на нужную позицию в начале числа
                        a = a | b; //сложение
                        pos = lenost; // следующая начальная позиция на размер остатка
                        continue; //переход к следующему числу
                    }

                    //сброс нового числа
                    a = 0x00000000;
                    pos = 0;
                }
            }
            //if (pos == 4 || pos == 2) //добавление остатка
            //{
            //    int b = int.MaxValue;
            //    int fc = b >> pos;
            //    a = a | fc;
            //    int fa = Perest(a);
            //    pairs.Add(fa);
            //}

            //создание списка координат
            List<List<int>> coords = new List<List<int>>();
            for (int i = 0; i < pairs.Count - 1; i += 2)
            {
                List<int> r = new List<int>();
                int lon = pairs[i];
                int lat = pairs[i + 1];
                r.Add(lon);
                r.Add(lat);
                coords.Add(r);
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
                res.Add(new TrackPoint(cd[1] / 1000000d, cd[0] / 1000000d));

            return res;
        }

        /// <summary>
        /// перестановка байт в обратном порядке
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public static int Perest(int inp)
        {
            uint a = (uint)inp;

            uint m = 0xFF000000;
            uint c;
            uint b = 0x00000000;

            // 1-й байт на место 4-го
            c = a & m;
            c = c >> 24;
            b = b | c;

            // 2-й байт на место 3-го
            m = m >> 8;
            c = a & m;
            c = c >> 8;
            b = b | c;

            // 3-й байт на место 2-го
            m = m >> 8;
            c = a & m;
            c = c << 8;
            b = b | c;

            // 4-й байт на место 1-го
            m = m >> 8;
            c = a & m;
            c = c << 24;
            b = b | c;

            return (int)b;
        }

        /// <summary>
        /// Старый алгоритм декодиорвания0 ломаной(через строки "в лоб")
        /// Алгоритм кодирования: 
        /// https://tech.yandex.ru/maps/doc/jsapi/1.x/dg/tasks/how-to-add-polyline-docpage/#encoding-polyline-points
        /// Для Python:
        /// https://yandex.ru/blog/mapsapi/16101
        /// Утилита кодирования:
        /// https://yandex.github.io/mapsapi-examples-old/html/mappolylineencodepoints.html
        /// </summary>
        /// <param name="encodedCoordinates"></param>
        /// <returns></returns>
        public static TrackFile DecodePolyline(string encodedCoordinates)
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

            //удаление последней точки (почему-то всегда лишняя)
            res.RemoveAt(res.Count - 1);

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
        /// построить все маршруты между точками. Используются разные потоки для получения данных о обработки ответов
        /// </summary>
        /// <param name="points">точки</param>
        /// <param name="callback">метод для вывода информации об операции</param>
        /// <returns></returns>
        public List<List<TrackFile>> CreateRoutes(BaseTrack points, Action<string> callback)
        {
            ConcurrentQueue<JObject> queue = new ConcurrentQueue<JObject>(); //очередь на обработку
            ConcurrentBag<List<TrackFile>> tracks = new ConcurrentBag<List<TrackFile>>(); //результирующая матрица маршрутов
            bool isRequestComplete = false; //истина, если завершены все запросы к серверу

            //токен авторизации
            string auth_token =getRouterToken();

            //действие обработки очереди JSON ответов сервера
            Action polylinerAction = new Action(() =>
            {
                temp_fold = "";
                if (Vars.Options.Services.UseFSCacheForCreatingRoutes)
                {
                    temp_fold = Application.StartupPath + Resources.temp_directory + @"\" + Guid.NewGuid().ToString();
                    Directory.CreateDirectory(temp_fold);
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
                        JObject jobj = null; //ждем добавления ответов в очередь
                        while (queue.IsEmpty || !queue.TryDequeue(out jobj))
                            Thread.Sleep(2000);

                        //если ответ null, то так и пишем
                        if (jobj == null)
                        {
                            row.Add(null);
                            pr++;
                        }
                        else //обработка JSON
                        {
                            if (Vars.Options.Services.UseFSCacheForCreatingRoutes) //если надо использовать кэш ФС (не обрабатывать все пути, а только те, которые будут использоваться в маршурте)
                            {
                                TrackFile res = new TrackFile();
                                pr++;

                                //получение длины маршрута
                                try
                                {
                                    JToken r1 = jobj.SelectToken("data.features[0].properties", true);

                                    string str = jobj.ToString(Newtonsoft.Json.Formatting.Indented);

                                    //в зависимости от типа маршрута выбираем название поле с длиной маршрута
                                    string distName = Vars.Options.Services.PathRouteMode == PathRouteMode.Driving ? "Distance" : Vars.Options.Services.PathRouteMode == PathRouteMode.Walk ? "WalkingDistance" : "";
                                    JToken route = r1["RouteMetaData"][distName]["value"];

                                    //сохранения расстояния
                                    string routel = route.ToString();
                                    double distance = double.Parse(routel);
                                    res.setDistance(distance);
                                    row.Add(res);

                                    //запись данных маршрута в файл
                                    string textJO = jobj.ToString(Newtonsoft.Json.Formatting.None);
                                    StreamWriter sw = new StreamWriter(temp_fold + @"\" + i.ToString() + "_" + j.ToString() + ".json");
                                    sw.Write(textJO);
                                    sw.Close();
                                }
                                catch (Exception e) { throw e; }
                            }
                            else //если надо все хранить в памяти и обрабатывать сразу
                            {
                                TrackFile res = decodeJSONPath(jobj);
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
                        JObject jo = GetRouteJSON(points[i].Coordinates, points[j].Coordinates,null, auth_token);
                        try
                        {
                            jo.SelectToken("data.features[0].features[0].properties.encodedCoordinates", true);
                        }
                        catch (JsonException e)
                        {
                            TrackPoint sp = points[i];
                            TrackPoint fp = points[j];
                            throw new ApplicationException(string.Format("Не удалось проложить маршрут между точками:\r\nНачальная точка: {0}\r\nКонечная точка: {1}", sp.Name, fp.Name));
                        }
                        queue.Enqueue(jo);
                        if (callback != null)
                            callback.BeginInvoke("Построение оптимального маршрута: получение расстояний, завершено " + (k / all * 100d).ToString("0.0") + "%" + ", путей в очереди: " + queue.Count, null, null);
                        if (polyliner.Exception != null)
                            throw new ApplicationException("Ошибка сервиса Яндекс: " + polyliner.Exception.Message, polyliner.Exception);
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
            if (string.IsNullOrEmpty(temp_fold))
                throw new ApplicationException("Отсутствует временная папка");

            string fname = temp_fold + @"\" + i.ToString() + "_" + j.ToString() + ".json";
            if (!File.Exists(fname))
                throw new ApplicationException("Файл " + fname + " не найден во временной папке");

            StreamReader sr = new StreamReader(fname);
            string jtext = sr.ReadToEnd();
            JObject jo = JObject.Parse(jtext);
            TrackFile res = decodeJSONPath(jo);
            sr.Close();
            return res;
        }

        #endregion

        #region IGeocoderProvider

        /// <summary>
        /// узнать адрес по координате. Если адрес не найден, то null
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            string url = string.Format(
               "https://geocode-maps.yandex.ru/1.x/?geocode={0}&results=1",
               coordinate.Longitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.') + "," + coordinate.Latitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'));
            XmlDocument dc = SendXmlGetRequest(url);

            XmlNode found = dc.GetElementsByTagName("found")[0];
            if (found.InnerText == "0")
                throw new ApplicationException("Яндекс не нашел ни одного объекта");

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
            XmlDocument dc = SendXmlGetRequest(url);

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
            XmlDocument xml = SendXmlGetRequest(url);

            XmlNode cord = xml.GetElementsByTagName("featureMember")[0];
            if (cord == null)
                throw new ApplicationException("Не найден адрес: " + address);
            XmlNode nd = cord.ChildNodes[0]["Point"];

            string cd = nd["pos"].InnerText;

            string[] ar = cd.Split(' ');

            Coordinate res = new Coordinate(ar[1], ar[0]);
            return res;
        }

        /// <summary>
        /// получить информацию о временной зоне
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRastrMapLayerProvider

        /// <summary>
        /// получить тайл пробок Yandex
        /// </summary>
        /// <param name="x">тайловая координата х</param>
        /// <param name="y">тайловая координата у</param>
        /// <param name="z">масштаб</param>
        /// <returns></returns>
        public Image GetRastrTile(long x, long y, int z)
        {
            //последовательность запросов:
            //сначала надо получить "версию". callback и _ можно поставить любые
            //https://api-maps.yandex.ru/services/coverage/v2/layers_stamps?lang=ru_RU&l=trf&callback=id_153566920585677507&_=89244444
            //из ответа берется версия (version) и кладется сюда в параметр tm:
            //https://jgo.maps.yandex.net/1.1/tiles?trf&l=trf&lang=ru_RU&x=39660&y=20544&z=16&scale=1&tm=1535267771

            if (traffic_map == null)
                traffic_map = getTrafficMapId();

            int att = 0;
            while (att < MaxAttempts)
            {
                try
                {
                    string url = @"https://jgo.maps.yandex.net/1.1/tiles?trf&l=trf&lang=ru_RU&x={0}&y={1}&z={2}&scale=1&tm={3}";
                    url = string.Format(url, x, y, z, traffic_map);
                    //загрузка изображения
                    try
                    {
                        Image res = GetImage(url);
                        return res;
                    }
                    catch (ApplicationException ae)
                    {
                        traffic_map = getTrafficMapId();
                        continue;
                    }
                }
                catch (WebException we) //если ошибка подключения, то ожидаем немного и пробуем снова
                {
                    att++;
                    if (att == MaxAttempts)
                        throw we;
                    Thread.Sleep(50);
                    continue;
                }
            }
            throw new Exception("Превышено количество попыток подключения");
        }

        /// <summary>
        /// получить параметр tm для запроса пробок
        /// </summary>
        /// <returns></returns>
        private string getTrafficMapId()
        {
            //последовательность запросов:
            //сначала надо получить "версию". callback и _ можно поставить любые
            //https://api-maps.yandex.ru/services/coverage/v2/layers_stamps?lang=ru_RU&l=trf&callback=id_153566920585677507&_=89244444
            //из ответа берется версия (version) и кладется сюда в параметр tm:
            //https://jgo.maps.yandex.net/1.1/tiles?trf&l=trf&lang=ru_RU&x=39660&y=20544&z=16&scale=1&tm=1535267771

            int att = 0;
            while (att < MaxAttempts)
            {
                try
                {
                    Random rn = new Random(298347923);
                    string cb_id = "153" + rn.Next(999999).ToString() + rn.Next(999999).ToString() + "507";
                    long _ = rn.Next(23497611, 99999999);
                    string url_tm = string.Format("https://api-maps.yandex.ru/services/coverage/v2/layers_stamps?lang=ru_RU&l=trf&callback=id_{0}&_={1}", cb_id, _);
                    string tm_jo = SendStringGetRequest(url_tm);

                    //получение параметра tm для следующего запроса
                    int ind = tm_jo.IndexOf("version\":\"");
                    int l = tm_jo.IndexOf("\",\"zoomRan") - ind - "\",\"zoomRan".Length;
                    int start = ind + "version\":\"".Length;
                    return tm_jo.Substring(start, l);
                }
                catch (WebException we) //если ошибка подключения, то ожидаем немного и пробуем снова
                {
                    if (att == MaxAttempts)
                        throw we;
                    Thread.Sleep(50);
                    continue;
                }
            }
            throw new Exception("Превышено количество попыток подключения");
        }

        #endregion
    }
}
