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
        /// создает новый экземпляр с нстройками по умолчанию
        /// </summary>
        public Map()
        {
            this.AccessMode = AccessMode.ServerAndCache;
            this.LastCenterPoint = new PointLatLng(55, 37);
            this.Zoom = 10;
            this.MapProvider = this.AllProviders[3];
            this.IsFormNavigatorShow = true;
            this.MapLanguange = LanguageType.Russian;
            this.WinSize = new Size(750,700);
            this.MaxFullSearchNodes = 14;
            this.UseRouterInOptimal = false;
            this.UseBranchBoundsInRecurEnum = false;
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
        /// пставщик карты
        /// </summary>
        public MapProviderRecord MapProvider { get; set; }

        /// <summary>
        /// Если истина, то при открытии карты будут восстанавливаться последние маршруты и точки
        /// </summary>
        public bool RestoreRoutesWaypoints { get; set; }

        /// <summary>
        /// список всех поддерживаемых поставщиков карт
        /// </summary>
        public List<MapProviderRecord> AllProviders
        {
            get
            {
                return new List<MapProviderRecord>() { 
                        new MapProviderRecord(){ Enum = MapProviders.GoogleHybridMap, ID=0, Title = "Google.Гибрид",IconName="\\Images\\maps\\google_hibride.png" },
                        new MapProviderRecord(){ Enum = MapProviders.GoogleMap, ID=1, Title = "Google.Схема" ,IconName="\\Images\\maps\\google_map.png"},
                        new MapProviderRecord(){ Enum = MapProviders.GoogleSatelliteMap, ID=3, Title = "Google.Спутник" ,IconName="\\Images\\maps\\google_satellite.png"},
                        new MapProviderRecord(){ Enum = MapProviders.OpenCycleMap, ID=4, Title = "OSM Cycle Map",IconName="\\Images\\maps\\osm_cycle.png" },
                        new MapProviderRecord(){ Enum = MapProviders.YandexHybridMap, ID=5, Title = "Яндекс.Гибрид" ,IconName="\\Images\\maps\\yandex_hibride.png"},
                        new MapProviderRecord(){ Enum = MapProviders.YandexMap, ID=6, Title = "Яндекс.Схема" ,IconName="\\Images\\maps\\yandex_map.png"},
                        new MapProviderRecord(){ Enum = MapProviders.YandexSatelliteMap, ID=7, Title = "Яндекс.Спутник" ,IconName="\\Images\\maps\\yandex_satellite.png"}
                        };
            }
            set { }
        }

       


        /// <summary>
        /// было ли открыто окно навигации
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
        /// размеры окна
        /// </summary>
        public Size WinSize { get; set; }

        /// <summary>
        /// максимальное количество узлов, которой можно перебрать полным перебором
        /// </summary>
        public int MaxFullSearchNodes { get; set; }

        /// <summary>
        /// использование маршрутизатора при построении оптимальных маршрутов
        /// </summary>
        public bool UseRouterInOptimal { get; set; }

        /// <summary>
        /// использовать метод ветвей и границ при построении маршрута через группы в рекусивном полном переборе
        /// </summary>
        public bool UseBranchBoundsInRecurEnum { get; set; }
    }
}
