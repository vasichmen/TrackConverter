using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.ProviderRecords;
using TrackConverter.Lib.Classes.StackEdits;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Map;
using ZedGraph;

namespace TrackConverter.UI.Shell
{
    public partial class FormMain: Form
    {
        #region ПОЛЯ

        #region КАРТА

        #region флаги, передача параметров в методы

        /// <summary>
        /// индекс активного маркера при создании маршрута
        /// </summary>
        public int selectedPointIndex;

        /// <summary>
        /// если истина, значит создается маршрут
        /// </summary>
        public bool isCreatingRoute;

        /// <summary>
        /// если истина, то при нажатии на карту ЛКМ надо открыть меню редактирования точки и вызвать this.AfterSelectPointAction
        /// </summary>
        public bool isSelectingPoint = false;

        /// <summary>
        /// если истина, то идет измерение расстояния
        /// </summary>
        public bool isRuling;

        /// <summary>
        /// нажата ли кнопка стрелок. Исользкется в методе таймера передвижения карты
        /// </summary>
        public bool canMoveMap = false;

        /// <summary>
        /// при нажатии стрелок здесь информация, какая стрелка нажата
        /// </summary>
        public Keys canMoveMapDirection;

        /// <summary>
        /// действие, которое выполняется после выбора и редактирования точки (при редактировании путешествия)
        /// </summary>
        public Action<TrackPoint> AfterSelectPointAction;

        /// <summary>
        /// действие, выполняемое при отмене выбора точки на карте при создании путешествия
        /// </summary>
        public Action CancelSelectPointAction;

        /// <summary>
        /// координаты маркера до перемещения. (для отмены действия)
        /// </summary>
        public TrackPoint oldMarker = null;

        #endregion

        #region карта, слои, маршруты


        /// <summary>
        /// список загруженных путевых точек
        /// </summary>
        public TrackFile waypoints;

        /// <summary>
        /// создаваемый маршрут
        /// </summary>
        public TrackFile creatingRoute;

        /// <summary>
        /// слой с маркерами и маршрутами на карте
        /// </summary>
        public GMapOverlay baseOverlay;

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        public GMapOverlay creatingRouteOverlay;

        /// <summary>
        /// слой измерения расстояний
        /// </summary>
        public GMapOverlay rulerRouteOverlay;

        /// <summary>
        /// слой вывода создаваемого путешествия
        /// </summary>
        public GMapOverlay creatingTripOverlay;

        /// <summary>
        /// слой маркеров найденых результатов поиска
        /// </summary>
        public GMapOverlay searchOverlay;

        /// <summary>
        /// измерение расстояния
        /// </summary>
        public TrackFile rulerRoute;

        /// <summary>
        /// текущий выбранный маркер, на котором находится мышь
        /// </summary>
        public MapMarker currentMarker;

        /// <summary>
        /// точка, на которой произошло нажатие (для вывода ContextMenu на пустой карте)
        /// </summary>
        public PointLatLng pointClicked;

        /// <summary>
        /// маркер, на котором произошло нажатие(для вывода контекстного меню маркера)
        /// </summary>
        public MapMarker markerClicked;

        /// <summary>
        /// маршрут, для которого произошло нажатие (для вывода контекстного меню)
        /// </summary>
        public GMapRoute routeClicked;

        /// <summary>
        /// слой выделенной точки
        /// </summary>
        public GMapOverlay selectedPointsOverlay;

        /// <summary>
        /// слой, выделенного в списке маршрутов, маршрута
        /// </summary>
        public GMapOverlay selectedRouteOverlay;

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        public GMapOverlay fromToOverlay;

        /// <summary>
        /// слой точек "что здесь"
        /// </summary>
        public GMapOverlay whatThereOverlay;

        /// <summary>
        /// начало маршрута
        /// </summary>
        public TrackPoint fromPoint;

        /// <summary>
        /// конец маршрута
        /// </summary>
        public TrackPoint toPoint;

        /// <summary>
        /// промежуточные точки при построении маршрутов
        /// </summary>
        public TrackFile IntermediatePoints;

        #endregion

        #region навигация, взаимодействие

