using GMap.NET.WindowsForms;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

namespace TrackConverter.Lib.Tracking
{
    /// <summary>
    /// представление нескольких маршрутов
    /// </summary>
    //[Serializable]
    public class TrackFileList : List<TrackFile>
    {
        /// <summary>
        /// создает новый список маршрутов
        /// </summary>
        public TrackFileList() { }

        /// <summary>
        /// создает новый список маршуртов и добавляет первый маршрут
        /// </summary>
        /// <param name="trackFile">маршрут для добавления в список</param>
        public TrackFileList(TrackFile trackFile)
        {
            this.Add(trackFile);
        }

        /// <summary>
        /// создает новый список маршрутов на основе существующего
        /// </summary>
        /// <param name="list"></param>
        public TrackFileList(IEnumerable<TrackFile> list)
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
                foreach (TrackFile tf in this)
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
                res.Columns["Количество точек"].DataType = typeof(Int32);

                res.Columns.Add("Цвет");
                res.Columns["Цвет"].DataType = typeof(string);

                foreach (TrackFile tf in this)
                {
                    res.LoadDataRow(new object[] {
                    tf.Name,
                    tf.Distance,
                    tf.KmphSpeed,
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
                foreach (TrackFile tf in this)
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
        /// объединение треков в один
        /// </summary>
        /// <returns></returns>
        public TrackFile JoinTracks()
        {
            TrackFile res = new TrackFile();
            foreach (TrackFile tf in this)
                foreach (TrackPoint tp in tf)
                    res.Add(tp);

            return res;
        }

        /// <summary>
        /// добавление маршрута в конец списка
        /// </summary>
        /// <param name="item">маршрут для добавления</param>
        public new void Add(TrackFile item)
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
        internal void Add(IEnumerable<TrackFile> routes)
        {
            if (routes == null)
                throw new ArgumentNullException("Аргумент не существует");
            foreach (TrackFile t in routes)
                this.Add(t);
        }
    }
}
