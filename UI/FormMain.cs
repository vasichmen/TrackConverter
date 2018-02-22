using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.UI.Common;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }



        #region Основное меню

        #region Инструменты


        /// <summary>
        /// открытие окна правки точек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPointFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPoints fp = new FormPoints();
            fp.FormBorderStyle = FormBorderStyle.Sizable;
            fp.Show(this);
        }

        /// <summary>
        /// выход из программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// построение профиля высот
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElevProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winConverter.Tracks.Count == 0)
            {
                MessageBox.Show("Для профиля высот должен быть загружен хотя бы один маршрут", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            new FormElevVisual(Program.winConverter.Tracks).Show();
        }

        /// <summary>
        /// расчет расстояния между точками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormCalculateDistance().Show(this);
        }

        /// <summary>
        /// преобразование координат разных форматов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransformCoordinateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormTransformCoordinate().Show(this);
        }

        /// <summary>
        /// открытие окна консоли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormConsole().Show(this);
        }

        /// <summary>
        /// настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winOptionsNullOrDisposed)
                Program.winOptions = new FormOptions();
            if (!Program.winOptions.Visible)
                Program.winOptions.Show(this);
            Program.winOptions.Activate();
        }


        #endregion

        #region Справка

        /// <summary>
        /// показ справки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormBrowser().Show(Program.winMain);
        }

        /// <summary>
        /// вызов окна о программе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog(this);
        }


        #endregion

        #endregion

    }
}
