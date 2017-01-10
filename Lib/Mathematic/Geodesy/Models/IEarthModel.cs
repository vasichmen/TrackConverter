using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Geodesy.Models
{
    /// <summary>
    /// система координат
    /// </summary>
    public interface IEarthModel
    {
        /// <summary>
        /// Средний радиус Земли
        /// </summary>
        double AverageRadius { get; }

        /// <summary>
        /// Большая полуось
        /// </summary>
        double MaxAxis { get; }

        /// <summary>
        /// Малая полуось
        /// </summary>
        double MinAxis { get; }

        /// <summary>
        /// полярное сжатие эллипсоида
        /// </summary>
        double Compression { get; }

        /// <summary>
        /// расчет расстояния между двумя точками в метрах 
        /// </summary>
        /// <param name="p1">первая точка</param>
        /// <param name="p2">вторая точка</param>
        /// <returns></returns>
        double CalculateDistance(TrackPoint p1, TrackPoint p2);

        /// <summary>
        /// расчет расстояния между двумя координатами в метрах 
        /// </summary>
        /// <param name="c1">первая точка</param>
        /// <param name="c2">вторая точка</param>
        /// <returns></returns>
        double CalculateDistance(Coordinate c1, Coordinate c2);

        /// <summary>
        /// Вычисление магнитного склонения в заданной точке
        /// </summary>
        /// <param name="p1">точка, в которой определяется магнитное склонение</param>
        /// <returns></returns>
        double CalculateMagneticDeclination(TrackPoint p1);

        /// <summary>
        /// вычисление магнитного азимута в градусах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        double CalculateMagneticAzimuth(TrackPoint p1, TrackPoint p2);

        /// <summary>
        /// Вычисление истинного азимута в градусах
        /// </summary>
        /// <param name="p1">начальная точка</param>
        /// <param name="p2">конечная точка</param>
        /// <returns></returns>
        double CalculateTrueAzimuth(TrackPoint p1, TrackPoint p2);
    }
}