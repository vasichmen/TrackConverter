using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Geodesy.Systems
{
    /// <summary>
    /// система координат
    /// </summary>
    public interface IGeosystem
    {
        /// <summary>
        /// расчет расстояния между двумя точками в метрах заданным методом
        /// </summary>
        /// <param name="p1">первая точка</param>
        /// <param name="p2">вторая точка</param>
        /// <returns></returns>
        double CalculateDistance(TrackPoint p1, TrackPoint p2);

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