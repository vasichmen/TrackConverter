using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Mathematic.Routing.FullSearchElements;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing
{
    /// <summary>
    /// полный перебор всех вариантов 
    /// </summary>
    public class FullSearch : BaseLogist
    {
        /// <summary>
        /// матрица, представляющая расстояния между городами
        /// </summary>
        private  Graph graph;

        /// <summary>
        /// минимальный путь (в рекурсии используется для сравнения)
        /// </summary>
        private  int[] minWay = null;

        /// <summary>
        /// длина минимального пути (minWay)
        /// </summary>
        private  float minLength = float.MaxValue;

        /// <summary>
        /// Создает новый экземпляр класса с указанным действием вывода информации
        /// </summary>
        /// <param name="CallbackAction"></param>
        public FullSearch(Action<string> CallbackAction) : base(CallbackAction) { }


        /// <summary>
        /// построить маршрут полным перебором через указанные точки
        /// </summary>
        /// <param name="points">точки</param>
        /// <param name="start">начальная точка</param>
        /// <param name="isCycled">если истина, маршрут будет замкнутым</param>
        /// <returns></returns>
        public  TrackFile Make(TrackFile points, TrackPoint start, bool isCycled = false)
        {
            #region подготовка

            if (points.Count < 3)
                throw new ArgumentOutOfRangeException("waypoints.Count", "Количество точек должно быть не меньше трёх");

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: рассчет расстояний");

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
                    graph[i, j] = new Graph.Cell()
                    {
                        Length = lng,
                        From = points[i],
                        To = points[j],
                        Enabled = points[i] != points[j]
                    };
                }
            #endregion

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: построение маршрута");

            int[] nodes = new int[points.Count]; //узлы для первого шага(все узлы графа)
            for (int i = 0; i < points.Count; i++) nodes[i] = i; // запись всех узлов по порядку

            int[] result = FullSearchRec(nodes);

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: обработка резульататов");

            //заполнение результатов
            TrackFile res = ProcessResult(result,isCycled);
            return res;
        }

        /// <summary>
        /// Построение оптимального маршрута полным перебором.
        /// </summary>
        /// <param name="nodes">список узлов основного графа, через которые надо проложить маршрут</param>
        /// <returns></returns>
        private  int[] FullSearchRec(int[] nodes)
        {
            List<int> nodesList = new List<int>(nodes);
            int li = graph.RowCount;
            int lj = graph.ColCount;
            float[,] matr = new float[nodes.Length, nodes.Length];
            int y = 0;

            //копировние из исходной матрицы заданных узлов
            for (int i = 0; i < li; i++)
                if (nodesList.Contains(i))
                {
                    int x = 0;
                    for (int j = 0; j < lj; j++)
                        if (nodesList.Contains(j))
                        {
                            matr[y, x] = graph[i, j].Enabled ? (float)graph[i, j].Length : float.PositiveInfinity;
                            x++;
                        }
                    y++;
                }

            //путь в матрице matr полным перебором
            for (int i = 0; i < nodes.Length; i++)
                FullSearchRecStep(matr, new int[] { }, i, 0, nodes.Length);

            int[] res = minWay;

            //перевод в исходные узлы основной матрицы
            List<int> nodesSorted = new List<int>(nodes);

            //сортировка массива узлов по возрастанию
            nodesSorted.Sort((a, b) => { if (a > b) return 1; if (a < b) return -1; else return 0; });
            for (int i = 0; i < nodesSorted.Count; i++)
                res[i] = nodesSorted[res[i]];

            return res;
        }

        /// <summary>
        /// Обработка одного уровня рекурсии при полном переборе
        /// </summary>
        /// <param name="matrix">основная матриуца расстияний</param>
        /// <param name="baseway">уже готовый маршрут к этому уровню</param>
        /// <param name="next">следующий узел , обрабатываем на этом уровне</param>
        /// <param name="basewaylength">длина маршрута baseway</param>
        /// <param name="count">размер матрицы matrix</param>
        /// <returns>результат будет записан в RecursiveEnum.minWay</returns>
        private  int[] FullSearchRecStep(float[,] matrix, int[] baseway, int next, float basewaylength, int count)
        {
            //если длина частичного маршрута больше минимальной, то выходим и дальше не считаем
            if (basewaylength > minLength)
                return null;

            //обработка всех узлов матрицы, в которые можно папасть из текущего узла
            int[] newway = new int[baseway.Length + 1];
            for (int i = 0; i < baseway.Length; i++)
                newway[i] = baseway[i];
            newway[baseway.Length] = next;
            for (int i = 0; i < count; i++)
            {
                bool containsI = false;
                for (int g = 0; g < newway.Length; g++)
                    if (newway[g] == i) { containsI = true; break; }

                if (!containsI && i != newway.Length)
                {
                    int[] nbw = newway; //новый путь
                    int lastNode = nbw[nbw.Length - 1]; //узел, последний в текущем пути
                    float nbwl = basewaylength + matrix[lastNode, i]; //увеличиваем длину пути
                    FullSearchRecStep(matrix, nbw, i, nbwl, count);
                }
            }

            //расчет длины и сравнение с минимумом
            if (newway.Length == count) //если все точки использованы, то считаем длину и сравниваем с минимумом
            {
                float lg = basewaylength;
                lg += matrix[newway[newway.Length - 2], newway[newway.Length - 1]];
                if (lg < minLength)
                {
                    minWay = newway;
                    minLength = lg;
                }
                return null;
            }

            return null;
        }


    }
}
