using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.StackEdits;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Mathematic.Routing;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Tools;
using static TrackConverter.Lib.Classes.StackEdits.Actions;

namespace TrackConverter.UI.Map
{

    /// <summary>
    /// окно карты
    /// </summary>
    public partial class FormMap : Form
    {

        #region public поля

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

        #endregion

        #region карта, слои, маршруты

        /// <summary>
        /// список загруженных треков
        /// </summary>
        public TrackFileList tracks;

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
        /// измерение расстояния
        /// </summary>
        public TrackFile rulerRoute;

        #endregion

        #region навигация, взаимодействие

        /// <summary>
        /// стек последних позиций при переходах по карте
        /// </summary>
        public Stack<KeyValuePair<string, Coordinate>> PositionsStack { get; set; }

        #endregion

        #endregion

        #region private поля

        #region карта, слои, маршруты

        /// <summary>
        /// текущий выбранный маркер, на котором находится мышь
        /// </summary>
        private MapMarker currentMarker;

        /// <summary>
        /// точка, на которой произошло нажатие (для вывода ContextMenu на пустой карте)
        /// </summary>
        private PointLatLng pointClicked;

        /// <summary>
        /// маркер, на котором произошло нажатие(для вывода контекстного меню маркера)
        /// </summary>
        private MapMarker markerClicked;

        /// <summary>
        /// слой выделенной точки
        /// </summary>
        private GMapOverlay selectedPointsOverlay;

        /// <summary>
        /// слой, выделенного в списке маршрутов, маршрута
        /// </summary>
        private GMapOverlay selectedRouteOverlay;

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        private GMapOverlay fromToOverlay;

        /// <summary>
        /// начало маршрута
        /// </summary>
        private TrackPoint fromPoint;

        /// <summary>
        /// конец маршрута
        /// </summary>
        private TrackPoint toPoint;

        /// <summary>
        /// промежуточные точки при построении маршрутов
        /// </summary>
        private TrackFile IntermediatePoints;

        #endregion

        #region состояние карты

        /// <summary>
        /// состояние нажатия левой кнопки мыши. Для перетаскивания маркера
        /// </summary>
        private bool isLeftButtonDown = false;

        /// <summary>
        /// если истина, то значит, идет перемещаение маркера
        /// </summary>
        private bool isMarkerMoving;

        /// <summary>
        /// если истина, то последнее нажатие мыши было на маркере.
        /// Используется при создании маршрута при выделении маркера
        /// </summary>
        private bool isMarkerClicked;

        #endregion

        #region флаги, передача параметров

        /// <summary>
        /// координаты маркера до перемещения. (для отмены действия)
        /// </summary>
        private TrackPoint oldMarker = null;

        #endregion

        #region идентификаторы

        /// <summary>
        /// базовый слой
        /// </summary>
        private readonly string baseOverlayID = "baseOverlay";

        /// <summary>
        /// слой выделенного маршрут
        /// </summary>
        private readonly string selectedRouteOverlayID = "selectedRouteOverlay";

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        private readonly string creatingRouteOverlayID = "creatingRouteOverlay";

        /// <summary>
        /// линейка
        /// </summary>
        private readonly string rulerRouteOverlayID = "rulerRouteOverlay";

        /// <summary>
        /// слой выделенных точек через метод SelectPoint
        /// </summary>
        private readonly string selectedPointsOverlayID = "selectedPointsOverlay";

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        private readonly string fromToOverlayID = "fromToOverlay";

        #endregion

        #region работа интерфейса

        /// <summary>
        /// стек последних изменений на карте
        /// </summary>
        private Stack<StackItem> LastEditsStack { get; set; }

        /// <summary>
        /// таймер перемещения карты
        /// </summary>
        private System.Timers.Timer moveMapTimer;

        /// <summary>
        /// таймер обновления списка поиска 
        /// </summary>
        private System.Timers.Timer refreshGoToTimer;

        /// <summary>
        /// последнее изменение текста в комбобоксе поиска адреса
        /// </summary>
        private DateTime lastCBGoToChanged = DateTime.Now;
        private string lastGoToQuery = "";

        #endregion

        #endregion


        #region Конструкторы

        /// <summary>
        /// создает новое окно
        /// </summary>
        public FormMap()
            : this(null, null) { }

        /// <summary>
        /// создает новое окно с заданным списком треков
        /// </summary>
        /// <param name="tracksList">список треков</param>
        public FormMap(TrackFileList tracksList)
            : this(null, tracksList) { }

        /// <summary>
        /// создает новое окно с заданным списком путевых точек
        /// </summary>
        /// <param name="waypointsList">список путевых точек</param>
        public FormMap(TrackFile waypointsList)
            : this(waypointsList, null) { }

        /// <summary>
        /// создает новое окно карты с заданным списком трекови путевых точек
        /// </summary>
        /// <param name="tracksList">список треков для вывода</param>
        /// <param name="waypointsList">списк путевых точек</param>
        public FormMap(TrackFile waypointsList, TrackFileList tracksList)
        {
            InitializeComponent();
            ConfigureGMapControl();



            //запуск таймеров передвижения карты и обновления списка результатов поиск
            moveMapTimer = new System.Timers.Timer(30);
            moveMapTimer.Elapsed += moveMapTimer_Elapsed;
            moveMapTimer.AutoReset = true;
            moveMapTimer.Start();

            refreshGoToTimer = new System.Timers.Timer(100);
            refreshGoToTimer.Elapsed += refreshGoToTimer_Elapsed;
            refreshGoToTimer.AutoReset = true;
            refreshGoToTimer.Start();

            //список отображаемых маршрутов
            this.tracks = new TrackFileList();

            //стек перехода по поиску мест
            this.PositionsStack = new Stack<KeyValuePair<string, Coordinate>>();

            //стек последних изменнений на карте
            this.LastEditsStack = new Stack<StackItem>();

            //для работы кнопок навигации
            this.KeyPreview = true;

            //добавление поставщиков карты в основное меню
            //добавление поставщиков карты в меню инструментов
            toolStripDropDownButtonMapProvider.DropDownItems.Clear();
            mapProviderToolStripMenuItem.DropDownItems.Clear();
            foreach (MapProviderRecord mpr in Vars.Options.Map.AllProviders)
            {
                ToolStripMenuItem it1 = new ToolStripMenuItem();
                it1.Text = mpr.Title;
                it1.Click += mpProviderToolStripMenuItem_Click;
                it1.Tag = mpr;
                it1.Image = new Bitmap(Application.StartupPath + mpr.IconName);
                if (mpr.Enum == Vars.Options.Map.MapProvider.Enum)
                    it1.Checked = true;

                ToolStripMenuItem it2 = new ToolStripMenuItem();
                it2.Text = mpr.Title;
                it2.Click += mpProviderToolStripMenuItem_Click;
                it2.Tag = mpr;
                it2.Image = new Bitmap(Application.StartupPath + mpr.IconName);
                if (mpr.Enum == Vars.Options.Map.MapProvider.Enum)
                    it2.Checked = true;

                toolStripDropDownButtonMapProvider.DropDownItems.Add(it1);
                mapProviderToolStripMenuItem.DropDownItems.Add(it2);
            }


            GeoFile gf = null;
            if (Vars.Options.Map.RestoreRoutesWaypoints && File.Exists(Application.StartupPath + Resources.saveLast_file))
                gf = Serializer.DeserializeGeoFile(Application.StartupPath + Resources.saveLast_file);

            //обновление списка отображаемых маршрутов
            if (tracksList != null)
                ShowTracks(tracksList, true);
            //загрузка сохраненных маршрутов с послденего запуска
            else
             if (gf != null)
                ShowTracks(gf.Routes, true);

            //обновление путевых точек
            if (waypointsList != null)
                ShowWaypoints(waypointsList, baseOverlay, false);
            //загрузка точек с последнего запкска
            else if (gf != null)
                ShowWaypoints(gf.Waypoints, baseOverlay, false);

            //размеры окна
            this.Size = Vars.Options.Map.WinSize;
        }


        #endregion


        #region события

        #region главное меню

        #region выделить

        /// <summary>
        /// прямоугольник
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selParallelogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gmapControlMap.DisableAltForSelection = true;
        }

        /// <summary>
        /// путь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// многоугольник
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Окно

