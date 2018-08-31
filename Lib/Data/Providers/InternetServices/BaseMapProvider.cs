using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using System.IO;
using System.Net;
using GMap.NET.WindowsForms;
using System.Drawing;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// базовый класс для поставщика карты
    /// </summary>
    public abstract class BaseMapProvider : GMapProvider
    {

        /// <summary>
        /// возвращает картинку тайла по запросу
        /// </summary>
        /// <param name="url">запрос к серверу</param>
        /// <returns></returns>
        protected static GMapImage GetGMapImage(string url)
        {
            WebClient wc = new WebClient();

            Stream str = wc.OpenRead(url);
            if (wc.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                return new GMapImage();
            GMapImage res = new GMapImage();
            res.Data = CopyStream(str);
            res.Img = Image.FromStream(res.Data);
            return res;
        }

        /// <summary>
        /// копирует поток в MemoryStream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private static MemoryStream CopyStream(Stream inputStream)
        {
            const int readSize = 32 * 1024;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();
            {
                int count = 0;
                while ((count = inputStream.Read(buffer, 0, readSize)) > 0)
                {
                    ms.Write(buffer, 0, count);
                }
            }
            buffer = null;
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
