using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// исторические карты retromap.ru
    /// Источник: http://www.retromap.ru/functions.js
    /// </summary>
    public static class Retromap
    {
        /// <summary>
        /// вспомогательные методы для работы с картами retromap
        /// </summary>
        public static Service ServiceEngine;

        static Retromap()
        {
            ServiceEngine = new Service(System.Windows.Forms.Application.StartupPath + Res.Properties.Resources.cache_directory + "\\http_cache\\retromap\\service");
        }

        /// <summary>
        /// вспомогательные методы для работы с картами retromap
        /// </summary>
        public class Service : BaseConnection
        {
            public Service(string cacheDirectory, int duration = 168) : base(cacheDirectory, duration) { }

            public override TimeSpan MinQueryInterval { get { return TimeSpan.FromMilliseconds(50); } }
            public override int MaxAttempts { get { return 5; } }

            /// <summary>
            /// получить все карты Retromap, которые есть в заданной точке
            /// </summary>
            /// <param name="point"></param>
            /// <returns></returns>
            public List<string> MapsInPoint(PointLatLng point)
            {
                //http://www.retromap.ru/search_read_maps.php?mode=1&&lat=55.798193&lng=37.762413
                string url = "http://www.retromap.ru/search_read_maps.php?mode=1&&lat={0}&lng={1}";
                url = string.Format(url, point.Lat.ToString().Replace(Vars.DecimalSeparator, '.'), point.Lng.ToString().Replace(Vars.DecimalSeparator, '.'));
                string ans = SendStringGetRequest(url, false);
                ans = ans.Trim();
                string[] ids = ans.Split(' ');
                var res = new List<string>();
                foreach (var id in ids)
                    res.Add(id);
                return res;
            }

            /// <summary>
            /// проверяет наличие заданной карты в указанной точке при заданном масштабе
            /// </summary>
            /// <param name="mapID">значение поля MapID карты</param>
            /// <param name="position">координаты точки</param>
            /// <returns></returns>
            public bool Exist(string mapID, PointLatLng position, int zoom)
            {
                List<string> total_maps = MapsInPoint(position);
                bool res = total_maps.Contains(mapID);
                return res;
            }
        }

        /// <summary>
        /// базовый класс 
        /// </summary>
        public abstract class BaseRetromap : BaseMapProvider
        {
            /// <summary>
            /// проекция карты
            /// </summary>
            public override PureProjection Projection { get { return MercatorProjection.Instance; } }

            /// <summary>
            /// массив хостов хранения карт
            /// </summary>
            protected abstract string[] Mirrors { get; }

            /// <summary>
            /// шифрование имён тайлов 0, 1, 2, 3 (в описании map_list.encoding)
            /// </summary>
            protected abstract int Encryption { get; }

            /// <summary>
            /// идентификатор карты (в строке адреса)
            /// </summary>
            public abstract string MapID { get; }

            /// <summary>
            /// прямоугольник карты. Получается из свойства map_list[mapId].bounds
            /// </summary>
            public abstract RectLatLng Rectangle { get;}

            /// <summary>
            /// получить ссылку на тайл по координатам с учетом шифрования карты. Аналог фунции buckweatt
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="zoom"></param>
            /// <returns></returns>
            private string combineURL(GPoint pos, int zoom)
            {
                Random rnd = new Random();
                int ind = rnd.Next(Mirrors.Length - 1);
                if (Encryption == 3)
                {
                    string format = Mirrors[ind] + @"/Z{0}/{1}/{2}.jpg";
                    return string.Format(format, zoom, pos.Y, pos.X);
                }

                //перевод функции buckweatt (x,y,z)
                string addres;
                int m = int.Parse(MapID);
                string key = tileToQuadKey(pos, zoom);
                var prefix = m_prefix(key);
                if (this.Encryption == 0)
                    addres = key;
                if (Encryption != 2) //=> Encrition == 1 т.к. 3 отсеялось раньше, а 0 только что проверили
                    addres = prefix + key;

                if (prefix.Length == 0 || (prefix.Length > 0 && prefix[0] != 'F')) //сюда уже попадает только Encription == 2
                    addres = key;
                else
                {
                    string w = key, f = "", v = "";
                    if (key.Length > 16)
                        w = key.Substring(key.Length - 16, 16);
                    string r = (long.Parse(w) + 10 * m * (long.Parse(w[w.Length - 1].ToString()) + 1)).ToString();
                    if (key.Length > 16)
                    {
                        f = key.Substring(0, key.Length - 16);
                        v = "000000".Substring(0, 16 - r.Length);
                    }
                    addres = prefix.Replace("FF", "FE") + f + v + r;
                }
                string res = string.Concat(Mirrors[ind], "/", addres, ".jpg");
                return res;

            }

            /// <summary>
            /// получить ключ тайла по координате и масштабу
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="zl"></param>
            /// <returns></returns>
            private string tileToQuadKey(GPoint pos, int zl)
            {
                var quad = "";
                for (var i = zl; i > 0; i--)
                {
                    var mask = 1 << (i - 1);
                    var cell = 0;
                    if ((pos.X & mask) != 0)
                        cell++;
                    if ((pos.Y & mask) != 0)
                        cell += 2;
                    quad += cell;
                }
                return quad;
            }

            /// <summary>
            /// получить префикс для тайла
            /// </summary>
            /// <param name="qkey">QuadKey тайла</param>
            /// <returns></returns>
            private string m_prefix(string qkey)
            {
                if (Encryption != 0)
                {
                    long cod = (long.Parse(this.MapID) % 2);
                    string code = (cod % 2 != 0) ? "1" : "0";
                    int start1 = qkey.Length - 1;
                    int start2 = qkey.Length - 3;
                    while (start2 < 0)
                        start2 = qkey.Length + start2;
                    if ((qkey.Substring(start1, 1) == code) || (qkey.Substring(start2, 1) == "0"))
                    {
                        string res = "FF" + getHashString(" " + qkey + ".jpg").Substring(0, 4) + "_";
                        return res;
                    }
                }
                return "";
            }

            /// <summary>
            /// получение хэш-суммы строки
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            private string getHashString(string s)
            {
                //переводим строку в байт-массим  
                byte[] bytes = Encoding.UTF8.GetBytes(s); //для работы retromap должна быть кодировка UTF8

                //создаем объект для получения средст шифрования  
                MD5CryptoServiceProvider CSP =
                    new MD5CryptoServiceProvider();

                //вычисляем хеш-представление в байтах  
                byte[] byteHash = CSP.ComputeHash(bytes);

                string hash = string.Empty;

                //формируем одну цельную строку из массива  
                foreach (byte b in byteHash)
                    hash += string.Format("{0:x2}", b);

                return hash;
            }

            /// <summary>
            /// получить тайл карты в указанной позиции
            /// </summary>
            /// <param name="pos">координаты тайла</param>
            /// <param name="zoom">масштаб</param>
            /// <returns></returns>
            public override PureImage GetTileImage(GPoint pos, int zoom)
            {
                string url = combineURL(pos, zoom); //12031010110201
                GMapImage img = GetGMapImage(url, "http://www.retromap.ru/m.html");
                return img;
            }

        }

        /// <summary>
        /// Карта РККА 1941 г.  1:100,000
        /// </summary>
        public class RKKA1941 : BaseRetromap
        {
            public static RKKA1941 Instance;

            static RKKA1941()
            {
                Instance = new RKKA1941();
            }

            public override Guid Id { get { return new Guid(new byte[] { 34, 56, 212, 12, 56, 82, 115, 94, 14, 105, 14, 28, 214, 38, 23, 83 }); } }
            public override string Name { get { return "Карта РККА 1941 г. 1:100 000"; } }

            public override GMapProvider[] Overlays { get { return new GMapProvider[] { GoogleMapProvider.Instance, Instance }; } }

            protected override string[] Mirrors { get { return new string[] { "http://www.map.host.ru/f19f53e/14194126" }; } }

            protected override int Encryption { get { return 3; } }

            public override string MapID { get { return "14194126"; } }

            public override RectLatLng Rectangle { get {
                    PointLatLng swest = new PointLatLng(51.9781113, 29.9487305);
                    PointLatLng neast = new PointLatLng(58.0197334, 42.0227051);
                    return new RectLatLng(swest.Lat, neast.Lng, neast.Lng - swest.Lng, neast.Lat - swest.Lat);
                } }
        }

        public class GermanMoscowRegionMap1940 : BaseRetromap
        {
            public static GermanMoscowRegionMap1940 Instance;

            static GermanMoscowRegionMap1940()
            {
                Instance = new GermanMoscowRegionMap1940();
            }

            public override Guid Id { get { return new Guid(new byte[] { 34, 56, 212, 12, 56, 52, 115, 94, 14, 105, 14, 28, 214, 38, 23, 83 }); } }
            public override string Name { get { return "Немецкая карта Подмосковья 1940 г. 1:50 000"; } }
            public override GMapProvider[] Overlays { get { return new GMapProvider[] { GoogleMapProvider.Instance, Instance }; } }
            protected override string[] Mirrors { get { return new string[] { "http://www.map.host.ru/f19f53e/061940" }; } }

            protected override int Encryption { get { return 2; } }

            public override string MapID { get { return "061940"; } }

            public override RectLatLng Rectangle
            {
                get
                {
                    PointLatLng swest = new PointLatLng(lat: 55.3416405, lng: 36.496582);
                    PointLatLng neast = new PointLatLng(lat: 56.3409004, lng: 38.4960938);
                    return new RectLatLng(swest.Lat, neast.Lng, neast.Lng - swest.Lng, neast.Lat - swest.Lat);
                }
            }
        }

        /// <summary>
        /// почвенная карта МО 1985
        /// </summary>
        public class SoilMoscowRegionMap1985 : BaseRetromap
        {
            public static SoilMoscowRegionMap1985 Instance;

            static SoilMoscowRegionMap1985()
            {
                Instance = new SoilMoscowRegionMap1985();
            }

            public override Guid Id { get { return new Guid(new byte[] { 34, 56, 212, 13, 56, 52, 115, 94, 14, 105, 14, 28, 214, 38, 23, 83 }); } }
            public override string Name { get { return "Почвенная карта Подмосковья 1985 г. 1:300 000"; } }
            public override GMapProvider[] Overlays { get { return new GMapProvider[] { GoogleMapProvider.Instance, Instance }; } }
            protected override string[] Mirrors { get { return new string[] { "http://www.retromap.ru/f19f53e/121988" }; } }

            protected override int Encryption { get { return 0; } }

            public override string MapID { get { return "121988"; } }

            public override RectLatLng Rectangle
            {
                get
                {
                    PointLatLng swest = new PointLatLng(lat: 54.1624336, lng: 34.6289062);
                    PointLatLng neast = new PointLatLng(lat: 57.1362381, lng: 40.4296875);
                    return new RectLatLng(swest.Lat, neast.Lng, neast.Lng - swest.Lng, neast.Lat - swest.Lat);
                }
            }
        }

        /// <summary>
        /// почвенная карта МО 1985
        /// </summary>
        public class USMoscowRegionMap1953 : BaseRetromap
        {
            public static USMoscowRegionMap1953 Instance;

            static USMoscowRegionMap1953()
            {
                Instance = new USMoscowRegionMap1953();
            }

            public override Guid Id { get { return new Guid(new byte[] { 34, 56, 212, 13, 56, 52, 115, 94, 14, 105, 14, 20, 214, 38, 23, 83 }); } }
            public override string Name { get { return "Американская карта Подмосковья 1953 г. 1:250 000"; } }
            public override GMapProvider[] Overlays { get { return new GMapProvider[] { GoogleMapProvider.Instance, Instance }; } }
            protected override string[] Mirrors { get { return new string[] { "http://map.host.ru/f19f53e/1419547" }; } }

            protected override int Encryption { get { return 3; } }

            public override string MapID { get { return "1419547"; } }

            public override RectLatLng Rectangle
            {
                get
                {
                    PointLatLng swest = new PointLatLng(lat: 38.7540817, lng: 19.9511719);
                    PointLatLng neast = new PointLatLng(lat: 70.0205841, lng: 60.2929688);
                    return new RectLatLng(swest.Lat, neast.Lng, neast.Lng - swest.Lng, neast.Lat - swest.Lat);
                }
            }
        }

    }
}
