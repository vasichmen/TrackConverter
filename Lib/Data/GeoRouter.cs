using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// класс прокладки маршрутов
    /// </summary>
    public class GeoRouter
    {
        /// <summary>
        /// поставщик сервиса прокладки маршрутов
        /// </summary>
        private PathRouteProvider pathRouteProvider;
        private IRouterProvider router;

        /// <summary>
        /// создает новый экземпляр с указанным поставщиком сервиса прокладки маршрута
        /// </summary>
        /// <param name="pathRouteProvider">поставщик сервиса</param>
        public GeoRouter(PathRouteProvider pathRouteProvider)
        {
            this.pathRouteProvider = pathRouteProvider;
            switch (pathRouteProvider)
            {
                case PathRouteProvider.Google:
                    router = new Google(null);
                    break;
                case PathRouteProvider.Yandex:
                    router = new Yandex(null);
                    break;
                default:
                    throw new ArgumentException("Неизвестный поставщик сервиса прокладки маршрутов");
            }
        }

        /// <summary>
        /// проложить маршрут между заданными координатами
        /// </summary>
        /// <param name="from">откуда</param>
        /// <param name="to">куда</param>
        /// <param name="waypoints">промежуточные точки</param>
        /// <returns></returns>
        public TrackFile CreateRoute(Coordinate from, Coordinate to, TrackFile waypoints)
        {
            return this.router.CreateRoute(from, to, waypoints);
        }

        /// <summary>
        /// возвращает маршрут из файлового кэша (вызов при построенни графа маршрутов)
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        internal TrackFile GetRouteFromFSCache(int i, int j)
        {
            return router.GetRouteFromFSCache(i, j);
        }

        /// <summary>
        /// построить все пути между точками. В ячейки с одинаковыми индексами записывается null
        /// </summary>
        /// <param name="points">точки, между которыми надо построить маршруты</param>
        /// <param name="callback">действие, выполняемое во время обработки</param>
        /// <returns></returns>
        public List<List<TrackFile>> CreateRoutes(BaseTrack points, Action<string> callback)
        {
            return this.router.CreateRoutes(points, callback);
        }

    }
}
