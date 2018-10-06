using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Drawing;
using System.IO;
using System.Net;

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
        /// <param name="referer">заголовок HTTP REFERER, который будет указан в запросе (нужен для некоторых карт)</param>
        /// <returns></returns>
        protected static GMapImage GetGMapImage(string url, string referer = null)
        {
            try
            {
                WebClient wc = new WebClient();
                if (referer != null)
                    wc.Headers[HttpRequestHeader.Referer] = referer;

                Stream str = wc.OpenRead(url);
                if (wc.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                    return new GMapImage();
                GMapImage res = new GMapImage
                {
                    Data = copyStream(str)
                };
                res.Img = Image.FromStream(res.Data);
                return res;
            }
            catch (Exception e) {
                Image img = new Bitmap(256, 256);
                MemoryStream str = new MemoryStream();
                img.Save(str, System.Drawing.Imaging.ImageFormat.Bmp);
                return new GMapImage() {  Img=img, Data =  str}; }
        }

        /// <summary>
        /// копирует поток в MemoryStream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private static MemoryStream copyStream(Stream inputStream)
        {
            const int READSIZE = 32 * 1024;
            byte[] buffer = new byte[READSIZE];
            MemoryStream ms = new MemoryStream();
            {
                int count = 0;
                while ((count = inputStream.Read(buffer, 0, READSIZE)) > 0)
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
