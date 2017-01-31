using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Classes {
    /// <summary>
    /// тег маркера на карте
    /// </summary>
    public struct MarkerTag {

        /// <summary>
        /// информация о точке
        /// </summary>
        public TrackPoint Info { get; set; }

        /// <summary>
        /// тип маркера
        /// </summary>
        public MarkerTypes Type { get; set; }

        /// <summary>
        /// Тег
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// тип точки при прокладке маршрута
        /// </summary>
        public PathingType PathingType { get; set; }
    }
}
