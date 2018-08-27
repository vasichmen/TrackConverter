using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Работа с сервисом карт OSM
    /// 
    /// https://wiki.openstreetmap.org/wiki/RU:API_v0.6
    /// </summary>
     class OSM :BaseConnection, IRastrMapLayerProvider
    {
        /// <summary>
        /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
        /// </summary>
        /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
        /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
        public OSM(string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

        /// <summary>
        /// максимальное число попыток подключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(100);
            }
        }

        #region IRastrMapLayerProvider

        /// <summary>
        /// получить тайл GPS треков по тайловым координатам
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z">масштаб</param>
        /// <returns></returns>
        public Image GetRastrTile(long x, long y, int z)
        {
            //любой из этих серверов:
            //https://a.gps-tile.openstreetmap.org/lines/13/4954/2570.png
            //https://b.gps-tile.openstreetmap.org/lines/13/4954/2570.png
            //https://c.gps-tile.openstreetmap.org/lines/13/4954/2570.png

            string url = @"https://a.gps-tile.openstreetmap.org/lines/{0}/{1}/{2}.png";
            url = string.Format(url, z, x, y);
            Image res = GetImage(url);
            return res;
        }

        #endregion

    }
}
