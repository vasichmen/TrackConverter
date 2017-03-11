using System;
using System.Collections.Generic;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing
{
    /// <summary>
    /// Рекурсивный полный перебор
    /// https://habrahabr.ru/post/151151/
	/// https://habrahabr.ru/post/151954/
    /// </summary>
    public class PolarSearch : BaseLogist
    {
        /// <summary>
        /// начальная матрица расстояний
        /// </summary>
        int[,] graph;

        /// <summary>
        /// минимальный путь (в рекурсии используется для сравнения)
        /// </summary>
        private int[] minWay = null;

        /// <summary>
        /// длина минимального пути (minWay)
        /// </summary>
        private int minLength = int.MaxValue;

        private int startPointIndex = 0;
        private int endPointIndex = -1;

        /// <summary>
        /// Создает новый экземпляр класса с указанным действием вывода информации
        /// </summary>
        /// <param name="CallbackAction"></param>
        public PolarSearch(Action<string> CallbackAction) : base(CallbackAction) { }

        /// <summary>
        /// построить оптимальный маршрут
        /// </summary>
        /// <param name="waypoints">список точек</param>
        /// <param name="startPoint">начальная точка</param>
        /// <param name="endPoint">конечная точка</param>
        /// <param name="isCycled">если истина, то маршрут замкнутый</param>
        /// <param name="roundMeters">расстояние в м.  до которого округляется расстояние между точками</param>
        /// <returns></returns>
        public TrackFile Make(BaseTrack waypoints, TrackPoint startPoint, TrackPoint endPoint, bool isCycled, int roundMeters = 10)
        {
            #region подготовка

            if (waypoints.Count < 3)
                throw new ArgumentOutOfRangeException("waypoints.Count", "Количество точек должно быть не меньше трёх");
            if (isCycled)
            {
                startPoint = null;
                endPoint = null;
            }
            if (startPoint == endPoint && !isCycled)
                throw new ArgumentException("Начальная и конечная точки не могут совпадать.\r\nЕсли надо построить круговой маршрут, то используйте соответствующую функцию.");
            if (!isCycled)
            {
                startPointIndex = waypoints.IndexOf(startPoint);
                endPointIndex = waypoints.IndexOf(endPoint);
            }

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: рассчет расстояний");

            routes = GetRoutes(waypoints);
            graph = new int[waypoints.Count, waypoints.Count];

            //заполненние графа растояниями между точками
            //i - номер начальной точки, j - номер конечной точки
            for (int i = 0; i < waypoints.Count; i++)
                for (int j = 0; j < waypoints.Count; j++)
                {
                    int lng = 0;
                    if (i == j || i == endPointIndex || j == startPointIndex)
                        lng = -1;
                    else
                    {
                        //расстояние с использованием сервиса
                        float lngFR = (float)routes[i][j].Distance * 1000;

                        //округление до 10 метров
                        float l1 = lngFR / (float)roundMeters;
                        lng = (int)Math.Round(l1) * roundMeters;

                    }
                    //заполнение новой ячейки
                    graph[i, j] = i != j ? lng : -1;
                }

            #endregion

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: построение маршрута");

            //определение количества узлов для полного перебора
            int maxFullSearch;
            if (Vars.Options.Map.MaxFullSearchNodes > waypoints.Count)
                maxFullSearch = waypoints.Count;
            else
            {
                if (Vars.Options.Map.MaxFullSearchNodes < 2)
                    maxFullSearch = 2;
                else
                    maxFullSearch = Vars.Options.Map.MaxFullSearchNodes;
            }

            int[] nodes = new int[waypoints.Count]; //узлы для первого шага(все узлы графа)
            for (int i = 0; i < waypoints.Count; i++) nodes[i] = i; // запись всех узлов по порядку

            //построение пути через все точки
            int[] way = MakeStep(nodes, maxFullSearch);

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: обработка результататов");

            TrackFile res = ProcessResult(way, isCycled);

            //очистка ресурсов
            minWay = null;
            minLength = int.MaxValue;
            graph = null;
            endPointIndex = -1;
            startPointIndex = -1;
            routes = null;

            return res;
        }

        /// <summary>
        /// построить оптимальный маршрут в заданном графе. Возвращает последовательность узлов в оптимальном маршруте
        /// </summary>
        /// <param name="nodes">выбранные на данном этапе узлы</param>
        /// <param name="maxLength">максимальное количество узлов, которое можно обработать полным перебором</param>
        /// <returns>последовательность узлов в оптимальном маршруте</returns>
        private int[] MakeStep(int[] nodes, int maxLength)
        {
            //если мы достигли количества вершин, на котором возможен полный перебор, то считаем маршрут перебором
            if (nodes.Length <= maxLength)
                return FullSearchRec(nodes);

            //будем на новый уровень передавать 90% точек с наибольшим приоритетом из выбранных на предыдущем уровне
            int nlength = (int)(0.9d * nodes.Length); // округление всегда в меньшую сторону сторону, чтобы всегда уменьшалось количество узлов

            int[] grs = GetGroups(nodes, nlength); //узлы с наибольшим приоритетом на этом шаге

            //маршрут через узлы с наибольшим приоритетом на этом уровне
            //содеражит все узлы, выбранные в массив grs
            int[] res = MakeStep(grs, maxLength); //
            List<int> resList = new List<int>(res); ;

            //добавление остальных узлов (которые не попали в массив grs)
            //каждый узел добавляем в то ребро, где он меньше всего увеличит длину маршрута
            //добавление узлов из nodes, которых нет в grs, в маршрут res, с наименьшими потерями
            for (int i = 0; i < nodes.Length; i++)
            {
                //проверка существования узла в вбраных вершинах
                bool grsContainsNode = false;
                for (int j = 0; j < grs.Length; j++)
                    if (grs[j] == nodes[i])
                    {
                        grsContainsNode = true;
                        break;
                    }


                if (!grsContainsNode) //если узла nodes[i] нет в выбранных узлах, то надо его добавить в маршрут res
                {
                    //маршрут с уже добавленными некоторыми узлами
                    res = resList.ToArray();

                    //минимальное приращение длины 
                    int minIncLength = int.MaxValue;

                    //узел из res при вставлении в ребро, заканчивающееся этим узлом, приращение длины минамальное
                    int minNode = -1;

                    //перебор всех ребер в маршруте res и поиск подходящего для узла nodes[i]
                    for (int k = 1; k < res.Length; k++)
                    {
                        //приращение длины при добавлении к указанному узлу
                        // = расстояние через узел nodes[i] минус расстояние по прямой
                        int inc = graph[res[k - 1], nodes[i]] + graph[nodes[i], res[k]] - graph[res[k - 1], res[k]];
                        if (inc < minIncLength)
                        {
                            minIncLength = inc;
                            minNode = res[k];
                        }
                    }

                    //добавление узла в массив res в найденное ребро
                    resList.Insert(resList.IndexOf(minNode), nodes[i]);
                }
            }
            return resList.ToArray();
        }

        /// <summary>
        /// создать указанное количество узлов (групп), взаимодальних друг от остальных узлов 
        /// </summary>
        /// <param name="groupsCount">количество групп </param>
        /// <param name="nodes">узлы из которых надо выбирать самые приоритетные</param>
        /// <returns>список наиболее приоритетных узлов. Размер списка равен groupsCount</returns>
        private int[] GetGroups(int[] nodes, int groupsCount)
        {
            //массив со списком наиболее приоритетных узлов в этой матрице
            //чем дальше узел находится от остальных узлов, тем выше приоритет
            int[] res = new int[groupsCount];

            int from = 1; //индекс, с которого начинается заполнение массива res
            res[0] = startPointIndex; //первая точка пусть будет началом. Можно добавлять первую точку, которую выбрал пользователь

            if (endPointIndex > 0) //если есть конечная точка, то ее можно тоже сразу добавить
            {
                res[1] = endPointIndex;
                from = 2;
            }
            for (int i = from; i < groupsCount; i++) //добавление groupsCount групп. 
            {
                double max = double.MinValue;
                int maxNode = -1; //номер узла максимально удаленного от всех остальных
                for (int j = 0; j < nodes.Length; j++) // перебор всех узлов с предыдущего уровня(которые в nodes)
                {
                    int cur = nodes[j];

                    //поиск уже существующего элемента cur в результате
                    bool f = false;
                    for (int k = 0; k < i; k++)
                        if (res[k] == cur)
                        {
                            f = true;
                            break;
                        }
                    if (f) continue; //если есть, то переход к следующему узлу

                    //поиск минимального расстояния из всех расстояний между этим узлом и уже добавленными группами
                    double min = double.MaxValue;
                    for (int k = 0; k < i; k++)
                        if (graph[cur, res[k]] > 0 && graph[cur, res[k]] < min)
                            min = graph[cur, res[k]];

                    //если это расстояние больше уже найденного максимального, запоминаем
                    if (min > max)
                    {
                        max = min; //максимальное расстояние между узлом и другим
                        maxNode = cur; //номер максимального узла
                    }
                }
                res[i] = maxNode; //записываем наиболее отдаленый узел от всех уже выбранных узлов
            }
            return res;
        }

        /// <summary>
        /// Построение оптимального маршрута полным перебором.
        /// </summary>
        /// <param name="nodes">список узлов основного графа, через которые надо проложить маршрут</param>
        /// <returns></returns>
        private int[] FullSearchRec(int[] nodes)
        {
            List<int> nodesList = new List<int>(nodes);
            int li = graph.GetLength(0);
            int lj = graph.GetLength(1);
            int[,] matr = new int[nodes.Length, nodes.Length];

            //копировние из исходной матрицы заданных узлов
            int y = 0;
            for (int i = 0; i < li; i++)
                if (nodesList.Contains(i))
                {
                    int x = 0;
                    for (int j = 0; j < lj; j++)
                        if (nodesList.Contains(j))
                        {
                            matr[y, x] = graph[i, j];
                            x++;
                        }
                    y++;
                }

            //ПОСТРОЕНИЕ ПУТИ
            int[] res;
            if (!Vars.Options.Map.UseBranchBoundsInPolarSearch)//путь в матрице matr полным перебором
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (CallbackAction != null)
                        CallbackAction.Invoke("Построение оптимального маршрута через " + nodes.Length + " точек полным перебором: завершено " + ((i + 0.0d) / (nodes.Length + 0.0d) * 100d).ToString("0.0") + "%");
                    FullSearchRecStep(matr, new int[] { }, i, 0, nodes.Length);
                }
                res = (int[])minWay.Clone();
            }
            else //путь в матрице методом ветвей и границ
                res = new BranchBounds(null).Make(matr, nodes.Length, startPointIndex);

            if (res == null)
                throw new ApplicationException("Невозможно построить путь через заданные точки");

            //ОБРАБОТКА РЕЗУЛЬТАТОВ
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
        private int[] FullSearchRecStep(int[,] matrix, int[] baseway, int next, int basewaylength, int count)
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
                //проверка существования узла i в уже построенном маршруте
                bool containsI = false;
                for (int g = 0; g < newway.Length; g++)
                    if (newway[g] == i) { containsI = true; break; }

                if (!containsI && // если этого узла еще не было
                    //i != newway.Length && //и новый узел не попадет на индекс, равный своему номеру
                    matrix[newway[newway.Length - 1], i] > 0) //и есть путь из последнего выбранного узла в узел i
                {
                    int[] nbw = newway; //новый путь
                    int lastNode = nbw[nbw.Length - 1]; //узел, последний в текущем пути
                    int nbwl = basewaylength + matrix[lastNode, i]; //увеличиваем длину пути
                    FullSearchRecStep(matrix, nbw, i, nbwl, count); //обработка следующих вершин рекурсивно
                }
            }

            //расчет длины и сравнение с минимумом
            if (newway.Length == count) //если все точки использованы, то считаем длину и сравниваем с минимумом
            {
                int lg = basewaylength;
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
