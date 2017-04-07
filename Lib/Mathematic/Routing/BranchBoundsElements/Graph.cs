using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    /// <summary>
    /// двумерный массив объектов
    /// </summary>
     class Graph
    {

        private int[,] matrix;

        /// <summary>
        /// выбранные ребра на этом этапе
        /// </summary>
        public Dictionary<int, int> selectedIndexes;

        /// <summary>
        /// количество строк
        /// </summary>
        public int RowCount;

        /// <summary>
        /// количество столбцов
        /// </summary>
        public int ColCount;



        /// <summary>
        /// задает или возвращает значение ячейки
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        public int this[int i, int j]
        {
            get { return matrix[i, j]; }
            set { matrix[i, j] = value; }
        }

        /// <summary>
        /// создает граф размером i строк и j столбцов
        /// </summary>
        /// <param name="RowCount">количество строк</param>
        /// <param name="ColCount">количество столбцов</param>
        /// <param name="selected">выбранные на данном этапе точки</param>
        public Graph(int RowCount, int ColCount, Dictionary<int, int> selected)
        {
            //тормозит выделение памяти под массив
            //this.matrix = StatVars.allocator.Get();
            this.matrix = new int[RowCount, ColCount];
            this.selectedIndexes = new Dictionary<int, int>(selected);
            this.ColCount = ColCount;
            this.RowCount = RowCount;
        }

        /// <summary>
        /// создает копию графа. При этом оценки не копируются
        /// </summary>
        /// <returns></returns>
        public Graph Clone()
        {
            Graph r = new Graph(this.RowCount, this.ColCount, this.selectedIndexes);

            r.selectedIndexes = new Dictionary<int, int>(this.selectedIndexes);

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    r[i, j] = this[i, j]/*.Clone()*/;
            return r;
        }

        /// <summary>
        /// нахождение минимального расстояния в заданном столбце
        /// </summary>
        /// <param name="j">номер столбца</param>
        /// <param name="withoutRow">номер строки, который будет исключен из сравнения</param>
        /// <returns></returns>
        public int GetMinCol(int j, int withoutRow = -1)
        {
            int min = int.MaxValue;
            for (int i = 0; i < this.RowCount; i++)
                if (this[i, j] >=0) //если точка активна
                    if (i != withoutRow)//если номер строки не исключен
                        if (this[i, j] < min)//сравнение
                            min = this[i, j];
            return min != int.MaxValue && min>0? min : 0;
        }

        /// <summary>
        /// нахождение минимального расстояния в заданной строке
        /// </summary>
        /// <param name="i">номер строки</param>
        /// <param name="withoutCol">номер столбца, который будет исвключен из сравнения</param>
        /// <returns></returns>
        public int GetMinRow(int i, int withoutCol = -1)
        {
            int min = int.MaxValue;
            for (int j = 0; j < this.ColCount; j++)
                if (this[i, j]>=0)//если точка активна
                    if (j != withoutCol)//если номер столбца не исключен
                        if (this[i, j] < min)//сравнение
                            min = this[i, j];
            return min != int.MaxValue && min>0 ? min : 0;
        }

        /// <summary>
        /// вычитание из каждого элемента минимального в текущеем столбце
        /// </summary>
        /// <param name="row"></param>
        public void ReductGraphCols(int[] row)
        {
            for (int j = 0; j < this.ColCount; j++)
                for (int i = 0; i < this.RowCount; i++)
                    if (matrix[i, j]>=0)
                        matrix[i, j] -= row[j];
        }

        /// <summary>
        /// вычитание из каждого элемента минимального в текущей строке
        /// </summary>
        /// <param name="col">минимальные элементы в строках</param>
        public void ReductGraphRows(int[] col)
        {
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    if (matrix[i, j]>=0)
                        matrix[i, j] -= col[i];
        }

        /// <summary>
        /// Опередляет ребро ветвления. 
        /// Вычисляет оценки нулевых точек и возвращает координаты максимальной оценки.
        /// х - номер столбца, у - номер строки
        /// </summary>
        /// <returns></returns>
        public Point GetMaxMark()
        {
            int maxmark = int.MinValue;
            Point pt = new Point();

            //обход всех элементов
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    if (this[i, j]>=0)
                        if (this[i, j] == 0)
                        {
                            //поиск минимального расстояния в строке
                            int minRow = this.GetMinRow(i, j);

                            //поиск минимального расстояния в столбце
                            int minCol = this.GetMinCol(j, i);

                            //записываем оценку в клетку
                            int Mark = minCol + minRow;

                            if (Mark > maxmark)
                            {
                                maxmark = Mark;
                                //на место у пишется номер строки, на место х - номер столбца
                                pt = new Point(j, i);
                            }
                        }
            if (!pt.IsEmpty)
                return pt;
            else throw new InvalidOperationException("Ошибка при поиске минимальной оценки в графе");
        }

        /// <summary>
        /// Опередляет ребро ветвления. 
        /// Вычисляет оценки нулевых точек и возвращает координаты минимальной оценки.
        /// х - номер столбца, у - номер строки
        /// </summary>
        /// <returns></returns>
        public Point GetMinMark()
        {
            int minmark = int.MaxValue;
            Point pt = new Point();

            //обход всех элементов
            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    if (this[i, j] >= 0)
                        if (this[i, j] == 0)
                        {
                            //поиск минимального расстояния в строке
                            int minRow = this.GetMinRow(i, j);

                            //поиск минимального расстояния в столбце
                            int minCol = this.GetMinCol(j, i);

                            //записываем оценку в клетку
                            int Mark = minCol + minRow;

                            if (Mark < minmark)
                            {
                                minmark = Mark;
                                //на место у пишется номер строки, на место х - номер столбца
                                pt = new Point(j, i);
                            }
                        }
            if (!pt.IsEmpty)
                return pt;
            else throw new InvalidOperationException("Ошибка при поиске минимальной оценки в графе");
        }

        /// <summary>
        /// вычисление собственной нижней границы для этой вершины и приведение матрицы по столюцам и строкам
        /// </summary>
        /// <param name="reduce">если истина, то при вычислении нижней границы матрица будет приведена по строкам и столбцам. 
        /// То есть будут вычтены минимумы из всех элементов</param>
        /// <returns></returns>
        public int CalculatePrivateLowBound(bool? reduce)
        {
            //double f = 1.7976931348623157E+308;
            double minBoundRoot = 0;

            {
                int[] col = new int[this.RowCount];
                //поиск минимальных в строках
                for (int i = 0; i < this.RowCount; i++)
                {
                    col[i] = this.GetMinRow(i);
                    //minBoundRoot += !double.IsInfinity(col[i]) && col[i] != f ? col[i] : 0;
                    minBoundRoot += col[i];
                }
                //вычитание минимумов из строк
                if (reduce != false)
                    this.ReductGraphRows(col);
            }

            {
                int[] row = new int[this.ColCount];
                for (int j = 0; j < this.ColCount; j++)
                {
                    row[j] = this.GetMinCol(j);
                    //minBoundRoot += !double.IsInfinity(row[j]) && row[j] != f ? row[j] : 0;
                    minBoundRoot += row[j];
                }
                //вычитание минимумов из столбцов
                if (reduce != false)
                    this.ReductGraphCols(row);
            }
            return (int)minBoundRoot;
        }

        /// <summary>
        /// Выбор заданного ребра и замена всех элементов в заданном столюце и заданой строке на бесконечность
        /// </summary>
        /// <param name="row">строка</param>
        /// <param name="col">столюец</param>
        public void SelectEdge(int row, int col)
        {
            this.selectedIndexes.Add(row, col);

            for (int i = 0; i < this.RowCount; i++)
                matrix[i, col]=-1;
            for (int j = 0; j < this.ColCount; j++)
                matrix[row, j]=-1;
        }

        /// <summary>
        /// запись в заданную ячейку бесконечности
        /// </summary>
        /// <param name="j"></param>
        /// <param name="i"></param>
        public void RemoveCell(int i, int j)
        {
            matrix[i, j]=-1;
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
                    cur.Add(newStart, current); //добавление ребра в текущий маршрут
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
