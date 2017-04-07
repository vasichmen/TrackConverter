using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Mathematic.Routing.FullSearchElements
{
    class IndexArray
    {
        public int Length;
        public int[] array;
        private bool first;
        
        public double WayLength { get; set; }

        public int this[int index] { get { return array[index]; } }

        /// <summary>
        /// создает массив указанной длины
        /// </summary>
        /// <param name="length">количество строк в матрице (количество узлов в графе)</param>
        public IndexArray(int length)
        {
            this.Length = length;
            array = new int[length];
            for (int i = 0; i < length; i++)
                array[i] = i == length - 1 ? 0 : i + 1;
            this.first = true;
        }

        /// <summary>
        /// записывает в массив следующую комбинацию, удовлетворяющую условиям
        /// </summary>
        /// <returns></returns>
        public bool Next()
        {
            if (this.first)
            {
                this.first = false;
                return true;
            }

            //пока комбинация не соответствует условию существования пути
            bool f = true;
            while (f)
            {
                //начиная с конца прибавляем числа
                for (int i = Length - 1; i >= 0; i--)
                {
                    //если можно прибавить, то прибавляем к этой позиции и выход на проверку комбинации
                    if (array[i] < Length - 1)
                    {
                        array[i]++;

                        //если прибавили, то надо у всех правостоящих поставить 0
                        for (int j = i + 1; j < Length; j++)
                            array[j] = 0;

                        break;
                    }

                    //если ничего прибавить нельзя, то больше комбинаций нет инадо прекратить перебор
                    if (i == 0)
                        return false;
                }

                f = !IsAccords(); //если комбинация не подходит, то еще раз прибавляем
            }

            //если что-то прибавили, то продолжаем перебор
            return true;
        }

        /// <summary>
        /// возвращает истину, если указанная комбинация может существовать
        /// </summary>
        /// <returns></returns>
        private bool IsAccords()
        {
            //1. на позициях не могут стоять числа, равные индексу
            //2. все числа должны быть уникальны
            //3. не должно быть циклов

            //1 индексы
            bool res = true;
            for (int i = 0; i < this.Length; i++)
                res &= i != array[i];

            //если не подходит, то выход сразу
            if (!res) return res;

            //2 уникальность
            for (int i = 0; i < this.Length; i++)
            {
                int k = 0;
                //считаем элементы
                for (int j = i; j < this.Length; j++)
                    if (array[i] == array[j])
                        k++;
                res &= k <= 1; //если нашелся только один такой элемент (он сам), то все нормально
            }

            //если не подходит, то выход сразу
            if (!res) return res;

            //3 циклы
            //Dictionary<int, int> way = new Dictionary<int, int>();
            //for (int i = 0; i < this.Length; i++)
            //    way.Add(i, this[i]);

            //foreach (KeyValuePair<int, int> kv in way)
            //{
            //    int start = kv.Key; //начало всего маршрута из текущего ребра
            //    int current = kv.Value; //окончание текущего ребра

            //    Dictionary<int, int> cur = new Dictionary<int, int>();
            //    cur.Add(start, current); //добавление первого ребра в текущий маршрут


            //    //пока можно продолжить маршрут, добавляем ребра
            //    while (way.ContainsKey(current) && cur.Count < this.Length)
            //    {
            //        int newStart = current; //ищем начало следующего ребра
            //        current = way[current]; //конец следующего ребра
            //        if (cur.ContainsKey(newStart)) //если такой ключ уже есть, то комбинация не подходит
            //        {
            //            res &= false;
            //            return res;
            //        }
            //        cur.Add(newStart, current); //добавление ребра в текущий маршрут
            //    }

            //    // если есть цепочка, начинающаяся с этого ребра, длиной больше одного ребра
            //    if (cur.Count > 1)
            //    {
            //        //если длина цепочки равна числу вершин -1  (последняя вершина еще не добавлена)
            //        //то эта комбинация подходит
            //        if (cur.Count == this.Length)
            //        {
            //            res &= true;
            //            break;
            //        }
            //        else //если это все-таки преждевременное завершение пути, то  продолжаем поиск
            //        {
            //            res &= false;
            //            break;
            //        }
            //    }
            //}

            //если все ребра перебрали, значит нет ни одного маршрута длиной больше 1 ребра,
            //нет циклов,
            //нет результирующего маршрута, значит надо продолжить поиск
            return res;
        }


        /// <summary>
        /// копирование массива
        /// </summary>
        /// <returns></returns>
        public IndexArray CloneArray()
        {
            IndexArray res = new IndexArray(this.Length);
            res.WayLength = this.WayLength;
            for (int i = 0; i < this.Length; i++)
                res.array[i] = this[i];
            return res;
        }
    }

}
