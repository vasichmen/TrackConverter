using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{

    /// <summary>
    /// карта ребер начальных и конечных точек
    /// </summary>
    class MapCell
    {
        /// <summary>
        /// создает ячейку с указанными данными
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        public MapCell(TrackPoint From, TrackPoint To)
        {
            this.From = From;
            this.To = To;
        }

        /// <summary>
        /// начальная точка ребра
        /// </summary>
        public TrackPoint From;

        /// <summary>
        /// конечная точка ребра
        /// </summary>
        public TrackPoint To;
    }
}
