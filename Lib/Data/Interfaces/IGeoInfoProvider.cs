using GMap.NET;
using System;
using System.Collections.Generic;
using System.Threading;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;
namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик данных о высоте точек
    /// </summary>
      interface IGeoInfoProvider
    {
        /// <summary>
        /// если истина, то это локальный источник данных
        /// </summary>
        bool isLocal { get; }

        /// <summary>
        /// возвращает высоту над уровнем моря в метрах
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        double GetElevation(Coordinate coordinate);
    }
}
