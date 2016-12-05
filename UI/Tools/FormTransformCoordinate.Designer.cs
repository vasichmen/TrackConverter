namespace TrackConverter.UI.Tools
{
    partial class FormTransformCoordinate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTransformCoordinate));
            this.textBoxDegLatDeg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDegLonDeg = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabelGoogle = new System.Windows.Forms.LinkLabel();
            this.linkLabelYandex = new System.Windows.Forms.LinkLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxDegminsecLon = new System.Windows.Forms.ComboBox();
            this.comboBoxDegminsecLat = new System.Windows.Forms.ComboBox();
            this.textBoxDegminsecLatText = new System.Windows.Forms.TextBox();
            this.textBoxDegminsecLonText = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxDegminsecLatSec = new System.Windows.Forms.TextBox();
            this.textBoxDegminsecLonSec = new System.Windows.Forms.TextBox();
            this.textBoxDegminsecLatMin = new System.Windows.Forms.TextBox();
            this.textBoxDegminsecLonMin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxDegminsecLatDeg = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDegminsecLonDeg = new System.Windows.Forms.TextBox();
            this.radioButtonDegMinSec = new System.Windows.Forms.RadioButton();
            this.radioButtonDegMin = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxDegminLon = new System.Windows.Forms.ComboBox();
            this.comboBoxDegminLat = new System.Windows.Forms.ComboBox();
            this.textBoxDegminLatText = new System.Windows.Forms.TextBox();
            this.textBoxDegminLonText = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxDegminLatMin = new System.Windows.Forms.TextBox();
            this.textBoxDegminLonMin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDegminLatDeg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDegminLonDeg = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.radioButtonDeg = new System.Windows.Forms.RadioButton();
            this.buttonTransform = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxDegLatDeg
            // 
            this.textBoxDegLatDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegLatDeg.Location = new System.Drawing.Point(62, 32);
            this.textBoxDegLatDeg.Name = "textBoxDegLatDeg";
            this.textBoxDegLatDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegLatDeg.TabIndex = 0;
            this.textBoxDegLatDeg.Text = "55.562303";
            this.textBoxDegLatDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Широта";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Долгота";
            // 
            // textBoxDegLonDeg
            // 
            this.textBoxDegLonDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegLonDeg.Location = new System.Drawing.Point(259, 32);
            this.textBoxDegLonDeg.Name = "textBoxDegLonDeg";
            this.textBoxDegLonDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegLonDeg.TabIndex = 2;
            this.textBoxDegLonDeg.Text = "37.042162";
            this.textBoxDegLonDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabelGoogle);
            this.groupBox1.Controls.Add(this.linkLabelYandex);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.radioButtonDegMinSec);
            this.groupBox1.Controls.Add(this.radioButtonDegMin);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.radioButtonDeg);
            this.groupBox1.Location = new System.Drawing.Point(10, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 492);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // linkLabelGoogle
            // 
            this.linkLabelGoogle.AutoSize = true;
            this.linkLabelGoogle.Location = new System.Drawing.Point(241, 23);
            this.linkLabelGoogle.Name = "linkLabelGoogle";
            this.linkLabelGoogle.Size = new System.Drawing.Size(140, 13);
            this.linkLabelGoogle.TabIndex = 7;
            this.linkLabelGoogle.TabStop = true;
            this.linkLabelGoogle.Text = "Показать на карте Google";
            this.linkLabelGoogle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGoogle_LinkClicked);
            // 
            // linkLabelYandex
            // 
            this.linkLabelYandex.AutoSize = true;
            this.linkLabelYandex.Location = new System.Drawing.Point(80, 23);
            this.linkLabelYandex.Name = "linkLabelYandex";
            this.linkLabelYandex.Size = new System.Drawing.Size(144, 13);
            this.linkLabelYandex.TabIndex = 6;
            this.linkLabelYandex.TabStop = true;
            this.linkLabelYandex.Text = "Показать на карте Яндекс";
            this.linkLabelYandex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelYandex_LinkClicked);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxDegminsecLon);
            this.groupBox4.Controls.Add(this.comboBoxDegminsecLat);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLatText);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLonText);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLatSec);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLonSec);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLatMin);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLonMin);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLatDeg);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textBoxDegminsecLonDeg);
            this.groupBox4.Location = new System.Drawing.Point(6, 323);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(375, 163);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            // 
            // comboBoxDegminsecLon
            // 
            this.comboBoxDegminsecLon.DisplayMember = "1";
            this.comboBoxDegminsecLon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDegminsecLon.FormattingEnabled = true;
            this.comboBoxDegminsecLon.Items.AddRange(new object[] {
            "Восточной долготы",
            "Западной долготы"});
            this.comboBoxDegminsecLon.Location = new System.Drawing.Point(259, 126);
            this.comboBoxDegminsecLon.Name = "comboBoxDegminsecLon";
            this.comboBoxDegminsecLon.Size = new System.Drawing.Size(102, 21);
            this.comboBoxDegminsecLon.TabIndex = 31;
            // 
            // comboBoxDegminsecLat
            // 
            this.comboBoxDegminsecLat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDegminsecLat.FormattingEnabled = true;
            this.comboBoxDegminsecLat.Items.AddRange(new object[] {
            "Северной широты",
            "Южной широты"});
            this.comboBoxDegminsecLat.Location = new System.Drawing.Point(62, 126);
            this.comboBoxDegminsecLat.Name = "comboBoxDegminsecLat";
            this.comboBoxDegminsecLat.Size = new System.Drawing.Size(102, 21);
            this.comboBoxDegminsecLat.TabIndex = 30;
            // 
            // textBoxDegminsecLatText
            // 
            this.textBoxDegminsecLatText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLatText.Location = new System.Drawing.Point(62, 100);
            this.textBoxDegminsecLatText.Name = "textBoxDegminsecLatText";
            this.textBoxDegminsecLatText.ReadOnly = true;
            this.textBoxDegminsecLatText.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLatText.TabIndex = 28;
            // 
            // textBoxDegminsecLonText
            // 
            this.textBoxDegminsecLonText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLonText.Location = new System.Drawing.Point(259, 100);
            this.textBoxDegminsecLonText.Name = "textBoxDegminsecLonText";
            this.textBoxDegminsecLonText.ReadOnly = true;
            this.textBoxDegminsecLonText.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLonText.TabIndex = 29;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 77);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 13);
            this.label18.TabIndex = 27;
            this.label18.Text = "секунды:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(201, 77);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 13);
            this.label17.TabIndex = 26;
            this.label17.Text = "секунды:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 51);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "минуты:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(201, 51);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 24;
            this.label15.Text = "минуты:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "градусы:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(201, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "градусы:";
            // 
            // textBoxDegminsecLatSec
            // 
            this.textBoxDegminsecLatSec.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLatSec.Location = new System.Drawing.Point(61, 74);
            this.textBoxDegminsecLatSec.Name = "textBoxDegminsecLatSec";
            this.textBoxDegminsecLatSec.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLatSec.TabIndex = 21;
            this.textBoxDegminsecLatSec.Text = "24.562";
            this.textBoxDegminsecLatSec.Click += new System.EventHandler(this.textBoxReset);
            // 
            // textBoxDegminsecLonSec
            // 
            this.textBoxDegminsecLonSec.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLonSec.Location = new System.Drawing.Point(258, 74);
            this.textBoxDegminsecLonSec.Name = "textBoxDegminsecLonSec";
            this.textBoxDegminsecLonSec.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLonSec.TabIndex = 22;
            this.textBoxDegminsecLonSec.Text = "13.042";
            this.textBoxDegminsecLonSec.Click += new System.EventHandler(this.textBoxReset);
            // 
            // textBoxDegminsecLatMin
            // 
            this.textBoxDegminsecLatMin.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLatMin.Location = new System.Drawing.Point(61, 48);
            this.textBoxDegminsecLatMin.Name = "textBoxDegminsecLatMin";
            this.textBoxDegminsecLatMin.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLatMin.TabIndex = 19;
            this.textBoxDegminsecLatMin.Text = "32";
            this.textBoxDegminsecLatMin.Click += new System.EventHandler(this.textBoxReset);
            // 
            // textBoxDegminsecLonMin
            // 
            this.textBoxDegminsecLonMin.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLonMin.Location = new System.Drawing.Point(258, 48);
            this.textBoxDegminsecLonMin.Name = "textBoxDegminsecLonMin";
            this.textBoxDegminsecLonMin.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLonMin.TabIndex = 20;
            this.textBoxDegminsecLonMin.Text = "12";
            this.textBoxDegminsecLonMin.Click += new System.EventHandler(this.textBoxReset);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Широта";
            // 
            // textBoxDegminsecLatDeg
            // 
            this.textBoxDegminsecLatDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLatDeg.Location = new System.Drawing.Point(61, 22);
            this.textBoxDegminsecLatDeg.Name = "textBoxDegminsecLatDeg";
            this.textBoxDegminsecLatDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLatDeg.TabIndex = 15;
            this.textBoxDegminsecLatDeg.Text = "55";
            this.textBoxDegminsecLatDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(255, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Долгота";
            // 
            // textBoxDegminsecLonDeg
            // 
            this.textBoxDegminsecLonDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminsecLonDeg.Location = new System.Drawing.Point(258, 22);
            this.textBoxDegminsecLonDeg.Name = "textBoxDegminsecLonDeg";
            this.textBoxDegminsecLonDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminsecLonDeg.TabIndex = 17;
            this.textBoxDegminsecLonDeg.Text = "37";
            this.textBoxDegminsecLonDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // radioButtonDegMinSec
            // 
            this.radioButtonDegMinSec.AutoSize = true;
            this.radioButtonDegMinSec.Location = new System.Drawing.Point(6, 300);
            this.radioButtonDegMinSec.Name = "radioButtonDegMinSec";
            this.radioButtonDegMinSec.Size = new System.Drawing.Size(161, 17);
            this.radioButtonDegMinSec.TabIndex = 4;
            this.radioButtonDegMinSec.Text = "Градусы, минуты, секунды";
            this.radioButtonDegMinSec.UseVisualStyleBackColor = true;
            this.radioButtonDegMinSec.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonDegMin
            // 
            this.radioButtonDegMin.AutoSize = true;
            this.radioButtonDegMin.Location = new System.Drawing.Point(6, 116);
            this.radioButtonDegMin.Name = "radioButtonDegMin";
            this.radioButtonDegMin.Size = new System.Drawing.Size(112, 17);
            this.radioButtonDegMin.TabIndex = 2;
            this.radioButtonDegMin.Text = "Градусы, минуты";
            this.radioButtonDegMin.UseVisualStyleBackColor = true;
            this.radioButtonDegMin.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxDegminLon);
            this.groupBox3.Controls.Add(this.comboBoxDegminLat);
            this.groupBox3.Controls.Add(this.textBoxDegminLatText);
            this.groupBox3.Controls.Add(this.textBoxDegminLonText);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textBoxDegminLatMin);
            this.groupBox3.Controls.Add(this.textBoxDegminLonMin);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxDegminLatDeg);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBoxDegminLonDeg);
            this.groupBox3.Location = new System.Drawing.Point(6, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(375, 155);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            // 
            // comboBoxDegminLon
            // 
            this.comboBoxDegminLon.DisplayMember = "1";
            this.comboBoxDegminLon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDegminLon.FormattingEnabled = true;
            this.comboBoxDegminLon.Items.AddRange(new object[] {
            "Восточной долготы",
            "Западной долготы"});
            this.comboBoxDegminLon.Location = new System.Drawing.Point(259, 90);
            this.comboBoxDegminLon.Name = "comboBoxDegminLon";
            this.comboBoxDegminLon.Size = new System.Drawing.Size(102, 21);
            this.comboBoxDegminLon.TabIndex = 22;
            // 
            // comboBoxDegminLat
            // 
            this.comboBoxDegminLat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDegminLat.FormattingEnabled = true;
            this.comboBoxDegminLat.Items.AddRange(new object[] {
            "Северной широты",
            "Южной широты"});
            this.comboBoxDegminLat.Location = new System.Drawing.Point(62, 90);
            this.comboBoxDegminLat.Name = "comboBoxDegminLat";
            this.comboBoxDegminLat.Size = new System.Drawing.Size(102, 21);
            this.comboBoxDegminLat.TabIndex = 21;
            // 
            // textBoxDegminLatText
            // 
            this.textBoxDegminLatText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLatText.Location = new System.Drawing.Point(62, 117);
            this.textBoxDegminLatText.Name = "textBoxDegminLatText";
            this.textBoxDegminLatText.ReadOnly = true;
            this.textBoxDegminLatText.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLatText.TabIndex = 19;
            // 
            // textBoxDegminLonText
            // 
            this.textBoxDegminLonText.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLonText.Location = new System.Drawing.Point(259, 117);
            this.textBoxDegminLonText.Name = "textBoxDegminLonText";
            this.textBoxDegminLonText.ReadOnly = true;
            this.textBoxDegminLonText.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLonText.TabIndex = 20;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 18;
            this.label14.Text = "минуты:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(201, 67);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 17;
            this.label13.Text = "минуты:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "градусы:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(201, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "градусы:";
            // 
            // textBoxDegminLatMin
            // 
            this.textBoxDegminLatMin.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLatMin.Location = new System.Drawing.Point(62, 64);
            this.textBoxDegminLatMin.Name = "textBoxDegminLatMin";
            this.textBoxDegminLatMin.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLatMin.TabIndex = 8;
            this.textBoxDegminLatMin.Text = "33.562";
            this.textBoxDegminLatMin.Click += new System.EventHandler(this.textBoxReset);
            // 
            // textBoxDegminLonMin
            // 
            this.textBoxDegminLonMin.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLonMin.Location = new System.Drawing.Point(259, 64);
            this.textBoxDegminLonMin.Name = "textBoxDegminLonMin";
            this.textBoxDegminLonMin.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLonMin.TabIndex = 10;
            this.textBoxDegminLonMin.Text = "18.042";
            this.textBoxDegminLonMin.Click += new System.EventHandler(this.textBoxReset);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Широта";
            // 
            // textBoxDegminLatDeg
            // 
            this.textBoxDegminLatDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLatDeg.Location = new System.Drawing.Point(62, 38);
            this.textBoxDegminLatDeg.Name = "textBoxDegminLatDeg";
            this.textBoxDegminLatDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLatDeg.TabIndex = 4;
            this.textBoxDegminLatDeg.Text = "55";
            this.textBoxDegminLatDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(256, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Долгота";
            // 
            // textBoxDegminLonDeg
            // 
            this.textBoxDegminLonDeg.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxDegminLonDeg.Location = new System.Drawing.Point(259, 38);
            this.textBoxDegminLonDeg.Name = "textBoxDegminLonDeg";
            this.textBoxDegminLonDeg.Size = new System.Drawing.Size(102, 20);
            this.textBoxDegminLonDeg.TabIndex = 6;
            this.textBoxDegminLonDeg.Text = "37";
            this.textBoxDegminLonDeg.Click += new System.EventHandler(this.textBoxReset);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBoxDegLatDeg);
            this.groupBox2.Controls.Add(this.textBoxDegLonDeg);
            this.groupBox2.Location = new System.Drawing.Point(6, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(375, 68);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(201, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "градусы:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "градусы:";
            // 
            // radioButtonDeg
            // 
            this.radioButtonDeg.AutoSize = true;
            this.radioButtonDeg.Checked = true;
            this.radioButtonDeg.Location = new System.Drawing.Point(6, 19);
            this.radioButtonDeg.Name = "radioButtonDeg";
            this.radioButtonDeg.Size = new System.Drawing.Size(68, 17);
            this.radioButtonDeg.TabIndex = 0;
            this.radioButtonDeg.TabStop = true;
            this.radioButtonDeg.Text = "Градусы";
            this.radioButtonDeg.UseVisualStyleBackColor = true;
            this.radioButtonDeg.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // buttonTransform
            // 
            this.buttonTransform.Location = new System.Drawing.Point(10, 509);
            this.buttonTransform.Name = "buttonTransform";
            this.buttonTransform.Size = new System.Drawing.Size(391, 23);
            this.buttonTransform.TabIndex = 5;
            this.buttonTransform.Text = "Рассчитать";
            this.buttonTransform.UseVisualStyleBackColor = true;
            this.buttonTransform.Click += new System.EventHandler(this.buttonTransform_Click);
            // 
            // FormTransformCoordinate
            // 
            this.AcceptButton = this.buttonTransform;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 562);
            this.Controls.Add(this.buttonTransform);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTransformCoordinate";
            this.Text = "Преобразование координат";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDegLatDeg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDegLonDeg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxDegminsecLatSec;
        private System.Windows.Forms.TextBox textBoxDegminsecLonSec;
        private System.Windows.Forms.TextBox textBoxDegminsecLatMin;
        private System.Windows.Forms.TextBox textBoxDegminsecLonMin;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxDegminsecLatDeg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDegminsecLonDeg;
        private System.Windows.Forms.RadioButton radioButtonDegMinSec;
        private System.Windows.Forms.RadioButton radioButtonDegMin;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxDegminLatMin;
        private System.Windows.Forms.TextBox textBoxDegminLonMin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDegminLatDeg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDegminLonDeg;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonDeg;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxDegminsecLatText;
        private System.Windows.Forms.TextBox textBoxDegminsecLonText;
        private System.Windows.Forms.TextBox textBoxDegminLatText;
        private System.Windows.Forms.TextBox textBoxDegminLonText;
        private System.Windows.Forms.Button buttonTransform;
        private System.Windows.Forms.ComboBox comboBoxDegminLon;
        private System.Windows.Forms.ComboBox comboBoxDegminLat;
        private System.Windows.Forms.ComboBox comboBoxDegminsecLon;
        private System.Windows.Forms.ComboBox comboBoxDegminsecLat;
        private System.Windows.Forms.LinkLabel linkLabelGoogle;
        private System.Windows.Forms.LinkLabel linkLabelYandex;
    }
}