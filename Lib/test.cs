using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using TrackConverter.Lib.Mathematic.Geodesy.Projections.GMapImported;
using System.Drawing;
using GMap.NET.WindowsForms;
using System.Net;
using System.IO;

namespace TrackConverter.Lib
{
    public class test:GMapProvider
    {

        public static readonly test Instance;

        static test()
        {
            Instance = new test();
        }

        public override Guid Id
        {
            get
            {
                //return new Guid("82DC969D-0491-40F3-8C21-4D97B67F47EB");
                return Guid.NewGuid();
            }
        }

        public override string Name
        {
            get
            {
                return "test";
            }
        }

        public override GMapProvider[] Overlays
        {
            get
            {
                return new GMapProvider[1] { test.Instance };
            }
        }

        public override PureProjection Projection
        {
            get
            {
                return MercatorProjectionYandex.Instance;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            //https://sat04.maps.yandex.net/tiles?l=sat&v=3.426.0&x=9912&y=5132&z=14&lang=ru_RU
            string url = "https://sat0{0}.maps.yandex.net/tiles?l=sat&v=3.426.0&x={1}&y={2}&z={3}&lang={4}";
            int server = new Random().Next(1, 5);
            string lang;
            switch (Vars.Options.Map.MapLanguange)
            {
                case LanguageType.Russian:
                    lang = "ru_RU";
                    break;
                default: throw new Exception("Этот язык не реализован");
            }

            url = string.Format(url, server, pos.X, pos.Y, zoom, lang);
            Stream str = GetImage(url);
            //Image tile = GetImage(url);
            GMapImage res = new GMapImage();

            MemoryStream data = CopyStream(str, false);

            res.Data = data;

            res.Img = Image.FromStream(res.Data);
            return res;
        }

        public static Stream GetImage(string url)
        {
            WebClient wc = new WebClient();
          
                Stream str = wc.OpenRead(url);
//if (wc.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                //    return new Bitmap(256, 256);
                return str;
                str.Close();
                wc.Dispose();
               // return res;
           
        }


        public static MemoryStream CopyStream(Stream inputStream, bool SeekOriginBegin)
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
