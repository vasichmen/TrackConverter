using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.Local.OS
{
    //TODO: определения методов
    public class MemoryCache 
    {
        Dictionary<string, object> c;

        public MemoryCache()
        {
            c = new Dictionary<string, object>();
        }

        public bool ContainsObject(string id)
        {
            return c.ContainsKey(id);
        }

        public object GetObject(string id)
        {
            if (c.ContainsKey(id))
                return c[id];
            else
                return null;
        }

        public bool PutObject(string id, object obj)
        {
            if (!c.ContainsKey(id))
            {
                c.Add(id, obj);
                return true;
            }
            else
                return false;
        }
    }
}
