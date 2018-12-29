using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Mathematic.Astronomy;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно информации о точке
    /// </summary>
    public partial class FormWhatsthere: Form
    {
        private TrackPoint point;
        private Timer timer;

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
        private void formWhatsthere_Load(object sender, EventArgs e)
        {
            Task addr = new Task(new Action(() =>
            {
                if (point.Name == null)
                    try
                    {
                        string addrr = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddress(point.Coordinates);
                        lock (point)
                        {
                            point.Name = addrr;
                        }
                    }
                    catch (Exception exx)
                    {
                        lock (point)
                        {
                            point.Name = "Недоступный адрес";
                        }
                        this.Invoke(new Action(() => MessageBox.Show(null, "Не удалось узнать адрес точки. Причина:\r\n" + exx.Message + "\r\nПопробуйте другой геокодер", "Поиск адреса", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                    }
            }));
            Task alt = new Task(new Action(() =>
            {
                if (double.IsNaN(point.MetrAltitude))
                    try
                    {
                        double altt = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(point.Coordinates);
                        lock (point)
                        {
                            point.MetrAltitude = altt;
                        }
                    }
                    catch (Exception exx)
                    {
                        lock (point)
                        {
                            point.FeetAltitude = double.NaN;
                        }
                        this.Invoke(new Action(() => MessageBox.Show(null, "Не удалось получить высоту точки. Причина:\r\n" + exx.Message, "Получение высоты", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                    }
            }));
            Task tz = new Task(new Action(() =>
            {
                if (point.TimeZone == null)
                    try
                    {
                        TimeZoneInfo tzz = new GeoCoder(GeoCoderProvider.Google).GetTimeZone(point.Coordinates);
                        lock (point)
                        {
                            point.TimeZone = tzz;
                        }
                    }
                    catch (Exception exx)
                    {
                        int offset= AstronomyCalculations.CalculateTimeZone(point.Coordinates);
                        lock (point)
                        {
                            point.TimeZone = TimeZoneInfo.CreateCustomTimeZone(" ", TimeSpan.FromHours(offset), "unknown", "unknown");
                        }
                        this.Invoke(new Action(() => MessageBox.Show(null, "Не удалось получить локальное время. Причина:\r\n" + exx.Message + "\r\nДля вычислений будет использоваться часовой пояс, рассчитанный по координатам точки", "Получение локального времени", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                    }
            }));

            //TODO: этот вариант лучше, но из-за параллельного доступа к файлу data.txt в FileSystemCache бывают проблемы
            //addr.Start();
            //alt.Start();
            //tz.Start();
            //addr.Wait(5000);
            //alt.Wait(5000);
            //tz.Wait(5000);


            addr.Start();
            addr.Wait(5000);
            alt.Start();
            alt.Wait(5000);
            tz.Start();
            tz.Wait(5000);

            point.CalculateParametres();

            this.Text = point.Name == "Недоступный адрес" ? point.Coordinates.ToString("{lat} {lon}", "ddºmm'ss.s\"H") : point.Name;
            this.textBoxAddress.Text = point.Name;
            this.labelRise.Text = point.Rise.ToString();
            this.labelRiseAzi.Text = point.RiseAzi.ToString("00.0000") + "º";
            this.labelFallAzi.Text = point.FallAzi.ToString("00.0000") + "º";
            this.labelFall.Text = point.Fall.ToString();
            this.labelDec.Text = point.MagneticDeclination.ToString("0.0000") + "º";
            this.textBoxLat.Text = point.Coordinates.Latitude.ToString("00.000000") + "º";
            this.textBoxLon.Text = point.Coordinates.Longitude.ToString("00.000000") + "º";
            this.labelAlt.Text = double.IsNaN(point.MetrAltitude) ? "недоступно" : (point.MetrAltitude.ToString() + " м");
            this.labelTimeOffset.Text = (point.TimeZone != null) ? point.TimeZone.Id + (point.TimeZone.BaseUtcOffset.TotalHours > 0 ? " GMT+" : " GMT") + point.TimeZone.BaseUtcOffset.TotalHours.ToString() : "недоступно";
            this.labeldayLength.Text = point.DayLength.ToString();

            //обновление времени
            timer = new Timer
            {
                Interval = 100
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }
        
        private void timer_Tick(object sender, EventArgs e)
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
        private void formWhatsthere_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.winMain.ActiveWhatThereForms.ContainsKey(point))
                Program.winMain.ActiveWhatThereForms.Remove(point);
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
            Program.winMain.gmapControlMap.Position = this.point.Coordinates.GMap;
        }
    }
}
