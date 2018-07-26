using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Mathematic.Geodesy.MapCalibrations
{
    /// <summary>
    /// создает файлы привязки карт в формате OZI
    /// </summary>
    public static class MapCalibrationOzi
    {
        /// <summary>
        /// создание файла привязки
        /// </summary>
        /// <param name="fname">адрес файла изображения карты</param>
        /// <param name="Area">область сохраняемой карты</param>
        /// <param name="pxDelta">размер области карты ( X на Y точек)</param>
        /// <param name="projection">проекция карты для поределения координат точек привязки</param>
        /// <param name="Zoom">масштаб для определения пиксельных координат</param>
        /// <param name="basep">абсолютные пиксельные координаты левого верхнего угла прямоугольника</param>
        public static void CreateMapCalibrationFile(string fname, PureProjection projection, RectLatLng Area, GPoint pxDelta, int Zoom, GPoint basep)
        {
            makeMapLinkFile(fname, projection, Area, pxDelta, Zoom, basep);
        }

        /// <summary>
        /// создание файла привязки .map
        /// Описание формата:
        /// http://www.oziexplorer3.com/eng/help/map_file_format.html
        /// </summary>
        /// <param name="fname">адрес файла карты</param>
        /// <param name="Area">область сохраняемой карты</param>
        /// <param name="pxDelta">размер области карты ( X на Y точек)</param>
        /// <param name="projection">проекция карты для поределения координат точек привязки</param>
        /// <param name="Zoom">масштаб для определения пиксельных координат</param>
        /// <param name="basep">абсолютные пиксельные координаты левого верхнего угла прямоугольника</param>
        private static void makeMapLinkFile(string fname, PureProjection projection, RectLatLng Area, GPoint pxDelta, int Zoom, GPoint basep)
        {
            //исходные данные привязки
            string fold = Path.GetDirectoryName(fname);
            string nm = Path.GetFileNameWithoutExtension(fname);
            string mapFile = fold + "\\" + nm + ".map";
            StreamWriter sw = new StreamWriter(mapFile);
            string VMapName = Path.GetFileName(fname);

            string VProjection = "Mercator";
            string cCalibrationEmptyStrFormat = "Point{0:00},xy,     ,     ,in, deg,    ,        ,N,    ,        ,W, grid,   ,           ,           ,N\r\n";

            GRect XY = new GRect(0, 0, pxDelta.X, pxDelta.Y);

            GPoint xy_left_top = new GPoint(XY.Left, XY.Top);
            GPoint xy_right_bottom = new GPoint(XY.Right, XY.Bottom);
            GPoint xy_center_center = new GPoint((XY.Right - XY.Left) / 2, (XY.Bottom - XY.Top) / 2);
            GPoint xy_center_top = new GPoint((XY.Right - XY.Left) / 2, XY.Top);
            GPoint xy_right_top = new GPoint(XY.Right, XY.Top);
            GPoint xy_left_center = new GPoint(XY.Left, (XY.Bottom - XY.Top) / 2);
            GPoint xy_right_center = new GPoint(XY.Right, (XY.Bottom - XY.Top) / 2);
            GPoint xy_left_bottom = new GPoint(XY.Left, XY.Bottom);
            GPoint xy_center_bottom = new GPoint((XY.Right - XY.Left) / 2, XY.Bottom);

            PointLatLng ll_left_top = projection.FromPixelToLatLng(xy_left_top.X + basep.X, xy_left_top.Y + basep.Y, Zoom);
            PointLatLng ll_right_bottom = projection.FromPixelToLatLng(xy_right_bottom.X + basep.X, xy_right_bottom.Y + basep.Y, Zoom);
            PointLatLng ll_center_center = projection.FromPixelToLatLng(xy_center_center.X + basep.X, xy_center_center.Y + basep.Y, Zoom);
            PointLatLng ll_center_top = projection.FromPixelToLatLng(xy_center_top.X + basep.X, xy_center_top.Y + basep.Y, Zoom);
            PointLatLng ll_right_top = projection.FromPixelToLatLng(xy_right_top.X + basep.X, xy_right_top.Y + basep.Y, Zoom);
            PointLatLng ll_left_center = projection.FromPixelToLatLng(xy_left_center.X + basep.X, xy_left_center.Y + basep.Y, Zoom);
            PointLatLng ll_right_center = projection.FromPixelToLatLng(xy_right_center.X + basep.X, xy_right_center.Y + basep.Y, Zoom);
            PointLatLng ll_left_bottom = projection.FromPixelToLatLng(xy_left_bottom.X + basep.X, xy_left_bottom.Y + basep.Y, Zoom);
            PointLatLng ll_center_bottom = projection.FromPixelToLatLng(xy_center_bottom.X + basep.X, xy_center_bottom.Y + basep.Y, Zoom);

            //заголовок и точки привязки карты
            string text = "OziExplorer Map Data File Version 2.2\r\n" +
                VMapName + "\r\n" +
                VMapName + "\r\n" +
                "1 ,Map Code,\r\n" +
                "WGS 84,,   0.0000,   0.0000,WGS 84\r\n" +
                "Reserved 1\r\n" +
                "Reserved 2\r\n" +
                "Magnetic Variation,,,E\r\n" +
                "Map Projection," + VProjection + ",PolyCal,No,AutoCalOnly,No,BSBUseWPX,No\r\n" +
                GetPointCalibrationStr(1, xy_left_top, ll_left_top) +
                GetPointCalibrationStr(2, xy_right_bottom, ll_right_bottom) +
                GetPointCalibrationStr(3, xy_center_center, ll_center_center) +
                GetPointCalibrationStr(4, xy_center_top, ll_center_top) +
                GetPointCalibrationStr(5, xy_right_top, ll_right_top) +
                GetPointCalibrationStr(6, xy_left_center, ll_left_center) +
                GetPointCalibrationStr(7, xy_right_center, ll_right_center) +
                GetPointCalibrationStr(8, xy_left_bottom, ll_left_bottom) +
                GetPointCalibrationStr(9, xy_center_bottom, ll_center_bottom);

            //пустые точки привязки
            for (int i = 10; i <= 30; i++)
                text += string.Format(cCalibrationEmptyStrFormat, i);

            //масштаб 
            double dist = projection.GetDistance(ll_left_bottom, ll_right_bottom);
            double pix = pxDelta.X;
            double scale = dist / pix;

            char separator = Vars.DecimalSeparator;

            //информация о карте
            text += "Projection Setup,,,,,,,,,,\r\n" +
                 "Map Feature = MF ; Map Comment = MC     These follow if they exist\r\n" +
                 "Track File = TF      These follow if they exist\r\n" +
                 "Moving Map Parameters = MM?    These follow if they exist\r\n" +
                 "MM0,Yes\r\n" +
                 "MMPNUM,4\r\n" +
                 "MMPXY,1," + XY.Left + "," + XY.Top + "\r\n" +
                 "MMPXY,2," + XY.Right + "," + XY.Top + "\r\n" +
                 "MMPXY,3," + XY.Right + "," + XY.Bottom + "\r\n" +
                 "MMPXY,4," + XY.Left + "," + XY.Bottom + "\r\n" +
                 "MMPLL,1, " + ll_left_top.Lng.ToString("00.000000").Replace(separator, '.') + ", " + ll_left_top.Lat.ToString("00.000000").Replace(separator, '.') + "\r\n" +
                 "MMPLL,2, " + ll_right_bottom.Lng.ToString("00.000000").Replace(separator, '.') + ", " + ll_left_top.Lat.ToString("00.000000").Replace(separator, '.') + "\r\n" +
                 "MMPLL,3, " + ll_right_bottom.Lng.ToString("00.000000").Replace(separator, '.') + ", " + ll_right_bottom.Lat.ToString("00.000000").Replace(separator, '.') + "\r\n" +
                 "MMPLL,4, " + ll_left_top.Lng.ToString("00.000000").Replace(separator, '.') + ", " + ll_right_bottom.Lat.ToString("00.000000").Replace(separator, '.') + "\r\n" +
                 "MM1B," + scale.ToString().Replace(separator, '.') + "\r\n" +
                 "MOP,Map Open Position,0,0\r\n" +
                 "IWH,Map Image Width/Height," + XY.Right + "," + XY.Bottom + "\r\n";

            sw.Write(text);
            sw.Close();
        }

        private static string GetPointCalibrationStr(int APointID, GPoint XYpoint, PointLatLng LLPoint)
        {
            Coordinate coord = new Coordinate(LLPoint.Lat, LLPoint.Lng);
            string cCalibrationStrFormat = "Point{0:00},xy,    {1}, {2},in, deg, {3}, {4}, grid,   ,           ,           ,N\r\n";
            string lon = coord.Longitude.ToString("dd, mm.mmmm,H");
            string lat = coord.Latitude.ToString("dd, mm.mmmm,H");
            return string.Format(cCalibrationStrFormat, APointID, XYpoint.X, XYpoint.Y, lat, lon);
        }



    }
}
