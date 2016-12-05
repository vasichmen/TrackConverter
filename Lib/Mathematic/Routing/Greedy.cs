using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Mathematic.Routing.GreedyElements;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing
{
    /// <summary>
    /// "Жадный" алгоритм. 
    /// Выбирает каждый раз кратчайшее ребро.
    /// </summary>
    public class Greedy:BaseLogist
    {
        /// <summary>
        /// матрица, представляющая расстояния между городами
        /// </summary>
        private  Graph graph;

        /// <summary>
        /// Создает новый экземпляр класса с указанным действием вывода информации
        /// </summary>
        /// <param name="CallbackAction"></param>
        public Greedy(Action<string> CallbackAction) : base(CallbackAction) { }


        /// <summary>
        /// прокладывает кратчайший маршрут через все заданные точки, посещая каждую точку только один раз. 
        /// Расстояния рассчитываются заданным в настройках методом Vars.Options.Converter.DistanceMethodType
        /// </summary>
        /// <param name="points">точки, которые надо посетиь</param>
        /// <param name="start">начальная точка</param>
        /// <param name="isCycled">Если истина то маршрут выводится замкнутым</param>
        /// <returns></returns>
        public  TrackFile Make(TrackFile points, TrackPoint start, bool isCycled = false)
        {
            #region подготовка данных

            if (points.Count < 3)
                throw new ArgumentOutOfRangeException("waypoints.Count", "Количество точек должно быть не меньше трёх");

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: рассчет рсстояний");

            routes = GetRoutes(points);
            graph = new Graph(points.Count, points.Count);

            //заполненние графа растояниями между точками
            //i - номер начальной точки, j - номер конечной точки
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points.Count; j++)
                {
                    double lng = 0;
                    if (points[i] == points[j])
                        lng = double.PositiveInfinity;
                    else
                        //lng = Calc.CalculateDistance(points[i], points[j], Vars.Options.Converter.DistanceMethodType) / 1000;
                        lng = routes[i][j].Distance * 1000;

                    //заполнение новой ячейки
                    graph[i, j] = new Cell()
                    {
                        Length = lng,
                        From = points[i],
                        To = points[j],
                        Enabled = points[i] != points[j],
                    };
                }

            #endregion

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: построение маршрута");


            //алгоритм:
            //1. находим минимальное ребро в графе
            //2. выбираем его, удаляем строку, столбец
            //3. удаляем возможные циклы, проверяем на наличие результата
            //4. если нет резульата, то продолжаем поиск

            bool flag = true;
            while (flag)
            {
                Point ptmin = graph.GetMin();
                if (ptmin.IsEmpty)
                    throw new ApplicationException("В этом графе невозможно построить маршрут");
                int im = ptmin.Y;
                int jm = ptmin.X;
                TrackPoint from = graph[im, jm].From;
                TrackPoint to = graph[im, jm].To;
                graph.SelectEdge(im, jm);
                graph.RemoveCell(jm, im);
                flag = graph.RemoveCycles();
            }

            int[] result = new int[points.Count];
            int startPoint = 0;
            if (isCycled )
                startPoint = points.IndexOf(start);

            //ОБРАБОТКА ПАР ТОЧЕК МАРШРУТА
            //добавление первой пары
            result[0]=startPoint;
            int oldP = startPoint;

            int lim = isCycled ? 0 : 1;
            int k = 1;
            while (graph.selectedIndexes.Count != lim)
            {
                int t = graph.selectedIndexes[oldP];
                graph.selectedIndexes.Remove(oldP);
                result[k]=t;
                k++;
                oldP = t;
            }

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: обработка резульататов");

            TrackFile res = ProcessResult(result, isCycled);
            return res;
        }

      

    }
}
