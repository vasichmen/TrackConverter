using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// вспомогательные методы при работе с файлами rte
    /// </summary>
    static class RteHelper
    {
        /// <summary>
        /// дописать блок с отдельным треком в файл rte
        /// </summary>
        /// <param name="tf">трек, который надо записать</param>
        /// <param name="number">номер маршрута по порядку</param>
        /// <param name="outputS">объект StreamWriter, куда произвдится запись</param>
        public static void WriteTrackToRTEFile(BaseTrack tf, int number, StreamWriter outputS)
        {
            //заголовок
            //<номер маршрута>,<имя маршрута>,<описание маршрута>,<цвет маршрута>
            string header = string.Format("R,{0},{1},{2},{3}", number, tf.Name, tf.Description, tf.Color.ToArgb());
            outputS.WriteLine(header);
            //точки
            //<номер маршрута>,<номер точки в маршруте>,<имя точки>,<описание точки>,<широта>,<долгота>,<время>,<>,<>,<>,<>,<65535>,<описание>,<0>,<0>,<сходное расстояние>,<высота в футах>
            int i = 0;
            foreach (TrackPoint pt in tf)
            {
                string line = string.Format("W,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                    number, //<номер маршрута>
                    i, //<номер точки в маршруте>
                    pt.Name, //<имя точки>
                    pt.Description, //<описание точки>
                    pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.'), //<широта>
                    pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.'), //<долгота>
                    pt.Time.ToOADate().ToString().Replace(Vars.DecimalSeparator, '.'), //<время>
                    "",
                    "",
                    "",
                    "",
                    65535,
                    "", //<описание>
                    0,
                    0,
                    0,
                    pt.FeetAltitude.ToString().Replace(Vars.DecimalSeparator, '.')); //высота в футах
                i++;
                outputS.WriteLine(line);
            }
        }
    }
}
