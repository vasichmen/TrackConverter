using System;
using System.Text.RegularExpressions;
using GMap.NET;
using Newtonsoft.Json;

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
        public static Coordinate Empty { get { return new Coordinate(true); } }

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
        public Coordinate(int LatDeg, int LatMin, double LatSec, CoordinateChar LatChar, int LonDeg, int LonMin, double LonSec, CoordinateChar LonChar)
            : this(new CoordinateRecord(LatDeg, LatMin, LatSec, LatChar), new CoordinateRecord(LonDeg, LonMin, LonSec, LonChar)) { }

        /// <summary>
        /// создает новый экземпляр с указанными координатами
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        public Coordinate(double lat, double lon)
            : this(new CoordinateRecord(lat, lat >= 0 ? Coordinate.CoordinateChar.N : Coordinate.CoordinateChar.S), new CoordinateRecord(lon, lon >= 0 ? Coordinate.CoordinateChar.E : Coordinate.CoordinateChar.W)) { }


        /// <summary>
        /// создает новый экземпляр с указанными координатами формата dd.dddddd
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">широта</param>
        public Coordinate(string lat, string lon)
            : this(double.Parse(lat.Replace('.', Vars.DecimalSeparator)), double.Parse(lon.Replace('.', Vars.DecimalSeparator))) { }

        /// <summary>
        /// создает новый экземпляр Coordinate с заданными координатами
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <exception cref="ArgumentNullException">Если долгота или широта равна null</exception>
        public Coordinate(CoordinateRecord lat, CoordinateRecord lon)
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
        public Coordinate(PointLatLng point)
            : this(point.Lat, point.Lng) { }

        /// <summary>
        /// создает пустуые координаты
        /// </summary>
        /// <param name="isEmpty"></param>
        private Coordinate(bool isEmpty)
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


            return new Coordinate(CoordinateRecord.Parse(lat, coordFormat), CoordinateRecord.Parse(lon, coordFormat));
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
            if (!(obj is Coordinate))
                return false;

            if (obj == null)
                return false;
            if (this.isEmpty)
                return false;

            //return c1.Equals(c2);

            //return c1.strid == c2.strid;
            Coordinate d = (Coordinate)obj;
            return (this.Latitude.TotalDegrees == d.Latitude.TotalDegrees) && (this.Longitude.TotalDegrees == d.Longitude.TotalDegrees);
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