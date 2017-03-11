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
        /// <summary>
        /// Построить маршрут от заданной точки from до заданной точки to через точки waypoints
        /// </summary>
        /// <param name="from">начальная точка</param>
        /// <param name="to">конечная точка</param>
        /// <param name="waypoints">промежуточные точки</param>
        /// <returns></returns>
        TrackFile CreateRoute(Coordinate from, Coordinate to, TrackFile waypoints);

        /// <summary>
        /// построить маршруты между всеми зданными точками. 
        /// Возвращает матрицу, в которой элемент ij содержит маршрут из точки с номером i в точку с номеро j. 
        /// Элементы на главной диагонали равны null
        /// </summary>
        /// <param name="points">точки, между которыми надо построить маршруты</param>
        /// <param name="callback">дейстие, выполняемое при построении</param>
        /// <returns></returns>
        List<List<TrackFile>> CreateRoutes(BaseTrack points, Action<string> callback);

        /// <summary>
        /// возвращает маршрут из файлового кэша (вызов при построенни графа маршрутов)
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        TrackFile GetRouteFromFSCache(int i, int j);
    }
}
