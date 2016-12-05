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
            this.WinSize = new Size(1000, 600);
            this.WinPosition = new Point(50,50);
        }

        /// <summary>
        /// координаты окна
        /// </summary>
        public Point WinPosition { get; set; }

        /// <summary>
        /// размеры окна
        /// </summary>
        public Size WinSize { get; set; }

        /// <summary>
        /// состояние окна
        /// </summary>
        public FormWindowState WinState { get; set; }
    }
}
