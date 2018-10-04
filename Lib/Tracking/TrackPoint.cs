using System;
using GMap.NET;
using Newtonsoft.Json;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Exceptions;
using TrackConverter.Lib.Mathematic.Astronomy;
using TrackConverter.Lib.Mathematic.Geodesy;
using TrackConverter.Res;

namespace TrackConverter.Lib.Tracking
{
#pragma warning disable CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    /// <summary>
    /// Информация о точке (Широта, Долгота, Высота, Время)
    /// </summary>
    public class TrackPoint: IComparable
#pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        #region конструкторы

        /// <summary>
        /// конструктор для сериализации Json
        /// </summary>
        private TrackPoint()
        { }

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
            : this(double.Parse(lat.Replace('.', Vars.DecimalSeparator)), double.Parse(lon.Replace('.', Vars.DecimalSeparator))) { }
        
        /// <summary>
        /// создает  экземпляр TrackPoint с заданными координатами
        /// </summary>
        /// <param name="cd">новые координаты</param>
        public TrackPoint(Coordinate cd)
        {
            this.Coordinates = cd;
            this.MagneticAzimuth = double.NaN;
            this.TrueAzimuth = double.NaN;
            this.Icon = IconOffsets.MARKER;
            this.MetrAltitude = double.NaN;
            this.PointType = RouteWaypointType.None;
        }

        #endregion

        #region поля и свойства


        private string description;
        private string name;
        private double metrAltitude;

        /// <summary>
        /// преставление координат в виде структуры
        /// </summary>
        public  Coordinate Coordinates;

        /// <summary>
        /// высота в метрах
        /// </summary>
        public double MetrAltitude { get { return metrAltitude; } set { metrAltitude = value; } }

        /// <summary>
        /// высота в футах
        /// </summary>
        [JsonIgnore]
        public double FeetAltitude
        {
            get { return MetrAltitude / Constants.In1Feet; }
            set
            {
                MetrAltitude = value * Constants.In1Feet;
            }
        }

        /// <summary>
        /// время создания этой точки.
        /// Если время неизвестно, то значение null или DateTime.MinValue
        /// </summary>
        public DateTime Time { get; set; }


        /// <summary>
        /// тип путевой точки 
        /// </summary>
        public RouteWaypointType PointType { get; set; }

        /// <summary>
        /// текущее время в этой точке
        /// </summary>
        [JsonIgnore]
        public DateTime CurrentTime
        {
            get
            {
                return DateTime.UtcNow.Add(TimeZone.GetUtcOffset(DateTime.UtcNow));
            }
        }

        /// <summary>
        /// часовой пояс
        /// </summary>
        [JsonIgnore]
        public TimeZoneInfo TimeZone { get; set; }

        /// <summary>
        /// Имя точки
        /// </summary>
        public string Name { get => name; set { name = value.Replace(",", " ").Replace("\r\n", ""); } }

        /// <summary>
        /// Описание точки
        /// </summary>
        public string Description { get => description; set { description = value.Replace(",", " ").Replace("\r\n", ""); } }

        /// <summary>
        /// Скорость в этой точке в км/ч
        /// </summary>
        [JsonIgnore]
        public double KmphSpeed { get; set; }

        /// <summary>
        /// Скорость в этой точке в узлах
        /// </summary>
        [JsonIgnore]
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
        /// Магнитное склонение в градусах
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

        /// <summary>
        /// время восхода GMT+0
        /// </summary>
        public SunTime Rise { get; set; }

        /// <summary>
        /// время заката GMT+0
        /// </summary>
        public SunTime Fall { get; set; }

        /// <summary>
        /// азимут восхода
        /// </summary>
        public double RiseAzi { get; set; }

        /// <summary>
        /// азимут заката
        /// </summary>
        public double FallAzi { get; set; }

