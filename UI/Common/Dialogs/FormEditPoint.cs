using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// окно изменнения или добавления точки
    /// </summary>
    public partial class FormEditPoint : Form
    {

        /// <summary>
        /// редактируемая точка
        /// </summary>
        TrackPoint editingPoint;

        /// <summary>
        /// создает окно для редактирования точки
        /// </summary>
        /// <param name="point"></param>
        public FormEditPoint(TrackPoint point)
        {
            InitializeComponent();

            editingPoint = point;

            //заполнение иконок в выборе значка на карте

            //значок по умолчанию
            comboBoxSelectImage.Items.Add(Resources.marker);

            //значки из папки значков
            List<string> files = Directory.EnumerateFiles(Application.StartupPath + Resources.icons_directory, @"*.ico", SearchOption.TopDirectoryOnly).ToList<string>();

            comboBoxSelectImage.SuspendLayout();
            foreach (string file in files)
            {
                //выбираем только картинки с цифрами в имени
                string nm = Path.GetFileNameWithoutExtension(file);
                if (Regex.IsMatch(nm, @"^[0-9]+$"))
                {
                    Icon ic = new Icon(file);
                    comboBoxSelectImage.Items.Add(ic);
                }
            }
            comboBoxSelectImage.ResumeLayout();


            //заполнение информации о точке
            if (point == null || point.Icon == 70 || point.Icon == IconOffsets.ZeroOffset)
                comboBoxSelectImage.SelectedIndex = 0;
            else
                comboBoxSelectImage.SelectedIndex = point.Icon;


            if (point != null)
            {
                textBoxName.Text = point.Name;
                textBoxLat.Text = point.Coordinates.Latitude.TotalDegrees.ToString();
                textBoxLon.Text = point.Coordinates.Longitude.TotalDegrees.ToString();
                textBoxAlt.Text = point.MetrAltitude.ToString();
                textBoxDescription.Text = point.Description;

                try
                {
                    dateTimePickerDate.Value = point.Time;
                }
                catch (Exception) { }
                try
                {
                    dateTimePickerTime.Value = point.Time;
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// созадет новое окно  для создания новой точки
        /// </summary>
        public FormEditPoint()
            : this(null) { }

        /// <summary>
        /// результат работы диалогового окна
        /// </summary>
        public TrackPoint Result { get; set; }

        /// <summary>
        /// сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Result = new TrackPoint(new Coordinate(textBoxLat.Text, textBoxLon.Text))
            {
                Description = textBoxDescription.Text,
                MetrAltitude = textBoxAlt.Text != "" ? double.Parse(textBoxAlt.Text.Replace('.', ',')) : -777,
                Name = textBoxName.Text,
                Icon = comboBoxSelectImage.SelectedIndex == 0 ? 70 : comboBoxSelectImage.SelectedIndex
            };
            DateTime tm = dateTimePickerDate.Value.Date + dateTimePickerTime.Value.TimeOfDay;
            Result.Time = tm;
            Close();
        }

        /// <summary>
        /// отмена изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// получить высоту точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelGetElevation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                textBoxAlt.Text = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(new Coordinate(textBoxLat.Text, textBoxLon.Text)).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// получить координаты по адресу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelFindCoordinates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (textBoxName.Text == "")
            {
                MessageBox.Show("Сначала введите адрес в поле имени точки");
                return;
            }
            try
            {
                string adr = textBoxName.Text;
                Coordinate cd = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetCoordinate(adr);
                textBoxLat.Text = cd.Latitude.TotalDegrees.ToString();
                textBoxLon.Text = cd.Longitude.TotalDegrees.ToString();
                Program.winMap.gmapControlMap.Position = cd.GMap;
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Ошибка при подключении к серверу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// получить ссылку на точку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelGetLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string link = new Coordinate(textBoxLat.Text,textBoxLon.Text).ExportYandex();
            //сокращение ссылок образает основную ссылку по координаты. Остается только координаты ценра карты
            //string slink = new LinkShorter( Vars.Options.Map.LinkShorterProvider ).Short( link );
            new FormReadText(DialogType.ExportText, "Ссылка на точку:", link, false, true, true, true).Show(this);
        }

        /// <summary>
        /// отрисовка картинки в ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= comboBoxSelectImage.Items.Count)
                return;

            if (!(comboBoxSelectImage.Items[e.Index] is Icon))
                return;

            Icon item = comboBoxSelectImage.Items[e.Index] as Icon;

            Graphics g = e.Graphics;

            e.DrawBackground();

            g.DrawImage(item.ToBitmap(), e.Bounds.X, e.Bounds.Y, 40, 40);

            if (e.Index != comboBoxSelectImage.SelectedIndex)
                e.DrawFocusRectangle();

        }


        #region Панель инструментов описания
        private void toolStripButtonBold_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<b>" + newPart + "</b>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<b></b>");
            }
        }
        private void toolStripButtonItalic_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<i>" + newPart + "</i>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<i></i>");
            }
        }
        private void toolStripButtonUnderline_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<u>" + newPart + "</u>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<u></u>");
            }
        }
        private void toolStripButtonAlignLeft_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<div ALIGN=LEFT>" + newPart + "</div>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<div ALIGN=LEFT></div>");
            }
        }
        private void toolStripButtonAlignCenter_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<CENTER>" + newPart + "</CENTER>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<CENTER></CENTER>");
            }
        }
        private void toolStripButtonAlignRight_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.SelectionLength != 0)
            {
                int strt = textBoxDescription.SelectionStart;
                string newPart = textBoxDescription.Text.Substring(strt, textBoxDescription.SelectionLength);
                textBoxDescription.Text = textBoxDescription.Text.Remove(strt, textBoxDescription.SelectionLength);
                newPart = "<div ALIGN=RIGHT>" + newPart + "</div>";
                textBoxDescription.Text = textBoxDescription.Text.Insert(strt, newPart);
            }
            else
            {
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<div ALIGN=RIGHT></div>");
            }
        }
        private void toolStripButtonInsImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
            of.Filter = "Все изображения(*.jpg, *.jpeg, *.png, *.bmp, *.ico, *.wmf, *.emf)|*.jpg; *.jpeg; *.png; *.bmp; *.ico; *.wmf; *.emf";
            if (of.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<img src=\"" + of.FileName + "\"/>");
        }
        private void toolStripButtonInsLink_Click(object sender, EventArgs e)
        {
            FormReadText frt = new FormReadText(DialogType.ReadText, "Введите ссылку", "", false, false, false, false);
            if (frt.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                textBoxDescription.Text = textBoxDescription.Text.Insert(textBoxDescription.SelectionStart, "<a href=\"" + frt.Result + "\"></a>");
        }
        private void toolStripButtonPreview_Click(object sender, EventArgs e)
        {
            string file = Application.StartupPath + Resources.temp_directory + "\\description.html";
            Directory.CreateDirectory(Application.StartupPath + Resources.temp_directory);
            StreamWriter sw = new StreamWriter(file, false, Encoding.UTF8);
            sw.WriteLine(textBoxDescription.Text);
            sw.Close();
            new FormBrowser(file).ShowDialog(this);
            File.Delete(file);
        }

        #endregion


    }
}
