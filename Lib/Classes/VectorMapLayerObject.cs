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
    public class VectorMapLayerObject
    {
        private const char SEP = '#';

        /// <summary>
        /// координаты центра многоугольника
        /// </summary>
        private Coordinate geometryCenter;

        /// <summary>
        /// периметр объекта в метрах
        /// </summary>
        private double perimeter;

        /// <summary>
        /// ограничивающщий прямоугольник
        /// </summary>
        private RectLatLng bounds = RectLatLng.Empty;

        /// <summary>
        /// ID 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Ограничивающий прямоугольник
        /// </summary>
        public RectLatLng Bounds { get {
                if (bounds.IsEmpty)
                    bounds = getGeometryBounds();
                return bounds;
            } }


        /// <summary>
        /// Поставщик информации об объекте
        /// </summary>
        public MapLayerProviders LayerProvider { get; set; }

        /// <summary>
        /// создаёт новый объект слоя карты
        /// </summary>
        /// <param name="geometry"></param>
        public VectorMapLayerObject(GMapPolygon geometry)
        {
            Geometry = geometry;
            geometryCenter = Coordinate.Empty;
            Name = geometry.Name;
            perimeter = double.NaN;
        }

        /// <summary>
        /// создаёт новый объект слоя карты из точек 
        /// </summary>
        /// <param name="geometry">геометрия объекта, заданная списком точек</param>
        /// <param name="name">имя объекта</param>
        public VectorMapLayerObject(TrackFile geometry, string name)
        : this(new GMapPolygon(geometry==null?new List<PointLatLng>():geometry.GMapPoints, name)) { }

        /// <summary>
        /// преобразует строковое представление периметра объекта в полигон
        /// </summary>
        /// <param name="geometryString">строковое представленение координат вершин многоугольника</param>
        /// <param name="name">название объекта</param>
        /// <returns></returns>
        private GMapPolygon getGeometryFromString(string geometryString, string name)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            string[] pairs = geometryString.Split(SEP);
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
        private string getGeometryString(GMapPolygon geometry)
        {
            string res = string.Empty;
            foreach (PointLatLng pt in geometry.Points)
                res += pt.Lat.ToString().Replace(Vars.DecimalSeparator, '.') +
                    SEP +
                    pt.Lng.ToString().Replace(Vars.DecimalSeparator, '.') +
                    SEP;
            res.TrimEnd(new[] { SEP });
            return res;
        }

        /// <summary>
        /// вычисление координат центра многоугольника
        /// </summary>
        /// <param name="geometry">вершины многоугольника</param>
        /// <returns></returns>
        private Coordinate getGeometryCenter(GMapPolygon geometry)
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
        /// получить границы прямоугольника вокруг объекта
        /// </summary>
        /// <returns></returns>
        private RectLatLng getGeometryBounds()
        {
            double latMin = double.MaxValue;
            double lonMin = double.MaxValue;
            double latMax = double.MinValue;
            double lonMax = double.MinValue;
            foreach (var pt in Geometry.Points)
            {
                if (pt.Lat < latMin)
                    latMin = pt.Lat;
                if (pt.Lng < lonMin)
                    lonMin = pt.Lng;
                if (pt.Lat > latMax)
                    latMax = pt.Lat;
                if (pt.Lng > lonMax)
                    lonMax = pt.Lng;
            }
            RectLatLng res = new RectLatLng(latMax, lonMin, lonMax-lonMin, latMax-latMin);
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
                if (geometryCenter.isEmpty)
                    geometryCenter = getGeometryCenter(this.Geometry);
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
                if (double.IsNaN(perimeter))
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
        /// геометрия объекта в формате TrackFile
        /// </summary>
        public TrackFile GeometryTrackFile { get {
                return new TrackFile(this.Geometry);
            } }

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
