using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.OS
{
    /// <summary>
    /// предоставляет данные системы о местоположении
    /// </summary>
    public class Geolocation
    {
        /// <summary>
        /// создаёт новый экземпляр геолокатора
        /// </summary>
        public Geolocation() { }

        /// <summary>
        /// возвращает координаты устройства
        /// </summary>
        /// <returns></returns>
        public Coordinate GetLocation()
        {
            //TODO: сделать поиск устройства

            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.Start(false);
            GeoCoordinate coord = watcher.Position.Location;
            if (!coord.IsUnknown)
                return new Coordinate(coord.Latitude, coord.Longitude);
            else
                throw new Exception("Служба определения местоположения недоступна");
        }
    }
}
