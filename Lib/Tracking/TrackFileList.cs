using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace TrackConverter.Lib.Tracking
{
    /// <summary>
    /// представление нескольких маршрутов
    /// </summary>
    public class TrackFileList: List<BaseTrack>
    {
        /// <summary>
        /// создает новый список маршрутов
        /// </summary>
        [JsonConstructor]
        public TrackFileList() { }

        /// <summary>
        /// создает новый список маршрутов и добавляет первый маршрут
        /// </summary>
        /// <param name="trackFile">маршрут для добавления в список</param>
        public TrackFileList(BaseTrack trackFile)
        {
            this.Add(trackFile);
        }

        /// <summary>
        /// создает новый список маршрутов на основе существующего
        /// </summary>
        /// <param name="list"></param>
        public TrackFileList(IEnumerable<BaseTrack> list)
        {
            this.Add(list);
        }

        /// <summary>
        /// Содержит список имен файлов, загруженных в список треков
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                List<string> res = new List<string>();
                foreach (BaseTrack tf in this)
                    if (!string.IsNullOrEmpty(tf.FilePath))
                        res.Add(tf.FilePath);
                return res;
            }
        }

        /// <summary>
        /// представление списка маршрутов для вывода в DataGridView
        /// </summary>
        /// <returns></returns>
        public DataTable Source
        {
            get
            {
                DataTable res = new DataTable();
                res.Columns.Add("Название");

                res.Columns.Add("Расстояние, км");
                res.Columns["Расстояние, км"].DataType = typeof(double);

                res.Columns.Add("Скорость, км/ч");
                res.Columns["Скорость, км/ч"].DataType = typeof(double);

                res.Columns.Add("Время");
                res.Columns["Время"].DataType = typeof(TimeSpan);

                res.Columns.Add("Количество точек");
                res.Columns["Количество точек"].DataType = typeof(int);

                res.Columns.Add("Цвет");
                res.Columns["Цвет"].DataType = typeof(string);

                foreach (BaseTrack tf in this)
                {
                    res.LoadDataRow(new object[] {
                    tf.Name,
                    tf.Distance.ToString("0.00"),
                    tf.KmphSpeed.ToString("0.00"),
                    tf.Time,
                    tf.Count,
                    ""
                }, true);
                }
                return res;
            }
        }

        /// <summary>
        /// Общее количество точек во всех маршрутах
        /// </summary>
        public int TotalPoints
        {
            get
            {
                int res = 0;
                foreach (BaseTrack tf in this)
                    res += tf.Count;
                return res;
            }
        }

        /// <summary>
        /// Возвращает количество точек в наиболее длинном треке этого списка
        /// </summary>
        /// <returns></returns>
        public int GetMaxTrack()
        {
            int res = 0;
            foreach (TrackFile tf in this)
                if (tf.Count > res)
                    res = tf.Count;
            return res;
        }

        /// <summary>
        /// объединение треков в один. Имя нового трека - сумма всех имёт треков. Цвет - цвет первого трека в списке
        /// </summary>
        /// <returns></returns>
        public TrackFile JoinTracks()
        {
            if (this.Count == 0)
                return new TrackFile();
            TrackFile res = new TrackFile();
            string name = "";
            foreach (BaseTrack tf in this)
            {
                foreach (TrackPoint tp in tf)
                    res.Add(tp);
                name += tf.Name;
            }
            res.Name = name;
            res.Color = this[0].Color;
            return res;
        }

        /// <summary>
        /// добавление маршрута в конец списка
        /// </summary>
        /// <param name="item">маршрут для добавления</param>
        public new void Add(BaseTrack item)
        {
            if (item == null)
                return;
            if (this.Contains(item))
                return;
            base.Add(item);
        }

        /// <summary>
        /// добавление списка маршрутов
        /// </summary>
        /// <param name="routes">список маршрутов для добавления</param>
        internal void Add(IEnumerable<BaseTrack> routes)
        {
            if (routes == null)
                throw new ArgumentNullException("Аргумент не существует");
            foreach (BaseTrack t in routes)
                this.Add(t);
        }

        /// <summary>
        /// создаёт копию списка треков
        /// </summary>
        /// <returns></returns>
        internal TrackFileList Clone()
        {
            TrackFileList res = new TrackFileList();
            foreach (BaseTrack bt in this)
                res.Add(bt.Clone());
            return res;
        }

        /// <summary>
        /// преобразование списка маршрутов в путешествие
        /// </summary>
        /// <returns></returns>
        public TripRouteFile ToTripRoute()
        {
            if (this.Count == 0)
                return new TripRouteFile();
            TripRouteFile res = new TripRouteFile(this, null)
            {
                Color = Vars.Options.Converter.GetColor(),
                FilePath = null,
                Name = this[0].Name
            };
            return res;
        }
    }
}
