namespace TrackConverter.UI.Converter
{
    partial class FormTrackInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrackInformation));
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            this.textBoxDistance = new System.Windows.Forms.TextBox();
            this.textBoxSpeed = new System.Windows.Forms.TextBox();
            this.textBoxStart = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabelStartYandex = new System.Windows.Forms.LinkLabel();
            this.linkLabelFinishYandex = new System.Windows.Forms.LinkLabel();
            this.textBoxFinish = new System.Windows.Forms.TextBox();
            this.linkLabelFinishGoogle = new System.Windows.Forms.LinkLabel();
            this.linkLabelStartGoogle = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxCount = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonOpenWikimapia = new System.Windows.Forms.Button();
            this.buttonOpenYandex = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBoxColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(150, 12);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(246, 20);
            this.textBoxName.TabIndex = 0;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(150, 179);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(246, 57);
            this.textBoxDescription.TabIndex = 1;
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(150, 64);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.ReadOnly = true;
            this.textBoxTime.Size = new System.Drawing.Size(100, 20);
            this.textBoxTime.TabIndex = 2;
            // 
            // textBoxDistance
            // 
            this.textBoxDistance.Location = new System.Drawing.Point(150, 38);
            this.textBoxDistance.Name = "textBoxDistance";
            this.textBoxDistance.ReadOnly = true;
            this.textBoxDistance.Size = new System.Drawing.Size(100, 20);
            this.textBoxDistance.TabIndex = 3;
            // 
            // textBoxSpeed
            // 
            this.textBoxSpeed.Location = new System.Drawing.Point(150, 116);
            this.textBoxSpeed.Name = "textBoxSpeed";
            this.textBoxSpeed.ReadOnly = true;
            this.textBoxSpeed.Size = new System.Drawing.Size(100, 20);
            this.textBoxSpeed.TabIndex = 4;
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(150, 242);
            this.textBoxStart.Multiline = true;
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.ReadOnly = true;
            this.textBoxStart.Size = new System.Drawing.Size(100, 45);
            this.textBoxStart.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Название";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Конечная точка";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Начальная точка";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Средняя скорость, км/ч";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Общее время";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Длина, км";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 182);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Описание";
            // 
            // linkLabelStartYandex
            // 
            this.linkLabelStartYandex.AutoSize = true;
            this.linkLabelStartYandex.Location = new System.Drawing.Point(256, 245);
            this.linkLabelStartYandex.Name = "linkLabelStartYandex";
            this.linkLabelStartYandex.Size = new System.Drawing.Size(144, 13);
            this.linkLabelStartYandex.TabIndex = 14;
            this.linkLabelStartYandex.TabStop = true;
            this.linkLabelStartYandex.Text = "Открыть на картах Яндекс";
            this.linkLabelStartYandex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelStartYandex_LinkClicked);
            // 
            // linkLabelFinishYandex
            // 
            this.linkLabelFinishYandex.AutoSize = true;
            this.linkLabelFinishYandex.Location = new System.Drawing.Point(256, 296);
            this.linkLabelFinishYandex.Name = "linkLabelFinishYandex";
            this.linkLabelFinishYandex.Size = new System.Drawing.Size(144, 13);
            this.linkLabelFinishYandex.TabIndex = 15;
            this.linkLabelFinishYandex.TabStop = true;
            this.linkLabelFinishYandex.Text = "Открыть на картах Яндекс";
            this.linkLabelFinishYandex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFinishYandex_LinkClicked);
            // 
            // textBoxFinish
            // 
            this.textBoxFinish.Location = new System.Drawing.Point(150, 293);
            this.textBoxFinish.Multiline = true;
            this.textBoxFinish.Name = "textBoxFinish";
            this.textBoxFinish.ReadOnly = true;
            this.textBoxFinish.Size = new System.Drawing.Size(100, 45);
            this.textBoxFinish.TabIndex = 16;
            // 
            // linkLabelFinishGoogle
            // 
            this.linkLabelFinishGoogle.AutoSize = true;
            this.linkLabelFinishGoogle.Location = new System.Drawing.Point(256, 320);
            this.linkLabelFinishGoogle.Name = "linkLabelFinishGoogle";
            this.linkLabelFinishGoogle.Size = new System.Drawing.Size(140, 13);
            this.linkLabelFinishGoogle.TabIndex = 17;
            this.linkLabelFinishGoogle.TabStop = true;
            this.linkLabelFinishGoogle.Text = "Открыть на картах Google";
            this.linkLabelFinishGoogle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFinishGoogle_LinkClicked);
            // 
            // linkLabelStartGoogle
            // 
            this.linkLabelStartGoogle.AutoSize = true;
            this.linkLabelStartGoogle.Location = new System.Drawing.Point(256, 268);
            this.linkLabelStartGoogle.Name = "linkLabelStartGoogle";
            this.linkLabelStartGoogle.Size = new System.Drawing.Size(140, 13);
            this.linkLabelStartGoogle.TabIndex = 18;
            this.linkLabelStartGoogle.TabStop = true;
            this.linkLabelStartGoogle.Text = "Открыть на картах Google";
            this.linkLabelStartGoogle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelStartGoogle_LinkClicked);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 93);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Всего точек";
            // 
            // textBoxCount
            // 
            this.textBoxCount.Location = new System.Drawing.Point(150, 90);
            this.textBoxCount.Name = "textBoxCount";
            this.textBoxCount.ReadOnly = true;
            this.textBoxCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxCount.TabIndex = 19;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 373);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75
                , 23);
            this.buttonSave.TabIndex = 21;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(321, 373);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 22;
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonOpenWikimapia
            // 
            this.buttonOpenWikimapia.Location = new System.Drawing.Point(225, 344);
            this.buttonOpenWikimapia.Name = "buttonOpenWikimapia";
            this.buttonOpenWikimapia.Size = new System.Drawing.Size(171, 23);
            this.buttonOpenWikimapia.TabIndex = 23;
            this.buttonOpenWikimapia.Text = "Открыть маршрут в Wikimapia";
            this.buttonOpenWikimapia.UseVisualStyleBackColor = true;
            this.buttonOpenWikimapia.Click += new System.EventHandler(this.buttonOpenWikimapia_Click);
            // 
            // buttonOpenYandex
            // 
            this.buttonOpenYandex.Location = new System.Drawing.Point(12, 344);
            this.buttonOpenYandex.Name = "buttonOpenYandex";
            this.buttonOpenYandex.Size = new System.Drawing.Size(171, 23);
            this.buttonOpenYandex.TabIndex = 24;
            this.buttonOpenYandex.Text = "Открыть маршрут в Яндекс";
            this.buttonOpenYandex.UseVisualStyleBackColor = true;
            this.buttonOpenYandex.Click += new System.EventHandler(this.buttonOpenYandex_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 154);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Цвет";
            // 
            // pictureBoxColor
            // 
            this.pictureBoxColor.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pictureBoxColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxColor.Location = new System.Drawing.Point(150, 147);
            this.pictureBoxColor.Name = "pictureBoxColor";
            this.pictureBoxColor.Size = new System.Drawing.Size(100, 20);
            this.pictureBoxColor.TabIndex = 27;
            this.pictureBoxColor.TabStop = false;
            this.pictureBoxColor.Click += new System.EventHandler(this.pictureBoxColor_Click);
            // 
            // FormTrackInformation
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(411, 405);
            this.Controls.Add(this.pictureBoxColor);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.buttonOpenYandex);
            this.Controls.Add(this.buttonOpenWikimapia);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxCount);
            this.Controls.Add(this.linkLabelStartGoogle);
            this.Controls.Add(this.linkLabelFinishGoogle);
            this.Controls.Add(this.textBoxFinish);
            this.Controls.Add(this.linkLabelFinishYandex);
            this.Controls.Add(this.linkLabelStartYandex);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxStart);
            this.Controls.Add(this.textBoxSpeed);
            this.Controls.Add(this.textBoxDistance);
            this.Controls.Add(this.textBoxTime);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormTrackInformation";
            this.Text = "Подробная мнформация";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.TextBox textBoxDistance;
        private System.Windows.Forms.TextBox textBoxSpeed;
        private System.Windows.Forms.TextBox textBoxStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabelStartYandex;
        private System.Windows.Forms.LinkLabel linkLabelFinishYandex;
        private System.Windows.Forms.TextBox textBoxFinish;
        private System.Windows.Forms.LinkLabel linkLabelFinishGoogle;
        private System.Windows.Forms.LinkLabel linkLabelStartGoogle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxCount;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOpenWikimapia;
        private System.Windows.Forms.Button buttonOpenYandex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictureBoxColor;
    }
}