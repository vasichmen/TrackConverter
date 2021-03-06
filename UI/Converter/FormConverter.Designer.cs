﻿namespace TrackConverter.UI.Converter
{
    partial class FormConverter
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
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConverter));
            this.ClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelProgress = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenYandexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveYandexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveWikimapiaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAllSeparateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllInOneFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewConverter = new System.Windows.Forms.DataGridView();
            this.contextMenuStripDGW = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileContextToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveYandexContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveWikimapiaContextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.изменитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRouteMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadElevationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeElevationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.approximateAltitudesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalizeTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toTripRouteFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinToTripRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.показатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOnMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elevgraphWithIntermediatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elevgraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRouteFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToJoinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addComparisonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConverter)).BeginInit();
            this.contextMenuStripDGW.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClearToolStripMenuItem
            // 
            this.ClearToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ClearToolStripMenuItem.Image")));
            this.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            this.ClearToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.ClearToolStripMenuItem.Text = "Очистить";
            this.ClearToolStripMenuItem.ToolTipText = "Очистить список загруженных маршрутов";
            this.ClearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(12, 368);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(0, 13);
            this.labelProgress.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ClearToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(294, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.загрузитьToolStripMenuItem,
            this.SaveToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileToolStripMenuItem.Text = "Файл";
            // 
            // загрузитьToolStripMenuItem
            // 
            this.загрузитьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.OpenYandexToolStripMenuItem});
            this.загрузитьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("загрузитьToolStripMenuItem.Image")));
            this.загрузитьToolStripMenuItem.Name = "загрузитьToolStripMenuItem";
            this.загрузитьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.загрузитьToolStripMenuItem.Text = "Загрузить";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuItem1.Text = "Файл маршрута";
            this.toolStripMenuItem1.ToolTipText = "Загрузка любого поддерживаемого файла маршрута";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.OpenRouteFileToolStripMenuItem_Click);
            // 
            // OpenYandexToolStripMenuItem
            // 
            this.OpenYandexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenYandexToolStripMenuItem.Image")));
            this.OpenYandexToolStripMenuItem.Name = "OpenYandexToolStripMenuItem";
            this.OpenYandexToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.OpenYandexToolStripMenuItem.Text = "Ссылка";
            this.OpenYandexToolStripMenuItem.ToolTipText = "Загрузка маршрута из ссылки поддерживаемых сервисов";
            this.OpenYandexToolStripMenuItem.Click += new System.EventHandler(this.OpenLinkToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveFileToolStripMenuItem,
            this.SaveYandexToolStripMenuItem,
            this.SaveWikimapiaToolStripMenuItem,
            this.SaveAllSeparateToolStripMenuItem,
            this.saveAllInOneFileToolStripMenuItem});
            this.SaveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveToolStripMenuItem.Image")));
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.SaveToolStripMenuItem.Text = "Сохранить";
            // 
            // SaveFileToolStripMenuItem
            // 
            this.SaveFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveFileToolStripMenuItem.Image")));
            this.SaveFileToolStripMenuItem.Name = "SaveFileToolStripMenuItem";
            this.SaveFileToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.SaveFileToolStripMenuItem.Text = "Файл";
            this.SaveFileToolStripMenuItem.ToolTipText = "Сохранить один маршрут в файл";
            this.SaveFileToolStripMenuItem.Click += new System.EventHandler(this.SaveFileToolStripMenuItem_Click);
            // 
            // SaveYandexToolStripMenuItem
            // 
            this.SaveYandexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveYandexToolStripMenuItem.Image")));
            this.SaveYandexToolStripMenuItem.Name = "SaveYandexToolStripMenuItem";
            this.SaveYandexToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.SaveYandexToolStripMenuItem.Text = "Яндекс - сслыка";
            this.SaveYandexToolStripMenuItem.ToolTipText = "Сохранить один маршрут как ссылку на Яндекс.Карты";
            this.SaveYandexToolStripMenuItem.Click += new System.EventHandler(this.SaveYandexToolStripMenuItem_Click);
            // 
            // SaveWikimapiaToolStripMenuItem
            // 
            this.SaveWikimapiaToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveWikimapiaToolStripMenuItem.Image")));
            this.SaveWikimapiaToolStripMenuItem.Name = "SaveWikimapiaToolStripMenuItem";
            this.SaveWikimapiaToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.SaveWikimapiaToolStripMenuItem.Text = "Wikimapia - ссылка";
            this.SaveWikimapiaToolStripMenuItem.Click += new System.EventHandler(this.SaveWikimapiaToolStripMenuItem_Click);
            // 
            // SaveAllSeparateToolStripMenuItem
            // 
            this.SaveAllSeparateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveAllSeparateToolStripMenuItem.Image")));
            this.SaveAllSeparateToolStripMenuItem.Name = "SaveAllSeparateToolStripMenuItem";
            this.SaveAllSeparateToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.SaveAllSeparateToolStripMenuItem.Text = "Сохранить все машруты в отдельные файлы";
            this.SaveAllSeparateToolStripMenuItem.ToolTipText = "Сохранить несколько маршрутов в отдельный файлы выбранного формата";
            this.SaveAllSeparateToolStripMenuItem.Click += new System.EventHandler(this.SaveAllSeparateToolStripMenuItem_Click);
            // 
            // saveAllInOneFileToolStripMenuItem
            // 
            this.saveAllInOneFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAllInOneFileToolStripMenuItem.Image")));
            this.saveAllInOneFileToolStripMenuItem.Name = "saveAllInOneFileToolStripMenuItem";
            this.saveAllInOneFileToolStripMenuItem.Size = new System.Drawing.Size(321, 22);
            this.saveAllInOneFileToolStripMenuItem.Text = "Сохранить все маршруты в один файл";
            this.saveAllInOneFileToolStripMenuItem.ToolTipText = "Сохранить все маршруты в один файл";
            this.saveAllInOneFileToolStripMenuItem.Click += new System.EventHandler(this.saveAllInOneFileToolStripMenuItem_Click);
            // 
            // dataGridViewConverter
            // 
            this.dataGridViewConverter.AllowDrop = true;
            this.dataGridViewConverter.AllowUserToAddRows = false;
            this.dataGridViewConverter.AllowUserToDeleteRows = false;
            this.dataGridViewConverter.AllowUserToResizeRows = false;
            this.dataGridViewConverter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewConverter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConverter.ContextMenuStrip = this.contextMenuStripDGW;
            this.dataGridViewConverter.Location = new System.Drawing.Point(0, 27);
            this.dataGridViewConverter.Name = "dataGridViewConverter";
            this.dataGridViewConverter.ReadOnly = true;
            this.dataGridViewConverter.Size = new System.Drawing.Size(294, 415);
            this.dataGridViewConverter.TabIndex = 8;
            this.dataGridViewConverter.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridViewConverter.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridViewConverter.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDoubleClick);
            this.dataGridViewConverter.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridViewConverter.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridViewConverter.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
            this.dataGridViewConverter.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
            this.dataGridViewConverter.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridView1_Paint);
            this.dataGridViewConverter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // contextMenuStripDGW
            // 
            this.contextMenuStripDGW.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveToolStripMenuItem1,
            this.изменитьToolStripMenuItem,
            this.показатьToolStripMenuItem,
            this.openRouteFolderToolStripMenuItem,
            this.addToJoinToolStripMenuItem,
            this.addComparisonToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStripDGW.Name = "contextMenuStripDGW";
            this.contextMenuStripDGW.Size = new System.Drawing.Size(218, 202);
            this.contextMenuStripDGW.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDGW_Opening);
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.informationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("informationToolStripMenuItem.Image")));
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.informationToolStripMenuItem.Tag = "single";
            this.informationToolStripMenuItem.Text = "Подробная информация";
            this.informationToolStripMenuItem.ToolTipText = "Показать окно с подробной информацией о маршруте";
            this.informationToolStripMenuItem.Click += new System.EventHandler(this.informationToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveFileContextToolStripMenuItem1,
            this.saveYandexContextToolStripMenuItem,
            this.saveWikimapiaContextToolStripMenuItem});
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.saveAsToolStripMenuItem.Text = "Сохранить как";
            // 
            // saveFileContextToolStripMenuItem1
            // 
            this.saveFileContextToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("saveFileContextToolStripMenuItem1.Image")));
            this.saveFileContextToolStripMenuItem1.Name = "saveFileContextToolStripMenuItem1";
            this.saveFileContextToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
            this.saveFileContextToolStripMenuItem1.Tag = "multy";
            this.saveFileContextToolStripMenuItem1.Text = "Файл";
            this.saveFileContextToolStripMenuItem1.ToolTipText = "Сохранить маршрут в файл";
            this.saveFileContextToolStripMenuItem1.Click += new System.EventHandler(this.SaveFileToolStripMenuItem_Click);
            // 
            // saveYandexContextToolStripMenuItem
            // 
            this.saveYandexContextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveYandexContextToolStripMenuItem.Image")));
            this.saveYandexContextToolStripMenuItem.Name = "saveYandexContextToolStripMenuItem";
            this.saveYandexContextToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveYandexContextToolStripMenuItem.Tag = "single";
            this.saveYandexContextToolStripMenuItem.Text = "Яндекс - ссылка";
            this.saveYandexContextToolStripMenuItem.Click += new System.EventHandler(this.SaveYandexToolStripMenuItem_Click);
            // 
            // saveWikimapiaContextToolStripMenuItem
            // 
            this.saveWikimapiaContextToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveWikimapiaContextToolStripMenuItem.Image")));
            this.saveWikimapiaContextToolStripMenuItem.Name = "saveWikimapiaContextToolStripMenuItem";
            this.saveWikimapiaContextToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.saveWikimapiaContextToolStripMenuItem.Tag = "single";
            this.saveWikimapiaContextToolStripMenuItem.Text = "Wikimapia - ссылка";
            this.saveWikimapiaContextToolStripMenuItem.Click += new System.EventHandler(this.SaveWikimapiaToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem1.Image")));
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(217, 22);
            this.saveToolStripMenuItem1.Tag = "single";
            this.saveToolStripMenuItem1.Text = "Сохранить";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.overwriteFileToolStripMenuItem_Click);
            // 
            // изменитьToolStripMenuItem
            // 
            this.изменитьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRouteMapToolStripMenuItem,
            this.editWaypointsToolStripMenuItem,
            this.loadElevationsToolStripMenuItem,
            this.removeElevationsToolStripMenuItem,
            this.approximateAltitudesToolStripMenuItem,
            this.normalizeTrackToolStripMenuItem,
            this.toTripRouteFileToolStripMenuItem,
            this.joinToTripRouteToolStripMenuItem});
            this.изменитьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("изменитьToolStripMenuItem.Image")));
            this.изменитьToolStripMenuItem.Name = "изменитьToolStripMenuItem";
            this.изменитьToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.изменитьToolStripMenuItem.Text = "Изменить";
            // 
            // editRouteMapToolStripMenuItem
            // 
            this.editRouteMapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editRouteMapToolStripMenuItem.Image")));
            this.editRouteMapToolStripMenuItem.Name = "editRouteMapToolStripMenuItem";
            this.editRouteMapToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.editRouteMapToolStripMenuItem.Tag = "single";
            this.editRouteMapToolStripMenuItem.Text = "Редактировать на карте";
            this.editRouteMapToolStripMenuItem.ToolTipText = "Изменение маршрута на карте";
            this.editRouteMapToolStripMenuItem.Click += new System.EventHandler(this.editRouteMapToolStripMenuItem_Click);
            // 
            // editWaypointsToolStripMenuItem
            // 
            this.editWaypointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editWaypointsToolStripMenuItem.Image")));
            this.editWaypointsToolStripMenuItem.Name = "editWaypointsToolStripMenuItem";
            this.editWaypointsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.editWaypointsToolStripMenuItem.Tag = "single";
            this.editWaypointsToolStripMenuItem.Text = "Править путевые точки";
            this.editWaypointsToolStripMenuItem.ToolTipText = "Открыть как последовательность точек";
            this.editWaypointsToolStripMenuItem.Click += new System.EventHandler(this.editWaypointsToolStripMenuItem_Click);
            // 
            // loadElevationsToolStripMenuItem
            // 
            this.loadElevationsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadElevationsToolStripMenuItem.Image")));
            this.loadElevationsToolStripMenuItem.Name = "loadElevationsToolStripMenuItem";
            this.loadElevationsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.loadElevationsToolStripMenuItem.Tag = "multy";
            this.loadElevationsToolStripMenuItem.Text = "Загрузить высоты точек";
            this.loadElevationsToolStripMenuItem.ToolTipText = "Записать в маршрут высоты всех его точек. Если это путешествие, то будут записаны" +
    " высоты путевых точек и маршрутов";
            this.loadElevationsToolStripMenuItem.Click += new System.EventHandler(this.loadElevationsToolStripMenuItem_Click);
            // 
            // removeElevationsToolStripMenuItem
            // 
            this.removeElevationsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeElevationsToolStripMenuItem.Image")));
            this.removeElevationsToolStripMenuItem.Name = "removeElevationsToolStripMenuItem";
            this.removeElevationsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.removeElevationsToolStripMenuItem.Tag = "multy";
            this.removeElevationsToolStripMenuItem.Text = "Удалить высоты точек";
            this.removeElevationsToolStripMenuItem.ToolTipText = "Удалить все высоты точек из маршурта";
            this.removeElevationsToolStripMenuItem.Click += new System.EventHandler(this.removeElevationsToolStripMenuItem_Click);
            // 
            // approximateAltitudesToolStripMenuItem
            // 
            this.approximateAltitudesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("approximateAltitudesToolStripMenuItem.Image")));
            this.approximateAltitudesToolStripMenuItem.Name = "approximateAltitudesToolStripMenuItem";
            this.approximateAltitudesToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.approximateAltitudesToolStripMenuItem.Tag = "multy";
            this.approximateAltitudesToolStripMenuItem.Text = "Аппроксимировать высоты";
            this.approximateAltitudesToolStripMenuItem.ToolTipText = "Аппроксимация высот полиномом заданной степени. Степень задаётся в настройках";
            this.approximateAltitudesToolStripMenuItem.Click += new System.EventHandler(this.approximateAltitudesToolStripMenuItem_Click);
            // 
            // normalizeTrackToolStripMenuItem
            // 
            this.normalizeTrackToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("normalizeTrackToolStripMenuItem.Image")));
            this.normalizeTrackToolStripMenuItem.Name = "normalizeTrackToolStripMenuItem";
            this.normalizeTrackToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.normalizeTrackToolStripMenuItem.Tag = "multy";
            this.normalizeTrackToolStripMenuItem.Text = "Нормализовать трек";
            this.normalizeTrackToolStripMenuItem.ToolTipText = "Удаление из трека погрешностей, создаваемых GPS";
            this.normalizeTrackToolStripMenuItem.Click += new System.EventHandler(this.normalizeTrackToolStripMenuItem_Click);
            // 
            // toTripRouteFileToolStripMenuItem
            // 
            this.toTripRouteFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("toTripRouteFileToolStripMenuItem.Image")));
            this.toTripRouteFileToolStripMenuItem.Name = "toTripRouteFileToolStripMenuItem";
            this.toTripRouteFileToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.toTripRouteFileToolStripMenuItem.Tag = "multy";
            this.toTripRouteFileToolStripMenuItem.Text = "Преобразовать в путешествия";
            this.toTripRouteFileToolStripMenuItem.ToolTipText = "Преобразование каждого выделенного маршрута в отдельное путешествие";
            this.toTripRouteFileToolStripMenuItem.Click += new System.EventHandler(this.toTripRouteFileToolStripMenuItem_Click);
            // 
            // joinToTripRouteToolStripMenuItem
            // 
            this.joinToTripRouteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("joinToTripRouteToolStripMenuItem.Image")));
            this.joinToTripRouteToolStripMenuItem.Name = "joinToTripRouteToolStripMenuItem";
            this.joinToTripRouteToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.joinToTripRouteToolStripMenuItem.Tag = "multy";
            this.joinToTripRouteToolStripMenuItem.Text = "Объединить в путешествие";
            this.joinToTripRouteToolStripMenuItem.ToolTipText = "Объединить выбранные маршруты в одно путешествие";
            this.joinToTripRouteToolStripMenuItem.Click += new System.EventHandler(this.joinToTripRouteToolStripMenuItem_Click);
            // 
            // показатьToolStripMenuItem
            // 
            this.показатьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showWaypointsToolStripMenuItem,
            this.showOnMapToolStripMenuItem,
            this.elevgraphWithIntermediatesToolStripMenuItem,
            this.elevgraphToolStripMenuItem});
            this.показатьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("показатьToolStripMenuItem.Image")));
            this.показатьToolStripMenuItem.Name = "показатьToolStripMenuItem";
            this.показатьToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.показатьToolStripMenuItem.Text = "Показать";
            // 
            // showWaypointsToolStripMenuItem
            // 
            this.showWaypointsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showWaypointsToolStripMenuItem.Image")));
            this.showWaypointsToolStripMenuItem.Name = "showWaypointsToolStripMenuItem";
            this.showWaypointsToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.showWaypointsToolStripMenuItem.Tag = "multy";
            this.showWaypointsToolStripMenuItem.Text = "Показать точки на карте";
            this.showWaypointsToolStripMenuItem.ToolTipText = "По";
            this.showWaypointsToolStripMenuItem.Click += new System.EventHandler(this.showWaypointsOnMapToolStripMenuItem_Click);
            // 
            // showOnMapToolStripMenuItem
            // 
            this.showOnMapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showOnMapToolStripMenuItem.Image")));
            this.showOnMapToolStripMenuItem.Name = "showOnMapToolStripMenuItem";
            this.showOnMapToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.showOnMapToolStripMenuItem.Tag = "multy";
            this.showOnMapToolStripMenuItem.Text = "Маршрут";
            this.showOnMapToolStripMenuItem.Click += new System.EventHandler(this.showRouteOnMapToolStripMenuItem_Click);
            // 
            // elevgraphWithIntermediatesToolStripMenuItem
            // 
            this.elevgraphWithIntermediatesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("elevgraphWithIntermediatesToolStripMenuItem.Image")));
            this.elevgraphWithIntermediatesToolStripMenuItem.Name = "elevgraphWithIntermediatesToolStripMenuItem";
            this.elevgraphWithIntermediatesToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.elevgraphWithIntermediatesToolStripMenuItem.Tag = "multy";
            this.elevgraphWithIntermediatesToolStripMenuItem.Text = "Профиль высот с промежуточными точками";
            this.elevgraphWithIntermediatesToolStripMenuItem.ToolTipText = "Показать профиль высот ждя этого трека. Перед построением профиля в трек будут до" +
    "бавлены промежуточные точки для более точного построения";
            this.elevgraphWithIntermediatesToolStripMenuItem.Click += new System.EventHandler(this.elevgraphWithIntermediatesToolStripMenuItem_Click);
            // 
            // elevgraphToolStripMenuItem
            // 
            this.elevgraphToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("elevgraphToolStripMenuItem.Image")));
            this.elevgraphToolStripMenuItem.Name = "elevgraphToolStripMenuItem";
            this.elevgraphToolStripMenuItem.Size = new System.Drawing.Size(326, 22);
            this.elevgraphToolStripMenuItem.Tag = "multy";
            this.elevgraphToolStripMenuItem.Text = "Профиль высот";
            this.elevgraphToolStripMenuItem.Click += new System.EventHandler(this.elevgraphToolStripMenuItem_Click);
            // 
            // openRouteFolderToolStripMenuItem
            // 
            this.openRouteFolderToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openRouteFolderToolStripMenuItem.Image")));
            this.openRouteFolderToolStripMenuItem.Name = "openRouteFolderToolStripMenuItem";
            this.openRouteFolderToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.openRouteFolderToolStripMenuItem.Tag = "single";
            this.openRouteFolderToolStripMenuItem.Text = "Открыть в проводнике";
            this.openRouteFolderToolStripMenuItem.Click += new System.EventHandler(this.openRouteFolderToolStripMenuItem_Click);
            // 
            // addToJoinToolStripMenuItem
            // 
            this.addToJoinToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addToJoinToolStripMenuItem.Image")));
            this.addToJoinToolStripMenuItem.Name = "addToJoinToolStripMenuItem";
            this.addToJoinToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.addToJoinToolStripMenuItem.Tag = "multy";
            this.addToJoinToolStripMenuItem.Text = "Добавить в объединение";
            this.addToJoinToolStripMenuItem.ToolTipText = "Добавить маршрут для объединения с другими маршрутами";
            this.addToJoinToolStripMenuItem.Click += new System.EventHandler(this.addToJoinToolStripMenuItem_Click);
            // 
            // addComparisonToolStripMenuItem
            // 
            this.addComparisonToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addComparisonToolStripMenuItem.Image")));
            this.addComparisonToolStripMenuItem.Name = "addComparisonToolStripMenuItem";
            this.addComparisonToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.addComparisonToolStripMenuItem.Tag = "multy";
            this.addComparisonToolStripMenuItem.Text = "Добавить в сравнение";
            this.addComparisonToolStripMenuItem.Click += new System.EventHandler(this.addComparisonToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeToolStripMenuItem.Image")));
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.removeToolStripMenuItem.Tag = "multy";
            this.removeToolStripMenuItem.Text = "Удалить";
            this.removeToolStripMenuItem.ToolTipText = "Удаление маршрута из списка загруженных";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(127, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 442);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridViewConverter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Список маршрутов";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConverter_FormClosed);
            this.Load += new System.EventHandler(this.FormConverter_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConverter)).EndInit();
            this.contextMenuStripDGW.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem ClearToolStripMenuItem;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridView dataGridViewConverter;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem загрузитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem OpenYandexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveYandexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAllSeparateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllInOneFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveWikimapiaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveFileToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDGW;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileContextToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveYandexContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveWikimapiaContextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToJoinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem изменитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editWaypointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRouteMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadElevationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem показатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWaypointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOnMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRouteFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elevgraphWithIntermediatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addComparisonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem approximateAltitudesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elevgraphToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem removeElevationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalizeTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toTripRouteFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinToTripRouteToolStripMenuItem;
    }
}