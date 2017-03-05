namespace TrackConverter.UI.Common
{
    partial class FormContainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContainer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditPointFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CalculateDistanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TransformCoordinateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelCurrentOperation = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // создатьToolStripMenuItem
            // 
            this.создатьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditPointFileToolStripMenuItem,
            this.CalculateDistanceToolStripMenuItem,
            this.TransformCoordinateToolStripMenuItem,
            this.consoleToolStripMenuItem,
            this.toolStripSeparator2,
            this.OptionsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.создатьToolStripMenuItem.Text = "Инструменты";
            // 
            // EditPointFileToolStripMenuItem
            // 
            this.EditPointFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("EditPointFileToolStripMenuItem.Image")));
            this.EditPointFileToolStripMenuItem.Name = "EditPointFileToolStripMenuItem";
            this.EditPointFileToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.EditPointFileToolStripMenuItem.Text = "Правка путевых точек";
            this.EditPointFileToolStripMenuItem.ToolTipText = "Открытие маршрута как последовательность точек с возможностью редактирования";
            this.EditPointFileToolStripMenuItem.Click += new System.EventHandler(this.EditPointFileToolStripMenuItem_Click);
            // 
            // CalculateDistanceToolStripMenuItem
            // 
            this.CalculateDistanceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CalculateDistanceToolStripMenuItem.Image")));
            this.CalculateDistanceToolStripMenuItem.Name = "CalculateDistanceToolStripMenuItem";
            this.CalculateDistanceToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.CalculateDistanceToolStripMenuItem.Text = "Измерение расстояний";
            this.CalculateDistanceToolStripMenuItem.ToolTipText = "Измерение расстояний и азимутов между точками";
            this.CalculateDistanceToolStripMenuItem.Click += new System.EventHandler(this.CalculateDistanceToolStripMenuItem_Click);
            // 
            // TransformCoordinateToolStripMenuItem
            // 
            this.TransformCoordinateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("TransformCoordinateToolStripMenuItem.Image")));
            this.TransformCoordinateToolStripMenuItem.Name = "TransformCoordinateToolStripMenuItem";
            this.TransformCoordinateToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.TransformCoordinateToolStripMenuItem.Text = "Преобразование координат";
            this.TransformCoordinateToolStripMenuItem.ToolTipText = "Преобразование координат в разные форматы";
            this.TransformCoordinateToolStripMenuItem.Click += new System.EventHandler(this.TransformCoordinateToolStripMenuItem_Click);
            // 
            // consoleToolStripMenuItem
            // 
            this.consoleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("consoleToolStripMenuItem.Image")));
            this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
            this.consoleToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.consoleToolStripMenuItem.Text = "Консоль";
            this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(225, 6);
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OptionsToolStripMenuItem.Image")));
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.OptionsToolStripMenuItem.Text = "Настройки";
            this.OptionsToolStripMenuItem.ToolTipText = "Настройки программы";
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(225, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitToolStripMenuItem.Image")));
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // справкаToolStripMenuItem
            // 
            this.справкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.справкаToolStripMenuItem.Text = "Справка";
            this.справкаToolStripMenuItem.ToolTipText = "Помощь в использовании программы";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripMenuItem.Image")));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.helpToolStripMenuItem.Text = "Помощь";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "О программе";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelCurrentOperation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(984, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelCurrentOperation
            // 
            this.toolStripStatusLabelCurrentOperation.Name = "toolStripStatusLabelCurrentOperation";
            this.toolStripStatusLabelCurrentOperation.Size = new System.Drawing.Size(136, 17);
            this.toolStripStatusLabelCurrentOperation.Text = "Выполняется операция";
            this.toolStripStatusLabelCurrentOperation.Visible = false;
            // 
            // FormContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 562);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "FormContainer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TrackConverter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormContainer_FormClosing);
            this.Load += new System.EventHandler(this.FormContainer_Load);
            this.Resize += new System.EventHandler(this.FormContainer_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem создатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditPointFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CalculateDistanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TransformCoordinateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consoleToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        /// <summary>
        /// Строка информации о текущей операции
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCurrentOperation;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}