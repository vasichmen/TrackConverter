using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using GMap.NET;
using Newtonsoft.Json;
using TrackConverter.Lib.Mathematic.Geodesy;

namespace TrackConverter.Lib.Tracking
{
    /// <summary>
    /// основные свойства треков и маршрутов
    /// </summary>
    public abstract class BaseTrack : IEnumerable<TrackPoint>
    {

        /// <summary>
        /// список точек
        /// </summary>
        protected List<TrackPoint> Track;

        /// <summary>
        /// длина маршрута в км или NaN, если длина еще не вычислена
        /// </summary>
        protected double distance = double.NaN;
        private bool lockDist = false;


        /// <summary>
        /// длина трека в километрах
        /// </summary>
        public virtual double Distance
        {
            get
            {
                //если расстояние задано через метод setDistance(double)
                //то возвращаемое значение то, которое установлено через метод. 
                //иначе, рассчитывается расстояние маршрута
                if (!lockDist)
                    return CalculateDistance();
                else
                    return
                        distance;
            }
        }

        /// <summary>
        /// описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// имя фыайла
        /// </summary>
        public string FileName { get { return string.IsNullOrEmpty(FilePath) ? null : Path.GetFileNameWithoutExtension(FilePath); } }

        /// <summary>
        /// адрес файла
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// формат
        /// </summary>
        public FileFormats Format
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.FilePath))
                    return FileFormats.Undefined;
                string ext = Path.GetExtension(this.FilePath);
                switch (ext.ToLower())
                {
                    case ".plt":
                        return FileFormats.PltFile;
                    case ".crd":
                        return FileFormats.CrdFile;
                    case ".wpt":
                        return FileFormats.WptFile;
                    case ".rt2":
                        return FileFormats.Rt2File;
                    case ".gpx":
                        return FileFormats.GpxFile;
                    case ".kml":
                        return FileFormats.KmlFile;
                    case ".kmz":
                        return FileFormats.KmzFile;
                    case ".osm":
                        return FileFormats.OsmFile;
                    case ".txt":
                        return FileFormats.TxtFile;
                    case ".nmea":
                        return FileFormats.NmeaFile;
                    case ".csv":
                        return FileFormats.CsvFile;
                    case ".adrs":
                        return FileFormats.AddressList;
                    case ".trr":
                        return FileFormats.TrrFile;
                    default:
                        throw new Exception("Данный файл не поддерживается: " + ext);
                }
            }
        }

        /// <summary>
        /// средняя скорост в вм/ч
        /// </summary>
        public double KmphSpeed { get { return this.Distance / this.Time.Hours; } }

        /// <summary>
        /// средняя скорость в узлах
        /// </summary>
        [JsonIgnore]
        public double KnotSpeed { get { return this.KmphSpeed / Constants.In1Knot; } }

        /// <summary>
        /// цвет маршрута
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// если истина, то маршрут виден на карте
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// только чтение
        /// </summary>
        public bool IsReadOnly { get; }

        #region описание абстрактных методов и свойств

        #region свойства

        /// <summary>
        /// чтение или запись точки в маршрут
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract TrackPoint this[int index] { get; set; }

        /// <summary>
        /// список всех высот в маршруте
        /// </summary>
        public abstract List<double> AllAltitudes { get; }

        /// <summary>
        /// список всех широт в маршруте
        /// </summary>
        public abstract List<double> AllLatitudes { get; }

        /// <summary>
        /// список всех долгот в маршруте
        /// </summary>
        public abstract List<double> AllLongitudes { get; }

        /// <summary>
        /// количество точек в маршруте
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// список точек в формате GMap
        /// </summary>
        public abstract List<PointLatLng> GMapPoints { get; }

        /// <summary>
        /// источник данных для вывода в таблицу
        /// </summary>
        public abstract DataTable Source { get; set; }

        /// <summary>
        /// время на весь маршрут
        /// </summary>
        public abstract TimeSpan Time { get; }

        #endregion

        #region методы

        /// <summary>
        /// добавить между всеми точками промежуточные по прямой с заданным расстоянием межу ними
        /// </summary>
        /// <param name="length">расстояние в метрах</param>
        /// <returns></returns>
        public abstract BaseTrack AddIntermediatePoints(double length);

        /// <summary>
        /// вычисление всех параметрой трека
        /// </summary>
        public abstract void CalculateAll();

        /// <summary>
        /// вычисление полной длины маршрута
        /// </summary>
        /// <returns></returns>
        protected abstract double CalculateDistance();

        /// <summary>
        /// очистка всего маршрута
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// удаление всех высот
        /// </summary>
        public abstract void ClearAltitudes();

        /// <summary>
        /// создает копию маршрута
        /// </summary>
        /// <returns></returns>
        public abstract BaseTrack Clone(bool addPoints = true);

        #region интерфейсы .NET


        /// <summary>
        /// добавить точку в конец
        /// </summary>
        /// <param name="point">точка</param>
        public abstract void Add(TrackPoint point);

        /// <summary>
        /// добавить список точек в конец
        /// </summary>
        /// <param name="points"></param>
        public abstract void Add(IEnumerable<TrackPoint> points);

        /// <summary>
        /// возвращает истину, если точка есть в маршруте
        /// </summary>
        /// <param name="point">точка</param>
        /// <returns></returns>
        public abstract bool Contains(TrackPoint point);

        /// <summary>
        /// вставить новый элемент на указанную позицию
        /// </summary>
        /// <param name="v">позиция</param>
        /// <param name="trackPoint">элемент</param>
        public abstract void Insert(int v, TrackPoint trackPoint);

        /// <summary>
        /// удалить точку
        /// </summary>
        /// <param name="newP"></param>
        /// <returns></returns>
        public abstract bool Remove(TrackPoint newP);

        /// <summary>
        /// удалить точку с указанным индексом
        /// </summary>
        /// <param name="selectedPointIndex"></param>
        /// <returns></returns>
        public abstract bool Remove(int selectedPointIndex);

        /// <summary>
        /// узнать индекс точки
        /// </summary>
        /// <param name="newP"></param>
        /// <returns></returns>
        public abstract int IndexOf(TrackPoint newP);

        /// <summary>
        /// сортировка
        /// </summary>
        public abstract void Sort();

        /// <summary>
        /// разделить маршрут на несколько маршрутов по minInterval км каждый
        /// </summary>
        /// <param name="minInterval"></param>
        /// <returns></returns>
        internal abstract TrackFileList Split(double minInterval);

        /// <summary>
        /// удалить точку с указанным индексом
        /// </summary>
        /// <param name="index">индекс</param>
        public abstract void RemoveAt(int index);

        /// <summary>
        /// копирование маршрута в массив
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public abstract void CopyTo(TrackPoint[] array, int arrayIndex);

        /// <summary>
        /// копирует часть маршрута в новый
        /// </summary>
        /// <param name="i">индекс первого элемента</param>
        /// <param name="length">количество элементов</param>
        /// <returns></returns>
        internal abstract TrackFile GetRange(int i, int length);

        #endregion

        #endregion

        #endregion

        #region реализация общих методов

        /// <summary>
        /// запись поля только для чтения Distance
        /// </summary>
        /// <param name="newDist">расстояние маршрута в километрах</param>
        public void setDistance(double newDist)
        {
            this.distance = newDist;
            this.lockDist = true;
        }

        /// <summary>
        /// перечислитель
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<TrackPoint> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// сортирует заданный массив путевых точек по порядку в маршруте 
        /// </summary>
        /// <param name="waypoints"></param>
        public TrackFile SortWaypoints(TrackFile waypoints)
        {
            TrackFile track = null;
            if (this.GetType() == typeof(TripRouteFile))
                track = ((TripRouteFile)this).TotalTrack;
            else
                track = (TrackFile)this;

            if (track == null || waypoints == null)
                throw new ArgumentNullException("waypoints или track == null при сортировке точек вдоль трека");
            if (track.Count == 0 || waypoints.Count == 0)
                return waypoints;

            track.CalculateAll();
            TrackFile res = new TrackFile();

            Dictionary<TrackPoint, TrackFile> dict = new Dictionary<TrackPoint, TrackFile>();
            foreach (TrackPoint wpt in waypoints)
            {
                //поиск ближайшей точки
                TrackPoint nearest = null;
                double lenMin = double.MaxValue;
                foreach (TrackPoint pt in track)
                {
                    double len = Vars.CurrentGeosystem.CalculateDistance(wpt, pt);
                    if (len < lenMin)
                    {
                        lenMin = len;
                        nearest = pt;
                    }
                }

                //добавление в результат
                if (nearest == null)
                    throw new Exception("Ошибка при поиске ближайшей точки");

                wpt.StartDistance = nearest.StartDistance;
                int ind = track.IndexOf(nearest);
                if (!dict.ContainsKey(nearest))
                    dict.Add(nearest, new TrackFile() { wpt });
                else
                    dict[nearest].Add(wpt);
            }

            foreach (TrackPoint tp in track)
                if (dict.ContainsKey(tp))
                    res.Add(dict[tp]);

            if (waypoints.Count != res.Count)
                throw new Exception("Ошибка, пропущена точка при поиске ближайшей");

            return res;

        }

        /// <summary>
        /// Ищет в маршруте точку с заданными координатами. Если маршрут типа TripRouteFile, то поиск будет идти по Waypoints. 
        /// Если точка не найдена, то результат null
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        internal TrackPoint Find(double lat, double lng)
        {
            BaseTrack trf=null;
            if (this.GetType() == typeof(TrackFile))
                trf = this;
            else if (this.GetType() == typeof(TripRouteFile))
                trf = ((TripRouteFile)this).Waypoints;

            foreach (TrackPoint tp in trf)
                if (tp.Coordinates.Latitude.TotalDegrees == lat && tp.Coordinates.Longitude.TotalDegrees == lng)
                    return tp;
            return null;
        }

        #endregion

    }
}