using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GMap.NET;

namespace TrackConverter.Lib.Mathematic.Geodesy.MapCalibrations
{
    /// <summary>
    /// создает файл привязки карты в формате World
    /// </summary>
    public static class MapCalibrationWorld
    {
        /// <summary>
        /// создание файла прив\язки карты
        /// </summary>
        /// <param name="fname">имя файла изображения карты</param>
        /// <param name="Area">область карты</param>
        /// <param name="pxDelta">размер карты в пикселях</param>
        public static void CreateMapCalibrationFile(string fname, RectLatLng Area, GPoint pxDelta)
        {
            makeWLinkFile(fname, Area, pxDelta);
        }

        /// <summary>
        /// создание файла привязки .w
        /// </summary>
        /// <param name="fname">адрес файла карты</param>
        /// <param name="Area">область сохраняемой карты</param>
        /// <param name="pxDelta">размер области карты ( X на Y точек)</param>
        private static void makeWLinkFile(string fname, RectLatLng Area, GPoint pxDelta)
        {
            string wf = fname + "w";
            using (StreamWriter world = File.CreateText(wf))
            {
                world.WriteLine("{0:0.000000000000}", (Area.WidthLng / pxDelta.X));
                world.WriteLine("0.0000000");
                world.WriteLine("0.0000000");
                world.WriteLine("{0:0.000000000000}", (-Area.HeightLat / pxDelta.Y));
                world.WriteLine("{0:0.000000000000}", Area.Left);
                world.WriteLine("{0:0.000000000000}", Area.Top);
                world.Close();
            }
        }


    }
}
