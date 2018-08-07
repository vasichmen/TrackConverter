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
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно подробной информации об объекте
    /// </summary>
    public partial class FormShowObjectInfo : Form
    {
        private VectorMapLayerObject Obj;

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
            FormShowPicture fsp = new FormShowPicture(
                !string.IsNullOrWhiteSpace(pho.UrlFull) ? pho.UrlFull :
                !string.IsNullOrWhiteSpace(pho.UrlBig) ? pho.UrlBig :
                !string.IsNullOrWhiteSpace(pho.Url1280) ? pho.Url1280 : pho.Url960);
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
                        Wikimapia.ExtInfo info = null;
                        load = new Task(() =>
                        {
                            if (Vars.dataCache.ContainsVectorMapLayerObject(Obj.ID))
                                info = Vars.dataCache.GetVectorMapLayerObject(Obj.ID);
                            else
                            {
                                info = new Wikimapia().GetExtInfo(Obj.ID);
                                Vars.dataCache.PutVectorMapLayerObject(info);
                            }
                        });
                        load.Start();
                        load.Wait();

                        //основные данные
                        labelName.Text = info.Title;
                        new ToolTip().SetToolTip(labelName, info.Title);
                        textBoxDescription.Text = info.Description;
                        linkLabelLink.Text = info.Link;
                        if (string.IsNullOrWhiteSpace(info.Description))
                        {
                            this.Height -= textBoxDescription.Height;
                            textBoxDescription.Height = 1;
                        }


                        //комментарии
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

                        //картинки
                        imgs = new Task(() =>
                        {
                            foreach (Wikimapia.ExtInfo.PhotoInfo pho in info.Photos)
                            {
                                Image img;
                                if (Vars.dataCache.CheckImage(pho.UrlThumbnail))
                                    img = Vars.dataCache.GetImage(pho.UrlThumbnail);
                                else
                                {
                                    img = BaseConnection.GetImage(pho.UrlThumbnail);
                                    Vars.dataCache.PutImage(pho.UrlThumbnail, img);
                                }

                                this.Invoke(new Action(() =>
                                {
                                    PictureBox pb = new PictureBox();
                                    pb.BackgroundImage = img;
                                    pb.Width = 100;
                                    pb.Height = (int)((pb.BackgroundImage.Width / pb.BackgroundImage.Height) * 100);
                                    pb.BorderStyle = BorderStyle.FixedSingle;
                                    pb.Parent = flowLayoutPanelImages;
                                    pb.MouseClick += PictureBox_MouseClick;
                                    pb.Tag = pho;
                                    pb.Cursor = Cursors.Hand;
                                    pb.BackgroundImageLayout = ImageLayout.Stretch;
                                }));
                            }
                        });
                        imgs.Start();
                        if (info.Photos.Count == 0)
                        {
                            this.Height -= flowLayoutPanelImages.Height;
                            flowLayoutPanelImages.Height = 1;
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
