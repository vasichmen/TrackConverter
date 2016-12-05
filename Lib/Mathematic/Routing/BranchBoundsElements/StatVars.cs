using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    /// <summary>
    /// статические переменные для метода ветвей и границ
    /// </summary>
    static class StatVars
    {
        /// <summary>
        /// матрица, представляющая расстояния между городами
        /// </summary>
        public static Graph graph;

        /// <summary>
        /// Список листьев на данном этапе, отсортированный по возрастанию
        /// </summary>
        public static BinaryTree<Tree> leaf;
    }
}
