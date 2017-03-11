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
            double lat = trackPoint.Coordinates.Latitude.TotalDegrees;
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
            int ans = new SunRiseSet().RiseSet(trackPoint.TimeOffset, trackPoint.Coordinates.Latitude.TotalDegrees, trackPoint.Coordinates.Longitude.TotalDegrees, ref rise, ref set);
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
            int ans = new SunRiseSet().RiseSet(trackPoint.TimeOffset, trackPoint.Coordinates.Latitude.TotalDegrees, trackPoint.Coordinates.Longitude.TotalDegrees, ref rise, ref set);
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
            int ans = new SunRiseSet().Azimuths(trackPoint.TimeOffset, trackPoint.Coordinates.Latitude.TotalDegrees, trackPoint.Coordinates.Longitude.TotalDegrees, ref rise, ref set);
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
            int ans = new SunRiseSet().Azimuths(trackPoint.TimeOffset, trackPoint.Coordinates.Latitude.TotalDegrees, trackPoint.Coordinates.Longitude.TotalDegrees, ref rise, ref set);
            if (ans != 1)
                return double.NaN;
            return set;
        }
    }
}
