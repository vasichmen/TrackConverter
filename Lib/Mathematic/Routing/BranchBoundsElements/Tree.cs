using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    /// <summary>
    /// узел дерева 
    /// </summary>
    class Tree : IComparable, IEquatable<Tree>
    {
        private bool isAdmit;

        /// <summary>
        /// родительский узел
        /// </summary>
        public Tree Parent;

        /// <summary>
        /// дочерний узел, использующий ребро
        /// </summary>
        public Tree With;

        /// <summary>
        /// дочерний узел, не использующий ребро
        /// </summary>
        public Tree Without;

        /// <summary>
        /// номер строки из которой взято это ребро
        /// </summary>
        public int I;

        /// <summary>
        /// номер столбца, из которого взято это ребро
        /// </summary>
        public int J;

        /// <summary>
        /// граф на этом шаге
        /// </summary>
        public Graph Graph;

        /// <summary>
        /// нижняя граница 
        /// </summary>
        public float LowBound;

        /// <summary>
        /// уровень дерева
        /// </summary>
        public int Level;

        /// <summary>
        /// создает узел с заданным родителем и графом. После создания вычисляется нижняя граница
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="graph"></param>
        /// <param name="isAdmitting">если истина, то это узел множества "включающего" ребро деления. 
        /// Если ложь, то узел не включает ребро деления. 
        /// Если null, то это верхний элемент дерева</param>
        public Tree(Tree parent, Graph graph, bool isAdmitting)
        {
            this.Parent = parent;
            this.Graph = graph;
            this.isAdmit = isAdmitting;
            this.Level = parent!=null?parent.Level+1:0;

            //если это первый элемент дерева, то понижаем матрицу, если нет - смотрим на аргумент
            //понижаются элементы, которые добавляют ребро деления
            //if (graph != null)
            //{
                float privateBound = graph.CalculatePrivateLowBound(isAdmitting);
                this.LowBound = privateBound + ((parent != null) ? parent.LowBound : 0);
            //}
        }

        /// <summary>
        /// вычисление ребра деления на основе минимальной оценки
        /// </summary>
        public void CalcMark()
        {
            //ребро ветвления
            Point pt = Graph.GetMaxMark();
            this.I = pt.Y;
            this.J = pt.X;
        }

        /// <summary>
        /// очистка ненужных ресурсов при переходе узла из листьев в глубину дерева
        /// </summary>
        public void Free()
        {
            this.Graph = null;
        }


        #region реализации интерфейсов

        /// <summary>
        /// сравнивает ссылки объектов и возвращает истину при равенстве
        /// </summary>
        /// <param name="other">второй объект Tree</param>
        /// <returns></returns>
        public bool Equals(Tree other)
        {
            bool f = object.ReferenceEquals(this, other);
            return f;
        }

        /// <summary>
        /// Компаратор 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            //Для сортировки объектов по нарастающей конкретная реализация данного метода 
            //должна возвращать нулевое значение, если значения сравниваемых объектов равны;
            //положительное — если значение вызывающего объекта больше, чем у объекта другого other;
            //и отрицательное — если значение вызывающего объекта меньше, чем у другого объекта other.

            if (typeof(Tree) != obj.GetType())
                throw new ArgumentException("несовпадающие типы");

            Tree t = obj as Tree;
            if (this.Equals(t))
                return 0;

            int res;
            int f = this.LowBound.CompareTo(t.LowBound);
            if (f == 0)
                if (this.isAdmit & t.isAdmit)
                {
                    int f2 = -this.Graph.selectedIndexes.Count.CompareTo(t.Graph.selectedIndexes.Count);
                    if (f2 == 0)
                        res = 1;
                    else
                        res = f2;
                }
                else
                    res = this.isAdmit ? -1 : 1;
            else
                res = f;

            return res;
        }


        #endregion
    }
}
