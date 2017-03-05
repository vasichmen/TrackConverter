using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Astronomy
{
    /// <summary>
    /// информация о времени
    /// </summary>
    public struct TimeStruct
    {
        /// <summary>
        /// прошло часов с полуночи [0 - 23]
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// день месяца - [1,31] 
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// минут с начала часа - [0,59]
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// месяцев с января - [0,11] 
        /// </summary>
        public int Mon { get; set; }

        /// <summary>
        /// секунд после минуты - [0,59] 
        /// </summary>
        public int Sec { get; set; }

        /// <summary>
        /// лет с 1900 года
        /// </summary>
        public int Year { get; set; }
    }

}
