using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
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
            refreshEnabling();

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
            : this()
        {
            textBoxDegLatDeg.Text = trackPoint.Coordinates.Latitude.ToString();
            textBoxDegLonDeg.Text = trackPoint.Coordinates.Longitude.ToString();
            buttonTransform_Click(null, null);
        }

        /// <summary>
        /// при изменении значения radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            refreshEnabling();
        }

        /// <summary>
        /// обновление активности элементов при изменении radioButton
        /// </summary>
        private void refreshEnabling()
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
                        double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                        double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator))
                        );

                    //градусы, минуты
                    textBoxDegminLatDeg.Text = ((int)(cr.Latitude)).ToString();
                    textBoxDegminLatMin.Text = (cr.Minutes(Coordinate.CoordinateKind.Latitude) + cr.Seconds(Coordinate.CoordinateKind.Latitude)/60d).ToString("00.0000");
                    textBoxDegminLatText.Text = cr.ToString(Coordinate.CoordinateKind.Latitude,"ddº mm.mmmm' H");

                    textBoxDegminLonDeg.Text = ((int)(cr.Longitude)).ToString();
                    textBoxDegminLatMin.Text = (cr.Minutes(Coordinate.CoordinateKind.Longitude) + cr.Seconds(Coordinate.CoordinateKind.Longitude) / 60d).ToString("00.0000");
                    textBoxDegminLonText.Text = cr.ToString(Coordinate.CoordinateKind.Longitude, "ddº mm.mmmm' H");
                   if(cr.Latitude > 0)
                            comboBoxDegminLat.SelectedIndex = 0;
                   else
                            comboBoxDegminLat.SelectedIndex = 1;
                    if (cr.Longitude > 0)
                        comboBoxDegminLon.SelectedIndex = 0;
                    else
                        comboBoxDegminLon.SelectedIndex = 1;
                  

                    //градусы, минуты, секунды
                    textBoxDegminsecLatDeg.Text = cr.Degrees(Coordinate.CoordinateKind.Latitude).ToString();
                    textBoxDegminsecLatMin.Text = cr.Minutes(Coordinate.CoordinateKind.Latitude).ToString();
                    textBoxDegminsecLatSec.Text = cr.Seconds(Coordinate.CoordinateKind.Latitude).ToString("00.0000");
                    textBoxDegminsecLatText.Text = cr.ToString(Coordinate.CoordinateKind.Latitude,"ddº mm' ss.ssss\" H");

                    textBoxDegminsecLonDeg.Text = cr.Degrees(Coordinate.CoordinateKind.Longitude).ToString();
                    textBoxDegminsecLonMin.Text = cr.Minutes(Coordinate.CoordinateKind.Longitude).ToString();
                    textBoxDegminsecLonSec.Text = cr.Seconds(Coordinate.CoordinateKind.Longitude).ToString("00.0000");
                    textBoxDegminsecLonText.Text = cr.ToString(Coordinate.CoordinateKind.Longitude,"ddº mm' ss.ssss\" H");


                    if (cr.Latitude > 0)
                        comboBoxDegminsecLat.SelectedIndex = 0;
                    else
                        comboBoxDegminsecLat.SelectedIndex = 1;
                    if (cr.Longitude > 0)
                        comboBoxDegminsecLon.SelectedIndex = 0;
                    else
                        comboBoxDegminsecLon.SelectedIndex = 1;
                }

                #endregion

                #region градусы, минуты, секунды

                if (radioButtonDegMinSec.Checked)
                {
                    Coordinate cr =
                        new Coordinate(
                            int.Parse(textBoxDegminsecLatDeg.Text),
                            int.Parse(textBoxDegminsecLatMin.Text),
                            double.Parse(textBoxDegminsecLatSec.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminsecLat.SelectedIndex == 0 ? Coordinate.CoordinateChar.N : Coordinate.CoordinateChar.S,
                            int.Parse(textBoxDegminsecLonDeg.Text),
                            int.Parse(textBoxDegminsecLonMin.Text),
                            double.Parse(textBoxDegminsecLonSec.Text.Replace('.', Vars.DecimalSeparator)),
                            comboBoxDegminsecLon.SelectedIndex == 0 ? Coordinate.CoordinateChar.E : Coordinate.CoordinateChar.W);

                    //градусы, минуты
                    textBoxDegminLatDeg.Text = ((int)(cr.Latitude)).ToString();
                    textBoxDegminLatMin.Text = (cr.Latitude / 60d).ToString("00.0000");

                    textBoxDegminLatText.Text = cr.ToString(Coordinate.CoordinateKind.Latitude, "ddº mm.mmmm' H");
                    textBoxDegminLonDeg.Text = ((int)(cr.Longitude)).ToString();
                    textBoxDegminLatMin.Text = (cr.Longitude / 60d).ToString("00.0000");
                    textBoxDegminLonText.Text = cr.ToString(Coordinate.CoordinateKind.Longitude, "ddº mm.mmmm' H");
                    if (cr.Latitude > 0)
                        comboBoxDegminLat.SelectedIndex = 0;
                    else
                        comboBoxDegminLat.SelectedIndex = 1;
                    if (cr.Longitude > 0)
                        comboBoxDegminLon.SelectedIndex = 0;
                    else
                        comboBoxDegminLon.SelectedIndex = 1;

                    //градусы
                    textBoxDegLatDeg.Text = cr.Latitude.ToString();
                    textBoxDegLonDeg.Text = cr.Longitude.ToString();

                    //градусы, минуты, секунды
                    textBoxDegminsecLatText.Text = cr.ToString(Coordinate.CoordinateKind.Latitude, "ddº mm' ss.ssss\" H");
                    textBoxDegminsecLonText.Text = cr.ToString(Coordinate.CoordinateKind.Longitude, "ddº mm' ss.ssss\" H");

                }

                #endregion

                #region градусы, минуты

                if (radioButtonDegMin.Checked)
                {
                    Coordinate cr = new Coordinate(
                       double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                       double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator))
                       );

                    //градусы
                    textBoxDegLatDeg.Text = cr.Latitude.ToString();
                    textBoxDegLonDeg.Text = cr.Longitude.ToString();

                    //градусы, минуты, секунды
                    textBoxDegminsecLatDeg.Text = cr.Degrees(Coordinate.CoordinateKind.Latitude).ToString();
                    textBoxDegminsecLatMin.Text = cr.Minutes(Coordinate.CoordinateKind.Latitude).ToString();
                    textBoxDegminsecLatSec.Text = cr.Seconds(Coordinate.CoordinateKind.Latitude).ToString("00.0000");
                    textBoxDegminsecLatText.Text = cr.ToString(Coordinate.CoordinateKind.Latitude, "ddº mm' ss.ssss\" H");

                    textBoxDegminsecLonDeg.Text = cr.Degrees(Coordinate.CoordinateKind.Longitude).ToString();
                    textBoxDegminsecLonMin.Text = cr.Minutes(Coordinate.CoordinateKind.Longitude).ToString();
                    textBoxDegminsecLonSec.Text = cr.Seconds(Coordinate.CoordinateKind.Longitude).ToString("00.0000");
                    textBoxDegminsecLonText.Text = cr.ToString(Coordinate.CoordinateKind.Longitude, "ddº mm' ss.ssss\" H");


                    if (cr.Latitude > 0)
                        comboBoxDegminsecLat.SelectedIndex = 0;
                    else
                        comboBoxDegminsecLat.SelectedIndex = 1;
                    if (cr.Longitude > 0)
                        comboBoxDegminsecLon.SelectedIndex = 0;
                    else
                        comboBoxDegminsecLon.SelectedIndex = 1;

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
                               double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                               double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator))
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
                              double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                              double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator))
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

        /// <summary>
        /// точку на центр карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelMap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Coordinate cr = new Coordinate(
                              double.Parse(textBoxDegLatDeg.Text.Replace('.', Vars.DecimalSeparator)),
                              double.Parse(textBoxDegLonDeg.Text.Replace('.', Vars.DecimalSeparator))
                      );
            Program.winMain.gmapControlMap.Position = cr.GMap;
        }
    }
}
