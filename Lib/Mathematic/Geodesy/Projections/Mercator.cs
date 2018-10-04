using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Geodesy.Models;

namespace TrackConverter.Lib.Mathematic.Geodesy.Projections
{
    /// <summary>
    /// Википедия:
    /// https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%BE%D0%B5%D0%BA%D1%86%D0%B8%D1%8F_%D0%9C%D0%B5%D1%80%D0%BA%D0%B0%D1%82%D0%BE%D1%80%D0%B0
    /// проекция Меркатора:
    /// http://flot.com/publications/books/shelf/rulkov/19.htm
    /// Теория:
    /// http://blog.foxylab.com/prakticheskaya-kartografiya/
	/// создание привязки (как в GMap)
	/// http://gis-lab.info/qa/tfw.html
    /// </summary>
    public class Mercator : IProjection
    {
        private IEarthModel earthModel;

        /// <summary>
        /// создает проекцию по заданной модели Земли
        /// </summary>
        /// <param name="model">Модель Земли</param>
        public Mercator(IEarthModel model)
        {
            this.earthModel = model;
        }

        /// <summary>
        /// получить географические координаты по координатам на карте
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Coordinate GetCoordinate(System.Windows.Point point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Источник:
        /// http://wiki.gis-lab.info/w/%D0%9F%D0%B5%D1%80%D0%B5%D1%81%D1%87%D0%B5%D1%82_%D0%BA%D0%BE%D0%BE%D1%80%D0%B4%D0%B8%D0%BD%D0%B0%D1%82_%D0%B8%D0%B7_Lat/Long_%D0%B2_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%86%D0%B8%D1%8E_%D0%9C%D0%B5%D1%80%D0%BA%D0%B0%D1%82%D0%BE%D1%80%D0%B0_%D0%B8_%D0%BE%D0%B1%D1%80%D0%B0%D1%82%D0%BD%D0%BE#.D0.9A.D0.BE.D0.B4_.D0.BD.D0.B0_Python
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public System.Windows.Point GetXY(Coordinate coordinate)
        {
            double lat = coordinate.Latitude;
            double lon = coordinate.Longitude;
            double deg2rad = Math.PI / 180d;

            if (lat > 85)
                lat = 85;
            if (lat < -85)
                lat = -85;


            double rLat = lat * deg2rad;
            double rLong = lon * deg2rad;
            double a = earthModel.MaxAxis;
            double b = earthModel.MinAxis;
            double f = earthModel.Compression;
            double e = Math.Sqrt(2 * f - Math.Pow(f, 2));
            double x = a * rLong;
            double y = a * Math.Log(Math.Tan(Math.PI / 4 + rLat / 2) * Math.Pow(((1 - e * Math.Sin(rLat)) / (1 + e * Math.Sin(rLat))), (e / 2)));
            return new System.Windows.Point(x, y);
        }

    }
}
