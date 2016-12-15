using System;
using System.Drawing;
using System.Windows.Forms;
using TrackConverter.Lib.Mathematic;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно вычисления расстояний
    /// </summary>
    public partial class FormCalculateDistance : Form
    {
        bool tbLat1 = false;
        bool tbLat2 = false;
        bool tbLon1 = false;
        bool tbLon2 = false;

        /// <summary>
        /// создает новое окно вычисления расстояний
        /// </summary>
        public FormCalculateDistance()
        {
            InitializeComponent();
            buttonCalculate.Focus();

            new ToolTip().SetToolTip(textBoxLat1, "Широта, выраженная в градусах, например: -55,234591");
            new ToolTip().SetToolTip(textBoxLat2, "Широта, выраженная в градусах, например: -55,234591");
            new ToolTip().SetToolTip(textBoxLon1, "Долгота, выраженная в градусах, например: 37,234591");
            new ToolTip().SetToolTip(textBoxLon2, "Долгота, выраженная в градусах, например: 37,234591");
            new ToolTip().SetToolTip(radioButtonGaversin, "Подходит для любых расстояний, если точки не лежат на концах диаметра");
            new ToolTip().SetToolTip(radioButtonModGaver, "Подходит для любых расстояний");
            new ToolTip().SetToolTip(radioButtonPifagor, "Подходит для маленьких расстояний");
            new ToolTip().SetToolTip(radioButtonSphere, "Подходит для больших расстояний");
            new ToolTip().SetToolTip(labelDistance, "Кратчайшее расстояние между точками на поверхности Земли, выраженное в метрах");
            new ToolTip().SetToolTip(labelMagneticAzimuth, "Угол в градусах между направление на магнитный северый полюс и направлением на вторую точку");
            new ToolTip().SetToolTip(labelMagneticDiv, "Разница в градусах между истинным и магнитным азимутами");
            new ToolTip().SetToolTip(labelTrueAzimuth, "Угол в градусах между направление на северый полюс и направлением на вторую точку");


        }

        /// <summary>
        /// кнопка вычисления параметров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            double lat1;
            double lat2;
            double lon1;
            double lon2;

            try
            {
                lat1 = double.Parse(textBoxLat1.Text.Replace('.', ','));
                lat2 = double.Parse(textBoxLat2.Text.Replace('.', ','));
                lon1 = double.Parse(textBoxLon1.Text.Replace('.', ','));
                lon2 = double.Parse(textBoxLon2.Text.Replace('.', ','));
            }
            catch (Exception ee)
            {
                MessageBox.Show(this, ee.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            TrackPoint p1 = new TrackPoint(lat1, lon1);
            TrackPoint p2 = new TrackPoint(lat2, lon2);

            double dist = Math.Round(Vars.CurrentGeosystem.CalculateDistance(p1, p2), 2);

            double trAzi = Math.Round(Vars.CurrentGeosystem.CalculateTrueAzimuth(p1, p2), 2);

            double mnAzi = Math.Round(Vars.CurrentGeosystem.CalculateMagneticAzimuth(p1, p2), 2);

            double mnDiv = Math.Round(Vars.CurrentGeosystem.CalculateMagneticDeclination(p1), 2);

            labelDistance.Text = "Расстояие: " + dist + " м";
            labelTrueAzimuth.Text = "Истинный азимут: " + trAzi + "º";
            labelMagneticAzimuth.Text = "Магнитный азимут: " + mnAzi + "º";
            labelMagneticDiv.Text = "Магнитное склоненение: " + mnDiv + "º";
        }

        #region обнуление значений в текстбоксах

        private void textBoxLat1_Click(object sender, EventArgs e)
        {
            if (!tbLat1)
            {
                textBoxLat1.Text = "";
                textBoxLat1.ForeColor = Color.Black;
                tbLat1 = true;
            }
        }

        private void textBoxLon1_Click(object sender, EventArgs e)
        {
            if (!tbLon1)
            {
                textBoxLon1.Text = "";
                textBoxLon1.ForeColor = Color.Black;
                tbLon1 = true;
            }
        }

        private void textBoxLat2_Click(object sender, EventArgs e)
        {
            if (!tbLat2)
            {
                textBoxLat2.Text = "";
                textBoxLat2.ForeColor = Color.Black;
                tbLat2 = true;
            }
        }

        private void textBoxLon2_Click(object sender, EventArgs e)
        {
            if (!tbLon2)
            {
                textBoxLon2.Text = "";
                textBoxLon2.ForeColor = Color.Black;
                tbLon2 = true;
            }
        }

        #endregion
    }
}
