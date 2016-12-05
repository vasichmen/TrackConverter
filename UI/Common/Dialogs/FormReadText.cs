using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Policy;
using System.Diagnostics;
using TrackConverter.Lib.Data;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// диалоговое окно для ввода количества точек (при экспорте в ссылку),
    /// для вывода ссылки при экспорте,
    /// для ввода ссылки при импорте
    /// </summary>
    public partial class FormReadText : Form
    {
        /// <summary>
        /// результат рабты диалогового окна
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// количество точек в треке
        /// </summary>
        private int pts;

        /// <summary>
        /// тип открытого диалогового окна
        /// </summary>
        private DialogType dialogType;

        /// <summary>
        /// Открытие диалога с заданными параметрами.
        /// Результат будет записан в поле result, если результат функции FormReadText.ShowDialog() == ...OK
        /// </summary>
        /// <param name="DialogType">тип диалогового окна</param>
        /// <param name="LabelText">Текст надписи, в котором указывается максимльное количество точек трека</param>
        /// <param name="BoxText">Текст внутри текстового блока</param>
        /// <param name="setallEnable">если истина, то кнопка "выбрать все" будет активна</param>
        /// <param name="BoxTextToLink">если истина,то параметр BoxText будет представлен в виде ссылки</param>
        /// <param name="CopyLink">Если истина, то быдет добавлена кнопка Копировать ссылку</param>
        /// <param name="ShortLink">Если истина, то будет добавлена кнопка Сократить ссылку</param>
        public FormReadText(
            DialogType DialogType,
            string LabelText,
            string BoxText,
            bool setallEnable,
            bool BoxTextToLink,
            bool ShortLink,
            bool CopyLink
            )
        {
            InitializeComponent();

            new ToolTip().SetToolTip(buttonSetAll, "Выбрать все точки маршрута");
            new ToolTip().SetToolTip(linkLabelOpenBrowser, "Открыть сохраненную ссылку в браузере");



            dialogType = DialogType;
            label1.Text = LabelText;
            textBox1.Text = BoxText;
            textBox1.SelectAll();
            textBox1.Focus();



            //создание ссылки с содержимым текстового поля
            if (BoxTextToLink)
            {
                linkLabelOpenBrowser.Visible = true;
                linkLabelOpenBrowser.LinkClicked += linkLabelOpenBrowser_LinkClicked;
            }

            //кнопка сокращение ссылки
            buttonShortLink.Visible = ShortLink;

            //кнопка копировать ссылку
            buttonCopyLink.Visible = CopyLink;

            //кнопка выбрать все точки
            buttonSetAll.Visible = setallEnable;

            //поиск количества точек в треке
            pts = -1;
            if (Regex.IsMatch(LabelText, "[0-9]{1,}$"))
            {
                Match mc = Regex.Match(LabelText, "[0-9]{1,}$");
                string mm = mc.Value;
                pts = Convert.ToInt32(mm);
            }
        }

        /// <summary>
        /// нажатие на ссылку и открытие маршрута в браузере
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void linkLabelOpenBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = textBox1.Text;
            Process.Start(link);
        }

        /// <summary>
        /// сохраняет введенные данные в this.result и закрытвает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dialogType == DialogType.ReadNumber)
            {
                Regex re = new Regex(@"\d+");
                Match mc = re.Match(textBox1.Text);
                Result = mc.Value;
            }
            else
                Result = textBox1.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        /// <summary>
        /// отменяет работу диалога
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Result = null;
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// выбор всех точек для экспорта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSetAll_Click(object sender, EventArgs e)
        {
            textBox1.Text = (pts != -1) ? pts.ToString() : "";
            buttonOK_Click(null, null);
        }

        /// <summary>
        /// Сократить ссылку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShortLink_Click(object sender, EventArgs e)
        {
            string l = textBox1.Text;
            string sl = new LinkShorter(Vars.Options.Services.LinkShorterProvider).Short(l);
            textBox1.Text = sl;
        }

        /// <summary>
        /// копирование содержимого тестового поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCopyLink_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("Ссылка скопирована в буфер обмена");
        }



    }
}
