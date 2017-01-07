using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// настройки сервисов (прокладка маршрутов, сокращение ссылок)
    /// </summary>
    public class Services
    {
        /// <summary>
        /// создает новый экземпляр настроек сервисов
        /// </summary>
        public Services()
        {
            this.ChangePathedRoute = false;
            this.PathRouteMode = PathRouteMode.Walk;
            this.LinkShorterProvider = LinkShorterProvider.Clck;
            this.PathRouteProvider = PathRouteProvider.Yandex;
            this.UseFSCacheForCreatingRoutes = true;
        }

        /// <summary>
        /// поставщик прокладки маршрутов
        /// </summary>
        public PathRouteProvider PathRouteProvider { get; set; }

        /// <summary>
        /// поставщик сокращения ссылок
        /// </summary>
        public LinkShorterProvider LinkShorterProvider { get; set; }

        /// <summary>
        /// если истина, то после прокладки маршрута с пмощью сервисов проложенный маршрут будет открыт для редактирования
        /// </summary>
        public bool ChangePathedRoute { get; set; }

        /// <summary>
        /// тип прокладываемого маршрута
        /// </summary>
        public PathRouteMode PathRouteMode { get; set; }

        /// <summary>
        /// если истина, то при построении маршрутов между точками информация JSON будет временно харнить в файлах , 
        /// а декодироваться только при необходимости
        /// </summary>
        public bool UseFSCacheForCreatingRoutes { get; set; }
    }
}
