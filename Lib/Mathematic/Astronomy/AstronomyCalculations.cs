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
        /// Вычисление восхода и захода солнца (везде время местное).
        /// 1 : все ОК 
        /// или -1 : сегодня не всходит (весь день темно или полярная ночь) 
        /// или -2 : сегодня не заходит (весь день светло или полярный день)
        /// </summary>
        /// <param name="timeOffset">часовой пояс (+ или - кол-во часов от UTC)</param>
        /// <param name="coordinates">координаты точки</param>
        /// <param name="rise">восход солнца в часах</param>
        /// <param name="set">заход солнца в часах</param>
        /// <returns></returns>
        public static int GetSunParametres(double timeOffset, Coordinate coordinates, ref double rise, ref double set)
        {
            return new SunParametres().SunRiseSet(timeOffset, coordinates.Latitude.TotalDegrees, coordinates.Longitude.TotalDegrees, ref rise, ref set);
        }

        /// <summary>
        /// вычисление часового пояса для точки на основе строгого деления на пояса
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateTimeOffset(TrackPoint trackPoint)
        {
            //double lat = trackPoint.Coordinates.Latitude.TotalDegrees;
            //double in1ho = 180d / 12d;
            //double offset = lat / in1ho;
            //return offset;
            return (DateTime.Now- DateTime.UtcNow).Hours;
        }

        /// <summary>
        /// вычисление времени восхода GMT+0
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static SunTime CalculateRise(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = GetSunParametres(trackPoint.TimeOffset, trackPoint.Coordinates, ref rise, ref set);
                return new SunTime(rise, ans);
        }

        /// <summary>
        /// вычисление времени заката GMT+0
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static SunTime CalculateFall(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = GetSunParametres(trackPoint.TimeOffset, trackPoint.Coordinates, ref rise, ref set);
            return new SunTime(set, ans);
        }

        /// <summary>
        /// вычисление азимута восхода в градусах
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateRiseAzimuth(TrackPoint trackPoint)
        {
            return 0;
        }

        /// <summary>
        /// вычисление азимута заката в градусах
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static double CalculateFallAzimuth(TrackPoint trackPoint)
        {
            return 0;
        }
    }
}