        /// <summary>
        /// продолжительность дня
        /// </summary>
        [JsonIgnore]
        public TimeSpan DayLength
        {
            get
            {
                if (Rise == null || Fall == null)
                    return TimeSpan.Zero;
                if (Rise.Empty)
                    Rise = AstronomyCalculations.CalculateRise(this);
                if (Fall.Empty)
                    Fall = AstronomyCalculations.CalculateFall(this);
                if (Rise.DayType == SunTime.WOSet.PolarDay)
                    return TimeSpan.FromHours(24);
                if (Rise.DayType == SunTime.WOSet.PolarNight)
                    return TimeSpan.FromHours(0);

                int hours = Fall.Hour - Rise.Hour;
                int mins = Fall.Minutes = Rise.Minutes;
                int secs = Fall.Seconds - Rise.Seconds;
                return new TimeSpan(hours, mins, secs);
            }
        }

        /// <summary>
        /// Текстовое представление типа точки
        /// </summary>
        /// <exception cref="TrackConverterException"></exception>
        public string PointTypeString
        {
            get
            {
                switch (this.PointType)
                {
                    case RouteWaypointType.Camp:
                        return "Привал";
                    case RouteWaypointType.CollectPoint:
                        return "Точка сбора";
                    case RouteWaypointType.Finish:
                        return "Финиш";
                    case RouteWaypointType.Interest:
                        return "Достопримечательность";
                    case RouteWaypointType.None:
                        return "Точка";
                    case RouteWaypointType.Overnight:
                        return "Ночёвка";
                    case RouteWaypointType.Start:
                        return "Старт";
                    case RouteWaypointType.WaterSource:
                        return "Источник воды";
                    case RouteWaypointType.Shop:
                        return "Магазин";
                    default:
                        throw new TrackConverterException("неизвестный тип точки " + this.PointType);
                }
            }
        }

        /// <summary>
        /// коорднаты точки в формате GMap
        /// </summary>
        private PointLatLng gmap = PointLatLng.Empty;

        /// <summary>
        /// координаты в формате GMap
        /// </summary>
        public PointLatLng GMap
        {
            get
            {
                if(gmap.IsEmpty)
                    gmap =  new PointLatLng(Coordinates.Latitude, Coordinates.Longitude);
                return gmap;
            }
        }

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
            if (Vars.CurrentGeosystem.CalculateDistance(this, trackPoint) <= length)
            {
                res.Add(trackPoint);
                return res;
            }

            //вычисление серединной точки
            double clat = 0;
            double clon = 0;
            //разницы по координатам
            double dlat = trackPoint.Coordinates.Latitude - this.Coordinates.Latitude;
            double dlon = trackPoint.Coordinates.Longitude - this.Coordinates.Longitude;
            //прибавки к координатам
            double plat = dlat / 2.0;
            double plon = dlon / 2.0;
            //вычисление новых координат
            clat = this.Coordinates.Latitude + plat;
            clon = this.Coordinates.Longitude + plon;

            TrackPoint cpoint = new TrackPoint(clat, clon);

            //вызов для каждого отрезка рекурсии
            TrackFile p1 = CalculateIntermediatePoints(cpoint, length);
            TrackFile p2 = cpoint.CalculateIntermediatePoints(trackPoint, length);
            res.Add(p1);
            res.Add(p2);

            return res;

        }

        /// <summary>
        /// вычисление параметров точки: склонение, восход, закат, время
        /// </summary>
        public void CalculateParametres()
        {
            MagneticDeclination = Vars.CurrentGeosystem.CalculateMagneticDeclination(this);
            //TimeOffset = AstronomyCalculations.CalculateTimeOffset(this);
            if (this.TimeZone != null)
            {
                Rise = AstronomyCalculations.CalculateRise(this);
                Fall = AstronomyCalculations.CalculateFall(this);
                RiseAzi = AstronomyCalculations.CalculateRiseAzimuth(this);
                FallAzi = AstronomyCalculations.CalculateFallAzimuth(this);
            }
        }

        /// <summary>
        /// создает копию точки
        /// </summary>
        /// <returns></returns>
        public TrackPoint Clone()
        {
            TrackPoint res = new TrackPoint(this.Coordinates.Latitude, this.Coordinates.Longitude)
            {
                Description = this.Description,
                Icon = this.Icon,
                MetrAltitude = this.MetrAltitude,
                Name = this.Name,
                Time = this.Time,
                PointType = this.PointType
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
            if (obj is TrackPoint)
                return this.Coordinates.Equals(((TrackPoint)obj).Coordinates);
            else
                return false;
        }




        #endregion

    }


}
