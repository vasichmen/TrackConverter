using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Geodesy.Models;

namespace TrackConverter.Lib.Mathematic.Geodesy.Projections
{
    /// <summary>
    /// проекция модели Земли на плоскость
    /// </summary>
    public interface IProjection
    {
        /// <summary>
        /// получить координаты на плоскости по географичесим координатам
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        Point GetXY(Coordinate coordinate);

        /// <summary>
        /// получить географические координаты по координатам плоскости
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Coordinate GetCoordinate(Point point);
    }
}
