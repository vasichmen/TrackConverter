namespace TrackConverter.UI.Map {
    partial class FormSelectMapActions {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && (components != null) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectMapActions));
            this.tabControlActions = new System.Windows.Forms.TabControl();
            this.tabPageLoad = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.checkedListBoxZoom = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxLoadMapProvider = new System.Windows.Forms.ComboBox();
            this.tabPageSave = new System.Windows.Forms.TabPage();
            this.checkBoxMapLinkingFile = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxSaveMapProvider = new System.Windows.Forms.ComboBox();
            this.buttonSelectSaveFile = new System.Windows.Forms.Button();
            this.textBoxSavePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxSaveImageFormat = new System.Windows.Forms.ComboBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControlActions.SuspendLayout();
            this.tabPageLoad.SuspendLayout();
            this.tabPageSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlActions
            // 
            this.tabControlActions.Controls.Add(this.tabPageLoad);
            this.tabControlActions.Controls.Add(this.tabPageSave);
            this.tabControlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlActions.Location = new System.Drawing.Point(0, 0);
            this.tabControlActions.Name = "tabControlActions";
            this.tabControlActions.SelectedIndex = 0;
            this.tabControlActions.Size = new System.Drawing.Size(542, 347);
            this.tabControlActions.TabIndex = 0;
            // 
            // tabPageLoad
            // 
            this.tabPageLoad.Controls.Add(this.label2);
            this.tabPageLoad.Controls.Add(this.checkedListBoxZoom);
            this.tabPageLoad.Controls.Add(this.label1);
            this.tabPageLoad.Controls.Add(this.comboBoxLoadMapProvider);
            this.tabPageLoad.Location = new System.Drawing.Point(4, 22);
            this.tabPageLoad.Name = "tabPageLoad";
            this.tabPageLoad.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLoad.Size = new System.Drawing.Size(534, 321);
            this.tabPageLoad.TabIndex = 0;
            this.tabPageLoad.Text = "Загрузить";
            this.tabPageLoad.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Масштаб";
            // 
            // checkedListBoxZoom
            // 
            this.checkedListBoxZoom.FormattingEnabled = true;
            this.checkedListBoxZoom.Location = new System.Drawing.Point(11, 65);
            this.checkedListBoxZoom.Name = "checkedListBoxZoom";
            this.checkedListBoxZoom.Size = new System.Drawing.Size(151, 244);
            this.checkedListBoxZoom.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Карта";
            // 
            // comboBoxLoadMapProvider
            // 
            this.comboBoxLoadMapProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLoadMapProvider.FormattingEnabled = true;
            this.comboBoxLoadMapProvider.Items.AddRange(new object[] {
            "Google.Гибрид",
            "Google.Карта",
            "Google.Спутник",
            "OSM Cycle Map",
            "Яндекс.Гибрид",
            "Яндекс.Карта",
            "Яндекс.Спутник"});
            this.comboBoxLoadMapProvider.Location = new System.Drawing.Point(11, 25);
            this.comboBoxLoadMapProvider.Name = "comboBoxLoadMapProvider";
            this.comboBoxLoadMapProvider.Size = new System.Drawing.Size(176, 21);
            this.comboBoxLoadMapProvider.TabIndex = 0;
            // 
            // tabPageSave
            // 
            this.tabPageSave.Controls.Add(this.checkBoxMapLinkingFile);
            this.tabPageSave.Controls.Add(this.label5);
            this.tabPageSave.Controls.Add(this.comboBoxSaveMapProvider);
            this.tabPageSave.Controls.Add(this.buttonSelectSaveFile);
            this.tabPageSave.Controls.Add(this.textBoxSavePath);
            this.tabPageSave.Controls.Add(this.label4);
            this.tabPageSave.Controls.Add(this.label3);
            this.tabPageSave.Controls.Add(this.comboBoxSaveImageFormat);
            this.tabPageSave.Location = new System.Drawing.Point(4, 22);
            this.tabPageSave.Name = "tabPageSave";
            this.tabPageSave.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSave.Size = new System.Drawing.Size(534, 321);
            this.tabPageSave.TabIndex = 1;
            this.tabPageSave.Text = "Сохранить";
            this.tabPageSave.UseVisualStyleBackColor = true;
            // 
            // checkBoxMapLinkingFile
            // 
            this.checkBoxMapLinkingFile.AutoSize = true;
            this.checkBoxMapLinkingFile.Checked = true;
            this.checkBoxMapLinkingFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMapLinkingFile.Location = new System.Drawing.Point(11, 86);
            this.checkBoxMapLinkingFile.Name = "checkBoxMapLinkingFile";
            this.checkBoxMapLinkingFile.Size = new System.Drawing.Size(132, 17);
            this.checkBoxMapLinkingFile.TabIndex = 8;
            this.checkBoxMapLinkingFile.Text = "Файл привязки .map";
            this.checkBoxMapLinkingFile.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Тип карты";
            // 
            // comboBoxSaveMapProvider
            // 
            this.comboBoxSaveMapProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSaveMapProvider.FormattingEnabled = true;
            this.comboBoxSaveMapProvider.Items.AddRange(new object[] {
            "Google.Гибрид",
            "Google.Карта",
            "Google.Спутник",
            "OSM Cycle Map",
            "Яндекс.Гибрид",
            "Яндекс.Карта",
            "Яндекс.Спутник"});
            this.comboBoxSaveMapProvider.Location = new System.Drawing.Point(74, 59);
            this.comboBoxSaveMapProvider.Name = "comboBoxSaveMapProvider";
            this.comboBoxSaveMapProvider.Size = new System.Drawing.Size(176, 21);
            this.comboBoxSaveMapProvider.TabIndex = 6;
            // 
            // buttonSelectSaveFile
            // 
            this.buttonSelectSaveFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonSelectSaveFile.Location = new System.Drawing.Point(499, 33);
            this.buttonSelectSaveFile.Name = "buttonSelectSaveFile";
            this.buttonSelectSaveFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectSaveFile.TabIndex = 5;
            this.buttonSelectSaveFile.Text = "...";
            this.buttonSelectSaveFile.UseVisualStyleBackColor = true;
            this.buttonSelectSaveFile.Click += new System.EventHandler(this.buttonSelectSaveFile_Click);
            // 
            // textBoxSavePath
            // 
            this.textBoxSavePath.Location = new System.Drawing.Point(100, 33);
            this.textBoxSavePath.Name = "textBoxSavePath";
            this.textBoxSavePath.ReadOnly = true;
            this.textBoxSavePath.Size = new System.Drawing.Size(393, 20);
            this.textBoxSavePath.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Куда сохранять";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Результирующий формат";
            // 
            // comboBoxSaveImageFormat
            // 
            this.comboBoxSaveImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSaveImageFormat.FormattingEnabled = true;
            this.comboBoxSaveImageFormat.Items.AddRange(new object[] {
            "JPEG"});
            this.comboBoxSaveImageFormat.Location = new System.Drawing.Point(150, 6);
            this.comboBoxSaveImageFormat.Name = "comboBoxSaveImageFormat";
            this.comboBoxSaveImageFormat.Size = new System.Drawing.Size(176, 21);
            this.comboBoxSaveImageFormat.TabIndex = 1;
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStart.Location = new System.Drawing.Point(376, 349);
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
            this.buttonCancel.Location = new System.Drawing.Point(457, 349);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormSelectMapActions
            // 
            this.AcceptButton = this.buttonStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(542, 381);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.tabControlActions);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSelectMapActions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormSaveMap";
            this.tabControlActions.ResumeLayout(false);
            this.tabPageLoad.ResumeLayout(false);
            this.tabPageLoad.PerformLayout();
            this.tabPageSave.ResumeLayout(false);
            this.tabPageSave.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlActions;
        private System.Windows.Forms.TabPage tabPageLoad;
        private System.Windows.Forms.TabPage tabPageSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxLoadMapProvider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox checkedListBoxZoom;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxSaveImageFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSavePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonSelectSaveFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxSaveMapProvider;
        private System.Windows.Forms.CheckBox checkBoxMapLinkingFile;

    }
}