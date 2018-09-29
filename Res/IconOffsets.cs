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
        /// иконка маркера обычной точки (красный баллон)
        /// </summary>
        public const int MARKER = 70;

        /// <summary>
        /// иконка начала маршрута
        /// </summary>
        public const int MARKER_START = 72;

        /// <summary>
        /// иконка конца маршрута
        /// </summary>
        public const int MARKER_FINISH = 73;

        /// <summary>
        /// иконка промежуточной точки при построении маршрута
        /// </summary>
        public const int MARKER_INTERMEDIATE = 77;

        /// <summary>
        /// иконки при построении маршрута
        /// </summary>
        public const int CREATING_ROUTE_MARKER = 78;

        /// <summary>
        /// иконка "что здесь"
        /// </summary>
        public const int WHAT_THERE = 74;

        /// <summary>
        /// исконка результатов поиска на карте
        /// </summary>
        public const int SEARCH_RESULT_ICON = 75;

        /// <summary>
        /// выделенная иконка при выделении из графика или списка (большой красный круг)
        /// </summary>
        public const int SELECTED_POINT_ICON = 76;

        /// <summary>
        /// маркер поиска категорий по викимапии
        /// </summary>
        public const int CATEGORY_MARKER = 79;

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
            if (icon == MARKER) return new Point(0, -16);

            //для иконки конца маршрута
            if (icon == MARKER_FINISH) return new Point(0, -16);

            //для иконки начала маршрута
            if (icon == MARKER_START) return new Point(16, -16);

            //иконка Что здесь?
            if (icon == WHAT_THERE) return new Point(0, -16);

            //иконка без сдвига
            if (icon == MARKER_INTERMEDIATE) return new Point(0, 0);

            //иконка результатов поиска
            if (icon == SEARCH_RESULT_ICON)
                return new Point(0, -16);

            //иконка выделенной точки на карте или иконки построения маршрута, маркера категории wikimapia
            if (icon == SELECTED_POINT_ICON || icon == CREATING_ROUTE_MARKER || icon==CATEGORY_MARKER)
                return new Point(0, 0);


            //ИКОНКИ ИЗ ПАПКИ ИКОНОК

            //иконка паровоза 011.ico
            if (icon == 11) return new Point(0, -15);

            //иконка ёлки 016.ico
            if (icon == 16) return new Point(0, -14);

            //иконки, подобные стандартной, но из папки иконок 000.ico - 005.ico
            if (icon > 0 && icon < 6) return new Point(0, -16);

            //иконки "канцелярская кнопка" 006.ico - 008.ico
            if (icon >= 6 && icon < 9) return new Point(7, -16);

            //иконки "квадрат с указателем" 009.ico - 022.ico
            if (icon >= 9 && icon <= 22) return new Point(0, -16);



            //остальные иконки без сдвига
            return new Point(0, 0);
        }
    }
}
