using System;
using System.Collections.Generic;
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
    }
}
