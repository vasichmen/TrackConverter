using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.MapProviders;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Exceptions;

namespace TrackConverter.Lib.Classes.ProviderRecords
{

    /// <summary>
    /// структура поставщика карты для настроек и тега в кнопке выбора источника карты
    /// </summary>
    public class MapProviderRecord: BaseProviderRecord
    {

        /// <summary>
        /// поставщик
        /// </summary>
        public MapProviders Enum { get; set; }
        
        /// <summary>
        /// класс поставщика карты
        /// </summary>
        public MapProviderClasses MapProviderClass { get; set; }

        /// <summary>
        /// поставщик карты по ID
        /// </summary>
        /// <param name="id">ID поставщика</param>
        /// <returns></returns>
        public static MapProviderRecord FromID(int id)
        {
            foreach (MapProviderRecord m in Vars.Options.Map.AllMapProviders)
                if (m.ID == id)
                    return m;
            throw new ArgumentOutOfRangeException("Поставщика карты с id " + id + " не обнаружено!");
        }

        /// <summary>
        /// возвращает текстовое имя класса карт (для вывода пользователю)
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
        public static string GetMapProviderClassName(MapProviderClasses cl)
        {
            switch (cl)
            {
                case MapProviderClasses.Genshtab: return "Генштаб";
                case MapProviderClasses.Retromap: return "Исторические карты";
                case MapProviderClasses.Google: return "Google";
                case MapProviderClasses.OSM: return "OpenStreetMaps";
                case MapProviderClasses.Yandex: return "Яндекс";
                case MapProviderClasses.None: throw new Exception("У этого класса поставщика не имени!");
                default: throw new TrackConverterException("Этот класс поставщика не реализован!");
            }
        }

        /// <summary>
        /// возвращает класс карты по заданному перечислению
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static GMapProvider MapProviderToClass(MapProviders provider)
        {
            switch (provider)
            {
                case MapProviders.GoogleHybridMap:
                    return  GMapProviders.GoogleHybridMap;
                case MapProviders.GoogleMap:
                    return GMapProviders.GoogleMap;
                case MapProviders.GoogleSatelliteMap:
                    return  GMapProviders.GoogleSatelliteMap;
                case MapProviders.OpenCycleMap:
                    return  GMapProviders.OpenCycleMap;
                case MapProviders.YandexHybridMap:
                    return  Yandex.HybridMap.Instance;
                case MapProviders.YandexMap:
                    return  GMapProviders.YandexMap;
                case MapProviders.YandexSatelliteMap:
                    return  Yandex.SatelliteMap.Instance;
                case MapProviders.WikimapiaMap:
                    return  GMapProviders.WikiMapiaMap;
                case MapProviders.Genshtab_1km:
                    return  GenshtabGGC.KM1.Instance;
                case MapProviders.Genshtab_10km:
                    return  GenshtabGGC.KM10.Instance;
                case MapProviders.Genshtab_250m:
                    return  GenshtabGGC.M250.Instance;
                case MapProviders.Genshtab_500m:
                    return  GenshtabGGC.M500.Instance;
                case MapProviders.Genshtab_5km:
                    return  GenshtabGGC.KM5.Instance;
                case MapProviders.RKKA1941:
                    return  Retromap.RKKA1941.Instance;
                case MapProviders.GermanMoscowRegionMap1940:
                    return  Retromap.GermanMoscowRegionMap1940.Instance;
                case MapProviders.SoilMoscowRegionMap1985:
                    return  Retromap.SoilMoscowRegionMap1985.Instance;
                case MapProviders.USMoscowRegionMap1953:
                    return  Retromap.USMoscowRegionMap1953.Instance;
                default:
                    throw new NotSupportedException("Этот поставщик карты не поддерживается " + provider);
            }
        }
    }
}
