using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Routing.GreedyElements
{
    /// <summary>
    /// содержимое ячейк матрицы
    /// </summary>
    class Cell
    {
        private double length;
        private bool enabled;

        public Cell() { }

        /// <summary>
        /// создает экземпляр ячейки из заданного 
        /// </summary>
        /// <param name="cell"></param>
        public Cell(Cell cell) : this()
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
    }
}
