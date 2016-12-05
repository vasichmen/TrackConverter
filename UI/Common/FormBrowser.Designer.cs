namespace TrackConverter.UI.Common
{
    partial class FormBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBrowser));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.buttonGoBack = new System.Windows.Forms.Button();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(0, 26);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(740, 470);
            this.webBrowser1.TabIndex = 0;
            // 
            // buttonGoBack
            // 
            this.buttonGoBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonGoBack.Location = new System.Drawing.Point(0, 0);
            this.buttonGoBack.Margin = new System.Windows.Forms.Padding(0);
            this.buttonGoBack.Name = "buttonGoBack";
            this.buttonGoBack.Size = new System.Drawing.Size(75, 23);
            this.buttonGoBack.TabIndex = 1;
            this.buttonGoBack.Text = "Назад";
            this.buttonGoBack.UseVisualStyleBackColor = true;
            this.buttonGoBack.Click += new System.EventHandler(this.buttonGoBack_Click);
            // 
            // buttonForward
            // 
            this.buttonForward.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonForward.Location = new System.Drawing.Point(75, 0);
            this.buttonForward.Margin = new System.Windows.Forms.Padding(0);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(75, 23);
            this.buttonForward.TabIndex = 2;
            this.buttonForward.Text = "Вперед";
            this.buttonForward.UseVisualStyleBackColor = true;
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRefresh.Location = new System.Drawing.Point(150, 0);
            this.buttonRefresh.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Обновить";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 496);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonForward);
            this.Controls.Add(this.buttonGoBack);
            this.Controls.Add(this.webBrowser1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormBrowser";
            this.Text = "Справка";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button buttonGoBack;
        private System.Windows.Forms.Button buttonForward;
        private System.Windows.Forms.Button buttonRefresh;
    }
}