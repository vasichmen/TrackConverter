using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Geodesy.Models
{
    /// <summary>
    /// параметры СК ПЗ-90.11 
    /// Источник: http://structure.mil.ru/files/pz-90.pdf
    /// </summary>
   public class PZ90 : BaseModel
    {
        /// <summary>
        /// большая полуось
        /// </summary>
        public override double MaxAxis
        {
            get
            {
                return 6378136;
            }
        }

        /// <summary>
        /// скорость света
        /// </summary>
        public override double LightSpeed
        {
            get
            {
                return 299792458;
            }
        }

        /// <summary>
        /// полярное сжатие
        /// </summary>
        public override double Compression
        {
            get
            {
                return 1 / 298.25784d;
            }
        }


        /// <summary>
        /// угловая скорость вращения
        /// </summary>
        public override double AngleSpeed
        {
            get
            {
                return 7.292115e-5;
            }
        }
    }
}
