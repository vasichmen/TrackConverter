using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.Projections;
using System.Net;
using System.IO;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    //TODO: комментарии, доделать связь с сервисом
    public static class Genshtab
    {
        /// <summary>
        /// основные методы для получения карт ГГЦ
        /// </summary>
        public abstract class BaseGenshtab : BaseMapProvider
        {
            public abstract string[] Mirrors { get; }

            int buf_ok = int.MinValue;
            int buf_no = int.MinValue;

            private int GetCode(string url)
            {
                WebClient wc = new WebClient();
                try
                {
                    string ans = wc.DownloadString(url);
                    return int.Parse(ans);
                }
                catch (WebException e)
                {
                    Stream resp = e.Response.GetResponseStream();
                    StreamReader sr = new StreamReader(resp);
                    string ans = sr.ReadToEnd();
                    if (ans.Contains("403"))
                        return 200;
                    else
                        return 404;
                }
            }

            protected string CombineURL(GPoint pos, int zoom)
            {
                //выбор масштаба, которые есть в карте
                int z_ok, z_no;
                if (buf_ok == int.MinValue)
                    buf_ok = 1;
                if (buf_no == int.MinValue)
                    buf_no = 25;
                z_ok = buf_ok;
                z_no = buf_no;

                if (zoom > z_no)
                    return "";

                int r = new Random().Next(2);
                string url = Mirrors[r];

             
                if (zoom > z_ok)
                {
                    int code = GetCode(url + "z" + (zoom + 1) + "/");
                    // Внимание! Если сервер вернёт 403 - code почему-то будет 0. Так что выбор "404 или что-то иное"
                    // Если дело дошло до проверки наличия масштаба - значит какая-то граница (z_ok / z_no) точно подвинется.

                    if (code == 404)
                        z_no = zoom;
                    else
                        z_ok = zoom;

                    // Сохраняем актуализированные границы для последующих вызовов
                    buf_no = z_no;
                    buf_ok = z_ok;

                    if (zoom  >= z_no)
                            return "";
                }

                int divx = (int)(pos.X / 1024);
                int divy = (int)(pos.Y / 1024);

                string res = url + string.Format("z{0}/{1}/x{2}/{3}/y{4}.jpg", zoom + 1, divx, pos.X, divy, pos.Y);
                return res;
            }
        }

        /// <summary>
        /// километровка 
        /// </summary>
        public class KM1 : BaseGenshtab
        {
            public static KM1 Instance;

            static KM1()
            {
                Instance = new KM1();
            }

            public override string[] Mirrors
            {
                get
                {
                    return new string[] {
                        "http://91.237.82.95:8088/pub/genshtab/1km/",
                        "http://maps.melda.ru/pub/genshtab/1km/"
                    };
                }
            }

            public override Guid Id
            {
                get
                {
                    return new Guid("BF6C9C19-AD78-4F1F-96CE-6EF1F5B52EF7");
                }
            }

            public override string Name
            {
                get
                {
                    return "Генштаб 1км";
                }
            }

            public override GMapProvider[] Overlays
            {
                get
                {
                    return new GMapProvider[1] { Instance };
                }
            }

            public override PureProjection Projection
            {
                get
                {
                    return new MercatorProjection();
                }
            }

            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                string url = CombineURL(pos, zoom);
                if (string.IsNullOrEmpty(url))
                    return null;
                return GetGMapImage(url);
            }
        }

        /// <summary>
        /// 250 метров 
        /// </summary>
        public class M250 : BaseGenshtab
        {
            public static M250 Instance;

            static M250()
            {
                Instance = new M250();
            }

            public override string[] Mirrors
            {
                get
                {
                    return new string[] {
                        "http://91.237.82.95:8088/pub/genshtab/250m/",
                        "http://maps.melda.ru/pub/genshtab/250m/"
                    };
                }
            }

            public override Guid Id
            {
                get
                {
                    return new Guid("BF6C9C79-AD78-4F1F-96CE-6EF1F5B52EF7");
                }
            }

            public override string Name
            {
                get
                {
                    return "Генштаб 250м";
                }
            }

            public override GMapProvider[] Overlays
            {
                get
                {
                    return new GMapProvider[1] { Instance };
                }
            }

            public override PureProjection Projection
            {
                get
                {
                    return new MercatorProjection();
                }
            }

            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                string url = CombineURL(pos, zoom);
                if (string.IsNullOrEmpty(url))
                    return null;
                return GetGMapImage(url);
            }
        }
    }
}
