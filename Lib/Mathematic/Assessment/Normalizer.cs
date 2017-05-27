using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Geodesy.Models;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Assessment
{
    /// <summary>
    /// удаляет из трека или заменяет точки, которые приводят к острым углам (возникающий из-за погрешности GPS)
    /// </summary>
    static class Normalizer
    {

        /// <summary>
        /// удаление из трека точек, которые приводят к острым углам. Минимальный допустимый угол задаётся параметром
        /// </summary>
        /// <param name="track">трек</param>
        /// <param name="minimalAngle">минимально допустимый угол при нормализации в градусах</param>
        /// <param name="behavior">поведение нормализатора</param>
        public static TrackFile Normalize(TrackFile track, double minimalAngle, NormalizerBehavior behavior)
        {
            switch (behavior)
            {
                case NormalizerBehavior.AddCritical: return AddCritical(track, minimalAngle);
                case NormalizerBehavior.RemovePoint: return RemovePoint(track, minimalAngle);
                default: throw new ArgumentException("неизвестное поведение нормализатора: " + behavior);
            }
        }

        /// <summary>
        /// нормализация с удалением точек
        /// </summary>
        /// <param name="track"></param>
        /// <param name="minimalAngle"></param>
        /// <returns></returns>
        private static TrackFile RemovePoint(TrackFile track, double minimalAngle)
        {
            for (int i = 1; i < track.Count - 1; i++)
            {
                //поиск угла BAC
                double angle = BaseModel.CalculateAngle(
                    track[i].Coordinates, //A
                    track[i - 1].Coordinates, //B
                    track[i + 1].Coordinates, //C
                    AngleMeasure.Degrees);

                //приведение к минимальному углу
                angle = angle > 180 ? 360 - angle : angle; //если измеренный угол больше 180, то вычитаем из 360

                if (angle < minimalAngle)
                {
                    track.Remove(track[i]); //удаление точки
                    i = i < 3 ? 1 : i - 3; //переход на 2 точки назад
                }
            }
            return track;
        }

        /// <summary>
        /// нормализация с добавлением точки с минимальным углом
        /// </summary>
        /// <param name="track"></param>
        /// <param name="minimalAngle"></param>
        /// <returns></returns>
        private static TrackFile AddCritical(TrackFile track, double minimalAngle)
        {

            for (int i = 1; i < track.Count - 1; i++)
            {
                //поиск угла BAC
                double angle = BaseModel.CalculateAngle(
                    track[i].Coordinates, //A
                    track[i - 1].Coordinates, //B
                    track[i + 1].Coordinates, //C
                    AngleMeasure.Degrees);

                //приведение к минимальному углу
                angle = angle > 180 ? 360 - angle : angle; //если измеренный угол больше 180, то вычитаем из 360

                if (angle < minimalAngle)
                {
                    //вычисление координат новой точки
                    Coordinate cord = Coordinate.Empty;
                    throw new NotImplementedException();

                    track.Remove(track[i]);
                    track.Insert(i, new TrackPoint(cord));


                    //переход на 2 точки назад
                    //i = i < 3 ? 1 : i - 3;
                }
            }
            return track;
        }
    }
}
