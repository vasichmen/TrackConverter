using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Res.Properties;

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
        public VectorMapLayer(MapLayerProviders provider)
        {
            switch (provider)
            {
                case MapLayerProviders.Wikimapia:
                    layer = new Wikimapia(Application.StartupPath + Resources.cache_directory + "\\http_cache\\wikimapia");
                    break;
                case MapLayerProviders.None:
                    throw new Exception("Нельзя создать поставщика слоя None");
                default: throw new Exception("Данный поставщик слоя нельзя использовать в этом классе");
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
            if (Vars.Options.DataSources.UseMapLayerCache)
            {
                List<VectorMapLayerObject> objs = Vars.dataCache.GetVectorMapLayerObjects(area.LocationMiddle, zoom);
                if (objs != null)
                    return objs;
                else
                {
                    List<VectorMapLayerObject> objects = layer.GetObjects(area, perimeter);
                    Vars.dataCache.PutVectorMapLayerObjects(area.LocationMiddle, zoom, objects);
                    return objects;
                }
            }
            else
                return layer.GetObjects(area, perimeter);
        }
    }
}
