using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing.FullSearchElements
{
    /// <summary>
    /// двумерный массив объектов
    /// </summary>
     class Graph
    {
        /// <summary>
        /// содержимое ячейк матрицы
        /// </summary>
        public class Cell
        {
            private double length;
            private bool enabled;

            public Cell() { }

            /// <summary>
            /// создает экземпляр ячейки из заданного 
            /// </summary>
            /// <param name="cell"></param>
            public Cell(Cell cell)
            {
                this.From = cell.From;
                this.Length = this.Enabled ? cell.Length : double.PositiveInfinity;
                this.To = cell.To;
                this.Enabled = cell.Enabled;
            }

            /// <summary>
            /// расстояние между точками
            /// </summary>
            public double Length { get { if (Enabled) return length; else throw new InvalidOperationException("Ячейка отключена!"); } set { length = value; } }

            /// <summary>
            /// точка отправления
            /// </summary>
            public TrackPoint From { get; set; }

            /// <summary>
            /// точка прибытия
            /// </summary>
            public TrackPoint To { get; set; }

            /// <summary>
            /// если истина, значит значение длины +бесконечность
            /// </summary>
            public bool Enabled { get { return enabled; } set { enabled = value; } }

            /// <summary>
            /// создает копию. ОЦЕНКИ НЕ КОПИРУЮТСЯ
            /// </summary>
            /// <returns></returns>
            public Cell Clone()
            {
                return new Cell()
                {
                    From = this.From,
                    Length = this.Enabled ? this.Length : double.PositiveInfinity,
                    Enabled = this.Enabled,
                    To = this.To,
                };
            }
        }

        private Cell[,] matrix;
        private int Cols;
        private int Rows;
        public Dictionary<TrackPoint, TrackPoint> selected;

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
        /// создает копию графа. При этом оценки не копируются
        /// </summary>
        /// <returns></returns>
        public Graph Clone()
        {
            Graph r = new Graph(this.RowCount, this.ColCount);

            for (int i = 0; i < this.RowCount; i++)
                for (int j = 0; j < this.ColCount; j++)
                    r[i, j] = this[i, j].Clone();
            return r;
        }
    }


}
