using System;
using System.Collections.Generic;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// кэш высот, временных зон
    /// </summary>
    public interface IGeoInfoCache
    {
        /// <summary>
        /// удалить все высоты 
        /// </summary>
        void ClearAltitudes();

        /// <summary>
        /// узнать высоту точки
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        double GetElevation(Coordinate coordinate);

        /// <summary>
        /// добавить высоту точки
        /// </summary>
        /// <param name="Coordinate">координаты точки</param>
        /// <param name="Altitude">высота в метрах</param>
        void PutGeoInfo(Coordinate Coordinate, double Altitude);

        /// <summary>
        /// добавить список высот точек. Каждой точке из списка должна соответствовать её высота из списка высот
        /// </summary>
        /// <param name="track">список точек</param>
        /// <param name="els">список высот этих точек</param>
        /// <param name="callback">функция - вывод информации о процессе</param>
        void PutGeoInfo(BaseTrack track, List<double> els, Action<string> callback = null);

        /// <summary>
        /// попытка получить высоты точек для всего списка. Возвращает true, если все высоты нашлись
        /// </summary>
        /// <param name="track">список точек, куда будут записаны высоты</param>
        /// <returns></returns>
        bool TryGetElevations(ref BaseTrack track);
    }
}