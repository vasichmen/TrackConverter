namespace TrackConverter.UI.Common.Dialogs
{
    partial class FormSeparateTrackTypeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSeparateTrackTypeDialog));
            this.buttonNearest = new System.Windows.Forms.Button();
            this.buttonLength = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxLabel = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxAddCommonPoint = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonNearest
            // 
            resources.ApplyResources(this.buttonNearest, "buttonNearest");
            this.buttonNearest.Name = "buttonNearest";
            this.toolTip1.SetToolTip(this.buttonNearest, resources.GetString("buttonNearest.ToolTip"));
            this.buttonNearest.UseVisualStyleBackColor = true;
            this.buttonNearest.Click += new System.EventHandler(this.buttonNearest_Click);
            // 
            // buttonLength
            // 
            resources.ApplyResources(this.buttonLength, "buttonLength");
            this.buttonLength.Name = "buttonLength";
            this.toolTip1.SetToolTip(this.buttonLength, resources.GetString("buttonLength.ToolTip"));
            this.buttonLength.UseVisualStyleBackColor = true;
            this.buttonLength.Click += new System.EventHandler(this.buttonLength_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxLabel
            // 
            this.textBoxLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxLabel.Cursor = System.Windows.Forms.Cursors.Arrow;
            resources.ApplyResources(this.textBoxLabel, "textBoxLabel");
            this.textBoxLabel.Name = "textBoxLabel";
            this.textBoxLabel.ReadOnly = true;
            // 
            // checkBoxAddCommonPoint
            // 
            resources.ApplyResources(this.checkBoxAddCommonPoint, "checkBoxAddCommonPoint");
            this.checkBoxAddCommonPoint.Checked = true;
            this.checkBoxAddCommonPoint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAddCommonPoint.Name = "checkBoxAddCommonPoint";
            this.toolTip1.SetToolTip(this.checkBoxAddCommonPoint, resources.GetString("checkBoxAddCommonPoint.ToolTip"));
            this.checkBoxAddCommonPoint.UseVisualStyleBackColor = true;
            // 
            // FormSeparateTrackTypeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxAddCommonPoint);
            this.Controls.Add(this.textBoxLabel);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonLength);
            this.Controls.Add(this.buttonNearest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormSeparateTrackTypeDialog";
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSeparateTrackTypeDialog_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonNearest;
        private System.Windows.Forms.Button buttonLength;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxAddCommonPoint;
    }
}