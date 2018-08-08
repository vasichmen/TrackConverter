using System;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// основные методы кэша геокодера
    /// </summary>
    public interface IGeocoderCache
    {
        /// <summary>
        /// удалить записи геокодера из кэша
        /// </summary>
        void ClearGeocoder();

        /// <summary>
        /// получить адрес точки
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        string GetAddress(Coordinate coordinate);

        /// <summary>
        /// получить координаты адреса
        /// </summary>
        /// <param name="address">адрес</param>
        /// <returns></returns>
        Coordinate GetCoordinate(string address);

        /// <summary>
        /// добавить информацию в кэш
        /// </summary>
        /// <param name="Coordinate">координаты адреса</param>
        /// <param name="Address">адрес</param>
        void PutGeocoder(Coordinate Coordinate, string Address);

        /// <summary>
        /// узнать временную зону точки
        /// </summary>
        /// <param name="coordinates">координаты точки</param>
        /// <returns></returns>
        TimeZoneInfo GetTimeZone(Coordinate coordinates);

        /// <summary>
        /// добавить информацию о временной зоне точки
        /// </summary>
        /// <param name="coordinates">координаты точки</param>
        /// <param name="tzi">временная зона</param>
        void PutGeoInfo(Coordinate coordinates, TimeZoneInfo tzi);
    }
}