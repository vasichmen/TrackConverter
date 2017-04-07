using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Data;
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
            FillDGV(tripFile);
            dataGridViewDays.Rows[0].Selected = true;
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
                DialogResult msg = MessageBox.Show(this, "Вы действительно хотите отменить изменения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Yes;
                    if (cancelAction != null)
                        cancelAction.Invoke();
                    Program.winMap.creatingTripOverlay.Clear();
                }
                if (msg == DialogResult.No)
                    e.Cancel = true;
            }
        }

        #region контекстное меню списка маршрутов

        /// <summary>
        /// перед открытием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripDays_Opening(object sender, CancelEventArgs e)
        {
            invertToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
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
            this.WindowState = FormWindowState.Minimized;
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
            Action<TrackFile> after = new Action<TrackFile>((tr) =>
            {
                this.WindowState = FormWindowState.Normal;
                int ind = trip.DaysRoutes.IndexOf(selectedTrack);
                trip.setDayRoute(ind, tr.Clone() as TrackFile);
                this.FillDGV(trip);
            });
            Action canc = new Action(() =>
            {
                this.WindowState = FormWindowState.Normal;
            });
            Program.winMap.BeginEditRoute(selectedTrack as TrackFile, after, canc);
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
        /// инвертирование маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridViewDays.SelectedRows)
                if (r.Index == 0)
                    continue;
                else
                {
                    TrackFile buf = trip.DaysRoutes[r.Index - 1] as TrackFile;
                    int ind = trip.DaysRoutes.IndexOf(buf);
                    buf = buf.Invert();
                    trip.setDayRoute(ind, buf);
                }
            FillDGV(trip);
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
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this); //обновление окон для того, чтобы убрать выделенный трек с карты
        }

        /// <summary>
        /// вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oldPos = trip.DaysRoutes.IndexOf(selectedTrack);
            int newPos = oldPos + 1;
            trip.MoveDay(oldPos, newPos);
            FillDGV(trip);
        }

        /// <summary>
        /// вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oldPos = trip.DaysRoutes.IndexOf(selectedTrack);
            int newPos = oldPos - 1;
            trip.MoveDay(oldPos, newPos);
            FillDGV(trip);
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
        /// добавление маршрута 
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
                Program.winMap.isSelectingPoint = false;
                Program.winMap.gmapControlMap.DragButton = MouseButtons.Left;
                Program.winMap.gmapControlMap.Cursor = Cursors.Arrow;
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
        /// экспорт дневных маршртов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportDays_Click(object sender, EventArgs e)
        {
            FormReadText rt = new FormReadText(DialogType.ReadExtension, "Bведите формат файлов (wpt,crd,plt,rt2,gpx,kml,kmz,osm,nmea,txt,csv)", Vars.Options.Common.IsExtension ? Vars.Options.Common.LastSaveSeparateExtension : "", false, false, false, false);
            if (rt.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.ShowNewFolderButton = true;

                if (Vars.Options.Common.IsSaveDir)
                    fb.SelectedPath = Vars.Options.Common.LastFileSaveDirectory;
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    ParallelLoopResult pr = Parallel.ForEach(trip.DaysRoutes, new Action<BaseTrack>((BaseTrack tf) =>
                    {
                        string nm = "";
                        if (string.IsNullOrEmpty(tf.Name))
                            if (string.IsNullOrEmpty(tf.FileName))
                                nm = Guid.NewGuid().ToString();
                            else
                                nm = tf.FileName;
                        else
                            nm = tf.Name;

                        string basename = fb.SelectedPath + "\\" + nm;
                        switch (rt.Result)
                        {
                            case "wpt":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.WptFile);
                                break;
                            case "crd":
                                Serializer.Serialize(basename + ".crd", tf, FileFormats.CrdFile);
                                break;
                            case "plt":
                                Serializer.Serialize(basename + ".plt", tf, FileFormats.PltFile);
                                break;
                            case "rt2":
                                Serializer.Serialize(basename + ".rt2", tf, FileFormats.Rt2File);
                                break;
                            case "gpx":
                                Serializer.Serialize(basename + ".gpx", tf, FileFormats.GpxFile);
                                break;
                            case "kml":
                                Serializer.Serialize(basename + ".kml", tf, FileFormats.KmlFile);
                                break;
                            case "kmz":
                                Serializer.Serialize(basename + ".kmz", tf, FileFormats.KmzFile);
                                break;
                            case "osm":
                                Serializer.Serialize(basename + ".osm", tf, FileFormats.OsmFile);
                                break;
                            case "nmea":
                                Serializer.Serialize(basename + ".nmea", tf, FileFormats.NmeaFile);
                                break;
                            case "csv":
                                Serializer.Serialize(basename + ".csv", tf, FileFormats.CsvFile);
                                break;
                            case "txt":
                                Serializer.Serialize(basename + ".txt", tf, FileFormats.TxtFile);
                                break;
                            default:
                                MessageBox.Show(this, "Данный формат не поддерживается: " + rt.Result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                        }
                        Vars.Options.Common.LastSaveSeparateExtension = rt.Result;
                    }));
                    MessageBox.Show("Файлы сoхранены");
                }
            }
        }

        /// <summary>
        /// экспорт всех точек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportWaypoints_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Файл маршрута Androzic (*.rt2)|*.rt2";
            sf.Filter += "|Треки Androzic (*.plt)|*.plt";
            sf.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
            sf.Filter += "|Файл координат(*.crd)|*.crd";
            sf.Filter += "|Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";
            sf.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
            sf.Filter += "|Файл NMEA(*.nmea)|*.nmea";
            sf.Filter += "|Файл Excel(*.csv)|*.csv";
            sf.Filter += "|Текстовый файл(*.txt)|*.txt";
            sf.Filter += "|Список адресов(*.adrs)|*.adrs";

            sf.AddExtension = true;

            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack;
            sf.FileName = Path.GetFileNameWithoutExtension(trip.Waypoints.FileName);

            if (sf.ShowDialog() == DialogResult.OK)
            {
                switch (sf.FilterIndex)
                {
                    case 1:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.Rt2File);
                        break;
                    case 2:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.PltFile);
                        break;
                    case 3:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.WptFile);
                        break;
                    case 4:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.CrdFile);
                        break;
                    case 5:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.KmlFile);
                        break;
                    case 6:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.GpxFile);
                        break;
                    case 7:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.KmzFile);
                        break;
                    case 8:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.OsmFile);
                        break;
                    case 9:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.NmeaFile);
                        break;
                    case 10:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.CsvFile);
                        break;
                    case 11:
                        Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.TxtFile);
                        break;
                    case 12:
                        Program.winMain.BeginOperation();
                        Action act = new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, trip.Waypoints, FileFormats.AddressList, Program.winMain.setCurrentOperation);
                            Program.winMain.EndOperation();
                        });
                        new Task(act).Start();
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack = sf.FilterIndex;
            }
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

        /// <summary>
        /// загрузить высоты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadElevations_Click(object sender, EventArgs e)
        {
            Program.winMain.BeginOperation();
            Task ts = new Task(new Action(() =>
            {
                try
                {
                    trip = (TripRouteFile)new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(trip, Program.winMain.setCurrentOperation);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                        MessageBox.Show(this, "Не удалось получить информацию из-за проблем с соединением.\r\n" + ex.Message + "\r\nПроверьте соединение с Интернет", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ));
                }
                this.Invoke(new Action(() =>
                {
                    dataGridViewDays.ClearSelection();
                    dataGridViewDays.Rows[0].Selected = true;
                    Program.winMain.EndOperation();
                }));
            }));
            ts.Start();
        }

    }
}
