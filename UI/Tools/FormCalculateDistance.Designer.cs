namespace TrackConverter.UI.Tools
{
    partial class FormCalculateDistance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCalculateDistance));
            this.textBoxLat1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxLon1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLat2 = new System.Windows.Forms.TextBox();
            this.textBoxLon2 = new System.Windows.Forms.TextBox();
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.labelDistance = new System.Windows.Forms.Label();
            this.labelTrueAzimuth = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonModGaver = new System.Windows.Forms.RadioButton();
            this.radioButtonGaversin = new System.Windows.Forms.RadioButton();
            this.radioButtonSphere = new System.Windows.Forms.RadioButton();
            this.radioButtonPifagor = new System.Windows.Forms.RadioButton();
            this.labelMagneticAzimuth = new System.Windows.Forms.Label();
            this.labelMagneticDiv = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxLat1
            // 
            this.textBoxLat1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxLat1.Location = new System.Drawing.Point(18, 57);
            this.textBoxLat1.Name = "textBoxLat1";
            this.textBoxLat1.Size = new System.Drawing.Size(100, 20);
            this.textBoxLat1.TabIndex = 0;
            this.textBoxLat1.Text = "55,291628";
            this.textBoxLat1.Click += new System.EventHandler(this.textBoxLat1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Широта";
            // 
            // textBoxLon1
            // 
            this.textBoxLon1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxLon1.Location = new System.Drawing.Point(148, 57);
            this.textBoxLon1.Name = "textBoxLon1";
            this.textBoxLon1.Size = new System.Drawing.Size(100, 20);
            this.textBoxLon1.TabIndex = 2;
            this.textBoxLon1.Text = "37,092741";
            this.textBoxLon1.Click += new System.EventHandler(this.textBoxLon1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Долгота";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxLat1);
            this.groupBox1.Controls.Add(this.textBoxLon1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 118);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Первая точка";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxLat2);
            this.groupBox2.Controls.Add(this.textBoxLon2);
            this.groupBox2.Location = new System.Drawing.Point(12, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 118);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Вторая точка";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Широта";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(145, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Долгота";
            // 
            // textBoxLat2
            // 
            this.textBoxLat2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxLat2.Location = new System.Drawing.Point(18, 57);
            this.textBoxLat2.Name = "textBoxLat2";
            this.textBoxLat2.Size = new System.Drawing.Size(100, 20);
            this.textBoxLat2.TabIndex = 0;
            this.textBoxLat2.Text = "58,5";
            this.textBoxLat2.Click += new System.EventHandler(this.textBoxLat2_Click);
            // 
            // textBoxLon2
            // 
            this.textBoxLon2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.textBoxLon2.Location = new System.Drawing.Point(148, 57);
            this.textBoxLon2.Name = "textBoxLon2";
            this.textBoxLon2.Size = new System.Drawing.Size(100, 20);
            this.textBoxLon2.TabIndex = 2;
            this.textBoxLon2.Text = "37,872551";
            this.textBoxLon2.Click += new System.EventHandler(this.textBoxLon2_Click);
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Location = new System.Drawing.Point(12, 262);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonCalculate.Size = new System.Drawing.Size(276, 23);
            this.buttonCalculate.TabIndex = 6;
            this.buttonCalculate.Text = "Рассчитать ";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // labelDistance
            // 
            this.labelDistance.AutoSize = true;
            this.labelDistance.Location = new System.Drawing.Point(9, 292);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(70, 13);
            this.labelDistance.TabIndex = 7;
            this.labelDistance.Text = "Расстояние:";
            // 
            // labelTrueAzimuth
            // 
            this.labelTrueAzimuth.AutoSize = true;
            this.labelTrueAzimuth.Location = new System.Drawing.Point(9, 305);
            this.labelTrueAzimuth.Name = "labelTrueAzimuth";
            this.labelTrueAzimuth.Size = new System.Drawing.Size(100, 13);
            this.labelTrueAzimuth.TabIndex = 8;
            this.labelTrueAzimuth.Text = "Истинный азимут:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(294, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 242);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Настройки";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonModGaver);
            this.groupBox4.Controls.Add(this.radioButtonGaversin);
            this.groupBox4.Controls.Add(this.radioButtonSphere);
            this.groupBox4.Controls.Add(this.radioButtonPifagor);
            this.groupBox4.Location = new System.Drawing.Point(6, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(262, 124);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Способ вычисления расстояний";
            // 
            // radioButtonModGaver
            // 
            this.radioButtonModGaver.AutoSize = true;
            this.radioButtonModGaver.Checked = true;
            this.radioButtonModGaver.Location = new System.Drawing.Point(6, 88);
            this.radioButtonModGaver.Name = "radioButtonModGaver";
            this.radioButtonModGaver.Size = new System.Drawing.Size(244, 17);
            this.radioButtonModGaver.TabIndex = 6;
            this.radioButtonModGaver.TabStop = true;
            this.radioButtonModGaver.Text = "модифицированная теорема гаверсинусов";
            this.radioButtonModGaver.UseVisualStyleBackColor = true;
            // 
            // radioButtonGaversin
            // 
            this.radioButtonGaversin.AutoSize = true;
            this.radioButtonGaversin.Location = new System.Drawing.Point(6, 65);
            this.radioButtonGaversin.Name = "radioButtonGaversin";
            this.radioButtonGaversin.Size = new System.Drawing.Size(141, 17);
            this.radioButtonGaversin.TabIndex = 5;
            this.radioButtonGaversin.Text = "теорема гаверсинусов";
            this.radioButtonGaversin.UseVisualStyleBackColor = true;
            // 
            // radioButtonSphere
            // 
            this.radioButtonSphere.AutoSize = true;
            this.radioButtonSphere.Location = new System.Drawing.Point(6, 42);
            this.radioButtonSphere.Name = "radioButtonSphere";
            this.radioButtonSphere.Size = new System.Drawing.Size(182, 17);
            this.radioButtonSphere.TabIndex = 4;
            this.radioButtonSphere.Text = "сферическая теорема синусов";
            this.radioButtonSphere.UseVisualStyleBackColor = true;
            // 
            // radioButtonPifagor
            // 
            this.radioButtonPifagor.AutoSize = true;
            this.radioButtonPifagor.Location = new System.Drawing.Point(6, 19);
            this.radioButtonPifagor.Name = "radioButtonPifagor";
            this.radioButtonPifagor.Size = new System.Drawing.Size(122, 17);
            this.radioButtonPifagor.TabIndex = 3;
            this.radioButtonPifagor.Text = "теорема Пифагора";
            this.radioButtonPifagor.UseVisualStyleBackColor = true;
            // 
            // labelMagneticAzimuth
            // 
            this.labelMagneticAzimuth.AutoSize = true;
            this.labelMagneticAzimuth.Location = new System.Drawing.Point(9, 318);
            this.labelMagneticAzimuth.Name = "labelMagneticAzimuth";
            this.labelMagneticAzimuth.Size = new System.Drawing.Size(106, 13);
            this.labelMagneticAzimuth.TabIndex = 10;
            this.labelMagneticAzimuth.Text = "Магнитный азимут:";
            // 
            // labelMagneticDiv
            // 
            this.labelMagneticDiv.AutoSize = true;
            this.labelMagneticDiv.Location = new System.Drawing.Point(9, 331);
            this.labelMagneticDiv.Name = "labelMagneticDiv";
            this.labelMagneticDiv.Size = new System.Drawing.Size(125, 13);
            this.labelMagneticDiv.TabIndex = 11;
            this.labelMagneticDiv.Text = "Магнитное склонение: ";
            // 
            // FormCalculateDistance
            // 
            this.AcceptButton = this.buttonCalculate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 392);
            this.Controls.Add(this.labelMagneticDiv);
            this.Controls.Add(this.labelMagneticAzimuth);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.labelTrueAzimuth);
            this.Controls.Add(this.labelDistance);
            this.Controls.Add(this.buttonCalculate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCalculateDistance";
            this.Text = "Измерение расстояний";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLat1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxLon1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxLat2;
        private System.Windows.Forms.TextBox textBoxLon2;
        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.Label labelDistance;
        private System.Windows.Forms.Label labelTrueAzimuth;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButtonGaversin;
        private System.Windows.Forms.RadioButton radioButtonSphere;
        private System.Windows.Forms.RadioButton radioButtonPifagor;
        private System.Windows.Forms.Label labelMagneticAzimuth;
        private System.Windows.Forms.Label labelMagneticDiv;
        private System.Windows.Forms.RadioButton radioButtonModGaver;
    }
}