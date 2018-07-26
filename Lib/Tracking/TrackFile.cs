using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using GMap.NET;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic;
using TrackConverter.Lib.Mathematic.Geodesy;

namespace TrackConverter.Lib.Tracking
{
#pragma warning disable CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
#pragma warning disable CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    /// <summary>
    /// Трек
    /// </summary>
    public class TrackFile : BaseTrack, IList<TrackPoint>, IEnumerable<TrackPoint>, ICollection<TrackPoint>, IComparable
#pragma warning restore CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
#pragma warning restore CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
    {


        #region поля и свойства


        /// <summary>
        /// Возвращает или задает точку с указанным индексом
        /// </summary>
        /// <param name="index">отсчитываемый от нуля индекс точки</param>
        /// <returns></returns>
        public override TrackPoint this[int index]
        {
            get
            {
                return Track[index];
            }

            set
            {
                Track[index] = value;
            }
        }


        /// <summary>
        /// время прохождения трека
        /// </summary>
        public override TimeSpan Time
        {
            get
            {
                if (Track.Count <= 1)
                    return new TimeSpan(0);
                else
                    return Track[Track.Count - 1].Time - Track[0].Time;
            }
        }


        /// <summary>
        /// последовательность высот в метрах
        /// </summary>
        [JsonIgnore]
        public override List<double> AllAltitudes
        {
            get
            {
                List<double> res = new List<double>();
                foreach (TrackPoint tt in Track)
                    res.Add(tt.MetrAltitude);
                return res;
            }
        }

        /// <summary>
        /// последовательность долгот
        /// </summary>
        [JsonIgnore]
        public override List<double> AllLongitudes
        {
            get
            {
                List<double> res = new List<double>();
                foreach (TrackPoint tt in Track)
                    res.Add(tt.Coordinates.Longitude.TotalDegrees);
                return res;
            }
        }

        /// <summary>
        /// последовательность широт 
        /// </summary>
        [JsonIgnore]
        public override List<double> AllLatitudes
        {
            get
            {
                List<double> res = new List<double>();
                foreach (TrackPoint tt in Track)
                    res.Add(tt.Coordinates.Latitude.TotalDegrees);
                return res;
            }
        }

        /// <summary>
        /// количество точек в треке
        /// </summary>
        public override int Count { get { return Track.Count; } }

        /// <summary>
        /// возвращает список точек для представления в таблице
        /// </summary>
        [JsonIgnore]
        public override DataTable Source
        {
            get
            {
                DataTable res = new DataTable();

                res.Columns.Add("Название");
                res.Columns["Название"].DataType = typeof(string);
                res.Columns.Add("Широта, º");
                res.Columns["Широта, º"].DataType = typeof(double);
                res.Columns.Add("Долгота, º");
                res.Columns["Долгота, º"].DataType = typeof(double);
                res.Columns.Add("Высота, м");
                res.Columns["Высота, м"].DataType = typeof(double);
                res.Columns.Add("Описание");
                res.Columns["Описание"].DataType = typeof(string);
                res.Columns.Add("Время");
                res.Columns["Время"].DataType = typeof(DateTime);
                res.Columns.Add("Расстояние, км");
                res.Columns["Расстояние, км"].DataType = typeof(double);
                res.Columns["Расстояние, км"].ReadOnly = true;
                res.Columns.Add("Расстояние от старта, км");
                res.Columns["Расстояние от старта, км"].DataType = typeof(double);
                res.Columns["Расстояние от старта, км"].ReadOnly = true;
                res.Columns.Add("Истинный азимут, º");
                res.Columns["Истинный азимут, º"].DataType = typeof(double);
                res.Columns["Истинный азимут, º"].ReadOnly = true;
                res.Columns.Add("Магнитный азимут, º");
                res.Columns["Магнитный азимут, º"].DataType = typeof(double);
                res.Columns["Магнитный азимут, º"].ReadOnly = true;
                res.Columns.Add("Магнитное склонение, º");
                res.Columns["Магнитное склонение, º"].DataType = typeof(double);
                res.Columns["Магнитное склонение, º"].ReadOnly = true;



                foreach (TrackPoint tp in this)
                {
                    res.LoadDataRow(new object[] {
                    tp.Name,
                    tp.Coordinates.Latitude.TotalDegrees,
                    tp.Coordinates.Longitude.TotalDegrees,
                    Math.Round(tp.MetrAltitude==-777?double.NaN:tp.MetrAltitude,3),
                    tp.Description,
                    tp.Time,
                    Math.Round(tp.Distance,3),
                    Math.Round(tp.StartDistance,3),
                    tp.TrueAzimuth,
                    tp.MagneticAzimuth,
                    tp.MagneticDeclination
                }, true);
                }
                return res;

            }

            set
            {
                this.Clear();
                foreach (DataRow dr in value.Rows)
                {
                    double lat = dr["Широта, º"] is  DBNull ? 0 : ((double)dr["Широта, º"]);
                    double lon = dr["Долгота, º"] is DBNull ? 0 : ((double)dr["Долгота, º"]);

                    TrackPoint np = new TrackPoint(lat, lon);

                    //просто так почему-то высота не записываетс\я
                    object alt = dr.ItemArray[3];
                    np.MetrAltitude = (alt == null || alt is DBNull || double.IsNaN((double)alt)) ? -777 : (double)alt /* double.Parse(((string)alt).Replace('.',','))*/;

                    //np.MetrAltitude = dr["Высота, м"].GetType() == typeof(DBNull) ? -777 :((string)dr["Высота, м"]) == "не число"?-777: double.Parse(((string)dr["Высота, м"]).Replace('.', Vars.DecimalSeparator));
                    np.Description = dr["Описание"] is DBNull ? "" : (string)dr["Описание"];
                    np.Description.Replace(',', '_');
                    np.Name = (dr["Название"] is DBNull ? "" : (string)dr["Название"]).Replace(',', '_');
                    np.Description.Replace(',', '_');
                    np.KmphSpeed = 0;
                    np.Time = dr["Время"] is DBNull ? DateTime.MinValue : (DateTime)dr["Время"];
                    this.Add(np);
                }
                this.CalculateAll();
            }
        }

        /// <summary>
        /// точки в формате GMap
        /// </summary>
        [JsonIgnore]
        public override List<PointLatLng> GMapPoints
        {
            get
            {
                List<PointLatLng> gr = new List<PointLatLng>();
                foreach (TrackPoint tt in Track)
                    gr.Add(tt.Coordinates.GMap);

                return gr;
            }
        }

        #endregion

        #region конструкторы

        /// <summary>
        /// создает пустой объект TrackFile
        /// </summary>
        public TrackFile()
        {
            Track = new List<TrackPoint>();
            IsVisible = false;
        }

        /// <summary>
        /// создает объект TrackFile из заданного маршрута GMap
        /// </summary>
        /// <param name="route">маршрут GMap</param>
        public TrackFile(MapRoute route)
            : this()
        {
            if (route == null)
                return;

            foreach (PointLatLng pll in route.Points)
            {
                Track.Add(new TrackPoint(pll));
            }
        }

        /// <summary>
        /// создает маршрут из указанного списка точек
        /// </summary>
        /// <param name="points">список точек</param>
        public TrackFile(IEnumerable<TrackPoint> points)
            : this()
        {
            if (points == null)
                return;
            foreach (TrackPoint tt in points)
                Track.Add(tt);
        }

        #endregion

        #region вычисление параметров трека

        /// <summary>
        /// вычисление времени на весь трек, расстояния в км, средней скорости, скоростей в каждой точке, азумутов  в точках
        /// </summary>
        public override void CalculateAll()
        {
            CalculatePartSpeedsDistances();
            CalculateAzimuthsDeclination();
            foreach (TrackPoint tr in this)
                tr.CalculateParametres();
        }

        /// <summary>
        /// вычмсление азимутов в каждой точке
        /// </summary>
        private void CalculateAzimuthsDeclination()
        {
            for (int i = 0; i < this.Count - 1; i++)
            {
                this.Track[i].MagneticAzimuth = Math.Round(Vars.CurrentGeosystem.CalculateMagneticAzimuth(Track[i], Track[i + 1]), 3);
                this.Track[i].TrueAzimuth = Math.Round(Vars.CurrentGeosystem.CalculateTrueAzimuth(Track[i], Track[i + 1]), 3);
                this.Track[i].MagneticDeclination = Math.Round(Vars.CurrentGeosystem.CalculateMagneticDeclination(Track[i]), 3);
            }
        }

        /// <summary>
        /// расчет длины трека в километрах
        /// </summary>
        protected override double CalculateDistance()
        {
            double distance = 0;
            for (int i = 0; i < Track.Count - 1; i++)
            {
                double d = Vars.CurrentGeosystem.CalculateDistance(Track[i], Track[i + 1]);

                //если действительное число, то прибавляем
                distance += !double.IsNaN(d) ? d : 0;
            }
            distance = Math.Round(distance / 1000, 3);
            return distance;
        }

        /// <summary>
        /// Расчет скоростей и расстояний в каждой точке
        /// </summary>
        private void CalculatePartSpeedsDistances()
        {
            if (Track.Count <= 0) return;
            this.Track[0].StartDistance = 0;

            //начиная со второй точки считаем скорости
            for (int i = 1; i < this.Count; i++)
            {
                this.Track[i].Distance = Vars.CurrentGeosystem.CalculateDistance(this.Track[i - 1], this.Track[i]) / 1000.00;
                TimeSpan tm = this.Track[i].Time - this.Track[i - 1].Time;
                this.Track[i].KmphSpeed = this.Track[i].Distance / tm.TotalHours;
                this.Track[i].StartDistance = this.Track[i - 1].StartDistance + this.Track[i].Distance;
            }
        }


        #endregion

        #region операции с треком

        /// <summary>
        /// Добавление новой точки в конец трека
        /// </summary>
        /// <param name="point">новая точка</param>
        public override void Add(TrackPoint point)
        {
            if (point == null)
                return;
            Track.Add(point);
        }

        /// <summary>
        /// добавление нескольких точек в конец трека
        /// </summary>
        /// <param name="points"></param>
        public override void Add(IEnumerable<TrackPoint> points)
        {
            foreach (TrackPoint pt in points)
                this.Add(pt);
        }

        /// <summary>
        /// вставка точки на указанной позиции
        /// </summary>
        /// <param name="index">позиция для вставки</param>
        /// <param name="trackPoint">точка</param>
        public override void Insert(int index, TrackPoint trackPoint)
        {
            Track.Insert(index, trackPoint);
        }


        /// <summary>
        /// Возвращает истину, если указанная точка уже есть в треке
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool Contains(TrackPoint point)
        {
            return this.Track.Contains(point);
        }

        /// <summary>
        /// инвертировать трек. Переставить все точки в обратном порядке
        /// </summary>
        public TrackFile Invert()
        {
            if (this.Count == 0)
                return new TrackFile();
            TrackFile res = this.Clone(false) as TrackFile;
            res.Track.Clear();
            for (int i = this.Count - 1; i >= 0; i--)
                res.Add(this[i]);
            return res;
        }

        /// <summary>
        /// удаление всех точек из трека
        /// </summary>
        public override void Clear()
        {
            Track.Clear();
        }

        /// <summary>
        /// очистка высот всех точек
        /// </summary>
        public override void ClearAltitudes()
        {
            foreach (TrackPoint t in this)
                t.MetrAltitude = double.NaN;
        }

        /// <summary>
        /// создает копию маршрута
        /// </summary>
        /// <returns></returns>
        public override BaseTrack Clone(bool addPoints = true)
        {
            TrackFile res = new TrackFile();
            if (addPoints)
                foreach (TrackPoint t in this)
                    res.Add(t.Clone());
            res.Description = this.Description;
            res.FilePath = this.FilePath;
            res.Name = this.Name;
            res.Color = this.Color;
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// возвращает индекс заданного элемента
        /// </summary>
        /// <param name="trackPoint"></param>
        /// <returns></returns>
        public override int IndexOf(TrackPoint trackPoint)
        {
            return Track.IndexOf(trackPoint);
        }

        /// <summary>
        /// сортировка точек по полю Name
        /// </summary>
        public override void Sort()
        {
            Track.Sort();
        }

        /// <summary>
        /// получить отрезок маршрута с заданной позиции в метрах start длиной length метрах
        /// </summary>
        /// <param name="start">начало в метрах</param>
        /// <param name="length">длина отрезка в метрах. Для выбора всего оставшегося маршрута передать length=double.MaxValue</param>
        /// <returns></returns>
        public TrackFile Subtrack(double start, double length)
        {
            if (start > this.Distance * 1000) throw new ArgumentException("Начало отрезка больше длины маршрута");

            TrackPoint startPt = null;

            //ищем начальную точку (длина start)
            int startPos = -1;
            for (int i = 0; i < this.Count; i++)
                if (start < this[i].StartDistance * 1000)
                {
                    startPt = this[i - 1];
                    startPos = i - 1;
                    break;
                }

            if (startPos == -1)
                throw new ApplicationException();

            //ищем конечную точку (start + length)
            for (int i = startPos; i < this.Count; i++)
                if (this[i].StartDistance * 1000 - startPt.StartDistance * 1000 >= length)
                {
                    return this.Subtrack(startPos, i - 1); //выделяем и копируем маршрут
                }

            return this.Subtrack(startPos, this.Count - 1); //если коенчную точку не нашли, то копируем весь оставшийся маршрут
        }

        /// <summary>
        /// Возвращает отрезок маршрута между точками c указанными номерами (включительно)
        /// </summary>
        /// <param name="start">номер первой точки</param>
        /// <param name="end">номр последней точки</param>
        /// <returns></returns>
        public TrackFile Subtrack(int start, int end)
        {
            if (end < start) throw new ArgumentException("Неправильное начало или конец отрезка");
            TrackFile res = new TrackFile();
            for (int i = start; i <= end; i++)
                res.Add(this[i]);
            return res;
        }

        /// <summary>
        /// возвращает отрезок маршрута между указанными точками(включительно)
        /// </summary>
        /// <param name="start">первая точка</param>
        /// <param name="end">последняя точка</param>
        /// <returns></returns>
        public TrackFile Subtrack(TrackPoint start, TrackPoint end)
        { return Subtrack(this.IndexOf(start), this.IndexOf(end)); }

        /// <summary>
        /// разделяет маршрут на отрезки, меньшие или равные minInterval метров
        /// </summary>
        /// <param name="minInterval">длина отрезка в метрах</param>
        /// <returns></returns>
        internal override TrackFileList Split(double minInterval)
        {
            TrackFileList res = new TrackFileList();

            TrackPoint start = this[0];
            TrackPoint end = null;
            int si = this.IndexOf(start);
            int ei = 0;
            int i = 0;


            while (i < this.Count)
            {

                double dist = this[i].StartDistance - start.StartDistance;
                dist *= 1000;
                if (dist > minInterval)
                {
                    TrackFile leg = this.Subtrack(start, this[i]);

                    ei = i;
                    end = this[i];
                    res.Add(leg);

                    si = i;
                    start = this[i];
                }
                i++;
            }
            return res;
        }

        /// <summary>
        /// Добавляет в исходные трек промежуточные точки на расстоянии не менее length метров друг от друга
        /// </summary>
        /// <param name="length">расстояние в метрах между промежуточными точками в метрах</param>
        /// <returns>новый трек с добавленными точками</returns>
        public override BaseTrack AddIntermediatePoints(double length)
        {
            TrackFile res = new TrackFile();
            res.Name = this.Name;
            res.FilePath = this.FilePath;
            res.Description = this.Description;
            res.Color = this.Color;
            if (this.Count < 2)
                return this;

            if (this.Count == 2)
            {
                res.Add(this[0]);
                TrackFile npts = this[0].CalculateIntermediatePoints(this[1], length);
                res.Add(npts);
                return res;
            }

            for (int i = 0; i < this.Count - 2; i++)
            {
                res.Add(this[i]);
                TrackFile npts = this[i].CalculateIntermediatePoints(this[i + 1], length);
                res.Add(npts);
            }

            return res;
        }

        /// <summary>
        /// Возвращает точку в маршруте, которая ближе всего к заданной точке point
        /// </summary>
        /// <param name="point">заданная точка для поиска</param>
        /// <returns></returns>
        public TrackPoint GetNearestPoint(TrackPoint point)
        {
            double min = double.MaxValue;
            TrackPoint res = null;
            foreach (TrackPoint t in this)
            {
                double len = Vars.CurrentGeosystem.CalculateDistance(t, point);
                if (len < min)
                {
                    res = t;
                    min = len;
                }
            }
            return res;
        }

        /// <summary>
        /// преобразует TrackFile в TripRouteFile
        /// </summary>
        /// <returns></returns>
        public TripRouteFile ToTripRoute()
        {
            TripRouteFile res = new TripRouteFile();
            res.AddDay(this.Clone() as TrackFile);
            res.Name = this.Name;
            res.Description = this.Description;
            res.Color = this.Color;
            //FilePath не копируется
            return res;
        }

        /// <summary>
        /// возвращает часть маршрута
        /// </summary>
        /// <param name="i">индекс первой точки в маршруте</param>
        /// <param name="length">количество копируемых точек</param>
        /// <returns></returns>
        internal override TrackFile GetRange(int i, int length)
        {
            return new TrackFile(Track.GetRange(i, length));
        }

        #region операторы

        /// <summary>
        /// добавляет точку к треку
        /// </summary>
        /// <param name="tf">трек</param>
        /// <param name="tp">точка</param>
        /// <returns></returns>
        public static TrackFile operator +(TrackFile tf, TrackPoint tp)
        {
            if (tp == null & tf == null)
                return null;
            if (tp == null)
                return tf;
            if (tf == null)
                return new TrackFile() { tp };
            tf.Track.Add(tp);
            return tf;
        }

        /// <summary>
        /// объединяет два трека. Точки из f2 дописываются в конец трека f1
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static TrackFile operator +(TrackFile f1, TrackFile f2)
        {
            if (f2 == null & f1 == null)
                return null;
            if (f1 == null)
                return f2;
            if (f2 == null)
                return f1;
            foreach (TrackPoint tt in f2.Track)
                f1 += tt;
            return f1;
        }

        /// <summary>
        /// удаление точки из трека
        /// </summary>
        /// <param name="track">трек из которого будет удалена точка</param>
        /// <param name="point">точка, которая будет удалена</param>
        /// <returns></returns>
        public static TrackFile operator -(TrackFile track, TrackPoint point)
        {
            track.Track.Remove(point);
            return track;
        }

        /// <summary>
        /// удаление нескольких точек из трека
        /// </summary>
        /// <param name="t1">трек, откуда надо удалить точки</param>
        /// <param name="t2">список точек, кторые будут удалены</param>
        /// <returns></returns>
        public static TrackFile operator -(TrackFile t1, TrackFile t2)
        {
            foreach (TrackPoint pp in t2)
                t1 -= pp;
            return t1;
        }

        /// <summary>
        /// срвнение треков
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(TrackFile t1, TrackFile t2)
        {
            if (((object)t1) == null && ((object)t2) == null)
                return true;
            if (((object)t1) == null || ((object)t2) == null)
                return false;

            if (t1.Count != t2.Count)
                return false;


            bool res = true;
            for (int i = 0; i < t1.Count; i++)
                res &= t1[i] == t2[i];
            return res;
        }

        /// <summary>
        /// оператор !=
        /// </summary>
        /// <param name="t1">первый  трек</param>
        /// <param name="t2">второй трек</param>
        /// <returns></returns>
        public static bool operator !=(TrackFile t1, TrackFile t2)
        {
            return !(t1 == t2);
        }

        #endregion


        #region реализации интерфейсов .NET


        /// <summary>
        /// реализация интерфейса IEnumerable
        /// </summary>
        /// <returns></returns>
        IEnumerator<TrackPoint> IEnumerable<TrackPoint>.GetEnumerator()
        {
            return Track.GetEnumerator();
        }

        /// <summary>
        /// реализация интерфейса IEnumerable
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Track.GetEnumerator();
        }

        /// <summary>
        /// копирует чсть массива в другой массив
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public override void CopyTo(TrackPoint[] array, int arrayIndex)
        {
            Track.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// удаление указанного элемента
        /// </summary>
        /// <param name="point"></param>
        public override bool Remove(TrackPoint point)
        {
            if (point == null)
                return false;
            return Track.Remove(point);
        }

        /// <summary>
        /// удаление элемента с указанным индексом
        /// </summary>
        /// <param name="index">индекс удаляемой точки</param>
        public override bool Remove(int index)
        {
            if (index >= Track.Count)
                return false;
            return Remove(Track[index]);
        }

        /// <summary>
        /// сравнение с другим треком
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj);
        }

        /// <summary>
        /// удаление элемента с указаным индексом
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index)
        {
            Track.RemoveAt(index);
        }


        /// <summary>
        /// перечислитель
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<TrackPoint> GetEnumerator()
        {
            return Track.GetEnumerator();
        }


        /// <summary>
        /// показывает, является ли коллекция доступна только для чтения
        /// </summary>
        public new bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// если истина,то маршрут виден на карте
        /// </summary>
        public new bool IsVisible { get; set; }




        #endregion



        #endregion

    }
}