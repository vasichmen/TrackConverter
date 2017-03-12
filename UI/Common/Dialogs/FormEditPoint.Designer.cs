namespace TrackConverter.UI.Common.Dialogs
{
    partial class FormEditPoint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditPoint));
            this.textBoxAlt = new System.Windows.Forms.TextBox();
            this.textBoxLon = new System.Windows.Forms.TextBox();
            this.textBoxLat = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dateTimePickerTime = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabelGetElevation = new System.Windows.Forms.LinkLabel();
            this.linkLabelFindCoordinates = new System.Windows.Forms.LinkLabel();
            this.linkLabelGetLink = new System.Windows.Forms.LinkLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBold = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonItalic = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUnderline = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAlignLeft = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAlignRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonInsImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInsLink = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPreview = new System.Windows.Forms.ToolStripButton();
            this.comboBoxSelectImage = new System.Windows.Forms.ComboBox();
            this.buttonAdditionInfo = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comboBoxPointType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxAlt
            // 
            this.textBoxAlt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAlt.Location = new System.Drawing.Point(99, 84);
            this.textBoxAlt.Name = "textBoxAlt";
            this.textBoxAlt.Size = new System.Drawing.Size(233, 20);
            this.textBoxAlt.TabIndex = 0;
            // 
            // textBoxLon
            // 
            this.textBoxLon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLon.Location = new System.Drawing.Point(99, 58);
            this.textBoxLon.Name = "textBoxLon";
            this.textBoxLon.Size = new System.Drawing.Size(233, 20);
            this.textBoxLon.TabIndex = 1;
            // 
            // textBoxLat
            // 
            this.textBoxLat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLat.Location = new System.Drawing.Point(99, 32);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.Size = new System.Drawing.Size(233, 20);
            this.textBoxLat.TabIndex = 2;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(99, 6);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(233, 20);
            this.textBoxName.TabIndex = 3;
            this.textBoxName.Text = "Имя точки";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(15, 204);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(406, 133);
            this.textBoxDescription.TabIndex = 4;
            this.toolTip1.SetToolTip(this.textBoxDescription, "Подробное описание точки. Можно использовать HTML");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Описание";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Дата";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Высота, м";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Долгота, º";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Широта, º";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Название";
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerDate.Location = new System.Drawing.Point(99, 110);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(233, 20);
            this.dateTimePickerDate.TabIndex = 13;
            this.dateTimePickerDate.Value = new System.DateTime(2016, 4, 18, 0, 0, 0, 0);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(15, 343);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Сохранить";
            this.toolTip1.SetToolTip(this.buttonSave, "Сохранить изменения");
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(341, 343);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // dateTimePickerTime
            // 
            this.dateTimePickerTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerTime.Location = new System.Drawing.Point(99, 136);
            this.dateTimePickerTime.Name = "dateTimePickerTime";
            this.dateTimePickerTime.Size = new System.Drawing.Size(150, 20);
            this.dateTimePickerTime.TabIndex = 17;
            this.dateTimePickerTime.Value = new System.DateTime(2016, 4, 18, 0, 0, 0, 0);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Время";
            // 
            // linkLabelGetElevation
            // 
            this.linkLabelGetElevation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelGetElevation.AutoSize = true;
            this.linkLabelGetElevation.Location = new System.Drawing.Point(338, 87);
            this.linkLabelGetElevation.Name = "linkLabelGetElevation";
            this.linkLabelGetElevation.Size = new System.Drawing.Size(83, 13);
            this.linkLabelGetElevation.TabIndex = 18;
            this.linkLabelGetElevation.TabStop = true;
            this.linkLabelGetElevation.Text = "Узнать высоту";
            this.linkLabelGetElevation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGetElevation_LinkClicked);
            // 
            // linkLabelFindCoordinates
            // 
            this.linkLabelFindCoordinates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelFindCoordinates.AutoSize = true;
            this.linkLabelFindCoordinates.Location = new System.Drawing.Point(338, 9);
            this.linkLabelFindCoordinates.Name = "linkLabelFindCoordinates";
            this.linkLabelFindCoordinates.Size = new System.Drawing.Size(71, 13);
            this.linkLabelFindCoordinates.TabIndex = 19;
            this.linkLabelFindCoordinates.TabStop = true;
            this.linkLabelFindCoordinates.Text = "Найти адрес";
            this.toolTip1.SetToolTip(this.linkLabelFindCoordinates, "На основании используемого геокодера найти адрес координат");
            this.linkLabelFindCoordinates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFindCoordinates_LinkClicked);
            // 
            // linkLabelGetLink
            // 
            this.linkLabelGetLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelGetLink.AutoSize = true;
            this.linkLabelGetLink.Location = new System.Drawing.Point(339, 46);
            this.linkLabelGetLink.Name = "linkLabelGetLink";
            this.linkLabelGetLink.Size = new System.Drawing.Size(46, 13);
            this.linkLabelGetLink.TabIndex = 20;
            this.linkLabelGetLink.TabStop = true;
            this.linkLabelGetLink.Text = "Ссылка";
            this.toolTip1.SetToolTip(this.linkLabelGetLink, "Узнать ссылку на точку в картах");
            this.linkLabelGetLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGetLink_LinkClicked);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBold,
            this.toolStripButtonItalic,
            this.toolStripButtonUnderline,
            this.toolStripSeparator1,
            this.toolStripButtonAlignLeft,
            this.toolStripButtonAlignCenter,
            this.toolStripButtonAlignRight,
            this.toolStripSeparator2,
            this.toolStripButtonInsImage,
            this.toolStripButtonInsLink,
            this.toolStripSeparator3,
            this.toolStripButtonPreview});
            this.toolStrip1.Location = new System.Drawing.Point(432, 162);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(228, 25);
            this.toolStrip1.TabIndex = 22;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonBold
            // 
            this.toolStripButtonBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBold.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBold.Image")));
            this.toolStripButtonBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBold.Name = "toolStripButtonBold";
            this.toolStripButtonBold.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBold.Text = "toolStripButton3";
            this.toolStripButtonBold.ToolTipText = "Жирный";
            this.toolStripButtonBold.Click += new System.EventHandler(this.toolStripButtonBold_Click);
            // 
            // toolStripButtonItalic
            // 
            this.toolStripButtonItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonItalic.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonItalic.Image")));
            this.toolStripButtonItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonItalic.Name = "toolStripButtonItalic";
            this.toolStripButtonItalic.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonItalic.Text = "toolStripButton4";
            this.toolStripButtonItalic.ToolTipText = "Курсив";
            this.toolStripButtonItalic.Click += new System.EventHandler(this.toolStripButtonItalic_Click);
            // 
            // toolStripButtonUnderline
            // 
            this.toolStripButtonUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUnderline.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUnderline.Image")));
            this.toolStripButtonUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUnderline.Name = "toolStripButtonUnderline";
            this.toolStripButtonUnderline.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUnderline.Text = "toolStripButton5";
            this.toolStripButtonUnderline.ToolTipText = "Подчеркнутый";
            this.toolStripButtonUnderline.Click += new System.EventHandler(this.toolStripButtonUnderline_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAlignLeft
            // 
            this.toolStripButtonAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAlignLeft.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAlignLeft.Image")));
            this.toolStripButtonAlignLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAlignLeft.Name = "toolStripButtonAlignLeft";
            this.toolStripButtonAlignLeft.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAlignLeft.Text = "toolStripButton6";
            this.toolStripButtonAlignLeft.ToolTipText = "Выровнять по левому краю";
            this.toolStripButtonAlignLeft.Click += new System.EventHandler(this.toolStripButtonAlignLeft_Click);
            // 
            // toolStripButtonAlignCenter
            // 
            this.toolStripButtonAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAlignCenter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAlignCenter.Image")));
            this.toolStripButtonAlignCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAlignCenter.Name = "toolStripButtonAlignCenter";
            this.toolStripButtonAlignCenter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAlignCenter.Text = "toolStripButton7";
            this.toolStripButtonAlignCenter.ToolTipText = "Выровнять по центру";
            this.toolStripButtonAlignCenter.Click += new System.EventHandler(this.toolStripButtonAlignCenter_Click);
            // 
            // toolStripButtonAlignRight
            // 
            this.toolStripButtonAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAlignRight.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAlignRight.Image")));
            this.toolStripButtonAlignRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAlignRight.Name = "toolStripButtonAlignRight";
            this.toolStripButtonAlignRight.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAlignRight.Text = "toolStripButton8";
            this.toolStripButtonAlignRight.ToolTipText = "Выровнять по правому краю";
            this.toolStripButtonAlignRight.Click += new System.EventHandler(this.toolStripButtonAlignRight_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonInsImage
            // 
            this.toolStripButtonInsImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInsImage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInsImage.Image")));
            this.toolStripButtonInsImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInsImage.Name = "toolStripButtonInsImage";
            this.toolStripButtonInsImage.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonInsImage.Text = "toolStripButton1";
            this.toolStripButtonInsImage.ToolTipText = "Добавить изображение";
            this.toolStripButtonInsImage.Click += new System.EventHandler(this.toolStripButtonInsImage_Click);
            // 
            // toolStripButtonInsLink
            // 
            this.toolStripButtonInsLink.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInsLink.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInsLink.Image")));
            this.toolStripButtonInsLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInsLink.Name = "toolStripButtonInsLink";
            this.toolStripButtonInsLink.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonInsLink.Text = "toolStripButton2";
            this.toolStripButtonInsLink.ToolTipText = "Добавить ссылку";
            this.toolStripButtonInsLink.Click += new System.EventHandler(this.toolStripButtonInsLink_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPreview
            // 
            this.toolStripButtonPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPreview.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPreview.Image")));
            this.toolStripButtonPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPreview.Name = "toolStripButtonPreview";
            this.toolStripButtonPreview.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPreview.Text = "toolStripButton1";
            this.toolStripButtonPreview.ToolTipText = "Открыть в окне просмотра";
            this.toolStripButtonPreview.Click += new System.EventHandler(this.toolStripButtonPreview_Click);
            // 
            // comboBoxSelectImage
            // 
            this.comboBoxSelectImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSelectImage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxSelectImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectImage.FormattingEnabled = true;
            this.comboBoxSelectImage.ItemHeight = 40;
            this.comboBoxSelectImage.Location = new System.Drawing.Point(255, 136);
            this.comboBoxSelectImage.Name = "comboBoxSelectImage";
            this.comboBoxSelectImage.Size = new System.Drawing.Size(77, 46);
            this.comboBoxSelectImage.TabIndex = 23;
            this.toolTip1.SetToolTip(this.comboBoxSelectImage, "Картинка, которая будет отображаться на карте");
            this.comboBoxSelectImage.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            // 
            // buttonAdditionInfo
            // 
            this.buttonAdditionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAdditionInfo.Location = new System.Drawing.Point(143, 343);
            this.buttonAdditionInfo.Name = "buttonAdditionInfo";
            this.buttonAdditionInfo.Size = new System.Drawing.Size(146, 23);
            this.buttonAdditionInfo.TabIndex = 24;
            this.buttonAdditionInfo.Text = "Подробная информация";
            this.toolTip1.SetToolTip(this.buttonAdditionInfo, "Открыть окно с подробной информацией о точке");
            this.buttonAdditionInfo.UseVisualStyleBackColor = true;
            this.buttonAdditionInfo.Click += new System.EventHandler(this.buttonAdditionInfo_Click);
            // 
            // comboBoxPointType
            // 
            this.comboBoxPointType.FormattingEnabled = true;
            this.comboBoxPointType.Items.AddRange(new object[] {
            "Старт",
            "Достопримечательность",
            "Точка сбора",
            "Привал",
            "Место для ночёвки",
            "Финиш",
            "Точка"});
            this.comboBoxPointType.Location = new System.Drawing.Point(99, 162);
            this.comboBoxPointType.Name = "comboBoxPointType";
            this.comboBoxPointType.Size = new System.Drawing.Size(150, 21);
            this.comboBoxPointType.TabIndex = 26;
            this.toolTip1.SetToolTip(this.comboBoxPointType, "Тип точки в маршруте");
            this.comboBoxPointType.SelectedIndexChanged += new System.EventHandler(this.comboBoxPointType_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Тип точки";
            // 
            // FormEditPoint
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(426, 371);
            this.Controls.Add(this.comboBoxPointType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.buttonAdditionInfo);
            this.Controls.Add(this.comboBoxSelectImage);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.linkLabelGetLink);
            this.Controls.Add(this.linkLabelFindCoordinates);
            this.Controls.Add(this.linkLabelGetElevation);
            this.Controls.Add(this.dateTimePickerTime);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.dateTimePickerDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxLat);
            this.Controls.Add(this.textBoxLon);
            this.Controls.Add(this.textBoxAlt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(442, 377);
            this.Name = "FormEditPoint";
            this.ShowInTaskbar = false;
            this.Text = "Редактирование точки";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAlt;
        private System.Windows.Forms.TextBox textBoxLon;
        private System.Windows.Forms.TextBox textBoxLat;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePickerDate;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DateTimePicker dateTimePickerTime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabelGetElevation;
        private System.Windows.Forms.LinkLabel linkLabelFindCoordinates;
        private System.Windows.Forms.LinkLabel linkLabelGetLink;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonBold;
        private System.Windows.Forms.ToolStripButton toolStripButtonItalic;
        private System.Windows.Forms.ToolStripButton toolStripButtonUnderline;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAlignLeft;
        private System.Windows.Forms.ToolStripButton toolStripButtonAlignCenter;
        private System.Windows.Forms.ToolStripButton toolStripButtonAlignRight;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsImage;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsLink;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonPreview;
        private System.Windows.Forms.ComboBox comboBoxSelectImage;
        private System.Windows.Forms.Button buttonAdditionInfo;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxPointType;
    }
}