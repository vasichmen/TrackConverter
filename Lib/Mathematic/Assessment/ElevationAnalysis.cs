using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Assessment
{
    /// <summary>
    /// класс анализа горок в маршруте
    /// </summary>
    public static class ElevationAnalysis
    {
        /// <summary>
        /// информация о точке экстремума
        /// </summary>
        private struct PointInfo
        {

            /// <summary>
            /// географическая информация о точке
            /// </summary>
            public TrackPoint Point { get; set; }

            /// <summary>
            /// тип экстремума
            /// </summary>
            public ExtremeType Type { get; set; }

            /// <summary>
            /// координата х
            /// </summary>
            public double X { get { return Point.StartDistance; } }

            /// <summary>
            /// координата у
            /// </summary>
            public double Y { get { return Point.MetrAltitude; } }
        }

        /// <summary>
        /// тип экстремума: локальный минимум, локальный максимум
        /// </summary>
        private enum ExtremeType
        {
            /// <summary>
            /// локальный минимум 
            /// </summary>
            Minimum,
            /// <summary>
            /// локальный максимум
            /// </summary>
            Maximum
        }

        /// <summary>
        /// Построить таблицу сравнения маршрутов. 
        /// Столбцы - параметры, строки-маршруты
        /// </summary>
        /// <param name="Tracks">маршруты для сравнения</param>
        /// <returns></returns>
        public static DataTable AnalyzeTracks(TrackFileList Tracks)
        {
            foreach (TrackFile t in Tracks)
                if (t.AllAltitudes.Contains(double.NaN))
                    throw new ApplicationException("В маршрут " + t.Name + " не загружены высоты");

            DataTable res = new DataTable();

            res.Columns.Add("Название", typeof(string));
            res.Columns.Add("Длина, км", typeof(double));
            res.Columns.Add("Перепад высот, м", typeof(double)); //разница высот начала и конца маршрута

            //максимальное расстояние от центральной прямой 
            //до самой высокой или самой низкой точки маршрута
            res.Columns.Add("Отклонение по высоте, м", typeof(double));

            res.Columns.Add("Общая длина подъемов,км", typeof(double)); //общее расстояние движения в горку
            res.Columns.Add("Подъемы, %", typeof(double)); //доля подъемов

            res.Columns.Add("Общая длина спусков,км", typeof(double)); //общее расстояние движения в под горку
            res.Columns.Add("Спуски, %", typeof(double)); //доля спусков 
            res.Columns.Add("Максимальная крутизна подъема, º", typeof(double)); //самый большой наклон подъема в градусах
            res.Columns.Add("Максимальная крутизна спуска, º", typeof(double)); //самый большой наклон подъема в градусах

            Parallel.ForEach<BaseTrack>(Tracks, new Action<BaseTrack>((tf) =>
            {
                if (Vars.Options.Converter.IsApproximateAltitudes)
                    if (Vars.Options.Converter.ApproximateAmount >= tf.Count)
                        tf = Approximator.Approximate(tf, tf.Count - 10);
                    else
                        tf = Approximator.Approximate(tf, (int)Vars.Options.Converter.ApproximateAmount);

                DateTime start = DateTime.Now;
                double elevDivision = tf[tf.Count - 1].MetrAltitude - tf[0].MetrAltitude;
                double deviation = GetElevDeviation(tf);
                TimeSpan t1 = DateTime.Now - start;

                start = DateTime.Now;

                if (tf.Distance / tf.Count > Vars.Options.Converter.MinimumRiseInterval)
                    throw new ApplicationException("Для корректного сравнения маршрутов необходимо, чтобы минимальная длина горки была больше среднего расстояния между точками маршрута");
                List<PointInfo> extrems = GetExtremePoints(tf, Vars.Options.Converter.MinimumRiseInterval);

                //List<PointInfo> extrems = GetAllExtremePoints(tf);

                double riseLength = GetRiseLength(tf, extrems) / 1000.000;

                TimeSpan t2 = DateTime.Now - start;

                start = DateTime.Now;
                double risePart = (riseLength / tf.Distance) * 100.000;
                double maxRise = GetMaxRise(tf, extrems, AngleMeasure.Degrees);
                TimeSpan t3 = DateTime.Now - start;

                start = DateTime.Now;
                double maxDownhill = GetMaxDownhill(tf, extrems, AngleMeasure.Degrees);
                TimeSpan t4 = DateTime.Now - start;

                start = DateTime.Now;
                double downhillLength = GetDownhillLength(tf, extrems) / 1000.000;
                TimeSpan t5 = DateTime.Now - start;

                double downhillPart = (downhillLength / tf.Distance) * 100.000;

                lock (res)
                {
                    res.LoadDataRow(new object[] { 
                    tf.Name,  //имя
                    tf.Distance, //длина, км
                    elevDivision.ToString("00.000"), //перепад высот, м
                    deviation.ToString("00.000"), //отклонение по высоте, м
                    riseLength.ToString("00.000"), //длина подъемов, км
                    risePart.ToString("00.000"), //доля подъемов
                    downhillLength.ToString("00.000"), //длина спусков, км
                    downhillPart.ToString("00.000"), //доля спусков
                    maxRise.ToString("00.000"), //максимальный угол подъема
                    maxDownhill.ToString("00.000") //максимальный угол спуска
                }, true);
                }

            }));

            return res;
        }

        /// <summary>
        /// находит максимальное расстояние от центральной линии в графике высот (отклонение по высоте)
        /// </summary>
        /// <param name="route">маршрут с загруженными высотами</param>
        /// <returns></returns>
        private static double GetElevDeviation(BaseTrack route)
        {
            if (route.Count < 3) throw new ArgumentException("Длина маршрута должна быть не меньше трех точек");

            //1. построить центральную линию y=kx+b (найти коэффициенты k и b)
            //2. для каждой точки расчитать расстояние до центральной линии
            //3. выбрать максимальное расстояние от центральной линии

            //вычисление параметров трека
            route.CalculateAll();

            double k1, b1;

            #region коэффициенты центральной прямой kx+b
            {
                //ВСЕ КООРДИНАТЫ В МЕТРАХ
                //координаты начала трека
                double x1 = route[0].StartDistance * 1000; //обычно ноль
                double y1 = route[0].MetrAltitude;

                //координаты конца трека
                double x2 = route[route.Count - 1].StartDistance * 1000;
                double y2 = route[route.Count - 1].MetrAltitude;

                //угловой коэффициент центральнойц прямой
                k1 = (y2 - y1) / (x2 - x1); //наклон
                b1 = (y1 * x2 - x1 * y2) / (x2 - x1); //смещение по у
            }
            #endregion

            #region расчет расстояния до центральной прямой и поиск точки с максимальным расстоянием

            double max = double.MinValue;

            foreach (TrackPoint tp in route)
            {
                double lg;

                //y = k1*x+b1 // центральная прямая, коэффициенты расчитаны выше
                //x1,y1 - координаты точки на графике (расстояние от старта, высота)
                //x2,y2 - координаты точки пересечения перпендикуляра и центральной прямой

                //b2 - смещение по у перпендикуляра от точки к центральной прямой
                //k2 - угловой коэффициент перпендикуляра

                double x1 = tp.StartDistance * 1000;
                double y1 = tp.MetrAltitude;

                //параметры перпендикуляра
                double k2 = -1 / k1;
                double b2 = y1 + x1 / k1;

                //координаты точки пересечения перпендикуляра и центральной прямой
                double x2 = (b2 - b1) / (k1 - k2);
                double y2 = k1 * x2 + b1;

                //вцычисление расстояния от точки до центральной прямой
                //длина = корень((x2-x1)^2 + (y2-y1)^2)
                lg = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

                if (lg > max)
                    max = lg;
            }

            #endregion

            return max;
        }

        /// <summary>
        /// вычисление максимального угла подъема 
        /// </summary>
        /// <param name="tf">трек для вычисления</param>
        /// <param name="extrems">точки экстремума</param>
        /// <param name="Measure">единицы измерения углов</param>
        /// <returns></returns>
        private static double GetMaxRise(BaseTrack tf, List<PointInfo> extrems, AngleMeasure Measure)
        {
            double rad1deg = Math.PI / 180;

            //поиск максимального подъема
            Dictionary<int, int> climbs = GetClimbs(tf, extrems);

            //считаеются углы между прямой, соединяющей локальный минимум с локальным максимумом, и горизонтом

            double tan = double.MinValue;

            foreach (KeyValuePair<int, int> kv in climbs)
            {
                //вычисление угла подъема через тангенс
                double h = tf[kv.Value].MetrAltitude - tf[kv.Key].MetrAltitude; //высота конечной точки минус начальной
                double l = tf[kv.Value].StartDistance - tf[kv.Key].StartDistance; //расстояние от старта 
                l *= 1000; //перевод расстояния в метры
                double tan1 = h / l;

                if (tan1 > tan)
                    tan = tan1;
            }

            double F = Math.Atan(tan);

            switch (Measure)
            {
                case AngleMeasure.Degrees: return F / rad1deg;
                case AngleMeasure.Radians: return F;
                default: throw new ArgumentException("Данные единицы измерения углов не поддерживаются: " + Measure);
            }
        }

        /// <summary>
        /// вычисление максимального угла спуска 
        /// </summary>
        /// <param name="tf">маршрут</param>
        /// <param name="extrems">точки экстремума</param>
        /// <param name="Measure">единицы измерения углов</param>
        /// <returns></returns>
        private static double GetMaxDownhill(BaseTrack tf, List<PointInfo> extrems, AngleMeasure Measure)
        {
            double rad1deg = Math.PI / 180;

            //поиск максимального подъема
            Dictionary<int, int> downs = GetDownhills(tf, extrems);

            //считаеются углы между прямой, соединяющей локальный минимум с локальным максимумом и горизонтом

            double tan = double.MinValue;

            foreach (KeyValuePair<int, int> kv in downs)
            {
                //вычисление угла подъема через тангенс
                double h = tf[kv.Key].MetrAltitude-tf[kv.Value].MetrAltitude; //высота начальной точки минус конечной
                double l = tf[kv.Value].StartDistance - tf[kv.Key].StartDistance; //расстояние от старта 
                l *= 1000; //перевод расстояния в метры
                double tan1 = h / l;

                if (tan1 > tan)
                    tan = tan1;
            }

            double F = Math.Atan(tan);

            switch (Measure)
            {
                case AngleMeasure.Degrees: return F / rad1deg;
                case AngleMeasure.Radians: return F;
                default: throw new ArgumentException("Данные единицы измерения углов не поддерживаются: " + Measure);
            }
        }

        /// <summary>
        /// возвращает общую длину в м подъемов в треке
        /// </summary>
        /// <param name="tf">трек</param>
        /// <param name="extrems">точки экстремума</param>
        /// <returns></returns>
        private static double GetRiseLength(BaseTrack tf, List<PointInfo> extrems)
        {
            double res = 0;
            Dictionary<int, int> climbs = GetClimbs(tf, extrems);
            foreach (KeyValuePair<int, int> kv in climbs)
            {
                TrackPoint a = tf[kv.Key];
                TrackPoint b = tf[kv.Value];
                res += b.StartDistance - a.StartDistance;
            }

            return res * 1000.000;
        }

        /// <summary>
        /// вычисление общей длины в м спусков и ровных участков
        /// </summary>
        /// <param name="tf">маршрут для расчета</param>
        /// <param name="extrems">точки экстремума</param>
        /// <returns></returns>
        private static double GetDownhillLength(BaseTrack tf, List<PointInfo> extrems)
        {
            double res = 0;
            Dictionary<int, int> climbs = GetDownhills(tf, extrems);
            foreach (KeyValuePair<int, int> kv in climbs)
            {
                TrackPoint a = tf[kv.Key];
                TrackPoint b = tf[kv.Value];
                res += b.StartDistance - a.StartDistance;
            }

            return res * 1000.000;
        }

        /// <summary>
        /// Получить экстремальные точки на интервалах, равным minInterval метров.
        /// Расстояние между точками маршрута должно быть меньше minInterval.
        /// Перед использованием метода рекомендуется добавить промежуточные точки в маршрут и загрузить высоты
        /// </summary>
        /// <param name="leg">машрут для анализа</param>
        /// <param name="minInterval">заданный интервал в метрах, по достижении которого будут искаться мин и макс значения</param>
        /// <returns></returns>
        private static List<PointInfo> GetExtremePoints(BaseTrack leg, double minInterval)
        {
            if (leg.Count < 3) throw new ArgumentException("Длина маршрута должна быть не меньше трех точек");

            List<PointInfo> res = new List<PointInfo>();
            BaseTrack tf = leg.Clone();

            tf.CalculateAll();
            TrackFileList intervals = tf.Split(minInterval);

            foreach (TrackFile track in intervals)
            {
                List<double> alts = track.AllAltitudes;
                double min = alts.Min();
                double max = alts.Max();
                int imin = alts.IndexOf(min);
                int imax = alts.IndexOf(max);
                if (imin != 0 && imin != track.Count - 1)
                    res.Add(new PointInfo() { Point = track[imin], Type = ExtremeType.Minimum });
                if (imin != 0 && imin != track.Count - 1)
                    res.Add(new PointInfo() { Point = track[imax], Type = ExtremeType.Maximum });
            }

            //добавление первой точки
            if (tf[0].MetrAltitude < res[0].Y)
                res.Add(new PointInfo() { Point = tf[0], Type = ExtremeType.Minimum });
            else
                res.Add(new PointInfo() { Point = tf[0], Type = ExtremeType.Maximum });

            //добавление последней точки
            if (tf[tf.Count - 1].MetrAltitude < res[res.Count - 1].Y)
                res.Add(new PointInfo() { Point = tf[tf.Count - 1], Type = ExtremeType.Minimum });
            else
                res.Add(new PointInfo() { Point = tf[tf.Count - 1], Type = ExtremeType.Maximum });

            //упорядочивание по расстоянию от старта
            res.Sort(new Comparison<PointInfo>(pinfoComparer));

            //удаление парных максимумов и минимумов
            bool f = true;
            while (f)
            {
                f = false;
                for (int i = 0; i < res.Count - 1; i++)
                {
                    //если есть пара максимумов, то оставляем максимальный
                    if (res[i].Type == ExtremeType.Maximum && res[i + 1].Type == ExtremeType.Maximum)
                    {
                        if (res[i].Y > res[i + 1].Y)
                            res.RemoveAt(i + 1);
                        else
                            res.RemoveAt(i);
                        //i--;
                        f = true;
                    }

                    //если есть пара минимумов, то оставляем минимальный
                    if (res[i].Type == ExtremeType.Minimum && res[i + 1].Type == ExtremeType.Minimum)
                    {
                        if (res[i].Y < res[i + 1].Y)
                            res.RemoveAt(i + 1);
                        else
                            res.RemoveAt(i);
                        //i--;
                        f = true;
                    }
                }
            }

            return res;

            #region old

            //if (lebLg * 1000 > minInterval)
            //{
            //    res.AddRange(GetExtremePoints(leg.Subtrack(0, leg.Count/2), minInterval)); //первая половина пути
            //    res.AddRange(GetExtremePoints(leg.Subtrack(leg.Count / 2, leg.Count-1), minInterval)); //вторая половина пути

            //    //сортировка по возрастанию растояния от старта
            //    res.Sort(new Comparison<PointInfo>(pinfoComparer));

            ////удаление парных максимумов и минимумов
            //bool f = true;
            //while (f)
            //{
            //    f = false;
            //    for (int i = 0; i < res.Count - 1; i++)
            //    {
            //        //если есть пара максимумов, то оставляем максимальный
            //        if (res[i].Type == ExtremeType.Maximum && res[i + 1].Type == ExtremeType.Maximum)
            //        {
            //            if (res[i].Y > res[i + 1].Y)
            //                res.RemoveAt(i + 1);
            //            else
            //                res.RemoveAt(i);
            //            //i--;
            //            f = true;
            //        }

            //        //если есть пара минимумов, то оставляем минимальный
            //        if (res[i].Type == ExtremeType.Minimum && res[i + 1].Type == ExtremeType.Minimum)
            //        {
            //            if (res[i].Y < res[i + 1].Y)
            //                res.RemoveAt(i + 1);
            //            else
            //                res.RemoveAt(i);
            //            //i--;
            //            f = true;
            //        }
            //    }

            //    }
            //    return res;
            //}

            ////если отрезок меньше или равен минимальному, то ищем минимумы и максимумы
            //List<double> alts = leg.AllAltitudes;
            //double max = alts.Max();
            //double min = alts.Min();

            //int maxi = alts.IndexOf(max);
            //int mini = alts.IndexOf(min);

            //if (maxi != 0 && maxi != leg.Count - 1)
            //    res.Add(new PointInfo()
            //    {
            //        Point = leg[maxi],
            //        Type = ExtremeType.Maximum
            //    });

            //if (mini != 0 && mini != leg.Count - 1)
            //    res.Add(new PointInfo()
            //    {
            //        Point = leg[mini],
            //        Type = ExtremeType.Minimum
            //    });
            //return res;

            #endregion
        }
        private static int pinfoComparer(PointInfo x, PointInfo y)
        {
            if (x.X > y.X)
                return 1;
            else if (x.X < y.X)
                return -1;
            else return 0;
        }

        /// <summary>
        /// узнать все точки экстремума. В маршруте должно быть не меньше 3 точек.
        /// </summary>
        /// <param name="tf">маршрут для анализа</param>
        /// <returns></returns>
        private static List<PointInfo> GetAllExtremePoints(BaseTrack tf)
        {
            if (tf.Count < 3) throw new ArgumentException("Длина маршрута должна быть не меньше трех точек");


            List<PointInfo> res = new List<PointInfo>();

            if (tf[0].MetrAltitude > tf[1].MetrAltitude)
                res.Add(new PointInfo() { Type = ExtremeType.Maximum, Point = tf[0] });
            if (tf[0].MetrAltitude < tf[1].MetrAltitude)
                res.Add(new PointInfo() { Type = ExtremeType.Minimum, Point = tf[0] });

            for (int i = 1; i < tf.Count - 1; i++)
            {
                double y0 = tf[i - 1].MetrAltitude;
                double y1 = tf[i].MetrAltitude;
                double y2 = tf[i + 1].MetrAltitude;

                if (y1 == y2 && y1 == y0) continue; //если все три точки равны, то это не минимум и не максимум

                if (y0 <= y1 && y1 >= y2) //y1 - локальный максимум
                    res.Add(new PointInfo() { Point = tf[i], Type = ExtremeType.Maximum });

                if (y0 >= y1 && y1 <= y2) //y1 - локальный минимум
                    res.Add(new PointInfo() { Point = tf[i], Type = ExtremeType.Minimum });

            }

            if (tf[tf.Count - 1].MetrAltitude > tf[tf.Count - 2].MetrAltitude)
                res.Add(new PointInfo() { Type = ExtremeType.Maximum, Point = tf[tf.Count - 1] });
            if (tf[tf.Count - 1].MetrAltitude < tf[tf.Count - 2].MetrAltitude)
                res.Add(new PointInfo() { Type = ExtremeType.Minimum, Point = tf[tf.Count - 1] });

            return res;

        }

        /// <summary>
        /// найти диапазоны подъемов
        /// </summary>
        /// <param name="extrems">точки экстремума</param>
        /// <param name="tf">маршрут для поиска</param>
        /// <returns></returns>
        private static Dictionary<int, int> GetClimbs(BaseTrack tf, List<PointInfo> extrems)
        {
            Dictionary<int, int> res = new Dictionary<int, int>();
            for (int i = 0; i < extrems.Count - 1; i++)
            {
                //если данная точка экстремума - минимум, значит после нее идет горка. 
                //=>Следующая точка экстремума это максимум, значит записываем диапазон этой горки
                if (extrems[i].Type == ExtremeType.Minimum)
                {
                    int a = tf.IndexOf(extrems[i].Point);
                    int b = tf.IndexOf(extrems[i + 1].Point);
                    if (a == b)
                        continue;
                    res.Add(a, b);
                }
            }
            return res;
        }

        /// <summary>
        /// найти диапазоны спусков и ровных участков
        /// </summary>
        /// <param name="extrems">точки экстремума</param>
        /// <param name="tf">маршрут для поиска</param>
        /// <returns></returns>
        private static Dictionary<int, int> GetDownhills(BaseTrack tf, List<PointInfo> extrems)
        {
            Dictionary<int, int> res = new Dictionary<int, int>();
            for (int i = 0; i < extrems.Count - 1; i++)
            {
                //если данная точка экстремума - максимум, значит после нее идет спуск. 
                //=>Следующая точка экстремума это минимум, значит записываем диапазон этого спуска
                if (extrems[i].Type == ExtremeType.Maximum)
                {
                    int a = tf.IndexOf(extrems[i].Point);
                    int b = tf.IndexOf(extrems[i + 1].Point);
                    if (a == b)
                        continue;
                    res.Add(a, b);
                }
            }
            return res;

        }
    }
}
