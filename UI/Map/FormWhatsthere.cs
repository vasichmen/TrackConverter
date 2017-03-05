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
            this.Text = point.Name;
            this.labelAddress.Text = point.Name;
            this.labelRise.Text = point.Rise.ToString("HH:MM");
            this.labelRiseAzi.Text = point.RiseAzi.ToString("00.0000") + "º";
            this.labelFallAzi.Text = point.FallAzi.ToString("00.0000") + "º";
            this.labelFall.Text = point.Fall.ToString("HH:MM");
            this.labelDec.Text = point.MagneticDeclination.ToString("0.0000") + "º";
            this.labelLat.Text = point.Coordinates.Latitude.TotalDegrees.ToString("00.000000") + "º";
            this.labelLon.Text = point.Coordinates.Longitude.TotalDegrees.ToString("00.000000") + "º";
            this.labelAlt.Text = double.IsNaN(point.MetrAltitude) ? "недоступно" : (point.MetrAltitude.ToString() + " м");
            this.labelLocalTime.Text = point.CurrentTime.ToString();
            this.labelTimeOffset.Text = ((point.TimeOffset >= 0) ? "GMT+" : "GMT") + point.TimeOffset.ToString("00");
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
