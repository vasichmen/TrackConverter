using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик векторного слоя для карты
    /// </summary>
    public interface IVectorMapLayerProvider
    {
        /// <summary>
        /// получить объекты в заданной области 
        /// </summary>
        /// <param name="area">область</param>
        /// <param name="perimeter">минимальный размер периметра объекта в метрах</param>
        /// <returns></returns>
        List<VectorMapLayerObject> GetObjects(RectLatLng area, double perimeter);
    }
}
