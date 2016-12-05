using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackConverter.Lib;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic;
using TrackConverter.Res;

namespace TrackConverter.Lib.Tracking
{
    /// <summary>
    /// Информация о точке (Широта, Долгота, Высота, Время)
    /// </summary>
    public class TrackPoint : IComparable
    {
        #region конструкторы

        /// <summary>
        /// создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        /// <param name="point">координата в формете GMap</param>
        public TrackPoint(PointLatLng point)
            : this(point.Lat, point.Lng) { }

        /// <summary>
        /// создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public TrackPoint(double lat, double lon)
            : this(new Coordinate(lat, lon)) { }

        /// <summary>
        ///  создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        ///  <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public TrackPoint(string lat, string lon)
            : this(double.Parse(lat.Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0])), double.Parse(lon.Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]))) { }

        /// <summary>
        ///  создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        ///  <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public TrackPoint(Coordinate.CoordinateRecord lat, Coordinate.CoordinateRecord lon)
            : this(new Coordinate(lat, lon)) { }

        /// <summary>
        /// создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        /// <param name="cd">новые координаты</param>
        public TrackPoint(Coordinate cd)
        {
            this.Coordinates = cd;
            this.MagneticAzimuth = double.NaN;
            this.TrueAzimuth = double.NaN;
            this.Icon = IconOffsets.ZeroOffset;
            this.MetrAltitude = double.NaN;
        }

        #endregion

        #region поля и свойства

        private string description;
        private string name;
        private double metrAltitude;

        /// <summary>
        /// преставление координат в виде структуры
        /// </summary>
        public Coordinate Coordinates { get; set; }

        /// <summary>
        /// высота в метрах
        /// </summary>
        public double MetrAltitude { get { return metrAltitude; } set { metrAltitude = value == 0 ? double.NaN : value; } }

        /// <summary>
        /// высота в футах
        /// </summary>
        public double FeetAltitude { get { return MetrAltitude / Constants.In1Feet; } set { MetrAltitude = value * Constants.In1Feet; } }

        /// <summary>
        /// время создания этой точки
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Имя точки
        /// </summary>
        public string Name { get { return name != null ? name.Replace(",", " ").Replace("\r\n", "") : null; } set { name = value; } }

        /// <summary>
        /// Описание точки
        /// </summary>
        public string Description { get { return description != null ? description.Replace(",", " ").Replace("\r\n", "") : null; } set { description = value; } }

        /// <summary>
        /// Скорость в этой точке в км/ч
        /// </summary>
        public double KmphSpeed { get; set; }

        /// <summary>
        /// Скорость в этой точке в узлах
        /// </summary>
        public double KnotSpeed { get { return KmphSpeed / Constants.In1Knot; } set { KmphSpeed = value * Constants.In1Knot; } }

        /// <summary>
        /// Истинный азимут в градусах на следующую точку
        /// </summary>
        public double TrueAzimuth { get; set; }

        /// <summary>
        /// Магнитный азимут в градусах на следующую точку
        /// </summary>
        public double MagneticAzimuth { get; set; }

        /// <summary>
        /// Магнитное склонение
        /// </summary>
        public double MagneticDeclination { get; set; }

        /// <summary>
        /// расстояние от предыдущей точки в километрах
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// расстояние от старта в километрах
        /// </summary>
        public double StartDistance { get; set; }

        /// <summary>
        /// номер файла иконки
        /// </summary>
        public int Icon { get; set; }

        #endregion

      
        /// <summary>
        /// Создает список точек между этой точкой и заданной trackPoint
        /// </summary>
        /// <param name="trackPoint">точка, до которой добавляются новые</param>
        /// <param name="length">минимальное расстояние в метрах между точками</param>
        /// <returns></returns>
        public TrackFile CalculateIntermediatePoints(TrackPoint trackPoint, double length)
        {
            TrackFile res = new TrackFile();
            //если расстояние до следующей точки меньше заданного, то добавляем конечную и выходим
            if (Calc.CalculateDistance(this, trackPoint, Vars.Options.Converter.DistanceMethodType) <= length)
            {
                res.Add(trackPoint);
                return res;
            }

            //вычисление серединной точки
            double clat = 0;
            double clon = 0;
            //разницы по координатам
            double dlat = trackPoint.Coordinates.Latitude.TotalDegrees - this.Coordinates.Latitude.TotalDegrees;
            double dlon = trackPoint.Coordinates.Longitude.TotalDegrees - this.Coordinates.Longitude.TotalDegrees;
            //прибавки к координатам
            double plat = dlat / 2.0;
            double plon = dlon / 2.0;
            //вычисление новых координат
            clat = this.Coordinates.Latitude.TotalDegrees + plat;
            clon = this.Coordinates.Longitude.TotalDegrees + plon;

            TrackPoint cpoint = new TrackPoint(clat, clon);

            //вызов для каждого отрезка рекурсии
            TrackFile p1 = CalculateIntermediatePoints(cpoint, length);
            TrackFile p2 = cpoint.CalculateIntermediatePoints(trackPoint, length);
            res.Add(p1);
            res.Add(p2);

            return res;

        }

        /// <summary>
        /// создает копию точки
        /// </summary>
        /// <returns></returns>
        public TrackPoint Clone()
        {
            TrackPoint res = new TrackPoint(this.Coordinates.Latitude.TotalDegrees, this.Coordinates.Longitude.TotalDegrees)
            {
                Description = this.Description,
                Icon = this.Icon,
                MetrAltitude = this.MetrAltitude,
                Name = this.Name,
                Time = this.Time
            };
            return res;
        }

        #region реализации интерфейсов

        /// <summary>
        /// сравнение с другой точкой по имени
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return Name.CompareTo(((TrackPoint)obj).name);
        }

        /// <summary>
        /// сравнение на равенство(координаты одинаковые)
        /// </summary>
        /// <param name="obj">объект-параметр</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Coordinates.Equals(((TrackPoint)obj).Coordinates);
        }


        #endregion

    }


}
