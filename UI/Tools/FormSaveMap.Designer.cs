namespace TrackConverter.UI.Tools
{
    partial class FormSaveMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaveMap));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancelClose = new System.Windows.Forms.Button();
            this.checkBoxMapLinkingFile = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSaveMapProvider = new System.Windows.Forms.ComboBox();
            this.buttonSelectSaveFile = new System.Windows.Forms.Button();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.progressBarProgress = new System.Windows.Forms.ProgressBar();
            this.checkBoxSaveObjects = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxWLinkingFile = new System.Windows.Forms.CheckBox();
            this.numericUpDownZoom = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(162, 152);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Начать";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonCancelClose
            // 
            this.buttonCancelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancelClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancelClose.Location = new System.Drawing.Point(243, 152);
            this.buttonCancelClose.Name = "buttonCancelClose";
            this.buttonCancelClose.Size = new System.Drawing.Size(75, 23);
            this.buttonCancelClose.TabIndex = 2;
            this.buttonCancelClose.Text = "Закрыть";
            this.buttonCancelClose.UseVisualStyleBackColor = true;
            this.buttonCancelClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // checkBoxMapLinkingFile
            // 
            this.checkBoxMapLinkingFile.AutoSize = true;
            this.checkBoxMapLinkingFile.Checked = true;
            this.checkBoxMapLinkingFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMapLinkingFile.Location = new System.Drawing.Point(10, 65);
            this.checkBoxMapLinkingFile.Name = "checkBoxMapLinkingFile";
            this.checkBoxMapLinkingFile.Size = new System.Drawing.Size(132, 17);
            this.checkBoxMapLinkingFile.TabIndex = 16;
            this.checkBoxMapLinkingFile.Text = "Файл привязки .map";
            this.toolTip1.SetToolTip(this.checkBoxMapLinkingFile, "В папке с файлом будет создан файл привязки карты .map");
            this.checkBoxMapLinkingFile.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Тип карты";
            // 
            // comboBoxSaveMapProvider
            // 
            this.comboBoxSaveMapProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSaveMapProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSaveMapProvider.FormattingEnabled = true;
            this.comboBoxSaveMapProvider.Location = new System.Drawing.Point(73, 38);
            this.comboBoxSaveMapProvider.Name = "comboBoxSaveMapProvider";
            this.comboBoxSaveMapProvider.Size = new System.Drawing.Size(139, 21);
            this.comboBoxSaveMapProvider.TabIndex = 14;
            // 
            // buttonSelectSaveFile
            // 
            this.buttonSelectSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectSaveFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectSaveFile.Location = new System.Drawing.Point(289, 11);
            this.buttonSelectSaveFile.Name = "buttonSelectSaveFile";
            this.buttonSelectSaveFile.Size = new System.Drawing.Size(29, 20);
            this.buttonSelectSaveFile.TabIndex = 13;
            this.buttonSelectSaveFile.Text = "...";
            this.buttonSelectSaveFile.UseVisualStyleBackColor = true;
            this.buttonSelectSaveFile.Click += new System.EventHandler(this.buttonSelectSaveFile_Click);
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSavePath.Location = new System.Drawing.Point(99, 12);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.ReadOnly = true;
            this.textBoxSavePath.Size = new System.Drawing.Size(184, 20);
            this.textBoxSavePath.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Куда сохранять";
            // 
            // progressBarProgress
            // 
            this.progressBarProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarProgress.Location = new System.Drawing.Point(10, 123);
            this.progressBarProgress.Name = "progressBarProgress";
            this.progressBarProgress.Size = new System.Drawing.Size(308, 23);
            this.progressBarProgress.TabIndex = 17;
            // 
            // checkBoxSaveObjects
            // 
            this.checkBoxSaveObjects.AutoSize = true;
            this.checkBoxSaveObjects.Location = new System.Drawing.Point(148, 65);
            this.checkBoxSaveObjects.Name = "checkBoxSaveObjects";
            this.checkBoxSaveObjects.Size = new System.Drawing.Size(126, 17);
            this.checkBoxSaveObjects.TabIndex = 18;
            this.checkBoxSaveObjects.Text = "Сохранять объекты";
            this.toolTip1.SetToolTip(this.checkBoxSaveObjects, "На карту будут перенесены маршруты и маркеры");
            this.checkBoxSaveObjects.UseVisualStyleBackColor = true;
            // 
            // checkBoxWLinkingFile
            // 
            this.checkBoxWLinkingFile.AutoSize = true;
            this.checkBoxWLinkingFile.Location = new System.Drawing.Point(10, 88);
            this.checkBoxWLinkingFile.Name = "checkBoxWLinkingFile";
            this.checkBoxWLinkingFile.Size = new System.Drawing.Size(120, 17);
            this.checkBoxWLinkingFile.TabIndex = 21;
            this.checkBoxWLinkingFile.Text = "Файл привязки .w";
            this.toolTip1.SetToolTip(this.checkBoxWLinkingFile, "В папке с файлом будет создан файл привязки карты .map");
            this.checkBoxWLinkingFile.UseVisualStyleBackColor = true;
            // 
            // numericUpDownZoom
            // 
            this.numericUpDownZoom.Location = new System.Drawing.Point(277, 39);
            this.numericUpDownZoom.Name = "numericUpDownZoom";
            this.numericUpDownZoom.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownZoom.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Масштаб";
            // 
            // FormSaveMap
            // 
            this.AcceptButton = this.buttonStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancelClose;
            this.ClientSize = new System.Drawing.Size(327, 187);
            this.Controls.Add(this.checkBoxWLinkingFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownZoom);
            this.Controls.Add(this.checkBoxSaveObjects);
            this.Controls.Add(this.progressBarProgress);
            this.Controls.Add(this.checkBoxMapLinkingFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxSaveMapProvider);
            this.Controls.Add(this.buttonSelectSaveFile);
            this.Controls.Add(this.textBoxSavePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonCancelClose);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSaveMap";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Сохранение карты";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancelClose;
        private System.Windows.Forms.CheckBox checkBoxMapLinkingFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSaveMapProvider;
        private System.Windows.Forms.Button buttonSelectSaveFile;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar progressBarProgress;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxSaveObjects;
        private System.Windows.Forms.NumericUpDown numericUpDownZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxWLinkingFile;
    }
}