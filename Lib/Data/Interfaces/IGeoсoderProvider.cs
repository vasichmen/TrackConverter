using GMap.NET;
using System;
using System.Collections.Generic;
using TrackConverter.Lib.Classes;
namespace TrackConverter.Lib.Data.Interfaces
{
   /// <summary>
   /// взаимодействие с кэшем геокодера
   /// </summary>
    interface IGeoсoderProvider
    {
        /// <summary>
        /// получить адрес по координатам
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        string GetAddress(Coordinate coordinate);
    
        /// <summary>
        /// получить координаты по адресу
        /// </summary>
        /// <param name="address">адрес точки</param>
        /// <returns></returns>
        Coordinate GetCoordinate(string address);

        /// <summary>
        /// получить временную зону по координатам точки
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        TimeZoneInfo GetTimeZone(Coordinate coordinate);
    }
}
