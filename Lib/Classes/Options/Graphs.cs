using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// настройки построения графиков и профилей высот
    /// </summary>
    public class Graphs
    {
        /// <summary>
        /// создает новый экземпляр с настройками по умолчанию
        /// </summary>
        public Graphs()
        {
            isXKm = true;
            isYKm = false;
            IntermediateDistance = 100;
            IsAddIntermediatePoints = true;
        }

        /// <summary>
        /// если истина, то ось Х на графике в киломатрах
        /// </summary>
        public bool isXKm { get; set; }

        /// <summary>
        /// если истина, то ось У на графике в киломатрах
        /// </summary>
        public bool isYKm { get; set; }

        /// <summary>
        /// Расстояние в метрах между промежуточными точками при построении профиля высот 
        /// </summary>
        public double IntermediateDistance { get; set; }

        /// <summary>
        /// Если истина, то при построении профиля высот по пути будут добавляться промежуточные точки
        /// </summary>
        public bool IsAddIntermediatePoints { get; set; }
    }
}
