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
        List<Wikimapia.ExtInfo.PhotoInfo> photos;

        /// <summary>
        /// текущий номер изображения
        /// </summary>
        int cur_ind;

        /// <summary>
        /// первое изображение, которое надо показать при запуске
        /// </summary>
        int start;

        /// <summary>
        /// url текущего изображения
        /// </summary>
        string cur_url;

        /// <summary>
        /// создаёт окно вывода изображения
        /// </summary>
        /// <param name="url"></param>
        public FormShowPicture(List<Wikimapia.ExtInfo.PhotoInfo> photos, int start)
        {
            InitializeComponent();
            this.photos = photos;
            this.start = start;
            this.Text = "Загрузка изображения...";
        }

        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Application.StartupPath;
            sf.FileName = Path.GetFileName(cur_url);

            if (sf.ShowDialog() == DialogResult.OK)
                pictureBoxImage.BackgroundImage.Save(sf.FileName);
        }

        private void FormShowPicture_Shown(object sender, EventArgs e)
        {
            ShowPict(start);
            cur_ind = start;
            Text = "Изображение " +( cur_ind + 1) + "/" + photos.Count;
        }

        /// <summary>
        /// переход на другое изображение по клику 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxImage_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int c = pictureBoxImage.Width / 2;


            if (x < c) //предыдущее изображение
                if (cur_ind == 0)
                    return;
                else
                {
                    cur_ind--;
                    ShowPict(cur_ind);
                }
            else //следующее изображение
            {
                if (cur_ind == photos.Count - 1)
                    return;
                else
                {
                    cur_ind++;
                    ShowPict(cur_ind);
                }
            }
            Text = "Изображение " + (cur_ind + 1) + "/" + photos.Count;
        }
        /// <summary>
        /// загрузить и показать указанное изображени по ссылке
        /// </summary>
        /// <param name="url"></param>
        private void ShowPict(int ind)
        {
            Wikimapia.ExtInfo.PhotoInfo pho = photos[ind];
            cur_url = !string.IsNullOrWhiteSpace(pho.UrlFull) ? pho.UrlFull :
                      !string.IsNullOrWhiteSpace(pho.UrlBig) ? pho.UrlBig :
                      !string.IsNullOrWhiteSpace(pho.Url1280) ? pho.Url1280 : pho.Url960;
            Task load = null;
            try
            {
                Program.winMain.BeginOperation();
                Program.winMain.setCurrentOperation("Загрузка изображения...");
                load = new Task(() =>
                {
                    Image img;
                    if (Vars.dataCache.CheckImage(cur_url))
                        img = Vars.dataCache.GetImage(cur_url);
                    else
                    {
                        img = BaseConnection.GetImage(cur_url);
                        Vars.dataCache.PutImage(cur_url, img);
                    }

                    pictureBoxImage.BackgroundImage = img;

                });
                load.Start();
                load.Wait();
                pictureBoxImage.BackgroundImageLayout = ImageLayout.Zoom;
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
