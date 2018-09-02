using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// окно вывода изображения
    /// </summary>
    public partial class FormShowPicture: Form
    {
        /// <summary>
        /// адрес изображения
        /// </summary>
        private List<Wikimapia.ExtInfo.PhotoInfo> photos;

        /// <summary>
        /// текущий номер изображения
        /// </summary>
        private int cur_ind;

        /// <summary>
        /// первое изображение, которое надо показать при запуске
        /// </summary>
        private int start;

        /// <summary>
        /// url текущего изображения
        /// </summary>
        private string cur_url;

        /// <summary>
        /// тултип на изображении (ссылка)
        /// </summary>
        private ToolTip tooltip = new ToolTip();

        /// <summary>
        /// метод установки заголовка окна
        /// </summary>
        private Action<string> setOperation;

        /// <summary>
        /// создаёт окно вывода изображения
        /// </summary>
        /// <param name="url"></param>
        public FormShowPicture(List<Wikimapia.ExtInfo.PhotoInfo> photos, int start)
        {
            InitializeComponent();
            this.photos = photos;
            this.start = start;

            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;

            setOperation = new Action<string>((obj) =>
            {
                try
                {
                    if (this.InvokeRequired)
                        this.Invoke(new Action(() =>
                        {
                            if (this.IsDisposed)
                                return;
                            else
                                this.Text = obj;
                        }));

                    else
                    {
                        if (this.IsDisposed)
                            return;
                        else
                            this.Text = obj;
                    }
                }
                catch (Exception) { }
            });

        }

        private void formShowPicture_Shown(object sender, EventArgs e)
        {
            showPict(start);
            cur_ind = start;
            Text = "Изображение " + (cur_ind + 1) + "/" + photos.Count;
        }

        /// <summary>
        /// переход на другое изображение по клику 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X;
                int c = pictureBoxImage.Width / 2;


                if (x < c) //предыдущее изображение
                    if (cur_ind == 0)
                        return;
                    else
                    {
                        cur_ind--;
                        showPict(cur_ind);
                    }
                else //следующее изображение
                {
                    if (cur_ind == photos.Count - 1)
                        return;
                    else
                    {
                        cur_ind++;
                        showPict(cur_ind);
                    }
                }
                Text = "Изображение " + (cur_ind + 1) + "/" + photos.Count + ", загружено " + photos[cur_ind].TimeString;
            }
        }
        /// <summary>
        /// загрузить и показать указанное изображени по ссылке
        /// </summary>
        /// <param name="url"></param>
        private void showPict(int ind)
        {
            Wikimapia.ExtInfo.PhotoInfo pho = photos[ind];
            cur_url = !string.IsNullOrWhiteSpace(pho.UrlFull) ? pho.UrlFull :
                      !string.IsNullOrWhiteSpace(pho.UrlBig) ? pho.UrlBig :
                      !string.IsNullOrWhiteSpace(pho.Url1280) ? pho.Url1280 : pho.Url960;
            Task load = null;
            try
            {
                load = new Task(() =>
                {

                    if (Vars.dataCache.CheckImage(cur_url))
                    {
                        Image img = Vars.dataCache.GetImage(cur_url);
                        pictureBoxImage.Image = img;
                    }
                    else
                    {
                        BaseConnection.GetFileAsync(cur_url, setOperation, new Action<string>((file) =>
                        {
                            Image imag = Image.FromFile(file);
                            Vars.dataCache.PutImage(cur_url, imag); //добавлять в кэш можно только ДО использования объекта, иначе - InvalidOperationException
                            pictureBoxImage.Image = imag;
                            setOperation.Invoke("Изображение " + (cur_ind + 1) + "/" + photos.Count + ", загружено " + photos[cur_ind].TimeString);
                        }));
                    }


                });
                load.Start();
                load.Wait();
                tooltip.SetToolTip(pictureBoxImage, cur_url);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                MessageBox.Show(this, "Произошла ошибка при загрузке фотографии.\r\nПричина: " + ex.Message, "Загрузка фотографии", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Close();
            }
        }

        private void formShowPicture_FormClosed(object sender, FormClosedEventArgs e)
        {
            pictureBoxImage.Image = null;
        }

        #region контекстное меню картинки

        private void copyLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Wikimapia.ExtInfo.PhotoInfo pho = photos[cur_ind];
            string link = !string.IsNullOrWhiteSpace(pho.UrlFull) ? pho.UrlFull :
                      !string.IsNullOrWhiteSpace(pho.UrlBig) ? pho.UrlBig :
                      !string.IsNullOrWhiteSpace(pho.Url1280) ? pho.Url1280 : pho.Url960;
            Clipboard.SetText(link);
        }

        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBoxImage.Image == null)
            {
                MessageBox.Show(this, "Дождитесь загрузки изображения!", "Копирование изображения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Clipboard.SetImage(pictureBoxImage.Image);
        }

        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                InitialDirectory = Application.StartupPath,
                FileName = Path.GetFileName(cur_url)
            };

            if (sf.ShowDialog() == DialogResult.OK)
                pictureBoxImage.Image.Save(sf.FileName);
        }

        #endregion

    }
}
