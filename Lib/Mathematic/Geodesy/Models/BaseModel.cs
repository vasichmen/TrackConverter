using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Astronomy;
using TrackConverter.Lib.Mathematic.Base;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Geodesy.Models
{
    /// <summary>
    /// Система координат. 
    /// Вычисления на эллипсоиде: 
    /// http://www.geokniga.org/bookfiles/geokniga-podshivalov-vp-kurs-lekciy-po-vysshey-geodezii-sferoidicheskaya-geodeziya-2005.pdf
    /// Теория:
    /// http://blog.foxylab.com/prakticheskaya-kartografiya/
    /// </summary>
    public abstract class BaseModel : IEarthModel
    {

        #region параметры эллипсоида

        /// <summary>
        /// скорость света в вакууме, м/с
        /// </summary>
        public abstract double LightSpeed { get; }

        /// <summary>
        /// Угловая скорость вращения Земли, рад/с
        /// </summary>
        public abstract double AngleSpeed { get; }

        /// <summary>
        /// Больший радиус общеземного эллипсоида (к экватору), м
        /// </summary>
        public abstract double MaxAxis { get; }

        /// <summary>
        /// Полярное сжатие общеземного  эллипсоида. 
        /// Compression = (MaxAxis - MinAxis)/MaxAxis
        /// </summary>
        public abstract double Compression { get; }

        /// <summary>
        /// Средний радиус, м
        /// </summary>
        public double AverageRadius { get { return Math.Sqrt(MinAxis * MaxAxis); } }

        /// <summary>
        /// Малый радиус (к полюсу), м
        /// </summary>
        public double MinAxis { get { return MaxAxis - Compression * MaxAxis; } }

        /// <summary>
        /// Длина экватора, м
        /// </summary>
        public double Equator { get { return 2 * Math.PI * MaxAxis; } }

        /// <summary>
        /// Длина меридина, м
        /// </summary>
        public double Meridian { get { return 2 * Math.PI * MinAxis; } }

        /// <summary>
        /// Расстояние в одном градусе широты
        /// </summary>
        public double In1Lat { get { return Meridian / 360d; } }

        #endregion

        #region Реализации интерфейсов

        /// <summary>
        /// вычисление расстояния между точками в метрах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateDistance(TrackPoint p1, TrackPoint p2)
        {
            return this.CalculateDistance(p1.Coordinates, p2.Coordinates);
        }

        /// <summary>
        /// вычисление расстояния между точками в метрах
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public double CalculateDistance(Coordinate c1, Coordinate c2)
        {
            return CalculateDistance(c1.Latitude, c1.Longitude, c2.Latitude, c2.Longitude);
        }

        /// <summary>
        /// расчет расстояния между двумя точками в метрах
        /// </summary>
        /// <param name="p1">точка 1</param>
        /// <param name="p2">точка 2</param>
        /// <returns></returns>
        public double CalculateDistance(PointLatLng p1, PointLatLng p2)
        {
            return CalculateDistance(p1.Lat, p1.Lng, p2.Lat, p2.Lng);
        }

        /// <summary>
        /// расчет расстояния между двумя точками в метрах
        /// </summary>
        /// <param name="lat1">широта 1 точки</param>
        /// <param name="lon1">долгота 1 точки</param>
        /// <param name="lat2">широта 2 точки</param>
        /// <param name="lon2">долгота 2 точки</param>
        /// <returns></returns>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double d = SphereCalculations.CalculateDistance(lat1, lon1, lat2, lon2);
            double D = d * this.AverageRadius;
            return D;
        }

        /// <summary>
        /// расчет длины многоугольника в метрах
        /// </summary>
        /// <param name="points">координаты вершин многоугольника</param>
        /// <returns></returns>
        public double CalculateDistance(List<PointLatLng> points)
        {
            double res = 0;
            for (int i = 0; i < points.Count - 1; i++)
                res += CalculateDistance(points[i], points[i + 1]);
            res += CalculateDistance(points[points.Count - 1], points[0]);
            return res;
        }

        /// <summary>
        /// вычисление магнитного склонение в заданной точке
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public double CalculateMagneticDeclination(TrackPoint p1)
        {
            TrackPoint p2 = new TrackPoint(new Coordinate(37.459201, 55.935163));
            double trA = CalculateTrueAzimuth(p1, p2);
            double mnA = CalculateMagneticAzimuth(p1, p2);
            return mnA - trA;
        }

        /// <summary>
        /// вычисление магнитного азимута в градусах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateMagneticAzimuth(TrackPoint p1, TrackPoint p2)
        {
            if (Vars.Options.Converter.NorthPoleLatitude == 0)
                Vars.Options.Converter.NorthPoleLatitude = 85.90000;
            if (Vars.Options.Converter.NorthPoleLongitude == 0)
                Vars.Options.Converter.NorthPoleLongitude = -147.00000;

            return SphereCalculations.CalculateAngle(p1.Coordinates, new Coordinate(Vars.Options.Converter.NorthPoleLatitude, Vars.Options.Converter.NorthPoleLongitude), p2.Coordinates, AngleMeasure.Degrees);

        }

        /// <summary>
        /// вычисление истинного азимута от p1 к р2 в градусах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateTrueAzimuth(TrackPoint p1, TrackPoint p2)
        {
            //перевод градусов в радианы
            double rad1deg = Math.PI / 180;
            double lat1 = p1.Coordinates.Latitude * rad1deg; //α1

            //sin α1 = 0 - это означает, что начальная точка A находится на полюсе земли. 
            //Если широта начальной точки 90° (северный полюс), 
            //то любое направление из этой точки идет на юг. 
            //Поэтому φ = 180°. Если широта начальной точки -90° (южный полюс), 
            //то любое направление из этой точки идет на север. Поэтому φ = 0°.
            if (Math.Sin(lat1) == 0)
                if (lat1 < 0)
                    return 0;
                else return 180;

            return SphereCalculations.CalculateAngle(p1.Coordinates, new Coordinate(90, 0), p2.Coordinates, AngleMeasure.Degrees);
        }





        #endregion



    }
}