        /// <summary>
        /// стек последних позиций при переходах по карте
        /// </summary>
        public Stack<KeyValuePair<string, Coordinate>> PositionsStack { get; set; }

        /// <summary>
        /// словарь с соответствиями точек и окон "что здесь"
        /// </summary>
        public Dictionary<TrackPoint, FormWhatsthere> ActiveWhatThereForms { get; set; }

        #endregion

        #region состояние карты

        /// <summary>
        /// состояние нажатия левой кнопки мыши. Для перетаскивания маркера
        /// </summary>
        public bool isLeftButtonDown = false;

        /// <summary>
        /// если истина, то значит, идет перемещаение маркера
        /// </summary>
        public bool isMarkerMoving;

        /// <summary>
        /// если истина, то последнее нажатие мыши было на маркере.
        /// Используется при создании маршрута при выделении маркера
        /// </summary>
        public bool isMarkerClicked;

        #endregion

        #region идентификаторы

        /// <summary>
        /// базовый слой
        /// </summary>
        public readonly string baseOverlayID = "baseOverlay";

        /// <summary>
        /// слой выделенного маршрут
        /// </summary>
        public readonly string selectedRouteOverlayID = "selectedRouteOverlay";

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        public readonly string creatingRouteOverlayID = "creatingRouteOverlay";

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        public readonly string creatingTripOverlayID = "creatingTripOverlay";

        /// <summary>
        /// линейка
        /// </summary>
        public readonly string rulerRouteOverlayID = "rulerRouteOverlay";

        /// <summary>
        /// слой выделенных точек через метод SelectPoint
        /// </summary>
        public readonly string selectedPointsOverlayID = "selectedPointsOverlay";

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        public readonly string fromToOverlayID = "fromToOverlay";

        /// <summary>
        /// слой маркеров "что здесь"
        /// </summary>
        public readonly string whatThereOverlayID = "whatThereOverlay";

        /// <summary>
        /// слой маркеров найденых результатов поиска
        /// </summary>
        public readonly string searchOverlayID = "searchOverlay";
        #endregion

        #region работа интерфейса

        /// <summary>
        /// стек последних изменений на карте
        /// </summary>
        public Stack<StackItem> LastEditsStack { get; set; }



        /// <summary>
        /// таймер перемещения карты
        /// </summary>
        public System.Timers.Timer moveMapTimer;

        /// <summary>
        /// таймер обновления списка поиска 
        /// </summary>
        public System.Timers.Timer refreshGoToTimer;

        /// <summary>
        /// последнее изменение текста в комбобоксе поиска адреса
        /// </summary>
        public DateTime lastCBGoToChanged = DateTime.Now;

        /// <summary>
        /// последний запрос в поиске мест
        /// </summary>
        public string lastGoToQuery = "";

        #endregion

        #endregion

        #region КОНВЕРТЕР

        /// <summary>
        /// список загруженных маршрутов в списке маршрутов
        /// </summary>
        internal TrackFileList Tracks;

        /// <summary>
        /// список маршрутов, выводимый на карте постоянной
        /// </summary>
        internal TrackFileList showingRoutesList;

        /// <summary>
        /// список путевый точек, выводимых на карте постоянно
        /// </summary>
        internal TrackFileList showingWaypointsList;


        /// <summary>
        /// открываемый файл при запуске приложения (для асинхронной загрузки файла)
        /// </summary>
        internal string openingFile = null;

        #endregion

        #region ТОЧКИ

        /// <summary>
        /// список загруженных точек
        /// </summary>
        internal BaseTrack Points;

        /// <summary>
        /// если истина, то есть несохраненные изменения
        /// </summary>
        internal bool isEditedPoints = false;

        #endregion

        #region ГРАФИК

        /// <summary>
        /// список выводящих треков, не включая выделенный трек
        /// </summary>
        public TrackFileList tracks;

        /// <summary>
        /// основные линии на графике, не включая линию выдленного трека
        /// </summary>
        public List<LineItem> mainCurves;

        /// <summary>
        /// линия высот выделенного трека
        /// </summary>
        public LineItem curveSelectedTrack;

        #endregion

        #endregion


        /// <summary>
        /// Счетчик одновременно запущенных операций.
        /// </summary>
        private int OperationCounter = 0;


