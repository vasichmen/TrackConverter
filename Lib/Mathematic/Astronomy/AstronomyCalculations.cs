using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Astronomy
{
    /// <summary>
    /// Астрономические вычисления
    /// </summary>
    public static class AstronomyCalculations
    {
        /// <summary>
        /// вычисление часового пояса для точки на основе строгого деления на пояса
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateTimeOffset(TrackPoint trackPoint)
        {
            double lat = trackPoint.Coordinates.Latitude;
            double in1ho = 180d / 12d;
            double offset = lat / in1ho;
            return (int)offset;
           // return offset;
            //return (DateTime.Now- DateTime.UtcNow).Hours;
        }

        /// <summary>
        /// вычисление времени восходав локальном времени
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static SunTime CalculateRise(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = new SunRiseSet().RiseSet(trackPoint.TimeZone.BaseUtcOffset.TotalHours, trackPoint.Coordinates.Latitude, trackPoint.Coordinates.Longitude, ref rise, ref set);
            return new SunTime(rise, ans);
        }

        /// <summary>
        /// вычисление времени заката в локальном времени
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static SunTime CalculateFall(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = new SunRiseSet().RiseSet(trackPoint.TimeZone.BaseUtcOffset.TotalHours, trackPoint.Coordinates.Latitude, trackPoint.Coordinates.Longitude, ref rise, ref set);
            return new SunTime(set, ans);
        }

        /// <summary>
        /// вычисление азимута восхода в градусах
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateRiseAzimuth(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = new SunRiseSet().Azimuths(trackPoint.TimeZone.BaseUtcOffset.TotalHours, trackPoint.Coordinates.Latitude, trackPoint.Coordinates.Longitude, ref rise, ref set);
            if (ans != 1)
                return double.NaN;
            return rise;
        }

        /// <summary>
        /// вычисление азимута захода в градусах
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateFallAzimuth(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = new SunRiseSet().Azimuths(trackPoint.TimeZone.BaseUtcOffset.TotalHours, trackPoint.Coordinates.Latitude, trackPoint.Coordinates.Longitude, ref rise, ref set);
            if (ans != 1)
                return double.NaN;
            return set;
        }

        /// <summary>
        /// вычисление часового пояса по координатам точки (соответствует математическим границам часовых поясов)
        /// </summary>
        /// <param name="point">соординаты точки</param>
        /// <returns></returns>
        public static int CalculateTimeZone(Coordinate point) {
            if (point.Longitude == 180 )
                return 12;
            if (point.Longitude == -180)
                return -12;
            if (point.Longitude == 0)
                return 0;
            if (point.Longitude == -0)
                return 0;
            if (point.Longitude > 0)
                return (int)(point.Longitude / 15 + 1);
            else
                return -(int)(Math.Abs(point.Longitude) / 15 + 1);
        }
    }
}
