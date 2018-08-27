using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// общий класс для работы с сервисами OSM 
    /// https://wiki.openstreetmap.org/wiki/RU:API_v0.6
    /// </summary>
    static class OSM
    {
        /// <summary>
        /// Работа с GPS треками OSM
        /// </summary>
        public class GpsTracks : BaseConnection, IRastrMapLayerProvider
        {
            /// <summary>
            /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
            /// </summary>
            /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
            /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
            public GpsTracks(string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

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

                //TODO: сделать случайный выбор сервера
                string url = @"https://a.gps-tile.openstreetmap.org/lines/{0}/{1}/{2}.png";
                url = string.Format(url, z, x, y);
                Image res = GetImage(url);
                return res;
            }

            #endregion

        }

        /// <summary>
        /// Работа с GPS треками OSM
        /// </summary>
        public class RoadSurface : BaseConnection, IRastrMapLayerProvider
        {
            /// <summary>
            /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
            /// </summary>
            /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
            /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
            public RoadSurface(string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

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
            /// получить тайл дооржного покрытия по тайловым координатам
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z">масштаб</param>
            /// <returns></returns>
            public Image GetRastrTile(long x, long y, int z)
            {
                //любой из этих серверов:
                //http://t0.beta.itoworld.com/25/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t1.beta.itoworld.com/25/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t2.beta.itoworld.com/25/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t3.beta.itoworld.com/25/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png

                //TODO: сделать случайный выбор сервера
                string url = @"http://t0.beta.itoworld.com/25/136bdef4afc8d6e92586bc38e7b843c1/{0}/{1}/{2}.png";
                url = string.Format(url, z, x, y);
                Image res = GetImage(url);
                return res;
            }

            #endregion

        }

        /// <summary>
        /// Работа с GPS треками OSM
        /// </summary>
        public class Railways : BaseConnection, IRastrMapLayerProvider
        {
            /// <summary>
            /// Создаёт новый объект связи с сервисом с заданной папкой кэша запросов и временем хранения кэша
            /// </summary>
            /// <param name="cacheDirectory">папка с кэшем или null, если не надо использовать кэш</param>
            /// <param name="duration">время хранения кэша в часах. По умолчанию - неделя</param>
            public Railways (string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

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
            /// получить тайл железных дорог по тайловым координатам
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z">масштаб</param>
            /// <returns></returns>
            public Image GetRastrTile(long x, long y, int z)
            {
                //любой из этих серверов:
                //http://t0.beta.itoworld.com/15/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t1.beta.itoworld.com/15/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t2.beta.itoworld.com/15/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png
                //http://t3.beta.itoworld.com/15/136bdef4afc8d6e92586bc38e7b843c1/13/4954/2570.png

                //TODO: сделать случайный выбор сервера
                string url = @"http://t0.beta.itoworld.com/15/136bdef4afc8d6e92586bc38e7b843c1/{0}/{1}/{2}.png";
                url = string.Format(url, z, x, y);
                Image res = GetImage(url);
                return res;
            }

            #endregion

        }
    }
}
