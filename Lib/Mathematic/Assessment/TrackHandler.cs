using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Assessment
{
    /// <summary>
    /// класс, выполняющий преобразования и анализ треков 
    /// </summary>
    public class TrackHandler
    {
        /// <summary>
        /// аппроксимация высот трека полиномом заданной степени
        /// </summary>
        /// <param name="tf">трек</param>
        /// <param name="amount">степень полинома</param>
        /// <returns></returns>
        public static BaseTrack Approximate(BaseTrack tf, int amount)
        {
            return Approximator.Approximate(tf, amount);
        }

        /// <summary>
        /// анализ треков и вывод данных в виде таблицы
        /// </summary>
        /// <param name="tracks">треки для анализа</param>
        /// <returns></returns>
        public static DataTable AnalyzeTracks(TrackFileList tracks)
        {
            return ElevationAnalysis.AnalyzeTracks(tracks);
        }

        /// <summary>
        /// удаление из трека точек, которые приводят к острым углам. Минимальный допустимый угол задаётся параметром
        /// </summary>
        /// <param name="track">трек</param>
        /// <param name="minimalAngle">минимально допустимый угол при нормализации в градусах</param>
        /// <param name="behavior">поведение нормализатора</param>
        /// <returns></returns>
        public static TrackFile Normalize(TrackFile track, double minimalAngle, NormalizerBehavior behavior)
        {
            return Normalizer.Normalize(track, minimalAngle, behavior);
        }
    }
}
