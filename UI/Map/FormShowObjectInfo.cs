using GMap.NET;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно подробной информации об объекте
    /// </summary>
    public partial class FormShowObjectInfo: Form
    {
        private VectorMapLayerObject Obj;
        private Wikimapia.ExtInfo info = null;
        /// <summary>
        /// создает окно подробной информации об объекте
        /// </summary>
        /// <param name="obj">объект</param>
        public FormShowObjectInfo(VectorMapLayerObject obj)
        {
            InitializeComponent();
            this.Obj = obj;
        }

        /// <summary>
        /// событие нажатия на изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Wikimapia.ExtInfo.PhotoInfo pho = (sender as PictureBox).Tag as Wikimapia.ExtInfo.PhotoInfo;
            FormShowPicture fsp = new FormShowPicture(info.Photos, info.Photos.IndexOf(pho));
            fsp.Show(this);
        }


        private void formShowObjectInfo_Shown(object sender, EventArgs e)
        {
            this.Text = Obj.Name;
            this.labelName.Text = Obj.Name;

            switch (Obj.LayerProvider)
            {
                case MapLayerProviders.Wikimapia:
                    Task load = null;
                    Task imgs = null;
                    try
                    {
                        Program.winMain.BeginOperation();
                        Program.winMain.SetCurrentOperation("Загрузка информации об объекте...");
                        load = new Task(() =>
                        {
                            if (Vars.dataCache.ContainsLayerObjectExtInfo(Obj.ID))
                                info = Vars.dataCache.GetLayerObjectExtInfo(Obj.ID);
                            else
                            {
                                info = new Wikimapia(Application.StartupPath + Resources.cache_directory + "\\http_cache\\wikimapia\\objects_info").GetExtInfo(Obj);
                                Vars.dataCache.PutLayerObjectExtInfo(info);
                            }
                        });
                        load.Start();
                        load.Wait();

                        //ОСНОВНЫЕ ДАННЫЕ
                        //labelName.Text = info.Title; //на этой строке по неизвестной причине ширина textBoxComments становится равно нулю!!!
                        new ToolTip().SetToolTip(labelName, info.Title);
                        textBoxDescription.Text = info.Description;
                        linkLabelLink.Text = info.Link;
                        if (string.IsNullOrWhiteSpace(info.Description))
                        {
                            this.Height -= textBoxDescription.Height;
                            textBoxDescription.Height = 1;
                        }


                        //КОММЕНТАРИИ
                        string comms = "";
                        foreach (Wikimapia.ExtInfo.CommentInfo com in info.Comments)
                            comms += "===" + com.UserName + "=== " +
                                (com.Date == DateTime.MinValue ? com.StringDate : com.Date.ToString()) +
                                "\r\n" + com.Message + "\r\n\r\n";
                        textBoxComments.Text = comms;
                        if (string.IsNullOrWhiteSpace(comms))
                        {
                            this.Height -= textBoxComments.Height;
                            textBoxComments.Height = 1;
                        }

                        //КАРТИНКИ

                        //метод добавления картинки на панель
                        Action<Image, Wikimapia.ExtInfo.PhotoInfo> addImg = new Action<Image, Wikimapia.ExtInfo.PhotoInfo>((img, pho) =>
                          {
                              this.Invoke(new Action(() =>
                              {
                                  PictureBox pb = new PictureBox
                                  {
                                      BackgroundImage = img,
                                      Width = 120 - 3,
                                      BorderStyle = BorderStyle.FixedSingle,
                                      Parent = flowLayoutPanelImages,
                                      Tag = pho,
                                      Cursor = Cursors.Hand,
                                      BackgroundImageLayout = ImageLayout.Stretch
                                  };
                                  pb.Height = (pb.BackgroundImage.Width / pb.BackgroundImage.Height) * pb.Width;
                                  pb.MouseClick += pictureBox_MouseClick;
                                  new ToolTip().SetToolTip(pb, "Загружено " + pho.TimeString);
                              }));
                          });
                        imgs = new Task(() =>
                        {
                            foreach (Wikimapia.ExtInfo.PhotoInfo pho in info.Photos)
                            {
                                if (Vars.dataCache.CheckImage(pho.UrlThumbnail))
                                {
                                    Image img = Vars.dataCache.GetImage(pho.UrlThumbnail);
                                    addImg.Invoke(img, pho);
                                }
                                else
                                {
                                    Image imag = BaseConnection.GetImage(pho.UrlThumbnail);

                                    //добавлять в кэш можно только ДО использования объекта, иначе - InvalidOperationException
                                    Vars.dataCache.PutImage(pho.UrlThumbnail, imag);
                                    addImg.Invoke(imag, pho);
                                }
                            }
                        });
                        imgs.Start();

                        //НАСТРОЙКИ ВНЕШНЕГО ВИДА ОКНА
                        if (info.Photos.Count == 0)
                        {
                            this.Height -= flowLayoutPanelImages.Height;
                            flowLayoutPanelImages.Height = 1;
                        }
                        if (info.Photos.Count > 4)
                        {
                            this.Height += flowLayoutPanelImages.Height;
                            flowLayoutPanelImages.Height *= 2;
                        }

                        //выделение выбранного объекта
                        Program.winMain.gmapControlMap.SelectPolygon(this.Obj.ID);
                    }
                    catch (Exception ex)
                    {
                        ex = load.IsFaulted ? load.Exception.InnerException : imgs.IsFaulted ? imgs.Exception.InnerException : ex;
                        MessageBox.Show(this, "Произошла ошибка при загрузке информации об объекте.\r\nПричина: " + ex.Message, "Загрузка информации об объекте " + Obj.ID, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        Close();
                    }
                    finally
                    {
                        Program.winMain.EndOperation();
                    }
                    break;
                case MapLayerProviders.None:
                    throw new Exception("Заданный объект из неизвестного источника");
            }
        }

        /// <summary>
        /// при закрытии формы снимаем выделение объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formShowObjectInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.winMain.gmapControlMap.DisSelectPolygon(this.Obj.ID);

            //освобождаем ресурсы картинок
            foreach (PictureBox pb in flowLayoutPanelImages.Controls)
            {
                pb.BackgroundImage = null;
            }
        }

        /// <summary>
        /// переход по ссылке на объект викимапии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Process.Start(linkLabelLink.Text);
        }

        /// <summary>
        /// перевод центра карты на центр объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelCenterMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PointLatLng c = Obj.Bounds.LocationMiddle;
            Program.winMain.gmapControlMap.Position = c;
        }
        /// <summary>
        /// копировать ссылку на объект
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Obj.Link);
        }

        /// <summary>
        /// скопировать ссылку на объект с привязкой к карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyLinkMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // http://wikimapia.org/#lang=ru&lat=55.864909&lon=38.164444&z=12&m=b&show=/29795352/ru
            string base_url = "http://wikimapia.org/#lang=ru&lat={0}&lon={1}&z={2}&m=b&show=/{3}/ru";

            double lat = Obj.GeometryCenter.Latitude;
            double lon = Obj.GeometryCenter.Longitude;
            int z = (int)Program.winMain.gmapControlMap.Zoom;
            int id = Obj.ID;

            string url = string.Format(base_url, lat.ToString().Replace(Vars.DecimalSeparator, '.'), lon.ToString().Replace(Vars.DecimalSeparator, '.'), z, id);
            Clipboard.SetText(url);
        }

    }
}
