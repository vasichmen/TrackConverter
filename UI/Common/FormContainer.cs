using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.Local.ETOPO;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI.Common
{
    /// <summary>
    /// основное окно-контейнер 
    /// </summary>
    public partial class FormContainer : Form
    {
        /// <summary>
        /// метод обновления информации о выполняемой операции
        /// </summary>
        public Action<string> setCurrentOperation;

        /// <summary>
        /// создает экземпляр основного она
        /// </summary>
        public FormContainer()
        {
            InitializeComponent();
            setCurrentOperation = new Action<string>((text) =>
             {
                 if (this.InvokeRequired)
                 {
                     this.Invoke(new Action(() =>
                     {
                         toolStripStatusLabelCurrentOperation.Text = text;
                         Application.DoEvents();
                     }));
                 }
                 else
                 {
                     toolStripStatusLabelCurrentOperation.Text = text;
                     Application.DoEvents();
                 }
             });
        }

        #region основное меню

        /// <summary>
        /// показ справки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string lnk = (Application.StartupPath + Res.Properties.Resources.help_doc_file).Replace("#", "%23");

            //Process.Start(lnk);

            new FormBrowser().Show();
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
            new FormCalculateDistance().Show();
        }

        /// <summary>
        /// преобразование координат разных форматов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransformCoordinateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormTransformCoordinate().Show();
        }

        /// <summary>
        /// открытие окна консоли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormConsole().Show();
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

        #region события

        /// <summary>
        /// изменение размеров дочерних окон
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormContainer_Resize(object sender, EventArgs e)
        {
            if (!Program.winMapNullOrDisposed &&
                !Program.winConverterNullOrDisposed &&
                !Program.winPointsNullOrDisposed &&
                !Program.winElevVisualNullOrDisposed)
            {
                Program.winConverter.Width = (int)(Program.winMain.ClientSize.Width * 0.25);
                Program.winMap.Height = (int)(Program.winMain.ClientSize.Height * 0.66);

                Program.winConverter.Height = Program.winMap.Height;

                Program.winMap.Left = Program.winConverter.Width;
                Program.winMap.Width = Program.winMain.ClientSize.Width - Program.winConverter.Width - 5;

                Program.winElevVisual.Top = Program.winMap.Height;
                Program.winElevVisual.Width = Program.winMap.Width;
                Program.winElevVisual.Height = Program.winMain.ClientSize.Height - Program.winMap.Height - 52;
                Program.winElevVisual.Left = Program.winMap.Left;

                Program.winPoints.Height = Program.winElevVisual.Height;
                Program.winPoints.Width = Program.winConverter.Width;
                Program.winPoints.Top = Program.winConverter.Height;
            }
        }

        /// <summary>
        /// после загрузки окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormContainer_Load(object sender, EventArgs e)
        {
            //установка размеров дочених окон
            this.FormContainer_Resize(null, null);

            //если надо - запускаем загрузку ЕТОРО2
            if (Vars.Options.Common.IsLoadETOPOOnStart)
                if (GeoInfo.ETOPOProvider == null)
                {
                    BeginOperation();
                    Vars.TaskLoadingETOPO.Start();
                }
        }

        /// <summary>
        /// сохранение информации состояния окна при закрытии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Vars.Options.Container.WinSize = this.Size;
            Vars.Options.Container.WinState = this.WindowState;
            Vars.Options.Container.WinPosition = new Point(Left, Top);
        }



        #endregion

        #region взаимодействие

        /// <summary>
        /// окончание выполнения операции (сброс надписей, курсора)
        /// </summary>
        internal void EndOperation()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripStatusLabelCurrentOperation.Visible = false;
                    this.Cursor = Cursors.Arrow;
                    foreach (var f in this.MdiChildren)
                        f.Cursor = Cursors.Arrow;
                }));

            else
            {
                toolStripStatusLabelCurrentOperation.Visible = false;
                this.Cursor = Cursors.Arrow;
                foreach (var f in this.MdiChildren)
                    f.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// начало выполнения асинхронной операции (установка надписей, курсора)
        /// </summary>
        internal void BeginOperation()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripStatusLabelCurrentOperation.Visible = true;
                    this.Cursor = Cursors.AppStarting;
                    foreach (var f in this.MdiChildren)
                        f.Cursor = Cursors.AppStarting;
                }));

            else
            {
                toolStripStatusLabelCurrentOperation.Visible = true;
                this.Cursor = Cursors.AppStarting;
                foreach (var f in this.MdiChildren)
                    f.Cursor = Cursors.AppStarting;
            }
        }


        #endregion


    }
}
