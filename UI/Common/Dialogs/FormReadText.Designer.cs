namespace TrackConverter.UI.Common.Dialogs
{
    partial class FormReadText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReadText));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSetAll = new System.Windows.Forms.Button();
            this.linkLabelOpenBrowser = new System.Windows.Forms.LinkLabel();
            this.buttonShortLink = new System.Windows.Forms.Button();
            this.buttonCopyLink = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 23);
            this.textBox1.Margin = new System.Windows.Forms.Padding(20, 20, 20, 50);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(370, 70);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Введите ссылку на маршрут";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(12, 129);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(88, 129);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSetAll
            // 
            this.buttonSetAll.Location = new System.Drawing.Point(169, 129);
            this.buttonSetAll.Name = "buttonSetAll";
            this.buttonSetAll.Size = new System.Drawing.Size(151, 23);
            this.buttonSetAll.TabIndex = 4;
            this.buttonSetAll.Text = "Все точки";
            this.buttonSetAll.UseVisualStyleBackColor = true;
            this.buttonSetAll.Visible = false;
            this.buttonSetAll.Click += new System.EventHandler(this.btSetAll_Click);
            // 
            // linkLabelOpenBrowser
            // 
            this.linkLabelOpenBrowser.AutoSize = true;
            this.linkLabelOpenBrowser.Location = new System.Drawing.Point(12, 97);
            this.linkLabelOpenBrowser.Name = "linkLabelOpenBrowser";
            this.linkLabelOpenBrowser.Size = new System.Drawing.Size(110, 13);
            this.linkLabelOpenBrowser.TabIndex = 5;
            this.linkLabelOpenBrowser.TabStop = true;
            this.linkLabelOpenBrowser.Text = "Открыть в браузере";
            this.linkLabelOpenBrowser.Visible = false;
            // 
            // buttonShortLink
            // 
            this.buttonShortLink.Location = new System.Drawing.Point(12, 158);
            this.buttonShortLink.Name = "buttonShortLink";
            this.buttonShortLink.Size = new System.Drawing.Size(151, 23);
            this.buttonShortLink.TabIndex = 6;
            this.buttonShortLink.Text = "Сократить ссылку";
            this.buttonShortLink.UseVisualStyleBackColor = true;
            this.buttonShortLink.Visible = false;
            this.buttonShortLink.Click += new System.EventHandler(this.buttonShortLink_Click);
            // 
            // buttonCopyLink
            // 
            this.buttonCopyLink.Location = new System.Drawing.Point(169, 158);
            this.buttonCopyLink.Name = "buttonCopyLink";
            this.buttonCopyLink.Size = new System.Drawing.Size(151, 23);
            this.buttonCopyLink.TabIndex = 7;
            this.buttonCopyLink.Text = "Копировать ссылку";
            this.buttonCopyLink.UseVisualStyleBackColor = true;
            this.buttonCopyLink.Visible = false;
            this.buttonCopyLink.Click += new System.EventHandler(this.buttonCopyLink_Click);
            // 
            // FormReadText
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 191);
            this.Controls.Add(this.buttonCopyLink);
            this.Controls.Add(this.buttonShortLink);
            this.Controls.Add(this.linkLabelOpenBrowser);
            this.Controls.Add(this.buttonSetAll);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormReadText";
            this.Text = "Ввод текста";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSetAll;
        private System.Windows.Forms.LinkLabel linkLabelOpenBrowser;
        private System.Windows.Forms.Button buttonShortLink;
        private System.Windows.Forms.Button buttonCopyLink;
    }
}