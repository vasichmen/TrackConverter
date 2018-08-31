using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;
using System.Drawing;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// настройки карты
    /// </summary>
    public class Map
    {
        /// <summary>
        /// максимальный масштаб карты
        /// </summary>
        public readonly int MaximalZoom = 21;

        /// <summary>
        /// создает новый экземпляр с нстройками по умолчанию
        /// </summary>
        public Map()
        {
            this.AccessMode = AccessMode.ServerAndCache;
            this.LastCenterPoint = new PointLatLng(55, 37);
            this.Zoom = 10;
            this.MapProvider = this.AllMapProviders[3];
            this.LayerProvider = this.AllLayerProviders[0];
            this.IsFormNavigatorShow = false;
            this.MapLanguange = LanguageType.Russian;
            this.MaxFullSearchNodes = 14;
            this.UseRouterInOptimal = true;
            this.UseBranchBoundsInPolarSearch = false;
            this.LastSelectedArea = RectLatLng.Empty;
            this.OptimalRouteMethod = OptimalMethodType.PolarSearch;
            this.RestoreRoutesWaypoints = true;
            this.LastSearchRequests = new List<string>();
            this.ShowAziMarkers = true;
        }

        /// <summary>
        /// способ доступа к данным
        /// </summary>
        public AccessMode AccessMode { get; set; }

        /// <summary>
        /// последняя центральная точка
        /// </summary>
        public PointLatLng LastCenterPoint { get; set; }

        /// <summary>
        /// приближение
        /// </summary>
        public double Zoom { get; set; }

        /// <summary>
        /// пoставщик карты
        /// </summary>
        public MapProviderRecord MapProvider { get; set; }

        /// <summary>
        /// поставщик слоя на карте
        /// </summary>
        public MapLayerProviderRecord LayerProvider { get; set; }

        /// <summary>
        /// Если истина, то при открытии карты будут восстанавливаться последние маршруты и точки
        /// </summary>
        public bool RestoreRoutesWaypoints { get; set; }

        /// <summary>
        /// список всех поддерживаемых поставщиков карт
        /// </summary>
        public List<MapProviderRecord> AllMapProviders
        {
            get
            {
                return new List<MapProviderRecord>() {
                        new MapProviderRecord(){ Enum = MapProviders.GoogleHybridMap, ID=0, Title = "Google.Гибрид",IconName="\\Images\\maps\\google_hibride.png", MapProviderClass = MapProviderClasses.Google},
                        new MapProviderRecord(){ Enum = MapProviders.GoogleMap, ID=1, Title = "Google.Схема" ,IconName="\\Images\\maps\\google_map.png", MapProviderClass = MapProviderClasses.Google},
                        new MapProviderRecord(){ Enum = MapProviders.GoogleSatelliteMap, ID=2, Title = "Google.Спутник" ,IconName="\\Images\\maps\\google_satellite.png", MapProviderClass = MapProviderClasses.Google},
                        new MapProviderRecord(){ Enum = MapProviders.OpenCycleMap, ID=3, Title = "OSM Cycle Map",IconName="\\Images\\maps\\osm_cycle.png" , MapProviderClass = MapProviderClasses.OSM},
                        new MapProviderRecord(){ Enum = MapProviders.YandexHybridMap, ID=4, Title = "Яндекс.Гибрид" ,IconName="\\Images\\maps\\yandex_hibride.png", MapProviderClass = MapProviderClasses.Yandex},
                        new MapProviderRecord(){ Enum = MapProviders.YandexMap, ID=5, Title = "Яндекс.Схема" ,IconName="\\Images\\maps\\yandex_map.png", MapProviderClass = MapProviderClasses.Yandex},
                        new MapProviderRecord(){ Enum = MapProviders.YandexSatelliteMap, ID=6, Title = "Яндекс.Спутник" ,IconName="\\Images\\maps\\yandex_satellite.png", MapProviderClass = MapProviderClasses.Yandex},
                        new MapProviderRecord(){ Enum = MapProviders.WikimapiaMap, ID=7, Title = "Карта Викимапии" ,IconName="\\Images\\maps\\wikimapia_map.png", MapProviderClass = MapProviderClasses.None},
                        new MapProviderRecord(){ Enum = MapProviders.Genshtab_1km, ID=8, Title = "ГГЦ 1км" ,IconName="\\Images\\maps\\ggc_1km.png", MapProviderClass = MapProviderClasses.Genshtab},
                        new MapProviderRecord(){ Enum = MapProviders.Genshtab_10km, ID=9, Title = "ГГЦ 10км" ,IconName="\\Images\\maps\\ggc_1km.png", MapProviderClass = MapProviderClasses.Genshtab},
                        new MapProviderRecord(){ Enum = MapProviders.Genshtab_250m, ID=10, Title = "ГГЦ 250м" ,IconName="\\Images\\maps\\ggc_1km.png", MapProviderClass = MapProviderClasses.Genshtab},
                        new MapProviderRecord(){ Enum = MapProviders.Genshtab_500m, ID=11, Title = "ГГЦ 500м" ,IconName="\\Images\\maps\\ggc_1km.png", MapProviderClass = MapProviderClasses.Genshtab},
                        new MapProviderRecord(){ Enum = MapProviders.Genshtab_5km, ID=12, Title = "ГГЦ 5км" ,IconName="\\Images\\maps\\ggc_1km.png", MapProviderClass = MapProviderClasses.Genshtab}
                        };
            }
            set { }
        }

        /// <summary>
        /// список всех поддерживаемых поставщиков слоёв
        /// </summary>
        public List<MapLayerProviderRecord> AllLayerProviders
        {
            get
            {
                return new List<MapLayerProviderRecord>() {
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.None, ID=0, Title = "Нет Слоя",IconName="\\Images\\layers\\none.png", MaxParallelPool = 1},
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.Wikimapia, ID=1, Title = "Слой карты Wikimapia",IconName="\\Images\\layers\\wikimapia.png", MaxParallelPool = 1},
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.YandexTraffic, ID=2, Title = "Яндекс.Пробки",IconName="\\Images\\layers\\yandex_traffic.png", MaxParallelPool = 1},
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.OSMGPSTracks, ID=3, Title = "GPS треки OSM",IconName="\\Images\\layers\\osm_gps_tracks.png", MaxParallelPool = 1 },
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.OSMRailways, ID=4, Title = "Железные дороги OSM",IconName="\\Images\\layers\\osm_railways.png", MaxParallelPool = 1},
                        new MapLayerProviderRecord(){ Enum = MapLayerProviders.OSMRoadSurface, ID=5, Title = "Дорожное покрытие OSM",IconName="\\Images\\layers\\osm_roadsurface.png", MaxParallelPool = 1}
                    };
            }
            set { }
        }


        /// <summary>
        /// было ли открыто окно навигации при выходе
        /// </summary>
        public bool IsFormNavigatorShow { get; set; }

        /// <summary>
        /// язык карты
        /// </summary>
        public LanguageType MapLanguange { get; set; }

        /// <summary>
        /// способ построения оптимального маршрута через путевые точки
        /// </summary>
        public OptimalMethodType OptimalRouteMethod { get; set; }

        /// <summary>
        /// максимальное количество узлов, которой можно перебрать полным перебором
        /// </summary>
        public int MaxFullSearchNodes { get; set; }

        /// <summary>
        /// использование маршрутизатора при построении оптимальных маршрутов
        /// </summary>
        public bool UseRouterInOptimal { get; set; }

        /// <summary>
        /// использовать метод ветвей и границ при построении маршрута через группы в полярном переборе
        /// </summary>
        public bool UseBranchBoundsInPolarSearch { get; set; }

        /// <summary>
        /// последняя выделенная область карты
        /// </summary>
        public RectLatLng LastSelectedArea { get; set; }

        /// <summary>
        /// Список последних запросов в поле поиска по карте
        /// </summary>
        public List<string> LastSearchRequests { get; set; }

        /// <summary>
        /// Если истина, то при редактировании не будут показываться маркеры азимутов
        /// </summary>
        public bool ShowAziMarkers { get; set; }
    }
}
