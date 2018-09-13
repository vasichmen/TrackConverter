using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Mathematic.Assessment;
using TrackConverter.Lib.Mathematic.Routing;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Map;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI.Shell
{
    internal class ConverterHelper
    {
        private FormMain formMain;

        public ConverterHelper(FormMain formMain)
        {
            this.formMain = formMain;
        }


        /// <summary>
        /// добавление маршрута в список маршрутов
        /// </summary>
        /// <param name="trackFile"></param>
        public void AddRouteToList(BaseTrack trackFile)
        {
            if (trackFile == null)
                return;
            if (trackFile.Color.IsEmpty)
                trackFile.Color = Vars.Options.Converter.GetColor();
            formMain.Tracks.Add(trackFile);
            RefreshData();
        }

        /// <summary>
        /// добавление нескольких маршрутов в список
        /// </summary>
        /// <param name="routes"></param>
        internal void AddRouteToList(TrackFileList routes)
        {
            foreach (BaseTrack tf in routes)
                AddRouteToList(tf);
        }
        /// <summary>
        /// обновление информации
        /// </summary>
        internal void RefreshData()
        {
            formMain.dataGridViewConverter.DataSource = null;
            formMain.dataGridViewConverter.DataSource = formMain.Tracks.Source;
            formMain.dataGridViewConverter.Refresh();

            foreach (DataGridViewColumn column in formMain.dataGridViewConverter.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            refreshRecentFiles(); //обновление списка последних файлов
            formMain.dataGridViewConverter.ClearSelection();

            if (Vars.currentSelectedTrack != null)
            {
                int ind = formMain.Tracks.IndexOf(Vars.currentSelectedTrack);
                if (ind == -1)
                    return;
                formMain.dataGridViewConverter.Rows[ind].Selected = true;
            }
        }

        /// <summary>
        /// завершение редактирования маршрута
        /// </summary>
        /// <param name="route"></param>
        public void EndEditRouteOrTrip(BaseTrack route)
        {
            if (route == null)
                return;
            Vars.currentSelectedTrack = route;
            AddRouteToList(route);
        }

        /// <summary>
        /// обновление списка последних загруженных файлов
        /// </summary>
        private void refreshRecentFiles()
        {
            for (int i = 0; i < formMain.FileToolStripMenuItem.DropDownItems.Count; i += 0)
            {
                if (formMain.FileToolStripMenuItem.DropDownItems[i].Name.Contains("tsmiRecentButt"))
                {
                    formMain.FileToolStripMenuItem.DropDownItems.RemoveAt(i);
                    continue;
                }
                i++;
            }

            if (Vars.Options.Converter.RecentFiles != null)
            {
                int i = 0;
                foreach (string str in Vars.Options.Converter.RecentFiles)
                {
                    ToolStripMenuItem nm = new ToolStripMenuItem(str);
                    nm.Click += nm_Click;
                    nm.Tag = str;
                    nm.ToolTipText = str;
                    nm.Text = Path.GetFileName(str);
                    nm.Name = "tsmiRecentButt" + i;
                    formMain.FileToolStripMenuItem.DropDownItems.Add(nm);
                    i++;
                }
            }
        }

        /// <summary>
        /// подготовка списка пунктов контекстного меню маршрута перед открытием
        /// </summary>
        /// <param name="items"></param>
        private void prepareToolStripMenuItemList(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem item in items)
            {
                if (item.HasDropDownItems)  //если у пункта есть дочерние элементы
                {
                    prepareToolStripMenuItemList(item.DropDownItems);
                    continue;
                }

                if ((string)item.Tag == "single")
                    item.Visible = formMain.dataGridViewConverter.SelectedRows.Count == 1;

                if (item.Name == "showWaypointsToolStripMenuItem" || item.Name == "showOnMapToolStripMenuItem")
                    if (formMain.dataGridViewConverter.SelectedRows.Count == 1)
                    {
                        BaseTrack tf = formMain.Tracks[formMain.dataGridViewConverter.SelectedRows[0].Index];
                        if (item.Name == "showWaypointsToolStripMenuItem")
                            item.Checked = formMain.showingWaypointsList.Contains(tf);
                        if (item.Name == "showOnMapToolStripMenuItem")
                            item.Checked = formMain.showingRoutesList.Contains(tf);
                    }
                    else
                        item.CheckState = CheckState.Indeterminate;
            }
        }

        /// <summary>
        /// удаление маршрута из списка
        /// </summary>
        /// <param name="route"></param>
        public void DeleteRoute(BaseTrack route)
        {
            TrackFileList tl = new TrackFileList() { route };
            deleteRoute(tl);
        }

        /// <summary>
        /// удаление нескольких  маршрутов из списка
        /// </summary>
        /// <param name="list"></param>
        private void deleteRoute(TrackFileList list)
        {
            foreach (BaseTrack tf in list)
                formMain.Tracks.Remove(tf);

            if (formMain.Tracks.Count > 0)
                Vars.currentSelectedTrack = formMain.Tracks[0];
            else
                Vars.currentSelectedTrack = null;
            this.RefreshData();
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
        }

        /// <summary>
        /// начало редактирования маршута , который уже есть в списке маршутов
        /// </summary>
        /// <param name="route"></param>
        public void BeginEditRoute(TrackFile route)
        {
            if (!formMain.Tracks.Contains(route))
                throw new ApplicationException("Маршрута нет в списке");

            BaseTrack backup = route.Clone(); //запоминаем старый маршрут
            TrackFile ed = route.Clone() as TrackFile; //новый маршрут для редактирования
            formMain.Tracks.Remove(route); //удаление из списка
            RefreshData();
            //начало редактирования
            formMain.mapHelper.BeginEditRoute(ed,
                (tf) =>
                {
                    EndEditRouteOrTrip(tf);
                },
                () =>
                {
                    backup.CalculateAll();
                    AddRouteToList(backup);
                }
                );
            Vars.currentSelectedTrack = null;
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
        }

        /// <summary>
        /// начало редактирования путешествия, которое уже есть в списке маршутов
        /// </summary>
        /// <param name="tripRouteFile">путешествие для редактирования</param>
        private void beginEditTrip(TripRouteFile tripRouteFile)
        {
            if (!formMain.Tracks.Contains(tripRouteFile))
                throw new ApplicationException("Маршрута нет в списке");

            BaseTrack backup = tripRouteFile.Clone(); //запоминаем старый маршрут
            TripRouteFile ed = tripRouteFile.Clone() as TripRouteFile; //новый маршрут для редактирования
            formMain.Tracks.Remove(tripRouteFile); //удаление из списка
            RefreshData();
            //начало редактирования
            formMain.mapHelper.BeginEditTrip(ed,
                (tf) =>
                {
                    EndEditRouteOrTrip(tf);
                },
                () =>
                {
                    backup.CalculateAll();
                    AddRouteToList(backup);
                }
                );
            Vars.currentSelectedTrack = null;
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
        }


        /// <summary>
        /// присваивает текущему выделенному треку значение из Vars.currentSelectedTrack
        /// </summary>
        internal void UpdateSelectedTrack()
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count > 0)
            {
                int index = formMain.dataGridViewConverter.SelectedRows[0].Index;
                formMain.Tracks[index] = Vars.currentSelectedTrack;
            }
        }

        /// <summary>
        /// открытие файла 
        /// </summary>
        /// <param name="FileName"> имя файла</param>
        public void OpenFile(string FileName)
        {
            if (!File.Exists(FileName))
                return;
            BaseTrack toShow = null;

            string ext = Path.GetExtension(FileName).ToLower();
            switch (ext)
            {
                case ".trr": //файл путешествия
                    TripRouteFile tr = Serializer.DeserializeTripRouteFile(FileName);
                    AddRouteToList(tr);
                    formMain.mapHelper.Clear();
                    toShow = tr;
                    break;
                case ".adrs"://файл списка адресов
                    //открывается в новом потоке т.к. открытие идёт долго
                    new Task(new Action(() =>
                    {
                        BaseTrack tf = Serializer.DeserializeTrackFile(FileName, formMain.SetCurrentOperation);
                        formMain.Invoke(new Action(() =>
                        {
                            AddRouteToList(tf);
                        }));
                    })).Start();
                    break;
                default://остальные файлы (где один маршрут на файл)
                    TrackFileList tfl = Serializer.DeserializeTrackFileList(FileName);
                    if (tfl.Count > 0)
                    {
                        toShow = tfl[0];
                        AddRouteToList(tfl);
                    }
                    break;
            }

            Vars.Options.Converter.AddRecentFile(FileName); //добавление последнего открытого файла в список в меню
            Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(FileName); //послденяя открытая папка
            if (formMain.Tracks.Count > 0)
                Vars.currentSelectedTrack = formMain.Tracks[0];
            RefreshData();
        }

        /// <summary>
        /// открытие ссылки
        /// </summary>
        /// <param name="link"></param>
        public void OpenLink(string link)
        {
            BaseTrack tf = Serializer.DeserializeTrackFile(link);
            AddRouteToList(tf);
        }


        private void saveAllInOne(object sender)
        {
            if (formMain.Tracks.Count == 0)
            {
                MessageBox.Show(formMain, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //выбор элементов для сохранения
            TrackFileList tfs;

            //если контекстное меню, то выбираем выделенные маршруты
            if (sender is ToolStripMenuItem)
            {
                if (formMain.dataGridViewConverter.SelectedRows.Count < 1)
                {
                    MessageBox.Show(formMain, "Для сохранения сначала выделите маршруты!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tfs = new TrackFileList();
                foreach (DataGridViewRow row in formMain.dataGridViewConverter.SelectedRows)
                    tfs.Add(formMain.Tracks[row.Index]);
            }
            //если главное меню, выбираем все загруженные маршруты
            else
                tfs = formMain.Tracks;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter += "Файл маршрутов OziExplorer(*.rte)|*.rte";
            sf.Filter += "| Файл Google Earth (*.kml)|*.kml";
            sf.Filter += "| Архивированный файл Google Earth (*.kmz)|*.kmz";
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveAllInOne;
            sf.AddExtension = true;
            sf.InitialDirectory = Application.StartupPath;
            sf.FileName = tfs.Count > 0 ? tfs[0].Name : "";

            if (sf.ShowDialog() == DialogResult.OK)
            {
                if (Program.winWaitingNullOrDisposed)
                    Program.winWaiting = new FormWaiting();

                Program.winWaiting.Show(formMain);
                Program.winWaiting.Focus();
                try
                {
                    Task pr = new Task(new Action(() =>
                    {
                        switch (Path.GetExtension(sf.FileName).ToLower())
                        {
                            case ".rte":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.RteFile);
                                break;
                            case ".kml":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.KmlFile);
                                break;
                            case ".kmz":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.KmzFile);
                                break;
                        }


                    }));
                    pr.Start();
                    pr.Wait();
                    Vars.Options.Common.LastSaveExtensionNumberSaveAllInOne = sf.FilterIndex;
                    MessageBox.Show("Файл сохранен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(formMain, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    Program.winWaiting.Visible = false;
                }

            }
        }

        /// <summary>
        /// нажатие на пункт меню последний открытый файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            try
            {
                OpenFile((string)item.Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show(formMain, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Vars.Options.Converter.RecentFiles.Remove((string)item.Tag);
                refreshRecentFiles();
            }
        }





        #region Контекстное меню списка маршрутов

        internal void ContextMenuListOpening(object sender, CancelEventArgs e)
        {
            //если все выделенные маршрты - TrackFile, то добавление пункта "преобразовать в путешествие и объединить в путешествие"
            bool allTF = true;
            foreach (DataGridViewRow dr in formMain.dataGridViewConverter.SelectedRows)
                allTF &= formMain.Tracks[dr.Index] is TrackFile;
            if (allTF)
            {
                formMain.toTripRouteFileToolStripMenuItem.Visible = true;
                formMain.joinToTripRouteToolStripMenuItem.Visible = formMain.dataGridViewConverter.SelectedRows.Count > 1; //объединение только если больше одного маршрута
            }
            else
            {
                formMain.toTripRouteFileToolStripMenuItem.Visible = false;
                formMain.joinToTripRouteToolStripMenuItem.Visible = false;
            }

            //если один выделенный маршрут - путешествие, то переименовываем кнопку редактирования
            if (formMain.dataGridViewConverter.SelectedRows.Count == 1)
            {
                int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;
                BaseTrack bt = formMain.Tracks[ind];
                if (bt is TripRouteFile)
                    formMain.editRouteMapToolStripMenuItem.Text = "Редактировать путешествие";
                else
                    formMain.editRouteMapToolStripMenuItem.Text = "Редактировать на карте";
            }

            //подготовка кнопки "открыть в проводнике"
            formMain.openRouteFolderToolStripMenuItem.Visible = true;
            foreach (DataGridViewRow dr in formMain.dataGridViewConverter.SelectedRows)
                formMain.openRouteFolderToolStripMenuItem.Visible &= File.Exists(formMain.Tracks[dr.Index].FilePath);

            //если файл один и не путешествие, то видна кнопка построения оптимального маршурта
            int index = formMain.dataGridViewConverter.SelectedRows[0].Index;
            BaseTrack t = formMain.Tracks[index];
            bool vis = t is TrackFile && formMain.dataGridViewConverter.SelectedRows.Count == 1;
            formMain.createOptimalOnBaseToolStripMenuItem.Visible = vis;
            formMain.separateRouteToolStripMenuItem.Visible = vis; // видимость кнопки "разделить маршрут"

            //если элемент - первое контектное меню, то выбираем пункты на основе тегов
            if (sender is ContextMenuStrip)
            {
                ContextMenuStrip menu = (ContextMenuStrip)sender;
                //если не выделено ни одного элемента, то не выводим меню
                if (formMain.dataGridViewConverter.SelectedRows.Count < 1)
                {
                    e.Cancel = true;
                    return;
                }
                prepareToolStripMenuItemList(menu.Items);
            }

            //если элемент - пункт с дочерними элементами, то выбираем пункты
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menu = (ToolStripMenuItem)sender;
                prepareToolStripMenuItemList(menu.DropDownItems);
            }

        }

        internal void ToolStripRemove(EventArgs e)
        {
            TrackFileList list = new TrackFileList();
            foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
            {
                int row = dgvr.Index;
                BaseTrack tttt = formMain.Tracks[row];
                list.Add(tttt);
            }
            deleteRoute(list);

        }

        internal void ToolStripAddToComparison(EventArgs e)
        {
            if (Program.winCompareTrackNullOrDisposed)
                Program.winCompareTrack = new FormTrackComparison();
            if (!Program.winCompareTrack.Visible)
                Program.winCompareTrack.Show(Program.winMain);
            foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                Program.winCompareTrack.AddTrack(formMain.Tracks[dgvr.Index]);
            Program.winCompareTrack.CalculateComparison();
            Program.winCompareTrack.Activate();
        }

        internal void ToolStripAddToJoin(EventArgs e)
        {
            if (Program.winJoinTrackNullOrDisposed)
                Program.winJoinTrack = new FormJoinTracks();
            if (!Program.winJoinTrack.Visible)
                Program.winJoinTrack.Show(Program.winMain);

            foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                Program.winJoinTrack.AddTrack(formMain.Tracks[dgvr.Index]);

            Program.winJoinTrack.Activate();
        }

        internal void ToolStripOpenRouteFolder(EventArgs e)
        {

            if (formMain.dataGridViewConverter.SelectedRows.Count > 1)
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = formMain.dataGridViewConverter.SelectedRows[0].Index;
            BaseTrack tf = formMain.Tracks[row];


            if (tf.FilePath != null && tf.FilePath.Length > 1)
                Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + tf.FilePath));
            else
                MessageBox.Show(formMain, "Этот маршрут еще не сохранен на диске", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        internal void ToolStripElevGraph(EventArgs e)
        {
            TrackFileList tfl = new TrackFileList();
            Task pr = new Task(new Action(() =>
            {
                Program.winMain.BeginOperation();
                foreach (DataGridViewRow row in formMain.dataGridViewConverter.SelectedRows)
                {
                    int rowI = row.Index;
                    BaseTrack tf = formMain.Tracks[rowI];

                    tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.SetCurrentOperation);
                    tf.CalculateAll();
                    tfl.Add(tf);
                }
                formMain.Invoke(new Action(() => new FormElevVisual(tfl) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain)));
                Program.winMain.EndOperation();
            }));
            pr.Start();
        }

        internal void ToolStripElevGraphWithIntermediates(EventArgs e)
        {
            FormReadText frt = new FormReadText(
               DialogType.ReadNumber,
               "Bведите минимальное расстояние в метрах между точками",
               "",
               false,
               false,
               false,
               false);
            if (frt.ShowDialog(formMain) == DialogResult.OK)
            {
                TrackFileList tfl = new TrackFileList();
                Task pr = new Task(new Action(() =>
                {
                    Program.winMain.BeginOperation();
                    foreach (DataGridViewRow row in formMain.dataGridViewConverter.SelectedRows)
                    {
                        int rowI = row.Index;
                        BaseTrack tf = formMain.Tracks[rowI].Clone();

                        double lg = double.Parse(frt.Result);
                        tf = tf.AddIntermediatePoints(lg);

                        tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.SetCurrentOperation);
                        tf.CalculateAll();
                        tfl.Add(tf);
                    }
                    formMain.Invoke(new Action(() => new FormElevVisual(tfl) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain)));
                    Program.winMain.EndOperation();
                }));
                pr.Start();
            }
        }

        internal void ToolStripShowRouteOnMap(EventArgs e)
        {
            foreach (DataGridViewRow r in formMain.dataGridViewConverter.SelectedRows)
            {

                BaseTrack tf = formMain.Tracks[r.Index];
                if (formMain.showingRoutesList.Contains(tf))
                    formMain.showingRoutesList.Remove(tf);
                else
                {
                    if (tf.Color.IsEmpty)
                        tf.Color = Vars.Options.Converter.GetColor();
                    formMain.showingRoutesList.Add(tf);
                }
                tf.IsVisible = !tf.IsVisible;
                formMain.mapHelper.RefreshData();
            }
        }

        internal void ToolStripShowWaypoints(EventArgs e)
        {
            foreach (DataGridViewRow r in formMain.dataGridViewConverter.SelectedRows)
            {
                BaseTrack tf = formMain.Tracks[r.Index];
                if (formMain.showingWaypointsList.Contains(tf))
                {
                    formMain.showingWaypointsList.Remove(tf);
                    formMain.mapHelper.DeleteWaypoints(tf);
                }
                else
                {
                    formMain.showingWaypointsList.Add(tf);
                    formMain.mapHelper.ShowWaypoints(tf, false, true);
                }
            }
        }

        internal void ToolStripJoinToTripRoute(EventArgs e)
        {
            int first = formMain.dataGridViewConverter.SelectedRows[0].Index;
            TrackFileList tfl = new TrackFileList();
            foreach (DataGridViewRow dr in formMain.dataGridViewConverter.SelectedRows)
            {
                int ind = dr.Index;
                TrackFile tf = (TrackFile)formMain.Tracks[ind - tfl.Count];
                tfl.Add(tf);
                formMain.Tracks.Remove(tf);
            }
            TripRouteFile trr = tfl.ToTripRoute();
            formMain.Tracks.Insert(first, trr);
            Vars.currentSelectedTrack = trr;
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
            RefreshData();
        }

        internal void ToolStripConvertToTripRoute(EventArgs e)
        {
            foreach (DataGridViewRow dr in formMain.dataGridViewConverter.SelectedRows)
            {
                int ind = dr.Index;
                TrackFile tf = (TrackFile)formMain.Tracks[ind];
                TripRouteFile tr = tf.ToTripRoute();
                formMain.Tracks.Remove(tf);
                formMain.Tracks.Insert(ind, tr);
            }
            Vars.currentSelectedTrack = null;
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
            RefreshData();
        }

        internal void ToolStripNormalizeTrack(EventArgs e)
        {
            int min = int.MaxValue;
            foreach (DataGridViewRow r in formMain.dataGridViewConverter.SelectedRows)
            {
                if (formMain.Tracks[r.Index] is TrackFile)
                {
                    if (min > r.Index)
                        min = r.Index;
                    formMain.Tracks[r.Index] = TrackHandler.Normalize(formMain.Tracks[r.Index] as TrackFile, Vars.Options.Converter.MinimalNormalizeAngle, Vars.Options.Converter.NormalizerBehavior);
                }
            }
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
            formMain.dataGridViewConverter.ClearSelection();
            formMain.dataGridViewConverter.Rows[min].Selected = true;
        }

        /// <summary>
        /// построение оптимального маршрута на основе 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ToolStripCreateOptimalOnBase(object sender, EventArgs e)
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count > 1)
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = formMain.dataGridViewConverter.SelectedCells[0].RowIndex;
            if (!(formMain.Tracks[row] is TrackFile))
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен маршрут, а не путешествие!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TrackFile track = (TrackFile)formMain.Tracks[row];

            if (track.Count < 3)
            {
                MessageBox.Show(formMain, "Для построения оптимального маршрута надо поставить хотя бы 3 точки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TrackPoint startPoint = null;
            TrackPoint endPoint = null;


            FormSelectPoint fsp = new FormSelectPoint(track, true, "Выберите начальную точку");
            if (fsp.ShowDialog(Program.winMain) == DialogResult.OK)
                startPoint = fsp.Result;
            else
                return;

            bool isCycled = startPoint == null;

            if (Vars.Options.Map.OptimalRouteMethod == OptimalMethodType.PolarSearch && !isCycled)
            {
                FormSelectPoint fsp1 = new FormSelectPoint(track, startPoint, false, "Выберите конечную точку");
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
                            route = new BranchBounds(Program.winMain.SetCurrentOperation).Make(track, startPoint, isCycled);
                            break;
                        case OptimalMethodType.Greedy:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(track, startPoint, isCycled);
                            break;
                        case OptimalMethodType.FullSearch:
                            route = new FullSearch(Program.winMain.SetCurrentOperation).Make(track, startPoint, isCycled);
                            break;
                        case OptimalMethodType.PolarSearch:
                            route = new PolarSearch(Program.winMain.SetCurrentOperation).Make(track, startPoint, endPoint, isCycled);
                            break;
                        default:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(track, startPoint, isCycled);
                            break;
                    }

                    //вывод маршрута на карту в базовом потоке
                    formMain.Invoke(new Action(() =>
                    {
                        route.CalculateAll();
                        route.Name = "Оптимальный маршрут";
                        TimeSpan span = DateTime.Now - start;
                        MessageBox.Show("Маршрут построен за:\r\n" + span.ToString(@"mm\:ss\.fff"));

                        if (Vars.Options.Services.ChangePathedRoute)
                            //если надо открыть маршрут для редактирования
                            formMain.mapHelper.BeginEditRoute(route, (tf) =>
                            {
//ввод названия марщрута
readName:
                                FormReadText fr = new FormReadText(DialogType.ReadText, "Введите название маршрута", "", false, false, false, false);
                                if (fr.ShowDialog(formMain) == DialogResult.OK)
                                {
                                    tf.Name = fr.Result;
                                    //вычисление параметров
                                    tf.CalculateAll();
                                    //добавление в основное окно списка маршрутов
                                    formMain.converterHelper.AddRouteToList(tf);
                                    //вывод на карту
                                    formMain.mapHelper.ShowRoute(formMain.creatingRoute);
                                    //добавление в список маршрутов на карте
                                    formMain.converterHelper.AddRouteToList(route);
                                }
                                else
                                    switch (MessageBox.Show(formMain, "Отменить создание маршрута? Все именения будут потеряны!", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                                    {
                                        case System.Windows.Forms.DialogResult.No:
                                            //открываем заново ввод названия
                                            goto readName;
                                        case System.Windows.Forms.DialogResult.Yes:
                                            return;
                                    }
                            });
                        else
                        {
                            //если не надо открывать маршрут
                            AddRouteToList(route);
                            formMain.mapHelper.ShowRoute(route);
                        }
                    }));

                }
                catch (Exception exx)
                {
                    formMain.Invoke(new Action(() =>
                         MessageBox.Show(formMain, exx.Message + "\r\n" + (exx.InnerException != null ? exx.InnerException.Message : ""), "Ошибка при создании маршрута", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

        /// <summary>
        /// загрузить адреса в описания точек
        /// </summary>
        /// <param name="e"></param>
        internal void ToolStripLoadAddresses(EventArgs e)
        {
            Program.winMain.BeginOperation();
            Task ts = new Task(new Action(() =>
            {
                int er = 0;
                try
                {
                    foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                    {
                        int row = dgvr.Index;
                        BaseTrack tf = formMain.Tracks[row];
                        new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddresses(tf, Program.winMain.SetCurrentOperation);
                        er = 0;
                    }
                }
                catch (Exception ex)
                {
                    er = -1;
                    formMain.Invoke(new Action(() =>
                        MessageBox.Show(formMain, "Не удалось получить адреса из-за проблем с соединением.\r\n" + ex.Message + "\r\nПроверьте соединение с Интернет", "Загрузка адресов", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ));
                }
                finally
                {
                    formMain.Invoke(new Action(() => Program.winMain.EndOperation()));
                }
            }));
            ts.Start();
        }

        /// <summary>
        /// разделение маршрута на 2 части
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ToolStripSeparateRoute(object sender, EventArgs e)
        {
            //1. выбор способа: по точке или по расстоянию
            //2. раделение маршурта и добавление в список
            if (formMain.dataGridViewConverter.SelectedRows.Count != 1)
            {
                MessageBox.Show(formMain, "Должен быть выделен только один маршрут!", "Разбиение маршрута", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;
            TrackFile route = (TrackFile)formMain.Tracks[ind]; //разделяемый маршрут
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
                formMain.Tracks.Insert(ind, second);
                formMain.Tracks.Insert(ind, first);
                formMain.Tracks.Remove(route);

                //только при добавлении в список маршуртов
                if (formMain.Tracks.Count > 0)
                    Vars.currentSelectedTrack = formMain.Tracks[0];
                else
                    Vars.currentSelectedTrack = null;
                this.RefreshData();
                formMain.mapHelper.RefreshData();
                formMain.graphHelper.RefreshData();
                formMain.pointsHelper.RefreshData();

            });

            #endregion

            FormSeparateTrackTypeDialog fstt = new FormSeparateTrackTypeDialog("Выберите один из способов разделения маршрута", "Разделение маршрута");
            if (fstt.ShowDialog(formMain) == DialogResult.OK)
            {
                SeparateRouteType sep_type = fstt.Result;
                bool addCommon = fstt.ResultAddCommon;
                switch (sep_type)
                {
                    case SeparateRouteType.Length:
#region разделение по длине
readLength:
                        FormReadText frt = new FormReadText(DialogType.ReadNumber, "Введите длину первого отрезка в км. Максимальная длина " + route.Distance.ToString("0.0") + " км.", "", false, false, false, false);
                        if (frt.ShowDialog(formMain) == DialogResult.OK)
                        {
                            double length = double.Parse(frt.Result
                                .Trim()
                                .Replace('.', Vars.DecimalSeparator)
                                .Replace('.', Vars.DecimalSeparator));

                            if (length >= route.Distance)
                            {
                                MessageBox.Show(formMain, "Длина первого отрезка должна быть меньше длины маршрута", "Разделение маршрута", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            formMain.isSelectingPoint = false;
                            formMain.gmapControlMap.Cursor = Cursors.Arrow;

                            //добавление в список
                            addList.Invoke();
                        });
                        formMain.mapHelper.BeginSelectPoint(after);
                        #endregion
                        break;
                    case SeparateRouteType.None:
                        return;
                }
            }
            else
                return;
        }

        internal void ToolStripApproximateAltitudes(EventArgs e)
        {
            FormReadText frt = new FormReadText(
               DialogType.ReadNumber,
               "Введите степень полинома",
               Vars.Options.Converter.ApproximateAmount.ToString(),
               false,
               false,
               false,
               false);

            if (frt.ShowDialog(formMain) == System.Windows.Forms.DialogResult.OK)
            {
                int amount = (int)double.Parse(frt.Result);
                foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                {
                    int row = dgvr.Index;
                    BaseTrack tf = formMain.Tracks[row];
                    tf = TrackHandler.Approximate(tf, amount);
                }
                MessageBox.Show(formMain, "Обработка завершена", "Выполнено!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //если выделен один маршрут, то одновлям данные в окнах
                if (formMain.dataGridViewConverter.SelectedRows.Count == 1)
                {
                    int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;
                    Vars.currentSelectedTrack = formMain.Tracks[ind];
                    formMain.mapHelper.RefreshData();
                    formMain.graphHelper.RefreshData();
                    formMain.pointsHelper.RefreshData();
                }
            }
        }

        internal void ToolStripRemoveElevations(EventArgs e)
        {
            if (MessageBox.Show(formMain, "Вы действительно хотите очистить высоты точек у выбранных маршрутов?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                {
                    int row = dgvr.Index;
                    BaseTrack tf = formMain.Tracks[row];
                    tf.ClearAltitudes();
                }

                //если выделен один маршрут, то обновлям данные в окнах
                if (formMain.dataGridViewConverter.SelectedRows.Count == 1)
                {
                    int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;
                    Vars.currentSelectedTrack = this.formMain.Tracks[ind];
                    formMain.mapHelper.RefreshData();
                    formMain.graphHelper.RefreshData();
                    formMain.pointsHelper.RefreshData();
                }
            }
        }

        internal void ToolStripLoadElevations(EventArgs e)
        {
            Program.winMain.BeginOperation();
            Task ts = new Task(new Action(() =>
            {
                int er = 0;
                try
                {
                    foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.SelectedRows)
                    {
                        int row = dgvr.Index;
                        BaseTrack tf = formMain.Tracks[row];
                        tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.SetCurrentOperation);
                        er = 0;
                    }
                }
                catch (Exception ex)
                {
                    er = -1;
                    formMain.Invoke(new Action(() =>
                        MessageBox.Show(formMain, "Не удалось получить высоты из-за проблем с соединением.\r\n" + ex.Message + "\r\nПроверьте соединение с Интернет", "Загрузка высот", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ));
                }
                finally
                {
                    formMain.Invoke(new Action(() => Program.winMain.EndOperation()));
                }
                formMain.Invoke(new Action(() =>
                {
                    if (er == 0)
                    {
                        //если выделен один маршрут, то обновлям данные в окнах
                        if (formMain.dataGridViewConverter.SelectedRows.Count == 1)
                        {
                            int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;
                            Vars.currentSelectedTrack = formMain.Tracks[ind];
                            formMain.mapHelper.RefreshData();
                            formMain.graphHelper.RefreshData();
                            formMain.pointsHelper.RefreshData();
                        }
                    }
                }));
            }));
            ts.Start();
        }

        internal void ToolStripEditWaypoints(EventArgs e)
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count > 1)
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = formMain.dataGridViewConverter.SelectedCells[0].RowIndex;
            new FormPoints(formMain.Tracks[row]) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain);
        }

        internal void ToolStripSave(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewConverter.SelectedRows[0].Index;
            BaseTrack tf = formMain.Tracks[row];


            if (!string.IsNullOrEmpty(tf.FilePath))
            {
                if (MessageBox.Show(formMain, "Вы действительно хотите перезаписать файл\r\n " + tf.FilePath, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (Path.GetExtension(tf.FilePath) == ".kml" || Path.GetExtension(tf.FilePath) == ".kmz")
                    {
                        string msg = "Данный формат (" + tf.Format.ToString() + ") поддерживает хранение маршрутов и путевых точек." +
                            "\r\nЕсли продолжить сохранение, информация о путевых точках в этом файле может быть утеряна." +
                            "\r\nДля сохранения маршрутов и точек в один файл используйте соответствующий пункт меню карты." +
                            "\r\nПродолжить?";
                        if (MessageBox.Show(formMain, msg, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                            return;
                    }
                    Serializer.Serialize(tf.FilePath, tf, tf.Format);
                }
            }
            else
                ToolStripSaveAsFile(sender, e);

        }

        /// <summary>
        /// Редактировать маршрут на карте
        /// </summary>
        /// <param name="e"></param>
        internal void ToolStripEditRoute(EventArgs e)
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count > 1)
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BaseTrack bt = formMain.Tracks[formMain.dataGridViewConverter.SelectedRows[0].Index];

            //если в маршруте больше 1000 точек, то предлагаем отменить редактирование
            if (bt.Count > 1000)
                if (MessageBox.Show("В маршруте больше 1000 точек, вы уверены, что хотите продолжить?\r\nМаршруты с большим количеством точек могут вызвать ошибки при редактировании, рекомендуется разделить маршрут на несколько отдельных частей.\r\nПродолжить редактирование?", "Редактирование маршрута", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                    return;
                
            if (bt is TrackFile)
                BeginEditRoute(bt as TrackFile);
            else
                beginEditTrip(bt as TripRouteFile);
        }

        internal void ToolStripSaveAsWikimapia(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            if (((ToolStripMenuItem)sender).Name == "saveWikimapiaContextToolStripMenuItem")
                tf = formMain.Tracks[formMain.dataGridViewConverter.SelectedCells[0].RowIndex];
            else
            {
                if (formMain.Tracks.Count > 1)
                {
                    MessageBox.Show(formMain, "Для сохранения в ссылку должен быть загружен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (formMain.Tracks.Count == 0)
                {
                    MessageBox.Show(formMain, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = formMain.Tracks[0];
            }

            FormReadText of = new FormReadText(DialogType.ReadNumber, "Введите количество точек в результате, всего: " + tf.Count, "", true, false, false, false);

            int count = 0;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                count = Convert.ToInt32(of.Result);
            }
            else
            {
                MessageBox.Show(formMain, "Надо указать количество точек в ссылке!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string res = Serializer.Serialize(count.ToString(), tf, FileFormats.WikimapiaLink);

            if (!(res.Count<char>() > 64000))
                new FormReadText(DialogType.ExportText, "Ссылка на маршрут", res, false, true, true, true).ShowDialog();
            else
            {
                MessageBox.Show(formMain, "Слишком длинный маршрут для вывода в формате ссылки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        internal void ToolStripSaveAsYandex(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            if (((ToolStripMenuItem)sender).Name == "saveYandexContextToolStripMenuItem")
                tf = formMain.Tracks[formMain.dataGridViewConverter.SelectedCells[0].RowIndex];
            else
            {
                if (formMain.Tracks.Count > 1)
                {
                    MessageBox.Show(formMain, "Для сохранения в ссылку должен быть загружен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (formMain.Tracks.Count == 0)
                {
                    MessageBox.Show(formMain, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = formMain.Tracks[0];
            }
            FormReadText of = new FormReadText(DialogType.ReadNumber, "Введите количество точек в результате, всего: " + tf.Count, "", true, false, false, false);

            int count = 0;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                count = Convert.ToInt32(of.Result);
            }
            else
            {
                MessageBox.Show(formMain, "Надо указать количество точек в ссылке!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string res = Serializer.Serialize(count.ToString(), tf, FileFormats.YandexLink);

            if (!(res.Count<char>() > 64000))
                new FormReadText(DialogType.ExportText, "Ссылка на маршрут", res, false, true, true, true).ShowDialog();
            else
            {
                MessageBox.Show(formMain, "Слишком длинный маршрут для вывода в формате ссылки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        internal void ToolStripInformation(EventArgs e)
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count > 1)
            {
                MessageBox.Show(formMain, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = formMain.dataGridViewConverter.SelectedCells[0].RowIndex;
            FormTrackInformation fti = new FormTrackInformation(formMain.Tracks[row]);
            if (fti.ShowDialog() == DialogResult.OK)
            {
                formMain.Tracks[row] = fti.Result;
                RefreshData();

                Vars.currentSelectedTrack = formMain.Tracks[row];
                formMain.mapHelper.RefreshData();
                formMain.graphHelper.RefreshData();
                formMain.pointsHelper.RefreshData();
            }
        }

        internal void ToolStripSaveAsFile(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            //если кнопка контекстного меню
            if (((ToolStripMenuItem)sender).Name == "saveFileContextToolStripMenuItem1" || ((ToolStripMenuItem)sender).Name == "saveToolStripMenuItem1")
                if (formMain.dataGridViewConverter.SelectedRows.Count > 1) //если выделено больше одного маршрута, то сохранение в один файл
                {
                    saveAllInOne(sender);
                    return;
                }
                else
                    tf = formMain.Tracks[formMain.dataGridViewConverter.SelectedRows[0].Index];
            else
            {//если нажата кнопка главного меню
                if (formMain.Tracks.Count > 1)
                {
                    MessageBox.Show(formMain, "Для сохранения в один файл должен быть загружен только один маршрут! Если требуется сохранить несколько маршрутов в один файл, используйте соответствующий пункт меню.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (formMain.Tracks.Count == 0)
                {
                    MessageBox.Show(formMain, "Для сохранения сначала надо загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = formMain.Tracks[0];
            }


            SaveFileDialog sf = new SaveFileDialog
            {
                Filter = "Файл маршрута TrackConverter(*.trr)|*.trr"
            };
            sf.Filter += "|Файл маршрута Androzic (*.rt2)|*.rt2";
            sf.Filter += "|Треки Androzic (*.plt)|*.plt";
            sf.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
            sf.Filter += "|Файл координат(*.crd)|*.crd";
            sf.Filter += "|Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";
            sf.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
            sf.Filter += "|Файл NMEA(*.nmea)|*.nmea";
            sf.Filter += "|Файл Excel(*.csv)|*.csv";
            sf.Filter += "|Файл нескольких маршрутов Ozi(*.rte)|*.rte";
            sf.Filter += "|Текстовый файл(*.txt)|*.txt";
            sf.Filter += "|Список адресов(*.adrs)|*.adrs";

            sf.AddExtension = true;
            sf.FileName = tf.Name;
            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack;

            //sf.FileName = tf.Name; //при сохранении как файл не записывать сохранённый путь


            if (sf.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(tf.FilePath))
                    tf.FilePath = sf.FileName;
                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".rt2":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.Rt2File);
                        break;
                    case ".plt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.PltFile);
                        break;
                    case ".wpt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.WptFile);
                        break;
                    case ".crd":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CrdFile);
                        break;
                    case ".kml":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmlFile);
                        break;
                    case ".gpx":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.GpxFile);
                        break;
                    case ".kmz":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmzFile);
                        break;
                    case ".osm":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.OsmFile);
                        break;
                    case ".nmea":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.NmeaFile);
                        break;
                    case ".csv":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CsvFile);
                        break;
                    case ".rte":
                        if (tf is TripRouteFile)
                            Serializer.Serialize(sf.FileName, (tf as TripRouteFile).DaysRoutes, FileFormats.RteFile);
                        else
                            Serializer.Serialize(sf.FileName, new TrackFileList() { tf }, FileFormats.RteFile);
                        break;
                    case ".txt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.TxtFile);
                        break;
                    case ".adrs":
                        Program.winMain.BeginOperation();
                        Task ts = new Task(new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, tf, FileFormats.AddressList, Program.winMain.SetCurrentOperation);
                            Program.winMain.EndOperation();
                        }));
                        ts.Start();
                        break;
                    case ".trr":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.TrrFile);
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack = sf.FilterIndex;
            }
        }


        #endregion

        #region события списка маршрутов


        internal void DataGridViewSelectionChanged(EventArgs e)
        {
            if (formMain.dataGridViewConverter.SelectedRows.Count != 1)
                return;
            int ind = formMain.dataGridViewConverter.SelectedRows[0].Index;

            BaseTrack tf = formMain.Tracks[ind];

            Vars.currentSelectedTrack = tf.Clone();
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
        }

        internal void DataGridViewPaint(PaintEventArgs e)
        {
            if (formMain.dataGridViewConverter.RowCount > 0 && formMain.dataGridViewConverter.ColumnCount > 0 && formMain.Tracks.Count > 0)
            {
                int i = 0;
                foreach (DataGridViewRow dgvr in formMain.dataGridViewConverter.Rows)
                {
                    BaseTrack tf = formMain.Tracks[i];
                    DataGridViewCell cell = dgvr.Cells["Цвет"];
                    cell.Style = new DataGridViewCellStyle() { BackColor = tf.Color };
                    i++;
                }
            }
        }

        internal void DataGridViewKeyDown(KeyEventArgs e)
        {
            if (formMain.dataGridViewConverter.RowCount == 0)
                return;
            if (e.KeyData != Keys.Delete)
                return;
            ToolStripRemove(null);
        }

        internal void DataGridViewDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.UnicodeText))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        internal void DataGridViewDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string ss in s)
                    OpenFile(ss);
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string s = (string)e.Data.GetData(DataFormats.UnicodeText, false);
                this.OpenLink(s);
            }
        }

        internal void DataGridViewCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            Keyboard ss = new Keyboard();
            if (!ss.CtrlKeyDown && e.Button != MouseButtons.Right)
                formMain.dataGridViewConverter.ClearSelection();
            if (e.Button == MouseButtons.Right)
                formMain.dataGridViewConverter.Rows[e.RowIndex].Selected = true;
        }

        internal void DataGridViewCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            formMain.dataGridViewConverter.ClearSelection();
            formMain.dataGridViewConverter.Rows[e.RowIndex].Selected = true;
            if (formMain.Tracks[e.RowIndex] is TripRouteFile)
                ToolStripEditRoute(new EventArgs());
            else
                ToolStripInformation(new EventArgs());
        }

        internal void DataGridViewCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (formMain.Tracks.Count == 0 || e.RowIndex >= formMain.Tracks.Count)
                return;
            if ((e.ColumnIndex == formMain.dataGridViewConverter.Columns[0].Index) && e.Value != null)
            {
                DataGridViewCell cell = formMain.dataGridViewConverter.Rows[e.RowIndex].Cells[e.ColumnIndex];
                BaseTrack tf = formMain.Tracks[e.RowIndex];
                string text = "Название: " + tf.Name + "\r\n";
                text += "Длина: " + tf.Distance.ToString("00.00") + " км\r\n";
                text += "Количество точек: " + tf.Count + "\r\n";
                text += "Средняя скорость: " + tf.KmphSpeed + " км/ч\r\n";
                text += "Описание: " + tf.Description + "\r\n";
                text += "Общее время: " + tf.Time + "\r\n";
                text += "Имя файла: " + tf.FileName + "\r\n";
                cell.ToolTipText = text;
            }
        }

        internal void DataGridViewCellClick(DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            formMain.dataGridViewConverter.Rows[e.RowIndex].Selected = true;
        }


        #endregion
    }
}