        internal GraphHelper graphHelper;
        internal ConverterHelper converterHelper;
        internal PointsHelper pointsHelper;
        internal MapHelper mapHelper;
        internal MainHelper mainHelper;

        private FormMain()
        {
            InitializeComponent();
            graphHelper = new GraphHelper(this);
            converterHelper = new ConverterHelper(this);
            pointsHelper = new PointsHelper(this);
            mapHelper = new MapHelper(this);
            mainHelper = new MainHelper(this);

#if DEBUG
            this.button1.Visible = true;
#else
            this.button1.Visible = false;
#endif
        }

        /// <summary>
        /// создаёт новое окно с параметрами коандной строки (файлы для загрузки)
        /// </summary>
        /// <param name="args"></param>
        public FormMain(string[] args)
            : this()
        {
            this.SuspendLayout();
            #region СПИСОК МАРШРУТОВ

            Tracks = new TrackFileList();
            showingRoutesList = new TrackFileList();
            showingWaypointsList = new TrackFileList();
            mainHelper.RefreshRecentFiles(); //обновление писка последних загруженных файлов

            #endregion

            #region КАРТА


            //стек перехода по поиску мест
            this.PositionsStack = new Stack<KeyValuePair<string, Coordinate>>();

            //стек последних изменнений на карте
            this.LastEditsStack = new Stack<StackItem>();

            //для работы кнопок навигации
            this.KeyPreview = true;

            #region добавление поставщиков карт

            //добавление поставщиков карты в основное меню
            //добавление поставщиков карты в меню инструментов
            toolStripDropDownButtonMapProvider.DropDownItems.Clear();
            mapProviderToolStripMenuItem.DropDownItems.Clear();
            Dictionary<MapProviderClasses, ToolStripMenuItem> classes1 = new Dictionary<MapProviderClasses, ToolStripMenuItem>();
            Dictionary<MapProviderClasses, ToolStripMenuItem> classes2 = new Dictionary<MapProviderClasses, ToolStripMenuItem>();
            ToolStripMenuItem c1 = null, c2 = null;
            foreach (MapProviderRecord mpr in Vars.Options.Map.AllMapProviders)
            {
                ToolStripMenuItem it1 = new ToolStripMenuItem
                {
                    Text = mpr.Title
                };
                it1.Click += mapHelper.MpProvider_Click;
                it1.Tag = mpr;
                it1.Image = new Bitmap(Application.StartupPath + mpr.IconName);
                if (mpr.Enum == Vars.Options.Map.MapProvider.Enum)
                {
                    it1.Checked = true;
                    if (mpr.MapProviderClass != MapProviderClasses.None)
                        c1 = it1;
                }

                ToolStripMenuItem it2 = new ToolStripMenuItem
                {
                    Text = mpr.Title
                };
                it2.Click += mapHelper.MpProvider_Click;
                it2.Tag = mpr;
                it2.Image = new Bitmap(Application.StartupPath + mpr.IconName);
                if (mpr.Enum == Vars.Options.Map.MapProvider.Enum)
                {
                    it2.Checked = true;
                    if (mpr.MapProviderClass != MapProviderClasses.None)
                        c2 = it2;
                }

                if (classes1.ContainsKey(mpr.MapProviderClass))
                {
                    var button1 = classes1[mpr.MapProviderClass];
                    var button2 = classes2[mpr.MapProviderClass];
                    button1.DropDownItems.Add(it1);
                    button2.DropDownItems.Add(it2);
                }
                else
                {
                    if (mpr.MapProviderClass == MapProviderClasses.None)
                    {
                        toolStripDropDownButtonMapProvider.DropDownItems.Add(it1);
                        mapProviderToolStripMenuItem.DropDownItems.Add(it2);
                    }
                    else
                    {
                        ToolStripMenuItem button1 = new ToolStripMenuItem(MapProviderRecord.GetMapProviderClassName(mpr.MapProviderClass));
                        ToolStripMenuItem button2 = new ToolStripMenuItem(MapProviderRecord.GetMapProviderClassName(mpr.MapProviderClass));
                        button1.DropDownItems.Add(it1);
                        button2.DropDownItems.Add(it2);
                        classes1.Add(mpr.MapProviderClass, button1);
                        classes2.Add(mpr.MapProviderClass, button2);
                        toolStripDropDownButtonMapProvider.DropDownItems.Add(button1);
                        mapProviderToolStripMenuItem.DropDownItems.Add(button2);
                    }
                }

            }
            //выделяем родительские кнопки выбранных карт
            if (c1 != null && c2 != null)
            {
                (c1.OwnerItem as ToolStripMenuItem).Checked = true;
                (c2.OwnerItem as ToolStripMenuItem).Checked = true;
            }

            #endregion

            #region добавление поставщиков слоёв

            //добавление поставщиков карты в основное меню
            //добавление поставщиков карты в меню инструментов
            layerProviderToolStripMenuItem.DropDownItems.Clear();
            Dictionary<MapLayerProvidersClasses, ToolStripMenuItem> classes3 = new Dictionary<MapLayerProvidersClasses, ToolStripMenuItem>();
            ToolStripMenuItem c3 = null;
            foreach (MapLayerProviderRecord mpr in Vars.Options.Map.AllLayerProviders)
            {
                ToolStripMenuItem it1 = new ToolStripMenuItem
                {
                    Text = mpr.Title
                };
                it1.Click += mapHelper.LrProvider_Click;
                it1.Tag = mpr;
                it1.Image = new Bitmap(Application.StartupPath + mpr.IconName);
                if (mpr.Enum == Vars.Options.Map.LayerProvider.Enum)
                {
                    it1.Checked = true;
                    if (mpr.MapLayerProviderClass != MapLayerProvidersClasses.None)
                        c3 = it1;
                }

                

                if (classes3.ContainsKey(mpr.MapLayerProviderClass))
                {
                    var button1 = classes3[mpr.MapLayerProviderClass];
                    button1.DropDownItems.Add(it1);
                }
                else
                {
                    if (mpr.MapLayerProviderClass == MapLayerProvidersClasses.None)
                    {
                        layerProviderToolStripMenuItem.DropDownItems.Add(it1);
                    }
                    else
                    {
                        ToolStripMenuItem button1 = new ToolStripMenuItem(MapLayerProviderRecord.GetMapLayerProviderClassName(mpr.MapLayerProviderClass));
                        button1.DropDownItems.Add(it1);
                        classes3.Add(mpr.MapLayerProviderClass, button1);
                        layerProviderToolStripMenuItem.DropDownItems.Add(button1);
                    }
                }

            }
            //выделяем родительские кнопки выбранных слоёв
            if (c3 != null)
            {
                (c3.OwnerItem as ToolStripMenuItem).Checked = true;
            }

            #endregion
            

            #endregion

            #region ТОЧКИ

            if (waypoints == null)
                waypoints = new TrackFile();

            this.Points = waypoints;
            pointsHelper.FillDGV(Points.Source);

            #endregion

            #region ГРАФИКИ

            Task pr = new Task(new Action(() =>
            {
                BeginOperation();
                SetCurrentOperation("Построение профиля...");
                graphHelper.ConfigureGraph();
                EndOperation();
            }));
            pr.Start();

            #endregion

            //если есть аргументы командной строки 
            if (args.Length > 0)
            {
                //загрузка файлов из параметров
                foreach (string arg in args)
                {
                    converterHelper.OpenFile(arg);
                }
            }
            this.ResumeLayout();
        }

