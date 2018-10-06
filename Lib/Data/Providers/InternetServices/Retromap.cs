using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using GMap.NET.WindowsForms;
using System;
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
            protected abstract string MapID { get; }

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

                //проверка
                //string q = xyToQuadKey(new GPoint(1235, 640), 11);
                //12031010011


                //попытка перевести функцию buckweatt 
                string addres;
                int m = int.Parse(MapID);
                string key = tileToQuadKey(pos, zoom);
                var prefix = m_prefix(key);
                if (this.Encryption == 0)
                    addres = key;
                if (Encryption != 2) //=> Encrition == 1 т.к. 3 отсеялось раньше, а 0 только что проверили
                    addres = prefix + key;

                if (prefix.Length == 0 ||( prefix.Length > 0 && prefix[0] != 'F')) //сюда уже попадает только Encription == 2
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



                //Random rnd = new Random();
                //int ind = rnd.Next(Mirrors.Length - 1);
                //string key = xyToQuadKey(pos, zoom);
                //string prefix = m_prefix(key);
                //switch (Encryption)
                //{
                //    case 0://простое шифрование с QuadKey
                //        return Mirrors[ind] + "/" + key + ".jpg";
                //    case 1://перед QuadKey добавляется префикс, полученный из строки QuadKey с помощью алгоритма MD5
                //        return Mirrors[ind] + "/" + prefix + key + ".jpg";
                //    case 2://префикс и ключ модифицируются
                //        if (prefix.Length > 0 && prefix[0] != 'F') //по какой-то причине при таком условии нужно получать картинку просто по QuadKey
                //            return Mirrors[ind] + "/" + key + ".jpg";
                //        string w = key,
                //            f = "",
                //            v = "";
                //        if (key.Length > 16)
                //            w = key.Substring(key.Length - 16, 16);
                //        long wi = long.Parse(w);
                //        string lastc = w[w.Length - 1].ToString();
                //        long lastc_i = long.Parse(lastc);
                //        long m_i = long.Parse(this.MapID);
                //        string r = (wi + 10 * m_i * (lastc_i + 1)).ToString();
                //        if (key.Length > 16)
                //        {
                //            f = key.Substring(0, key.Length - 16);
                //            v = "000000".Substring(0, 16 - r.Length);
                //        }
                //        string res = prefix.Replace("FF", "FE") + f + v + r;
                //        return Mirrors[ind] + "/" + res + ".jpg";
                //    case 3://обычный тайловый способ
                //        string format = Mirrors[ind] + @"/Z{0}/{1}/{2}.jpg";
                //        return string.Format(format, zoom, pos.Y, pos.X);

                //    default: throw new Exception("Этот способ кодировки не определён!");
                //}
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
                        string res = set_prefix(qkey + ".jpg");
                        return res;
                    }
                }
                return "";
            }

            private string set_prefix(string str)
            {

                string huff = getHashString(" " + str);
                string res = "FF" + huff.Substring(0, 4) + "_";

                //проверка кодировки при расчёте хэша
                //string h = getHashString(" 12013232300.jpg");
                //"978127e5db4cd7a4562eac5e610068fe"
                return res;
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

            protected override string[] Mirrors { get { return new string[] { "http://www.retromap.host.ru/f19f53e/14194126" }; } }

            protected override int Encryption { get { return 3; } }

            protected override string MapID { get { return "14194126"; } }
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

            protected override string MapID { get { return "061940"; } }
        }


    }
}
