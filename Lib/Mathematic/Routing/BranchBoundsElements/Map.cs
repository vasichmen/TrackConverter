namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    /// <summary>
    /// матрица ребер графа
    /// </summary>
    internal class Map
    {

        private readonly MapCell[,] map;

        /// <summary>
        /// создает матрицу указанного размера
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Map(int rows, int cols)
        {
            this.map = new MapCell[rows, cols];
        }

        /// <summary>
        /// возвращает ячейку по координатам
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <returns></returns>
        public MapCell this[int i, int j]
        {
            get { return this.map[i, j]; }
            set { this.map[i, j] = value; }
        }


    }
}
