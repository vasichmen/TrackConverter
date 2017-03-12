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
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddDay = new System.Windows.Forms.Button();
            this.buttonAddDayFromFile = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonAddWaypoint = new System.Windows.Forms.Button();
            this.buttonWptFromFile = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDays)).BeginInit();
            this.contextMenuStripDays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWaypoints)).BeginInit();
            this.contextMenuStripWaypoints.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonCancel.Location = new System.Drawing.Point(463, 261);
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
            this.buttonSave.Location = new System.Drawing.Point(3, 0);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 32);
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
            this.dataGridViewDays.Size = new System.Drawing.Size(223, 232);
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
            this.insertDayToolStripMenuItem,
            this.removeDayToolStripMenuItem});
            this.contextMenuStripDays.Name = "contextMenuStripDays";
            this.contextMenuStripDays.Size = new System.Drawing.Size(162, 136);
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
            this.dataGridViewWaypoints.Location = new System.Drawing.Point(232, 23);
            this.dataGridViewWaypoints.Name = "dataGridViewWaypoints";
            this.dataGridViewWaypoints.ReadOnly = true;
            this.dataGridViewWaypoints.Size = new System.Drawing.Size(223, 232);
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
            this.label1.Location = new System.Drawing.Point(47, 1);
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
            this.label2.Location = new System.Drawing.Point(290, 1);
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
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewWaypoints, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.dataGridViewDays, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonCancel, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(544, 296);
            this.tableLayoutPanel1.TabIndex = 60;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonSave);
            this.flowLayoutPanel1.Controls.Add(this.buttonLoadElevations);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(461, 23);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(80, 232);
            this.flowLayoutPanel1.TabIndex = 60;
            // 
            // buttonLoadElevations
            // 
            this.buttonLoadElevations.Location = new System.Drawing.Point(3, 38);
            this.buttonLoadElevations.Name = "buttonLoadElevations";
            this.buttonLoadElevations.Size = new System.Drawing.Size(75, 38);
            this.buttonLoadElevations.TabIndex = 2;
            this.buttonLoadElevations.Text = "Загрузить высоты";
            this.buttonLoadElevations.UseVisualStyleBackColor = true;
            this.buttonLoadElevations.Click += new System.EventHandler(this.buttonLoadElevations_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.buttonAddDay);
            this.flowLayoutPanel2.Controls.Add(this.buttonAddDayFromFile);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 258);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(229, 38);
            this.flowLayoutPanel2.TabIndex = 62;
            // 
            // buttonAddDay
            // 
            this.buttonAddDay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonAddDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddDay.Location = new System.Drawing.Point(3, 3);
            this.buttonAddDay.Name = "buttonAddDay";
            this.buttonAddDay.Size = new System.Drawing.Size(108, 32);
            this.buttonAddDay.TabIndex = 62;
            this.buttonAddDay.Text = "Добавить";
            this.toolTip1.SetToolTip(this.buttonAddDay, "Создать маршрут на карте");
            this.buttonAddDay.UseVisualStyleBackColor = true;
            this.buttonAddDay.Click += new System.EventHandler(this.buttonAddDay_Click);
            // 
            // buttonAddDayFromFile
            // 
            this.buttonAddDayFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddDayFromFile.Location = new System.Drawing.Point(117, 3);
            this.buttonAddDayFromFile.Name = "buttonAddDayFromFile";
            this.buttonAddDayFromFile.Size = new System.Drawing.Size(109, 32);
            this.buttonAddDayFromFile.TabIndex = 2;
            this.buttonAddDayFromFile.Text = "Из файла";
            this.toolTip1.SetToolTip(this.buttonAddDayFromFile, "Загрузить дневной маршурт из файла");
            this.buttonAddDayFromFile.UseVisualStyleBackColor = true;
            this.buttonAddDayFromFile.Click += new System.EventHandler(this.buttonAddDayFromFile_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.buttonAddWaypoint);
            this.flowLayoutPanel3.Controls.Add(this.buttonWptFromFile);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(229, 258);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(229, 38);
            this.flowLayoutPanel3.TabIndex = 63;
            // 
            // buttonAddWaypoint
            // 
            this.buttonAddWaypoint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonAddWaypoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonAddWaypoint.Location = new System.Drawing.Point(3, 3);
            this.buttonAddWaypoint.Name = "buttonAddWaypoint";
            this.buttonAddWaypoint.Size = new System.Drawing.Size(107, 32);
            this.buttonAddWaypoint.TabIndex = 61;
            this.buttonAddWaypoint.Text = "Добавить";
            this.toolTip1.SetToolTip(this.buttonAddWaypoint, "Добавить точку на карте");
            this.buttonAddWaypoint.UseVisualStyleBackColor = true;
            this.buttonAddWaypoint.Click += new System.EventHandler(this.buttonAddWaypoint_Click);
            // 
            // buttonWptFromFile
            // 
            this.buttonWptFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonWptFromFile.Location = new System.Drawing.Point(116, 3);
            this.buttonWptFromFile.Name = "buttonWptFromFile";
            this.buttonWptFromFile.Size = new System.Drawing.Size(109, 32);
            this.buttonWptFromFile.TabIndex = 62;
            this.buttonWptFromFile.Text = "Из файла";
            this.toolTip1.SetToolTip(this.buttonWptFromFile, "Загрузить список путевых точек из файла");
            this.buttonWptFromFile.UseVisualStyleBackColor = true;
            this.buttonWptFromFile.Click += new System.EventHandler(this.buttonWptFromFile_Click);
            // 
            // FormEditTrip
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 296);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(404, 162);
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
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
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
        private System.Windows.Forms.Button buttonAddWaypoint;
        private System.Windows.Forms.Button buttonAddDay;
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button buttonAddDayFromFile;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button buttonWptFromFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonLoadElevations;
    }
}