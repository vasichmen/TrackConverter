using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.Local.OS
{
    /// <summary>
    /// кэш объектов в оперативной памяти
    /// </summary>
    public class MemoryCache 
    {
        Dictionary<string, DateTime> d;
        Dictionary<string, object> c;

        /// <summary>
        /// создаёт новый объект кэша в оперативной памяти
        /// </summary>
        public MemoryCache()
        {
            c = new Dictionary<string, object>();
            d = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// возвращает true, если объект с таким id есть в кэше
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public bool ContainsObject(string id)
        {
            return c.ContainsKey(id);
        }

        /// <summary>
        /// возвращает объект из кэша по id
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        public object GetObject(string id)
        {
            if (c.ContainsKey(id))
                return c[id];
            else
                return null;
        }

        /// <summary>
        /// добавляет объект в кэш. Возвращает false, если объект с таким is уже есть в кэше
        /// </summary>
        /// <param name="id">id объекта в кэше</param>
        /// <param name="obj">объект</param>
        /// <returns></returns>
        public bool PutObject(string id, object obj)
        {
            if (!c.ContainsKey(id))
            {
                c.Add(id, obj);
                d.Add(id, DateTime.Now);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// удалить все данные из кэша, созданные ранее указанной даты. Возвращает количество удалённых элементов
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int RemoveOlderThan(DateTime date)
        {
            List<string> list = new List<string>();
            foreach (var kv in d)
                if (kv.Value < date)
                    list.Add(kv.Key);
            foreach (var v in list)
                c.Remove(v);
            return list.Count;
        }
    }
}
