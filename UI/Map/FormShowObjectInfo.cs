using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно подробной информации об объекте
    /// </summary>
    public partial class FormShowObjectInfo : Form
    {
        private VectorMapLayerObject Obj;
        Wikimapia.ExtInfo info = null;
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
        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Wikimapia.ExtInfo.PhotoInfo pho = (sender as PictureBox).Tag as Wikimapia.ExtInfo.PhotoInfo;
            FormShowPicture fsp = new FormShowPicture(info.Photos, info.Photos.IndexOf(pho));
            fsp.Show();
        }


        private void FormShowObjectInfo_Shown(object sender, EventArgs e)
        {
            this.Text = Obj.Name;
            this.labelName.Text = Obj.Name;

            switch (Obj.LayerProvider)
            {
                case VectorMapLayerProviders.Wikimapia:
                    Task load = null;
                    Task imgs = null;
                    try
                    {
                        Program.winMain.BeginOperation();
                        Program.winMain.setCurrentOperation("Загрузка информации об объекте...");
                        load = new Task(() =>
                        {
                            if (Vars.dataCache.ContainsLayerObjectExtInfo(Obj.ID))
                                info = Vars.dataCache.GetLayerObjectExtInfo(Obj.ID);
                            else
                            {
                                info = new Wikimapia(Application.StartupPath + Resources.cache_directory + "\\http_cache\\wikimapia").GetExtInfo(Obj.ID);
                                Vars.dataCache.PutLayerObjectExtInfo(info);
                            }
                        });
                        load.Start();
                        load.Wait();

                        //ОСНОВНЫЕ ДАННЫЕ
                        labelName.Text = info.Title;
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

                        //метод добавления катинки на панель
                        Action<Image, Wikimapia.ExtInfo.PhotoInfo> addImg = new Action<Image, Wikimapia.ExtInfo.PhotoInfo>((img, pho) =>
                          {
                              this.Invoke(new Action(() =>
                              {
                                  PictureBox pb = new PictureBox();
                                  pb.BackgroundImage = img;
                                  pb.Width = 120 - 3;
                                  pb.Height = (int)((pb.BackgroundImage.Width / pb.BackgroundImage.Height) * pb.Width);
                                  pb.BorderStyle = BorderStyle.FixedSingle;
                                  pb.Parent = flowLayoutPanelImages;
                                  pb.MouseClick += PictureBox_MouseClick;
                                  pb.Tag = pho;
                                  pb.Cursor = Cursors.Hand;
                                  pb.BackgroundImageLayout = ImageLayout.Stretch;
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
                case VectorMapLayerProviders.None:
                    throw new Exception("Заданный объект из неизвестного источника");
            }
        }

        /// <summary>
        /// при закрытии формы снимаем выделение объекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormShowObjectInfo_FormClosed(object sender, FormClosedEventArgs e)
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
            Process.Start(linkLabelLink.Text);
        }
    }
}
