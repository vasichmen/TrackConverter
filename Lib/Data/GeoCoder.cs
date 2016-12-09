using GMap.NET;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
        private GeoCoderProvider provider;

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
            if (Vars.dataCache == null)
                Vars.dataCache = new SQLiteCache(Application.StartupPath + Resources.cache_directory + "\\geocoder");
            switch (provider)
            {
                case GeoCoderProvider.Yandex:
                    coder = new Yandex();
                    break;
                case GeoCoderProvider.Google:
                    coder = new Google();
                    break;
                case GeoCoderProvider.Nominatim:
                    coder = new Nominatim();
                    break;
                default: throw new Exception("Неизвестный поставщик геокодера");
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
                Vars.dataCache.Put(coordinate, adrI);
                return adrI;
            }
            else return adr;
        }

        /// <summary>
        /// получить адрес по точке на карте
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <returns></returns>
        public string GetAddress(Coordinate.CoordinateRecord lat, Coordinate.CoordinateRecord lon)
        { return GetAddress(new Coordinate(lat, lon)); }

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
                Vars.dataCache.Put(coord, address);
                return coord;
            }
            else return res;


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
            return new Yandex().GetAddresses(query);
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
    }
}
