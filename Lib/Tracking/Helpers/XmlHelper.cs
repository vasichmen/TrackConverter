using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// вспомогательные методы при работе с XML
    /// </summary>
   static class XmlHelper
    {
        /// <summary>
        /// получить первый элемент, LocalName которого совпадает с name. Регистр учитывается!
        /// Возвращает найденный элемент, если элемент не найден, то null
        /// </summary>
        /// <param name="root">корневой элемент, откуда начать поиск</param>
        /// <param name="name">имя искомого элемента</param>
        /// <param name="recursive">если истина, то поиск будет произведен и для вложенных элементов</param>
        /// <returns></returns>
        public static XmlNode GetChild(XmlNode root, string name, bool recursive = false)
        {
            foreach (XmlNode nd in root.ChildNodes)
            {
                if (nd.LocalName == name)
                    return nd;
                if (recursive && nd.HasChildNodes)
                {
                    XmlNode r = GetChild(nd, name, recursive);
                    if (r == null)
                        continue;
                    else return r;
                }
            }
            return null;
        }
    }
}
