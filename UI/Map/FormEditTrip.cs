using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Mathematic.Routing;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно редактирования путешествия
    /// </summary>
    public partial class FormEditTrip: Form
    {
        /// <summary>
        /// действие после редактирования
        /// </summary>
        private Action<TripRouteFile> afterAction;

        /// <summary>
        /// действие при отмене редактирования
        /// </summary>
        private Action cancelAction;

        /// <summary>
        /// путешествие для редактирвания
        /// </summary>
        private TripRouteFile trip;

        /// <summary>
        /// выбраный маршрут по дням
        /// </summary>
        private BaseTrack selectedTrack;

        /// <summary>
        /// выбранная точка в списке
        /// </summary>
        private TrackPoint selectedPoint;

        /// <summary>
        /// если истина, то нажата кнопка сохранить
        /// </summary>
        private bool saved = false;

        /// <summary>
        /// создаёт окно редактирования путешествия
        /// </summary>
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
            fillDGV(tripFile);
            dataGridViewDays.Rows[0].Selected = true;
        }

        /// <summary>
        /// заполнение таблицы заданным путешествием и обновление карты 
        /// </summary>
        /// <param name="tripRoute">путешествие</param>
        /// <param name="centring">если истина, то карта будет отцентрована по маршруту</param>
        private void fillDGV(TripRouteFile tripRoute, bool centring = false)
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

            foreach (DataGridViewColumn column in dataGridViewDays.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            foreach (DataGridViewColumn column in dataGridViewWaypoints.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            //путевые точки
            trip.Waypoints = trip.SortWaypoints(trip.Waypoints);
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
            refreshOverlay(tripRoute, centring);
        }

        /// <summary>
        /// заполнение слоя карты создаваемого путешествия
        /// </summary>
        /// <param name="tripRoute"></param>
        /// <param name="centring">если истина, то карта будет отцентрована по маршруту</param>
        private void refreshOverlay(TripRouteFile tripRoute, bool centring = false)
        {
            Program.winMain.creatingTripOverlay.Clear();
            foreach (TrackFile tfl in tripRoute.DaysRoutes)
                Program.winMain.mapHelper.ShowRoute(tfl, Program.winMain.creatingTripOverlay, centring);
            Program.winMain.mapHelper.ShowWaypoints(tripRoute.Waypoints, Program.winMain.creatingTripOverlay, false, false);
        }

        /// <summary>
        /// отмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formEditTrip_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (!saved && (
                e.CloseReason == CloseReason.UserClosing ||
                e.CloseReason == CloseReason.FormOwnerClosing ||
                e.CloseReason == CloseReason.WindowsShutDown))
            {
                DialogResult msg = MessageBox.Show(this, "Вы действительно хотите отменить изменения?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msg == DialogResult.Yes)
                {
                    DialogResult = DialogResult.Yes;
                    if (cancelAction != null)
                        cancelAction.Invoke();
                    Program.winMain.creatingTripOverlay.Clear();
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
            if (dataGridViewDays.SelectedRows.Count == 1)
            {
                int ind = dataGridViewDays.SelectedRows[0].Index;
                upDayToolStripMenuItem.Visible = ind != 1;
                downDayToolStripMenuItem.Visible = ind != dataGridViewDays.Rows.Count - 1;
                informationDayToolStripMenuItem.Visible = true;
                editDayToolStripMenuItem.Visible = true;
                separateTrackToolStripMenuItem.Visible = true;
                addIntermedPointsToolStripMenuItem.Visible = true;
                joinToolStripMenuItem.Visible = false;
                saveAsToolStripMenuItem.Visible = true;
            }
            else
            {
                upDayToolStripMenuItem.Visible = false;
                downDayToolStripMenuItem.Visible = false;
                informationDayToolStripMenuItem.Visible = false;
                editDayToolStripMenuItem.Visible = false;
                separateTrackToolStripMenuItem.Visible = false;
                addIntermedPointsToolStripMenuItem.Visible = true;
                joinToolStripMenuItem.Visible = true;
                saveAsToolStripMenuItem.Visible = false;
            }
            invertToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
            separateTrackToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
            addIntermedPointsToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
            joinToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
            saveAsToolStripMenuItem.Visible = !dataGridViewDays.Rows[0].Selected;
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
                fillDGV(trip);
            }
        }

        /// <summary>
        /// редактировать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTrack is TripRouteFile)
                return;
            this.WindowState = FormWindowState.Minimized;
            Vars.currentSelectedTrack = null;
            Program.winMain.mapHelper.RefreshData();
            Program.winMain.converterHelper.RefreshData();
            Program.winMain.graphHelper.RefreshData();
            Program.winMain.pointsHelper.RefreshData();
            Action<TrackFile> after = new Action<TrackFile>((tr) =>
            {
                this.WindowState = FormWindowState.Normal;
                int ind = trip.DaysRoutes.IndexOf(selectedTrack);
                trip.setDayRoute(ind, tr.Clone() as TrackFile);
                this.fillDGV(trip);
            });
            Action canc = new Action(() =>
            {
                this.WindowState = FormWindowState.Normal;
            });
            Program.winMain.mapHelper.BeginEditRoute(selectedTrack as TrackFile, after, canc);
        }


        /// <summary>
        /// сохранить как 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "Файл маршрута Androzic (*.rt2)|*.rt2"
            };
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
            sf.FileName = Path.GetFileNameWithoutExtension(this.selectedTrack.FileName);

            if (sf.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".rt2":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.Rt2File);
                        break;
                    case ".plt":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.PltFile);
                        break;
                    case ".wpt":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.WptFile);
                        break;
                    case ".crd":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.CrdFile);
                        break;
                    case ".kml":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.KmlFile);
                        break;
                    case ".gpx":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.GpxFile);
                        break;
                    case ".kmz":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.KmzFile);
                        break;
                    case ".osm":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.OsmFile);
                        break;
                    case ".nmea":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.NmeaFile);
                        break;
                    case ".csv":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.CsvFile);
                        break;
                    case ".txt":
                        Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.TxtFile);
                        break;
                    case ".adrs":
                        Program.winMain.BeginOperation();
                        Action act = new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, this.selectedTrack, FileFormats.AddressList, Program.winMain.SetCurrentOperation);
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
            fillDGV(trip);
        }

        /// <summary>
        /// разделить маршрут на две части
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void separateTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ind = dataGridViewDays.SelectedRows[0].Index - 1; //первый маршрут в списке - всё путешествие
            TrackFile route = (TrackFile)trip.DaysRoutes[ind]; //разделяемый маршрут
            TrackFile first = null, second = null; //будущие отрезки маршрута

            #region добавление отрезков в список
            Action addList = new Action(() =>
            {
                //первая часть маршрута
                first.CalculateAll();
                first.Color = route.Color;
                first.Name = route.Name + " 1";
                first.Description = route.Description;

                //вторая часть
                second.CalculateAll();
                second.Color = Vars.Options.Converter.GetColor();
                second.Name = route.Name + " 2";
                second.Description = route.Description;

                //добавление в список маршрутов
                trip.DaysRoutes.Insert(ind, second);
                trip.DaysRoutes.Insert(ind, first);
                trip.DaysRoutes.Remove(route);
                fillDGV(trip);
            });

            #endregion

            FormSeparateTrackTypeDialog fstt = new FormSeparateTrackTypeDialog("Выберите один из способов разделения маршрута", "Разделение маршрута");
            if (fstt.ShowDialog(this) == DialogResult.OK)
            {
                SeparateRouteType sep_type = fstt.Result;
                bool addCommon = fstt.ResultAddCommon;
                switch (sep_type)
                {
                    case SeparateRouteType.Length:
#region разделение по длине
readLength:
                        FormReadText frt = new FormReadText(DialogType.ReadNumber, "Введите длину первого отрезка в км. Максимальная длина " + route.Distance.ToString("0.0") + " км.", "", false, false, false, false);
                        if (frt.ShowDialog(this) == DialogResult.OK)
                        {
                            double length = double.Parse(frt.Result
                                .Trim()
                                .Replace('.', Vars.DecimalSeparator)
                                .Replace(',', Vars.DecimalSeparator));

                            if (length >= route.Distance)
                            {
                                MessageBox.Show(this, "Длина первого отрезка должна быть меньше длины маршрута", "Разделение маршрута", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                goto readLength;
                            }

                            first = route.Subtrack(0, length * 1000); //передаём длину в метрах
                            second = route.Subtrack(length * 1000, double.MaxValue);
                            addList.Invoke();
                        }

                        #endregion
                        break;
                    case SeparateRouteType.Nearest:
                        #region разделение по ближайшей точке
                        Action<TrackPoint> after = new Action<TrackPoint>((pt) =>
                     {
                         TrackPoint nearest = route.GetNearestPoint(pt);
                         int indn = route.IndexOf(nearest);
                         if (indn == 0) //если ближайшая точка - первая в маршруте
                         {
                             first = new TrackFile() { nearest };
                             second = route.Subtrack(addCommon ? 0 : 1, route.Count - 1);
                         }
                         else
                         if (indn == route.Count - 1) //если ближайшая  - последняя в маршруте
                         {
                             second = new TrackFile() { nearest };
                             first = route.Subtrack(0, route.Count - (addCommon ? 1 : 2));
                         }
                         else //если ближайшая точка внутри маршрута
                         {
                             first = route.Subtrack(0, indn);
                             second = route.Subtrack(indn + (addCommon ? 0 : 1), route.Count - 1);
                         }

                         //сброс курсоров и флажка выбора
                         Program.winMain.isSelectingPoint = false;
                         Program.winMain.gmapControlMap.Cursor = Cursors.Arrow;

                         //добавление в список
                         addList.Invoke();
                     });
                        Program.winMain.mapHelper.BeginSelectPoint(after);
                        #endregion
                        break;
                    case SeparateRouteType.None:
                        return;
                }
            }
            else
                return;
        }

        /// <summary>
        /// объединить маршруты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackFileList tfl = new TrackFileList();
            int startIndex = int.MaxValue;
            foreach (DataGridViewRow dr in dataGridViewDays.SelectedRows)
                if (dr.Index < startIndex)
                    startIndex = dr.Index;

            startIndex--; //убираем первый пункт (само путешествие)
            foreach (DataGridViewRow dr in dataGridViewDays.SelectedRows)
            {
                BaseTrack bt = trip.DaysRoutes[dataGridViewDays.SelectedRows.Count - dr.Index];
                tfl.Add(bt);
            }
            foreach (TrackFile tf in tfl)
                trip.DaysRoutes.Remove(tf);

            TrackFile res = tfl.JoinTracks();
            trip.DaysRoutes.Insert(startIndex - 1, res);
            fillDGV(trip);
        }

        /// <summary>
        /// добавление промежуточных точек в маршрут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addIntermedPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReadText frt = new FormReadText(DialogType.ReadNumber, "Введите максимальное расстояние в метрах между точками", "", false, false, false, false);


            if (frt.ShowDialog(this) == DialogResult.OK)
            {
                double length = double.Parse(frt.Result
                    .Trim()
                    .Replace('.', Vars.DecimalSeparator)
                    .Replace(',', Vars.DecimalSeparator));

                foreach (DataGridViewRow dr in dataGridViewDays.SelectedRows)
                {
                    int ind = dr.Index - 1; //первый маршрут в списке - всё путешествие
                    TrackFile tf = (TrackFile)trip.DaysRoutes[ind];

                    TrackFile ntf = (TrackFile)tf.AddIntermediatePoints(length);
                    trip.DaysRoutes.Insert(ind, ntf);
                    trip.DaysRoutes.Remove(tf);

                }
                fillDGV(trip);
            }

        }

        /// <summary>
        /// удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewDays.SelectedRows[0].Index == 0)
                return;
            int first = int.MaxValue;
            int i = 0;
            foreach (DataGridViewRow r in dataGridViewDays.SelectedRows)
                if (r.Index == 0)
                    continue;
                else
                {
                    if (r.Index < first)
                        first = r.Index;
                    trip.RemoveDay(r.Index - 1 - i);
                    i++;
                }
            fillDGV(trip);

            dataGridViewDays.ClearSelection();
            if (dataGridViewDays.Rows.Count != 0)
                dataGridViewDays.Rows[first - 1].Selected = true;

            Vars.currentSelectedTrack = null;
            Program.winMain.mapHelper.RefreshData();
            Program.winMain.converterHelper.RefreshData();
            Program.winMain.graphHelper.RefreshData();
            Program.winMain.pointsHelper.RefreshData();//обновление окон для того, чтобы убрать выделенный трек с карты
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
            fillDGV(trip);
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
            fillDGV(trip);
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
                informationPointToolStripMenuItem.Visible = true;
            else
                informationPointToolStripMenuItem.Visible = false;

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
                fillDGV(trip);
            }
        }

        /// <summary>
        /// удалить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePointToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.SelectedRows.Count == 0)
                return;
            int first = int.MaxValue;
            int i = 0;
            foreach (DataGridViewRow r in dataGridViewWaypoints.SelectedRows)
            {
                if (r.Index < first)
                    first = r.Index;
                trip.Waypoints.Remove(r.Index - i);
                i++;
            }
            fillDGV(trip);
            dataGridViewWaypoints.ClearSelection();
            if (dataGridViewWaypoints.Rows.Count != 0)
                dataGridViewWaypoints.Rows[first - 1].Selected = true;
        }


        #endregion

        #region контекстное меню кнопки добавления маршрута
        /// <summary>
        /// Нарисовать маршрут на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void paintNewRouteToolStripMenuItem_Click(object sender, EventArgs e)
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
                fillDGV(trip);
            });
            Action canc = new Action(() =>
            {
                this.WindowState = FormWindowState.Normal;
            });
            Program.winMain.mapHelper.BeginEditRoute(newt, afterAct, canc);
        }

        /// <summary>
        /// Построить оптимальный через точки путешествия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optimalNewRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trip.Waypoints.Count < 3)
            {
                MessageBox.Show(this, "Для построения оптимального маршрута надо поставить хотя бы 3 точки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TrackPoint startPoint = null;
            TrackPoint endPoint = null;


            FormSelectPoint fsp = new FormSelectPoint(trip.Waypoints, true, "Выберите начальную точку");
            if (fsp.ShowDialog(Program.winMain) == DialogResult.OK)
                startPoint = fsp.Result;
            else
                return;

            bool isCycled = startPoint == null;

            if (Vars.Options.Map.OptimalRouteMethod == OptimalMethodType.PolarSearch && !isCycled)
            {
                FormSelectPoint fsp1 = new FormSelectPoint(trip.Waypoints, startPoint, false, "Выберите конечную точку");
                if (fsp1.ShowDialog(Program.winMain) == DialogResult.OK)
                    endPoint = fsp1.Result;
                else
                    return;
            }

            Program.winMain.BeginOperation();

            //построение маршрута асинхронно
            Task ts = new Task(new Action(() =>
            {
                try
                {
                    TrackFile route = null;
                    DateTime start = DateTime.Now;
                    switch (Vars.Options.Map.OptimalRouteMethod)
                    {
                        case OptimalMethodType.BranchBounds:
                            route = new BranchBounds(Program.winMain.SetCurrentOperation).Make(trip.Waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.Greedy:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(trip.Waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.FullSearch:
                            route = new FullSearch(Program.winMain.SetCurrentOperation).Make(trip.Waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.PolarSearch:
                            route = new PolarSearch(Program.winMain.SetCurrentOperation).Make(trip.Waypoints, startPoint, endPoint, isCycled);
                            break;
                        default:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(trip.Waypoints, startPoint, isCycled);
                            break;
                    }

                    //вывод маршрута на карту в базовом потоке
                    this.Invoke(new Action(() =>
                    {
                        route.CalculateAll();
                        route.Name = "День " + (trip.DaysRoutes.Count + 1).ToString();
                        route.Color = Vars.Options.Converter.GetColor();
                        TimeSpan span = DateTime.Now - start;
                        MessageBox.Show("Маршрут построен за:\r\n" + span.ToString(@"mm\:ss\.fff"));

                        //если не надо открывать маршрут
                        trip.DaysRoutes.Add(route);
                        fillDGV(trip, true);
                    }));

                }
                catch (Exception exx)
                {
                    this.Invoke(new Action(() =>
                         MessageBox.Show(this, exx.Message + "\r\n" + (exx.InnerException != null ? exx.InnerException.Message : ""), "Ошибка при создании маршрута", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ));
                    return;
                }
                finally
                {
                    Program.winMain.EndOperation();
                }
            }));
            ts.Start();

        }

        #endregion

        #region обработчики кнопок

        /// <summary>
        /// Открытие контекстного меню выбора способа добавления маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddDay_Click(object sender, EventArgs e)
        {
            contextMenuStripNewRoute.Show(Cursor.Position);
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
                OpenFileDialog of = new OpenFileDialog
                {
                    Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.gpx, *.osm, *.nmea, *.csv, *.txt)| *.crd; *.wpt; *.plt; *rt2; *.gpx; *.osm; *.nmea; *.csv; *.txt"
                };
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
                    fillDGV(trip);
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
                FormEditPoint fep = new FormEditPoint(point);
                DialogResult res = fep.ShowDialog();
                if (res == DialogResult.OK)
                {
                    TrackPoint newPoint = fep.Result;
                    this.trip.Waypoints.Add(newPoint);
                    fillDGV(trip);
                }
                Program.winMain.gmapControlMap.DragButton = MouseButtons.Left;
                this.WindowState = FormWindowState.Normal;
                Program.winMain.gmapControlMap.Cursor = Cursors.Arrow;
                Program.winMain.isSelectingPoint = false;
            });
            Program.winMain.mapHelper.BeginSelectPoint(after);
        }

        /// <summary>
        /// экспорт дневных маршртов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportDays_Click(object sender, EventArgs e)
        {
            FormReadText rt = new FormReadText(DialogType.ReadExtension, "Bведите формат файлов (wpt,crd,plt,rt2,gpx,kml,kmz,osm,nmea,txt,csv,rte)", Vars.Options.Common.IsExtension ? Vars.Options.Common.LastSaveSeparateExtension : "", false, false, false, false);
            if (rt.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog fb = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true
                };

                if (Vars.Options.Common.IsSaveDir)
                    fb.SelectedPath = Vars.Options.Common.LastFileSaveDirectory;
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    string basename = fb.SelectedPath + "\\";
                    if (rt.Result == "rte")
                    {
                        basename += trip.Name;
                        Serializer.Serialize(basename + ".rte", trip.DaysRoutes, FileFormats.RteFile);
                    }
                    else
                    {

                        Parallel.ForEach(trip.DaysRoutes, new Action<BaseTrack>((BaseTrack tf) =>
                        {
                            string nm = "";
                            if (string.IsNullOrEmpty(tf.Name))
                                if (string.IsNullOrEmpty(tf.FileName))
                                    nm = Guid.NewGuid().ToString();
                                else
                                    nm = tf.FileName;
                            else
                                nm = tf.Name;

                            switch (rt.Result)
                            {
                                case "wpt":
                                    Serializer.Serialize(basename + nm + ".wpt", tf, FileFormats.WptFile);
                                    break;
                                case "crd":
                                    Serializer.Serialize(basename + nm + ".crd", tf, FileFormats.CrdFile);
                                    break;
                                case "plt":
                                    Serializer.Serialize(basename + nm + ".plt", tf, FileFormats.PltFile);
                                    break;
                                case "rt2":
                                    Serializer.Serialize(basename + nm + ".rt2", tf, FileFormats.Rt2File);
                                    break;
                                case "gpx":
                                    Serializer.Serialize(basename + nm + ".gpx", tf, FileFormats.GpxFile);
                                    break;
                                case "kml":
                                    Serializer.Serialize(basename + nm + ".kml", tf, FileFormats.KmlFile);
                                    break;
                                case "kmz":
                                    Serializer.Serialize(basename + nm + ".kmz", tf, FileFormats.KmzFile);
                                    break;
                                case "osm":
                                    Serializer.Serialize(basename + nm + ".osm", tf, FileFormats.OsmFile);
                                    break;
                                case "nmea":
                                    Serializer.Serialize(basename + nm + ".nmea", tf, FileFormats.NmeaFile);
                                    break;
                                case "csv":
                                    Serializer.Serialize(basename + nm + ".csv", tf, FileFormats.CsvFile);
                                    break;
                                case "txt":
                                    Serializer.Serialize(basename + nm + ".txt", tf, FileFormats.TxtFile);
                                    break;
                                default:
                                    MessageBox.Show(this, "Данный формат не поддерживается: " + rt.Result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                            }
                        }));
                        Vars.Options.Common.LastSaveSeparateExtension = rt.Result;
                        MessageBox.Show("Файлы сoхранены");
                    }
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
            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "Файл маршрута Androzic (*.rt2)|*.rt2"
            };
            sf.Filter += "|Список адресов(*.adrs)|*.adrs";
            sf.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
            sf.Filter += "|Путевые точки Ozi с адресами(*.wpt)|*.wpt"; //не менять позицию этого пункта! (ниже от него зависит добавление адресов)
            sf.Filter += "|Документ Word (*.doc)|*.doc";
            sf.Filter += "|Треки Androzic (*.plt)|*.plt";
            sf.Filter += "|Файл координат(*.crd)|*.crd";
            sf.Filter += "|Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";
            sf.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
            sf.Filter += "|Файл NMEA(*.nmea)|*.nmea";
            sf.Filter += "|Файл Excel(*.csv)|*.csv";
            sf.Filter += "|Файл Яндекс (*.csv)|*.csv"; //не менять позицию этого пункта! (ниже от него зависит яндекс csv)
            sf.Filter += "|Текстовый файл(*.txt)|*.txt";

            sf.AddExtension = true;

            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack;
            sf.FileName = "Путевые точки " + trip.Name;

            if (sf.ShowDialog() == DialogResult.OK)
            {
                BaseTrack bt;
                //если надо сохранить адреса в файл wpt
                if (sf.FilterIndex == 4)
                {
                    bt = trip.Waypoints.Clone();
                    new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddresses(bt, Program.winMain.SetCurrentOperation);
                }
                else
                    bt = trip.Waypoints;

                //если надо сохранить в формате яндекс csv
                bool isYandexCsv = false;
                if (sf.FilterIndex == 14)
                    isYandexCsv = true;

                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".rt2":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.Rt2File);
                        break;
                    case ".plt":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.PltFile);
                        break;
                    case ".wpt":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.WptFile);
                        break;
                    case ".crd":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.CrdFile);
                        break;
                    case ".kml":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.KmlFile);
                        break;
                    case ".gpx":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.GpxFile);
                        break;
                    case ".kmz":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.KmzFile);
                        break;
                    case ".osm":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.OsmFile);
                        break;
                    case ".nmea":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.NmeaFile);
                        break;
                    case ".csv":
                        Serializer.Serialize(sf.FileName, bt,isYandexCsv?FileFormats.CsvYandexFile: FileFormats.CsvFile);
                        break;
                    case ".txt":
                        Serializer.Serialize(sf.FileName, bt, FileFormats.TxtFile);
                        break;
                    case ".doc":
                        try
                        {
                            Program.winMain.BeginOperation();
                            Action actn = new Action(() =>
                            {
                                Serializer.Serialize(sf.FileName, bt, FileFormats.DocFile, Program.winMain.SetCurrentOperation);
                                Program.winMain.EndOperation();
                            });
                            new Task(actn).Start();
                        }
                        catch (Exception exxx)
                        { MessageBox.Show(this, exxx.Message, "Сохранение файла", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        break;
                    case ".adrs":
                        Program.winMain.BeginOperation();
                        Action act = new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, bt, FileFormats.AddressList, Program.winMain.SetCurrentOperation);
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
                OpenFileDialog of = new OpenFileDialog
                {
                    Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.gpx, *.osm, *.nmea, *.csv, *.txt)| *.crd; *.wpt; *.plt; *rt2; *.gpx; *.osm; *.nmea; *.csv; *.txt"
                };
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
                    fillDGV(trip);
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
            saved = true;
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
            saved = false;
            Close();
        }

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
                    trip = (TripRouteFile)new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(trip, Program.winMain.SetCurrentOperation);
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
                Program.winMain.gmapControlMap.Position = this.selectedPoint.Coordinates.GMap;
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
            if (dataGridViewWaypoints.RowCount == 0)
                return;
            if (e.KeyData != Keys.Delete)
                return;
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
                Program.winMain.mapHelper.RefreshData();
                Program.winMain.converterHelper.RefreshData();
                Program.winMain.graphHelper.RefreshData();
                Program.winMain.pointsHelper.RefreshData();
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
            if (dataGridViewDays.RowCount == 0)
                return;
            if (e.KeyData != Keys.Delete)
                return;
            removeDayToolStripMenuItem_Click(null, null);
        }





        #endregion

        #endregion

    }
}
