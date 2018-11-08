using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Exceptions;

namespace TrackConverter.Lib.Classes.ProviderRecords
{
    /// <summary>
    /// информация о поставщике слоя на карте 
    /// </summary>
    public class MapLayerProviderRecord:BaseProviderRecord
    {
        /// <summary>
        /// значение перечисления
        /// </summary>
        public MapLayerProviders Enum { get; set; }

        /// <summary>
        /// класс поставщика слоя карты
        /// </summary>
        public MapLayerProvidersClasses MapLayerProviderClass { get; set; }

        /// <summary>
        /// поставщик слоя по ID
        /// </summary>
        /// <param name="id">ID поставщика</param>
        /// <returns></returns>
        public static MapLayerProviderRecord FromID(int id)
        {
            foreach (MapLayerProviderRecord m in Vars.Options.Map.AllLayerProviders)
                if (m.ID == id)
                    return m;
            throw new ArgumentOutOfRangeException("Поставщика слоя с id " + id + " не обнаружено!");
        }

        /// <summary>
        /// получить название слоя по перечислению
        /// </summary>
        /// <param name="mapLayerProviderClass">слой карты</param>
        /// <returns></returns>
        public static string GetMapLayerProviderClassName(MapLayerProvidersClasses mapLayerProviderClass)
        {
            switch (mapLayerProviderClass)
            {
                case MapLayerProvidersClasses.None: return "Нет слоя";
                case MapLayerProvidersClasses.OSM: return "OSM";
                default: throw new TrackConverterException("Этот класс поставщика слоя не реализован!");
            }
        }

        /// <summary>
        /// получить цвет границы объекта для указанно карты
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Color GetObjectBorderColor(MapProviders map)
        {
            switch (map)
            {
                case MapProviders.Genshtab_10km:
                case MapProviders.Genshtab_1km:
                case MapProviders.Genshtab_250m:
                case MapProviders.Genshtab_500m:
                case MapProviders.Genshtab_5km:
                case MapProviders.GermanMoscowRegionMap1940:
                    return Color.Red;
                case MapProviders.SoilMoscowRegionMap1985:
                case MapProviders.USMoscowRegionMap1953:
                case MapProviders.RKKA1941:
                    return Color.Black;
                case MapProviders.GoogleSatelliteMap:
                case MapProviders.YandexSatelliteMap:
                    return Color.LightGray;
                case MapProviders.OpenCycleMap:
                case MapProviders.GoogleMap:
                case MapProviders.YandexMap:
                    return Color.DarkGray;
                default: return Color.LightGray;
            }
        }
    }
}
