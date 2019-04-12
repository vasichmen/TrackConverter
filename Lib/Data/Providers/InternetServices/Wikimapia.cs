using GMap.NET;
using GMap.NET.WindowsForms;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;
using TrackConverter.Lib.Tracking.Helpers;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Класс взаимодействия с API Wikimapia.org
    /// </summary>
    public class Wikimapia : BaseConnection, IVectorMapLayerProvider
    {
        #region Структуры

        /// <summary>
        /// структура информации об улице
        /// </summary>
        public class StreetInfo
        {
            /// <summary>
            /// ID улицы
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// нащвание улицы
            /// </summary>
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// структура для сохранения и редактирования информации об объекте
        /// </summary>
        public class ObjectEditInfo
        {
            public ObjectEditInfo()
            { Categories = new List<string>(); Geometry = new TrackFile(); }

            public int ID { get; set; }
            public int ObjectType { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string StreetName { get; set; }
            public string BuildingNumber { get; set; }
            public bool IsBuilding { get; set; }
            public string WikipediaLink { get; set; }
            public TrackFile Geometry { get; set; }
            public List<string> Categories { get; set; }
            public int StreetID { get; set; }
        }

        /// <summary>
        /// расширенная инфорация об объекте (фотографии, комментарии итд)
        /// </summary>
        public class ExtInfo
        {
            /// <summary>
            /// информация о фотографии 
            /// </summary>
            public class PhotoInfo
            {
                /// <summary>
                /// ID фотографии
                /// </summary>
                public int ID { get; internal set; }

                /// <summary>
                /// текстовое представление даты добавления (сколько времени прошло)
                /// </summary>
                public string TimeString { get; internal set; }

                /// <summary>
                /// ID пользователя, загрузившего фотографию
                /// </summary>
                public int UploadUserID { get; internal set; }

                /// <summary>
                /// ссылка на фотографию 1280
                /// </summary>
                public string Url1280 { get; internal set; }

                /// <summary>
                /// ссылка на фотографию 960
                /// </summary>
                public string Url960 { get; internal set; }

                /// <summary>
                /// ссылка на фотографию
                /// </summary>
                public string UrlBig { get; internal set; }
                public string UrlFull { get; internal set; }

                /// <summary>
                /// ссылка на эскиз
                /// </summary>
                public string UrlThumbnail { get; internal set; }

                /// <summary>
                /// имя пользователя, добавившего фото
                /// </summary>
                public string UserName { get; internal set; }
            }

            /// <summary>
            /// информация о комментарии
            /// </summary>
            public class CommentInfo
            {
                /// <summary>
                /// дата написания или DateTime.MinValue, если точной даты нет
                /// </summary>
                public DateTime Date { get; internal set; }

                /// <summary>
                /// текст
                /// </summary>
                public string Message { get; internal set; }

                /// <summary>
                /// строковое представление даты, если нормальное недоступно
                /// </summary>
                public string StringDate { get; internal set; }

                /// <summary>
                /// Ссылка на пользователя, оставившего комментарий
                /// </summary>
                public string UserLink { get; internal set; }

                /// <summary>
                /// имя пользователя
                /// </summary>
                public string UserName { get; internal set; }

                /// <summary>
                /// фото пользователя
                /// </summary>
                public string UserPhoto { get; internal set; }
            }

            /// <summary>
            /// создаёт новый объект информации о объекте
            /// </summary>
            public ExtInfo()
            {
                Tags = new List<string>();
                Comments = new List<CommentInfo>();
                Photos = new List<PhotoInfo>();
            }

            /// <summary>
            /// Описание объекта
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// ID
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// ссылка на страницу объекта на викимапии
            /// </summary>
            public string Link { get { return "http://wikimapia.org/" + this.ID + "/ru"; } }

            /// <summary>
            /// список категорий объекта
            /// </summary>
            public List<string> Tags { get; internal set; }

            /// <summary>
            /// фотографии
            /// </summary>
            public List<PhotoInfo> Photos { get; set; }

            /// <summary>
            /// заголовок
            /// </summary>
            public string Title { get; set; }

            public TrackFile Geometry { get; set; }

            /// <summary>
            /// Название улицы
            /// </summary>
            public string Street { get; set; }

            /// <summary>
            /// номер дома
            /// </summary>
            public string BuildingNumber { get; set; }

            /// <summary>
            /// ссылка на википедию (если есть)
            /// </summary>
            public string Wikipedia { get; internal set; }

            /// <summary>
            /// список комментариев к объекту
            /// </summary>
            public List<CommentInfo> Comments { get; private set; }

            /// <summary>
            /// адрес объекта
            /// </summary>
            public string Address { get; internal set; }
        }

        /// <summary>
        /// информация о категории
        /// </summary>
        public class CategoryInfo
        {
            /// <summary>
            /// ID категории для запросов к серверу
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Название категории 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Количество объектов в категории
            /// </summary>
            public int Amount { get; set; }

            /// <summary>
            /// имя категории + количество элементов
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Name + " " + Amount;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (!(obj is CategoryInfo))
                    return false;
                return this.ID == ((CategoryInfo)obj).ID;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return 1213502048 + ID.GetHashCode();
                }
            }

            public static bool operator !=(CategoryInfo obj1, CategoryInfo obj2)
            {
                return !Equals(obj1, obj2);
            }

            public static bool operator ==(CategoryInfo obj1, CategoryInfo obj2)
            {
                return Equals(obj1, obj2);
            }
        }

        /// <summary>
        /// информация об элементе ответа на поисковый запрос
        /// </summary>
        public class SearchObjectItemInfo
        {
            /// <summary>
            /// координаты объекта
            /// </summary>
            public Coordinate Coordinates { get; set; }

            /// <summary>
            /// масштаб для показа объекта
            /// </summary>
            public int Zoom { get; set; }

            /// <summary>
            /// назание объекта
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// назание ближайшего к объекту города
            /// </summary>
            public string NearestCity { get; set; }

            /// <summary>
            /// расстояние от центра карты до объекта в километрах
            /// </summary>
            public double Distance { get; set; }
        }

        #endregion

        #region Классы

        /// <summary>
        /// сервисный класс
        /// </summary>
        public class Service : BaseConnection
        {
            /// <summary>
            /// UID авторизованного пользователя
            /// </summary>
            private int UID=-1;

            public override TimeSpan MinQueryInterval { get { return TimeSpan.FromMilliseconds(50); } }
            public override int MaxAttempts { get { return 5; } }

            /// <summary>
            /// возвращает истину, если пользователь авторизован
            /// </summary>
            public bool LoggedIn { get { return UID > 0; }  }

            public Service() : base(null)
            {

            }

            /// <summary>
            /// удаление объекта с карты (требуется авторизация)
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public bool RemoveObject(int id)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// редактирование объекта (требуется авторизация)
            /// </summary>
            /// <param name="id"></param>
            /// <param name="newInfo"></param>
            /// <returns></returns>
            public bool EditObject(int id, ObjectEditInfo newInfo)
            {

                throw new NotImplementedException();
            }

            /// <summary>
            /// создаёт новый объект от имени гостя
            /// </summary>
            /// <param name="info"></param>
            /// <returns></returns>
            public int CreateObject(ObjectEditInfo info)
            {
                //http://wikimapia.org/ajax/check/?function=place_add&qo
                //вернуть должен просто "true"

                //http://wikimapia.org/object/edit/?checkpoly=1
                //в POST параметрах: x=991292095&y=736642332&x2=992133235&y2=736927921&poly=221443%3B199770%3B0%3B73855%3B219726%3B0%3B547599%3B17380%3B841140%3B199770%3B660896%3B285589%3B425720%3B235453&surface=0&zoom=13&_time=12045
                //вернуть должен {"code":0}

                //http://wikimapia.org/object/edit/?do=saveinfo
                //в POST параметрах: id=0&parent_id=0&xy=&poly=221443%3B199770%3B0%3B73855%3B219726%3B0%3B547599%3B17380%3B841140%3B199770%3B660896%3B285589%3B425720%3B235453&x=991292095&y=736642332&zoom=13&pl=&object_type=1&langid=1&title=-&description=&street_name=&building_number=&street_id=0&tcats_add=++&tcats_del=++&tcats_curr=++&is_building=0&wikipedia=&_time=26617
                //возвращает: {"code":20,"action":{"objectId":38644399,"trackAction":"\/add.new.place.save.info.ok","promote":false}}

                //первый шаг - разрешение на создание
                string url1 = "http://wikimapia.org/ajax/check/?function=place_add&qo";
                string ans1 = SendStringGetRequest(url1, false);

                if (ans1 != "true")
                    throw new Exception("Разрешение на создание объекта не получено");

                //второй шаг - проверка геометрии
                string poly = GetPolygon(info.Geometry);
                string url2 = "http://wikimapia.org/object/edit/?checkpoly=1";
                string data2 = "x={0}&y={1}&x2={2}&y2={3}&poly={4}&surface=0&zoom=19";
                data2 = string.Format(data2,
                    (int)(info.Geometry.Bounds.Left * 10000000),
                    (int)(info.Geometry.Bounds.Bottom * 10000000),
                    (int)(info.Geometry.Bounds.Right * 10000000),
                    (int)(info.Geometry.Bounds.Top * 10000000),
                    HttpUtility.UrlEncode(poly).ToUpper() //без перевода в верхний регистр не работает
                    );
                int time = dataCheckSum(data2); //получаем хэш-сумму
                data2 += "&_time=" + time;

                string ans2 = SendStringPostRequest(url2, data2,null);
                if (ans2 != "{\"code\":0}")
                    throw new Exception("Ошибка вычисления хэш-суммы геометрии объекта");

                //третий шаг - сохранение информации
                string url3 = "http://wikimapia.org/object/edit/?do=saveinfo";
                string data3 = "id=0&parent_id=0&xy=&poly={0}&x={1}&y={2}&zoom=13&pl=&object_type={3}&langid=1&title={4}&description={5}&street_name={6}&building_number={7}&street_id={10}&tcats_add=++&tcats_del=++&tcats_curr=++&is_building={8}&wikipedia={9}";
                data3 = string.Format(data3,
                    HttpUtility.UrlEncode(poly).ToUpper(),
                    (int)(info.Geometry.Bounds.Left * 10000000),
                    (int)(info.Geometry.Bounds.Bottom * 10000000),
                    info.ObjectType,
                    HttpUtility.UrlEncode(info.Title).ToUpper(),
                    HttpUtility.UrlEncode(info.Description).ToUpper(),
                    HttpUtility.UrlEncode(info.StreetName).ToUpper(),
                    HttpUtility.UrlEncode(info.BuildingNumber).ToUpper(),
                    info.IsBuilding ? 1 : 0,
                    HttpUtility.UrlEncode(info.WikipediaLink).ToUpper(),
                    info.StreetID!=0?info.StreetID.ToString():""
                    );
                data3 += "&_time=" + dataCheckSum(data3);
                JToken ans3 = SendJsonPostRequest(url3, data3,null);
                //string ans3s = ans3.ToString();

                //обработка ошибок
                var codeNode = ans3["code"];
                if (codeNode == null)
                {
                    string message = ans3["message"].ToString();
                    throw new Exception(message);
                }
                string code = ans3["code"].ToString();

                switch (code)
                {
                    case "20": //успех
                        string ids = ans3["action"]["objectId"].ToString();
                        int id = int.Parse(ids);
                        return id;
                    case "503": //где-то ошибка в запросе
                        throw new Exception("Ошибка в геометрии объекта, контрольной сумме или в информации");
                    case "11": //уже существующий объект
                        throw new Exception("Этот объект уже добавлен на карту, попробуйте очистить кэш и загрузить карту заново");
                    default:
                        throw new Exception("Ошибка запроса.\r\nКод ошибки: " + code + "\r\nТекст:\r\n" + ans3["message"].ToString());
                }
            }

            /// <summary>
            /// хэш-сумма для проверки координат полигона, возвращает значение парамера _time при создании объекта.
            /// Источник: application.js:Wikimapia.Net.dataCheckSum(t)
            /// </summary>
            /// <param name="arg">параметры POST запроса к http://wikimapia.org/object/edit/?checkpoly=1</param>
            /// <returns></returns>
            private static int dataCheckSum(string arg)
            {
                //application.js:Wikimapia.Net.dataCheckSum(t)
                //x=378710424&y=557754084&x2=378716647&y2=557757855&poly=2145;3771;6222;1025;3808;0;0;2805&surface=0&zoom=18
                //8242

                //dataCheckSum: 
                //function (t) 
                //{ var e = 0; 
                //for (var i = 0, a = t.length; i < a; i++) 
                //{ e += t.charCodeAt(i) | 10 } 
                //return e }

                //arg = "x=370000000&y=550000000&x2=370010000&y2=550010000&poly=0%3b0%3b10000%3b10000%3b10000%3b0&surface=0&zoom=19";

                var chaarr = arg.ToCharArray();
                var e = 0;
                var a = arg.Length;
                for (int i = 0; i < a; i++)
                {
                    e += chaarr[i] | 10;
                }
                return e;
            }


            /// <summary>
            /// получение основных категорий
            /// </summary>
            /// <param name="query">часть названия категории. По умолчанию - базовые категории</param>
            /// <returns></returns>
            public List<CategoryInfo> GetCategories(string query = null)
            {
                //базовые категории
                //http://wikimapia.org/localization/tags/ru.json
                string url;


                if (string.IsNullOrWhiteSpace(query))
                    url = "http://wikimapia.org/localization/tags/ru.json"; //основные категории
                else
                    url = string.Format("http://wikimapia.org/ajax/get_category/?search_tcat={0}&format=json&lng=1", query);  //поиск по категориям

                JObject jo = SendJsonGetRequest(url);
                JToken cats = jo["categories"];
                List<CategoryInfo> res = new List<CategoryInfo>();
                if (cats == null)
                    return new List<CategoryInfo>();
                foreach (var cat in cats)
                    foreach (var catt in cat)
                    {
                        CategoryInfo i = new CategoryInfo();
                        i.Amount = int.Parse(catt["count"].ToString());
                        i.ID = int.Parse(catt["category_id"].ToString());
                        string n = catt["name"].ToString();
                        i.Name = n;
                        res.Add(i);
                    }
                return res;
            }

            /// <summary>
            /// получить список улиц в заданной области карты
            /// </summary>
            /// <param name="rect">область карты</param>
            /// <returns></returns>
            public List<StreetInfo> GetNearestStreets(RectLatLng rect)
            {
                //http://wikimapia.org/ajax/getNearestLinear/?action=near_streets&x1=37.911951541900635&y1=55.760518932336176&x2=37.917240858078&y2=55.76339829069628&ltype=0&lng=1
                string url = "http://wikimapia.org/ajax/getNearestLinear/?action=near_streets&x1={0}&y1={1}&x2={2}&y2={3}&ltype=0&lng=1";
                url = string.Format(url,
                    rect.Left.ToString().Replace(Vars.DecimalSeparator, '.'),
                    rect.Bottom.ToString().Replace(Vars.DecimalSeparator, '.'),
                    rect.Right.ToString().Replace(Vars.DecimalSeparator, '.'),
                    rect.Top.ToString().Replace(Vars.DecimalSeparator, '.'));
                JToken ans = SendJsonGetRequest(url, true);
                if (ans == null)
                    return new List<StreetInfo>();
                List<StreetInfo> res = new List<StreetInfo>();
                foreach (JToken jt in ans["array"])
                {
                    string name = jt["value"].ToString();
                    string ids = jt["id"].ToString();
                    res.Add(new StreetInfo() { Name = name, ID = int.Parse(ids) });
                }
                return res;
            }

            /// <summary>
            /// авторизация на сервере. Возвращает код ошибки, при успехе 200
            /// </summary>
            /// <param name="login"></param>
            /// <param name="password"></param>
            /// <returns></returns>
            public string Login(string login, string password)
            {
                login = "vass.vass@mail.ru";
                password = "WRitNadbichPod3";
                string pwh = "81ee968587b6ab1a95d74a723d3db996";

                string url = "http://wikimapia.org/user/login/";
                string data = string.Format("username={0}&pw1={1}", HttpUtility.UrlEncode(login), password);
                data = data + "&_time=" + dataCheckSum(data);

                //CookieCollection cook = new CookieCollection() { new Cookie("pwh", pwh,"/", "wikimapia.org"), new Cookie("uid", "2071779", "/", "wikimapia.org")  };
                JObject ans = SendJsonPostRequest(url, data,null);

                string code = ans["code"].ToString();
                switch (code)
                {
                    case "200":
                        string uids = ans["uid"].ToString();
                        int uid = int.Parse(uids);
                        this.UID = uid;
                        return "200";
                    case "400":
                        return "400";
                    default: return "unknown error";
                }

                throw new NotImplementedException();
            }

            /// <summary>
            /// получение хэш-суммы строки
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private string getHashString(string s)
            {
                //переводим строку в байт-массим  
                byte[] bytes = Encoding.UTF8.GetBytes(s); //для работы retromap должна быть кодировка UTF8

                //создаем объект для получения средст шифрования  
                MD5CryptoServiceProvider CSP =
                    new MD5CryptoServiceProvider();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = CSP.ComputeHash(bytes);

                string hash = string.Empty;

                //формируем одну цельную строку из массива  
                foreach (byte b in byteHash)
                    hash += string.Format("{0:x2}", b);

                return hash;
            }

            /// <summary>
            /// выход
            /// </summary>
            public void Logout()
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        /// <summary>
        /// список основных категорий
        /// </summary>
        private static List<CategoryInfo> basicCategories = null;

        /// <summary>
        /// список основных категорий объектов 
        /// </summary>
        public List<CategoryInfo> BasicCategories
        {
            get
            {
                if (basicCategories == null)
                    basicCategories = ServiceEngine.GetCategories();
                return basicCategories;
            }
        }

        /// <summary>
        /// Максимальное число попыток переподключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// минимальный интервал между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromSeconds(0.1);
            }
        }

        public static Service ServiceEngine;

        /// <summary>
        /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
        /// </summary>
        /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
        /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
        public Wikimapia(string cacheDirectory, double duration = 24 * 7) : base(cacheDirectory, duration)
        { }

        static Wikimapia()
        {
            ServiceEngine = new Service();
        }

        #region Слой объектов


        /// <summary>
        /// получение списка объектов в заданной области при заданном масштабе. Через API функцию BBOX
        /// http://wikimapia.org/api?action=how_to#oldbox
        /// </summary>
        /// <param name="area">область</param>
        /// <param name="perimeter">минимальный размер периметра объекта в метрах</param>
        private List<VectorMapLayerObject> getObjects2(RectLatLng area, double perimeter)
        {
            //http://api.wikimapia.org/?function=box&key=087ECBE0-AACD80A2-30627FE3-02F13F18-24AF43B7-E7C48BDE-202D8280-69841637&bbox=37.61605,55.73522,37.64279,55.74341&count=100&format=xml&language=ru&page=2&disable=location

            //координаты углов области
            double lon_max = Math.Max(area.LocationTopLeft.Lng, area.LocationRightBottom.Lng);
            double lat_max = Math.Max(area.LocationRightBottom.Lat, area.LocationTopLeft.Lat);
            double lon_min = Math.Min(area.LocationRightBottom.Lng, area.LocationTopLeft.Lng);
            double lat_min = Math.Min(area.LocationTopLeft.Lat, area.LocationRightBottom.Lat);

            string baseURL = "http://api.wikimapia.org/?function=box&key=";
            string key = Res.Properties.Resources.wikimapia_key;
            string bbox = "&bbox=" + lon_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                          "," + lat_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                          "," + lon_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                          "," + lat_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.');
            int count = 100;
            string format = "json";
            string url = string.Format(baseURL + "{0}{1}&count={2}&format={3}&language={4}", key, bbox, count, format, "ru");
            int page = 1;

            List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();

            bool stop = false; //флаг остановки цикла при достижении минимального периметра объекта
            while (!stop)
            {
                JObject jo = SendJsonGetRequest(url);
                JToken folder = jo["folder"];

                //проверка, не произошло ли ошибки
                if (folder == null)
                {
                    JToken debug = jo["debug"];
                    if (debug == null)
                        throw new Exception("Неизвестная ошибка Wikimapia");
                    else
                    {
                        string msg = debug["message"].ToString();
                        throw new Exception(msg);
                    }
                }

                //проверка превышения лимита на область и проверка на выход за диапазон
                int found = int.Parse(jo["found"].ToString()); //количество найденных объектов
                if (Math.Ceiling(found / count + 0.0d) == page) //если мы на последней странице, то останавливаем цикл
                    stop = true;
                if ((page + 1) * count >= 10000) //если достигнут предел объектов на область, то останавливаем цикл
                    stop = true;

                //если ошибок нет, то обрабатываем ответ
                if (found > 0) //если хоть что-то нашлось
                {
                    foreach (JToken jt in folder)
                    {
                        JToken polygon = jt["polygon"];
                        GMapPolygon pol = getPolygon(polygon); //преобразуем координаты
                        string name = jt["name"].ToString();
                        string link = jt["url"].ToString();
                        string id = jt["id"].ToString();
                        pol.Name = name;
                        VectorMapLayerObject lo = new VectorMapLayerObject(pol) { ID = int.Parse(id),  Name = name, LayerProvider = MapLayerProviders.Wikimapia };
                        res.Add(lo);
                    }

                    //проверяем длину периметра. Если периметр последнего объекта меньше заданного, то выходим из цикла
                    double per = getPerimeter(res[res.Count - 1].Geometry);
                    if (per < perimeter)
                        stop = true;

                    //если дошли до 2 страницы, то выход
                    if (page >= 1)
                        stop = true;

                    //переход на следующую страницу и обновление запроса
                    page++;
                    url = string.Format(baseURL + "{0}{1}&count={2}&format={3}&language={4}&page={5}&disable=location", key, bbox, count, format, "ru", page);
                }
                else
                    stop = true; //если ничего не нашлось, то выходим из цикла
            }
            return res;
        }

        /// <summary>
        /// получение списка объектов в заданной области при заданном масштабе (как браузеры)
        /// </summary>
        /// <param name="area">область</param>
        /// <param name="perimeter">минимальный размер периметра объекта в метрах</param>
        public List<VectorMapLayerObject> GetObjects(RectLatLng area, double perimeter)
        {
            //http://wikimapia.org/d?lng=1&BBOX=35.68359,55.77657,36.85938,56.87531
            //координаты углов области
            double lon_max = Math.Max(area.LocationTopLeft.Lng, area.LocationRightBottom.Lng);
            double lat_max = Math.Max(area.LocationRightBottom.Lat, area.LocationTopLeft.Lat);
            double lon_min = Math.Min(area.LocationRightBottom.Lng, area.LocationTopLeft.Lng);
            double lat_min = Math.Min(area.LocationTopLeft.Lat, area.LocationRightBottom.Lat);
            string bbox = string.Concat("&BBOX=", lon_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.'),
                               ",", lat_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.'),
                               ",", lon_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.'),
                               ",", lat_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.'));
            string url = string.Format("http://wikimapia.org/d?lng=1{0}", bbox);

            string kml = SendStringGetRequest(url);
            List<VectorMapLayerObject> res = KmlHelper.ParseWikimapiaObjectsAnswer(kml, perimeter);
            return res;
        }

        /// <summary>
        /// получить расширенную информацию об объекте(через API)
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public ExtInfo GetExtInfo2(int id)
        {
            //через API
            //http://wikimapia.org/api/#placegetbyid
            //http://api.wikimapia.org/?function=place.getbyid&id=61941&language=ru&key=087ECBE0-7869F692-405FA237-852AD116-23CA037A-A588C4C0-2000C58D-5A0D8212
            //http://api.wikimapia.org/?function=place.getbyid&id=619451&language=ru&key=087ECBE0-7869F692-405FA237-852AD116-23CA037A-A588C4C0-2000C58D-5A0D8212


            string baseURL = "http://api.wikimapia.org/?function=place.getbyid&data_blocks=main,photos,comments&key=";
            string key = Resources.wikimapia_key;
            string format = "json";
            string url = string.Format(baseURL + "{0}&format={1}&language={2}&id={3}", key, format, "ru", id);
            JObject json = SendJsonGetRequest(url);

            if (json["debug"] != null)
                throw new Exception(json["debug"]["message"].ToString());

            ExtInfo res = new ExtInfo
            {
                Description = json["description"] != null ? json["description"].ToString() : "",
                Title = json["title"].ToString(),
                //Link = "htp://wikimapia.org/" + id + "/" + json["language_iso"].ToString(),
                Wikipedia = json["wikipedia"] != null ? json["wikipedia"].ToString() : ""
            };
            foreach (JObject jo in json["tags"])
                res.Tags.Add(jo["title"].ToString());
            foreach (JObject jo in json["photos"])
            {
                ExtInfo.PhotoInfo np = new ExtInfo.PhotoInfo
                {
                    TimeString = jo["time_str"].ToString(),
                    Url960 = jo["960_url"]?.ToString(),
                    Url1280 = jo["1280_url"]?.ToString(),
                    UrlBig = jo["big_url"]?.ToString(),
                    UrlFull = jo["full_url"]?.ToString(),
                    UrlThumbnail = jo["thumbnail_url"] != null ? jo["thumbnail_url"].ToString() : "",
                    UserName = jo["user_name"].ToString()
                };
                res.Photos.Add(np);
            }
            foreach (JObject jo in json["comments"])
            {
                ExtInfo.CommentInfo nc = new ExtInfo.CommentInfo
                {
                    UserName = jo["name"].ToString(),
                    Message = jo["message"].ToString(),
                    Date = DateTime.Parse("01.01.1970") + TimeSpan.FromSeconds(int.Parse(jo["date"].ToString())),
                    UserPhoto = "http://wikimapia.org" + jo["user_photo"].ToString()
                };
                res.Comments.Add(nc);
            }
            return res;
        }

        /// <summary>
        /// Получение расширенной информации об объекте через HTML
        /// </summary>
        /// <param name="id">ID объекта</param>
        /// <returns></returns>
        public ExtInfo GetExtInfo(VectorMapLayerObject obj)
        {

            //через HTML
            //http://wikimapia.org/61941/ru

            string url = "http://wikimapia.org/";
            string lang;
            switch (Vars.Options.Map.MapLanguange)
            {
                case LanguageType.Russian:
                    lang = "ru";
                    break;
                case LanguageType.English:
                    lang = "en";
                    break;
                case LanguageType.German:
                    lang = "de";
                    break;
                default:
                    lang = "ru";
                    break;
            }
            url = string.Format(url + "{0}/{1}", obj.ID, lang);

            // url = "http://wikimapia.org/32984750/ru/"; //удаленный объект
            //url = "http://wikimapia.org/11025563/ru/"; // невидимый объект

            HtmlDocument html = SendHtmlGetRequest(url, out HttpStatusCode code);

            //HtmlDocument html = new HtmlDocument();
            //html.Load(new StreamReader("f.html"));

            //проверка на ошибку
            switch (code)
            {
                case HttpStatusCode.Accepted:
                case HttpStatusCode.Created:
                case HttpStatusCode.OK:
                    //обработка ответа сервера

                    ExtInfo res = new ExtInfo
                    {
                        ID = obj.ID
                    };

                    var body = html.DocumentNode.ChildNodes["html"].ChildNodes["body"];

                    //заголовок объекта
                    res.Title = html.DocumentNode.SelectSingleNode(@".//html/head/title").InnerText;
                    if (string.IsNullOrEmpty(res.Title))
                    {
                        var del_bage = body.SelectSingleNode(".//div[@class = 'deletion-badge']");
                        if (del_bage != null) //если это удалённый объект, то пишем
                            res.Title = "Удалённый объект";
                    }



                    //описание
                    var description = html.GetElementbyId("place-description");
                    //res.Description = (description == null ? "" : description.InnerText.Trim(new[] { '\r', '\n', ' ' })).Replace("&quot;", "\"").Replace("&amp;quot;", "\"").Replace("&#039;", "'");
                    res.Description = description == null ? "" : parseDescription(description);

                    //wikipedia
                    var wikipedia = body.SelectSingleNode(@".//div[@class = 'placeinfo-row wikipedia-link']/a");
                    res.Wikipedia = wikipedia == null ? "" : wikipedia.InnerText;

                    //адрес
                    var address = html.GetElementbyId(@"placeinfo-locationtree");
                    string addr;
                    if (address == null)
                        addr = "";
                    else
                        addr = address.InnerText.Replace("  ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                    res.Address = addr;
                    int zpt = addr.LastIndexOf(',');
                    if (zpt != -1)
                    {
                        string bnums = addr.Substring(zpt + 1);
                        int sl = addr.LastIndexOf('/');
                        string streets = addr.Substring(sl + 1, zpt - sl - 1);
                        res.Street = streets;
                        res.BuildingNumber = bnums;
                    }

                    //геометрия объекта
                    res.Geometry = obj.GeometryTrackFile;

                    //категории объекта
                    var category_block = html.GetElementbyId("placeinfo-categories");
                    if (category_block != null)
                    {
                        var categories = category_block.SelectNodes(@".//strong");
                        foreach (HtmlNode node in categories)
                            res.Tags.Add(node.InnerText);
                    }

                    //комментарии
                    var comments_blocks = body.SelectNodes(@".//div[@id='comments']/ul");
                    if (comments_blocks != null)
                        foreach (HtmlNode block in comments_blocks)
                        {
                            var comments = block.SelectNodes(@".//li[@class='comment']");
                            if (comments == null)
                                continue;
                            foreach (HtmlNode comment in comments)
                            {
                                ExtInfo.CommentInfo com = new ExtInfo.CommentInfo();

                                //дата написания комментария (только текстовая)
                                HtmlNode date = comment.SelectSingleNode(@".//span[@class = 'time']");
                                com.StringDate = date == null ? "" : date.InnerText;
                                com.Date = DateTime.MinValue;

                                //текст комментария
                                HtmlNode message = comment.SelectSingleNode(@".//div[@class = 'comment-content']");
                                com.Message = (message == null ? "" : message.InnerText.Trim(new[] { '\r', '\n', ' ' })).Replace("&quot;", "\"").Replace("&amp;quot;", "\"").Replace("&#039;", "'");

                                //автор комментария
                                HtmlNode username = comment.SelectSingleNode(@".//div[@class='comment-header']/a");
                                com.UserName = username == null ? "" : username.InnerText;

                                //ID автора
                                var userID = comment.SelectSingleNode(@".//div[@class='comment-body']/div[@class='comment-header']/a");
                                com.UserLink = userID.Attributes.Contains("href") ? "http://wikimapia.org" + userID.Attributes["href"].Value : "";

                                //фотография автора
                                HtmlNode userphoto = comment.SelectSingleNode(@".//div[@class='comment-avatar']/div/img");
                                com.UserPhoto = "http://wikimapia.org" + userphoto.Attributes["src"].Value;

                                res.Comments.Add(com);
                            }
                        }

                    //фотографии
                    var photos_blocks = body.SelectNodes(@".//div[@id='place-photos']/div");
                    if (photos_blocks != null)
                        foreach (var photo_block in photos_blocks)
                        {
                            ExtInfo.PhotoInfo pho = new ExtInfo.PhotoInfo();
                            var img = photo_block.SelectSingleNode(@".//a/img");
                            var a = photo_block.SelectSingleNode(@".//a");

                            pho.TimeString = a.Attributes.Contains("data-uploaded") ? a.Attributes["data-uploaded"].Value : "";
                            pho.UserName = a.Attributes.Contains("data-user-name") ? a.Attributes["data-user-name"].Value : "";
                            pho.UrlBig = a.Attributes.Contains("href") ? a.Attributes["href"].Value : "";
                            pho.UrlFull = a.Attributes.Contains("data-full-url") ? a.Attributes["data-full-url"].Value : "";
                            pho.UrlThumbnail = img.Attributes.Contains("src") ? img.Attributes["src"].Value : "";
                            pho.UploadUserID = a.Attributes.Contains("data-user-id") ? int.Parse(a.Attributes["data-user-id"].Value) : -1;
                            pho.ID = a.Attributes.Contains("data-id") ? int.Parse(a.Attributes["data-id"].Value) : -1;


                            res.Photos.Add(pho);
                        }



                    return res;

                //обработка ошибок
                case HttpStatusCode.NotFound:
                    throw new Exception("Объект с id " + obj.ID + " не существует");
                default:
                    throw new Exception("Неизвестная ошибка: " + code.ToString());
            }

        }

        /// <summary>
        /// получение описания объекта из HTML узла
        /// </summary>
        /// <param name="description">узел div, внутри которого описание объекта</param>
        /// <returns></returns>
        private string parseDescription(HtmlNode description)
        {
            string res = "";
            foreach (var node in description.ChildNodes)
            {
                switch (node.Name)
                {
                    case "#text":
                        res += node.InnerText;
                        break;
                    case "br":
                        res += "\r\n";
                        break;
                    case "a":
                        res += node.Attributes["href"].Value.Replace("/external_link?url=", "");
                        break;

                }
            }
            res = res.Replace("&quot;", "\"").Replace("&amp;quot;", "\"").Replace("&#039;", "'").Trim();
            return res;
        }

        /// <summary>
        /// расчёт периметра полигона в метрах
        /// </summary>
        /// <param name="geometry">полигон</param>
        /// <returns></returns>
        private double getPerimeter(GMapPolygon geometry)
        {
            double res = 0;
            for (int i = 1; i < geometry.Points.Count; i++)
                res += Vars.CurrentGeosystem.CalculateDistance(geometry.Points[i - 1], geometry.Points[i]);
            return res;
        }

        /// <summary>
        /// преобразование координат JSON в полигон карты
        /// </summary>
        /// <param name="polygon">координаты в JSON</param>
        /// <returns></returns>
        private GMapPolygon getPolygon(JToken polygon)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (JToken jt in polygon)
            {
                double lat = double.Parse(jt["y"].ToString());
                double lon = double.Parse(jt["x"].ToString());
                points.Add(new PointLatLng(lat, lon));
            }
            return new GMapPolygon(points, "noname");
        }

        /// <summary>
        /// декодирование периметра объекта из MK2. 
        /// Источник: http://wikimapia.org/js/application.js#Wikimapia.Parser.Itiles.prototype.decodePolygonMK2 = function(..
        /// </summary>
        /// <param name="base64code">кодированный периметр</param>
        /// <param name="name">название объекта</param>
        /// <returns></returns>
        private GMapPolygon getPolygon(string base64code, string name)
        {
            //name=wdfjgAshxoiBaSa{P~_SwBjLhzParNxB{NoImXnI;

            var e = base64code.Length;
            int i = 0, n = 0;
            double o = 0;
            bool s = false;
            var a = new List<PointLatLng>();

            while (i < e)
            {
                int p = 0, l = 0, c = 0;
                do
                {
                    p = base64code[i++] - 63;
                    c |= (p & 31) << l;
                    l += 5;
                } while (p >= 32);
                n += ((c & 1) != 0) ? ~(c >> 1) : c >> 1;
                if (n > 180 * 1e6 || n < -(180 * 1e6))
                {
                    s = true;
                }
                l = 0;
                c = 0;
                do
                {
                    p = base64code[i++] - 63;
                    c |= (p & 31) << l;
                    l += 5;
                } while (p >= 32);
                o += ((c & 1) != 0) ? ~(c >> 1) : c >> 1;
                double m = (o > 90 * 1e6 || o < -(90 * 1e6) ? 1e7 : 1e6);
                a.Add(new PointLatLng(o / m, n / m));
            }
            GMapPolygon res = new GMapPolygon(a, name);
            return res;
        }



        #endregion

        #region Категории


        /// <summary>
        /// получить код тайла. Источник: http://wikimapia.org/js/application.js#getQuadKey:function(..
        /// </summary>
        /// <param name="x">тайловая координата х</param>
        /// <param name="y">тайловая координата у</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        private string getQuadKey(int x, int y, int zoom)
        {
            int tileWidth = 1024;
            int tileFactor = (int)(Math.Log(tileWidth) / Math.Log(2));

            var ob = new int[][] { new int[] { -2, 1 }, new int[] { 0, 2 }, new int[] { 2, 3 } };
            var o = ob[tileFactor - 8];
            string n = "0";
            int s;
            y = ((1 << zoom - o[0]) - y - 1); //????
            zoom -= o[1];
            while (zoom >= 0)
            {
                s = 1 << zoom;
                n += ((x & s) > 0 ? 1 : 0) + ((y & s) > 0 ? 2 : 0);
                zoom--;
            }
            return n;
        }

        /// <summary>
        /// получить список точек категории в заданном тайле карты
        /// </summary>
        /// <param name="catID">ID  категории</param>
        /// <param name="x">координата x тайла</param>
        /// <param name="y">координата у тайла</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        public List<VectorMapLayerObject> GetCategoryTile(int catID, int x, int y, int zoom)
        {
            //"wikimapia.org/z1/cat/000/000/{category}/{tileID}.xy"

            //получение quad кода
            //при x=2474, y=1278, zoom=14, factor=10 => quadCode = 0302310101113
            //после этого после каждого третьего символа ставится \ и отправляется на сервер. хэш сумма в конце не имеет значения

            string quadKey = getQuadKey(x, y, zoom);
            for (int i = 0; i < quadKey.Length; i += 4)
                quadKey = quadKey.Insert(i, "/");
            string baseUrl = "http://wikimapia.org/z1/cat{0}/{1}.xy?{2}";

            string hash = new Random().Next(9999999).ToString();

            string catid = catID.ToString("000000000");
            for (int i = 0; i < catid.Length; i += 4)
                catid = catid.Insert(i, "/");

            string url = string.Format(baseUrl, catid, quadKey, hash);
            string ans = SendStringGetRequest(url, out HttpStatusCode code);

            /* разбор ответа сервера.
             * 
             * первая строка - базовые параметры запроса(через | : quadKey|первичный масштаб|остальное - обычные базовые координаты bbox)
             * третья строка - номер категории
             * список объектов: 
             * 1640327|3052997|822667|104713|92138|15|!Войсковая часть 63553 Radiocentr CUP<Centrum Łączności|1|wdfjgAshxoiBaSa{P~_SwBjLhzParNxB{NoImXnI
             * 
             *      id
             *      | приращение долготы для LeftTop относительно базовых 
             *      | приращ. широты для LeftTop 
             *      | приращение долготы для RightBottom 
             *      | отрицатлеьное приращение широты для RightBottom 
             *      | масштаб карты для вывода объекта 
             *      | название объекта 
             *      | кодированный периметр объекта
             */

            List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();
            string[] arr = ans.Split('\n');
            string[] ar = arr[0].Split('|');
            double minLat = double.Parse(ar[3]) / 1e7;
            double minLon = double.Parse(ar[4]) / 1e7;
            double maxLat = double.Parse(ar[5]) / 1e7;
            double maxLon = double.Parse(ar[6]) / 1e7;

            for (int i = 4; i < arr.Length - 1; i++) // (-1 потому что последняя строка пустая)
            {
                string[] obj = arr[i].Split(new char[] { '|' }, 9, StringSplitOptions.None);
                int id = int.Parse(obj[0]);
                string name = obj[6].TrimStart('!');
                int start = arr[i].IndexOf("|1|");
                string str1 = arr[i].Substring(start + 3);
                string str2 = obj[8];
                bool flag = str1.Length == str2.Length; //проверка параметров сплиттера. Потом можно удалить
                GMapPolygon geom = getPolygon(str2, name);
                VectorMapLayerObject item = new VectorMapLayerObject(geom) { ID = id, LayerProvider = MapLayerProviders.Wikimapia };
                res.Add(item);
            }
            return res;
        }


        #endregion

        #region Поиск

        /// <summary>
        /// поиск объектов
        /// </summary>
        /// <param name="query">строка запроса</param>
        /// <param name="point">точка, от которой надо искать</param>
        /// <param name="z">масштаб, при котором производится поиск</param>
        /// <param name="start">номер последнего элемента из предыдущего результата (на каждой странице по 10 элементов)</param>
        /// <returns></returns>
        public List<SearchObjectItemInfo> Search(string query, PointLatLng point, int z, int start)
        {
            string url = "http://wikimapia.org/search/?q=" + query;
            int y = (int)(point.Lat * 1e7);
            int x = (int)(point.Lng * 1e7);
            string jtype = "simple"; //simple - всё, geo - города, coord - координаты
            int tryp = 0;
            string data = string.Format("x={0}&y={1}&z={2}&qu={3}&jtype={4}&start={5}&try={6}", x, y, z, query, jtype, start, tryp);
            HtmlDocument html = SendHtmlPostRequest(url, data);

            HtmlNode searchlist = html.DocumentNode.SelectSingleNode(@".//div[@class='row-fluid']/ul[@class='nav searchlist']");

            List<SearchObjectItemInfo> res = new List<SearchObjectItemInfo>();
            if (searchlist == null)
                return new List<SearchObjectItemInfo>();
            HtmlNodeCollection items = searchlist.SelectNodes(@".//li[@class='search-result-item']");
            foreach (var item in items)
            {
                int zoom = item.Attributes.Contains("data-zoom") ? int.Parse(item.Attributes["data-zoom"].Value) : 14;
                double lat = item.Attributes.Contains("data-latitude") ? double.Parse(item.Attributes["data-latitude"].Value.Replace('.', Vars.DecimalSeparator)) : double.NaN;
                double lon = item.Attributes.Contains("data-longitude") ? double.Parse(item.Attributes["data-longitude"].Value.Replace('.', Vars.DecimalSeparator)) : double.NaN;

                HtmlNode name = item.SelectSingleNode(@".//div/strong");
                string nameS = name.InnerText;

                string distance = item.SelectSingleNode(@".//span[@class='label label-info']").InnerText;
                string d1 = distance.Replace("&nbsp;км", "").Replace("&nbsp;km", "");
                double dist = double.Parse(d1.Replace('.', Vars.DecimalSeparator));

                string city = item.SelectSingleNode(@".//small").InnerText.Trim(new char[] { '\r', '\n', ' ' });

                // var userID = comment.SelectSingleNode(@".//div[@class='comment-body']/div[@class='comment-header']/a");
                //com.UserLink = userID.Attributes.Contains("href") ? "http://wikimapia.org" + userID.Attributes["href"].Value : "";
                SearchObjectItemInfo ni = new SearchObjectItemInfo()
                {
                    Name = nameS,
                    Coordinates = new Coordinate(lat, lon),
                    Distance = dist,
                    NearestCity = city,
                    Zoom = zoom
                };
                res.Add(ni);
            }
            return res;
        }


        #endregion

        /// <summary>
        /// получить кодированный полигон из координат
        /// </summary>
        /// <param name="track">список точек</param>
        /// <param name="step">шаг с корорым будет идти цикл по точкам</param>
        /// <returns></returns>
        public static string GetPolygon(BaseTrack track, int step = 1)
        {
            int latBase = (int)(track.Bounds.Bottom * 10000000);
            int lonBase = (int)(track.Bounds.Left * 10000000);
            string res = "";

            for (int i = 0; i < track.Count - 1; i += step)
            {
                string ndata = "";
                int dlonA = (int)(track[i].Coordinates.Longitude * 10000000);
                int dlatA = (int)(track[i].Coordinates.Latitude * 10000000);
                int dlon = dlonA - lonBase;
                int dlat = dlatA - latBase;
                ndata += ";" + dlon.ToString() + ";" + dlat.ToString();
                res += ndata;
            }
            //добавление последней точки
            string ndata1 = "";
            int dlonA1 = (int)(track[track.Count - 1].Coordinates.Longitude * 10000000);
            int dlatA1 = (int)(track[track.Count - 1].Coordinates.Latitude * 10000000);
            int dlon1 = dlonA1 - lonBase;
            int dlat1 = dlatA1 - latBase;
            ndata1 += ";" + dlon1.ToString() + ";" + dlat1.ToString();
            res += ndata1;


            return res.Trim(';');
        }
    }
}
