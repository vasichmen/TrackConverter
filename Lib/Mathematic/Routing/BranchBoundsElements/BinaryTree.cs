using System;
using System.Collections;
using System.Collections.Generic;

namespace TrackConverter.Lib.Mathematic.Routing.BranchBoundsElements
{
    /// <summary>
    /// бинарное дерево поиска. Хранит упоредоченную коллекцию элементов в иде дерева
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class BinaryTree<T> : ICollection<T>
    {
        /// <summary>
        /// узел дерева
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        protected class Node<TValue>
        {
            /// <summary>
            /// значение 
            /// </summary>
            public TValue Value
            {
                get;
                set;
            }

            /// <summary>
            /// значение, меньшее , чем Value
            /// </summary>
            public Node<TValue> Left
            {
                get;
                set;
            }

            /// <summary>
            ///  значение, большее, чемValue
            /// </summary>
            public Node<TValue> Right
            {
                get;
                set;
            }

            /// <summary>
            /// конструктор
            /// </summary>
            /// <param name="value"></param>
            public Node(TValue value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// первый элемент дерева
        /// </summary>
        protected Node<T> root;

        /// <summary>
        /// компаратор значений
        /// </summary>
        protected IComparer<T> comparer;

        /// <summary>
        /// создает деерво с компаратором по умолчанию
        /// </summary>
        public BinaryTree() : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// создает дерево с заданным компаратором
        /// </summary>
        /// <param name="defaultComparer"></param>
        public BinaryTree(IComparer<T> defaultComparer)
        {
            if (defaultComparer == null)
                throw new ArgumentNullException("Default comparer is null");
            comparer = defaultComparer;
        }

        /// <summary>
        /// создает дерево с заданными элементами
        /// </summary>
        /// <param name="collection"></param>
        public BinaryTree(IEnumerable<T> collection) : this(collection, Comparer<T>.Default)
        {

        }

        /// <summary>
        /// создает дерево с  заданными элементами и компаратором
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="defaultComparer"></param>
        public BinaryTree(IEnumerable<T> collection, IComparer<T> defaultComparer) : this(defaultComparer)
        {
            AddRange(collection);
        }

        /// <summary>
        /// минимальное значение в коллекции
        /// </summary>
        public T MinValue
        {
            get
            {
                if (root == null)
                    throw new InvalidOperationException("Tree is empty");
                var current = root;
                while (current.Left != null)
                    current = current.Left;
                return current.Value;
            }
        }

        /// <summary>
        /// максимальное значение в коллекции
        /// </summary>
        public T MaxValue
        {
            get
            {
                if (root == null)
                    throw new InvalidOperationException("Tree is empty");
                var current = root;
                while (current.Right != null)
                    current = current.Right;
                return current.Value;
            }
        }

        /// <summary>
        /// добавление нескольких элементов
        /// </summary>
        /// <param name="collection">коллекция добавляемых жлементов</param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var value in collection)
                Add(value);
        }

        public IEnumerable<T> Inorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            var node = root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    yield return node.Value;
                    node = node.Right;
                }
                else
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }
        public IEnumerable<T> Preorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                yield return node.Value;
                if (node.Right != null)
                    stack.Push(node.Right);
                if (node.Left != null)
                    stack.Push(node.Left);
            }
        }
        public IEnumerable<T> Postorder()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            var node = root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    if (stack.Count > 0 && node.Right == stack.Peek())
                    {
                        stack.Pop();
                        stack.Push(node);
                        node = node.Right;
                    }
                    else
                    {
                        yield return node.Value;
                        node = null;
                    }
                }
                else
                {
                    if (node.Right != null)
                        stack.Push(node.Right);
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }
        public IEnumerable<T> Levelorder()
        {
            if (root == null)
                yield break;

            var queue = new Queue<Node<T>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                yield return node.Value;
                if (node.Left != null)
                    queue.Enqueue(node.Left);
                if (node.Right != null)
                    queue.Enqueue(node.Right);
            }
        }

        #region ICollection<T> Members

        /// <summary>
        /// возвращает количество элементов  в коллекции
        /// </summary>
        public int Count
        {
            get;
            protected set;
        }

        /// <summary>
        /// коллекция только для чтения
        /// </summary>
        public bool IsReadOnly
        {
            get;
        }

        /// <summary>
        /// добавляет новый элемент в коллекцию
        /// </summary>
        /// <param name="item">Добавляемый элемент</param>
        public virtual void Add(T item)
        {
            var node = new Node<T>(item);

            if (root == null)
                root = node;
            else
            {
                Node<T> current = root, parent = null;

                while (current != null)
                {
                    parent = current;
                    if (comparer.Compare(item, current.Value) < 0)
                        current = current.Left;
                    else
                        current = current.Right;
                }

                if (comparer.Compare(item, parent.Value) < 0)
                    parent.Left = node;
                else
                    parent.Right = node;
            }
            ++Count;
        }

        /// <summary>
        /// удаляет указанный элемент из коллекции
        /// </summary>
        /// <param name="item">удаляемый элемент. Равенство элементов проверяется через компаратор!</param>
        /// <returns></returns>
        public virtual bool Remove(T item)
        {
            if (root == null)
                return false;

            Node<T> current = root, parent = null;

            int result;
            do
            {
                result = comparer.Compare(item, current.Value);
                if (result < 0)
                {
                    parent = current;
                    current = current.Left;
                }
                else if (result > 0)
                {
                    parent = current;
                    current = current.Right;
                }
                if (current == null)
                    return false;
            }
            while (result != 0);

            if (current.Right == null)
            {
                if (current == root)
                    root = current.Left;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = current.Left;
                    else
                        parent.Right = current.Left;
                }
            }
            else if (current.Right.Left == null)
            {
                current.Right.Left = current.Left;
                if (current == root)
                    root = current.Right;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = current.Right;
                    else
                        parent.Right = current.Right;
                }
            }
            else
            {
                Node<T> min = current.Right.Left, prev = current.Right;
                while (min.Left != null)
                {
                    prev = min;
                    min = min.Left;
                }
                prev.Left = min.Right;
                min.Left = current.Left;
                min.Right = current.Right;

                if (current == root)
                    root = min;
                else
                {
                    result = comparer.Compare(current.Value, parent.Value);
                    if (result < 0)
                        parent.Left = min;
                    else
                        parent.Right = min;
                }
            }
            --Count;
            return true;
        }

        /// <summary>
        /// удаляет все элементы
        /// </summary>
        public void Clear()
        {
            root = null;
            Count = 0;
        }

        /// <summary>
        /// копирует все элементы в массив, начиная с указанного индекса
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var value in this)
                array[arrayIndex++] = value;
        }


        /// <summary>
        /// возвращает истину, если указанный элемент есть в коллекции. Равенство проверяется с помощью компаратора
        /// </summary>
        /// <param name="item">элемент</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var current = root;
            while (current != null)
            {
                var result = comparer.Compare(item, current.Value);
                if (result == 0)
                    return true;
                if (result < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }
            return false;
        }
        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// перечислитель для этой коллекции
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Inorder().GetEnumerator();
        }
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// перечислитель для этой коллекции
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
