namespace TrackConverter.UI.Map
{
    partial class FormWhatsthere
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWhatsthere));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelAlt = new System.Windows.Forms.Label();
            this.labelTimeOffset = new System.Windows.Forms.Label();
            this.labelFall = new System.Windows.Forms.Label();
            this.labelRise = new System.Windows.Forms.Label();
            this.labeldayLength = new System.Windows.Forms.Label();
            this.labelRiseAzi = new System.Windows.Forms.Label();
            this.labelFallAzi = new System.Windows.Forms.Label();
            this.labelLocalTime = new System.Windows.Forms.Label();
            this.labelDec = new System.Windows.Forms.Label();
            this.textBoxAddress = new System.Windows.Forms.TextBox();
            this.textBoxLat = new System.Windows.Forms.TextBox();
            this.textBoxLon = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonCenter = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Адрес";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Широта";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Долгота";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 275);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Магнитное склонение";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Восход";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Закат";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 211F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonCenter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonClose, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 302F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(355, 347);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.labelAlt, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.labelTimeOffset, 0, 9);
            this.tableLayoutPanel3.Controls.Add(this.labelFall, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.labelRise, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.labeldayLength, 0, 6);
            this.tableLayoutPanel3.Controls.Add(this.labelRiseAzi, 0, 7);
            this.tableLayoutPanel3.Controls.Add(this.labelFallAzi, 0, 8);
            this.tableLayoutPanel3.Controls.Add(this.labelLocalTime, 0, 10);
            this.tableLayoutPanel3.Controls.Add(this.labelDec, 0, 11);
            this.tableLayoutPanel3.Controls.Add(this.textBoxAddress, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.textBoxLat, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBoxLon, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(147, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 12;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(205, 296);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // labelAlt
            // 
            this.labelAlt.AutoEllipsis = true;
            this.labelAlt.AutoSize = true;
            this.labelAlt.Location = new System.Drawing.Point(3, 75);
            this.labelAlt.Name = "labelAlt";
            this.labelAlt.Size = new System.Drawing.Size(45, 13);
            this.labelAlt.TabIndex = 6;
            this.labelAlt.Text = "Высота";
            // 
            // labelTimeOffset
            // 
            this.labelTimeOffset.AutoEllipsis = true;
            this.labelTimeOffset.AutoSize = true;
            this.labelTimeOffset.Location = new System.Drawing.Point(3, 225);
            this.labelTimeOffset.Name = "labelTimeOffset";
            this.labelTimeOffset.Size = new System.Drawing.Size(62, 13);
            this.labelTimeOffset.TabIndex = 7;
            this.labelTimeOffset.Text = "Склонение";
            // 
            // labelFall
            // 
            this.labelFall.AutoEllipsis = true;
            this.labelFall.AutoSize = true;
            this.labelFall.Location = new System.Drawing.Point(3, 125);
            this.labelFall.Name = "labelFall";
            this.labelFall.Size = new System.Drawing.Size(37, 13);
            this.labelFall.TabIndex = 5;
            this.labelFall.Text = "Закат";
            // 
            // labelRise
            // 
            this.labelRise.AutoEllipsis = true;
            this.labelRise.AutoSize = true;
            this.labelRise.Location = new System.Drawing.Point(3, 100);
            this.labelRise.Name = "labelRise";
            this.labelRise.Size = new System.Drawing.Size(43, 13);
            this.labelRise.TabIndex = 4;
            this.labelRise.Text = "Восход";
            // 
            // labeldayLength
            // 
            this.labeldayLength.AutoSize = true;
            this.labeldayLength.Location = new System.Drawing.Point(3, 150);
            this.labeldayLength.Name = "labeldayLength";
            this.labeldayLength.Size = new System.Drawing.Size(119, 13);
            this.labeldayLength.TabIndex = 11;
            this.labeldayLength.Text = "Магнитное склонение";
            // 
            // labelRiseAzi
            // 
            this.labelRiseAzi.AutoEllipsis = true;
            this.labelRiseAzi.AutoSize = true;
            this.labelRiseAzi.Location = new System.Drawing.Point(3, 175);
            this.labelRiseAzi.Name = "labelRiseAzi";
            this.labelRiseAzi.Size = new System.Drawing.Size(62, 13);
            this.labelRiseAzi.TabIndex = 10;
            this.labelRiseAzi.Text = "Склонение";
            // 
            // labelFallAzi
            // 
            this.labelFallAzi.AutoEllipsis = true;
            this.labelFallAzi.AutoSize = true;
            this.labelFallAzi.Location = new System.Drawing.Point(3, 200);
            this.labelFallAzi.Name = "labelFallAzi";
            this.labelFallAzi.Size = new System.Drawing.Size(62, 13);
            this.labelFallAzi.TabIndex = 9;
            this.labelFallAzi.Text = "Склонение";
            // 
            // labelLocalTime
            // 
            this.labelLocalTime.AutoEllipsis = true;
            this.labelLocalTime.AutoSize = true;
            this.labelLocalTime.Location = new System.Drawing.Point(3, 250);
            this.labelLocalTime.Name = "labelLocalTime";
            this.labelLocalTime.Size = new System.Drawing.Size(62, 13);
            this.labelLocalTime.TabIndex = 8;
            this.labelLocalTime.Text = "Склонение";
            // 
            // labelDec
            // 
            this.labelDec.AutoEllipsis = true;
            this.labelDec.AutoSize = true;
            this.labelDec.Location = new System.Drawing.Point(3, 275);
            this.labelDec.Name = "labelDec";
            this.labelDec.Size = new System.Drawing.Size(62, 13);
            this.labelDec.TabIndex = 3;
            this.labelDec.Text = "Склонение";
            // 
            // textBoxAddress
            // 
            this.textBoxAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAddress.Location = new System.Drawing.Point(3, 3);
            this.textBoxAddress.Name = "textBoxAddress";
            this.textBoxAddress.ReadOnly = true;
            this.textBoxAddress.Size = new System.Drawing.Size(199, 20);
            this.textBoxAddress.TabIndex = 12;
            // 
            // textBoxLat
            // 
            this.textBoxLat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLat.Location = new System.Drawing.Point(3, 28);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.ReadOnly = true;
            this.textBoxLat.Size = new System.Drawing.Size(199, 20);
            this.textBoxLat.TabIndex = 13;
            // 
            // textBoxLon
            // 
            this.textBoxLon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLon.Location = new System.Drawing.Point(3, 53);
            this.textBoxLon.Name = "textBoxLon";
            this.textBoxLon.ReadOnly = true;
            this.textBoxLon.Size = new System.Drawing.Size(199, 20);
            this.textBoxLon.TabIndex = 14;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 9);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.label12, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.label13, 0, 8);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 10);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 11);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 12;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(138, 296);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Высота";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 225);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Часовой пояс";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Продолжительность дня";
            // 
            // label12
            // 
            this.label12.AutoEllipsis = true;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 175);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Азимут восхода";
            // 
            // label13
            // 
            this.label13.AutoEllipsis = true;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 200);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(82, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Азимут заката";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 250);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(98, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Локальное время";
            // 
            // buttonCenter
            // 
            this.buttonCenter.Location = new System.Drawing.Point(18, 312);
            this.buttonCenter.Margin = new System.Windows.Forms.Padding(18, 10, 10, 10);
            this.buttonCenter.Name = "buttonCenter";
            this.buttonCenter.Size = new System.Drawing.Size(104, 20);
            this.buttonCenter.TabIndex = 2;
            this.buttonCenter.Text = "На центр карты";
            this.buttonCenter.UseVisualStyleBackColor = true;
            this.buttonCenter.Click += new System.EventHandler(this.buttonCenter_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(262, 312);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(10, 10, 18, 10);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 20);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Закрыть";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // FormWhatsthere
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 347);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(354, 381);
            this.Name = "FormWhatsthere";
            this.Text = "FormWhatsthere";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormWhatsthere_FormClosed);
            this.Load += new System.EventHandler(this.FormWhatsthere_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label labelFall;
        private System.Windows.Forms.Label labelRise;
        private System.Windows.Forms.Label labelDec;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label labelAlt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelLocalTime;
        private System.Windows.Forms.Label labelTimeOffset;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelRiseAzi;
        private System.Windows.Forms.Label labelFallAzi;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonCenter;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labeldayLength;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxAddress;
        private System.Windows.Forms.TextBox textBoxLat;
        private System.Windows.Forms.TextBox textBoxLon;
    }
}