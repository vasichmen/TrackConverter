using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using Newtonsoft.Json;
using TrackConverter.Lib.Mathematic.Geodesy;

namespace TrackConverter.Lib.Tracking
{
    /// <summary>
    /// структура данных для путешествия, сохраняется в файл trr
    /// </summary>
    public class TripRouteFile : BaseTrack, IEnumerable<TrackPoint>, IList<TrackPoint>
    {
        TrackFile totalTrack;


        /// <summary>
        /// создаёт новый объект путешествия
        /// </summary>
        public TripRouteFile()
        {
            Track = new List<TrackPoint>();
            this.DaysRoutes = new TrackFileList();
            this.Waypoints = new TrackFile();
        }

        /// <summary>
        /// весь трек
        /// </summary>
        public TrackFile TotalTrack { get { return totalTrack; } }
        private TrackFileList daysRoutes;

        /// <summary>
        /// длина трека в километрах
        /// </summary>
        public override double Distance
        {
            get
            {
                return CalculateDistance();
            }
        }

        /// <summary>
        /// список отрезков пути по дням
        /// </summary>
        public TrackFileList DaysRoutes
        {
            get { return daysRoutes; }
            set
            {
                daysRoutes = value;
                totalTrack = daysRoutes.JoinTracks();
            }
        }

        /// <summary>
        /// список путевых точек маршурта
        /// </summary>
        public TrackFile Waypoints { get; set; }

        /// <summary>
        /// создает маршрут всего путешествия для GMap
        /// </summary>
        /// <returns></returns>
        public MapRoute GetTotalMapRoute()
        {
            return new MapRoute(totalTrack.GMapPoints, this.Name);
        }

        /// <summary>
        /// дописать в таблицу маршруты по дням
        /// </summary>
        /// <param name="baseTable">таблица, в которой есть столбцы Название и Длина</param>
        /// <returns></returns>
        public DataTable GetDaysRoutesTable(DataTable baseTable)
        {
            foreach (TrackFile tf in this.DaysRoutes)
            {
                baseTable.LoadDataRow(
                    new object[] {
                        tf.Name,
                        tf.Distance
                    }
                    , true
                    );
            }
            return baseTable;
        }

        /// <summary>
        /// представление пуетвых точек в виде таблицы
        /// </summary>
        /// <param name="baseTable">базовая таблица с заполненными заголовками</param>
        /// <returns></returns>
        public DataTable GetWaypointsTable(DataTable baseTable)
        {
            foreach (TrackPoint tf in this.Waypoints)
            {
                baseTable.LoadDataRow(
                    new object[] {
                        tf.Name,
                        tf.PointTypeString
                    }
                    , true
                    );
            }
            return baseTable;
        }

        /// <summary>
        /// добавление нового дня в список и пересчет параметров
        /// </summary>
        /// <param name="day"></param>
        public void AddDay(TrackFile day)
        {
            daysRoutes.Add(day);
            totalTrack = daysRoutes.JoinTracks();
            totalTrack.CalculateAll();
        }

        /// <summary>
        /// удаление дня и перерасчет параметров
        /// </summary>
        /// <param name="selectedTrack"></param>
        public void RemoveDay(BaseTrack selectedTrack)
        {
            RemoveDay(daysRoutes.IndexOf(selectedTrack));
        }

        /// <summary>
        /// удаление дня и перерасчет параметров
        /// </summary>
        /// <param name="v">номер дня в списке</param>
        public void RemoveDay(int v)
        {
            daysRoutes.RemoveAt(v);
            totalTrack = daysRoutes.JoinTracks();
            totalTrack.CalculateAll();
        }

        /// <summary>
        /// перемещаение дня с обдной позиции на другую
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="newPos"></param>
        public void MoveDay(int oldPos, int newPos)
        {
            if (oldPos == newPos) return; //если двигать не надо, то выход
            if (oldPos > newPos) //если движение вверх
            {
                if (oldPos == 0) return; //двигать вверх нельзя, если это первый день
                TrackFile buf = (TrackFile)this.daysRoutes[oldPos];
                daysRoutes.Remove(buf);
                daysRoutes.Insert(newPos, buf);
                totalTrack = daysRoutes.JoinTracks();
                return;
            }
            if (oldPos < newPos) //если движение вниз
            {
                if (oldPos == daysRoutes.Count - 1) return; //двигать вниз нельзя, если это последний день
                TrackFile buf = (TrackFile)this.daysRoutes[oldPos];
                daysRoutes.Remove(buf);
                daysRoutes.Insert(newPos, buf);
                totalTrack = daysRoutes.JoinTracks();
                return;
            }
        }

