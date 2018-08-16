using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// основные методы кэша векторных слоёв
    /// </summary>
    public interface IVectorMapLayerCache
    {
        /// <summary>
        /// добавить объекты в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <param name="objects">список объектов</param>
        /// <returns></returns>
        bool PutVectorMapLayerObjects(PointLatLng locationMiddle, int zoom, List<VectorMapLayerObject> objects);

        /// <summary>
        /// получить объекты в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        List<VectorMapLayerObject> GetVectorMapLayerObjects(PointLatLng locationMiddle, int zoom);

        /// <summary>
        /// проверить существование объектов в заданной области при заданном масштабе
        /// </summary>
        /// <param name="locationMiddle">центр области</param>
        /// <param name="zoom">масштаб</param>
        /// <returns></returns>
        bool ContainsVectorMapLayerObjects(PointLatLng locationMiddle, int zoom);
    }
}
