using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.OS;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// Взаимодействие со всеми видами кэша. (БД, файловая система, память)
    /// </summary>
    public class Cache : IGeoсoderProvider, IGeoInfoProvider, IDisposable, IGeocoderCache, IGeoInfoCache, IImagesCache, ILayerObjectExtInfoCache
    {
        /// <summary>
        /// папка для хранения БД
        /// </summary>
        string sqliteDir;

        /// <summary>
        /// папка хранения фотографий
        /// </summary>
        string imagesDir;

        /// <summary>
        /// взаимодействие с БД
        /// </summary>
        SQLiteCache sqlite;

        /// <summary>
        /// взаимодействие с файловой системой
        /// </summary>
        FileSystemCache fs;

        /// <summary>
        /// кэш в оперативной памяти
        /// </summary>
        MemoryCache vectorMapLayerObjects;

        /// <summary>
        /// создает кэш в заданной базовой папке
        /// </summary>
        /// <param name="baseDir">путь к папке кэша</param>
        public Cache(string baseDir)
        {
            sqliteDir = baseDir + "\\geocoder";
            imagesDir = baseDir + "\\images";

            sqlite = new SQLiteCache(sqliteDir);
            fs = new FileSystemCache(imagesDir);
            vectorMapLayerObjects = new MemoryCache();
        }


        #region реализации интерфейсов TrackConverter

        /// <summary>
        /// если истина, то это локальный источник данных
        /// </summary>
        public bool isLocal
        {
            get
            {
                return ((IGeoInfoProvider)sqlite).isLocal;
            }
        }

        #region IGeocoderCache

        /// <summary>
        /// получить адрес по координатам
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            return ((IGeoсoderProvider)sqlite).GetAddress(coordinate);
        }

        /// <summary>
        /// получить координаты адреса
        /// </summary>
        /// <param name="address">строка адреса</param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            return ((IGeoсoderProvider)sqlite).GetCoordinate(address);
        }

        /// <summary>
        /// очистить данные геокодера
        /// </summary>
        public void ClearGeocoder()
        {
            ((IGeocoderCache)sqlite).ClearGeocoder();
        }

        /// <summary>
        /// добавить адрес в кэш
        /// </summary>
        /// <param name="Coordinate"></param>
        /// <param name="Address"></param>
        public void PutGeocoder(Coordinate Coordinate, string Address)
        {
            ((IGeocoderCache)sqlite).PutGeocoder(Coordinate, Address);
        }

        #endregion

        #region IGeoInfoCache

        /// <summary>
        /// получить временную зону по координатам места
        /// </summary>
        /// <param name="coordinate">координаты места</param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            return ((IGeoсoderProvider)sqlite).GetTimeZone(coordinate);
        }

        /// <summary>
        /// получить высоту по координатам
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            return ((IGeoInfoProvider)sqlite).GetElevation(coordinate);
        }

        /// <summary>
        /// очистить данные высот
        /// </summary>
        public void ClearAltitudes()
        {
            ((IGeoInfoCache)sqlite).ClearAltitudes();
        }

        /// <summary>
        /// добавить временную зону точки
        /// </summary>
        /// <param name="coordinates">координаты точки</param>
        /// <param name="tzi">зона</param>
        public void PutGeoInfo(Coordinate coordinates, TimeZoneInfo tzi)
        {
            ((IGeocoderCache)sqlite).PutGeoInfo(coordinates, tzi);
        }

        /// <summary>
        /// добавить высоту точки
        /// </summary>
        /// <param name="Coordinate">координаты </param>
        /// <param name="Altitude">высота в метрах</param>
        public void PutGeoInfo(Coordinate Coordinate, double Altitude)
        {
            ((IGeoInfoCache)sqlite).PutGeoInfo(Coordinate, Altitude);
        }

        /// <summary>
        /// добавитьсписок высот 
        /// </summary>
        /// <param name="track">координаты точек</param>
        /// <param name="els">список высот в метрах</param>
        /// <param name="callback">функция - вывод информации о процессе</param>
        public void PutGeoInfo(BaseTrack track, List<double> els, Action<string> callback = null)
        {
            ((IGeoInfoCache)sqlite).PutGeoInfo(track, els, callback);
        }

        /// <summary>
        /// попытка заполнить список точек координатами. Если удалось заполнить все точки, то возвращается истина
        /// </summary>
        /// <param name="track">список точек для заполнения</param>
        /// <returns></returns>
        public bool TryGetElevations(ref BaseTrack track)
        {
            return ((IGeoInfoCache)sqlite).TryGetElevations(ref track);
        }

        #endregion

        #region IImagesCache

        /// <summary>
        /// проверка существования картинки в кэше
        /// </summary>
        /// <param name="url">url картинки, по которому она была добавлена в кэш</param>
        /// <returns></returns>
        public bool CheckImage(string url)
        {
            return ((IImagesCache)fs).CheckImage(url);
        }

        /// <summary>
        /// получить картинку из кэша
        /// </summary>
        /// <param name="url">url картинки, по которому она была добавлена в кэш</param>
        /// <returns></returns>
        public Image GetImage(string url)
        {
            return ((IImagesCache)fs).GetImage(url);
        }

        /// <summary>
        /// добавить картинку в кэш
        /// </summary>
        /// <param name="url">url картинки</param>
        /// <param name="data">картинка</param>
        /// <returns></returns>
        public bool PutImage(string url, Image data)
        {
            return ((IImagesCache)fs).PutImage(url, data);
        }

        #endregion

        #region IVectorMapLayerObjectCache

        /// <summary>
        /// проверка существования информации об объекте в кэше
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public bool ContainsLayerObjectExtInfo(int id)
        {
            return vectorMapLayerObjects.ContainsObject(id.ToString());
        }

        /// <summary>
        /// получить объект из кэша
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public Wikimapia.ExtInfo GetLayerObjectExtInfo(int id)
        {
            return (Wikimapia.ExtInfo)vectorMapLayerObjects.GetObject(id.ToString());
        }

        /// <summary>
        /// добавить информацию об объекте в кэш
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool PutLayerObjectExtInfo(Wikimapia.ExtInfo obj)
        {
            return vectorMapLayerObjects.PutObject(obj.ID.ToString(), obj);
        }

        #endregion


        #endregion

        #region реализации интерфейсов .NET

        /// <summary>
        /// освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            sqlite.Dispose();
        }



        #endregion


    }
}
