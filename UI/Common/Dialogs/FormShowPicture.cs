using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// окно вывода изображения
    /// </summary>
    public partial class FormShowPicture : Form
    {
        /// <summary>
        /// адрес изображения
        /// </summary>
        string url;

        /// <summary>
        /// создаёт окно вывода изображения
        /// </summary>
        /// <param name="url"></param>
        public FormShowPicture(string url)
        {
            InitializeComponent();
            this.url = url;
        }

        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Application.StartupPath;
            sf.FileName = Path.GetFileName(url);
           
            if (sf.ShowDialog() == DialogResult.OK)
                pictureBoxImage.BackgroundImage.Save(sf.FileName);
        }

        private void FormShowPicture_Shown(object sender, EventArgs e)
        {
            Task load = null;
            try
            {
                Program.winMain.BeginOperation();
                Program.winMain.setCurrentOperation("Загрузка ихображения...");
                load = new Task(() => {
                    pictureBoxImage.BackgroundImage = BaseConnection.GetImage(url); });
                load.Start();
                load.Wait();
                pictureBoxImage.BackgroundImageLayout = ImageLayout.Zoom;
                this.Text = url;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                MessageBox.Show(this, "Произошла ошибка при загрузке фотографии.\r\nПричина: " + ex.Message, "Загрузка фотографии", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Close();
            }
            finally { Program.winMain.EndOperation(); }

        }
    }
}
