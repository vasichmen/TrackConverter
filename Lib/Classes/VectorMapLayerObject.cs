using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Classes
{
    /// <summary>
    /// информация об объекте на слое карты
    /// </summary>
    [Serializable]
    public class VectorMapLayerObject
    {
        private const char sep = '#';

        /// <summary>
        /// координаты центра многоугольника
        /// </summary>
        private Coordinate geometryCenter;

        /// <summary>
        /// периметр объекта в метрах
        /// </summary>
        private double perimeter;

        /// <summary>
        /// ID 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Поставщик информации об объекте
        /// </summary>
        public VectorMapLayerProviders LayerProvider { get; set; }

        /// <summary>
        /// создаёт новый объект слоя карты
        /// </summary>
        /// <param name="geometry"></param>
        public VectorMapLayerObject(GMapPolygon geometry)
        {
            Geometry = geometry;
            this.Name = geometry.Name;
            geometryCenter = GetGeometryCenter(geometry);
            perimeter = double.NaN;
        }

        /// <summary>
        /// создаёт новый объект слоя карты из точек 
        /// </summary>
        /// <param name="geometry">геометрия объекта, заданная списком точек</param>
        /// <param name="name">имя объекта</param>
        public VectorMapLayerObject(TrackFile geometry, string name)
        : this(new GMapPolygon(geometry.GMapPoints, name)) { }

        /// <summary>
        /// преобразует строковое представление периметра объекта в полигон
        /// </summary>
        /// <param name="geometryString">строковое представленение координат вершин многоугольника</param>
        /// <param name="name">название объекта</param>
        /// <returns></returns>
        private GMapPolygon GetGeometryFromString(string geometryString, string name)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            string[] pairs = geometryString.Split(sep);
            if (Math.IEEERemainder(pairs.Length, 2d) == 0)
                throw new Exception("Ошибка в строке координат вершин многоугольника. (не все числа имеют пары)");
            for (int i = 0; i < pairs.Length - 1; i += 2)
            {
                double lat = double.Parse(pairs[i].Replace('.', Vars.DecimalSeparator));
                double lon = double.Parse(pairs[i + 1].Replace('.', Vars.DecimalSeparator));
                points.Add(new PointLatLng(lat, lon));
            }

            return new GMapPolygon(points, name);
        }

        /// <summary>
        /// возвращает строковое представлениие координат вершит многоугольника
        /// </summary>
        /// <param name="geometry">вершины многоугольника</param>
        /// <returns></returns>
        private string GetGeometryString(GMapPolygon geometry)
        {
            string res = string.Empty;
            foreach (PointLatLng pt in geometry.Points)
                res += pt.Lat.ToString().Replace(Vars.DecimalSeparator, '.') +
                    sep +
                    pt.Lng.ToString().Replace(Vars.DecimalSeparator, '.') +
                    sep;
            res.TrimEnd(new[] { sep });
            return res;
        }

        /// <summary>
        /// вычисление координат центра многоугольника
        /// </summary>
        /// <param name="geometry">вершины многоугольника</param>
        /// <returns></returns>
        private Coordinate GetGeometryCenter(GMapPolygon geometry)
        {
            double lat_min = double.MaxValue;
            double lat_max = double.MinValue;
            double lon_min = double.MaxValue;
            double lon_max = double.MinValue;
            foreach (PointLatLng pt in geometry.Points)
            {
                if (lat_min > pt.Lat)
                    lat_min = pt.Lat;
                if (lat_max < pt.Lat)
                    lat_max = pt.Lat;
                if (lon_min > pt.Lng)
                    lon_min = pt.Lng;
                if (lon_max < pt.Lng)
                    lon_max = pt.Lng;
            }
            return new Coordinate((lat_min + lat_max) / 2, (lon_min + lon_max) / 2);
        }

        /// <summary>
        /// получение хэш-суммы строки
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetHashString(string s)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(s);

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
        /// Геометрия объекта
        /// </summary>
        public GMapPolygon Geometry { get; private set; }

        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сссылка на описание объекта
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// координаты центра многоугольника
        /// </summary>
        public Coordinate GeometryCenter
        {
            get
            {
                return geometryCenter;
            }
            private set { geometryCenter = value; }
        }

        /// <summary>
        /// длина периметра объекта в метрах
        /// </summary>
        public double Perimeter
        {
            get
            {
                if (perimeter == double.NaN)
                    perimeter = Vars.CurrentGeosystem.CalculateDistance(this.Geometry.Points);
                return perimeter;
            }
            private set { perimeter = value; }
        }

        /// <summary>
        /// если истина, то объект невидим на карте
        /// </summary>
        public bool Invisible { get; set; }

        /// <summary>
        /// текстовое представление объекта слоя (имя)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
