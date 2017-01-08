using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Res.Properties;

namespace TrackConverter.UI.Common
{
    /// <summary>
    /// окно браузера
    /// </summary>
    public partial class FormBrowser : Form
    {
        /// <summary>
        /// создает новое окно браузера с заданным файлом для просмотра
        /// </summary>
        /// <param name="FileName">Имя файла. Если имя файла null то загрузится файл справки</param>
        public FormBrowser(string FileName = null)
        {
            InitializeComponent();

            if (FileName == null)
            {
                this.Text = "Справка";
                FileName = Application.StartupPath + Resources.help_doc_file;
                if (!File.Exists(FileName))
                {
                    MessageBox.Show(this, "Файл справки \"" + Resources.help_doc_file + "\" не найден", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }
            else
                this.Text = "Просмотр документа";
            webBrowser1.Url = new Uri(FileName);
            webBrowser1.Refresh();
            webBrowser1.Navigated += webBrowser1_Navigated;
            buttonForward.Enabled = webBrowser1.CanGoForward;
            buttonGoBack.Enabled = webBrowser1.CanGoBack;
        }


        /// <summary>
        /// событие перехода на новый документ в браузере
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            buttonForward.Enabled = webBrowser1.CanGoForward;
            buttonGoBack.Enabled = webBrowser1.CanGoBack;
        }

        /// <summary>
        /// кнопка назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoBack_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        /// <summary>
        /// кнопка вперед
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonForward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        /// <summary>
        /// кнопка обновить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

    }
}