        /// <summary>
        /// настройка внешнего вида элементов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formMain_Shown(object sender, EventArgs e)
        {
            this.SuspendLayout();
            #region ВНЕШНИЙ ВИД ОКНА

            splitContainerHorizontalLeft.SplitterDistance = Vars.Options.Container.HorizontalLeftSplitter;
            splitContainerHorizontalRight.SplitterDistance = Vars.Options.Container.HorizontalRightSplitter;
            splitContainerVertical.SplitterDistance = Vars.Options.Container.VerticalSplitter;
            toolStripLabelCurrentOperation.Text = "";
            toolStripLabelFromStart.Text = "";
            toolStripLabelInfo.Text = "";
            toolStripLabelPosition.Text = "";

            #endregion

            #region КАРТА


            mapHelper.ConfigureGMapControl();

            TripRouteFile gf = null;
            try
            {
                if (Vars.Options.Map.RestoreRoutesWaypoints && File.Exists(Application.StartupPath + Resources.saveLast_file))
                    gf = Serializer.DeserializeTripRouteFile(Application.StartupPath + Resources.saveLast_file);
            }
            catch (Exception ex)
            {
                Vars.Options.Map.RestoreRoutesWaypoints = false;
                MessageBox.Show("Произошла ошибка при восстановлении точек или маршрутов.\r\n" + ex.Message + "\r\nВосстановление отключено.");
            }

            //обновление списка отображаемых маршрутов
            if (Tracks != null)
                mapHelper.RefreshData();
            //загрузка сохраненных маршрутов с послденего запуска
            else
             if (gf != null)
                converterHelper.AddRouteToList(gf.DaysRoutes);

            //обновление путевых точек
            if (waypoints != null)
                mapHelper.ShowWaypoints(waypoints, baseOverlay, false);

            //загрузка точек с последнего запуска
            else if (gf != null)
                mapHelper.ShowWaypoints(gf.Waypoints, baseOverlay, false, true);

            //последняя выделенная область карты
            gmapControlMap.SelectedArea = Vars.Options.Map.LastSelectedArea;

            gmapControlMap.RefreshLayers();

            #endregion

            #region КОНВЕРТЕР

            //открытие последних файлов
            if (this.Tracks == null || this.Tracks.Count == 0)
            {
                this.Tracks = Serializer.DeserializeTrackFileList(Vars.Options.Converter.LastLoadedTracks);
                converterHelper.RefreshData();
            }


            if (this.Tracks.Count != 0)
            {
                Vars.currentSelectedTrack = Tracks[0];
                mapHelper.RefreshData();
                pointsHelper.RefreshData();
                graphHelper.RefreshData();
            }

            //завершение открытия файлов, при запуске программы
            if (openingFile != null)
            {
                Task ts = new Task(new Action(() =>
                {
                    BaseTrack pts = null;
                    Program.winMain.BeginOperation();
                    //метод обновления информации о выполняемой операции

                    pts = Serializer.DeserializeTrackFile(openingFile, Program.winMain.SetCurrentOperation);
                    openingFile = null;
                    //обработка результатов
                    this.Invoke(new Action(() =>
                    {
                        EndOperation();
                        mapHelper.Clear();
                        mapHelper.ShowWaypoints(pts, false, false);
                    }));
                }));
                ts.Start();
            }

            #endregion

            #region ТОЧКИ



            #endregion

            #region ГРАФИКИ



            #endregion

            //если надо - запускаем загрузку ЕТОРО2
            if (Vars.Options.Common.IsLoadETOPOOnStart)
                if (GeoInfo.ETOPOProvider == null)
                {
                    BeginOperation();
                    Vars.TaskLoadingETOPO.Start();
                }
            this.ResumeLayout(true);
        }


        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = TryClose(sender, e);
            if (!e.Cancel)
            {
                Vars.Options.Map.LastCenterPoint = gmapControlMap.Position;
                Vars.Options.Map.IsFormNavigatorShow = !Program.winNavigatorNullOrDisposed;
                Vars.Options.Map.LastSelectedArea = gmapControlMap.SelectedArea;
            }

