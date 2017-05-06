namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно редактирования путешествия
    /// </summary>
    partial class FormEditTrip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditTrip));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.dataGridViewDays = new System.Windows.Forms.DataGridView();
            this.contextMenuStripDays = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.informationDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewWaypoints = new System.Windows.Forms.DataGridView();
            this.contextMenuStripWaypoints = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.informationPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPointНовуюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePointToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonLoadElevations = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonWptFromFile = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddWaypoint = new System.Windows.Forms.Button();
            this.buttonExportWaypoints = new System.Windows.Forms.Button();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddDayFromFile = new System.Windows.Forms.Button();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddDay = new System.Windows.Forms.Button();
            this.buttonExportDays = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDays)).BeginInit();
            this.contextMenuStripDays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWaypoints)).BeginInit();
            this.contextMenuStripWaypoints.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCancel.Location = new System.Drawing.Point(511, 305);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 32);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Отмена";
            this.toolTip1.SetToolTip(this.buttonCancel, "Отменить изменения");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(2, 0);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(2, 0, 2, 2);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(82, 32);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Сохранить";
            this.toolTip1.SetToolTip(this.buttonSave, "Сохранить в списке маршрутов");
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // dataGridViewDays
            // 
            this.dataGridViewDays.AllowUserToAddRows = false;
            this.dataGridViewDays.AllowUserToDeleteRows = false;
            this.dataGridViewDays.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDays.ContextMenuStrip = this.contextMenuStripDays;
            this.dataGridViewDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDays.Location = new System.Drawing.Point(3, 23);
            this.dataGridViewDays.Name = "dataGridViewDays";
            this.dataGridViewDays.ReadOnly = true;
            this.dataGridViewDays.Size = new System.Drawing.Size(245, 262);
            this.dataGridViewDays.TabIndex = 2;
            this.dataGridViewDays.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDays_CellClick);
            this.dataGridViewDays.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDays_CellDoubleClick);
            this.dataGridViewDays.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewDays_CellMouseDown);
            this.dataGridViewDays.SelectionChanged += new System.EventHandler(this.dataGridViewDays_SelectionChanged);
            this.dataGridViewDays.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewDays_KeyDown);
            // 
            // contextMenuStripDays
            // 
            this.contextMenuStripDays.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationDayToolStripMenuItem,
            this.upDayToolStripMenuItem,
            this.downDayToolStripMenuItem,
            this.editDayToolStripMenuItem,
            this.invertToolStripMenuItem,
            this.insertDayToolStripMenuItem,
            this.removeDayToolStripMenuItem});
            this.contextMenuStripDays.Name = "contextMenuStripDays";
            this.contextMenuStripDays.Size = new System.Drawing.Size(162, 158);
            this.contextMenuStripDays.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDays_Opening);
            // 
            // informationDayToolStripMenuItem
            // 
            this.informationDayToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.informationDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("informationDayToolStripMenuItem.Image")));
            this.informationDayToolStripMenuItem.Name = "informationDayToolStripMenuItem";
            this.informationDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.informationDayToolStripMenuItem.Text = "Информация";
            this.informationDayToolStripMenuItem.Click += new System.EventHandler(this.informationDayToolStripMenuItem_Click);
            // 
            // upDayToolStripMenuItem
            // 
            this.upDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("upDayToolStripMenuItem.Image")));
            this.upDayToolStripMenuItem.Name = "upDayToolStripMenuItem";
            this.upDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.upDayToolStripMenuItem.Text = "Вверх";
            this.upDayToolStripMenuItem.Click += new System.EventHandler(this.upDayToolStripMenuItem_Click);
            // 
            // downDayToolStripMenuItem
            // 
            this.downDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("downDayToolStripMenuItem.Image")));
            this.downDayToolStripMenuItem.Name = "downDayToolStripMenuItem";
            this.downDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.downDayToolStripMenuItem.Text = "Вниз";
            this.downDayToolStripMenuItem.Click += new System.EventHandler(this.downDayToolStripMenuItem_Click);
            // 
            // editDayToolStripMenuItem
            // 
            this.editDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editDayToolStripMenuItem.Image")));
            this.editDayToolStripMenuItem.Name = "editDayToolStripMenuItem";
            this.editDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.editDayToolStripMenuItem.Text = "Редактировать";
            this.editDayToolStripMenuItem.Click += new System.EventHandler(this.editDayToolStripMenuItem_Click);
            // 
            // invertToolStripMenuItem
            // 
            this.invertToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("invertToolStripMenuItem.Image")));
            this.invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            this.invertToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.invertToolStripMenuItem.Text = "Инвертировать";
            this.invertToolStripMenuItem.ToolTipText = "Инвертировать маршрут";
            this.invertToolStripMenuItem.Click += new System.EventHandler(this.invertToolStripMenuItem_Click);
            // 
            // insertDayToolStripMenuItem
            // 
            this.insertDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("insertDayToolStripMenuItem.Image")));
            this.insertDayToolStripMenuItem.Name = "insertDayToolStripMenuItem";
            this.insertDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.insertDayToolStripMenuItem.Text = "Вставить новый";
            this.insertDayToolStripMenuItem.Click += new System.EventHandler(this.insertDayToolStripMenuItem_Click);
            // 
            // removeDayToolStripMenuItem
            // 
            this.removeDayToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeDayToolStripMenuItem.Image")));
            this.removeDayToolStripMenuItem.Name = "removeDayToolStripMenuItem";
            this.removeDayToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.removeDayToolStripMenuItem.Text = "Удалить";
            this.removeDayToolStripMenuItem.Click += new System.EventHandler(this.removeDayToolStripMenuItem_Click);
            // 
            // dataGridViewWaypoints
            // 
            this.dataGridViewWaypoints.AllowUserToAddRows = false;
            this.dataGridViewWaypoints.AllowUserToDeleteRows = false;
            this.dataGridViewWaypoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewWaypoints.ContextMenuStrip = this.contextMenuStripWaypoints;
            this.dataGridViewWaypoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewWaypoints.Location = new System.Drawing.Point(254, 23);
            this.dataGridViewWaypoints.Name = "dataGridViewWaypoints";
            this.dataGridViewWaypoints.ReadOnly = true;
            this.dataGridViewWaypoints.Size = new System.Drawing.Size(245, 262);
            this.dataGridViewWaypoints.TabIndex = 54;
            this.dataGridViewWaypoints.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewWaypoints_CellClick);
            this.dataGridViewWaypoints.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewWaypoints_CellDoubleClick);
            this.dataGridViewWaypoints.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewWaypoints_CellMouseDown);
            this.dataGridViewWaypoints.SelectionChanged += new System.EventHandler(this.dataGridViewWaypoints_SelectionChanged);
            this.dataGridViewWaypoints.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewWaypoints_KeyDown);
            // 
            // contextMenuStripWaypoints
            // 
            this.contextMenuStripWaypoints.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationPointToolStripMenuItem,
            this.upPointToolStripMenuItem,
            this.downPointToolStripMenuItem,
            this.insertPointНовуюToolStripMenuItem,
            this.removePointToolStripMenuItem1});
            this.contextMenuStripWaypoints.Name = "contextMenuStripWaypoints";
            this.contextMenuStripWaypoints.Size = new System.Drawing.Size(162, 114);
            this.contextMenuStripWaypoints.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripWaypoints_Opening);
            // 
            // informationPointToolStripMenuItem
            // 
            this.informationPointToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.informationPointToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("informationPointToolStripMenuItem.Image")));
            this.informationPointToolStripMenuItem.Name = "informationPointToolStripMenuItem";
            this.informationPointToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.informationPointToolStripMenuItem.Text = "Информация";
            this.informationPointToolStripMenuItem.Click += new System.EventHandler(this.informationPointToolStripMenuItem_Click);
            // 
            // upPointToolStripMenuItem
            // 
            this.upPointToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("upPointToolStripMenuItem.Image")));
            this.upPointToolStripMenuItem.Name = "upPointToolStripMenuItem";
            this.upPointToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.upPointToolStripMenuItem.Text = "Вверх";
            this.upPointToolStripMenuItem.Click += new System.EventHandler(this.upPointToolStripMenuItem1_Click);
            // 
            // downPointToolStripMenuItem
            // 
            this.downPointToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("downPointToolStripMenuItem.Image")));
            this.downPointToolStripMenuItem.Name = "downPointToolStripMenuItem";
            this.downPointToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.downPointToolStripMenuItem.Text = "Вниз";
            this.downPointToolStripMenuItem.Click += new System.EventHandler(this.downPointToolStripMenuItem1_Click);
            // 
            // insertPointНовуюToolStripMenuItem
            // 
            this.insertPointНовуюToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("insertPointНовуюToolStripMenuItem.Image")));
            this.insertPointНовуюToolStripMenuItem.Name = "insertPointНовуюToolStripMenuItem";
            this.insertPointНовуюToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.insertPointНовуюToolStripMenuItem.Text = "Вставить новую";
            this.insertPointНовуюToolStripMenuItem.Click += new System.EventHandler(this.insertPointToolStripMenuItem_Click);
            // 
            // removePointToolStripMenuItem1
            // 
            this.removePointToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("removePointToolStripMenuItem1.Image")));
            this.removePointToolStripMenuItem1.Name = "removePointToolStripMenuItem1";
            this.removePointToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.removePointToolStripMenuItem1.Text = "Удалить";
            this.removePointToolStripMenuItem1.Click += new System.EventHandler(this.removePointToolStripMenuItem1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(58, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 58;
            this.label1.Text = "Маршруты по дням";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(323, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 17);
            this.label2.TabIndex = 59;
            this.label2.Text = "Путевые точки";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewWaypoints, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewDays, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(595, 354);
            this.tableLayoutPanel1.TabIndex = 60;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.buttonLoadElevations);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(505, 23);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(87, 262);
            this.flowLayoutPanel1.TabIndex = 60;
            // 
            // buttonLoadElevations
            // 
            this.buttonLoadElevations.Location = new System.Drawing.Point(3, 37);
            this.buttonLoadElevations.Name = "buttonLoadElevations";
            this.buttonLoadElevations.Size = new System.Drawing.Size(81, 38);
            this.buttonLoadElevations.TabIndex = 2;
            this.buttonLoadElevations.Text = "Загрузить высоты";
            this.buttonLoadElevations.UseVisualStyleBackColor = true;
            this.buttonLoadElevations.Click += new System.EventHandler(this.buttonLoadElevations_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel4, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(254, 291);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(245, 60);
            this.tableLayoutPanel2.TabIndex = 63;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.buttonWptFromFile);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(125, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(117, 54);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // buttonWptFromFile
            // 
            this.buttonWptFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonWptFromFile.Location = new System.Drawing.Point(2, 2);
            this.buttonWptFromFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonWptFromFile.Name = "buttonWptFromFile";
            this.buttonWptFromFile.Size = new System.Drawing.Size(109, 49);
            this.buttonWptFromFile.TabIndex = 71;
            this.buttonWptFromFile.Text = "Добавить из файла";
            this.toolTip1.SetToolTip(this.buttonWptFromFile, "Загрузить список путевых точек из файла");
            this.buttonWptFromFile.UseVisualStyleBackColor = true;
            this.buttonWptFromFile.Click += new System.EventHandler(this.buttonWptFromFile_Click);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.buttonAddWaypoint);
            this.flowLayoutPanel4.Controls.Add(this.buttonExportWaypoints);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(116, 54);
            this.flowLayoutPanel4.TabIndex = 1;
            // 
            // buttonAddWaypoint
            // 
            this.buttonAddWaypoint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonAddWaypoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddWaypoint.Location = new System.Drawing.Point(2, 2);
            this.buttonAddWaypoint.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddWaypoint.Name = "buttonAddWaypoint";
            this.buttonAddWaypoint.Size = new System.Drawing.Size(107, 23);
            this.buttonAddWaypoint.TabIndex = 70;
            this.buttonAddWaypoint.Text = "Добавить";
            this.toolTip1.SetToolTip(this.buttonAddWaypoint, "Добавить точку на карте");
            this.buttonAddWaypoint.UseVisualStyleBackColor = true;
            this.buttonAddWaypoint.Click += new System.EventHandler(this.buttonAddWaypoint_Click);
            // 
            // buttonExportWaypoints
            // 
            this.buttonExportWaypoints.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonExportWaypoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonExportWaypoints.Location = new System.Drawing.Point(2, 29);
            this.buttonExportWaypoints.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExportWaypoints.Name = "buttonExportWaypoints";
            this.buttonExportWaypoints.Size = new System.Drawing.Size(107, 23);
            this.buttonExportWaypoints.TabIndex = 72;
            this.buttonExportWaypoints.Text = "Экспорт";
            this.toolTip1.SetToolTip(this.buttonExportWaypoints, "Добавить точку на карте");
            this.buttonExportWaypoints.UseVisualStyleBackColor = true;
            this.buttonExportWaypoints.Click += new System.EventHandler(this.buttonExportWaypoints_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel7, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel6, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 291);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(245, 60);
            this.tableLayoutPanel4.TabIndex = 65;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.Controls.Add(this.buttonAddDayFromFile);
            this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(125, 3);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(117, 54);
            this.flowLayoutPanel7.TabIndex = 1;
            // 
            // buttonAddDayFromFile
            // 
            this.buttonAddDayFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddDayFromFile.Location = new System.Drawing.Point(2, 2);
            this.buttonAddDayFromFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddDayFromFile.Name = "buttonAddDayFromFile";
            this.buttonAddDayFromFile.Size = new System.Drawing.Size(109, 49);
            this.buttonAddDayFromFile.TabIndex = 73;
            this.buttonAddDayFromFile.Text = "Добавить из файла";
            this.toolTip1.SetToolTip(this.buttonAddDayFromFile, "Загрузить дневной маршрут из файла");
            this.buttonAddDayFromFile.UseVisualStyleBackColor = true;
            this.buttonAddDayFromFile.Click += new System.EventHandler(this.buttonAddDayFromFile_Click);
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Controls.Add(this.buttonAddDay);
            this.flowLayoutPanel6.Controls.Add(this.buttonExportDays);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(116, 54);
            this.flowLayoutPanel6.TabIndex = 0;
            // 
            // buttonAddDay
            // 
            this.buttonAddDay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonAddDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddDay.Location = new System.Drawing.Point(2, 2);
            this.buttonAddDay.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAddDay.Name = "buttonAddDay";
            this.buttonAddDay.Size = new System.Drawing.Size(108, 23);
            this.buttonAddDay.TabIndex = 74;
            this.buttonAddDay.Text = "Добавить";
            this.toolTip1.SetToolTip(this.buttonAddDay, "Создать маршрут на карте");
            this.buttonAddDay.UseVisualStyleBackColor = true;
            this.buttonAddDay.Click += new System.EventHandler(this.buttonAddDay_Click);
            // 
            // buttonExportDays
            // 
            this.buttonExportDays.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonExportDays.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonExportDays.Location = new System.Drawing.Point(2, 29);
            this.buttonExportDays.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExportDays.Name = "buttonExportDays";
            this.buttonExportDays.Size = new System.Drawing.Size(107, 23);
            this.buttonExportDays.TabIndex = 75;
            this.buttonExportDays.Text = "Экспорт";
            this.toolTip1.SetToolTip(this.buttonExportDays, "Добавить точку на карте");
            this.buttonExportDays.UseVisualStyleBackColor = true;
            this.buttonExportDays.Click += new System.EventHandler(this.buttonExportDays_Click);
            // 
            // FormEditTrip
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 354);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(598, 392);
            this.Name = "FormEditTrip";
            this.Text = "Редактирование путешествия";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditTrip_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDays)).EndInit();
            this.contextMenuStripDays.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWaypoints)).EndInit();
            this.contextMenuStripWaypoints.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.DataGridView dataGridViewDays;
        private System.Windows.Forms.DataGridView dataGridViewWaypoints;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDays;
        private System.Windows.Forms.ToolStripMenuItem informationDayToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripWaypoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem editDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertPointНовуюToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePointToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem downDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downPointToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonLoadElevations;
        private System.Windows.Forms.ToolStripMenuItem invertToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button buttonWptFromFile;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Button buttonAddWaypoint;
        private System.Windows.Forms.Button buttonExportWaypoints;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Button buttonAddDayFromFile;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Button buttonAddDay;
        private System.Windows.Forms.Button buttonExportDays;
    }
}