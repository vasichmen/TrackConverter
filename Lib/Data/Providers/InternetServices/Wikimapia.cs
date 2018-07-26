using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using GMap.NET.WindowsForms;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using TrackConverter.Lib.Tracking.Helpers;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Класс взаимодействия с API Wikimapia.org
    /// </summary>
    public class Wikimapia : BaseConnection, IVectorMapLayerProvider
    {
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
                /// текстовое представление даты добавления (сколько времени прошло)
                /// </summary>
                public string TimeString { get; internal set; }

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
                /// дата написания
                /// </summary>
                public DateTime Date { get; internal set; }

                /// <summary>
                /// текст
                /// </summary>
                public string Message { get; internal set; }

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
            public string Link { get; internal set; }

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

            /// <summary>
            /// ссылка на википедию (если есть)
            /// </summary>
            public string Wikipedia { get; internal set; }

            /// <summary>
            /// список комментариев к объекту
            /// </summary>
            public List<CommentInfo> Comments { get; private set; }
        }

        /// <summary>
        /// создаение нового объекта взаимодействия с Wikimapia.org
        /// </summary>
        public Wikimapia()
        { }

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
            string bbox = "&BBOX=" + lon_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                               "," + lat_min.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                               "," + lon_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.') +
                               "," + lat_max.ToString("0.00000").Replace(Vars.DecimalSeparator, '.');
            string url = string.Format("http://wikimapia.org/d?lng=1{0}", bbox);
            string kml = SendStringGetRequest(url);
            List<VectorMapLayerObject> res = KmlHelper.ParseWikimapiaObjectsAnswer(kml);
            return res;
        }

        /// <summary>
        /// получить расширенныю информацию об объекте
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public ExtInfo GetExtInfo(int id)
        {
            //через API
            //http://wikimapia.org/api/#placegetbyid
            //http://api.wikimapia.org/?function=place.getbyid&id=61941&language=ru&key=087ECBE0-7869F692-405FA237-852AD116-23CA037A-A588C4C0-2000C58D-5A0D8212
            //http://api.wikimapia.org/?function=place.getbyid&id=619451&language=ru&key=087ECBE0-7869F692-405FA237-852AD116-23CA037A-A588C4C0-2000C58D-5A0D8212

            //через HTML
            //http://wikimapia.org/61941/ru

            string baseURL = "http://api.wikimapia.org/?function=place.getbyid&data_blocks=main,photos,comments&key=";
            string key = Resources.wikimapia_key;
            string format = "json";
            string url = string.Format(baseURL + "{0}&format={1}&language={2}&id={3}", key, format, "ru", id);
            JObject json = SendJsonGetRequest(url);

            if (json["debug"] != null)
                throw new Exception(json["debug"]["message"].ToString());

            ExtInfo res = new ExtInfo();
            res.Description = json["description"] != null ? json["description"].ToString() : "";
            res.Title = json["title"].ToString();
            res.Link = "htp://wikimapia.org/" + id + "/" + json["language_iso"].ToString();
            res.Wikipedia = json["wikipedia"] != null ? json["wikipedia"].ToString() : "";
            foreach (JObject jo in json["tags"])
                res.Tags.Add(jo["title"].ToString());
            foreach (JObject jo in json["photos"])
            {
                ExtInfo.PhotoInfo np = new ExtInfo.PhotoInfo();
                np.TimeString = jo["time_str"].ToString();
                np.Url960 = jo["960_url"] != null ? jo["960_url"].ToString() : null;
                np.Url1280 = jo["1280_url"] != null ? jo["1280_url"].ToString() : null;
                np.UrlBig = jo["big_url"] != null ? jo["big_url"].ToString() : null;
                np.UrlFull = jo["full_url"] != null ? jo["full_url"].ToString() : null;
                np.UrlThumbnail = jo["thumbnail_url"] != null ? jo["thumbnail_url"].ToString() : "";
                np.UserName = jo["user_name"].ToString();
                res.Photos.Add(np);
            }
            foreach (JObject jo in json["comments"])
            {
                ExtInfo.CommentInfo nc = new ExtInfo.CommentInfo();
                nc.UserName = jo["name"].ToString();
                nc.Message = jo["message"].ToString();
                nc.Date = DateTime.Parse("01.01.1970") + TimeSpan.FromSeconds(int.Parse(jo["date"].ToString()));
                nc.UserPhoto = "http://wikimapia.org" + jo["user_photo"].ToString();
                res.Comments.Add(nc);
            }
            return res;
        }

        /// <summary>
        /// получение списка объектов в заданной области при заданном масштабе. Через API функцию BBOX
        /// http://wikimapia.org/api?action=how_to#oldbox
        /// </summary>
        /// <param name="area">область</param>
        /// <param name="perimeter">минимальный размер периметра объекта в метрах</param>
        List<VectorMapLayerObject> GetVectorMapLayerObjects2(RectLatLng area, double perimeter)
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
                        GMapPolygon pol = GetPolygon(polygon); //преобразуем координаты
                        string name = jt["name"].ToString();
                        string link = jt["url"].ToString();
                        string id = jt["id"].ToString();
                        pol.Name = name;
                        VectorMapLayerObject lo = new VectorMapLayerObject(pol) { ID = int.Parse(id), Link = link, Name = name, LayerProvider = VectorMapLayerProviders.Wikimapia };
                        res.Add(lo);
                    }

                    //проверяем длину периметра. Если периметр последнего объекта меньше заданного, то выходим из цикла
                    double per = GetPerimeter(res[res.Count - 1].Geometry);
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
        /// расчёт периметра полигона в метрах
        /// </summary>
        /// <param name="geometry">полигон</param>
        /// <returns></returns>
        private double GetPerimeter(GMapPolygon geometry)
        {
            double res = 0;
            for (int i = 1; i < geometry.Points.Count; i++)
                res += Vars.CurrentGeosystem.CalculateDistance(geometry.Points[i - 1], geometry.Points[i]);
            return res;
        }

        /// <summary>
        /// преобразование координат JSON в полиган карты
        /// </summary>
        /// <param name="polygon">координаты в JSON</param>
        /// <returns></returns>
        private GMapPolygon GetPolygon(JToken polygon)
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

    }
}
