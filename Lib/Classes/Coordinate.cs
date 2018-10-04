using GMap.NET;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace TrackConverter.Lib.Classes
{
    /// <summary>
    /// пара географических координат (широта, долгота)
    /// </summary>
    [Serializable]
#pragma warning disable CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
#pragma warning disable CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    public struct Coordinate
#pragma warning restore CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
#pragma warning restore CS0659 // Тип переопределяет Object.Equals(object o), но не переопределяет Object.GetHashCode()
    {
        #region классы, перечисления

        /// <summary>
        /// стороны света
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


        #endregion

        #region  поля, свойства

        private readonly bool NotEmpty;

        /// <summary>
        /// долгота
        /// </summary>
        public readonly double Longitude;

        /// <summary>
        /// широта
        /// </summary>
        public readonly double Latitude;

        /// <summary>
        /// координата в формате GMap
        /// </summary>
        [JsonIgnore]
        public PointLatLng GMap
        {
            get
            {
                return new PointLatLng(
                    this.Latitude,
                    this.Longitude);
            }

        }

        /// <summary>
        /// пустые координаты
        /// </summary>
        [JsonIgnore]
        public static Coordinate Empty { get { return default(Coordinate); } }

        /// <summary>
        /// если истина, то координата пуста
        /// </summary>
        [JsonIgnore]
        public bool isEmpty { get { return !NotEmpty; } }

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
        public Coordinate(int LatDeg, int LatMin, double LatSec, CoordinateChar LatChar, int LonDeg, int LonMin, double LonSec, CoordinateChar LonChar)
            : this(LatDeg + LatMin / 60d + LatSec / 3600d * ((LatChar == CoordinateChar.S || LatChar == CoordinateChar.W) ? -1 : 1),
                  LonDeg + LonMin / 60d + LonSec / 3600d * ((LonChar == CoordinateChar.S || LonChar == CoordinateChar.W) ? -1 : 1))
        { }

        /// <summary>
        /// создает новый экземпляр с указанными координатами
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public Coordinate(double lat, double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
            NotEmpty = true;
        }


        /// <summary>
        /// создает новый экземпляр с указанными координатами формата dd.dddddd
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public Coordinate(string lat, string lon)
            : this(double.Parse(lat), double.Parse(lon)) { }

        /// <summary>
        /// создает новые координаты из координат GMap
        /// </summary>
        /// <param name="point">координаты Gmap</param>
        public Coordinate(PointLatLng point)
            : this(point.Lat, point.Lng) { }

        #endregion


        /// <summary>
        /// возвращает целое число секунд в заданной координате
        /// </summary>
        /// <param name="kind">типа координаты</param>
        /// <returns></returns>
        public double Seconds(CoordinateKind kind)
        {
            double deg;
            switch (kind)
            {
                case CoordinateKind.Latitude: deg = Latitude; break;
                case CoordinateKind.Longitude: deg = Longitude; break;
                default: throw new Exception("О_о");
            }
            double mo = deg - (int)deg;
            double m = mo * 60;
            double so = m - ((int)m);
            double sec = so * 60;
            return Math.Abs(sec);
        }

        /// <summary>
        /// возвращает целое число минут в заданной координате
        /// </summary>
        /// <param name="kind">типа координаты</param>
        /// <returns></returns>
        public int Minutes(CoordinateKind kind)
        {
            double deg;
            switch (kind)
            {
                case CoordinateKind.Latitude: deg = Latitude; break;
                case CoordinateKind.Longitude: deg = Longitude; break;
                default: throw new Exception("О_о");
            }
            double mo = deg - (int)deg;
            int mm = (int)(mo * 60);
            return Math.Abs(mm);
        }

        /// <summary>
        /// возвращает целое число градусов в заданной координате
        /// </summary>
        /// <param name="kind">типа координаты</param>
        /// <returns></returns>
        public int Degrees(CoordinateKind kind)
        {
            switch (kind)
            {
                case CoordinateKind.Latitude: return Math.Abs((int)this.Latitude);
                case CoordinateKind.Longitude: return Math.Abs((int)this.Longitude);
                default: throw new Exception("О_о");
            }
        }

        /// <summary>
        /// возвращает сторону света для заданной координаты
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public CoordinateChar Char(CoordinateKind kind)
        {
            switch (kind)
            {
                case CoordinateKind.Latitude:
                    if (this.Latitude > 0)
                        return CoordinateChar.N;
                    else
                        return CoordinateChar.S;
                case CoordinateKind.Longitude:
                    if (this.Longitude > 0)
                        return CoordinateChar.E;
                    else
                        return CoordinateChar.W;
                default: throw new Exception("O_o");
            }
        }

        /// <summary>
        /// чтение строкового представления координаты
        /// </summary>
        /// <param name="totalFormat">Вместо "lon" будет выполнен поиск
        /// долготы, вмеcто "lat" - широты</param>
        /// <param name="coordFormat">формат координат долготы и широты</param>
        /// <param name="source">исходная строка</param>
        /// <returns></returns>
        public static Coordinate Parse(string source, string totalFormat, string coordFormat)
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


            return new Coordinate(parse(lat, coordFormat), parse(lon, coordFormat));
        }

        /// <summary>
        /// парсер одной координаты по заданному формату
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static double parse(string source, string format)

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

            double res = td * ((cc == CoordinateChar.S || cc == CoordinateChar.W) ? -1 : 1);
            return res;
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
        /// форматирование одной координаты в заданном формате
        /// </summary>
        /// <param name="kind">тип координаты</param>
        /// <param name="format">формат</param>
        /// <returns></returns>
        public string ToString(CoordinateKind kind, string format)
        {
            string res = "";
            double TotalDegrees;
            switch (kind)
            {
                case CoordinateKind.Latitude: TotalDegrees = Latitude; break;
                case CoordinateKind.Longitude: TotalDegrees = Longitude; break;
                default: throw new Exception("О_о");
            }


            switch (format)
            {
                case "Hddmm.mmm":
                    res = this.Char(kind).ToString() + Degrees(kind).ToString() + Math.Round((Math.Abs(TotalDegrees) - Degrees(kind)) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                    break;
                case "ddmm.mmm,H":
                    res = Degrees(kind).ToString() + Math.Round((Math.Abs(TotalDegrees) - Degrees(kind)) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.') + "," + this.Char(kind).ToString();
                    break;
                case "Hdd mm.mmm":
                    res = this.Char(kind).ToString() + Degrees(kind).ToString() + " " + Math.Round((Math.Abs(TotalDegrees) - Degrees(kind)) * 60, 3).ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                    break;
                case "ddº mm.mmmm' H":
                    res = this.Degrees(kind).ToString() + "º " + Math.Round((Math.Abs(TotalDegrees) - Degrees(kind)) * 60, 4).ToString("00.0000").Replace(Vars.DecimalSeparator, '.') + "' " + this.Char(kind).ToString();
                    break;
                case "ddº mm' ss.ssss\" H":
                    res = this.Degrees(kind).ToString() + "º " + this.Minutes(kind).ToString() + "' " + this.Seconds(kind).ToString("00.0000").Replace(Vars.DecimalSeparator, '.') + "\" " + this.Char(kind).ToString();
                    break;
                case "ddºmm'ss.s\"H":
                    res = this.Degrees(kind).ToString() + "º" + this.Minutes(kind).ToString() + "'" + this.Seconds(kind).ToString("00.0").Replace(Vars.DecimalSeparator, '.') + "\"" + this.Char(kind).ToString();
                    break;
                case "00.000000":
                    res = TotalDegrees.ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
                    break;
                case "00.000":
                    res = TotalDegrees.ToString("00.000").Replace(Vars.DecimalSeparator, '.');
                    break;
                case "dd, mm.mmmm,H":
                    res = this.Degrees(kind).ToString() + ", " + Math.Round((Math.Abs(TotalDegrees) - Degrees(kind)) * 60, 4).ToString("0.0000").Replace(Vars.DecimalSeparator, '.') + "," + this.Char(kind).ToString();
                    break;
                default:
                    throw new FormatException("Неподдерживаемый формат координат");
            }

            return res;
        }
        /// <summary>
        /// сохранение ссылки на google
        /// </summary>
        /// <returns></returns>
        public string ExportGoogle()
        {
            //https://www.google.ru/maps/place/55%C2%B040'51.6%22N+37%C2%B058'18.9%22E/@55.680696,37.9729728,17z/
            string ex = string.Format(@"https://www.google.ru/maps/place/{2}+{3}/@{0},{1},10z/",
                this.Latitude.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'),
                this.Longitude.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'),
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
            string ex = string.Format(@"https://yandex.ru/maps/?ll={1}%2C{0}&z=10&z=17&mt=map&p={1}%2C{0}&whatshere%5Bpoint%5D={1}%2C{0}&whatshere%5Bzoom%5D=10",
                this.Latitude.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'),
                this.Longitude.ToString("00.000000").Replace(Vars.DecimalSeparator, '.'));
            return ex;

        }

        /// <summary>
        /// object.Equals(obj)
        /// </summary>
        /// <param name="obj">объект-параметр</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Coordinate))
                return false;

            if (obj == null)
                return false;
            if (this.isEmpty)
                return false;

            //return c1.Equals(c2);

            //return c1.strid == c2.strid;
            Coordinate d = (Coordinate)obj;
            return (this.Latitude == d.Latitude) && (this.Longitude == d.Longitude);
        }

        /// <summary>
        /// сравнение координат на равенство
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator ==(Coordinate c1, Coordinate c2)
        { return c1.Equals(c2); }

        /// <summary>
        /// сравнение координат на неравенство
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool operator !=(Coordinate c1, Coordinate c2)
        { return c1.Equals(c2); }
    }

}