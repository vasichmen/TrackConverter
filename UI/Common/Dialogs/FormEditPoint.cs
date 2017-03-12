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
using TrackConverter.UI.Map;

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
       private TrackPoint editingPoint;

        /// <summary>
        /// создает окно для редактирования точки
        /// </summary>
        /// <param name="point"></param>
        /// <param name="cancelAction">действие при отмене редактирования</param>
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

            comboBoxPointType.SelectedIndex = 0;
            if (point != null)
            {
                textBoxName.Text = string.IsNullOrWhiteSpace(point.Name) ? "Имя точки" : point.Name;
                textBoxLat.Text = point.Coordinates.Latitude.TotalDegrees.ToString();
                textBoxLon.Text = point.Coordinates.Longitude.TotalDegrees.ToString();
                textBoxAlt.Text = point.MetrAltitude.ToString();
                textBoxDescription.Text = point.Description;

                if (point.Time > dateTimePickerDate.MinDate && point.Time < dateTimePickerDate.MaxDate)
                    dateTimePickerDate.Value = point.Time;
                if (point.Time > dateTimePickerTime.MinDate && point.Time < dateTimePickerTime.MaxDate)
                    dateTimePickerTime.Value = point.Time;

                switch (point.PointType)
                {
                    case RouteWaypointType.None:
                        comboBoxPointType.SelectedIndex = 6;
                        break;
                    case RouteWaypointType.Start:
                        comboBoxPointType.SelectedIndex = 0;
                        break;
                    case RouteWaypointType.Interest:
                        comboBoxPointType.SelectedIndex = 1;
                        break;
                    case RouteWaypointType.CollectPoint:
                        comboBoxPointType.SelectedIndex = 2;
                        break;
                    case RouteWaypointType.Camp:
                        comboBoxPointType.SelectedIndex = 3;
                        break;
                    case RouteWaypointType.Overnight:
                        comboBoxPointType.SelectedIndex = 4;
                        break;
                    case RouteWaypointType.Finish:
                        comboBoxPointType.SelectedIndex = 5;
                        break;    
                    default: throw new ApplicationException("неизвестный индекс точки " + comboBoxPointType.SelectedIndex);
                }
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
            //ожидание ввода коректного имени точки
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show(this, "Имя точки не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Result = new TrackPoint(new Coordinate(textBoxLat.Text, textBoxLon.Text))
            {
                Description = textBoxDescription.Text,
                MetrAltitude = textBoxAlt.Text != "" ? double.Parse(textBoxAlt.Text.Replace('.', ',')) : -777,
                Name = textBoxName.Text,
                Icon = comboBoxSelectImage.SelectedIndex == 0 ? 70 : comboBoxSelectImage.SelectedIndex
            };
            DateTime tm = dateTimePickerDate.Value.Date + dateTimePickerTime.Value.TimeOfDay;
            Result.Time = tm;

            RouteWaypointType type;
            switch (comboBoxPointType.SelectedIndex)
            {
                case 0:
                    type = RouteWaypointType.Start;
                    break;
                case 1:
                    type = RouteWaypointType.Interest;
                    break;
                case 2:
                    type = RouteWaypointType.CollectPoint;
                    break;
                case 3:
                    type = RouteWaypointType.Camp;
                    break;
                case 4:
                    type = RouteWaypointType.Overnight;
                    break;
                case 5:
                    type = RouteWaypointType.Finish;
                    break;
                case 6:
                    type = RouteWaypointType.None;
                    break; 
                default: throw new ApplicationException("неизвестный индекс точки " + comboBoxPointType.SelectedIndex);
            }
            Result.PointType = type;
            Close();
        }


        /// <summary>
        /// подробная информация о точке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdditionInfo_Click(object sender, EventArgs e)
        {
            this.editingPoint.CalculateParametres();
            if (Program.winMap.ActiveWhatThereForms.ContainsKey(this.editingPoint))
                Program.winMap.ActiveWhatThereForms[this.editingPoint].Activate();
            else
            {
                FormWhatsthere fw = new FormWhatsthere(this.editingPoint);
                fw.Show(Program.winMain);
                if (Program.winMap.ActiveWhatThereForms == null)
                    Program.winMap.ActiveWhatThereForms = new Dictionary<TrackPoint, FormWhatsthere>();
                Program.winMap.ActiveWhatThereForms.Add(this.editingPoint, fw);
            }
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
            string link = "";
            switch (Vars.Options.DataSources.GeoCoderProvider)
            {
                case GeoCoderProvider.Nominatim:
                case GeoCoderProvider.Yandex:
                    link = new Coordinate(textBoxLat.Text, textBoxLon.Text).ExportYandex();
                    break;
                case GeoCoderProvider.Google:
                    link = new Coordinate(textBoxLat.Text, textBoxLon.Text).ExportGoogle();
                    break;
                default: throw new ApplicationException("Неподдерживаемый геокодер " + Vars.Options.DataSources.GeoCoderProvider);
            }

            new FormReadText(DialogType.ExportText, "Ссылка на точку:", link, false, true, true, true).ShowDialog(this);
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

        /// <summary>
        /// измененеи картинки точки в зависимости от типа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxPointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxPointType.SelectedIndex)
            {
                case 0:
                    comboBoxSelectImage.SelectedIndex = 4; //старт
                    break;
                case 1:
                    comboBoxSelectImage.SelectedIndex = 14; //достопримечательность
                    break;
                case 2:
                    comboBoxSelectImage.SelectedIndex = 3; //точка сбора
                    break;
                case 3:
                    comboBoxSelectImage.SelectedIndex = 16; //привал
                    break;
                case 4:
                    comboBoxSelectImage.SelectedIndex = 9; //место для ночевки
                    break;
                case 5:
                    comboBoxSelectImage.SelectedIndex = 5; //финиш
                    break;
                case 6:
                    comboBoxSelectImage.SelectedIndex = 0; //просто точка
                    break;
            }
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
