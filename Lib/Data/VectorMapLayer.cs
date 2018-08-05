using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// поставщик векторного слоя для карты
    /// </summary>
    public class VectorMapLayer
    {
        /// <summary>
        /// соединение с поставщиком слоя
        /// </summary>
        private IVectorMapLayerProvider layer;


        /// <summary>
        /// создаёт новый экземпляр поставщика слоя
        /// </summary>
        /// <param name="provider">сервис - источник слоя карты</param>
        public VectorMapLayer(VectorMapLayerProviders provider)
        {
            switch (provider)
            {
                case VectorMapLayerProviders.Wikimapia:
                    layer = new Wikimapia();
                    break;
                case VectorMapLayerProviders.None:
                    throw new Exception("Нельзя создать поставщика слоя None");
            }
        }

        /// <summary>
        /// получить список объектов слоя по заданной области карты и минимальному периметру объекта
        /// </summary>
        /// <param name="area">область карты</param>
        /// <param name="perimeter">минимальный размер периметра объекта в метрах</param>
        /// <param name="zoom">масштаб карты для которого выбираются объекты</param>
        /// <returns></returns>
        public List<VectorMapLayerObject> GetObjects(RectLatLng area, double perimeter, int zoom)
        {
            //if (Vars.Options.DataSources.UseMapLayerCache)
            //{
            //    List<VectorMapLayerObject> objs = Vars.dataCache.GetVectorMapLayerObjects(area, perimeter);
            //    if (objs != null)
            //        return objs;
            //    else
            //    {
            //        List<VectorMapLayerObject> objects = layer.GetVectorMapLayerObjects(area, perimeter);
            //        Vars.dataCache.AddMapLayerObjects(objects, zoom);
            //        return objects;
            //    }
            //}
            //else
            return layer.GetObjects(area, perimeter);
        }
    }
}
