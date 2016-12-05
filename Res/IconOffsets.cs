using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter.Res
{
    /// <summary>
    /// Сдвиги иконок относительно центра при выводе на карту
    /// </summary>
    public static class IconOffsets
    {
        /// <summary>
        /// нулевой сдвиг иконки
        /// </summary>
        public static int ZeroOffset = 9999;

        public const int marker = 70;
        public const int marker_start = 72;
        public const int marker_finish = 73;
        public const int what_there = 74;

        /// <summary>
        /// возращает отступы по осям для отображения маркера на карте для заданной иконки.
        /// </summary>
        /// <param name="icon">номер иконки из TrackPoint.Icon</param>
        /// <returns></returns>
        public static Point GetOffset(int icon)
        {
            if (icon < 1) throw new ArgumentOutOfRangeException("Номер иконки должен быть больше 1");

            //ВСТРОЕННЫЕ ИКОНКИ

            //для стандартной иконки из ресурсов (красный указатель)
            if (icon == marker) return new Point(0, -16);

            //для иконки конца маршрута
            if (icon == marker_finish) return new Point(0, -16);

            //для иконки начала маршрута
            if (icon == marker_start) return new Point(16, -16);

            //иконка Что здесь?
            if (icon == what_there) return new Point(0, -16);

            //иконка без сдвига
            if (icon == ZeroOffset) return new Point(0, 0);





            //ИКОНКИ ИЗ ПАПКИ ИКОНОК

            //иконка паровоза
            if (icon == 11) return new Point(0, -15);

            //иконка ёлки
            if (icon == 16) return new Point(0, -14);

            //иконки, подобные стандартной, но из папки иконок
            if (icon > 0 && icon < 6) return new Point(0, -16);

            //иконки "канцелярская кнопка"
            if (icon >= 6 && icon < 9) return new Point(7, -16);

            //иконки "квадрат с указателем"
            if (icon >= 9 && icon <= 22) return new Point(0, -16);



            //остальные иконки без сдвига
            return new Point(0, 0);
        }
    }
}
