using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// работа с растровыми слоями карты
    /// </summary>
    public class RastrMapLayer : IRastrMapLayerProvider
    {
        private MapLayerProviders provider;
        private IRastrMapLayerProvider engine;

        /// <summary>
        /// создаёт новый обхект связи с указанным источником данных
        /// </summary>
        /// <param name="provider">провайдер слоя</param>
        public RastrMapLayer(MapLayerProviders provider)
        {
            this.provider = provider;
            switch (provider)
            {
                case MapLayerProviders.YandexTraffic:
                    engine = new Yandex(null);
                    break;
                case MapLayerProviders.OSMGPSTracks:
                    engine = new OSM.GpsTracks(null);
                    break;
                case MapLayerProviders.OSMRailways:
                    engine = new OSM.Railways(null);
                    break;
                case MapLayerProviders.OSMRoadSurface:
                    engine = new OSM.RoadSurface(null);
                    break;
                case MapLayerProviders.None:
                    throw new Exception("Нельзя создать поставщика слоя None");
                default: throw new Exception("Данный поставщик слоя нельзя использовать в этом классе");
            }
        }

        /// <summary>
        /// получить картинку слоя по заданным тайловым координатам
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Image GetRastrTile(long x, long y, int z)
        {
            return engine.GetRastrTile(x, y, z);
        }
    }
}
