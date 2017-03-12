using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно редактирования путешествия
    /// </summary>
    public partial class FormEditTrip : Form
    {
        private Action<TripRouteFile> afterAction;
        private Action cancelAction;
        private TripRouteFile trip;
        private BaseTrack selectedTrack;
        private TrackPoint selectedPoint;
        private bool cancel = false;

        private FormEditTrip()
        {
            InitializeComponent();
        }

        /// <summary>
        /// создает новое окно редактирования путешествия
        /// </summary>
        /// <param name="caption">заголовок окна</param>
        /// <param name="tripFile">путешествие для редактирования</param>
        /// <param name="afterAction">действие после сохаренеия путешествия</param>
        /// <param name="cancelAction">действие при отмене редактирования</param>
        public FormEditTrip(string caption, TripRouteFile tripFile, Action<TripRouteFile> afterAction, Action cancelAction) : this()
        {
            this.Text = caption;
            this.trip = tripFile;
            this.afterAction = afterAction;
            this.cancelAction = cancelAction;
            this.selectedTrack = tripFile;
            FillDGV(tripFile);
        }

        /// <summary>
        /// заполнение таблицы заданным путешествием и обновление карты 
        /// </summary>
        /// <param name="tripRoute">путешествие</param>
        private void FillDGV(TripRouteFile tripRoute)
        {
            //маршруты
            DataTable sourceD = new DataTable();
            sourceD.Columns.Add("Название");
            sourceD.Columns["Название"].DataType = typeof(string);
            sourceD.Columns.Add("Длина, км");
            sourceD.Columns["Длина, км"].DataType = typeof(double);
            sourceD.LoadDataRow(
                new object[] {
                      tripRoute.Name,
                      tripRoute.Distance
                }
                , true
                );
            sourceD = tripRoute.GetDaysRoutesTable(sourceD);
            dataGridViewDays.DataSource = null;
            dataGridViewDays.DataSource = sourceD;
            dataGridViewDays.Refresh();

            //путевые точки
            DataTable sourceW = new DataTable();
            sourceW.Columns.Add("Название");
            sourceW.Columns["Название"].DataType = typeof(string);
            sourceW.Columns.Add("Тип точки");
            sourceW.Columns["Тип точки"].DataType = typeof(string);
            sourceW = tripRoute.GetWaypointsTable(sourceW);
            dataGridViewWaypoints.DataSource = null;
            dataGridViewWaypoints.DataSource = sourceW;
            dataGridViewWaypoints.Refresh();

            //обновление карты
            RefreshOverlay(tripRoute);
        }

        /// <summary>
        /// заполнение слоя карты создаваемого путешествия
        /// </summary>
        /// <param name="tripRoute"></param>
        private void RefreshOverlay(TripRouteFile tripRoute)
        {
            Program.winMap.creatingTripOverlay.Clear();
            foreach (TrackFile tfl in tripRoute.DaysRoutes)
                Program.winMap.ShowRoute(tfl, Program.winMap.creatingTripOverlay, true);
            Program.winMap.ShowWaypoints(tripRoute.Waypoints, Program.winMap.creatingTripOverlay, false, false);
        }

        /// <summary>
        /// отмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEditTrip_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cancel)
            {
                DialogResult = DialogResult.Cancel;
                if (cancelAction != null)
                    cancelAction.Invoke();
            }
            Program.winMap.creatingTripOverlay.Clear();
        }


        #region контекстное меню списка маршуртов

        /// <summary>
        /// перед открытием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripDays_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridViewDays.SelectedRows.Count == 1)
            {
                int ind = dataGridViewDays.SelectedRows[0].Index;
                upDayToolStripMenuItem.Visible = ind != 1;
                downDayToolStripMenuItem.Visible = ind != dataGridViewDays.Rows.Count - 1;
                informationDayToolStripMenuItem.Visible = true;
                editDayToolStripMenuItem.Visible = true;
            }
            else
            {
                upDayToolStripMenuItem.Visible = false;
                downDayToolStripMenuItem.Visible = false;
                informationDayToolStripMenuItem.Visible = false;
                editDayToolStripMenuItem.Visible = false;
            }

        }

        /// <summary>
        /// подробная информация о маршруте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void informationDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = dataGridViewDays.SelectedCells[0].RowIndex;
            FormTrackInformation fti = new FormTrackInformation(selectedTrack);
            if (fti.ShowDialog() == DialogResult.OK)
            {
                selectedTrack = fti.Result;
                if (row == 0)
                    trip = selectedTrack as TripRouteFile;
                else
                    trip.setDayRoute(row - 1, selectedTrack as TrackFile);
                FillDGV(trip);
            }
        }

        /// <summary>
        /// редактировать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTrack.GetType() == typeof(TripRouteFile))
                return;

            Action<TrackFile> after = new Action<TrackFile>((tr) =>
            {
                int ind = trip.DaysRoutes.IndexOf(selectedTrack);
                trip.setDayRoute(ind, tr.Clone() as TrackFile);
                this.FillDGV(trip);
            });
            Program.winMap.BeginEditRoute(selectedTrack as TrackFile, after);
        }

        /// <summary>
        /// вставить новый
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertDayToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridViewDays.SelectedRows)
                if (r.Index == 0)
                    continue;
                else
                    trip.RemoveDay(r.Index - 1);

            FillDGV(trip);
        }

        /// <summary>
        /// вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downDayToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upDayToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        #endregion

        #region контекстное меню списка путевых точек

        /// <summary>
        /// перед открытием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripWaypoints_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridViewWaypoints.SelectedRows.Count == 1)
            {
                int ind = dataGridViewWaypoints.SelectedRows[0].Index;
                upPointToolStripMenuItem.Visible = ind != 0;
                downPointToolStripMenuItem.Visible = ind != dataGridViewWaypoints.Rows.Count - 1;
                informationPointToolStripMenuItem.Visible = true;
            }
            else
            {
                upPointToolStripMenuItem.Visible = false;
                downPointToolStripMenuItem.Visible = false;
                informationPointToolStripMenuItem.Visible = false;
            }

        }

        /// <summary>
        /// информация
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void informationPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должна быть выделена только одна точка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FormEditPoint fep = new FormEditPoint(this.selectedPoint);
            int row = dataGridViewWaypoints.SelectedCells[0].RowIndex;
            if (fep.ShowDialog() == DialogResult.OK)
            {
                this.selectedPoint = fep.Result;
                trip.Waypoints[row] = selectedPoint;
                FillDGV(trip);
            }
        }

        /// <summary>
        /// вставить новую
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertPointToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePointToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridViewWaypoints.SelectedRows)
                trip.Waypoints.Remove(r.Index);
            FillDGV(trip);
        }

        /// <summary>
        /// вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upPointToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downPointToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }


        #endregion

        #region обработчики кнопок

        /// <summary>
        /// добавление маршурта 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDay_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            TrackFile newt = new TrackFile();
            Action<TrackFile> afterAct = new Action<TrackFile>((tfl) =>
            {
                this.WindowState = FormWindowState.Normal;
                tfl.CalculateAll();
                tfl.Name = "День " + (trip.DaysRoutes.Count + 1).ToString();
                tfl.Color = Vars.Options.Converter.GetColor();
                this.selectedTrack = tfl;
                this.trip.AddDay(tfl);
                FillDGV(trip);
            });
            Action canc = new Action(() =>
            {
                this.WindowState = FormWindowState.Normal;
            });
            Program.winMap.BeginEditRoute(newt, afterAct, canc);
        }

        /// <summary>
        /// добавить маршрут из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDayFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.gpx, *.osm, *.nmea, *.csv, *.txt)| *.crd; *.wpt; *.plt; *rt2; *.gpx; *.osm; *.nmea; *.csv; *.txt";
                of.Filter += "|Треки Androzic (*.plt)|*.plt";
                of.Filter += "|Маршрут Androzic (*.rt2)|*.rt2";
                of.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
                of.Filter += "|Файл координат(*.crd)|*.crd";
                of.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
                of.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
                of.Filter += "|Файл NMEA(*.nmea)|*.nmea";
                of.Filter += "|Файл Excel(*.csv)|*.csv";
                of.Filter += "|Текстовый файл(*.txt)|*.txt";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = false;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == DialogResult.OK)
                {
                    TrackFile tf = Serializer.DeserializeTrackFile(of.FileName) as TrackFile;
                    tf.CalculateAll();
                    this.selectedTrack = tf;
                    this.trip.AddDay(tf);
                    FillDGV(trip);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        /// <summary>
        /// добавление путевой точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddWaypoint_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Action<TrackPoint> after = new Action<TrackPoint>((point) =>
            {
                this.WindowState = FormWindowState.Normal;
                this.trip.Waypoints.Add(point);
                FillDGV(trip);
            });

            Action canc = new Action(() =>
            {
                Program.winMap.isSelectingPoint = false;
                this.WindowState = FormWindowState.Normal;
                Program.winMap.gmapControlMap.DragButton = MouseButtons.Left;
                Program.winMap.gmapControlMap.Cursor = Cursors.Arrow;
            });
            Program.winMap.BeginSelectPoint(after, canc);
        }

        /// <summary>
        /// загрузить путевые точки из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWptFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.gpx, *.osm, *.nmea, *.csv, *.txt)| *.crd; *.wpt; *.plt; *rt2; *.gpx; *.osm; *.nmea; *.csv; *.txt";
                of.Filter += "|Треки Androzic (*.plt)|*.plt";
                of.Filter += "|Маршрут Androzic (*.rt2)|*.rt2";
                of.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
                of.Filter += "|Файл координат(*.crd)|*.crd";
                of.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
                of.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
                of.Filter += "|Файл NMEA(*.nmea)|*.nmea";
                of.Filter += "|Файл Excel(*.csv)|*.csv";
                of.Filter += "|Текстовый файл(*.txt)|*.txt";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = false;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == DialogResult.OK)
                {
                    TrackFile tf = Serializer.DeserializeTrackFile(of.FileName) as TrackFile;
                    this.trip.Waypoints.Add(tf);
                    FillDGV(trip);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        /// <summary>
        /// сохранение путешествия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            cancel = false;
            afterAction.Invoke(this.trip.Clone() as TripRouteFile);
            Close();
        }

        /// <summary>
        /// отмена и закрытие формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            Close();
        }


        #endregion

        #region события таблиц

        #region точки

        /// <summary>
        /// изменнение выделенной точки в таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewWaypoints_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.SelectedRows.Count == 1)
            {
                int ind = dataGridViewWaypoints.SelectedRows[0].Index;
                this.selectedPoint = trip.Waypoints[ind];
                Program.winMap.gmapControlMap.Position = this.selectedPoint.GMap;
            }
        }

        /// <summary>
        /// выделение всего ряда при клике на ячейку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewWaypoints_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            Keyboard ss = new Keyboard();
            if (!ss.CtrlKeyDown && e.Button != MouseButtons.Right)
                dataGridViewWaypoints.ClearSelection();
            if (e.Button == MouseButtons.Right)
                dataGridViewWaypoints.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// открытие информации по двойному клику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewWaypoints_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            informationPointToolStripMenuItem_Click(sender, null);
        }

        /// <summary>
        /// нажатие на ячейку и выделение строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewWaypoints_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            dataGridViewWaypoints.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// нажатие кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewWaypoints_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridViewWaypoints.RowCount == 0) return;
            if (e.KeyData != Keys.Delete) return;
            removePointToolStripMenuItem1_Click(null, null);
        }

        #endregion

        #region дни

        /// <summary>
        /// изменение выделенного трека в таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewDays_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewDays.SelectedRows.Count == 1)
            {

                int ind = dataGridViewDays.SelectedRows[0].Index;
                if (ind > 0) // если нажали не на первую строку
                    this.selectedTrack = trip.DaysRoutes[ind - 1];//перевой стрчтой в таблице стоит всё путешествие, поэтому -1
                if (ind == 0)
                    this.selectedTrack = trip;
                Vars.currentSelectedTrack = this.selectedTrack.Clone();
                Program.RefreshWindows(this);
            }
        }

        /// <summary>
        /// выделение всего ряда при клике на ячейку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewDays_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            Keyboard ss = new Keyboard();
            if (!ss.CtrlKeyDown && e.Button != MouseButtons.Right)
                dataGridViewDays.ClearSelection();
            if (e.Button == MouseButtons.Right)
                dataGridViewDays.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// открытие информации по двойному клику
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewDays_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            informationDayToolStripMenuItem_Click(sender, null);
        }

        /// <summary>
        /// нажатие на ячейку и выделение строки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewDays_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            dataGridViewDays.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// нажатие кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewDays_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridViewDays.RowCount == 0) return;
            if (e.KeyData != Keys.Delete) return;
            removeDayToolStripMenuItem_Click(null, null);
        }

        #endregion

        #endregion


    }
}
