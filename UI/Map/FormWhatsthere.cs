using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TrackConverter;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно информации о точке
    /// </summary>
    public partial class FormWhatsthere : Form
    {
        TrackPoint point;
        Timer timer;

        /// <summary>
        /// создает новое окно информации о точке с заданными данными
        /// </summary>
        /// <param name="point"></param>
        public FormWhatsthere(TrackPoint point)
        {
            InitializeComponent();
            this.point = point;
        }

        /// <summary>
        /// обновление информации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormWhatsthere_Load(object sender, EventArgs e)
        {
            if (point.Name == null)
                try
                {
                    point.Name = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddress(point.Coordinates);
                }
                catch (Exception exx)
                {
                    MessageBox.Show(this, "Не удалось узнать адрес точки:\r\n" + exx.Message, "Поиск адреса", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    point.Name = "Недоступный адрес";
                }

            if (double.IsNaN(point.MetrAltitude))
                try
                {
                    point.MetrAltitude = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(point.Coordinates);
                }
                catch (Exception exx)
                {
                    MessageBox.Show(this, "Не удалось получить высоту точки:\r\n" + exx.Message, "Получение высоты", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    point.FeetAltitude = double.NaN;
                }


            this.Text = point.Name == "Недоступный адрес" ? point.Coordinates.ToString("{lat} {lon}", "ddºmm'ss.s\"H") : point.Name;
            this.textBoxAddress.Text = point.Name;
            this.labelRise.Text = point.Rise.ToString();
            this.labelRiseAzi.Text = point.RiseAzi.ToString("00.0000") + "º";
            this.labelFallAzi.Text = point.FallAzi.ToString("00.0000") + "º";
            this.labelFall.Text = point.Fall.ToString();
            this.labelDec.Text = point.MagneticDeclination.ToString("0.0000") + "º";
            this.textBoxLat.Text = point.Coordinates.Latitude.TotalDegrees.ToString("00.000000") + "º";
            this.textBoxLon.Text = point.Coordinates.Longitude.TotalDegrees.ToString("00.000000") + "º";
            this.labelAlt.Text = double.IsNaN(point.MetrAltitude) ? "недоступно" : (point.MetrAltitude.ToString() + " м");
            this.labelTimeOffset.Text = ((point.TimeOffset >= 0) ? "GMT+" : "GMT") + point.TimeOffset.ToString("00");
            this.labeldayLength.Text = point.DayLength.ToString();

            //обновление времени
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
          this.labelLocalTime.Text = point.CurrentTime.ToString())
          );

        }

        /// <summary>
        /// при закрытии окна удаляется из словаря окно с точкой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormWhatsthere_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.winMap.ActiveWhatThereForms.ContainsKey(point))
                Program.winMap.ActiveWhatThereForms.Remove(point);
            timer.Stop();
        }

        /// <summary>
        /// закрытие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// на центр карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCenter_Click(object sender, EventArgs e)
        {
            Program.winMap.gmapControlMap.Position = this.point.Coordinates.GMap;
        }
    }
}
