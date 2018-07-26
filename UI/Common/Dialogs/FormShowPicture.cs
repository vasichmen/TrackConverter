using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

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
            this.Text = url;
        }

        /// <summary>
        /// загрузка изображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormShowPicture_Load(object sender, EventArgs e)
        {
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url);
            request1.Method = "GET";
            request1.KeepAlive = true;
            HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();
            pictureBoxImage.BackgroundImage = Image.FromStream(response1.GetResponseStream());
            pictureBoxImage.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void saveImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Application.StartupPath;
            sf.FileName = Path.GetFileName(url);
           
            if (sf.ShowDialog() == DialogResult.OK)
                pictureBoxImage.BackgroundImage.Save(sf.FileName);
        }
    }
}