            Vars.Options.Container.WinSize = this.Size;
            Vars.Options.Container.WinState = this.WindowState;
            Vars.Options.Container.WinPosition = new Point(Left, Top);
            Vars.Options.Converter.LastLoadedTracks = this.Tracks.FilePaths;

            //размеры окон
            Vars.Options.Container.VerticalSplitter = splitContainerVertical.SplitterDistance;
            Vars.Options.Container.HorizontalLeftSplitter = splitContainerHorizontalLeft.SplitterDistance;
            Vars.Options.Container.HorizontalRightSplitter = splitContainerHorizontalRight.SplitterDistance;
        }


        #region Управление состоянием статус бар панели внизу экрана


        /// <summary>
        /// окончание выполнения операции (сброс надписей, курсора)
        /// </summary>
        internal void EndOperation()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    this.Cursor = Cursors.Arrow;
                    toolStripLabelCurrentOperation.Text = "";
                    OperationCounter--;
                    if (OperationCounter == 0)
                        toolStripLabelCurrentOperation.Visible = false;
                }));

            else
            {
                this.Cursor = Cursors.Arrow;
                toolStripLabelCurrentOperation.Text = "";
                OperationCounter--;
                if (OperationCounter == 0)
                    toolStripLabelCurrentOperation.Visible = false;
            }
        }

        /// <summary>
        /// начало выполнения асинхронной операции (установка надписей, курсора)
        /// </summary>
        internal void BeginOperation()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripLabelCurrentOperation.Visible = true;
                    this.Cursor = Cursors.AppStarting;
                    OperationCounter++;
                }));

            else
            {
                toolStripLabelCurrentOperation.Visible = true;
                this.Cursor = Cursors.AppStarting;
                OperationCounter++;
            }
        }

        /// <summary>
        /// установка надписи строки состояния операции внизу экрана
        /// </summary>
        /// <param name="obj"></param>
        internal void SetCurrentOperation(string obj)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripLabelCurrentOperation.Text = obj;
                }));

            else
            {
                toolStripLabelCurrentOperation.Text = obj;
            }
        }

        #endregion

        #region События карты

        #region GMapControl
        private void gmapControlMap_MouseClick(object sender, MouseEventArgs e)
        {
            mapHelper.MouseClick(e);
        }

        private void gmapControlMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            mapHelper.MouseDoubleClick(e);
        }

        private void gmapControlMap_MouseDown(object sender, MouseEventArgs e)
        {
            mapHelper.MouseDown(e);
        }

        private void gmapControlMap_MouseMove(object sender, MouseEventArgs e)
        {
            mapHelper.MouseMove(e);
        }

        private void gmapControlMap_MouseUp(object sender, MouseEventArgs e)
        {
            mapHelper.MouseUp(e);
        }

        private void gmapControlMap_OnMapDrag()
        {
            mapHelper.OnMapDrag();
        }

        private void gmapControlMap_OnMapTypeChanged(GMapProvider type)
        {
            mapHelper.MapTypeChanged(type);
        }

        private void gmapControlMap_OnMarkerLeave(GMapMarker item)
        {
            mapHelper.OnMarkerLeave(item);
        }

        private void gmapControlMap_OnMarkerEnter(GMapMarker item)
        {
            mapHelper.OnMarkerEnter(item);
        }

        private void gmapControlMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            mapHelper.OnMarkerClick(item, e);
        }

        private void gmapControlMap_OnMapZoomChanged()
        {
            mapHelper.OnMapZoomChanged();
        }

        private void gmapControlMap_OnPositionChanged(PointLatLng point)
        {
            mapHelper.OnPositionChanged(point);
        }

        private void gmapControlMap_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            mapHelper.OnRouteClick(item, e);

        }
        #endregion

        #region Кнопки 
        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            mapHelper.UndoClick(e);
        }

        /// <summary>
        /// Кнопка поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonFind_Click(object sender, EventArgs e)
        {
            mapHelper.ButtonFindClick(sender, e);
        }

        /// <summary>
        /// Удалить найденные объекты с карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonClearSearchMarks_Click(object sender, EventArgs e)
        {
            mapHelper.ButtonClearSearchMarks(sender, e);
        }

        /// <summary>
        /// Нажатие кнопки Enter  в поле поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            mapHelper.BoxSearchKeyDown(sender, e);
        }

        /// <summary>
        /// добавление элементов последних запросов в список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxSearch_DropDown(object sender, EventArgs e)
        {
            mapHelper.ToolStripComboBoxSearch_DropDown(sender, e);
        }

        /// <summary>
        /// показать местоположение устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonLocateDevice_Click(object sender, EventArgs e)
        {
            mapHelper.ToolstripButtonLocateDevice(sender, e);
        }

        /// <summary>
        /// поиск при выборе из последних результатов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            mapHelper.ButtonFindClick(sender, e);
        }

        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            mapHelper.ZoomIn_Click(e);
        }

        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            mapHelper.ZoomOutClick(e);
        }

        private void toolStripButtonRuler_Click(object sender, EventArgs e)
        {
            mapHelper.RulerClick(e);
        }

        #endregion

        #region контекстные меню карты


        private void contextMenuStripMap_Opening(object sender, CancelEventArgs e)
        {
            mapHelper.ContextMenuMapOpening(sender, e);
        }

        private void contextMenuStripMarker_Opening(object sender, CancelEventArgs e)
        {
            mapHelper.ContextMenuMarkerOpening(sender, e);
        }

        private void toolStripMenuItemAddWaypoint_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripAddWaypoint(e);
        }

        private void fromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripCreateRoute(sender, e);
        }

        private void intermediatePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripCreateRoute(sender, e);
        }

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripCreateRoute(sender, e);
        }

        private void clearFromtoMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripClearFromToMarkers(e);
        }

        private void toolStripMenuItemWhatsThere_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripWhatThere(e);
        }

        private void copyCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripCopyCoordinates(e);
        }

        private void editMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripEditMarker(e);
        }

        private void deleteMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripDeleteMarker(e);
        }

        private void editRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripEditRoute(e);
        }

        private void removeRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.ToolStripDeleteRoute(e);
        }


        #endregion

        #endregion

        #region События конвертера

        #region Контекстное меню списка маршрутов

        private void contextMenuStripConverter_Opening(object sender, CancelEventArgs e)
        {
            converterHelper.ContextMenuListOpening(sender, e);
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripInformation(e);
        }

        private void saveFileContextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripSaveAsFile(sender, e);
        }

        private void saveYandexContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripSaveAsYandex(sender, e);
        }

        private void saveWikimapiaContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripSaveAsWikimapia(sender, e);
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripSave(sender, e);
        }

        private void editRouteMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripEditRoute(e);
        }

        private void editWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripEditWaypoints(e);
        }

        private void separateRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripSeparateRoute(sender, e);
        }

        private void loadElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripLoadElevations(e);
        }

        /// <summary>
        /// загрузить адреса точек в описания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripLoadAddresses(e);
        }

        /// <summary>
        /// на основе точек маршрута построить оптимальный маршрут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createOptimalOnBaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripCreateOptimalOnBase(sender, e);
        }

        private void removeElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripRemoveElevations(e);
        }

        private void approximateAltitudesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripApproximateAltitudes(e);
        }

        private void normalizeTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripNormalizeTrack(e);
        }

        private void toTripRouteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripConvertToTripRoute(e);
        }

        private void joinToTripRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripJoinToTripRoute(e);
        }

        private void showWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripShowWaypoints(e);
        }

        private void showOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripShowRouteOnMap(e);
        }

        private void elevgraphWithIntermediatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripElevGraphWithIntermediates(e);
        }

        private void elevgraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripElevGraph(e);
        }

        private void toolStripMenuItemShowElevGraphOnRoute_Paint(object sender, PaintEventArgs e)
        {
            mainHelper.ToolStripShowElevGraphOnRoute_Paint(sender, e);
        }

        private void openRouteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripOpenRouteFolder(e);
        }

        private void addToJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripAddToJoin(e);
        }

        private void addComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripAddToComparison(e);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.ToolStripRemove(e);
        }

        #endregion

        #region События списка марщрутов

        private void dataGridViewConverter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            converterHelper.DataGridViewCellClick(e);
        }

        private void dataGridViewConverter_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            converterHelper.DataGridViewCellFormatting(e);
        }

        private void dataGridViewConverter_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            converterHelper.DataGridViewCellMouseDoubleClick(sender, e);
        }

        private void dataGridViewConverter_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            converterHelper.DataGridViewCellMouseDown(e);
        }

        private void dataGridViewConverter_DragDrop(object sender, DragEventArgs e)
        {
            converterHelper.DataGridViewDragDrop(e);
        }

        private void dataGridViewConverter_DragEnter(object sender, DragEventArgs e)
        {
            converterHelper.DataGridViewDragEnter(e);
        }

        private void dataGridViewConverter_KeyDown(object sender, KeyEventArgs e)
        {
            converterHelper.DataGridViewKeyDown(e);
        }

        private void dataGridViewConverter_Paint(object sender, PaintEventArgs e)
        {
            converterHelper.DataGridViewPaint(e);
        }

        private void dataGridViewConverter_SelectionChanged(object sender, EventArgs e)
        {
            converterHelper.DataGridViewSelectionChanged(e);
        }


        #endregion

        #endregion

        #region Главное меню

        private void saveFileWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSaveFileWaypoints(sender, e);
        }

        private void saveFileWaypointsRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSaveFileWaypointsRoutes(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripOpen(sender, e);
        }

        private void createRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripCreateRoute(sender, e);
        }

        private void createTripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripCreateTrip(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripExit(sender, e);
        }

        private void tmInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSourceInternet(sender, e);
        }

        private void tmCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSourceCache(sender, e);
        }

        private void tmInternetCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSourceInternetCache(sender, e);
        }

        private void selectMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripSelectMap(sender, e);
        }

        private void clearRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripClearRoutes(sender, e);
        }

        private void clearMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripClearMarkers(sender, e);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripClearAll(sender, e);
        }

        private void editPointFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripEditPointFile(sender, e);
        }

        private void calculateDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripCalculateDistance(sender, e);
        }

        private void transformCoordinateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripTransformCoordinate(sender, e);
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripConsole(sender, e);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripOptions(sender, e);
        }

        private void showNavigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripShowNavigator(sender, e);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripHelp(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripAbout(sender, e);
        }

        private void toolStripMenuItemEditWaypoints_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripEditWaypoints(sender, e);
        }

        private void toolStripMenuItemShowElevGraphOnRoute_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripShowElevGraphOnRoute(sender, e);
        }

        private void toolStripMenuItemcreateOptimalRoute_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripCreateOptimalRoute(sender, e);
        }

        private void toolStripMenuItemPointsToRoute_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripPointsToRoute(sender, e);
        }

        private void toolStripMenuItemRouteToPoints_Click(object sender, EventArgs e)
        {
            mainHelper.ToolStripRouteToPoints(sender, e);
        }

        #endregion

        #region События списка точек

        #region Контекстное меню списка

        private void addPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripAddPoint(sender, e);
        }

        private void editPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripEditPoint(sender, e);
        }

        private void openWithConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripOpenWithConverter(sender, e);
        }

        private void showPointOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripShowPointOnMap(sender, e);
        }

        private void showYandexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripShowYandex(sender, e);
        }

        private void showGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripShowGoogle(sender, e);
        }

        private void removePointtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            pointsHelper.toolStripRemovePoint(sender, e);
        }



        #endregion

        #region События списка точек

        private void dataGridViewPoints_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pointsHelper.dataGridCellClick(sender, e);
        }

        private void dataGridViewPoints_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            pointsHelper.dataGridCellDoubleClick(sender, e);
        }

        private void dataGridViewPoints_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            pointsHelper.dataGridCellMouseDown(sender, e);
        }

        private void dataGridViewPoints_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            pointsHelper.dataGridCellValidating(sender, e);
        }

        private void dataGridViewPoints_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            pointsHelper.dataGridCellValueChanged(sender, e);
        }

        private void dataGridViewPoints_KeyDown(object sender, KeyEventArgs e)
        {
            pointsHelper.dataGridKeyDown(sender, e);
        }

        private void dataGridViewPoints_SelectionChanged(object sender, EventArgs e)
        {
            pointsHelper.dataGridSelectionChanged(sender, e);
        }



        #endregion

        #endregion

        #region События графиков

        private void zedGraph_MouseLeave(object sender, EventArgs e)
        {
            graphHelper.MouseLeave(sender, e);
        }

        private bool zedGraph_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            return graphHelper.MoseMoveEvent(sender, e);
        }

        private string zedGraph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            return graphHelper.PointValueEvent(sender, pane, curve, iPt);
        }

        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            
            Wikimapia eng = new Wikimapia(Application.StartupPath+Resources.cache_directory+"\\tmp");
            var tls = gmapControlMap.GetViewArea();

            for (int i = 0; i < 500; i++)
                eng.GetObjects(tls, 1);
        }


    }
}

