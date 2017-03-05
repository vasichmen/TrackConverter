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
        /// <param name="now">дата для которой вычилаются параметры</param>
        /// <param name="timeOffset">часовой пояс (+ или - кол-во часов от UTC)</param>
        /// <param name="coordinates">координаты точки</param>
        /// <param name="rise">восход солнца в часах</param>
        /// <param name="set">заход солнца в часах</param>
        /// <returns></returns>
        public static int GetSunParametres(DateTime now, double timeOffset, Coordinate coordinates, ref double rise, ref double set)
        {
            TimeStruct ts = new TimeStruct();
            ts.Day = now.Day;
            ts.Hour = now.Hour;
            ts.Min = now.Minute;
            ts.Mon = now.Month;
            ts.Sec = now.Second;
            ts.Year = now.Year - 1900;
            return new SunParametres().GetSunSet(ts, timeOffset, coordinates.Latitude.TotalDegrees, coordinates.Longitude.TotalDegrees, ref rise, ref set);
        }

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
            return offset;
        }

        /// <summary>
        /// вычисление времени восхода GMT+0
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static DateTime CalculateRise(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = AstronomyCalculations.GetSunParametres(DateTime.Now, trackPoint.TimeOffset, trackPoint.Coordinates, ref rise, ref set);
            if (ans == -1 || ans == -2)
                return new DateTime();
            else
            {
                return new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    (int)rise,
                    (int)((rise - ((int)rise)) * 60.0),
                    0
                    );
            }
        }

        /// <summary>
        /// вычисление времени заката GMT+0
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public static DateTime CalculateFall(TrackPoint trackPoint)
        {
            double rise = 0, set = 0;
            int ans = GetSunParametres(DateTime.Now, trackPoint.TimeOffset, trackPoint.Coordinates, ref rise, ref set);
            if (ans == -1 || ans == -2)
                return new DateTime();
            else
            {
                return new DateTime(
                    DateTime.Now.Year,
                    DateTime.Now.Month,
                    DateTime.Now.Day,
                    (int)set,
                    (int)((set - ((int)set)) * 60.0),
                    0
                    );
            }
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
