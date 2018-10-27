using GMap.NET;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using TrackConverter.Lib.Exceptions;
using TrackConverter.Lib.Mathematic.Astronomy;
using TrackConverter.Lib.Mathematic.Geodesy;
using TrackConverter.Res;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// методы для работы с форматом Trip Route
    /// </summary>
    internal static class TrrHelper
    {
        #region классы


        /// <summary>
        /// Информация о точке (Широта, Долгота, Высота, Время)
        /// </summary>
        private class TrackPointOld : IComparable
        {
            #region конструкторы

            /// <summary>
            /// конструктор для сериализации Json
            /// </summary>
            private TrackPointOld()
            { }

            /// <summary>
            /// создает  экземпляр TrackPointOld с заданными координатами
            /// </summary>
            /// <param name="point">координата в формете GMap</param>
            public TrackPointOld(PointLatLng point)
                : this(point.Lat, point.Lng) { }

            /// <summary>
            /// создает  экземпляр TrackPointOld с заданными координатами
            /// </summary>
            /// <param name="lat">широта</param>
            /// <param name="lon">долгота</param>
            public TrackPointOld(double lat, double lon)
                : this(new CoordinateOld(lat, lon)) { }

            /// <summary>
            ///  создает  экземпляр TrackPointOld с заданными координатами
            /// </summary>
            ///  <param name="lat">широта</param>
            /// <param name="lon">долгота</param>
            public TrackPointOld(string lat, string lon)
                : this(double.Parse(lat.Replace('.', Vars.DecimalSeparator)), double.Parse(lon.Replace('.', Vars.DecimalSeparator))) { }

            /// <summary>
            /// создает  экземпляр TrackPointOld с заданными координатами
            /// </summary>
            /// <param name="cd">новые координаты</param>
            public TrackPointOld(CoordinateOld cd)
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
            public CoordinateOld Coordinates;

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
            public string Name { get => name; set { name = value?.Replace(",", " ").Replace("\r\n", ""); } }

            /// <summary>
            /// Описание точки
            /// </summary>
            public string Description { get => description; set { description = value?.Replace(",", " ").Replace("\r\n", ""); } }

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
                    if (gmap.IsEmpty)
                        gmap = new PointLatLng(Coordinates.Latitude.TotalDegrees, Coordinates.Longitude.TotalDegrees);
                    return gmap;
                }
            }

            #endregion


            ///// <summary>
            ///// вычисление параметров точки: склонение, восход, закат, время
            ///// </summary>
            //public void CalculateParametres()
            //{
            //    MagneticDeclination = Vars.CurrentGeosystem.CalculateMagneticDeclination(this);
            //    //TimeOffset = AstronomyCalculations.CalculateTimeOffset(this);
            //    if (this.TimeZone != null)
            //    {
            //        Rise = AstronomyCalculations.CalculateRise(this);
            //        Fall = AstronomyCalculations.CalculateFall(this);
            //        RiseAzi = AstronomyCalculations.CalculateRiseAzimuth(this);
            //        FallAzi = AstronomyCalculations.CalculateFallAzimuth(this);
            //    }
            //}

            /// <summary>
            /// создает копию точки
            /// </summary>
            /// <returns></returns>
            public TrackPointOld Clone()
            {
                TrackPointOld res = new TrackPointOld(this.Coordinates.Latitude.TotalDegrees, this.Coordinates.Longitude.TotalDegrees)
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
                return Name.CompareTo(((TrackPointOld)obj).name);
            }

            /// <summary>
            /// сравнение на равенство(координаты одинаковые)
            /// </summary>
            /// <param name="obj">объект-параметр</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is TrackPointOld)
                    return this.Coordinates.Equals(((TrackPointOld)obj).Coordinates);
                else
                    return false;
            }

            internal TrackPoint ToTrackPoint()
            {
                return new TrackPoint(this.Coordinates.Latitude.TotalDegrees, Coordinates.Longitude.TotalDegrees)
                {
                    Description = this.Description,
                    Distance = this.Distance,
                    Fall = Fall,
                    FallAzi = FallAzi,
                    MetrAltitude = MetrAltitude,
                    Icon = Icon,
                    KmphSpeed = KmphSpeed,
                    MagneticAzimuth = MagneticAzimuth,
                    MagneticDeclination = MagneticDeclination,
                    Name = Name,
                    PointType = PointType,
                    Rise = Rise,
                    RiseAzi = RiseAzi,
                    StartDistance = StartDistance,
                    Time = Time,
                    TimeZone = TimeZone,
                    TrueAzimuth = TrueAzimuth
                };
            }




            #endregion

        }

        private struct CoordinateOld
        {
            private readonly long llon;
            private readonly long llat;

            #region классы, перечисления

            /// <summary>
            /// 
            /// </summary>
            public enum CoordinateChar
            {
                /// <summary>
                /// северная широта
                /// </summary>
                N,

                /// <summary>
                /// южная широта
                /// </summary>
                S,

                /// <summary>
                /// западная долгота
                /// </summary>
                W,

                /// <summary>
                /// восточная долгота
                /// </summary>
                E
            }

            /// <summary>
            /// Тип координаты
            /// </summary>
            public enum CoordinateKind
            {
                /// <summary>
                /// Широта
                /// </summary>
                Latitude,

                /// <summary>
                /// Долгота
                /// </summary>
                Longitude
            }

            /// <summary>
            /// Одна координата
            /// </summary>
            [Serializable]
            public struct CoordinateRecord
            {

                /// <summary>
                /// создает новый экземпляр с указанной координатой
                /// </summary>
                /// <param name="degrees">дробное представление в градусах. (если задано положительное значение degrees , 
                /// но полушарие западное или южное, то в CoordinateRecord = -degrees)</param>
                /// <param name="Char">полушарие. 
                /// Если указано южное или западное полушарие, то значение CoordinateRecord.TotalDegrees 
                /// будет отрицательным</param>
                /// <exception cref="ArgumentOutOfRangeException">Возникает, если широта или долгота не попадают в свой диапазон значений</exception>
                public CoordinateRecord(double degrees, CoordinateChar Char)
                {
                    if ((Char == CoordinateChar.N || Char == CoordinateChar.S) && (Math.Abs(degrees) > 90))
                        throw new ArgumentOutOfRangeException("Значение широты должно быть в диапазоне от -90 до 90 градусов");
                    if ((Char == CoordinateChar.W || Char == CoordinateChar.E) && (Math.Abs(degrees) > 180))
                        throw new ArgumentOutOfRangeException("Значение долготы должно быть в диапазоне от -180 до 180 градусов");


                    this.TotalDegrees = degrees;
                    this.Char = Char;
                    if ((Char == CoordinateChar.W || Char == CoordinateChar.S) && this.TotalDegrees > 0)
                        this.TotalDegrees = -this.TotalDegrees;
                }

                /// <summary>
                /// создает новый экземпляр с указанной координатой. 
                /// Если количество минут или количество секунд превышает 60, 
                /// то частное от деления минут или секунд на 60 прибавляется к градусам, а остаток к минутам.
                /// </summary>
                /// <param name="Deg">градусы</param>
                /// <param name="Min">минуты</param>
                /// <param name="Sec">секунды</param>
                /// <param name="Char">полушарие. </param>
                public CoordinateRecord(int Deg, int Min, double Sec, CoordinateChar Char)
                    : this(Deg + Min / 60 + Sec / 3600, Char) { }

                /// <summary>
                /// создает новый экземпляр с указанной координатой
                /// </summary>
                /// <param name="Deg">градусы</param>
                /// <param name="Min">минуты</param>
                /// <param name="Char">полушарие</param>
                public CoordinateRecord(int Deg, double Min, CoordinateChar Char)
                    : this(Deg + Min / 60, Char) { }

                /// <summary>
                /// создает пустую координату
                /// </summary>
                /// <param name="isEmpty"></param>
                public CoordinateRecord(bool isEmpty)
                { TotalDegrees = double.NaN; Char = CoordinateChar.E; }

                /// <summary>
                /// дробное представление координаты, включая знак
                /// </summary>
                public double TotalDegrees { get; set; }

                /// <summary>
                /// целое положительное число градусов в координате
                /// </summary>
                [JsonIgnore]
                public int Degrees
                {
                    get
                    {
                        int dd = (int)TotalDegrees;
                        return Math.Abs(dd);
                    }
                }

                /// <summary>
                /// целое положительное число минут в координате
                /// </summary>
                [JsonIgnore]
                public int Minutes
                {
                    get
                    {
                        double mo = TotalDegrees - (int)TotalDegrees;
                        int mm = (int)(mo * 60);
                        return Math.Abs(mm);
                    }
                }

                /// <summary>
                /// положительное число секунд в координате
                /// </summary>
                [JsonIgnore]
                public double Seconds
                {
                    get
                    {
                        double mo = TotalDegrees - (int)TotalDegrees;
                        double m = mo * 60;
                        double so = m - ((int)m);
                        double sec = so * 60;
                        return Math.Abs(sec);
                    }
                }

                /// <summary>
                /// знак при координате
                /// </summary>
                [JsonIgnore]
                public CoordinateChar Char { get; set; }

                /// <summary>
                /// тип координаты: Широта/Долгота
                /// </summary>
                [JsonIgnore]
                public CoordinateKind Kind { get { return Char == CoordinateChar.E || Char == CoordinateChar.W ? CoordinateKind.Longitude : CoordinateKind.Latitude; } }

                /// <summary>
                /// если истина, то координата пуста
                /// </summary>
                [JsonIgnore]
                public bool isEmpty { get { return double.IsNaN(TotalDegrees); } }

                /// <summary>
                /// перевод строкового представления координаты
                /// </summary>
                /// <param name="source">строковое представление</param>
                /// <param name="format">формат записи координаты. Hddmm.mmm  ddmm.mmm,H</param>
                /// <returns></returns>
                public static CoordinateRecord Parse(string source, string format)
                {
                    double Min = 0;
                    double Deg = 0;
                    double Sec = 0;
                    string ch = "";
                    string min = "";
                    string grad = "";
                    string sec = "";

                    switch (format)
                    {

                        case "Hddmm.mmm":
                            min = source.Substring(3);
                            grad = source.Substring(1, 2);
                            sec = "0";
                            ch = source[0].ToString();
                            break;
                        case "ddmm.mmm,H":
                            min = source.Substring(2, 6);
                            grad = source.Substring(0, 2);
                            sec = "0";
                            ch = source[9].ToString();
                            break;
                        case "Hdd mm.mmm":
                            min = source.Substring(4, 6);
                            grad = source.Substring(1, 2);
                            sec = "0";
                            ch = source.Substring(0, 1);
                            break;
                        case "ddº mm.mmmm' H":
                            min = source.Substring(4, 7);
                            grad = source.Substring(0, 2);
                            sec = "0";
                            ch = source.Substring(12, 1);
                            break;
                        case "ddº mm' ss.ssss\" H":
                            min = source.Substring(4, 2);
                            grad = source.Substring(0, 2);
                            sec = source.Substring(8, 6);
                            ch = source.Substring(17, 1);
                            break;
                        case "ddºmm'ss.s\"H":
                            min = source.Substring(3, 2);
                            grad = source.Substring(0, 2);
                            sec = source.Substring(6, 4);
                            ch = source.Substring(11, 1);
                            break;
                        default:
                            throw new FormatException("Неподдерживаемый формат координат");
                    }

                    Min = double.Parse(min.Replace('.', Vars.DecimalSeparator));
                    Deg = double.Parse(grad.Replace('.', Vars.DecimalSeparator));
                    Sec = double.Parse(sec.Replace('.', Vars.DecimalSeparator));
                    double td = Deg + Min / 60 + Sec / 3600;

                    CoordinateChar cc = CoordinateChar.N;
                    switch (ch.ToLower())
                    {
                        case "w":
                            cc = CoordinateChar.W;
                            break;
                        case "s":
                            cc = CoordinateChar.S;
                            break;
                        case "n":
                            cc = CoordinateChar.N;
                            break;
                        case "e":
                            cc = CoordinateChar.E;
                            break;
                        default:
                            throw new Exception("Не удалось распознать полушарие");
                    }


                    return new CoordinateRecord(td, cc);
                }

                /// <summary>
                /// строковое представление координаты в указанном формате
                /// </summary>
                /// <param name="format">формат записи координаты. 
                /// Hddmm.mmm     ddmm.mmm,H    dd, mm.mmmm,H     Hdd mm.mmm       ddº mm.mmmm' H     ddº mm' ss.sss\" H     ddºmm'ss.s\"H       00.000000 </param>
                /// <returns></returns>
                /// <exception cref="FormatException">Возникает если заданный формат не поддерживается</exception>
                public string ToString(string format)
                {
                    string res = "";

                    switch (format)
                    {
                        case "Hddmm.mmm":
                            res = this.Char.ToString() + Degrees.ToString() + Math.Round((Math.Abs(TotalDegrees) - Degrees) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                            break;
                        case "ddmm.mmm,H":
                            res = Degrees.ToString() + Math.Round((Math.Abs(TotalDegrees) - Degrees) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.') + "," + this.Char.ToString();
                            break;
                        case "Hdd mm.mmm":
                            res = this.Char.ToString() + Degrees.ToString() + " " + Math.Round((Math.Abs(TotalDegrees) - Degrees) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                            break;
                        case "ddº mm.mmmm' H":
                            res = this.Degrees.ToString() + "º " + Math.Round((Math.Abs(TotalDegrees) - Degrees) * 60, 4).ToString("00.0000").Replace(Vars.DecimalSeparator, '.') + "' " + this.Char.ToString();
                            break;
                        case "ddº mm' ss.ssss\" H":
                            res = this.Degrees.ToString() + "º " + this.Minutes.ToString() + "' " + this.Seconds.ToString("00.0000").Replace(Vars.DecimalSeparator, '.') + "\" " + this.Char.ToString();
                            break;
                        case "ddºmm'ss.s\"H":
                            res = this.Degrees.ToString() + "º" + this.Minutes.ToString() + "'" + this.Seconds.ToString("00.0").Replace(Vars.DecimalSeparator, '.') + "\"" + this.Char.ToString();
                            break;
                        case "00.000000":
                            res = this.TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
                            break;
                        case "00.000":
                            res = this.TotalDegrees.ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                            break;
                        case "dd, mm.mmmm,H":
                            res = this.Degrees.ToString() + ", " + Math.Round((Math.Abs(TotalDegrees) - Degrees) * 60, 4).ToString("0.0000").Replace(Vars.DecimalSeparator, '.') + "," + this.Char.ToString();
                            break;
                        default:
                            throw new FormatException("Неподдерживаемый формат координат");
                    }

                    return res;
                }
            }

            #endregion

            #region  поля, свойства

            /// <summary>
            /// долгота
            /// </summary>
            public CoordinateRecord Longitude { get; set; }

            /// <summary>
            /// широта
            /// </summary>
            public CoordinateRecord Latitude { get; set; }

            /// <summary>
            /// координата в формате GMap
            /// </summary>
            [JsonIgnore]
            public PointLatLng GMap
            {
                get
                {
                    return new PointLatLng(
                        this.Latitude.TotalDegrees,
                        this.Longitude.TotalDegrees);
                }

            }

            /// <summary>
            /// пустые координаты
            /// </summary>
            [JsonIgnore]
            public static CoordinateOld Empty { get { return new CoordinateOld(true); } }

            /// <summary>
            /// если истина, то координата пуста
            /// </summary>
            [JsonIgnore]
            public bool isEmpty { get { return this.Latitude.isEmpty || this.Longitude.isEmpty; } }

            #endregion

            #region конструкторы

            /// <summary>
            /// создает новый экземпляр из заданных значений
            /// </summary>
            /// <param name="LatDeg">целые градусы широты</param>
            /// <param name="LatMin">целые минуты широты</param>
            /// <param name="LatSec">секунды широты</param>
            /// <param name="LatChar">полушарие широты</param>
            /// <param name="LonDeg">целые градусы долготы</param>
            /// <param name="LonMin">целые минуты долготы</param>
            /// <param name="LonSec">секунды долготы</param>
            /// <param name="LonChar">полушарие долготы</param>
            public CoordinateOld(int LatDeg, int LatMin, double LatSec, CoordinateChar LatChar, int LonDeg, int LonMin, double LonSec, CoordinateChar LonChar)
                : this(new CoordinateRecord(LatDeg, LatMin, LatSec, LatChar), new CoordinateRecord(LonDeg, LonMin, LonSec, LonChar)) { }

            /// <summary>
            /// создает новый экземпляр с указанными координатами
            /// </summary>
            /// <param name="lat">широта</param>
            /// <param name="lon">долгота</param>
            public CoordinateOld(double lat, double lon)
                : this(new CoordinateRecord(lat, lat >= 0 ? CoordinateOld.CoordinateChar.N : CoordinateOld.CoordinateChar.S), new CoordinateRecord(lon, lon >= 0 ? CoordinateOld.CoordinateChar.E : CoordinateOld.CoordinateChar.W)) { }


            /// <summary>
            /// создает новый экземпляр с указанными координатами формата dd.dddddd
            /// </summary>
            /// <param name="lat">широта</param>
            /// <param name="lon">широта</param>
            public CoordinateOld(string lat, string lon)
                : this(double.Parse(lat.Replace('.', Vars.DecimalSeparator)), double.Parse(lon.Replace('.', Vars.DecimalSeparator))) { }

            /// <summary>
            /// создает новый экземпляр Coordinate с заданными координатами
            /// </summary>
            /// <param name="lat">широта</param>
            /// <param name="lon">долгота</param>
            /// <exception cref="ArgumentNullException">Если долгота или широта равна null</exception>
            public CoordinateOld(CoordinateRecord lat, CoordinateRecord lon)
            {
                // if (lat == null || lon == null)
                //    throw new ArgumentNullException("Один из аргументов равен null");
                Latitude = lat;
                Longitude = lon;
                this.llon = (long)(lon.TotalDegrees * 10000000);
                this.llat = (long)(lat.TotalDegrees * 10000000);
            }

            /// <summary>
            /// создает новые координаты из координат GMap
            /// </summary>
            /// <param name="point">координаты Gmap</param>
            public CoordinateOld(PointLatLng point)
                : this(point.Lat, point.Lng) { }

            /// <summary>
            /// создает пустуые координаты
            /// </summary>
            /// <param name="isEmpty"></param>
            private CoordinateOld(bool isEmpty)
            {
                this.Latitude = new CoordinateRecord(true);
                this.Longitude = new CoordinateRecord(true);
                llat = long.MinValue;
                llon = long.MinValue;
            }

            #endregion


            /// <summary>
            /// чтение строкового представления координаты
            /// </summary>
            /// <param name="totalFormat">Вместо "lon" будет выполнен поиск
            /// долготы, вмеcто "lat" - широты</param>
            /// <param name="coordFormat">формат координат долготы и широты</param>
            /// <param name="source">исходная строка</param>
            /// <returns></returns>
            public static CoordinateOld Parse(string source, string totalFormat, string coordFormat)
            {
                string lat = "";
                string lon = "";

                //string[] tForm = Regex.Split(totalFormat, @"\w*lat\w*lon\w*");
                string[] tForm = Regex.Split(totalFormat, @"\w*(lat|lon)\w*");
                if (totalFormat.IndexOf("lat") < totalFormat.IndexOf("lon"))
                {
                    lat = source.Substring(tForm[0].Length, coordFormat.Length);
                    lon = source.Substring(tForm[0].Length + coordFormat.Length + 1, coordFormat.Length);
                }
                else
                {
                    lon = source.Substring(tForm[0].Length, coordFormat.Length);
                    lat = source.Substring(tForm[0].Length + coordFormat.Length + 1, coordFormat.Length);
                }


                return new CoordinateOld(CoordinateRecord.Parse(lat, coordFormat), CoordinateRecord.Parse(lon, coordFormat));
            }

            /// <summary>
            /// строковое представление пары координат
            /// </summary>
            /// <param name="totalFormat">общий формат пары координат. Вместо "{lon}" будет подставлено строковое представление
            /// долготы, вмеcто "{lat}" - широты</param>
            /// <param name="coordFormat">формат координат долготы и широты
            /// Hddmm.mmm     ddmm.mmm,H     Hdd mm.mmm       ddº mm.mmmm' H     ddº mm' ss.sss\" H     ddºmm'ss.s\"H       00.000000 </param>
            /// <returns></returns>
            public string ToString(string totalFormat, string coordFormat)
            {
                string ft = totalFormat.Replace("{lat}", "{0}");
                ft = ft.Replace("{lon}", "{1}");
                return string.Format(ft, Latitude.ToString(coordFormat), Longitude.ToString(coordFormat));
            }

            /// <summary>
            /// сохранение ссылки на google
            /// </summary>
            /// <returns></returns>
            public string ExportGoogle()
            {
                //https://www.google.ru/maps/place/55%C2%B040'51.6%22N+37%C2%B058'18.9%22E/@55.680696,37.9729728,17z/
                string ex = string.Format(@"https://www.google.ru/maps/place/{2}+{3}/@{0},{1},10z/",
                    this.Latitude.TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'),
                    this.Longitude.TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'),
                    this.Latitude.ToString("ddºmm'ss.s\"H"),
                    this.Longitude.ToString("ddºmm'ss.s\"H")
                    );
                return ex;
            }

            /// <summary>
            /// сохранение ссылки на яндекс
            /// </summary>
            /// <returns></returns>
            public string ExportYandex()
            {
                //https://yandex.ru/maps/213/moscow/?ll=37.598420%2C55.760155&z=10&z=17&mt=map&p=37.598420%2C55.760155&whatshere%5Bpoint%5D=37.679444%2C55.750087&whatshere%5Bzoom%5D=10
                string ex = string.Format(@"https://yandex.ru/maps/?ll={1}%2C{0}&z=10&z=17&mt=map&p={1}%2C{0}&whatshere%5Bpoint%5D={1}%2C{0}&whatshere%5Bzoom%5D=10", this.Latitude.TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'), this.Longitude.TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'));
                return ex;

            }


            /// <summary>
            /// object.Equals(obj)
            /// </summary>
            /// <param name="obj">объект-параметр</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (!(obj is CoordinateOld))
                    return false;

                if (obj == null)
                    return false;
                if (this.isEmpty)
                    return false;

                //return c1.Equals(c2);

                //return c1.strid == c2.strid;
                CoordinateOld d = (CoordinateOld)obj;
                return (this.Latitude.TotalDegrees == d.Latitude.TotalDegrees) && (this.Longitude.TotalDegrees == d.Longitude.TotalDegrees);
            }

            /// <summary>
            /// сравнение координат на равенство
            /// </summary>
            /// <param name="c1"></param>
            /// <param name="c2"></param>
            /// <returns></returns>
            public static bool operator ==(CoordinateOld c1, CoordinateOld c2)
            { return c1.Equals(c2); }

            /// <summary>
            /// сравнение координат на неравенство
            /// </summary>
            /// <param name="c1"></param>
            /// <param name="c2"></param>
            /// <returns></returns>
            public static bool operator !=(CoordinateOld c1, CoordinateOld c2)
            { return c1.Equals(c2); }
        }

        private class TrackFileOld : IList<TrackPointOld>, IEnumerable<TrackPointOld>, ICollection<TrackPointOld>
        {
            public TrackFileOld()
            { Track = new List<TrackPointOld>(); }

            private new List<TrackPointOld> Track;

            public Color Color { get; internal set; }
            public string Name { get; internal set; }
            public string Description { get; internal set; }

            public int Count => ((IList<TrackPointOld>)Track).Count;

            public bool IsReadOnly => ((IList<TrackPointOld>)Track).IsReadOnly;

            public TrackPointOld this[int index] { get => ((IList<TrackPointOld>)Track)[index]; set => ((IList<TrackPointOld>)Track)[index] = value; }

            internal TrackFile ToTrackFile()
            {
                TrackFile res = new TrackFile();
                foreach (var t in Track)
                    res.Add(t.ToTrackPoint());
                return res;
            }

            public int IndexOf(TrackPointOld item)
            {
                return ((IList<TrackPointOld>)Track).IndexOf(item);
            }

            public void Insert(int index, TrackPointOld item)
            {
                ((IList<TrackPointOld>)Track).Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                ((IList<TrackPointOld>)Track).RemoveAt(index);
            }

            public void Add(TrackPointOld item)
            {
                ((IList<TrackPointOld>)Track).Add(item);
            }

            public void Clear()
            {
                ((IList<TrackPointOld>)Track).Clear();
            }

            public bool Contains(TrackPointOld item)
            {
                return ((IList<TrackPointOld>)Track).Contains(item);
            }

            public void CopyTo(TrackPointOld[] array, int arrayIndex)
            {
                ((IList<TrackPointOld>)Track).CopyTo(array, arrayIndex);
            }

            public bool Remove(TrackPointOld item)
            {
                return ((IList<TrackPointOld>)Track).Remove(item);
            }

            public IEnumerator<TrackPointOld> GetEnumerator()
            {
                return ((IList<TrackPointOld>)Track).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IList<TrackPointOld>)Track).GetEnumerator();
            }
        }


        #endregion


        /// <summary>
        /// разделитель разделов информации, точек, маршртов
        /// </summary>
        public static readonly string separatorMain = "<section>";

        /// <summary>
        /// разделитель массива маршрутов и массива информации о маршрутах
        /// </summary>
        public static readonly string separatorDays = "<day_infos>";

        /// <summary>
        /// заголовок с версией для экспорта
        /// </summary>
        public static readonly string header = "Track Converter Trip File 2.0";

        /// <summary>
        /// структура для сериализации информации
        /// </summary>
        private class Info
        {
            /// <summary>
            /// для Json
            /// </summary>
            public Info()
            { }

            /// <summary>
            /// создает структуру из объекта  TrackFile
            /// </summary>
            /// <param name="trip"></param>
            public Info(TripRouteFile trip)
            {
                this.Name = trip.Name;
                this.Description = trip.Description;
                this.Distance = trip.Distance;
                this.KmphSpeed = trip.KmphSpeed;
                this.Color = trip.Color;
                this.Time = trip.Time;
            }

            /// <summary>
            /// преобразует структуру в TripRouteFile
            /// </summary>
            /// <returns></returns>
            public TripRouteFile ToTripRoute()
            {
                TripRouteFile res = new TripRouteFile
                {
                    Name = this.Name,
                    Description = this.Description,
                    Color = this.Color
                };
                return res;
            }

            public Color Color { get; set; }
            public string Description { get; set; }
            public double Distance { get; set; }
            public double KmphSpeed { get; set; }
            public string Name { get; set; }
            public TimeSpan Time { get; set; }
        }


        #region экспорт в строки

        public static string GetInformation(TripRouteFile trip)
        {
            Info info = new Info(trip);
            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            return json;
        }

        public static string GetRoutes(TripRouteFile trip)
        {
            string array = JsonConvert.SerializeObject(trip.DaysRoutes, Formatting.Indented);
            List<Info> infs = new List<Info>();
            foreach (TrackFile tf in trip.DaysRoutes)
                infs.Add(new Info()
                {
                    Color = tf.Color,
                    Time = tf.Time,
                    Description = tf.Description,
                    KmphSpeed = tf.KmphSpeed,
                    Distance = tf.Distance,
                    Name = tf.Name
                });
            string array_info = JsonConvert.SerializeObject(infs, Formatting.Indented);

            return array + "\r\n" + separatorDays + "\r\n" + array_info;
        }

        public static string GetWaypoints(TripRouteFile trip)
        {
            string json = JsonConvert.SerializeObject(trip.Waypoints, Formatting.Indented);
            return json;
        }

        #endregion

        #region десериализация из строк

        public static TripRouteFile GetInformation(string jsonInformation)
        {
            Info info = JsonConvert.DeserializeObject<Info>(jsonInformation.Trim(new char[] { '\r', '\n', ' ' }));
            return info.ToTripRoute();
        }

        public static TrackFileList GetRoutes(string jsonRoutes)
        {
            string[] data = Regex.Split(jsonRoutes, "w*" + separatorDays + "w*");

            if (data.Length != 2)
                throw new ApplicationException("Файл поврежден");

            //маршруты
            List<TrackFile> array = JsonConvert.DeserializeObject<List<TrackFile>>(data[0]);

            //информация
            List<Info> infs = JsonConvert.DeserializeObject<List<Info>>(data[1]);

            if (array.Count != infs.Count)
                throw new ApplicationException("В файле не хватает информации о дневных маршрутах");
            for (int i = 0; i < array.Count; i++)
            {
                array[i].Color = infs[i].Color;
                array[i].Name = infs[i].Name;
                array[i].Description = infs[i].Description;
                array[i].CalculateAll();
            }
            return new TrackFileList(array);
        }

        public static TrackFile GetWaypoints(string jsonWaypoints)
        {
            TrackFile wpts = JsonConvert.DeserializeObject<TrackFile>(jsonWaypoints);
            return wpts;
        }

        #region импорт из файлов версии 1.0

        internal static TrackFileList GetRoutes10(string jsonRoutes)
        {
            string[] data = Regex.Split(jsonRoutes, "w*" + separatorDays + "w*");

            if (data.Length != 2)
                throw new ApplicationException("Файл поврежден");

            //маршруты
            List<TrackFileOld> array = JsonConvert.DeserializeObject<List<TrackFileOld>>(data[0]);

            //информация
            List<Info> infs = JsonConvert.DeserializeObject<List<Info>>(data[1]);

            if (array.Count != infs.Count)
                throw new ApplicationException("В файле не хватает информации о дневных маршрутах");
            for (int i = 0; i < array.Count; i++)
            {
                array[i].Color = infs[i].Color;
                array[i].Name = infs[i].Name;
                array[i].Description = infs[i].Description;
                //array[i].CalculateAll();
            }

            TrackFileList res = new TrackFileList();
            foreach (var tf in array)
                res.Add(tf.ToTrackFile());


            return res;


        }

        internal static TrackFile GetWaypoints10(string jsonWaypoints)
        {
            TrackFileOld wpts = JsonConvert.DeserializeObject<TrackFileOld>(jsonWaypoints);

            TrackFile res = wpts.ToTrackFile();


            return res;
        }



        #endregion

        #endregion
    }
}
