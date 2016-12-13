namespace TrackConverter.Lib.Mathematic
{
    /// <summary>
    /// основные константы для расчетов 
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// длина меридиана в м
        /// </summary>
        public const double Meridian = 40008600;

        /// <summary>
        /// длина экватора в м
        /// </summary>
        public const double Equator = 40075696;

        /// <summary>
        /// Длина среднего радиуса Земли в метрах
        /// </summary>
        public const double Radius = 6372795;

        /// <summary>
        /// длина одного градуса широты в метрах
        /// </summary>
        public const double In1Lat = Meridian / 360;

        /// <summary>
        /// длина одного фута в метрах
        /// </summary>
        public const double In1Feet = 0.3048;

        /// <summary>
        /// километров/час в одном узле
        /// </summary>
        public const double In1Knot = 1.852;

    }
}
