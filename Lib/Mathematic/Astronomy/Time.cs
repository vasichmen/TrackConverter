using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TrackConverter.Lib.Mathematic.Astronomy
{
    /// <summary>
    /// солнечное время
    /// </summary>
    public class SunTime
    {
        /// <summary>
        /// тип солнечного дня
        /// </summary>
        public enum WOSet
        {
            /// <summary>
            /// Полярный день 
            /// </summary>
            PolarDay,

            /// <summary>
            /// полярная ночь
            /// </summary>
            PolarNight,

            /// <summary>
            /// нормальный режим
            /// </summary>
            Normal
        }

        /// <summary>
        /// тип солнечного дня
        /// </summary>
        public WOSet DayType { get; set; }

        /// <summary>
        /// создает солнечное время из часов и кода ответа метода SunParametres.sun_rise_set (1,-1,-2)
        /// </summary>
        /// <param name="hours">количество часов от начала суток в локальном часовом поясе</param>
        /// <param name="code">код ответа метода SunParametres.sun_rise_set (1,-1,-2)</param>
        public SunTime(double hours, int code)
        {
            if (code == -2) //полярный день
            {
                this.DayType = WOSet.PolarDay;
                return;
            }
            if (code == -1) //полярная ночь
            {
                this.DayType = WOSet.PolarNight;
                return;
            }
            if (code == 1) //нормальный режим
            {
                Hour = (int)hours;
                Minutes = (int)((hours - Math.Floor(hours)) * 60);
                double secs = ((hours - Math.Floor(hours)) * 60);
                Seconds = (int)((secs - Math.Floor(secs)) * 60);
                this.DayType = WOSet.Normal;
                return;
            }
        }

        /// <summary>
        /// пустая структура SunTime
        /// </summary>
        public SunTime()
        {
            Hour = -1;
            Seconds = -1;
            Minutes = -1;
        }

        /// <summary>
        /// количество часов
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// строковое предствление времени
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            switch (this.DayType)
            {
                case WOSet.Normal:
                    return string.Format("{0}:{1}", this.Hour.ToString("00"), this.Minutes.ToString("00"));
                case WOSet.PolarDay:
                    return "Полярный день";
                case WOSet.PolarNight:
                    return "Полярная ночь";
            }
            throw new ApplicationException("что-то не так");
        }

        /// <summary>
        /// минуты 
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// секунды
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// истина, если для экземпляра не заданы значения
        /// </summary>
        [JsonIgnore]
        public bool Empty { get {
                return !(Hour == -1 & Seconds == -1 & Minutes == -1);
            } }
    }
}
