﻿using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.StackEdits;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Map;
using ZedGraph;

namespace TrackConverter.UI.Shell
{
    public partial class FormMain : Form
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
        /// список загруженных маршрутов
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
        /// если истина, то при закрытии окна будет вызван метод map.EndEditWaypoints
        /// </summary>
        internal Action<BaseTrack> endEditWaypointsAction;

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
        }

        public FormMain(string[] args)
            : this()
        {
            //если есть аргументы командной строки 
            if (args.Length > 0)
            {

            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //если надо - запускаем загрузку ЕТОРО2
            if (Vars.Options.Common.IsLoadETOPOOnStart)
                if (GeoInfo.ETOPOProvider == null)
                {
                    BeginOperation();
                    Vars.TaskLoadingETOPO.Start();
                }

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
            gmapControlMap.Refresh();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
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
                    foreach (var f in this.MdiChildren)
                        f.Cursor = Cursors.Arrow;
                    OperationCounter--;
                    if (OperationCounter == 0)
                        toolStripStatusLabelCurrentOperation.Visible = false;
                }));

            else
            {
                this.Cursor = Cursors.Arrow;
                foreach (var f in this.MdiChildren)
                    f.Cursor = Cursors.Arrow;
                OperationCounter--;
                if (OperationCounter == 0)
                    toolStripStatusLabelCurrentOperation.Visible = false;
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
                    toolStripStatusLabelCurrentOperation.Visible = true;
                    this.Cursor = Cursors.AppStarting;
                    foreach (var f in this.MdiChildren)
                        f.Cursor = Cursors.AppStarting;
                    OperationCounter++;
                }));

            else
            {
                toolStripStatusLabelCurrentOperation.Visible = true;
                this.Cursor = Cursors.AppStarting;
                foreach (var f in this.MdiChildren)
                    f.Cursor = Cursors.AppStarting;
                OperationCounter++;
            }
        }

        /// <summary>
        /// установка надписи строки состояния операции внизу экрана
        /// </summary>
        /// <param name="obj"></param>
        internal void setCurrentOperation(string obj)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    toolStripStatusLabelCurrentOperation.Text = obj;
                }));

            else
            {
                toolStripStatusLabelCurrentOperation.Text = obj;
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

        private void toolStripMenuItemAddWaypoint_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripAddWaypoint(e);
        }

        private void fromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripCreateRoute(sender, e);
        }

        private void intermediatePointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripCreateRoute(sender, e);
        }

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripCreateRoute(sender, e);
        }

        private void clearFromtoMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripClearFromToMarkers(e);
        }

        private void toolStripMenuItemWhatsThere_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripWhatThere(e);
        }

        private void copyCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripCopyCoordinates(e);
        }

        private void editMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripEditMarker(e);
        }

        private void deleteMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripDeleteMarker(e);
        }

        private void editRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripEditRoute(e);
        }

        private void removeRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mapHelper.toolStripDeleteRoute(e);
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
            converterHelper.toolStripInformation(e);
        }

        private void saveFileContextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripSaveAsFile(sender, e);
        }

        private void saveYandexContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripSaveAsYandex(sender, e);
        }

        private void saveWikimapiaContextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripSaveAsWikimapia(sender, e);
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripSave(sender, e);
        }

        private void editRouteMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripEditRoute(e);
        }

        private void editWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripEditWaypoints(e);
        }

        private void loadElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripLoadElevations(e);
        }

        private void removeElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripRemoveElevations(e);
        }

        private void approximateAltitudesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripApproximateAltitudes(e);
        }

        private void normalizeTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripNormalizeTrack(e);
        }

        private void toTripRouteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripConvertToTripRoute(e);
        }

        private void joinToTripRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripJoinToTripRoute(e);
        }

        private void showWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripShowWaypoints(e);
        }

        private void showOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripShowRouteOnMap(e);
        }

        private void elevgraphWithIntermediatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripElevGraphWithIntermediates(e);
        }

        private void elevgraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripElevGraph(e);
        }

        private void toolStripMenuItemShowElevGraphOnRoute_Paint(object sender, PaintEventArgs e)
        {
            mainHelper.toolStripShowElevGraphOnRoute_Paint(sender, e);
        }

        private void openRouteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripOpenRouteFolder(e);
        }

        private void addToJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripAddToJoin(e);
        }

        private void addComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripAddToComparison(e);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            converterHelper.toolStripRemove(e);
        }

        #endregion

        #region События списка марщрутов

        private void dataGridViewConverter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            converterHelper.dataGridViewCellClick(e);
        }

        private void dataGridViewConverter_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            converterHelper.dataGridViewCellFormatting(e);
        }

        private void dataGridViewConverter_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            converterHelper.dataGridViewCellMouseDoubleClick(sender, e);
        }

        private void dataGridViewConverter_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            converterHelper.dataGridViewCellMouseDown(e);
        }

        private void dataGridViewConverter_DragDrop(object sender, DragEventArgs e)
        {
            converterHelper.dataGridViewDragDrop(e);
        }

        private void dataGridViewConverter_DragEnter(object sender, DragEventArgs e)
        {
            converterHelper.dataGridViewDragEnter(e);
        }

        private void dataGridViewConverter_KeyDown(object sender, KeyEventArgs e)
        {
            converterHelper.dataGridViewKeyDown(e);
        }

        private void dataGridViewConverter_Paint(object sender, PaintEventArgs e)
        {
            converterHelper.dataGridViewPaint(e);
        }

        private void dataGridViewConverter_SelectionChanged(object sender, EventArgs e)
        {
            converterHelper.dataGridViewSelectionChanged(e);
        }


        #endregion

        #endregion

        #region Главное меню

        private void saveFileWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSaveFileWaypoints(sender, e);
        }

        private void saveFileWaypointsRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSaveFileWaypointsRoutes(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripOpen(sender, e);
        }

        private void createRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripCreateRoute(sender, e);
        }

        private void createTripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripCreateTrip(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripExit(sender, e);
        }

        private void tmInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSourceInternet(sender, e);
        }

        private void tmCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSourceCache(sender, e);
        }

        private void tmInternetCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSourceInternetCache(sender, e);
        }

        private void selectMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripSelectMap(sender, e);
        }

        private void clearRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripClearRoutes(sender, e);
        }

        private void clearMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripClearMarkers(sender, e);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripClearAll(sender, e);
        }

        private void EditPointFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripEditPointFile(sender, e);
        }

        private void CalculateDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripCalculateDistance(sender, e);
        }

        private void TransformCoordinateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripTransformCoordinate(sender, e);
        }

        private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripConsole(sender, e);
        }

        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripOptions(sender, e);
        }

        private void showNavigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripShowNavigator(sender, e);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripHelp(sender, e);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripAbout(sender, e);
        }

        private void toolStripMenuItemEditWaypoints_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripEditWaypoints(sender, e);
        }

        private void toolStripMenuItemShowElevGraphOnRoute_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripShowElevGraphOnRoute(sender, e);
        }

        private void toolStripMenuItemcreateOptimalRoute_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripCreateOptimalRoute(sender, e);
        }

        private void toolStripMenuItemPointsToRoute_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripPointsToRoute(sender, e);
        }

        private void toolStripMenuItemRouteToPoints_Click(object sender, EventArgs e)
        {
            mainHelper.toolStripRouteToPoints(sender, e);
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

        private void RemovePointtoolStripMenuItem_Click(object sender, EventArgs e)
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

    }
}
