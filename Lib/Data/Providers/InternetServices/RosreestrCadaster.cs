using GMap.NET;
using GMap.NET.Projections;
using System;
using System.Drawing;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// слой кадастровых границ росреестра
    /// </summary>
    public class RosreestrCadaster : BaseConnection, IRastrMapLayerProvider
    {
        public RosreestrCadaster() : base(null)
        { }


        public override TimeSpan MinQueryInterval { get => TimeSpan.FromMilliseconds(100); }
        public override int MaxAttempts { get => 4; }
        

        /// <summary>
        /// ???????????
        /// </summary>
        /// <param name="APoint"></param>
        /// <returns></returns>
        private PointLatLng lonLat2RelativeInternal(PointLatLng APoint)
        {
            double pi = Math.PI;
            double c, z;
            PointLatLng res = new PointLatLng();
            res.Lng = 0.5 + APoint.Lng / 360d;
            z = Math.Sin(APoint.Lat * pi / 180d);
            c = 1d / (2d * pi);
            res.Lat = 0.5 - 0.5 * Math.Log((1 + z) / (1 - z)) * c;
            return res;
        }

        /// <summary>
        /// ??????
        /// </summary>
        /// <param name="APoint"></param>
        /// <returns></returns>
        private PointLatLng lonLat2Metr(PointLatLng APoint, PureProjection proj)
        {
            var pi = Math.PI;
            PointLatLng res = new PointLatLng();
            var Vrelative = lonLat2RelativeInternal(APoint);
            var VMul = 2 * pi * proj.Axis;
            res.Lng = (Vrelative.Lng - 0.5) * VMul;
            res.Lat = (0.5 - Vrelative.Lat) * VMul;
            return res;
        }

        /// <summary>
        /// создать параметр bbox с метрическими координатами для формирования запроса
        /// </summary>
        /// <param name="x">координата тайла х</param>
        /// <param name="y">координатат тайла у</param>
        /// <param name="z">масштаб</param>
        /// <returns></returns>
        private string combineBBOX(RectLatLng tile, PureProjection projection)
        {

            PointLatLng locationLT= lonLat2Metr(tile.LocationTopLeft, projection);
            PointLatLng locationRB = lonLat2Metr(tile.LocationRightBottom, projection);
            double widthLng = locationRB.Lng - locationLT.Lng;
            double heighLat = locationRB.Lat - locationLT.Lat;
            RectLatLng rectM = new RectLatLng(locationLT.Lat,locationLT.Lng,widthLng,heighLat);

            string res = string.Format("{0},{1},{2},{3}",
                rectM.Left.ToString().Replace(Vars.DecimalSeparator,'.'),
                rectM.Bottom.ToString().Replace(Vars.DecimalSeparator, '.'),
                rectM.Right.ToString().Replace(Vars.DecimalSeparator, '.'),
                rectM.Top.ToString().Replace(Vars.DecimalSeparator, '.')
                );
            return res;
        }

        /// <summary>
        /// получить тайл карты по заданным тайловым координатам
        /// </summary>
        /// <param name="x">координата тайла</param>
        /// <param name="y">координата тайла</param>
        /// <param name="z">масштаб</param>
        /// <returns></returns>
        public Image GetRastrTile(long x, long y, int z)
        {
            throw new NotImplementedException("Для этого поставщика слоя надо использовать метод GetRastrTile(RectLatLng tile, PureProjection projection)");
        }

        /// <summary>
        /// получить изображение карты в данном прямоугольнике
        /// </summary>
        /// <param name="tile">координатный прямоугольник</param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public Image GetRastrTile(RectLatLng tile, PureProjection projection)
        {

            //http://pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/Cadastre/MapServer
            //http://pkk5.rosreestr.ru/arcgis/sdk/rest/index.html#//02ss0000006v000000

            //TODO: из файла описания преобразования координат из  планеты реализовать преобразование в метрические координаты
            //                                      minX       minY    maxX       maxY
            //границы WebMercator: WGS84 Bounds: -180.0000, -76.6798, 180.0000, 76.6798

            /*http://c.pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/Cadastre/MapServer/export/?
             * &service=WMS
             * &request=GetMap
             * &layers=show:0,1,2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,23,24,37,36,25,26,27,28,29,30,31
             * &styles=
             * &format=png32
             * &transparent=true
             * &version=
             * &dpi=96
             * &bboxSR=102100
             * &imageSR=102100
             * &size=1024,1024
             * &F=image
             * &height=1024
             * &width=1024
             * &srs=EPSG:3857
             * &bbox=4211986.006626352,7509173.658735716,4216877.976436603,7514065.628545967
             */
            //сервера a,b,c,d
            string baseUrl = "http://{0}.pkk5.rosreestr.ru/arcgis/rest/services/Cadastre/Cadastre/MapServer/export/?";
            baseUrl += "&service=WMS" +
                "&request=GetMap" +
                "&layers=show:0,1,2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,23,24,37,36,25,26,27,28,29,30,31" +
                "&styles=format=png32" +
                "&transparent=true" +
                "&dpi=96" +
                "&bboxSR=102100" +
                "&imageSR=102100";
            baseUrl += "&size=256,256";
            baseUrl += "&F=image&height=256&width=256&srs=EPSG:3857";
            baseUrl += "&bbox={1}";


            char s = new char[] { 'a', 'b', 'c', 'd' }[new Random().Next(3)];
            string bbox = combineBBOX(tile,projection);
            string url = string.Format(baseUrl, s, bbox);

            Image res = GetImage(url);
            return res;
        }
    }
}
