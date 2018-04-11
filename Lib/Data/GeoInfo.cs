using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.ETOPO;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// поставщик информации 
    /// </summary>
    public class GeoInfo
    {
        /// <summary>
        /// База данных ETOPO
        /// </summary>
        public static ETOPOProvider ETOPOProvider { get; set; }

        /// <summary>
        /// поставщик геокодера
        /// </summary>
        private GeoInfoProvider provider;

        /// <summary>
        /// поставщик информации о высотах
        /// </summary>
        private IGeoInfoProvider geoinfo;

        /// <summary>
        /// создает новый экземпляр с заданным источником геоданных
        /// </summary>
        /// <param name="provider"></param>
        public GeoInfo(GeoInfoProvider provider)
        {
            this.provider = provider;
            switch (provider)
            {
                case GeoInfoProvider.ETOPO:
                    if (ETOPOProvider == null)
                        if (Vars.TaskLoadingETOPO.Status != TaskStatus.Running)
                            Vars.TaskLoadingETOPO.Start();
                    Vars.TaskLoadingETOPO.Wait();
                    if (ETOPOProvider == null)
                        throw new ApplicationException("Ошибка при загрузке базы данных ETOPO из директории " + Vars.Options.DataSources.ETOPODBFolder);
                    break;
                case GeoInfoProvider.Google:
                    geoinfo = new Google();
                    break;
                case GeoInfoProvider.GTOPO30:
                    geoinfo = new GTOPO30();
                    break;
                default: throw new Exception("Неизвестный поставщик ");
            }
        }

        /// <summary>
        /// возвращает высоту по заданной точке в метрах
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            if (provider == GeoInfoProvider.ETOPO)
                return ETOPOProvider.GetElevation(coordinate);

            double res = double.NaN;
            if (Vars.Options.DataSources.UseGeocoderCache)
                res = Vars.dataCache.GetElevation(coordinate);
            if (double.IsNaN(res))
            {
                double elev = geoinfo.GetElevation(coordinate);
                Vars.dataCache.Put(coordinate, elev);
                return elev;
            }
            else return res;
        }

        /// <summary>
        ///  возвращает высоту по заданной точке в метрах
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <returns></returns>
        public double GetElevation(Coordinate.CoordinateRecord lat, Coordinate.CoordinateRecord lon)
        { return GetElevation(lat, lon); }

        /// <summary>
        /// создает новый трек с высотами каждой точки в метрах
        /// </summary>
        /// <param name="track">трек</param>
        /// <param name="callback">действие, выполняемое при обработке точек</param>
        /// <returns>трек с высотами точек</returns>
        public BaseTrack GetElevation(BaseTrack track, Action<string> callback = null)
        {
            //если путешествие, то обрабатываем части
            if (track is TripRouteFile)
            {
                TripRouteFile trip = track as TripRouteFile;
                trip.Waypoints = (TrackFile)GetElevation(trip.Waypoints, callback);

                //дни обрабатываются через копирование для того, чтобы вычислялся TotalTrack
                TrackFileList days = trip.DaysRoutes.Clone();
                trip.DaysRoutes.Clear();
                for (int i = 0; i < days.Count; i++)
                    trip.AddDay((TrackFile)GetElevation(days[i], callback));
                return trip;
            }
            else //если это маршрут
            {
                //еси провайдер поддерживает множетсвенные запроссы высот
                if (provider == GeoInfoProvider.Google)
                    return ((Google)geoinfo).GetElevations(track, callback);

                //если приходится работать с каждой точкой отдельно
                TrackFile res = new TrackFile();
                res.Name = track.Name;
                res.FilePath = track.FilePath;
                res.Description = track.Description;
                res.Color = track.Color;
                double all = track.Count;
                double c = 0;
                foreach (TrackPoint tp in track)
                {
                    if (callback != null)
                        callback.Invoke("Получение высот точек маршрута " + track.Name + ", завершено " + (c / all * 100d).ToString("0.0") + "%");
                    if (double.IsNaN(tp.MetrAltitude))
                        tp.MetrAltitude = GetElevation(tp.Coordinates);
                    res.Add(tp);
                    c++;
                }
                return res;
            }
        }

        /// <summary>
        /// истина, если база данных ETOPO установлена
        /// </summary>
        public static bool IsETOPOReady
        {
            get
            {
                return ETOPOProvider.DatabaseInstalled(Vars.Options.DataSources.ETOPODBFolder);
            }
        }
    }
}
