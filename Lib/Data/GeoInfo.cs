using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.ETOPO2;
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
        /// База данных Etopo2
        /// </summary>
        public static ETOPO2Provider ETOPO2Provider { get; set; }

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
            if (Vars.dataCache == null)
                Vars.dataCache = new SQLiteCache(Application.StartupPath + Resources.cache_directory + "\\geocoder");

            switch (provider)
            {
                case GeoInfoProvider.ETOPO2:
                    if (ETOPO2Provider == null)
                        if (Vars.TaskLoadingETOPO2.Status != TaskStatus.Running)
                            Vars.TaskLoadingETOPO2.Start();
                        Vars.TaskLoadingETOPO2.Wait();
                    if (ETOPO2Provider == null)
                        throw new ApplicationException("Ошибка при загрузке базы данных ETOPO2 из директории " + Vars.Options.DataSources.ETOPO2DBFolder);
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
        /// возвращает высоту по заданной точке
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            if (provider == GeoInfoProvider.ETOPO2)
                return ETOPO2Provider.GetElevation(coordinate);
            double res = Vars.dataCache.GetElevation(coordinate);
            if (double.IsNaN(res))
            {
                double elev = geoinfo.GetElevation(coordinate);
                Vars.dataCache.Put(coordinate, elev);
                return elev;
            }
            else return res;
        }

        /// <summary>
        ///  возвращает высоту по заданной точке
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <returns></returns>
        public double GetElevation(Coordinate.CoordinateRecord lat, Coordinate.CoordinateRecord lon)
        { return GetElevation(lat, lon); }

        /// <summary>
        /// создает новый трек с высотами каждой точки
        /// </summary>
        /// <param name="track">трек</param>
        /// <param name="callback">действие, выполняемое при обработке точек</param>
        /// <returns>трек с высотами точек</returns>
        public TrackFile GetElevation(TrackFile track, Action<string> callback=null)
        {
            //еси провайдер поддерживает множетсвенные запроссы высот
            if (provider == GeoInfoProvider.Google)
                return ((Google)geoinfo).GetElevations(track, callback);

            //если приходится работать с каждой точкой отдельно
            TrackFile res = new TrackFile();
            res.Name = track.Name;
            res.FileName = track.FileName;
            res.FilePath = track.FilePath;
            res.Description = track.Description;
            res.Color = track.Color;
            double all = track.Count;
            double c = 0;
            foreach (TrackPoint tp in track)
            {
                if (callback != null)
                    callback.Invoke("Обрабатывается " + track.Name + ", завершено " + (c / all * 100d).ToString("0.0") + "%");
                if (double.IsNaN(tp.MetrAltitude))
                    tp.MetrAltitude = GetElevation(tp.Coordinates);
                res.Add(tp);
                c++;
            }
            return res;
        }

        /// <summary>
        /// истина, если база данных ETOPO2 установлена
        /// </summary>
        public static bool IsETOPO2Ready
        {
            get
            {
                return ETOPO2Provider.DatabaseInstalled(Vars.Options.DataSources.ETOPO2DBFolder);
            }
        }
    }
}
