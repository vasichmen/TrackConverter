using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// настройки окна-контейнера
    /// </summary>
    public class Container
    {
        /// <summary>
        /// создает новый экземпляр настроек с настройками по умолчанию
        /// </summary>
        public Container()
        {
            this.WinState = FormWindowState.Maximized;
            this.WinSize = new System.Drawing.Size(1000, 600);
            this.WinPosition = new System.Drawing.Point(50, 50);

            this.HorizontalRightSplitter = 387;
            this.HorizontalLeftSplitter = 387;
            this.VerticalSplitter = 331;
        }

        /// <summary>
        /// положение разделителей окна
        /// </summary>
        public int HorizontalLeftSplitter { get; set; }

        /// <summary>
        /// положение разделителей окна
        /// </summary>
        public int HorizontalRightSplitter { get; set; }

        /// <summary>
        /// положение разделителей окна
        /// </summary>
        public int VerticalSplitter { get; set; }

        /// <summary>
        /// координаты окна
        /// </summary>
        public System.Drawing.Point WinPosition { get; set; }

        /// <summary>
        /// размеры окна
        /// </summary>
        public System.Drawing.Size WinSize { get; set; }

        /// <summary>
        /// состояние окна
        /// </summary>
        public FormWindowState WinState { get; set; }
    }
}
