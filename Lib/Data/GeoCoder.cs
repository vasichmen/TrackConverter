using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// геокодер для работы с различными сервисами
    /// </summary>
    public class GeoCoder
    {
        /// <summary>
        /// поставщик геокодера
        /// </summary>
        private readonly GeoCoderProvider provider;

        /// <summary>
        /// геокодер
        /// </summary>
        private IGeoсoderProvider coder;

        /// <summary>
        /// создает новый экземпляр с заданным источником геоданных. 
        /// </summary>
        /// <param name="provider">для получения адреса только яндекс, для получения координат только гугл</param>
        public GeoCoder(GeoCoderProvider provider)
        {
            this.provider = provider;

            switch (provider)
            {
                case GeoCoderProvider.Yandex:
                    coder = new Yandex(Application.StartupPath + Resources.cache_directory + "\\http_cache\\yandex");
                    break;
                case GeoCoderProvider.Google:
                    coder = new Google(Application.StartupPath + Resources.cache_directory + "\\http_cache\\google");
                    break;
                case GeoCoderProvider.Nominatim:
                    coder = new Nominatim();
                    break;
                case GeoCoderProvider.Arcgis:
                    coder = new Arcgis(Application.StartupPath + Resources.cache_directory + "\\http_cache\\arcgis");
                    break;
                        
                default:
                    throw new Exception("Неизвестный поставщик геокодера");
            }
        }

        /// <summary>
        /// получить адрес по точке на карте
        /// </summary>
        /// <param name="point">координаты точки</param>
        /// <returns></returns>
        public string GetAddress(PointLatLng point)
        {
            return GetAddress(new Coordinate(point.Lat, point.Lng));
        }

        /// <summary>
        /// получить адрес по точке на карте
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            string adr = Vars.dataCache.GetAddress(coordinate);
            if (adr == null)
            {
                string adrI = coder.GetAddress(coordinate);
                Vars.dataCache.PutGeocoder(coordinate, adrI);
                return adrI;
            }
            else
                return adr;
        }
        
        /// <summary>
        /// получить адрес по точке на карте
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <returns></returns>
        public string GetAddress(double lat, double lon)
        { return GetAddress(new Coordinate(lat, lon)); }

        /// <summary>
        /// получить координаты адреса
        /// </summary>
        /// <param name="address">адрес</param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            if (address == "" || address == null)
                throw new ArgumentNullException();
            Coordinate res = Coordinate.Empty;
            if (Vars.Options.DataSources.UseGeocoderCache)
                res = Vars.dataCache.GetCoordinate(address);
            if (res.isEmpty)
            {
                Coordinate coord = coder.GetCoordinate(address);
                Vars.dataCache.PutGeocoder(coord, address);
                return coord;
            }
            else
                return res;


        }

        /// <summary>
        /// Получить все адреса по запросу. Используется только сервис Yandex
        /// </summary>
        /// <param name="query">начало запроса</param>
        /// <returns></returns>
        public Dictionary<string, Coordinate> GetAddresses(string query)
        {
            if (query == "" || query == null)
                return null;
            return new Yandex(Application.StartupPath + Resources.cache_directory + "\\http_cache\\yandex").GetAddresses(query);
        }

        /// <summary>
        /// получить список координат по заданным адресам
        /// </summary>
        /// <param name="AddressList">список адресов</param>
        /// <param name="callback">действие, выполняемое при обработке каждого элемента</param>
        /// <returns></returns>
        public TrackFile GetCoordinates(List<string> AddressList, Action<string> callback = null)
        {
            TrackFile res = new TrackFile();
            for (int i = 0; i < AddressList.Count; i++)
            {
                string adr = AddressList[i];
                try
                {
                    Coordinate cd = this.GetCoordinate(adr);
                    res.Add(new TrackPoint(cd) { Name = adr });
                }
                catch (ApplicationException) { }
                if (callback != null)
                    callback.Invoke("Обработка адреса: " + adr);
            }
            return res;
        }

        /// <summary>
        /// получить информацию о часовом поясе
        /// </summary>
        /// <param name="coordinates">координаты</param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinates)
        {
            TimeZoneInfo tz = Vars.dataCache.GetTimeZone(coordinates);
            if (tz == null)
            {
                TimeZoneInfo tzi = coder.GetTimeZone(coordinates);
                Vars.dataCache.PutGeoInfo(coordinates, tzi);
                return tzi;
            }
            else
                return tz;
        }

        /// <summary>
        /// записать в трек адреса точек
        /// </summary>
        /// <param name="tf">точки</param>
        /// <param name="callback">вывод результатов в строку</param>
        /// <returns></returns>
        public void GetAddresses(BaseTrack tf, Action<string> callback)
        {
            //если путешествие, то записываемтолько в путевые точки
            if (tf.GetType() == typeof(TripRouteFile))
            {
                GetAddresses((tf as TripRouteFile).Waypoints, callback);
                return;
            }

            //если список точек , то записываем адреса
            double all = tf.Count;
            for (int i = 0; i < tf.Count; i++)
            {
                string adr = coder.GetAddress(tf[i].Coordinates);
                tf[i].Description = adr + "\r\n" + tf[i].Description;
                if (callback != null)
                    callback.Invoke("Получение адресов точек маршрута " + tf.Name + ", завершено " + (i / all * 100d).ToString("0.0") + "%");
            }
        }
    }
}
