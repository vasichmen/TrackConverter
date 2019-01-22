using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Mathematic.Routing;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Map;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI.Shell
{
    internal class MainHelper
    {
        private FormMain formMain;

        #region вспомогательные методы

        /// <summary>
        /// обновление списка последних загруженных файлов
        /// </summary>
        public void RefreshRecentFiles()
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
        /// нажатие на пункт меню последний открытый файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            try
            {
                formMain.converterHelper.OpenFile((string)item.Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show(formMain, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Vars.Options.Converter.RecentFiles.Remove((string)item.Tag);
                RefreshRecentFiles();
            }
        }

        #endregion

        #region основное меню

        public MainHelper(FormMain formMain)
        {
            this.formMain = formMain;
        }

        internal void ToolStripSaveFileWaypoints(object sender, EventArgs e)
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
            sf.FileName = "точки_" + DateTime.Now.ToString();



            if (sf.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".rt2":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.Rt2File);
                        break;
                    case ".plt":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.PltFile);
                        break;
                    case ".wpt":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.WptFile);
                        break;
                    case ".crd":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.CrdFile);
                        break;
                    case ".kml":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.KmlFile);
                        break;
                    case "gpx":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.GpxFile);
                        break;
                    case "kmz":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.KmzFile);
                        break;
                    case ".osm":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.OsmFile);
                        break;
                    case "nmea":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.NmeaFile);
                        break;
                    case "csv":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.CsvFile);
                        break;
                    case ".txt":
                        Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.TxtFile);
                        break;
                    case ".adrs":
                        Program.winMain.BeginOperation();
                        Action act = new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, formMain.waypoints, FileFormats.AddressList, Program.winMain.SetCurrentOperation);
                            Program.winMain.EndOperation();
                        });
                        new Task(act).Start();
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack = sf.FilterIndex;
            }
        }

        internal void ToolStripOpen(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog
                {
                    Filter = "Все поддерживаемые форматы(*.trr, *.crd, *.wpt, *.plt, *.rt2, *.kml, *.kmz, *.gpx, *.rte, *.osm, *.nmea, *.csv, *.txt)| *.trr; *.crd; *.wpt; *.plt; *rt2; *.kml; *.kmz; *.gpx; *.rte; *.osm; *.nmea; *.csv; *.txt"
                };
                of.Filter += "|Треки Androzic (*.plt)|*.plt";
                of.Filter += "|Маршрут Androzic (*.rt2)|*.rt2";
                of.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
                of.Filter += "|Файл координат(*.crd)|*.crd";
                of.Filter += "|Файл Google Earth(*.kml)|*.kml";
                of.Filter += "|Файл Google Earth(*.kmz)|*.kmz";
                of.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
                of.Filter += "|Файл маршрутов OziExplorer(*.rte)|*.rte";
                of.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
                of.Filter += "|Файл NMEA(*.nmea)|*.nmea";
                of.Filter += "|Файл списка адресов(*.adrs)|*.adrs";
                of.Filter += "|Файл Excel(*.csv)|*.csv";
                of.Filter += "|Текстовый файл(*.txt)|*.txt";
                of.Filter += "|Маршрут Track Converter(*.trr)|*.trr";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = true;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == DialogResult.OK)
                    foreach (string fn in of.FileNames)
                        formMain.converterHelper.OpenFile(fn);
            }
            catch (Exception ex) { MessageBox.Show(formMain, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        internal void ToolStripSaveFileWaypointsRoutes(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter += "Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";

            sf.AddExtension = true;

            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;

            if (sf.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".kml":
                        GeoFile gf = new GeoFile(formMain.Tracks, formMain.waypoints);
                        Serializer.Serialize(sf.FileName, gf, FileFormats.KmlFile);
                        break;
                    case ".kmz":
                        gf = new GeoFile(formMain.Tracks, formMain.waypoints);
                        Serializer.Serialize(sf.FileName, gf, FileFormats.KmzFile);
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                MessageBox.Show("Файл сохранен!");
            }
        }

        internal void ToolStripCreateRoute(object sender, EventArgs e)
        {
            formMain.creatingRoute = new TrackFile();

            Action<BaseTrack> after = new Action<BaseTrack>((tf) =>
            {
//ввод названия марщрута
readName:
                FormReadText fr = new FormReadText(DialogType.ReadText, "Введите название маршрута", "", false, false, false, false);
                if (fr.ShowDialog(formMain) == DialogResult.OK)
                {
                    tf.Name = fr.Result;

                    formMain.converterHelper.AddRouteToList(tf);

                    //вычисление параметров
                    formMain.creatingRoute.CalculateAll();

                    //вывод на карту
                    formMain.mapHelper.ShowRoute(formMain.creatingRoute);
                }
                else
                    switch (MessageBox.Show(formMain, "Отменить создание маршрута? Все именения будут потеряны!", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    {
                        case DialogResult.Yes:
                            formMain.creatingRouteOverlay.Routes.Clear();
                            formMain.creatingRouteOverlay.Markers.Clear();
                            formMain.creatingRoute = null;
                            break;
                        case DialogResult.No:
                            //открываем заново ввод названия
                            goto readName;
                    }
            });

            formMain.mapHelper.BeginEditRoute(formMain.creatingRoute, after);
        }

        internal void ToolStripCreateTrip(object sender, EventArgs e)
        {
            TripRouteFile trip = new TripRouteFile
            {
                Name = "Новое путешествие"
            };

            Action<TripRouteFile> after = new Action<TripRouteFile>((newTrip) =>
            {
                newTrip.CalculateAll();
                formMain.converterHelper.AddRouteToList(newTrip);
                formMain.creatingTripOverlay.Clear();
            }
                );
            Action canc = new Action(() =>
            {
                formMain.creatingTripOverlay.Clear();
                Vars.currentSelectedTrack = null;
                formMain.mapHelper.RefreshData();
                formMain.converterHelper.RefreshData();
                formMain.pointsHelper.RefreshData();
                formMain.graphHelper.RefreshData();
            });

            formMain.mapHelper.BeginEditTrip(trip, after, canc);
        }

        internal void ToolStripExit(object sender, EventArgs e)
        {
            formMain.Close();
        }

        internal void ToolStripSourceInternet(object sender, EventArgs e)
        {
            //порядок получения данных 
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            Vars.Options.Map.AccessMode = AccessMode.ServerOnly;
            formMain.tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            formMain.tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            formMain.tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }

        internal void ToolStripSourceCache(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.CacheOnly;
            Vars.Options.Map.AccessMode = AccessMode.CacheOnly;
            formMain.tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            formMain.tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            formMain.tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }

        internal void ToolStripSourceInternetCache(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Vars.Options.Map.AccessMode = AccessMode.ServerAndCache;
            formMain.tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            formMain.tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            formMain.tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }

        internal void ToolStripSelectMap(object sender, EventArgs e)
        {
            if (Program.winSaveMapNullOrDisposed)
            {
                Program.winSaveMap = new FormSaveMap();
                Program.winSaveMap.Show(Program.winMain);
            }
            Program.winSaveMap.Activate();
        }

        internal void ToolStripClearRoutes(object sender, EventArgs e)
        {
            foreach (GMapOverlay ov in formMain.gmapControlMap.Overlays)
                ov.Routes.Clear();
        }

        internal void ToolStripClearMarkers(object sender, EventArgs e)
        {
            if (formMain.waypoints != null)
                formMain.waypoints.Clear();
            foreach (GMapOverlay ov in formMain.gmapControlMap.Overlays)
                ov.Markers.Clear();
            formMain.fromPoint = null;
            formMain.toPoint = null;
            formMain.IntermediatePoints = null;
        }

        internal void ToolStripClearAll(object sender, EventArgs e)
        {
            ToolStripClearRoutes(sender, e);
            ToolStripClearMarkers(sender, e);
        }

        internal void ToolStripEditPointFile(object sender, EventArgs e)
        {
            new FormPoints(formMain.waypoints, (tt) => { formMain.mapHelper.EndEditWaypoints(tt); }) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain);
        }

        internal void ToolStripCalculateDistance(object sender, EventArgs e)
        {
            new FormCalculateDistance().Show(formMain);
        }

        internal void ToolStripTransformCoordinate(object sender, EventArgs e)
        {
            new FormTransformCoordinate().Show(formMain);
        }

        internal void ToolStripConsole(object sender, EventArgs e)
        {
            new FormConsole().Show(formMain);
        }

        internal void ToolStripOptions(object sender, EventArgs e)
        {
            if (Program.winOptionsNullOrDisposed)
                Program.winOptions = new FormOptions();
            if (!Program.winOptions.Visible)
                Program.winOptions.Show(formMain);
            Program.winOptions.Activate();
        }

        internal void ToolStripShowNavigator(object sender, EventArgs e)
        {
            if (Program.winNavigatorNullOrDisposed)
                Program.winNavigator = new FormMapNavigator();
            if (!Program.winNavigator.Visible)
                Program.winNavigator.Show(formMain);
            Program.winNavigator.Activate();
        }

        internal void ToolStripHelp(object sender, EventArgs e)
        {
            new FormBrowser().Show(Program.winMain);
        }

        internal void ToolStripAbout(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog(formMain);
        }

        internal void ToolStripEditWaypoints(object sender, EventArgs e)
        {
            FormPoints fp = new FormPoints
            {
                FormBorderStyle = FormBorderStyle.Sizable
            };
            fp.Show(formMain);
        }

        internal void ToolStripShowElevGraphOnRoute(object sender, EventArgs e)
        {
            try
            {
                formMain.creatingRoute = new TrackFile();
                formMain.mapHelper.BeginEditRoute(formMain.creatingRoute, (track) =>
                {
                    BaseTrack added = track;
                    if (Vars.Options.Graphs.IsAddIntermediatePoints)
                        added = track.AddIntermediatePoints(Vars.Options.Graphs.IntermediateDistance);

                    Task pr = new Task(new Action(() =>
                    {
                        Program.winMain.BeginOperation();
                        added = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(added, Program.winMain.SetCurrentOperation);
                        added.CalculateAll();
                        formMain.Invoke(new Action(() => new FormElevVisual(new TrackFileList() { added }) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain)));
                        ;
                        Program.winMain.EndOperation();
                    }));
                    pr.Start();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(formMain, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ToolStripShowElevGraphOnRoute_Paint(object sender, EventArgs e)
        {
            formMain.toolStripMenuItemShowElevGraphOnRoute.ToolTipText = " Будут добавлены дополнительные точки через каждые " + Vars.Options.Graphs.IntermediateDistance + " м. Это значение можно изменить в настройках";
        }

        internal void ToolStripCreateOptimalRoute(object sender, EventArgs e)
        {
            if (formMain.waypoints.Count < 3)
            {
                MessageBox.Show(formMain, "Для построения оптимального маршрута надо поставить хотя бы 3 точки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TrackPoint startPoint = null;
            TrackPoint endPoint = null;


            FormSelectPoint fsp = new FormSelectPoint(formMain.waypoints, true, "Выберите начальную точку");
            if (fsp.ShowDialog(Program.winMain) == DialogResult.OK)
                startPoint = fsp.Result;
            else
                return;

            bool isCycled = startPoint == null;

            if (Vars.Options.Map.OptimalRouteMethod == OptimalMethodType.PolarSearch && !isCycled)
            {
                FormSelectPoint fsp1 = new FormSelectPoint(formMain.waypoints, startPoint, false, "Выберите конечную точку");
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
                            route = new BranchBounds(Program.winMain.SetCurrentOperation).Make(formMain.waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.Greedy:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(formMain.waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.FullSearch:
                            route = new FullSearch(Program.winMain.SetCurrentOperation).Make(formMain.waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.PolarSearch:
                            route = new PolarSearch(Program.winMain.SetCurrentOperation).Make(formMain.waypoints, startPoint, endPoint, isCycled);
                            break;
                        default:
                            route = new Greedy(Program.winMain.SetCurrentOperation).Make(formMain.waypoints, startPoint, isCycled);
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
                            formMain.converterHelper.AddRouteToList(route);
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

        internal void ToolStripPointsToRoute(object sender, EventArgs e)
        {
            if (formMain.waypoints == null)
                formMain.waypoints = new TrackFile();
            if (formMain.waypoints.Count < 2)
            {
                MessageBox.Show(formMain, "Необходимо добавить на карту не менее двух точек", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BaseTrack ntf = formMain.waypoints.Clone();
            ntf.CalculateAll();
            formMain.converterHelper.AddRouteToList(ntf);
            Vars.currentSelectedTrack = ntf;
            formMain.mapHelper.RefreshData();
            formMain.converterHelper.RefreshData();
            formMain.pointsHelper.RefreshData();
            formMain.graphHelper.RefreshData();
        }

        internal void ToolStripRouteToPoints(object sender, EventArgs e)
        {
            if (formMain.Tracks.GetMaxTrack() > 1000)
            {
                MessageBox.Show(formMain, "Количество точек в маршруте не должно быть больше 1000", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (formMain.waypoints == null)
                formMain.waypoints = new TrackFile();
            foreach (TrackFile tf in formMain.Tracks)
                formMain.waypoints.Add(tf);
            formMain.mapHelper.ShowWaypoints(formMain.waypoints, formMain.baseOverlay, false);
        }




        #endregion
    }
}
