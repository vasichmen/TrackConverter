using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Geodesy.Systems
{
    /// <summary>
    /// Эллипсоид WGS84 
    /// Источник: https://confluence.qps.nl/pages/viewpage.action?pageId=38700159#WorldGeodeticSystem1984(WGS84)-Параметры
    /// </summary>
    public class WGS84 : BaseSystem
    {
        /// <summary>
        /// большая полуось,м
        /// </summary>
        public override double MaxAxis
        {
            get
            {
                return 6378137d;
            }
        }

        /// <summary>
        /// скорость света, м/с
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
                return 1 / 298.257223563d;
            }
        }

        /// <summary>
        /// гравитационная постоянная
        /// </summary>
        public override double f
        {
            get
            {
                return 3986004.418e8;
            }
        }

        /// <summary>
        /// угловая скорость врщения
        /// </summary>
        public override double AngleSpeed
        {
            get
            {
                return 7292115e-11;
            }
        }
    }
}
