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
    public abstract class BaseMapProvider : GMapProvider
    {

        protected static GMapImage GetGMapImage(string url)
        {
            WebClient wc = new WebClient();

            Stream str = wc.OpenRead(url);
            if (wc.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                return new GMapImage();
            GMapImage res = new GMapImage();
            res.Data = CopyStream(str, false);
            res.Img = Image.FromStream(res.Data);
            return res;
        }


        private static MemoryStream CopyStream(Stream inputStream, bool SeekOriginBegin)
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
            if (SeekOriginBegin)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
