using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing.GreedyElements
{
    /// <summary>
    /// двумерный массив объектов
    /// </summary>
    class Graph
    {
        /// <summary>
        /// выбранные элементы на этом уровне
        /// </summary>
        public Dictionary<TrackPoint, TrackPoint> selected;

        public Dictionary<int, int> selectedIndexes;

        private Cell[,] matrix;
        private int Cols;
        private int Rows;

        /// <summary>
        /// задает или возвращает значение ячейки
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        public Cell this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        /// <summary>
        /// создает граф размером i строк и j столбцов
        /// </summary>
        /// <param name="i">количество строк</param>
        /// <param name="j">количество столбцов</param>
        public Graph(int i, int j)
        {
            matrix = new Cell[i, j];
            selected = new Dictionary<TrackPoint, TrackPoint>();
            selectedIndexes = new Dictionary<int, int>();
            Cols = j;
            Rows = i;
        }

        /// <summary>
        /// количество строк
        /// </summary>
        public int RowCount { get { return Rows; } }

        /// <summary>
        /// количество столбцов
        /// </summary>
        public int ColCount { get { return Cols; } }

        /// <summary>
        /// нахождение координат минимального элемента
        /// <returns></returns>
        /// </summary>
        public Point GetMin()
        {
            Point res = new Point();
            double min = double.PositiveInfinity;
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    if (this[i, j].Enabled) //если точка активна
                        if (this[i, j].Length < min)//сравнение
                        {
                            min = this[i, j].Length;
                            res.X = j;
                            res.Y = i;
                        }
            return res;
        }

        /// <summary>
        /// Выбор заданного ребра и замена всех элементов в заданном столюце и заданой строке на бесконечность
        /// </summary>
        /// <param name="row">строка</param>
        /// <param name="col">столбец</param>
        public void SelectEdge(int row, int col)
        {
            selected.Add(matrix[row, col].From, matrix[row, col].To);
            selectedIndexes.Add(row, col);

            for (int i = 0; i < this.RowCount; i++)
                matrix[i, col].Enabled = false;
            for (int j = 0; j < this.ColCount; j++)
                matrix[row, j].Enabled = false;
        }

        /// <summary>
        /// удаление заданного ребра
        /// </summary>
        /// <param name="j"></param>
        /// <param name="i"></param>
        public void RemoveCell(int i, int j)
        {
            matrix[i, j].Length = double.PositiveInfinity;
            matrix[i, j].Enabled = false;
        }

        /// <summary>
        /// удаление ребра с указанными точками
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RemoveCell(TrackPoint from, TrackPoint to)
        {
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    if (matrix[i, j].From == from && matrix[i, j].To == to)
                        RemoveCell(i, j);
        }

        /// <summary>
        /// Удаление преждевременных циклов. 
        /// Возвращает истину, если циклы не найдены или удалены.  
        /// Возвращает ложь, если найден маршрут и надо прервать поиск.
        /// </summary>
        public bool RemoveCycles()
        {
            //в графе хранятся выбранные ребра.
            //проход по всем выбранным ребрам. 
            //Для каждой цепочки длиной > 2 ребер проверять, что удалено ребро: конец цепочки -> начало цепочки

            foreach (KeyValuePair<int, int> kv in this.selectedIndexes)
            {
                int start = kv.Key; //начало всего маршрута из текущего ребра
                int current = kv.Value; //окончание текущего ребра

                Dictionary<int, int> cur = new Dictionary<int, int>();
                cur.Add(start, current); //добавление первого ребра в текущий маршрут


                //пока можно продолжить маршрут, добавляем ребра
                while (this.selectedIndexes.ContainsKey(current) && cur.Count < this.ColCount)
                {
                    int newStart = current; //ищем начало следующего ребра
                    current = selectedIndexes[current]; //конец следующего ребра
                    cur.Add(newStart, current); //добавление ребра в текущий маршурт
                }

                // если есть цепочка, начинающаяся с этого ребра, длиной больше одного ребра
                if (cur.Count > 1)
                {
                    //если длина цепочки равна числу вершин -1  (последняя вершина еще не добавлена)
                    //то добавляем послденюю вершину и возвращаем результат
                    if (cur.Count == this.ColCount - 1)
                    {
                        this.selectedIndexes.Add(current, start); //добавление послен=днего ребра
                        return false; //выход из поиска маршрута
                    }
                    else //если это все-таки преждевременное завершение пути, то удаляем эту вершину и продолжаем поиск
                        this.RemoveCell(current, start); //удаление ребра из графа
                }
                // else return true;
            }

            //если все ребра перебрали, значит нет ни одного маршрута длиной больше 1 ребра,
            //нет циклов,
            //нет результирующего маршрута, значит надо продолжить поиск
            return true;
        }
    }

}
