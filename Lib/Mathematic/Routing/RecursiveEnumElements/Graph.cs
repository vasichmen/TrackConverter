using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Routing.RecursiveEnumElements
{
    class Graph
    {
        public int Cols;
        public int Rows;
        private double[,] matrix;

        public double this[int i, int j] { get { return matrix[i, j]; } set { matrix[i, j] = value; } }

        public Graph(int Cols, int Rows)
        {
            this.Cols = Cols;
            this.Rows = Rows;
            matrix = new double[Rows, Cols];
        }

       

        internal List<List<double>> GetMatr()
        {
            List<List<double>> res = new List<List<double>>();
            for (int i = 0; i < Rows; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < Cols; j++)
                    row.Add(matrix[i, j]);
                res.Add(row);
            }
            return res;
        }
    }
}
