using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Mathematic.Routing
{
    /// <summary>
    /// Прокладка маршрутов методом ветвей и границ. 
    /// Теория: http://kvckr.me/DM/DM28.html
    ///         http://5fan.ru/wievjob.php?id=4572
    /// Калькулятор: http://math.semestr.ru/kom/index.php
    ///              http://habr.x1site.ru/
    /// </summary>
    public class BranchBounds : BaseLogist
    {
        /// <summary>
        /// Создает новый экземпляр класса с указанным действием вывода информации
        /// </summary>
        /// <param name="CallbackAction"></param>
        public BranchBounds(Action<string> CallbackAction) : base(CallbackAction) { }

        /// <summary>
        /// прокладывает кратчайший маршрут через все заданные точки, посещая каждую точку только один раз. 
        /// Расстояния рассчитываются заданным в настройках методом Vars.Options.Converter.DistanceMethodType
        /// </summary>
        /// <param name="points">точки, которые надо посетиь</param>
        /// <param name="start">начальная точка</param>
        /// <param name="isCycled">Если истина то маршрут выводится замкнутым</param>
        /// <param name="roundMeters">минимальное расстояние в метрах, до которого идет округление при расчете расстояний</param>
        /// <returns></returns>
        public  TrackFile Make(TrackFile points, TrackPoint start, bool isCycled = false, int roundMeters = 10)
        {
            #region подготовка
            if (points.Count < 3)
                throw new ArgumentOutOfRangeException("waypoints.Count", "Количество точек должно быть не меньше трёх");
            if (isCycled)
            {
                start = null;
            }

            if (CallbackAction!=null)
            CallbackAction.Invoke("Построение оптимального маршрута: рассчет расстояний");
            routes = GetRoutes(points);

            int[,] matr = new int[points.Count, points.Count];

            //заполненние графа растояниями между точками
            //i - номер начальной точки, j - номер конечной точки
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points.Count; j++)
                {
                    int lng = 0;
                    if (i == j)
                        lng = -1;
                    else
                    {
                        //расстояние в метрах
                        float lngF = (float)routes[i][j].Distance * 1000;
                        //округление до 10 метров
                        float l1 = lngF / (float)roundMeters;
                        lng = (int)Math.Round(l1) * roundMeters;
                        matr[i, j] = lng;
                    }
                }


            int[] result = Make(matr, points.Count, points.IndexOf(start));

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: обработка результататов");

            TrackFile r = ProcessResult(result, isCycled);
            return r;
        }

        /// <summary>
        /// расчет кратчайшего пути методом ветвей и границ
        /// </summary>
        /// <param name="matrix">матрица расстояний</param>
        /// <param name="count">размер матрицы</param>
        /// <param name="start">номер начальной точки</param>
        /// <returns></returns>
        public int[] Make(int[,] matrix, int count, int start)
        {
            StatVars.graph = new Graph(count, count, new Dictionary<int, int>());
            StatVars.leaf = new BinaryTree<Tree>();

            //заполненние графа растояниями между точками
            //i - номер начальной точки, j - номер конечной точки
            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                {
                    float lng = matrix[i, j];

                    //заполнение новой ячейки
                    StatVars.graph[i, j] = i != j ? (int)lng : -1;
                }

            #endregion

            Directory.CreateDirectory(Application.StartupPath + Resources.temp_directory);
            StreamWriter sw = new StreamWriter(Application.StartupPath + Resources.temp_directory + "\\out.txt", false, Encoding.Default);

            if (CallbackAction != null)
                CallbackAction.Invoke("Построение оптимального маршрута: построение маршрута");

            //корень дерева
            Tree root = new Tree(null, StatVars.graph.Clone(), true);
            StatVars.leaf.Add(root);
            int edges = root.Graph.ColCount;
            Tree minBound = root;
            int level = 0;
            while (true)
            {
                #region описание алгоритма

                //1. осмотреть дерево, выбрать минимальную нижнюю границу в крайних узлах
                //2. разложить узел с минимальной нижней границей
                //   1. найти нулевую точку с максимальной оценкой(ребро ветвления)
                //   2. включение ребра:
                //      1. заменить строку и столбец выбранного ребра на бесконечность.
                //      2. заменить симметричное ребро на бесконечность
                //   3. исключение ребра:
                //      1. заменить только выбранное ребро на бесконечность
                //3. если глубина дерева достигла количества узлов графа, 
                //   то вычисление самой длинной ветви и запись в результат и выход из цикла

                #endregion

                level++;

                //поиск минимального элемента в листьях
                minBound = StatVars.leaf.MinValue; //узел с минимальной оценкой

                //если маршрут найден, то выход
                if (minBound.Graph.selectedIndexes.Count == edges )
                {
                    break;
                }
                minBound.CalcMark(); //вычисление ребра ветвления  
                bool f3 = StatVars.leaf.Remove(minBound);
                //добавление дочерних узлов к минимальной нижней границе

                int imax = minBound.I;
                int jmax = minBound.J;


                #region множество, включающее ребро ветвления (правый)

                Graph graphWith = minBound.Graph.Clone(); //копия графа для новой ветви
                graphWith.SelectEdge(imax, jmax); //удаляем строку и столбец выбранной ячейки
                graphWith.RemoveCell(jmax, imax); //удаляем обратный путь
                graphWith.RemoveCycles(); //удаляем (если есть) преждевременные циклы             
                Tree with = new Tree(minBound, graphWith, true);
                minBound.With = with; //добавление дочернего правого узла
                StatVars.leaf.Add(with); //добавление в список листьев

                #endregion

                #region  множество, НЕ включающее ребро ветвления (левый)

                Graph graphWithout = minBound.Graph; //второй раз граф можно не копировать т.к. старый все равно не нужен
                graphWithout.RemoveCell(imax, jmax); //удаление ячейки из графа
                Tree without = new Tree(minBound, graphWithout, false); //новый узел 
                minBound.Without = without; //добавление дочернего левого узла 
                StatVars.leaf.Add(without); //добавление листа

                #endregion

                #region  очистка

                minBound = null; //удаление элементов внутри дерева

                if (level % 10000 == 0)
                {
                    sw.WriteLine(
                        DateTime.Now.ToString() +
                        " Листьев: " + StatVars.leaf.Count +
                        " Мин. оценка: " + StatVars.leaf.MinValue.LowBound +
                        " на " + StatVars.leaf.MinValue.Level + " уровне дерева " +
                        " Макс. оценка: " + StatVars.leaf.MaxValue.LowBound +
                        " на " + StatVars.leaf.MaxValue.Level + " уровне дерева "
                    );
                    sw.Flush();
                }

                #endregion
            }

            sw.WriteLine(
                        DateTime.Now.ToString() +
                        " Листьев: " + StatVars.leaf.Count +
                        " Мин. оценка: " + StatVars.leaf.MinValue.LowBound +
                        " на " + StatVars.leaf.MinValue.Level + " уровне дерева " +
                        " Макс. оценка: " + StatVars.leaf.MaxValue.LowBound +
                        " на " + StatVars.leaf.MaxValue.Level + " уровне дерева "
                    );

            //обработка результатов
            int[] result = new int[count];


            //ОБРАБОТКА ПАР ТОЧЕК МАРШРУТА
            //добавление первой пары

            int oldP = start != -1 ? start : 0;
            result[0] = oldP;

            int k = 1;
            while (minBound.Graph.selectedIndexes.Count != 1)
            {
                int t = minBound.Graph.selectedIndexes[oldP];
                minBound.Graph.selectedIndexes.Remove(oldP);
                result[k] = t;
                k++;
                oldP = t;
            }

            //очистка ресурсов
            sw.Close();
            sw = null;
            root = null;
            minBound = null;
            StatVars.graph = null;
            StatVars.leaf.Clear();
            StatVars.leaf = null;
            GC.Collect();

            return result;
        }
    }
}