        #region реализация базового класса BaseTrack

        /// <summary>
        /// количество точек во всех днях
        /// </summary>
        public override int Count
        {
            get
            {
                return totalTrack.Count;
            }
        }

        /// <summary>
        /// время на весь маршрут
        /// </summary>
        public override TimeSpan Time
        {
            get
            {
                return totalTrack.Time;
            }
        }

        /// <summary>
        /// возвращает список точек для представления в таблице
        /// </summary>
        [JsonIgnore]
        public override DataTable Source
        {
            get
            {
                return totalTrack.Source;

            }

            set
            {
                totalTrack.Source = value;
            }
        }

        /// <summary>
        /// список точек в формате gmap
        /// </summary>
        public override List<PointLatLng> GMapPoints
        {
            get
            {
                return this.totalTrack.GMapPoints;
            }
        }

        /// <summary>
        /// список высот
        /// </summary>
        public override List<double> AllAltitudes
        {
            get
            {
                return totalTrack.AllAltitudes;
            }
        }

        /// <summary>
        /// список широт
        /// </summary>
        public override List<double> AllLatitudes
        {
            get
            {
                return totalTrack.AllLatitudes;
            }
        }

        /// <summary>
        /// установить в указанном индексе указанный маршурт в днях
        /// </summary>
        /// <param name="ind">номер, по которому надо установить маршурт</param>
        /// <param name="tr"></param>
        public void setDayRoute(int ind, TrackFile tr)
        {
            daysRoutes[ind] = tr;
            totalTrack = daysRoutes.JoinTracks();
        }

        /// <summary>
        /// список долгот
        /// </summary>
        public override List<double> AllLongitudes
        {
            get
            {
                return totalTrack.AllLongitudes;
            }
        }


        /// <summary>
        /// задаёт или возвращает точку по индексу в маршруте
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override TrackPoint this[int index]
        {
            get
            {
                return totalTrack[index];
            }

            set
            {
                totalTrack[index] = value;

                //запись в маршурты по дням
                int i = 0;
                while (index > DaysRoutes[i].Count)
                {
                    index -= DaysRoutes.Count;
                    i++;
                }
                DaysRoutes[i][index] = value;
            }
        }


        /// <summary>
        /// добавление точки в конец маршута
        /// </summary>
        /// <param name="point"></param>
        public override void Add(TrackPoint point)
        {
            DaysRoutes[DaysRoutes.Count - 1].Add(point);
            totalTrack.Add(point);
        }

        /// <summary>
        /// добавление точeк в конец трека
        /// </summary>
        /// <param name="points"></param>
        public override void Add(IEnumerable<TrackPoint> points)
        {
            totalTrack.Add(points);
            DaysRoutes[DaysRoutes.Count - 1].Add(points);
        }

        /// <summary>
        /// добавление промежуточных точек в маршрут. При этом сам маршрут не изменяется
        /// </summary>
        /// <param name="length">максимальное расстояние между добавляемыми точками</param>
        /// <returns></returns>
        public override BaseTrack AddIntermediatePoints(double length)
        {
            return totalTrack.AddIntermediatePoints(length);
        }

        /// <summary>
        /// вычисление всех параметров маршрута
        /// </summary>
        public override void CalculateAll()
        {
            totalTrack.CalculateAll();
            this.Waypoints.CalculateAll();
            foreach (TrackFile tf in this.DaysRoutes)
                tf.CalculateAll();
        }


        /// <summary>
        /// вычисляет длину трека
        /// </summary>
        /// <returns></returns>
        protected override double CalculateDistance()
        {
            double d = 0;
            foreach (TrackFile tf in this.DaysRoutes)
                d += tf.Distance;
            return d;
        }

        /// <summary>
        /// очищает путешествие
        /// </summary>
        public override void Clear()
        {
            this.Waypoints.Clear();
            this.DaysRoutes.Clear();
            this.totalTrack.Clear();
        }

