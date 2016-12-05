using System;
using System.Collections.Generic;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;
namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик прокладки маршрутов
    /// </summary>
    interface IRouterProvider
    {
        TrackFile CreateRoute(Coordinate from, Coordinate to, TrackFile waypoints);
        List<List<TrackFile>> CreateRoutes(TrackFile points, Action<string> callback);
    }
}
