namespace TrackConverter.UI.Tools
{
    partial class FormPoints
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPoints));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.йствияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshAzimuthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOnMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewPoints = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showYandexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGoogleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPoints)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.йствияToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(647, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьToolStripMenuItem,
            this.toolStripMenuItem1});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // сохранитьToolStripMenuItem
            // 
            this.сохранитьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveFileToolStripMenuItem});
            this.сохранитьToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("сохранитьToolStripMenuItem.Image")));
            this.сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            this.сохранитьToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.сохранитьToolStripMenuItem.Text = "Сохранить";
            // 
            // SaveFileToolStripMenuItem
            // 
            this.SaveFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveFileToolStripMenuItem.Image")));
            this.SaveFileToolStripMenuItem.Name = "SaveFileToolStripMenuItem";
            this.SaveFileToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.SaveFileToolStripMenuItem.Text = "Файл";
            this.SaveFileToolStripMenuItem.ToolTipText = "Сохранение точек в файл";
            this.SaveFileToolStripMenuItem.Click += new System.EventHandler(this.SaveFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(132, 22);
            this.toolStripMenuItem1.Text = "Открыть";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem2.Image")));
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(116, 22);
            this.toolStripMenuItem2.Text = "Файл";
            this.toolStripMenuItem2.ToolTipText = "Загрузка точек из файла ";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.LoadFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem3.Image")));
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(116, 22);
            this.toolStripMenuItem3.Text = "Ссылка";
            this.toolStripMenuItem3.ToolTipText = "Загрузка точек из ссылки";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.LoadLinkToolStripMenuItem_Click);
            // 
            // йствияToolStripMenuItem
            // 
            this.йствияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshAzimuthsToolStripMenuItem,
            this.openOnMapToolStripMenuItem});
            this.йствияToolStripMenuItem.Name = "йствияToolStripMenuItem";
            this.йствияToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.йствияToolStripMenuItem.Text = "Действия";
            // 
            // refreshAzimuthsToolStripMenuItem
            // 
            this.refreshAzimuthsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshAzimuthsToolStripMenuItem.Image")));
            this.refreshAzimuthsToolStripMenuItem.Name = "refreshAzimuthsToolStripMenuItem";
            this.refreshAzimuthsToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.refreshAzimuthsToolStripMenuItem.Text = "Обновить неизменяемые поля";
            this.refreshAzimuthsToolStripMenuItem.ToolTipText = "Обновить значения азимутов, расстояний";
            this.refreshAzimuthsToolStripMenuItem.Click += new System.EventHandler(this.refreshAzimuthsToolStripMenuItem_Click);
            // 
            // openOnMapToolStripMenuItem
            // 
            this.openOnMapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openOnMapToolStripMenuItem.Image")));
            this.openOnMapToolStripMenuItem.Name = "openOnMapToolStripMenuItem";
            this.openOnMapToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.openOnMapToolStripMenuItem.Text = "Открыть точки на карте";
            this.openOnMapToolStripMenuItem.Click += new System.EventHandler(this.OpenOnMapToolStripMenuItem_Click);
            // 
            // dataGridViewPoints
            // 
            this.dataGridViewPoints.AllowDrop = true;
            this.dataGridViewPoints.AllowUserToDeleteRows = false;
            this.dataGridViewPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPoints.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridViewPoints.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPoints.Location = new System.Drawing.Point(0, 24);
            this.dataGridViewPoints.Name = "dataGridViewPoints";
            this.dataGridViewPoints.ShowCellErrors = false;
            this.dataGridViewPoints.Size = new System.Drawing.Size(647, 238);
            this.dataGridViewPoints.StandardTab = true;
            this.dataGridViewPoints.TabIndex = 1;
            this.dataGridViewPoints.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridViewPoints.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridViewPoints.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridViewPoints.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            this.dataGridViewPoints.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragDrop);
            this.dataGridViewPoints.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView1_DragEnter);
            this.dataGridViewPoints.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.editToolStripMenuItem,
            this.openConverterToolStripMenuItem,
            this.showMapToolStripMenuItem,
            this.showYandexToolStripMenuItem,
            this.showGoogleToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(258, 158);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addToolStripMenuItem.Image")));
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.addToolStripMenuItem.Text = "Добавить";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editToolStripMenuItem.Image")));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.editToolStripMenuItem.Text = "Изменить";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // openConverterToolStripMenuItem
            // 
            this.openConverterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openConverterToolStripMenuItem.Image")));
            this.openConverterToolStripMenuItem.Name = "openConverterToolStripMenuItem";
            this.openConverterToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.openConverterToolStripMenuItem.Text = "Открыть в конвертере координат";
            this.openConverterToolStripMenuItem.Click += new System.EventHandler(this.openConverterToolStripMenuItem_Click);
            // 
            // showMapToolStripMenuItem
            // 
            this.showMapToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showMapToolStripMenuItem.Image")));
            this.showMapToolStripMenuItem.Name = "showMapToolStripMenuItem";
            this.showMapToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.showMapToolStripMenuItem.Text = "Покзать на карте";
            this.showMapToolStripMenuItem.Click += new System.EventHandler(this.showMapToolStripMenuItem_Click);
            // 
            // showYandexToolStripMenuItem
            // 
            this.showYandexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showYandexToolStripMenuItem.Image")));
            this.showYandexToolStripMenuItem.Name = "showYandexToolStripMenuItem";
            this.showYandexToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.showYandexToolStripMenuItem.Text = "Показать на карте Яндекс";
            this.showYandexToolStripMenuItem.Click += new System.EventHandler(this.showYandexToolStripMenuItem_Click);
            // 
            // showGoogleToolStripMenuItem
            // 
            this.showGoogleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showGoogleToolStripMenuItem.Image")));
            this.showGoogleToolStripMenuItem.Name = "showGoogleToolStripMenuItem";
            this.showGoogleToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.showGoogleToolStripMenuItem.Text = "Показать на карте Google";
            this.showGoogleToolStripMenuItem.Click += new System.EventHandler(this.showGoogleToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeToolStripMenuItem.Image")));
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.removeToolStripMenuItem.Text = "Удалить";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // FormPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 262);
            this.Controls.Add(this.dataGridViewPoints);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormPoints";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Правка путевых точек";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPoints_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPoints)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        /// <summary>
        /// таблица точек выделенного трека
        /// </summary>
        public System.Windows.Forms.DataGridView dataGridViewPoints;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openConverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showYandexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGoogleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem йствияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshAzimuthsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOnMapToolStripMenuItem;
    }
}