        /// <summary>
        /// удаляет высоты всех точек
        /// </summary>
        public override void ClearAltitudes()
        {
            this.totalTrack.ClearAltitudes();
            this.Waypoints.ClearAltitudes();
            foreach (TrackFile tf in DaysRoutes)
                tf.ClearAltitudes();
        }

        /// <summary>
        /// создаёт копию путешествия TripRouteFile
        /// </summary>
        /// <returns></returns>
        public override BaseTrack Clone(bool addPoints = true)
        {
            TripRouteFile res = new TripRouteFile();
            if (addPoints)
            {
                res.DaysRoutes = this.DaysRoutes.Clone();
                res.Waypoints = (TrackFile)this.Waypoints.Clone();
                res.totalTrack = (TrackFile)this.totalTrack.Clone();
            }
            res.Name = this.Name;
            res.Color = this.Color;
            res.Description = this.Description;
            res.FileName = this.FileName;
            res.FilePath = this.FilePath;
            return res;
        }

        /// <summary>
        /// возвращает истину,если точка есть в маршурте
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool Contains(TrackPoint point)
        {
            return totalTrack.Contains(point);
        }

        /// <summary>
        /// помещает trackPoint на индекс, равный v, сдвигая остальные точки вперед. Так же эта точка будет добавлена в соответствующий день
        /// </summary>
        /// <param name="v">номер, на который надо поместит новую точку</param>
        /// <param name="trackPoint">точка</param>
        public override void Insert(int v, TrackPoint trackPoint)
        {
            totalTrack.Insert(v, trackPoint);

            //добавление точки в нужный день
            int i = 0;
            while (v > DaysRoutes[i].Count)
            {
                v -= DaysRoutes.Count;
                i++;
            }
            DaysRoutes[i].Insert(v, trackPoint);
        }

        /// <summary>
        /// удаляет точку 
        /// </summary>
        /// <param name="newP"></param>
        /// <returns></returns>
        public override bool Remove(TrackPoint newP)
        {
            bool res = totalTrack.Remove(newP);
            foreach (BaseTrack bt in DaysRoutes)
                bt.Remove(newP);
            return res;
        }

        /// <summary>
        /// удаляет точку по индексу
        /// </summary>
        /// <param name="selectedPointIndex"></param>
        /// <returns></returns>
        public override bool Remove(int selectedPointIndex)
        {
            bool res = totalTrack.Remove(selectedPointIndex);
            foreach (BaseTrack bt in DaysRoutes)
                bt.Remove(selectedPointIndex);
            return res;
        }

        /// <summary>
        /// найти точку
        /// </summary>
        /// <param name="newP"></param>
        /// <returns></returns>
        public override int IndexOf(TrackPoint newP)
        {
            return totalTrack.IndexOf(newP);
        }

        /// <summary>
        /// сортирует весь маршрут и марушртфы по дням
        /// </summary>
        public override void Sort()
        {
            totalTrack.Sort();
            foreach (BaseTrack bt in DaysRoutes)
                bt.Sort();
        }

        /// <summary>
        /// разделяет маршрут на части по minInterval каждая, при этом поле DaysRoutes остаются неизменными
        /// </summary>
        /// <param name="minInterval">расстояние в метрах</param>
        /// <returns></returns>
        internal override TrackFileList Split(double minInterval)
        {
            return totalTrack.Split(minInterval);
        }

        /// <summary>
        /// удаляет точку по индексу
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index)
        {
            totalTrack.RemoveAt(index);
            foreach (BaseTrack bt in DaysRoutes)
                bt.RemoveAt(index);
        }

        /// <summary>
        /// копирование в массив
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public override void CopyTo(TrackPoint[] array, int arrayIndex)
        {
            totalTrack.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// возвращает часть трека
        /// </summary>
        /// <param name="i">индекс первой точки</param>
        /// <param name="length">количество копируемых точек</param>
        /// <returns></returns>
        internal override TrackFile GetRange(int i, int length)
        {
            return totalTrack.GetRange(i, length);
        }

        /// <summary>
        /// перечислитель
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TrackPoint>)this.totalTrack).GetEnumerator();
        }
        public override IEnumerator<TrackPoint> GetEnumerator()
        {
            return totalTrack.GetEnumerator();
        }


        #endregion


    }
}
