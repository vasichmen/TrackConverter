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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxMapLinkingFile = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSaveMapProvider = new System.Windows.Forms.ComboBox();
            this.buttonSelectSaveFile = new System.Windows.Forms.Button();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSaveImageFormat = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.checkBoxSaveObjects = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(163, 146);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Начать";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(244, 146);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxMapLinkingFile
            // 
            this.checkBoxMapLinkingFile.AutoSize = true;
            this.checkBoxMapLinkingFile.Checked = true;
            this.checkBoxMapLinkingFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMapLinkingFile.Location = new System.Drawing.Point(11, 92);
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
            this.label5.Location = new System.Drawing.Point(8, 68);
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
            this.comboBoxSaveMapProvider.Location = new System.Drawing.Point(74, 65);
            this.comboBoxSaveMapProvider.Name = "comboBoxSaveMapProvider";
            this.comboBoxSaveMapProvider.Size = new System.Drawing.Size(245, 21);
            this.comboBoxSaveMapProvider.TabIndex = 14;
            // 
            // buttonSelectSaveFile
            // 
            this.buttonSelectSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectSaveFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectSaveFile.Location = new System.Drawing.Point(291, 38);
            this.buttonSelectSaveFile.Name = "buttonSelectSaveFile";
            this.buttonSelectSaveFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectSaveFile.TabIndex = 13;
            this.buttonSelectSaveFile.Text = "...";
            this.buttonSelectSaveFile.UseVisualStyleBackColor = true;
            this.buttonSelectSaveFile.Click += new System.EventHandler(this.buttonSelectSaveFile_Click);
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSavePath.Location = new System.Drawing.Point(100, 39);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.ReadOnly = true;
            this.textBoxSavePath.Size = new System.Drawing.Size(183, 20);
            this.textBoxSavePath.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Куда сохранять";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Результирующий формат";
            // 
            // comboBoxSaveImageFormat
            // 
            this.comboBoxSaveImageFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSaveImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSaveImageFormat.FormattingEnabled = true;
            this.comboBoxSaveImageFormat.Items.AddRange(new object[] {
            "JPEG"});
            this.comboBoxSaveImageFormat.Location = new System.Drawing.Point(150, 12);
            this.comboBoxSaveImageFormat.Name = "comboBoxSaveImageFormat";
            this.comboBoxSaveImageFormat.Size = new System.Drawing.Size(169, 21);
            this.comboBoxSaveImageFormat.TabIndex = 9;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(11, 115);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(308, 23);
            this.progressBar1.TabIndex = 17;
            // 
            // checkBoxSaveObjects
            // 
            this.checkBoxSaveObjects.AutoSize = true;
            this.checkBoxSaveObjects.Checked = true;
            this.checkBoxSaveObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSaveObjects.Location = new System.Drawing.Point(149, 92);
            this.checkBoxSaveObjects.Name = "checkBoxSaveObjects";
            this.checkBoxSaveObjects.Size = new System.Drawing.Size(126, 17);
            this.checkBoxSaveObjects.TabIndex = 18;
            this.checkBoxSaveObjects.Text = "Сохранять объекты";
            this.toolTip1.SetToolTip(this.checkBoxSaveObjects, "На карту будут перенесены маршруты и маркеры");
            this.checkBoxSaveObjects.UseVisualStyleBackColor = true;
            // 
            // FormSaveMap
            // 
            this.AcceptButton = this.buttonStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(327, 178);
            this.Controls.Add(this.checkBoxSaveObjects);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkBoxMapLinkingFile);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxSaveMapProvider);
            this.Controls.Add(this.buttonSelectSaveFile);
            this.Controls.Add(this.textBoxSavePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxSaveImageFormat);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSaveMap";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Сохранение карты";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxMapLinkingFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSaveMapProvider;
        private System.Windows.Forms.Button buttonSelectSaveFile;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxSaveImageFormat;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxSaveObjects;
    }
}