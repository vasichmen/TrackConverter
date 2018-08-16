using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Classes;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace TrackConverter.Lib.Data.Providers.Local.OS
{
    /// <summary>
    /// кэш загруженных объектов в оперативной памяти по масштабам
    /// </summary>
    class ObjectsMemoryCache : IVectorMapLayerCache
    {
        /// <summary>
        /// адрес папки для сохранения
        /// </summary>
        string dname;

        /// <summary>
        /// максимальное количество масштабов карты
        /// </summary>
        const int zooms = 30;

        /// <summary>
        /// массив по масштабам карты (0..zooms-1)
        /// </summary>
        MemoryCache[] c;

        /// <summary>
        /// создвёт новый объект кэша в памяти
        /// <param name="dname">адрес папки кэша</param>
        /// <param name="duration">продолжительность хранения кэша</param>
        /// </summary>
        public ObjectsMemoryCache(string dname, TimeSpan duration)
        {
            this.dname = dname;
                c = new MemoryCache[zooms];

            removeOlderThan(DateTime.Now - duration);
        }

        /// <summary>
        /// удалить все записи, созданные ранее, чем указанная дата
        /// </summary>
        /// <param name="dateTime">дата, записи, созданные ранее которой, удаляются из кэша</param>
        private void removeOlderThan(DateTime dateTime)
        {
            for (int i = 0; i < zooms; i++)
                if (c[i] != null)
                    c[i].RemoveOlderThan(dateTime);
        }

        /// <summary>
        /// добавить объекты в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <param name="objects">список объектов </param>
        /// <returns></returns>
        public bool PutVectorMapLayerObjects(PointLatLng locationMiddle, int zoom, List<VectorMapLayerObject> objects)
        {
            if (c[zoom] == null)
                c[zoom] = new MemoryCache();
            string id = getId(locationMiddle, zoom);
            return c[zoom].PutObject(id, objects);
        }

        /// <summary>
        /// получить объекты в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        public List<VectorMapLayerObject> GetVectorMapLayerObjects(PointLatLng locationMiddle, int zoom)
        {
            if (c[zoom] == null)
                return null;
            string id = getId(locationMiddle, zoom);
            return (List<VectorMapLayerObject>)c[zoom].GetObject(id);
        }

        /// <summary>
        /// проверить существование объектов в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        public bool ContainsVectorMapLayerObjects(PointLatLng locationMiddle, int zoom)
        {
            if (c[zoom] == null)
                return false;
            string id = getId(locationMiddle, zoom);
            return c[zoom].ContainsObject(id);
        }

        /// <summary>
        /// получит ID области на основе центра и масштаба
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        private string getId(PointLatLng locationMiddle, int zoom)
        {
            string s = locationMiddle.Lat.ToString("00.00000") + locationMiddle.Lng.ToString("00.00000") + zoom.ToString("00");

            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(s);

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
        /// сохранить объекты на диск
        /// </summary>
        internal void Close()
        {
            //saveDirectory(dname);
        }

    }
}
