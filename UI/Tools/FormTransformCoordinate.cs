using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно преобразования координат
    /// </summary>
    public partial class FormTransformCoordinate : Form
    {
        /// <summary>
        /// создает новое окно преобразования координат
        /// </summary>
        public FormTransformCoordinate()
        {
            InitializeComponent();
            comboBoxDegminLat.SelectedIndex = 0;
            comboBoxDegminLon.SelectedIndex = 0;
            comboBoxDegminsecLat.SelectedIndex = 0;
            comboBoxDegminsecLon.SelectedIndex = 0;
            RefreshEnabling();

            new ToolTip().SetToolTip(textBoxDegLatDeg, "Дробное число от -90 до 90");
            new ToolTip().SetToolTip(textBoxDegLonDeg, "Дробное число от -180 до 180");
            new ToolTip().SetToolTip(textBoxDegminLatDeg, "Целое число от 0 до 90");
            new ToolTip().SetToolTip(textBoxDegminLatMin, "Дробное число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminLatText, "Строковое представление координаты");
            new ToolTip().SetToolTip(textBoxDegminLonDeg, "Целое число от 0 до 180");
            new ToolTip().SetToolTip(textBoxDegminLonMin, "Дробное число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminLonText, "Строковое представление координаты");
            new ToolTip().SetToolTip(textBoxDegminsecLatDeg, "Целое число от 0 до 90");
            new ToolTip().SetToolTip(textBoxDegminsecLatMin, "Целое число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminsecLatSec, "Дробное число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminsecLatText, "Строковое представление координаты");
            new ToolTip().SetToolTip(textBoxDegminsecLonDeg, "Целое число от 0 до 180");
            new ToolTip().SetToolTip(textBoxDegminsecLonMin, "Целое число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminsecLonSec, "Дробное число от 0 до 60");
            new ToolTip().SetToolTip(textBoxDegminsecLonText, "Строковое представление координаты"); 
            new ToolTip().SetToolTip(linkLabelGoogle, "Открыть точку c координатами, указанными ниже, на картах Google");
            new ToolTip().SetToolTip(linkLabelYandex, "Открыть точку c координатами, указанными ниже, на картах Яндекс");
        }

        /// <summary>
        /// создает новое  окно и записывает в него указанную точку
        /// </summary>
        /// <param name="trackPoint">точка, которая будет записана в окне</param>
        public FormTransformCoordinate(TrackPoint trackPoint)
            :this()
        {
            textBoxDegLatDeg.Text = trackPoint.Coordinates.Latitude.TotalDegrees.ToString();
            textBoxDegLonDeg.Text = trackPoint.Coordinates.Longitude.TotalDegrees.ToString();
            buttonTransform_Click(null, null);
        }

        /// <summary>
        /// при изменении значения radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RefreshEnabling();
        }

        /// <summary>
        /// обновление активности элементов при изменении radioButton
        /// </summary>
        private void RefreshEnabling()
        {
            textBoxDegLatDeg.ReadOnly = !radioButtonDeg.Checked;
            textBoxDegLonDeg.ReadOnly = !radioButtonDeg.Checked;

            textBoxDegminLatDeg.ReadOnly = !radioButtonDegMin.Checked;
            textBoxDegminLatMin.ReadOnly = !radioButtonDegMin.Checked;
            textBoxDegminLonDeg.ReadOnly = !radioButtonDegMin.Checked;
            textBoxDegminLonMin.ReadOnly = !radioButtonDegMin.Checked;
            comboBoxDegminLat.Enabled = radioButtonDegMin.Checked;
            comboBoxDegminLon.Enabled = radioButtonDegMin.Checked;

            textBoxDegminsecLatDeg.ReadOnly = !radioButtonDegMinSec.Checked;
            textBoxDegminsecLatMin.ReadOnly = !radioButtonDegMinSec.Checked;
            textBoxDegminsecLatSec.ReadOnly = !radioButtonDegMinSec.Checked;
            textBoxDegminsecLonDeg.ReadOnly = !radioButtonDegMinSec.Checked;
            textBoxDegminsecLonMin.ReadOnly = !radioButtonDegMinSec.Checked;
            textBoxDegminsecLonSec.ReadOnly = !radioButtonDegMinSec.Checked;
            comboBoxDegminsecLat.Enabled = radioButtonDegMinSec.Checked;
            comboBoxDegminsecLon.Enabled = radioButtonDegMinSec.Checked;
        }

        /// <summary>
        /// сброс примера в текстовом поле
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxReset(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Tag == null && !tb.ReadOnly)
            {
                tb.Tag = 1;
                tb.ForeColor = Color.Black;
                tb.Text = "";
            }
        }

        /// <summary>
        /// преобразовать координаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTransform_Click(object sender, EventArgs e)
        {
            #region проверка данных

            if (radioButtonDeg.Checked)
            {
                textBoxDegLatDeg.Text = textBoxDegLatDeg.Text.Trim();
                textBoxDegLonDeg.Text = textBoxDegLonDeg.Text.Trim();
                try
                {
                    double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator));
                    double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator));
                }
                catch (FormatException fe)
                {
                    MessageBox.Show(this, fe.Message + "\r\nВозможно, в записи числа присутствуют недопустимые символы или в поле для целого числа было введено дробное.\r\nЧитайте всплывающие подсказки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (radioButtonDegMin.Checked)
            {
                textBoxDegminLatDeg.Text = textBoxDegminLatDeg.Text.Trim();
                textBoxDegminLonDeg.Text = textBoxDegminLonDeg.Text.Trim();
                textBoxDegminLatMin.Text = textBoxDegminLatMin.Text.Trim();
                textBoxDegminLonMin.Text = textBoxDegminLonMin.Text.Trim();
                try
                {
                    int.Parse(textBoxDegminLatDeg.Text);
                    int.Parse(textBoxDegminLonDeg.Text);
                    double.Parse(textBoxDegminLatMin.Text.Replace('.', Vars.DecimalSeparator));
                    double.Parse(textBoxDegminLonMin.Text.Replace('.', Vars.DecimalSeparator));
                }
                catch (FormatException fe)
                {
                    MessageBox.Show(this, fe.Message + "\r\nВозможно, в записи числа присутствуют недопустимые символы или в поле для целого числа было введено дробное.\r\nЧитайте всплывающие подсказки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (radioButtonDegMinSec.Checked)
            {
                textBoxDegminsecLatDeg.Text = textBoxDegminsecLatDeg.Text.Trim();
                textBoxDegminsecLonDeg.Text = textBoxDegminsecLonDeg.Text.Trim();
                textBoxDegminsecLatMin.Text = textBoxDegminsecLatMin.Text.Trim();
                textBoxDegminsecLonMin.Text = textBoxDegminsecLonMin.Text.Trim();
                textBoxDegminsecLatSec.Text = textBoxDegminsecLatSec.Text.Trim();
                textBoxDegminsecLonSec.Text = textBoxDegminsecLonSec.Text.Trim();
                try
                {
                    int.Parse(textBoxDegminsecLatDeg.Text);
                    int.Parse(textBoxDegminsecLonDeg.Text);
                    int.Parse(textBoxDegminsecLatMin.Text);
                    int.Parse(textBoxDegminsecLonMin.Text);
                    double.Parse(textBoxDegminsecLatSec.Text.Replace('.', Vars.DecimalSeparator));
                    double.Parse(textBoxDegminsecLonSec.Text.Replace('.', Vars.DecimalSeparator));
                }
                catch (FormatException fe)
                {
                    MessageBox.Show(this, fe.Message + "\r\nВозможно, в записи числа присутствуют недопустимые символы или в поле для целого числа было введено дробное.\r\nЧитайте всплывающие подсказки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            #endregion

            try
            {

                #region градусы
                if (radioButtonDeg.Checked)
                {
                    Coordinate cr = new Coordinate(
                        new Coordinate.CoordinateRecord(
                            double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                            double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.S : Coordinate.CoordinateChar.N),
                        new Coordinate.CoordinateRecord(
                            double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)),
                            double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.W : Coordinate.CoordinateChar.E)
                    );

                    //градусы, минуты
                    textBoxDegminLatDeg.Text = cr.Latitude.Degrees.ToString();
                    textBoxDegminLatMin.Text = (cr.Latitude.Minutes + cr.Latitude.Seconds / 60.0000).ToString("00.0000");
                    textBoxDegminLatText.Text = cr.Latitude.ToString("ddº mm.mmmm' H");
                    textBoxDegminLonDeg.Text = cr.Longitude.Degrees.ToString();
                    textBoxDegminLonMin.Text = (cr.Longitude.Minutes + cr.Latitude.Seconds / 60.0000).ToString("00.0000");
                    textBoxDegminLonText.Text = cr.Longitude.ToString("ddº mm.mmmm' H");
                    switch (cr.Latitude.Char)
                    {
                        case Coordinate.CoordinateChar.N: comboBoxDegminLat.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.S: comboBoxDegminLat.SelectedIndex = 1;
                            break;
                    }
                    switch (cr.Longitude.Char)
                    {
                        case Coordinate.CoordinateChar.E: comboBoxDegminLon.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.W: comboBoxDegminLon.SelectedIndex = 1;
                            break;
                    }

                    //градусы, минуты, секунды
                    textBoxDegminsecLatDeg.Text = cr.Latitude.Degrees.ToString();
                    textBoxDegminsecLatMin.Text = cr.Latitude.Minutes.ToString();
                    textBoxDegminsecLatSec.Text = cr.Latitude.Seconds.ToString("00.0000");
                    textBoxDegminsecLatText.Text = cr.Latitude.ToString("ddº mm' ss.ssss\" H");
                    textBoxDegminsecLonDeg.Text = cr.Longitude.Degrees.ToString();
                    textBoxDegminsecLonMin.Text = cr.Longitude.Minutes.ToString();
                    textBoxDegminsecLonSec.Text = cr.Longitude.Seconds.ToString("00.0000");
                    textBoxDegminsecLonText.Text = cr.Longitude.ToString("ddº mm' ss.ssss\" H");
                    switch (cr.Latitude.Char)
                    {
                        case Coordinate.CoordinateChar.N: comboBoxDegminsecLat.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.S: comboBoxDegminsecLat.SelectedIndex = 1;
                            break;
                    }
                    switch (cr.Longitude.Char)
                    {
                        case Coordinate.CoordinateChar.E: comboBoxDegminsecLon.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.W: comboBoxDegminsecLon.SelectedIndex = 1;
                            break;
                    }
                }

                #endregion

                #region градусы, минуты, секунды

                if (radioButtonDegMinSec.Checked)
                {
                    Coordinate cr = new Coordinate(
                        new Coordinate.CoordinateRecord(
                            int.Parse(textBoxDegminsecLatDeg.Text),
                            int.Parse(textBoxDegminsecLatMin.Text),
                            double.Parse(textBoxDegminsecLatSec.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminsecLat.SelectedIndex == 0 ? Coordinate.CoordinateChar.N : Coordinate.CoordinateChar.S
                            ),
                        new Coordinate.CoordinateRecord(
                            int.Parse(textBoxDegminsecLonDeg.Text),
                            int.Parse(textBoxDegminsecLonMin.Text),
                            double.Parse(textBoxDegminsecLonSec.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminsecLon.SelectedIndex == 0 ? Coordinate.CoordinateChar.E : Coordinate.CoordinateChar.W
                            )
                    );

                    //градусы, минуты
                    textBoxDegminLatDeg.Text = cr.Latitude.Degrees.ToString();
                    textBoxDegminLatMin.Text = (cr.Latitude.Minutes + cr.Latitude.Seconds / 60.0000).ToString("00.0000");
                    textBoxDegminLatText.Text = cr.Latitude.ToString("ddº mm.mmmm' H");
                    textBoxDegminLonDeg.Text = cr.Longitude.Degrees.ToString();
                    textBoxDegminLonMin.Text = (cr.Longitude.Minutes + cr.Latitude.Seconds / 60.0000).ToString("00.0000");
                    textBoxDegminLonText.Text = cr.Longitude.ToString("ddº mm.mmmm' H");
                    switch (cr.Latitude.Char)
                    {
                        case Coordinate.CoordinateChar.N: comboBoxDegminLat.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.S: comboBoxDegminLat.SelectedIndex = 1;
                            break;
                    }
                    switch (cr.Longitude.Char)
                    {
                        case Coordinate.CoordinateChar.E: comboBoxDegminLon.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.W: comboBoxDegminLon.SelectedIndex = 1;
                            break;
                    }

                    //градусы
                    textBoxDegLatDeg.Text = cr.Latitude.TotalDegrees.ToString();
                    textBoxDegLonDeg.Text = cr.Longitude.TotalDegrees.ToString();

                    //градусы, минуты, секунды
                    textBoxDegminsecLatText.Text = cr.Latitude.ToString("ddº mm' ss.ssss\" H");
                    textBoxDegminsecLonText.Text = cr.Longitude.ToString("ddº mm' ss.ssss\" H");

                }

                #endregion

                #region градусы, минуты

                if (radioButtonDegMin.Checked)
                {
                    Coordinate cr = new Coordinate(
                        new Coordinate.CoordinateRecord(
                            int.Parse(textBoxDegminLatDeg.Text),
                            double.Parse(textBoxDegminLatMin.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminLat.SelectedIndex == 0 ? Coordinate.CoordinateChar.N : Coordinate.CoordinateChar.S
                            ),
                        new Coordinate.CoordinateRecord(
                            int.Parse(textBoxDegminLonDeg.Text),
                            double.Parse(textBoxDegminLonMin.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminLon.SelectedIndex == 0 ? Coordinate.CoordinateChar.E : Coordinate.CoordinateChar.W)
                    );

                    //градусы
                    textBoxDegLatDeg.Text = cr.Latitude.TotalDegrees.ToString();
                    textBoxDegLonDeg.Text = cr.Longitude.TotalDegrees.ToString();

                    //градусы, минуты, секунды
                    textBoxDegminsecLatDeg.Text = cr.Latitude.Degrees.ToString();
                    textBoxDegminsecLatMin.Text = cr.Latitude.Minutes.ToString();
                    textBoxDegminsecLatSec.Text = cr.Latitude.Seconds.ToString("00.0000");
                    textBoxDegminsecLatText.Text = cr.Latitude.ToString("ddº mm' ss.ssss\" H");
                    textBoxDegminsecLonDeg.Text = cr.Longitude.Degrees.ToString();
                    textBoxDegminsecLonMin.Text = cr.Longitude.Minutes.ToString();
                    textBoxDegminsecLonSec.Text = cr.Longitude.Seconds.ToString("00.0000");
                    textBoxDegminsecLonText.Text = cr.Longitude.ToString("ddº mm' ss.ssss\" H");
                    switch (cr.Latitude.Char)
                    {
                        case Coordinate.CoordinateChar.N: comboBoxDegminsecLat.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.S: comboBoxDegminsecLat.SelectedIndex = 1;
                            break;
                    }
                    switch (cr.Longitude.Char)
                    {
                        case Coordinate.CoordinateChar.E: comboBoxDegminsecLon.SelectedIndex = 0;
                            break;
                        case Coordinate.CoordinateChar.W: comboBoxDegminsecLon.SelectedIndex = 1;
                            break;
                    }

                    //градусы, минуты
                    textBoxDegminLonText.Text = cr.Longitude.ToString("ddº mm.mmmm' H");
                    textBoxDegminLatText.Text = cr.Latitude.ToString("ddº mm.mmmm' H");

                }

                #endregion
            }
            catch (ArgumentOutOfRangeException or)
            {
                MessageBox.Show(this, or.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (FormatException fe)
            {
                MessageBox.Show(this, fe.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ArgumentNullException an)
            {
                MessageBox.Show(this, an.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        /// <summary>
        /// Открытие точки на картах яндекс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelYandex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Coordinate cr = new Coordinate(
                           new Coordinate.CoordinateRecord(
                               double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                               double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.S : Coordinate.CoordinateChar.N),
                           new Coordinate.CoordinateRecord(
                               double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)),
                               double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.W : Coordinate.CoordinateChar.E)
                       );

                string link = cr.ExportYandex();
                Process pr = Process.Start(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// открытие точки накартах google
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelGoogle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Coordinate cr = new Coordinate(
                           new Coordinate.CoordinateRecord(
                               double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                               double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.S : Coordinate.CoordinateChar.N),
                           new Coordinate.CoordinateRecord(
                               double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)),
                               double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator)) <= 0 ? Coordinate.CoordinateChar.W : Coordinate.CoordinateChar.E)
                       );

                string link = cr.ExportGoogle();
                Process pr = Process.Start(link);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