        /// <summary>
        /// показать панель навигации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showNavigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winNavigatorNullOrDisposed)
                Program.winNavigator = new FormMapNavigator();
            if (!Program.winNavigator.Visible)
                Program.winNavigator.Show(this);
            Program.winNavigator.Activate();
        }
        #endregion

        #region Файл

        /// <summary>
        /// сохранение путевых точек в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
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
            sf.FileName = Path.GetFileNameWithoutExtension(waypoints.FileName);



            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (sf.FilterIndex)
                {
                    case 1:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.Rt2File);
                        break;
                    case 2:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.PltFile);
                        break;
                    case 3:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.WptFile);
                        break;
                    case 4:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.CrdFile);
                        break;
                    case 5:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.KmlFile);
                        break;
                    case 6:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.GpxFile);
                        break;
                    case 7:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.KmzFile);
                        break;
                    case 8:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.OsmFile);
                        break;
                    case 9:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.NmeaFile);
                        break;
                    case 10:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.CsvFile);
                        break;
                    case 11:
                        Serializer.Serialize(sf.FileName, waypoints, FileFormats.TxtFile);
                        break;
                    case 12:
                        Program.winMain.BeginOperation();
                        Action act = new Action(() =>
                        {
                            Serializer.Serialize(sf.FileName, waypoints, FileFormats.AddressList, Program.winMain.setCurrentOperation);
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
        /// сохранение маршрутов и точек в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileWaypointsRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter += "Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";

            sf.AddExtension = true;

            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;

            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (sf.FilterIndex)
                {
                    case 1:
                        GeoFile gf = new GeoFile(Program.winConverter.Tracks, waypoints);
                        Serializer.Serialize(sf.FileName, gf, FileFormats.KmlFile);
                        break;
                    case 2:
                        gf = new GeoFile(tracks, waypoints);
                        Serializer.Serialize(sf.FileName, gf, FileFormats.KmzFile);
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                MessageBox.Show("Файл сохранен!");
            }
        }

        /// <summary>
        /// открытие файла с точками и маршрутами 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileWaypointsRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter += "Файл координат(*.kml)|*.kml";
            of.Filter += "|Файл координат(*.kmz)|*.kmz";

            if (Vars.Options.Common.IsSaveDir)
                of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;

            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GeoFile gf = new GeoFile(tracks, waypoints);
                gf = Serializer.DeserializeGeoFile(of.FileName);
                this.waypoints = gf.Waypoints;
                this.tracks = gf.Routes;
                Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(of.FileName);
                ShowWaypoints(waypoints, baseOverlay, false);

                foreach (TrackFile tf in gf.Routes)
                    Program.winConverter.AddRouteToList(tf);
            }
        }

        /// <summary>
        /// создание маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            creatingRoute = new TrackFile();

            BeginEditRoute(creatingRoute, (tf) =>
            {
            //ввод названия марщрута
            readName:
                FormReadText fr = new FormReadText(DialogType.ReadText, "Введите название маршрута", "", false, false, false, false);
                if (fr.ShowDialog(this) == DialogResult.OK)
                {
                    tf.Name = fr.Result;

                    //добавление в основное окно списка маршрутов
                    if (Program.winConverterNullOrDisposed)
                        Program.winConverter = new FormConverter();
                    if (!Program.winConverter.Visible)
                        Program.winConverter.Show();

                    Program.winConverter.AddRouteToList(tf);

                    //вычисление параметров
                    this.creatingRoute.CalculateAll();

                    //вывод на карту
                    this.ShowRoute(this.creatingRoute);

                    //добавление в список маршрутов на карте
                    if (this.tracks == null)
                        this.tracks = new TrackFileList();
                    this.tracks.Add(this.creatingRoute);
                }
                else
                    switch (MessageBox.Show(this, "Отменить создание маршрута? Все именения будут потеряны!", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    {
                        case DialogResult.Yes:
                            creatingRouteOverlay.Routes.Clear();
                            creatingRouteOverlay.Markers.Clear();
                            creatingRoute = null;
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            //открываем заново ввод названия
                            goto readName;
                    }
            });
        }

        /// <summary>
        /// загрузка списка путевых точек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadWaypointsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.kml, *.kmz, *.gpx, *.rte, *.osm, *.nmea, *.csv, *.adrs, *.txt)|*.crd; *.wpt; *.plt; *rt2; *.kml; *.kmz; *.gpx; *.rte; *.osm; *.nmea; *.csv; *.adrs; *.txt";
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
                of.Filter += "|Файл Excel(*.csv)|*.csv";
                of.Filter += "|Список адресов(*.adrs)|*.adrs";
                of.Filter += "|Текстовый файл(*.txt)|*.txt";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = false;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == DialogResult.OK)
                {
                    Task ts = new Task(new Action(() =>
                        {
                            TrackFile tf = null;
                            if (Path.GetExtension(of.FileName) == ".adrs")
                            {
                                //MessageBox.Show("Для открытия этого файла требуется подключение к сети и много времени");
                                Program.winMain.BeginOperation();
                                tf = Serializer.DeserializeTrackFile(of.FileName, Program.winMain.setCurrentOperation);
                            }
                            else
                                tf = Serializer.DeserializeTrackFile(of.FileName);

                            this.Invoke(new Action(() =>
                                          {
                                              Program.winMap.clearMarkersToolStripMenuItem_Click(null, null);
                                              ShowWaypoints(tf, baseOverlay, false);
                                              if (waypoints == null)
                                                  waypoints = new TrackFile();
                                              waypoints.Add(tf);
                                              Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(of.FileName);
                                              Program.winMain.EndOperation();

                                          }));
                        }));
                    ts.Start();
                }
                else return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        #endregion

        #region карта

        #region поставщик карты

        /// <summary>
        /// выбор поставщика карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mpProviderToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MapProviderRecord mpr = (MapProviderRecord)((ToolStripMenuItem)sender).Tag;
            switch (mpr.Enum)
            {
                case MapProviders.GoogleHybridMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
                case MapProviders.GoogleMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleMap;
                    break;
                case MapProviders.GoogleSatelliteMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case MapProviders.OpenCycleMap:
                    gmapControlMap.MapProvider = GMapProviders.OpenCycleMap;
                    break;
                case MapProviders.YandexHybridMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexHybridMap;
                    break;
                case MapProviders.YandexMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexMap;
                    break;
                case MapProviders.YandexSatelliteMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexSatelliteMap;
                    break;
                default:
                    throw new NotSupportedException("Этот поставщик карты не поддерживается " + mpr.Enum);
            }


            Vars.Options.Map.MapProvider = mpr;

            foreach (ToolStripMenuItem ti in toolStripDropDownButtonMapProvider.DropDownItems)
                if (((MapProviderRecord)(ti.Tag)).ID == mpr.ID)
                    ti.Checked = true;
                else
                    ti.Checked = false;

            foreach (ToolStripMenuItem ti in mapProviderToolStripMenuItem.DropDownItems)
                if (((MapProviderRecord)(ti.Tag)).ID == mpr.ID)
                    ti.Checked = true;
                else
                    ti.Checked = false;

        }

        #endregion

        #region очистка

        /// <summary>
        /// очистка списка маршрутов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearRoutesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (creatingRouteOverlay != null)
                creatingRouteOverlay.Clear();
            if (rulerRouteOverlay != null)
                rulerRouteOverlay.Clear();
            fromToOverlay.Routes.Clear();
            baseOverlay.Routes.Clear();
            selectedRouteOverlay.Routes.Clear();

            if (tracks != null)
                tracks.Clear();
        }

        /// <summary>
        /// очистка маркеров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waypoints != null)
                waypoints.Clear();
            fromToOverlay.Markers.Clear();
            baseOverlay.Markers.Clear();
            selectedRouteOverlay.Markers.Clear();
        }

        /// <summary>
        /// очистка всей карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearMarkersToolStripMenuItem_Click(null, null);
            clearRoutesToolStripMenuItem_Click(null, null);
        }
        #endregion

        #region источник данных

        private void tmInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //порядок получения данных 
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            Vars.Options.Map.AccessMode = AccessMode.ServerOnly;
            tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }
        private void tmCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.CacheOnly;
            Vars.Options.Map.AccessMode = AccessMode.CacheOnly;
            tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }
        private void tmInternetCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Vars.Options.Map.AccessMode = AccessMode.ServerAndCache;
            tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;
        }

        #endregion

        #endregion

        #region Инструменты

        /// <summary>
        /// профиль высот по пути
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elevationGraphRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                creatingRoute = new TrackFile();
                BeginEditRoute(creatingRoute, (track) =>
                {
                    TrackFile added = track;
                    if (Vars.Options.Graphs.IsAddIntermediatePoints)
                        added = track.AddIntermediatePoints(Vars.Options.Graphs.IntermediateDistance);

                    Program.winWaiting.Visible = true;

                    TrackFile elevTrack = new TrackFile();
                    Task pr = new Task(new Action(() =>
                    {
                        try
                        {
                            if (GeoInfo.IsETOPOReady)
                                elevTrack = new GeoInfo(GeoInfoProvider.ETOPO).GetElevation(added);
                            else
                                throw new InvalidOperationException("База данных ETOPO не установлена");
                        }
                        catch (Exception exr)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    Program.winWaiting.Visible = false;
                                    MessageBox.Show(this, exr.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }));
                            }
                            return;
                        }
                    }));
                    pr.Start();
                    pr.Wait();
                    Program.winWaiting.Visible = false;
                    elevTrack.CalculateAll();
                    new FormElevVisual(new TrackFileList() { elevTrack }) { FormBorderStyle = FormBorderStyle.Sizable }.Show();
                });
            }
            catch (Exception ex)
            {
                Program.winWaiting.Visible = false;
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Program.winWaiting.Visible = false; }
        }

        /// <summary>
        /// обновление подсказки для кнопки построения профиля высот
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elevationGraphRouteToolStripMenuItem_Paint(object sender, PaintEventArgs e)
        {
            elevationGraphRouteToolStripMenuItem.ToolTipText = " Будут добавлены дополнительные точки через каждые " + Vars.Options.Graphs.IntermediateDistance + " м. Это значение можно изменить в настройках";
        }

        /// <summary>
        /// преобразовать точки в маршрут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pointsToRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waypoints == null)
                waypoints = new TrackFile();
            if (tracks == null)
                tracks = new TrackFileList();
            tracks.Add(waypoints);
            ShowTracks(this.tracks, true);
            waypoints = null;
            baseOverlay.Markers.Clear();
        }

        /// <summary>
        /// пребразовать маршрут в точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void routeToPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waypoints == null)
                waypoints = new TrackFile();
            foreach (TrackFile tf in tracks)
                waypoints.Add(tf);
            ShowWaypoints(waypoints, baseOverlay, false);
            baseOverlay.Routes.Clear();
            tracks = null;
        }

        /// <summary>
        /// открытие путевых точек в редакторе.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormPoints(waypoints, (tt) => { EndEditWaypoints(tt); }).Show();
        }

        /// <summary>
        /// построение оптимального маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createOptimalRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waypoints.Count < 3)
            {
                MessageBox.Show(this, "Для построения оптимального маршрута надо поставить хотя бы 3 точки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TrackPoint startPoint = null;
            TrackPoint endPoint = null;


            FormSelectPoint fsp = new FormSelectPoint(waypoints, true, "Выберите начальную точку");
            if (fsp.ShowDialog(this) == DialogResult.OK)
                startPoint = fsp.Result;
            else return;

            bool isCycled = startPoint == null;

            if (Vars.Options.Map.OptimalRouteMethod == OptimalMethodType.RecursiveEnum && !isCycled)
            {
                FormSelectPoint fsp1 = new FormSelectPoint(waypoints, startPoint, false, "Выберите конечную точку");
                if (fsp1.ShowDialog(this) == DialogResult.OK)
                    endPoint = fsp1.Result;
                else return;
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
                            route = new BranchBounds(Program.winMain.setCurrentOperation).Make(waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.Greedy:
                            route = new Greedy(Program.winMain.setCurrentOperation).Make(waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.FullSearch:
                            route = new FullSearch(Program.winMain.setCurrentOperation).Make(waypoints, startPoint, isCycled);
                            break;
                        case OptimalMethodType.RecursiveEnum:
                            route = new RecursiveEnum(Program.winMain.setCurrentOperation).Make(waypoints, startPoint, endPoint, isCycled);
                            break;
                        default:
                            route = new Greedy(Program.winMain.setCurrentOperation).Make(waypoints, startPoint, isCycled);
                            break;
                    }

                    //вывод маршрута на карту в базовом потоке
                    this.Invoke(new Action(() =>
                    {
                        route.CalculateAll();
                        TimeSpan span = DateTime.Now - start;
                        MessageBox.Show("Маршрут построен за:\r\n" + span.ToString(@"mm\:ss\.fff"));

                        if (Vars.Options.Services.ChangePathedRoute)
                            //если надо открыть маршрут для редактирования
                            BeginEditRoute(route, (tf) =>
                        {
                        //ввод названия марщрута
                        readName:
                            FormReadText fr = new FormReadText(DialogType.ReadText, "Введите название маршрута", "", false, false, false, false);
                            if (fr.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                            {
                                tf.Name = fr.Result;
                                //вычисление параметров
                                tf.CalculateAll();
                                //добавление в основное окно списка маршрутов
                                Program.winConverter.AddRouteToList(tf);
                                //вывод на карту
                                this.ShowRoute(this.creatingRoute);
                                //добавление в список маршрутов на карте
                                if (this.tracks == null)
                                    this.tracks = new TrackFileList();
                                this.tracks.Add(tf);
                            }
                            else
                                switch (MessageBox.Show(this, "Отменить создание маршрута? Все именения будут потеряны!", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
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
                            //если не надо открывать мршрут
                            tracks.Add(route);
                            Program.winConverter.AddRouteToList(route);
                            ShowRoute(route);
                        }
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


        #endregion

        #region контекстное меню карты

        #region на пустой карте

        /// <summary>
        /// вывод информации о точке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemWhatsThere_Click(object sender, EventArgs e)
        {
            TrackPoint tt = new TrackPoint(pointClicked);
            tt.Icon = IconOffsets.what_there;
            tt.Name = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddress(pointClicked);
            ShowWaypoint(tt, baseOverlay, Resources.what_there, MarkerTypes.WhatThere, MarkerTooltipMode.Always);
        }

        /// <summary>
        /// добавление новой путевой точки на карту
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAddWaypoint_Click(object sender, EventArgs e)
        {
            if (waypoints == null)
                waypoints = new TrackFile();
            TrackPoint tt = new TrackPoint(pointClicked);
            FormEditPoint fe = new FormEditPoint(tt);
            if (fe.ShowDialog(this) == DialogResult.OK)
            {
                waypoints.Add(fe.Result);
                ShowWaypoints(waypoints, baseOverlay, true);

                LastEditsStack.Push(
                    new StackItem(
                        new MarkerAddInfo(fe.Result, new Action<TrackPoint>(
                            (newP) =>
                            {
                                if (waypoints.Contains(newP))
                                {
                                    waypoints.Remove(newP);
                                    ShowWaypoints(waypoints, baseOverlay, true);
                                }
                            }
                            )
                        )
                    )
                    );
                UpdateUndoButton();

            }
        }

        /// <summary>
        /// установка точки начала или конца маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createRoutePathingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryPathRoute((string)((ToolStripMenuItem)sender).Tag, true);
        }

        /// <summary>
        /// копирование координат под курсором
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyCoordinatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Coordinate cr = new Coordinate(pointClicked);
            Clipboard.SetText(cr.ToString("{lat} {lon}", "ddºmm'ss.s\"H"));
        }


        #endregion

        #region на маркере

        /// <summary>
        /// изменение маркера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //запоминание предыдущего тега
            MarkerTag tag = markerClicked.Tag;


            FormEditPoint fe = new FormEditPoint(tag.Info);
            fe.ShowDialog(this);

            //если информация изменилась, то записываем новую
            if (fe.DialogResult == DialogResult.OK)
            {
                markerClicked.Tag.Info = fe.Result; //запись нофой информации в тег
                DeleteWaypoint(markerClicked.Tag.Info, baseOverlay); //удаление маркера со слоя
                ShowWaypoint(markerClicked.Tag.Info, baseOverlay); //добавление маркера в слой
                UpdateUndoButton();
                LastEditsStack.Push(
                    new StackItem(
                        new MarkerEditInfo(fe.Result, tag.Info, new Action<TrackPoint, TrackPoint>(
                            (newP, oldP) =>
                            {
                                if (waypoints.Contains(newP))
                                {
                                    waypoints[waypoints.IndexOf(newP)] = oldP;
                                    ShowWaypoints(waypoints, baseOverlay, true);
                                }
                            }
                            )
                    )
                    )

               );
                UpdateUndoButton();

            }
            //если нет - старую
            else
                markerClicked.Tag = tag;
        }

        /// <summary>
        /// удаление маркера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы действительно хотите удалить этот маркер?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                baseOverlay.Markers.Remove(markerClicked);
                waypoints.Remove(markerClicked.Tag.Info);

                UpdateUndoButton();
                LastEditsStack.Push(
                    new StackItem(
                        new MarkerDeleteInfo(markerClicked.Tag.Info, new Action<TrackPoint>(
                            (oldP) =>
                            {
                                waypoints.Add(oldP);
                                ShowWaypoints(waypoints, baseOverlay, false);
                            }
                            )
                            )
                        )
                    );
                UpdateUndoButton();
            }
        }

        #endregion


        #endregion

        #region панели инструментов

        /// <summary>
        /// отмена последнего изменения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            if (LastEditsStack.Count > 0)
                this.LastEditsStack.Pop().Data.Undo();
            UpdateUndoButton();

        }


        /// <summary>
        /// приближение 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            gmapControlMap.Zoom++;
        }

        /// <summary>
        /// отдаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            gmapControlMap.Zoom--;
        }

        /// <summary>
        /// Измерение расстояний
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRuler_Click(object sender, EventArgs e)
        {
            if (isCreatingRoute)
            {
                MessageBox.Show("Идет создание маршрута. Завершите Создание перед тем, как использовать этот инструмент");
                return;
            }
            gmapControlMap.DragButton = System.Windows.Forms.MouseButtons.Right;
            isRuling = true;
            rulerRouteOverlay.Clear();
            rulerRoute = new TrackFile();
            Program.winRouteEditor = new FormEditRoute(
                "Измерениe расстояния",
                rulerRoute,
                rulerRouteOverlay,
                null,
                null
                );
            Program.winRouteEditor.Show(Program.winMain);
        }

        /// <summary>
        /// изменение текста в поле поиска адреса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxGoTo_TextChanged(object sender, EventArgs e)
        {
            lastCBGoToChanged = DateTime.Now;
            //lastGoToQuery = toolStripComboBoxGoTo.Text;
        }

        /// <summary>
        /// выбор адреса для перехода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBoxGoTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, Coordinate> crds = (Dictionary<string, Coordinate>)toolStripComboBoxGoTo.Tag;
            //Coordinate cr = crds[toolStripComboBoxGoTo.SelectedItem.ToString()];

            foreach (KeyValuePair<string, Coordinate> pr in crds)
                if (pr.Key == toolStripComboBoxGoTo.SelectedItem.ToString())
                {
                    this.PositionsStack.Push(pr);
                    gmapControlMap.Position = pr.Value.GMap;
                    toolStripComboBoxGoTo.Text = "";
                    toolStripComboBoxGoTo.DroppedDown = false;
                    gmapControlMap.Focus();
                    break;
                }
        }


        #endregion

        /// <summary>
        /// закрытие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = TryClose(sender, e);
            if (!e.Cancel)
            {
                Vars.Options.Map.LastCenterPoint = gmapControlMap.Position;
                Vars.Options.Map.IsFormNavigatorShow = !Program.winNavigatorNullOrDisposed;
                Vars.Options.Map.WinSize = this.Size;
                this.refreshGoToTimer.Stop();
            }
        }

        /// <summary>
        /// горячие клавиши, перемещение карты стрелками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMap_KeyDown(object sender, KeyEventArgs e)
        {
            //CTRL+F
            if (new Keyboard().CtrlKeyDown)
                if (e.KeyCode == Keys.F)
                {
                    if (Program.winNavigatorNullOrDisposed)
                        Program.winNavigator = new FormMapNavigator();
                    if (!Program.winNavigator.Visible)
                        Program.winNavigator.Show(this);
                    Program.winNavigator.Activate();

                    this.toolStripComboBoxGoTo.Focus();
                }

            //CTRL+N
            if (new Keyboard().CtrlKeyDown)
                if (e.KeyCode == Keys.N)
                    createRouteToolStripMenuItem.PerformClick();
        }

        /// <summary>
        /// отпускание кнопки клавиатуры.
        /// Остановка перемещения карты стрелками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMap_KeyUp(object sender, KeyEventArgs e)
        {
            this.canMoveMap = false;
        }

        /// <summary>
        /// обработка нажатий на стрелки для перемещения карты
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData == Keys.Left) || (keyData == Keys.Right) || (keyData == Keys.Down) || (keyData == Keys.Up))
            {
                this.canMoveMap = true;
                this.canMoveMapDirection = keyData;
            }
            return base.ProcessDialogKey(keyData);
        }


        #endregion

        #region события карты

        /// <summary>
        /// нажатие на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseClick(object sender, MouseEventArgs e)
        {
            //добавление точки к новому маршруту если 
            //происходит создание маршрута, 
            //не происходит перемещение маркера,
            //указатель не находится на другом маркере
            if (e.Button == MouseButtons.Left && //если левая кнопка
                (isCreatingRoute || isRuling) && //создание маршрута или линейка
                !isMarkerMoving && //не происходит движение маркера
                !isMarkerClicked)
            {
                if (isCreatingRoute)
                {
                    PointLatLng pt = gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                    //если выделена последняя точка, то добавляем
                    if (selectedPointIndex == creatingRoute.Count)
                    {
                        creatingRoute.Add(new TrackPoint(pt));
                        selectedPointIndex = creatingRoute.Count - 1;
                    }
                    //если выделена точка в середине маршрута, то вставляем после нее
                    else
                    {
                        creatingRoute.Insert(selectedPointIndex + 1, new TrackPoint(pt));
                        selectedPointIndex++;
                    }

                    //вывод нового маршрута на экран
                    ShowCreatingRoute(creatingRouteOverlay, creatingRoute);
                }
                if (isRuling)
                {
                    PointLatLng pt = gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                    //если выделена последняя точка, то добавляем
                    if (selectedPointIndex == rulerRoute.Count)
                    {
                        rulerRoute.Add(new TrackPoint(pt));
                        selectedPointIndex = rulerRoute.Count - 1;
                    }
                    //если выделена точка в середине маршрута, то вставляем после нее
                    else
                    {
                        rulerRoute.Insert(selectedPointIndex + 1, new TrackPoint(pt));
                        selectedPointIndex++;
                    }

                    //вывод нового маршрута на экран
                    ShowCreatingRoute(rulerRouteOverlay, rulerRoute);
                }
                return;
            }
            isMarkerClicked = false;
        }

        /// <summary>
        /// приближение и отдаление карты по двойному нажатию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                gmapControlMap.Zoom++;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                gmapControlMap.Zoom--;
        }

        /// <summary>
        /// установка состояния левой кнопки мыши для перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseDown(object sender, MouseEventArgs e)
        {
            //Выполняем проверку, какая клавиша мыши была нажата,
            //если левая, то устанавливаем переменной значение true.
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isLeftButtonDown = true;
            }

            //сброс текущего маркера
            if (!gmapControlMap.IsMouseOverMarker)
                currentMarker = null;
        }

        /// <summary>
        /// установка состояния левой кнопки мыши для перетаскивания
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseUp(object sender, MouseEventArgs e)
        {
            //Выполняем проверку, какая клавиша мыши была отпущена,
            //если левая то устанавливаем переменной значение false.
            if (e.Button == MouseButtons.Left)
            {
                isLeftButtonDown = false;

                if (isMarkerMoving)
                {
                    //если это маркер построения маршрута, то перестраиваем маршрут
                    if (currentMarker.Tag.Type == MarkerTypes.PathingRoute)
                    {
                        TryPathRoute(currentMarker.Tag.Info.Name, false);
                    }

                    //добавление информации для отмены действия
                    LastEditsStack.Push(
                            new StackItem(
                                new MarkerMoveInfo(
                                    currentMarker.Tag.Info,
                                    oldMarker, new Action<TrackPoint, TrackPoint>(
                                        (newPt, oldPt) =>
                                        {
                                            if (creatingRoute != null && creatingRoute.Contains(newPt))
                                            {
                                                creatingRoute[creatingRoute.IndexOf(newPt)] = oldPt;
                                                ShowCreatingRoute(creatingRouteOverlay, creatingRoute);
                                            }
                                            if (rulerRoute != null && rulerRoute.Contains(newPt))
                                            {
                                                rulerRoute[rulerRoute.IndexOf(newPt)] = oldPt;
                                                ShowCreatingRoute(rulerRouteOverlay, rulerRoute);
                                            }
                                            if (waypoints != null && waypoints.Contains(newPt))
                                            {
                                                waypoints[waypoints.IndexOf(newPt)] = oldPt;
                                                ShowWaypoints(waypoints, baseOverlay, true);
                                            }
                                        }
                                    )
                                )
                            )
                        );
                    UpdateUndoButton();
                    isMarkerMoving = false;
                }

                //обновление информации в списке точек после перемещения
                RefreshWaypoints();
                return;
            }

            //вывод контекстного меню
            if (e.Button == MouseButtons.Right && !gmapControlMap.IsDragging)
            {
                if (!gmapControlMap.IsMouseOverMarker)
                {
                    contextMenuStripMap.Show(gmapControlMap, new Point(e.X, e.Y));
                    this.pointClicked = gmapControlMap.FromLocalToLatLng(e.X, e.Y);
                }
                return;
            }
        }

        /// <summary>
        /// передвижение маркера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseMove(object sender, MouseEventArgs e)
        {
            //если курсор за пределами карты, то выход.
            try { new Coordinate(gmapControlMap.FromLocalToLatLng(e.X, e.Y)); }
            catch (ArgumentOutOfRangeException) { return; }

            //вывод информации о координатах мыши
            Coordinate cr = new Coordinate(gmapControlMap.FromLocalToLatLng(e.X, e.Y));
            toolStripStatusLabelLat.Text = cr.Latitude.ToString("ddºmm'ss.s\"H");
            toolStripStatusLabelLon.Text = cr.Longitude.ToString("ddºmm'ss.s\"H");

            //вывод информации о высоте в этой точке
            //double alt = new GeoInfo( GeoInfoProvider.Google ).GetElevation( gmapControlMap.FromLocalToLatLng( e.X, e.Y ) );
            //toolStripStatusLabelAltitude.Text = "Высота: " + alt + " м";

            //передвижение маркера
            //Проверка, что нажата левая клавиша мыши и не происходит перемещение карты
            if (currentMarker != null &&
                e.Button == MouseButtons.Left &&
                isLeftButtonDown &&
                (currentMarker.Tag.Type != MarkerTypes.WhatThere))
            {

                if (!isMarkerMoving) //если движение маркера только начинается, то запоминаемего положение для отмены
                    oldMarker = currentMarker.Tag.Info.Clone();
                isMarkerMoving = true;

                PointLatLng point = gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                //Получение координат маркера.
                currentMarker.Position = point;
                currentMarker.Tag.Info.Coordinates = new Coordinate(point);

                //Вывод координат маркера в подсказке.
                currentMarker.ToolTipText = string.Format("{0},{1}", point.Lat.ToString("00.000"), point.Lng.ToString("00.000"));

                //начинаем перемещение маркера


                //если создается маршрут, то обновляем длину маршрута
                if (isCreatingRoute)
                    ShowCreatingRoute(creatingRouteOverlay, creatingRoute);
                if (isRuling)
                    ShowCreatingRoute(rulerRouteOverlay, rulerRoute);
            }


        }

        /// <summary>
        /// вывод подсказки при наведении на маркер
        /// </summary>
        /// <param name="itm"></param>
        private void gmapControlMap_OnMarkerEnter(GMapMarker itm)
        {
            MapMarker item = itm as MapMarker;
            if (!isMarkerMoving)
                currentMarker = item as MapMarker;

            //если маркер нового маршрута, то вывод расстояия от начала пути
            if (isCreatingRoute && item.Tag.Type == MarkerTypes.CreatingRoute)
            {
                TrackFile tf = new TrackFile(creatingRoute.Take(creatingRoute.IndexOf(item.Tag.Info) + 1));
                tf.CalculateAll();
                double lg = tf.Distance;
                toolStripStatusLabelFromStart.Text = "От начала пути: " + lg;
            }
            //вывод подсказки (долгота широта имя)
            if (!isCreatingRoute && (item.Tag.Type != MarkerTypes.WhatThere))
            {
                item.ToolTipText = item.Tag.Info.Name;
                //string.Format(
                //    "{2}\r\n{0},{1}",
                //    item.Position.Lat.ToString("00.000"),
                //    item.Position.Lng.ToString("00.000"),
                //    item.Tag.Info.Name
                //);
            }
            else
            {

            }
        }

        /// <summary>
        /// выход указателя за пределы маркера
        /// </summary>
        /// <param name="item">маркер</param>
        private void gmapControlMap_OnMarkerLeave(GMapMarker item)
        {
            toolStripStatusLabelFromStart.Text = "";
        }

        /// <summary>
        /// изменение информации о точке, изменение выделенной точки при создании маршрута
        /// </summary>
        /// <param name="itm"></param>
        /// <param name="e"></param>
        private void gmapControlMap_OnMarkerClick(GMapMarker itm, MouseEventArgs e)
        {
            MapMarker item = itm as MapMarker;

            //изменение инормаци по лкм
            //если не происходило перемещение маркера и не происходит создание маршрута и не линейка
            if (!isMarkerMoving && !isCreatingRoute && !isRuling && item.Tag.Type != MarkerTypes.WhatThere && e.Button == MouseButtons.Left)
            {
                markerClicked = item as MapMarker;
                isMarkerClicked = true;
                editMarkerToolStripMenuItem_Click(null, e);
            }

            //открытие контекстного меню по пкм
            //если не передвижение маркера и не создание маршрута и правая кнопка мыши то вывод меню маркера
            if (!isMarkerMoving && item.Tag.Type != MarkerTypes.CreatingRoute && e.Button == MouseButtons.Right)
            {
                markerClicked = item as MapMarker;
                contextMenuStripMarker.Show(new Point(e.X, e.Y));
            }

            //выделение нажатого маркера при создании маршрута
            if (e.Button == MouseButtons.Left && (isCreatingRoute || isRuling) && !isMarkerMoving && gmapControlMap.IsMouseOverMarker)
            {
                if (isCreatingRoute)
                {
                    if (creatingRoute.Contains(item.Tag.Info))
                    {
                        selectedPointIndex = creatingRoute.IndexOf(item.Tag.Info);
                        isMarkerClicked = true;
                        ShowCreatingRoute(creatingRouteOverlay, creatingRoute);
                    }
                }
                if (isRuling)
                {
                    if (rulerRoute.Contains(item.Tag.Info))
                    {
                        selectedPointIndex = rulerRoute.IndexOf(item.Tag.Info);
                        isMarkerClicked = true;
                        ShowCreatingRoute(rulerRouteOverlay, rulerRoute);
                    }
                }
            }

        }

        /// <summary>
        /// сброс активного маркера при перемещении карты
        /// </summary>
        private void gmapControlMap_OnMapDrag()
        {
            currentMarker = null;
        }

        /// <summary>
        /// изменение зума
        /// </summary>
        private void gmapControlMap_OnMapZoomChanged()
        {
            if (!Program.winNavigatorNullOrDisposed && Program.winNavigator.Visible)
                Program.winNavigator.labelZoom.Text = "Zoom: " + gmapControlMap.Zoom;
            toolStripLabelZoom.Text = gmapControlMap.Zoom.ToString();
            Vars.Options.Map.Zoom = gmapControlMap.Zoom;
        }

        /// <summary>
        /// действие с выделенной областью
        /// </summary>
        /// <param name="Selection">область</param>
        /// <param name="ZoomToFit"></param>
        private void gmapControlMap_OnSelectionChange(RectLatLng Selection, bool ZoomToFit)
        {
            new FormSelectMapActions(this, Selection).Show(this);
        }

        /// <summary>
        /// сохранение позиции при перемещении карты
        /// </summary>
        /// <param name="point"></param>
        private void gmapControlMap_OnPositionChanged(PointLatLng point)
        {
            Vars.Options.Map.LastCenterPoint = point;
        }

        /// <summary>
        /// при изменении поставщика карты установка максимального и минимального масштаба
        /// </summary>
        /// <param name="type"></param>
        private void gmapControlMap_OnMapTypeChanged(GMapProvider type)
        {
            if (type.MaxZoom == null)
                gmapControlMap.MaxZoom = 21;
            else
                gmapControlMap.MaxZoom = (int)type.MaxZoom;
            gmapControlMap.MinZoom = type.MinZoom;
        }

        #endregion

        #region вспомогательные методы

        #region отрисовка карты

        /// <summary>
        /// выод на карту списка точек. Новые точки будут добавлены в список
        /// </summary>
        /// <param name="isClearBefore">если истина, то перед выводом слой будет очищен от мареров</param>
        /// <param name="wpts">список точек для вывода</param>
        /// <param name="lay">слой, на котором будут выведены точки</param>
        private void ShowWaypoints(TrackFile wpts, GMapOverlay lay, bool isClearBefore)
        {
            if (this.waypoints == null)
                this.waypoints = new TrackFile();

            gmapControlMap.SuspendLayout();
            if (isClearBefore)
            {
                lay.Markers.Clear();
                this.waypoints = wpts;
            }

            foreach (TrackPoint tt in wpts)
            {
                ShowWaypoint(tt, lay);
                if (!waypoints.Contains(tt))
                    waypoints.Add(tt);
            }
            gmapControlMap.ResumeLayout();
        }

        /// <summary>
        /// удаление отображаемой точки точки с слоя 
        /// </summary>
        /// <param name="trackPoint">точка</param>
        /// <param name="lay">слой</param>
        private void DeleteWaypoint(TrackPoint trackPoint, GMapOverlay lay)
        {
            for (int i = 0; i < lay.Markers.Count; i++)
            {
                MapMarker it = lay.Markers[i] as MapMarker;
                if (it.Tag.Info == trackPoint)
                {
                    lay.Markers.RemoveAt(i);
                    return;
                }
            }
        }


        /// <summary>
        /// покзать одну точку
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="lay">слой в GMapControl для отображения маркера</param>
        private void ShowWaypoint(TrackPoint point, GMapOverlay lay)
        {
            //если иконки нет, то базовую иконку
            Icon ic;
            if (point.Icon == IconOffsets.marker)
                ic = Resources.marker;
            else
            {
                //если есть файл такой иконки, то его, если нет - базовую иконку
                string fn = Application.StartupPath + Resources.icons_directory + "\\" + point.Icon.ToString("000") + ".ico";
                if (File.Exists(fn))
                    ic = new Icon(fn);
                else
                    ic = Resources.marker;
            }
            ShowWaypoint(point, lay, ic, MarkerTypes.Waypoint);
        }

        /// <summary>
        /// показать одну точку с заданной картинкой
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="lay">слой</param>
        /// <param name="icon">относительный адрес картинки (из Resources)</param>
        /// <param name="type">тип маркера</param>
        private void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes type)
        {
            ShowWaypoint(point, lay, icon, type, MarkerTooltipMode.OnMouseOver);
        }

        /// <summary>
        /// показать одну точку с заданной картинкой, на указанном слое, с заданными типами подсказки и маркера
        /// </summary>
        /// <param name="point">информация о точке</param>
        /// <param name="lay">слой</param>
        /// <param name="icon">картинка</param>
        /// <param name="mType">тип  маркера</param>
        /// <param name="ttMode">тип всплывающей подсказки</param>
        private void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes mType, MarkerTooltipMode ttMode)
        {
            Point offsets = IconOffsets.GetOffset(point.Icon);
            MapMarker mar = new MapMarker(point.Coordinates.GMap, icon, offsets);

            if (string.IsNullOrWhiteSpace(point.Name))
                point.Name = point.Coordinates.ToString("{lat},{lon}", "00.000");

            mar.Tag.Type = mType;
            mar.ToolTipText = point.Name;
            mar.ToolTipMode = ttMode;
            mar.Tag.Info = point;

            lay.Markers.Add(mar);
        }

        /// <summary>
        /// вывод создаваемого маршрута и его маркеров 
        /// </summary>
        public void ShowCreatingRoute(GMapOverlay overlay, TrackFile track)
        {
            //gmapControlMap.SuspendLayout();
            track.CalculateAll();
            overlay.Clear();
            int i = 0;
            foreach (TrackPoint tt in track)
            {
                Icon ic;
                if (i == selectedPointIndex)
                    ic = Resources.route_point_selected;
                else
                    ic = Resources.route_point;

                MapMarker mar = new MapMarker(tt.Coordinates.GMap, ic);
                mar.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                mar.Tag.Info = tt;
                mar.Tag.Type = MarkerTypes.CreatingRoute;
                overlay.Markers.Add(mar);
                i++;
            }
            track.Color = Color.DarkBlue;
            ShowTrack(track, overlay, false);
            RefreshToolTipsCreatingRoute(overlay);
            //gmapControlMap.Overlays.Add(overlay);
            toolStripStatusLabelInfo.Text = "Расстояние: " + track.Distance + " км, количество точек: " + track.Count;
            //gmapControlMap.ResumeLayout(true);
        }


        /// <summary>
        /// показать на карте маршруты.
        /// </summary>
        /// <param name="tracksList">маршруты</param>
        /// <param name="isClearBefore">если истина, то перед добавление будут удалены предыдущие маршруты</param>
        private void ShowTracks(TrackFileList tracksList, bool isClearBefore)
        {
            if (isClearBefore)
                baseOverlay.Routes.Clear();

            foreach (TrackFile tf in tracksList)
            {
                if (!tracks.Contains(tf))
                    tracks.Add(tf);
                ShowTrack(tf, baseOverlay, false);
            }
        }

        /// <summary>
        /// отрисовка маршрута на заданном слое
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="lay">слой</param>
        /// <param name="centring">если истина, то после отрисовки карта будет подогнана под маршрут по центру и масштбу</param>
        private void ShowTrack(TrackFile route, GMapOverlay lay, bool centring = true)
        {
            if (route == null)
                return;
            if (route.Color.IsEmpty)
                route.Color = Vars.Options.Converter.GetColor();
            GMapRoute rr = new GMapRoute(route.GMapPoints, route.Name);
            rr.Stroke = new Pen(new SolidBrush(route.Color));
            rr.Stroke.Width = 3;

            lay.Routes.Add(rr);
            if (centring)
                gmapControlMap.ZoomAndCenterRoute(rr);
        }

        /// <summary>
        /// обновление подсказок над маркерами создаваемого маршрута
        /// </summary>
        private void RefreshToolTipsCreatingRoute(GMapOverlay overlay)
        {
            foreach (MapMarker item in overlay.Markers)
            {
                item.ToolTip = new GMapToolTip(item);
                item.ToolTipMode = MarkerTooltipMode.Always;
                item.ToolTip.Fill = new SolidBrush(Color.Transparent);
                item.ToolTip.Foreground = new SolidBrush(Color.Black);
                item.ToolTip.Stroke = new Pen(Color.Black, 1);
                item.ToolTip.Font = new Font("Times New Roman", 10, FontStyle.Bold);
                item.ToolTipText = item.Tag.Info.Distance.ToString("00.000") + " км";
                item.ToolTipText += !double.IsNaN(item.Tag.Info.TrueAzimuth) ? "\r\nАзимут: " + item.Tag.Info.TrueAzimuth + "º" : "";
            }
        }

        #endregion

        /// <summary>
        /// обновление списка точек после перемещения точки
        /// </summary>
        private void RefreshWaypoints()
        {
            if (waypoints == null)
                waypoints = new TrackFile();
            waypoints.Clear();
            foreach (MapMarker mm in baseOverlay.Markers)
            {
                TrackPoint tt = mm.Tag.Info;
                waypoints.Add(tt);
            }
        }

        /// <summary>
        /// настройки браузера карты
        /// </summary>
        private void ConfigureGMapControl()
        {

            #region системные настройки

            gmapControlMap.DragButton = MouseButtons.Left;

            //порядок получения данных 
            GMaps.Instance.Mode = Vars.Options.Map.AccessMode;

            //zoom
            gmapControlMap.Zoom = Vars.Options.Map.Zoom;
            gmapControlMap_OnMapZoomChanged();

            //информация о масштабе карты
            gmapControlMap.MapScaleInfoEnabled = true;

            //включение кэша карт, маршрутов итд
            gmapControlMap.Manager.UseDirectionsCache = true;
            gmapControlMap.Manager.UseGeocoderCache = true;
            gmapControlMap.Manager.UseMemoryCache = true;
            gmapControlMap.Manager.UsePlacemarkCache = true;
            gmapControlMap.Manager.UseRouteCache = true;
            gmapControlMap.Manager.UseUrlCache = true;

            //отключение черно-белого режима
            gmapControlMap.GrayScaleMode = false;

            //заполнение отсутствующих тайлов из меньшего масштаба
            gmapControlMap.FillEmptyTiles = true;

            //язык карты
            GMapProvider.Language = Vars.Options.Map.MapLanguange;

            //поставщик карты
            switch (Vars.Options.Map.MapProvider.Enum)
            {
                case MapProviders.GoogleHybridMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleHybridMap;
                    break;
                case MapProviders.GoogleMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleMap;
                    break;
                case MapProviders.GoogleSatelliteMap:
                    gmapControlMap.MapProvider = GMapProviders.GoogleSatelliteMap;
                    break;
                case MapProviders.OpenCycleMap:
                    gmapControlMap.MapProvider = GMapProviders.OpenCycleMap;
                    break;
                case MapProviders.YandexHybridMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexHybridMap;
                    break;
                case MapProviders.YandexMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexMap;
                    break;
                case MapProviders.YandexSatelliteMap:
                    gmapControlMap.MapProvider = GMapProviders.YandexSatelliteMap;
                    break;
                default:
                    throw new NotSupportedException("Этот поставщик карты не поддерживается " + Vars.Options.Map.MapProvider.Enum);
            }


            //Если вы используете интернет через прокси сервер,
            //указываем свои учетные данные.
            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            //центральная точка
            if (Vars.Options.Map.LastCenterPoint != null)
                gmapControlMap.Position = Vars.Options.Map.LastCenterPoint;
            else
            {
                gmapControlMap.Position = new PointLatLng(37, 55);
                Vars.Options.Map.LastCenterPoint = gmapControlMap.Position;
            }

            //вид пустых тайлов
            gmapControlMap.EmptyMapBackground = Color.White;
            gmapControlMap.EmptyTileColor = Color.White;
            gmapControlMap.EmptyTileText = "Не удалось загрузить изображение \r\n Возможно, проблема с интернет-соединением или попробуйте уменьшить масштаб.";
            gmapControlMap.MapScaleInfoEnabled = true;

            //папка с кэшем
            Directory.CreateDirectory(Application.StartupPath + Resources.cache_directory);
            gmapControlMap.CacheLocation = Application.StartupPath + Resources.cache_directory;

            #endregion


            //обновление галочек в меню способа загрузки тайлов
            tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;

            #region создание объектов

            //создание слоев на карте
            selectedPointsOverlay = new GMapOverlay(selectedPointsOverlayID);
            selectedRouteOverlay = new GMapOverlay(selectedRouteOverlayID);
            creatingRouteOverlay = new GMapOverlay(creatingRouteOverlayID);
            rulerRouteOverlay = new GMapOverlay(rulerRouteOverlayID);
            baseOverlay = new GMapOverlay(baseOverlayID);
            fromToOverlay = new GMapOverlay(fromToOverlayID);

            //добавление слоев на карту
            gmapControlMap.Overlays.Add(selectedPointsOverlay);
            gmapControlMap.Overlays.Add(selectedRouteOverlay);
            gmapControlMap.Overlays.Add(creatingRouteOverlay);
            gmapControlMap.Overlays.Add(rulerRouteOverlay);
            gmapControlMap.Overlays.Add(baseOverlay);
            gmapControlMap.Overlays.Add(fromToOverlay);

            #endregion
        }

        /// <summary>
        /// передвижение карты по таймеру
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveMapTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (canMoveMap)
            {
                //на сколько перемещается карта за один проход таймера
                double stepCrds = Vars.MapMoveSteps[(int)gmapControlMap.Zoom];

                switch (canMoveMapDirection)
                {
                    case Keys.Left:
                        if (gmapControlMap.InvokeRequired)
                            gmapControlMap.Invoke(new Action(() =>
                            {
                                gmapControlMap.Position = new PointLatLng(gmapControlMap.Position.Lat, gmapControlMap.Position.Lng - stepCrds);
                            }));
                        break;
                    case Keys.Up:
                        if (gmapControlMap.InvokeRequired)
                            gmapControlMap.Invoke(new Action(() =>
                            {
                                gmapControlMap.Position = new PointLatLng(gmapControlMap.Position.Lat + stepCrds, gmapControlMap.Position.Lng);
                            }));
                        break;
                    case Keys.Right:
                        if (gmapControlMap.InvokeRequired)
                            gmapControlMap.Invoke(new Action(() =>
                            {
                                gmapControlMap.Position = new PointLatLng(gmapControlMap.Position.Lat, gmapControlMap.Position.Lng + stepCrds);
                            }));

                        break;
                    case Keys.Down:
                        if (gmapControlMap.InvokeRequired)
                            gmapControlMap.Invoke(new Action(() =>
                            {
                                gmapControlMap.Position = new PointLatLng(gmapControlMap.Position.Lat - stepCrds, gmapControlMap.Position.Lng);
                            }));
                        break;
                }
            }
        }

        /// <summary>
        /// обновление списка результатов поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshGoToTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() =>
                    {
                        //выход, если запрос не изменился
                        if (lastGoToQuery == toolStripComboBoxGoTo.Text)
                            return;

                        //выход и очистка окна, если пустой запрос
                        if (toolStripComboBoxGoTo.Text.Length == 0)
                        {
                            toolStripComboBoxGoTo.DroppedDown = false;
                            toolStripComboBoxGoTo.Items.Clear();
                            return;
                        }

                        //выход, если запрос меньше 4 символов
                        if (toolStripComboBoxGoTo.Text.Length < 4)
                            return;

                        //выход, если прошло менее 500 мс с последнего изменения запроса
                        if (DateTime.Now - lastCBGoToChanged < TimeSpan.FromSeconds(0.5))
                            return;

                        //ДОБАВЛЕНИЕ РЕЗУЛЬТАТОВ В СПИСОК

                        //формирование списка точек
                        Dictionary<string, Coordinate> adrs = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddresses(toolStripComboBoxGoTo.Text);

                        //добавление к combobox
                        toolStripComboBoxGoTo.BeginUpdate();
                        toolStripComboBoxGoTo.Items.Clear();
                        foreach (KeyValuePair<string, Coordinate> kv in adrs)
                            toolStripComboBoxGoTo.Items.Add(kv.Key);
                        toolStripComboBoxGoTo.EndUpdate();

                        //запись последнего запроса
                        lastGoToQuery = toolStripComboBoxGoTo.Text;

                        //запись списка в тег для дальнейшего выбора (при нажатии на элемент списка)
                        toolStripComboBoxGoTo.Tag = adrs;

                        //открываем список
                        toolStripComboBoxGoTo.DroppedDown = true;

                        //возвращаем курсор к жизни
                        this.Cursor = Cursors.Arrow;

                        //перенос курсора в конец строки запроса
                        toolStripComboBoxGoTo.SelectionStart = toolStripComboBoxGoTo.Text.Length - 1;

                    }));
            }
            catch (ObjectDisposedException)
            { }
        }

        /// <summary>
        /// обновление состояния кнопки отмены действия
        /// </summary>
        private void UpdateUndoButton()
        {
            toolStripButtonUndo.Enabled = LastEditsStack.Count > 0;
            if (LastEditsStack.Count > 0)
                toolStripButtonUndo.ToolTipText = "Отменить " + LastEditsStack.Peek().Text;
            else
                toolStripButtonUndo.ToolTipText = "Нет доступных действий для отмены";
        }

        /// <summary>
        /// попытка построить маршрут. На вход передается тег нажатой кнопки контекстного меню 
        /// (конец, начало, промежуточная точка) 
        /// если достаточно данных для построения, строится маршрут. Перед этим очищается слой fromToOverlay
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="isAddedNewPoint">если истина, то была добавлена новая точка и надо определить ее тип, 
        /// если ложь, то значит были только передвижения точек  добавлять их не надо
        /// </param>
        private void TryPathRoute(string tag, bool isAddedNewPoint)
        {
            //если была добавлена новая точка, то проверяем, что за точка, добавляем, и пробуем строить
            if (isAddedNewPoint)
            {
                TrackPoint tt = new TrackPoint(pointClicked);

                if (tag == "from")
                {
                    tt.Icon = IconOffsets.marker_start;
                    ShowWaypoint(tt, fromToOverlay, Resources.marker_start, MarkerTypes.PathingRoute, MarkerTooltipMode.Never);
                    tt.Name = "from";
                    fromPoint = tt;
                }
                if (tag == "to")
                {
                    tt.Icon = IconOffsets.marker_finish;
                    ShowWaypoint(tt, fromToOverlay, Resources.marker_finish, MarkerTypes.PathingRoute, MarkerTooltipMode.Never);
                    tt.Name = "to";
                    toPoint = tt;
                }
                if (tag == "intermediate")
                {
                    tt.Icon = IconOffsets.ZeroOffset;
                    ShowWaypoint(tt, fromToOverlay, Resources.intermed_point, MarkerTypes.PathingRoute, MarkerTooltipMode.Never);
                    if (IntermediatePoints == null)
                        IntermediatePoints = new TrackFile();
                    tt.Name = "intermediate";
                    IntermediatePoints.Add(tt);
                }
            }

            if (toPoint != null && fromPoint != null) //если достаточно точек для построения маршрута, то строим
            {
                if (!isAddedNewPoint) //если новых точек нет, а только передвигали старые, то очищаем старые маршруты
                    fromToOverlay.Routes.Clear();

                //построение маршрута
                TrackFile rt = PathRoute(
                    Vars.Options.Services.PathRouteProvider,
                    fromPoint,
                    toPoint,
                    (IntermediatePoints == null) ? new TrackFile() : IntermediatePoints
                    );

                if (rt == null) //если ошибка, то очищаем слой и точки
                {
                    fromToOverlay.Markers.Clear();
                    fromToOverlay.Routes.Clear();
                    fromPoint = null;
                    toPoint = null;
                    IntermediatePoints = null;
                    return;
                }

                rt.Name = "Новый маршрут";
                if (Vars.Options.Services.ChangePathedRoute)
                {
                    //если надо открыть маршрут для редактирования
                    BeginEditRoute(rt, (tf) => { Program.winConverter.EndEditRoute(tf); }, () =>
                    {
                        for (int i = 0; i < fromToOverlay.Markers.Count;)
                        {
                            object o = fromToOverlay.Markers[i];
                            MapMarker m = o as MapMarker;
                            if (m.Tag.Type == MarkerTypes.PathingRoute)
                                fromToOverlay.Markers.Remove(o as GMapMarker);
                            else i++;
                        }
                    });
                }
                else
                {
                    //если не надо открывать мршрут
                    tracks.Add(rt);
                    Program.winConverter.AddRouteToList(rt);
                    ShowRoute(rt, fromToOverlay);
                }
            }
        }

        /// <summary>
        /// прокладка маршрута по заданным данным
        /// </summary>
        /// <param name="pathRouteProvider">сервис прокладки маршрута</param>
        /// <param name="from">откуда</param>
        /// <param name="waypoints">промежуточные точки</param>
        /// <param name="to">куда</param>
        private static TrackFile PathRoute(PathRouteProvider pathRouteProvider, TrackPoint from, TrackPoint to, TrackFile waypoints)
        {
            if (from == null || to == null)
                throw new ArgumentNullException("Начальная или конечная точка не заданы");
            try
            {
                GeoRouter gr = new GeoRouter(pathRouteProvider);
                TrackFile route = gr.CreateRoute(from.Coordinates, to.Coordinates, waypoints);
                if (route == null)
                    throw new Exception("Что-то пошло не так...");
                route.Color = Vars.Options.Converter.GetColor();
                route.CalculateAll();
                return route;
            }
            catch (Exception ex)
            {
                MessageBox.Show(null, ex.Message, "Ошибка при поиске маршрута", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion

        #region взаимодействие

        /// <summary>
        /// показать заданный маршрут. Если маршрута нет в списке машрутов карты то добавляет в общий список
        /// </summary>
        /// <param name="route">маршрут</param>
        public void ShowRoute(TrackFile route)
        {
            ShowRoute(route, baseOverlay);
        }

        /// <summary>
        /// показать маршрут на указанном слое
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="fromToOverlay">слой</param>
        private void ShowRoute(TrackFile route, GMapOverlay fromToOverlay)
        {
            if (route == null)
                throw new ArgumentNullException("route");
            if (tracks == null)
                tracks = new TrackFileList();
            if (!tracks.Contains(route))
                tracks.Add(route);
            ShowTrack(route, baseOverlay);
        }

        /// <summary>
        /// показать заданную точку
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="addToWaypoint">еси истина, то точка будет добавлена к путевым точкам</param>
        public void ShowPoint(TrackPoint point, bool addToWaypoint)
        {
            if (point == null)
                throw new ArgumentNullException("point");
            if (addToWaypoint)
                waypoints.Add(point);
            ShowWaypoint(point, baseOverlay);
        }

        /// <summary>
        /// выделить определенную точку на карте. 
        /// Перед выделением новой точки предыдущая выделенная точка будет удалена
        /// </summary>
        /// <param name="point">точка, которую надо выделить</param>
        public void SelectPoint(TrackPoint point)
        {
            if (point == null)
                throw new ArgumentNullException("point");
            if (selectedPointsOverlay == null)
            {
                selectedPointsOverlay = new GMapOverlay();
            }

            selectedPointsOverlay.Markers.Clear();
            point.Icon = 75;
            ShowWaypoint(point, selectedPointsOverlay, Resources.selected_point, MarkerTypes.SelectedPoint);

            //центр на точку
            gmapControlMap.Position = point.Coordinates.GMap;
        }

        /// <summary>
        /// отменить выделение всех точек
        /// </summary>
        public void DeselectPoints()
        {
            if (selectedPointsOverlay != null)
                selectedPointsOverlay.Markers.Clear();

        }

        /// <summary>
        /// показать путевые точки на карте
        /// </summary>
        /// <param name="points">точки для показа</param>
        /// <param name="addToWayPoints">если true , то точки будут добавлены в список путевых точек</param>
        public void ShowPoints(TrackFile points, bool addToWayPoints)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (waypoints == null && addToWayPoints)
                waypoints = new TrackFile();

            if (addToWayPoints)
                waypoints.Add(points);

            ShowWaypoints(points, baseOverlay, false);
            gmapControlMap.ZoomAndCenterMarkers(baseOverlay.Id);
        }

        /// <summary>
        /// редактирование мршрута
        /// </summary>
        /// <param name="trackFile">редактируемый маршрут</param>
        /// <param name="afterAction">Действие, выполняемое после нажатия кнопки сохранить</param>
        /// <param name="cancelAction">Действие, выполняемое после нажатия кнопки отменить или закрытии окна</param>
        public void BeginEditRoute(TrackFile trackFile, Action<TrackFile> afterAction, Action cancelAction = null)
        {
            if (trackFile == null)
                throw new ArgumentNullException("trackFile не может быть null в FormMap.BeginEditRoute()");

            gmapControlMap.DragButton = System.Windows.Forms.MouseButtons.Right;


            //если идет сздание маршрута, то прерываем
            if (isCreatingRoute)
                Program.winRouteEditor.Close();

            //если заднный для редактирования маршрут уже есть на карте, то удаляем
            if (tracks != null && tracks.Contains(trackFile))
            {
                tracks.Remove(trackFile);
                ShowTracks(this.tracks, true);
            }

            //открываем маршрут для редактирования
            creatingRoute = trackFile;
            creatingRouteOverlay.Clear();
            isCreatingRoute = true;
            Program.winRouteEditor = new FormEditRoute(
                "Редактирование маршрута",
                this.creatingRoute,
                this.creatingRouteOverlay,
                afterAction,
                cancelAction
                );
            Program.winRouteEditor.Show(this);
            ShowCreatingRoute(creatingRouteOverlay, creatingRoute);
            RefreshToolTipsCreatingRoute(creatingRouteOverlay);
            gmapControlMap.ZoomAndCenterRoute(creatingRouteOverlay.Routes[0]);
        }

        /// <summary>
        /// удаление трека с карты
        /// </summary>
        /// <param name="trackFile">трек для удаления</param>
        public void RemoveTrack(TrackFile trackFile)
        {
            if (trackFile == null)
                throw new ArgumentNullException("trackFile");
            if (tracks != null)
            {
                tracks.Remove(trackFile);
                ShowTracks(this.tracks, true);
            }
        }

        /// <summary>
        /// завершение редактирования путевых точек
        /// </summary>
        /// <param name="wpts"></param>
        public void EndEditWaypoints(TrackFile wpts)
        {
            waypoints = wpts;
            ShowPoints(waypoints, false);
        }

        /// <summary>
        /// очистка карты
        /// </summary>
        public void Clear()
        {
            if (baseOverlay != null)
                baseOverlay.Clear();
            if (creatingRouteOverlay != null)
                creatingRouteOverlay.Clear();
            if (tracks != null)
                tracks.Clear();
            if (waypoints != null)
                waypoints.Clear();
            if (selectedRouteOverlay != null)
                selectedRouteOverlay.Clear();
        }

        /// <summary>
        /// закрытие окна и сохранение маршрутов. Возвращает true, если требуется остановить зарытие
        /// </summary>
        /// <param name="formMain"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool TryClose(object formMain, FormClosingEventArgs e)
        {
            if (isCreatingRoute)
                if (MessageBox.Show(this, "Идет создание маршрута, вы действительно хотите выйти?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    e.Cancel = true;
                    return true;
                }

            GeoFile gf = new GeoFile(tracks, waypoints);
            Serializer.Serialize(Application.StartupPath + Resources.saveLast_file, gf, FileFormats.KmlFile);

            return false;
        }

        /// <summary>
        /// обновление данных выделенного трека
        /// </summary>
        public void RefreshData()
        {
            DeselectPoints();
            selectedRouteOverlay.Clear();
            if (Vars.currentSelectedTrack != null)
                ShowTrack(Vars.currentSelectedTrack, selectedRouteOverlay);
        }

        #endregion

    }
}