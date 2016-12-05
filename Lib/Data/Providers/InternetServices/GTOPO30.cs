using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;
using TrackConverter.Lib.Data.Interfaces;
using System.Threading;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// работа с сервисом GTOPO30
    /// </summary>
    class GTOPO30 : BaseConnection, IGeoInfoProvider
    {
        /// <summary>
        /// если истина, то это локальный источник данных
        /// </summary>
        public bool isLocal
        {
            get
            {
                return false;
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

        /// <summary>
        /// узнать высоту точки
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            //http://api.geonames.org/gtopo30?lat=47.01&lng=10.2&username=demo 

            string url = string.Format("http://api.geonames.org/gtopo30?lat={0}&lng={1}&username=demo",
               coordinate.Latitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0],'.'),
                coordinate.Longitude.TotalDegrees.ToString().Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0],'.')
                );

            string ans = SendStringRequest(url);

            try
            {
                double d = double.Parse(ans);
                if (d == -9999)
                    return double.NaN;
                else return d;
            }
            catch (FormatException)
            {
                throw new ApplicationException(ans);
            }
        }

    }
}
