using System;
using GMap.NET.WindowsForms;

namespace TrackConverter.UI.Map
{
    partial class FormMap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                try
                {
                    refreshGoToTimer.Dispose();
                    moveMapTimer.Dispose();
                    creatingRouteOverlay.Dispose();
                    baseOverlay.Dispose();
                    components.Dispose();
                    rulerRouteOverlay.Dispose();
                    fromToOverlay.Dispose();
                    selectedPointsOverlay.Dispose();
                    selectedRouteOverlay.Dispose();
                }
                catch (Exception) { }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMap));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ааToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileWaypointsRoutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileWaypointsRoutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadWaypointsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.картаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapProviderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.источникДанныхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmInternetCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.очисткаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRoutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMarkersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.окноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNavigatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.инструментыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.путевыеТочкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pointsToRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routeToPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elevationGraphRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createOptimalRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gmapControlMap = new GMapControl();
            this.contextMenuStripMap = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAddWaypoint = new System.Windows.Forms.ToolStripMenuItem();
            this.созданиеМаршрутаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intermediatePointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearFromtoMarkersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWhatsThere = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonMapProvider = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripComboBoxGoTo = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLat = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLon = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFromStart = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelAltitude = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStripMarker = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelZoom = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRuler = new System.Windows.Forms.ToolStripButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripRoute = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStripMap.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripMarker.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.contextMenuStripRoute.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ааToolStripMenuItem,
            this.картаToolStripMenuItem,
            this.окноToolStripMenuItem,
            this.инструментыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(923, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ааToolStripMenuItem
            // 
            this.ааToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьToolStripMenuItem,
            this.создатьToolStripMenuItem,
            this.открытьToolStripMenuItem});
            this.ааToolStripMenuItem.Name = "ааToolStripMenuItem";
            this.ааToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.ааToolStripMenuItem.Text = "Файл";
            // 
            // сохранитьToolStripMenuItem
            // 
            this.сохранитьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFileWaypointsToolStripMenuItem,
            this.saveFileWaypointsRoutesToolStripMenuItem});
            this.сохранитьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("сохранитьToolStripMenuItem.Image")));
            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.сохранитьToolStripMenuItem.Text = "Сохранить";
            // 
            // saveFileWaypointsToolStripMenuItem
            // 
            this.saveFileWaypointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveFileWaypointsToolStripMenuItem.Image")));
            this.saveFileWaypointsToolStripMenuItem.Name = "saveFileWaypointsToolStripMenuItem";
            this.saveFileWaypointsToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.saveFileWaypointsToolStripMenuItem.Text = "Путевые точки в файл";
            this.saveFileWaypointsToolStripMenuItem.Click += new System.EventHandler(this.saveFileWaypointsToolStripMenuItem_Click);
            // 
            // saveFileWaypointsRoutesToolStripMenuItem
            // 
            this.saveFileWaypointsRoutesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveFileWaypointsRoutesToolStripMenuItem.Image")));
            this.saveFileWaypointsRoutesToolStripMenuItem.Name = "saveFileWaypointsRoutesToolStripMenuItem";
            this.saveFileWaypointsRoutesToolStripMenuItem.Size = new System.Drawing.Size(270, 22);
            this.saveFileWaypointsRoutesToolStripMenuItem.Text = "Путевые точки и маршруты в файл";
            this.saveFileWaypointsRoutesToolStripMenuItem.ToolTipText = "Сохранение все маршрутов и путевых точек в один файл";
            this.saveFileWaypointsRoutesToolStripMenuItem.Click += new System.EventHandler(this.saveFileWaypointsRoutesToolStripMenuItem_Click);
            // 
            // создатьToolStripMenuItem
            // 
            this.создатьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createRouteToolStripMenuItem,
            this.createTripToolStripMenuItem});
            this.создатьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("создатьToolStripMenuItem.Image")));
            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.создатьToolStripMenuItem.Text = "Создать";
            // 
            // createRouteToolStripMenuItem
            // 
            this.createRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createRouteToolStripMenuItem.Image")));
            this.createRouteToolStripMenuItem.Name = "createRouteToolStripMenuItem";
            this.createRouteToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.createRouteToolStripMenuItem.Text = "Простой маршрут";
            this.createRouteToolStripMenuItem.Click += new System.EventHandler(this.createRouteToolStripMenuItem_Click);
            // 
            // createTripToolStripMenuItem
            // 
            this.createTripToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createTripToolStripMenuItem.Image")));
            this.createTripToolStripMenuItem.Name = "createTripToolStripMenuItem";
            this.createTripToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.createTripToolStripMenuItem.Text = "Путешествие";
            this.createTripToolStripMenuItem.Click += new System.EventHandler(this.createTripToolStripMenuItem_Click);
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileWaypointsRoutesToolStripMenuItem,
            this.loadWaypointsToolStripMenuItem1});
            this.открытьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("открытьToolStripMenuItem.Image")));
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.открытьToolStripMenuItem.Text = "Открыть ";
            // 
            // openFileWaypointsRoutesToolStripMenuItem
            // 
            this.openFileWaypointsRoutesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openFileWaypointsRoutesToolStripMenuItem.Image")));
            this.openFileWaypointsRoutesToolStripMenuItem.Name = "openFileWaypointsRoutesToolStripMenuItem";
            this.openFileWaypointsRoutesToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.openFileWaypointsRoutesToolStripMenuItem.Text = "Путевые точки и маршруты";
            this.openFileWaypointsRoutesToolStripMenuItem.Click += new System.EventHandler(this.openFileWaypointsRoutesToolStripMenuItem_Click);
            // 
            // loadWaypointsToolStripMenuItem1
            // 
            this.loadWaypointsToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("loadWaypointsToolStripMenuItem1.Image")));
            this.loadWaypointsToolStripMenuItem1.Name = "loadWaypointsToolStripMenuItem1";
            this.loadWaypointsToolStripMenuItem1.Size = new System.Drawing.Size(229, 22);
            this.loadWaypointsToolStripMenuItem1.Text = "Путевые точки";
            this.loadWaypointsToolStripMenuItem1.Click += new System.EventHandler(this.loadWaypointsToolStripMenuItem1_Click);
            // 
            // картаToolStripMenuItem
            // 
            this.картаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapProviderToolStripMenuItem,
            this.источникДанныхToolStripMenuItem,
            this.selectMapToolStripMenuItem,
            this.очисткаToolStripMenuItem});
            this.картаToolStripMenuItem.Name = "картаToolStripMenuItem";
            this.картаToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.картаToolStripMenuItem.Text = "Карта";
            // 
            // mapProviderToolStripMenuItem
            // 
            this.mapProviderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mapProviderToolStripMenuItem.Image")));
            this.mapProviderToolStripMenuItem.Name = "mapProviderToolStripMenuItem";
            this.mapProviderToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.mapProviderToolStripMenuItem.Text = "Поставщик карты";
            // 
            // источникДанныхToolStripMenuItem
            // 
            this.источникДанныхToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmInternetToolStripMenuItem,
            this.tmCacheToolStripMenuItem,
            this.tmInternetCacheToolStripMenuItem});
            this.источникДанныхToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("источникДанныхToolStripMenuItem.Image")));
            this.источникДанныхToolStripMenuItem.Name = "источникДанныхToolStripMenuItem";
            this.источникДанныхToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.источникДанныхToolStripMenuItem.Text = "Источник данных";
            // 
            // tmInternetToolStripMenuItem
            // 
            this.tmInternetToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tmInternetToolStripMenuItem.Image")));
            this.tmInternetToolStripMenuItem.Name = "tmInternetToolStripMenuItem";
            this.tmInternetToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.tmInternetToolStripMenuItem.Text = "Интернет";
            this.tmInternetToolStripMenuItem.Click += new System.EventHandler(this.tmInternetToolStripMenuItem_Click);
            // 
            // tmCacheToolStripMenuItem
            // 
            this.tmCacheToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tmCacheToolStripMenuItem.Image")));
            this.tmCacheToolStripMenuItem.Name = "tmCacheToolStripMenuItem";
            this.tmCacheToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.tmCacheToolStripMenuItem.Text = "Кэш";
            this.tmCacheToolStripMenuItem.Click += new System.EventHandler(this.tmCacheToolStripMenuItem_Click);
            // 
            // tmInternetCacheToolStripMenuItem
            // 
            this.tmInternetCacheToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("tmInternetCacheToolStripMenuItem.Image")));
            this.tmInternetCacheToolStripMenuItem.Name = "tmInternetCacheToolStripMenuItem";
            this.tmInternetCacheToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.tmInternetCacheToolStripMenuItem.Text = "Интернет+Кэш";
            this.tmInternetCacheToolStripMenuItem.Click += new System.EventHandler(this.tmInternetCacheToolStripMenuItem_Click);
            // 
            // selectMapToolStripMenuItem
            // 
            this.selectMapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("selectMapToolStripMenuItem.Image")));
            this.selectMapToolStripMenuItem.Name = "selectMapToolStripMenuItem";
            this.selectMapToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.selectMapToolStripMenuItem.Text = "Сохранить карту";
            this.selectMapToolStripMenuItem.Click += new System.EventHandler(this.selectMapToolStripMenuItem_Click);
            // 
            // очисткаToolStripMenuItem
            // 
            this.очисткаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearRoutesToolStripMenuItem,
            this.clearMarkersToolStripMenuItem,
            this.clearAllToolStripMenuItem});
            this.очисткаToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("очисткаToolStripMenuItem.Image")));
            this.очисткаToolStripMenuItem.Name = "очисткаToolStripMenuItem";
            this.очисткаToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.очисткаToolStripMenuItem.Text = "Очистка";
            // 
            // clearRoutesToolStripMenuItem
            // 
            this.clearRoutesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearRoutesToolStripMenuItem.Image")));
            this.clearRoutesToolStripMenuItem.Name = "clearRoutesToolStripMenuItem";
            this.clearRoutesToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.clearRoutesToolStripMenuItem.Text = "Очистить маршруты";
            this.clearRoutesToolStripMenuItem.Click += new System.EventHandler(this.clearRoutesToolStripMenuItem_Click);
            // 
            // clearMarkersToolStripMenuItem
            // 
            this.clearMarkersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearMarkersToolStripMenuItem.Image")));
            this.clearMarkersToolStripMenuItem.Name = "clearMarkersToolStripMenuItem";
            this.clearMarkersToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.clearMarkersToolStripMenuItem.Text = "Очистить маркеры";
            this.clearMarkersToolStripMenuItem.Click += new System.EventHandler(this.clearMarkersToolStripMenuItem_Click);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.clearAllToolStripMenuItem.Text = "Очистить всё";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // окноToolStripMenuItem
            // 
            this.окноToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showNavigatorToolStripMenuItem});
            this.окноToolStripMenuItem.Name = "окноToolStripMenuItem";
            this.окноToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.окноToolStripMenuItem.Text = "Окно";
            // 
            // showNavigatorToolStripMenuItem
            // 
            this.showNavigatorToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showNavigatorToolStripMenuItem.Image")));
            this.showNavigatorToolStripMenuItem.Name = "showNavigatorToolStripMenuItem";
            this.showNavigatorToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.showNavigatorToolStripMenuItem.Text = "Панель навигации";
            this.showNavigatorToolStripMenuItem.Click += new System.EventHandler(this.showNavigatorToolStripMenuItem_Click);
            // 
            // инструментыToolStripMenuItem
            // 
            this.инструментыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.путевыеТочкиToolStripMenuItem,
            this.editWaypointsToolStripMenuItem,
            this.elevationGraphRouteToolStripMenuItem,
            this.createOptimalRouteToolStripMenuItem});
            this.инструментыToolStripMenuItem.Name = "инструментыToolStripMenuItem";
            this.инструментыToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.инструментыToolStripMenuItem.Text = "Инструменты";
            // 
            // путевыеТочкиToolStripMenuItem
            // 
            this.путевыеТочкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pointsToRouteToolStripMenuItem,
            this.routeToPointsToolStripMenuItem});
            this.путевыеТочкиToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("путевыеТочкиToolStripMenuItem.Image")));
            this.путевыеТочкиToolStripMenuItem.Name = "путевыеТочкиToolStripMenuItem";
            this.путевыеТочкиToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.путевыеТочкиToolStripMenuItem.Text = "Преобразование";
            // 
            // pointsToRouteToolStripMenuItem
            // 
            this.pointsToRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pointsToRouteToolStripMenuItem.Image")));
            this.pointsToRouteToolStripMenuItem.Name = "pointsToRouteToolStripMenuItem";
            this.pointsToRouteToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.pointsToRouteToolStripMenuItem.Text = "Точки в маршрут";
            this.pointsToRouteToolStripMenuItem.ToolTipText = "Преобразовать путевые точки в маршрут";
            this.pointsToRouteToolStripMenuItem.Click += new System.EventHandler(this.pointsToRouteToolStripMenuItem_Click);
            // 
            // routeToPointsToolStripMenuItem
            // 
            this.routeToPointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("routeToPointsToolStripMenuItem.Image")));
            this.routeToPointsToolStripMenuItem.Name = "routeToPointsToolStripMenuItem";
            this.routeToPointsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.routeToPointsToolStripMenuItem.Text = "Маршрут в точки";
            this.routeToPointsToolStripMenuItem.ToolTipText = "Преобразовать все точки всех маршрутов в путевые точки";
            this.routeToPointsToolStripMenuItem.Click += new System.EventHandler(this.routeToPointsToolStripMenuItem_Click);
            // 
            // editWaypointsToolStripMenuItem
            // 
            this.editWaypointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editWaypointsToolStripMenuItem.Image")));
            this.editWaypointsToolStripMenuItem.Name = "editWaypointsToolStripMenuItem";
            this.editWaypointsToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.editWaypointsToolStripMenuItem.Text = "Открыть путевые точки в редакторе";
            this.editWaypointsToolStripMenuItem.ToolTipText = "Открыть в редакторе пктевых точек";
            this.editWaypointsToolStripMenuItem.Click += new System.EventHandler(this.editWaypointsToolStripMenuItem_Click);
            // 
            // elevationGraphRouteToolStripMenuItem
            // 
            this.elevationGraphRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("elevationGraphRouteToolStripMenuItem.Image")));
            this.elevationGraphRouteToolStripMenuItem.Name = "elevationGraphRouteToolStripMenuItem";
            this.elevationGraphRouteToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.elevationGraphRouteToolStripMenuItem.Text = "Построить профиль высот по пути";
            this.elevationGraphRouteToolStripMenuItem.Click += new System.EventHandler(this.elevationGraphRouteToolStripMenuItem_Click);
            this.elevationGraphRouteToolStripMenuItem.Paint += new System.Windows.Forms.PaintEventHandler(this.elevationGraphRouteToolStripMenuItem_Paint);
            // 
            // createOptimalRouteToolStripMenuItem
            // 
            this.createOptimalRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createOptimalRouteToolStripMenuItem.Image")));
            this.createOptimalRouteToolStripMenuItem.Name = "createOptimalRouteToolStripMenuItem";
            this.createOptimalRouteToolStripMenuItem.Size = new System.Drawing.Size(272, 22);
            this.createOptimalRouteToolStripMenuItem.Text = "Построить оптимальный маршрут";
            this.createOptimalRouteToolStripMenuItem.ToolTipText = "Построить кратчайший маршрут через все путевые точки. При этом в каждую точку мож" +
    "но попасть только один раз";
            this.createOptimalRouteToolStripMenuItem.Click += new System.EventHandler(this.createOptimalRouteToolStripMenuItem_Click);
            // 
            // gmapControlMapCunstructorField
            // 
            this.gmapControlMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gmapControlMap.Bearing = 0F;
            this.gmapControlMap.CanDragMap = true;
            this.gmapControlMap.Cursor = System.Windows.Forms.Cursors.Default;
            this.gmapControlMap.EmptyTileColor = System.Drawing.Color.Lavender;
            this.gmapControlMap.GrayScaleMode = false;
            this.gmapControlMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmapControlMap.LevelsKeepInMemmory = 5;
            this.gmapControlMap.Location = new System.Drawing.Point(0, 24);
            this.gmapControlMap.MarkersEnabled = true;
            this.gmapControlMap.MaxZoom = 23;
            this.gmapControlMap.MinZoom = 2;
            this.gmapControlMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gmapControlMap.Name = "gmapControlMap";
            this.gmapControlMap.NegativeMode = false;
            this.gmapControlMap.PolygonsEnabled = true;
            this.gmapControlMap.RetryLoadTile = 0;
            this.gmapControlMap.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gmapControlMap.RoutesEnabled = true;
            this.gmapControlMap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Fractional;
            this.gmapControlMap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmapControlMap.ShowTileGridLines = false;
            this.gmapControlMap.Size = new System.Drawing.Size(923, 604);
            this.gmapControlMap.TabIndex = 0;
            this.gmapControlMap.Zoom = 5D;
            this.gmapControlMap.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gmapControlMap_OnMarkerClick);
            this.gmapControlMap.OnRouteClick += new GMap.NET.WindowsForms.RouteClick(this.gmapControlMap_OnRouteClick);
            this.gmapControlMap.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.gmapControlMap_OnMarkerEnter);
            this.gmapControlMap.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.gmapControlMap_OnMarkerLeave);
            this.gmapControlMap.OnPositionChanged += new GMap.NET.PositionChanged(this.gmapControlMap_OnPositionChanged);
            this.gmapControlMap.OnMapDrag += new GMap.NET.MapDrag(this.gmapControlMap_OnMapDrag);
            this.gmapControlMap.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gmapControlMap_OnMapZoomChanged);
            this.gmapControlMap.OnMapTypeChanged += new GMap.NET.MapTypeChanged(this.gmapControlMap_OnMapTypeChanged);
            this.gmapControlMap.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMap_KeyUp);
            this.gmapControlMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gmapControlMap_MouseClick);
            this.gmapControlMap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gmapControlMap_MouseDoubleClick);
            this.gmapControlMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gmapControlMap_MouseDown);
            this.gmapControlMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gmapControlMap_MouseMove);
            this.gmapControlMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gmapControlMap_MouseUp);
            // 
            // contextMenuStripMap
            // 
            this.contextMenuStripMap.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAddWaypoint,
            this.созданиеМаршрутаToolStripMenuItem,
            this.toolStripMenuItemWhatsThere,
            this.copyCoordinatesToolStripMenuItem});
            this.contextMenuStripMap.Name = "contextMenuStripMap";
            this.contextMenuStripMap.Size = new System.Drawing.Size(210, 92);
            this.contextMenuStripMap.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMap_Opening);
            // 
            // toolStripMenuItemAddWaypoint
            // 
            this.toolStripMenuItemAddWaypoint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.toolStripMenuItemAddWaypoint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemAddWaypoint.Image")));
            this.toolStripMenuItemAddWaypoint.Name = "toolStripMenuItemAddWaypoint";
            this.toolStripMenuItemAddWaypoint.Size = new System.Drawing.Size(209, 22);
            this.toolStripMenuItemAddWaypoint.Text = "Добавить точку";
            this.toolStripMenuItemAddWaypoint.Click += new System.EventHandler(this.toolStripMenuItemAddWaypoint_Click);
            // 
            // созданиеМаршрутаToolStripMenuItem
            // 
            this.созданиеМаршрутаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromToolStripMenuItem,
            this.intermediatePointToolStripMenuItem,
            this.toToolStripMenuItem,
            this.clearFromtoMarkersToolStripMenuItem});
            this.созданиеМаршрутаToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("созданиеМаршрутаToolStripMenuItem.Image")));
            this.созданиеМаршрутаToolStripMenuItem.Name = "созданиеМаршрутаToolStripMenuItem";
            this.созданиеМаршрутаToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.созданиеМаршрутаToolStripMenuItem.Text = "Создание маршрута";
            // 
            // fromToolStripMenuItem
            // 
            this.fromToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fromToolStripMenuItem.Image")));
            this.fromToolStripMenuItem.Name = "fromToolStripMenuItem";
            this.fromToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.fromToolStripMenuItem.Tag = "from";
            this.fromToolStripMenuItem.Text = "Отсюда";
            this.fromToolStripMenuItem.Click += new System.EventHandler(this.createRoutePathingToolStripMenuItem_Click);
            // 
            // intermediatePointToolStripMenuItem
            // 
            this.intermediatePointToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("intermediatePointToolStripMenuItem.Image")));
            this.intermediatePointToolStripMenuItem.Name = "intermediatePointToolStripMenuItem";
            this.intermediatePointToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.intermediatePointToolStripMenuItem.Tag = "intermediate";
            this.intermediatePointToolStripMenuItem.Text = "Промежуточная точка";
            this.intermediatePointToolStripMenuItem.Click += new System.EventHandler(this.createRoutePathingToolStripMenuItem_Click);
            // 
            // toToolStripMenuItem
            // 
            this.toToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("toToolStripMenuItem.Image")));
            this.toToolStripMenuItem.Name = "toToolStripMenuItem";
            this.toToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.toToolStripMenuItem.Tag = "to";
            this.toToolStripMenuItem.Text = "Сюда";
            this.toToolStripMenuItem.Click += new System.EventHandler(this.createRoutePathingToolStripMenuItem_Click);
            // 
            // clearFromtoMarkersToolStripMenuItem
            // 
            this.clearFromtoMarkersToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("clearFromtoMarkersToolStripMenuItem.Image")));
            this.clearFromtoMarkersToolStripMenuItem.Name = "clearFromtoMarkersToolStripMenuItem";
            this.clearFromtoMarkersToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.clearFromtoMarkersToolStripMenuItem.Text = "Сброс маркеров";
            this.clearFromtoMarkersToolStripMenuItem.Click += new System.EventHandler(this.clearFromtoMarkersToolStripMenuItem_Click);
            // 
            // toolStripMenuItemWhatsThere
            // 
            this.toolStripMenuItemWhatsThere.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItemWhatsThere.Image")));
            this.toolStripMenuItemWhatsThere.Name = "toolStripMenuItemWhatsThere";
            this.toolStripMenuItemWhatsThere.Size = new System.Drawing.Size(209, 22);
            this.toolStripMenuItemWhatsThere.Text = "Что здесь?";
            this.toolStripMenuItemWhatsThere.Click += new System.EventHandler(this.toolStripMenuItemWhatsThere_Click);
            // 
            // copyCoordinatesToolStripMenuItem
            // 
            this.copyCoordinatesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyCoordinatesToolStripMenuItem.Image")));
            this.copyCoordinatesToolStripMenuItem.Name = "copyCoordinatesToolStripMenuItem";
            this.copyCoordinatesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.copyCoordinatesToolStripMenuItem.Text = "Копировать координаты";
            this.copyCoordinatesToolStripMenuItem.Click += new System.EventHandler(this.copyCoordinatesToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonUndo,
            this.toolStripDropDownButtonMapProvider,
            this.toolStripComboBoxGoTo,
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(923, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Enabled = false;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "toolStripButton1";
            this.toolStripButtonUndo.ToolTipText = "Отменить последнее действие";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // toolStripDropDownButtonMapProvider
            // 
            this.toolStripDropDownButtonMapProvider.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonMapProvider.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonMapProvider.Image")));
            this.toolStripDropDownButtonMapProvider.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonMapProvider.Name = "toolStripDropDownButtonMapProvider";
            this.toolStripDropDownButtonMapProvider.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonMapProvider.Text = "Поставщик карты";
            this.toolStripDropDownButtonMapProvider.ToolTipText = "Выбор источника карты";
            // 
            // toolStripComboBoxGoTo
            // 
            this.toolStripComboBoxGoTo.AutoSize = false;
            this.toolStripComboBoxGoTo.AutoToolTip = true;
            this.toolStripComboBoxGoTo.CausesValidation = false;
            this.toolStripComboBoxGoTo.DropDownWidth = 200;
            this.toolStripComboBoxGoTo.MaxDropDownItems = 10;
            this.toolStripComboBoxGoTo.Name = "toolStripComboBoxGoTo";
            this.toolStripComboBoxGoTo.Size = new System.Drawing.Size(250, 23);
            this.toolStripComboBoxGoTo.ToolTipText = "Введите адрес";
            this.toolStripComboBoxGoTo.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxGoTo_SelectedIndexChanged);
            this.toolStripComboBoxGoTo.TextChanged += new System.EventHandler(this.toolStripComboBoxGoTo_TextChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(71, 22);
            this.toolStripLabel1.Text = "Поиск мест";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLat,
            this.toolStripStatusLabelLon,
            this.toolStripStatusLabelInfo,
            this.toolStripStatusLabelFromStart,
            this.toolStripStatusLabelAltitude});
            this.statusStrip1.Location = new System.Drawing.Point(0, 606);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(923, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLat
            // 
            this.toolStripStatusLabelLat.Name = "toolStripStatusLabelLat";
            this.toolStripStatusLabelLat.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabelLat.Text = "Широта";
            // 
            // toolStripStatusLabelLon
            // 
            this.toolStripStatusLabelLon.Name = "toolStripStatusLabelLon";
            this.toolStripStatusLabelLon.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabelLon.Text = "Долгота";
            // 
            // toolStripStatusLabelInfo
            // 
            this.toolStripStatusLabelInfo.Name = "toolStripStatusLabelInfo";
            this.toolStripStatusLabelInfo.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelFromStart
            // 
            this.toolStripStatusLabelFromStart.Name = "toolStripStatusLabelFromStart";
            this.toolStripStatusLabelFromStart.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabelAltitude
            // 
            this.toolStripStatusLabelAltitude.Name = "toolStripStatusLabelAltitude";
            this.toolStripStatusLabelAltitude.Size = new System.Drawing.Size(0, 17);
            // 
            // contextMenuStripMarker
            // 
            this.contextMenuStripMarker.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMarkerToolStripMenuItem,
            this.deleteMarkerToolStripMenuItem});
            this.contextMenuStripMarker.Name = "contextMenuStripMarker";
            this.contextMenuStripMarker.Size = new System.Drawing.Size(129, 48);
            this.contextMenuStripMarker.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripMarker_Opening);
            // 
            // editMarkerToolStripMenuItem
            // 
            this.editMarkerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editMarkerToolStripMenuItem.Image")));
            this.editMarkerToolStripMenuItem.Name = "editMarkerToolStripMenuItem";
            this.editMarkerToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.editMarkerToolStripMenuItem.Text = "Изменить";
            this.editMarkerToolStripMenuItem.Click += new System.EventHandler(this.editMarkerToolStripMenuItem_Click);
            // 
            // deleteMarkerToolStripMenuItem
            // 
            this.deleteMarkerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("deleteMarkerToolStripMenuItem.Image")));
            this.deleteMarkerToolStripMenuItem.Name = "deleteMarkerToolStripMenuItem";
            this.deleteMarkerToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.deleteMarkerToolStripMenuItem.Text = "Удалить";
            this.deleteMarkerToolStripMenuItem.Click += new System.EventHandler(this.deleteMarkerToolStripMenuItem_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonZoomIn,
            this.toolStripButtonZoomOut,
            this.toolStripLabelZoom,
            this.toolStripSeparator2,
            this.toolStripButtonRuler});
            this.toolStrip2.Location = new System.Drawing.Point(0, 49);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(24, 557);
            this.toolStrip2.TabIndex = 4;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonZoomIn
            // 
            this.toolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomIn.Image")));
            this.toolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomIn.Margin = new System.Windows.Forms.Padding(0, 30, 0, 2);
            this.toolStripButtonZoomIn.Name = "toolStripButtonZoomIn";
            this.toolStripButtonZoomIn.Size = new System.Drawing.Size(21, 20);
            this.toolStripButtonZoomIn.ToolTipText = "Приблизить";
            this.toolStripButtonZoomIn.Click += new System.EventHandler(this.toolStripButtonZoomIn_Click);
            // 
            // toolStripButtonZoomOut
            // 
            this.toolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoomOut.Image")));
            this.toolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoomOut.Name = "toolStripButtonZoomOut";
            this.toolStripButtonZoomOut.Size = new System.Drawing.Size(21, 20);
            this.toolStripButtonZoomOut.ToolTipText = "Отдалить";
            this.toolStripButtonZoomOut.Click += new System.EventHandler(this.toolStripButtonZoomOut_Click);
            // 
            // toolStripLabelZoom
            // 
            this.toolStripLabelZoom.Name = "toolStripLabelZoom";
            this.toolStripLabelZoom.Size = new System.Drawing.Size(21, 0);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(21, 6);
            // 
            // toolStripButtonRuler
            // 
            this.toolStripButtonRuler.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRuler.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRuler.Image")));
            this.toolStripButtonRuler.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRuler.Name = "toolStripButtonRuler";
            this.toolStripButtonRuler.Size = new System.Drawing.Size(21, 20);
            this.toolStripButtonRuler.ToolTipText = "Измерение расстояний";
            this.toolStripButtonRuler.Click += new System.EventHandler(this.toolStripButtonRuler_Click);
            // 
            // contextMenuStripRoute
            // 
            this.contextMenuStripRoute.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditRouteToolStripMenuItem,
            this.RemoveRouteToolStripMenuItem});
            this.contextMenuStripRoute.Name = "contextMenuStripRoute";
            this.contextMenuStripRoute.Size = new System.Drawing.Size(155, 48);
            // 
            // EditRouteToolStripMenuItem
            // 
            this.EditRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("EditRouteToolStripMenuItem.Image")));
            this.EditRouteToolStripMenuItem.Name = "EditRouteToolStripMenuItem";
            this.EditRouteToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.EditRouteToolStripMenuItem.Text = "Редактировать";
            this.EditRouteToolStripMenuItem.Click += new System.EventHandler(this.EditRouteToolStripMenuItem_Click);
            // 
            // RemoveRouteToolStripMenuItem
            // 
            this.RemoveRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("RemoveRouteToolStripMenuItem.Image")));
            this.RemoveRouteToolStripMenuItem.Name = "RemoveRouteToolStripMenuItem";
            this.RemoveRouteToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.RemoveRouteToolStripMenuItem.Text = "Удалить";
            this.RemoveRouteToolStripMenuItem.Click += new System.EventHandler(this.RemoveRouteToolStripMenuItem_Click);
            // 
            // FormMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(923, 628);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gmapControlMap);
            this.Controls.Add(this.menuStrip1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Карта";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMap_FormClosing);
            this.Load += new System.EventHandler(this.FormMap_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMap_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormMap_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStripMap.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripMarker.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.contextMenuStripRoute.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ааToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem картаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapProviderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem источникДанныхToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMap;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWhatsThere;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAddWaypoint;
        private System.Windows.Forms.ToolStripMenuItem saveFileWaypointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem создатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLat;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLon;


        private System.Windows.Forms.ToolStripMenuItem tmInternetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmInternetCacheToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMarker;
        private System.Windows.Forms.ToolStripMenuItem editMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem очисткаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearRoutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMarkersToolStripMenuItem;
        /// <summary>
        /// надпись информации
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelInfo;
        private System.Windows.Forms.ToolStripMenuItem saveFileWaypointsRoutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonMapProvider;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFromStart;
        private System.Windows.Forms.ToolStripMenuItem copyCoordinatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileWaypointsRoutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAltitude;
        private System.Windows.Forms.ToolStripMenuItem инструментыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elevationGraphRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomIn;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoomOut;
        private System.Windows.Forms.ToolStripLabel toolStripLabelZoom;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem loadWaypointsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem путевыеТочкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pointsToRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routeToPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWaypointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createOptimalRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonRuler;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        /// <summary>
        /// поле ввода запроса для поиска на карте
        /// </summary>
        public System.Windows.Forms.ToolStripComboBox toolStripComboBoxGoTo;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripMenuItem окноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showNavigatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;

        /// <summary>
        /// область карты GmapControl
        /// </summary>
        public GMapControl gmapControlMap;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRoute;
        private System.Windows.Forms.ToolStripMenuItem EditRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem созданиеМаршрутаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem intermediatePointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFromtoMarkersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTripToolStripMenuItem;
    }
}