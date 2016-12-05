namespace TrackConverter.UI.Map
{
    partial class FormMapNavigator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapNavigator));
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.labelZoom = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.buttonGoBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.buttonZoomIn.Location = new System.Drawing.Point(33, 1);
            this.buttonZoomIn.Margin = new System.Windows.Forms.Padding(1);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(30, 30);
            this.buttonZoomIn.TabIndex = 0;
            this.buttonZoomIn.Text = "+";
            this.buttonZoomIn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonZoomIn.UseVisualStyleBackColor = true;
            this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.buttonZoomOut.Location = new System.Drawing.Point(1, 1);
            this.buttonZoomOut.Margin = new System.Windows.Forms.Padding(1);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(30, 30);
            this.buttonZoomOut.TabIndex = 1;
            this.buttonZoomOut.Text = "-";
            this.buttonZoomOut.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonZoomOut.UseVisualStyleBackColor = true;
            this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(67, 11);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(34, 13);
            this.labelZoom.TabIndex = 4;
            this.labelZoom.Text = "Zoom";
            // 
            // buttonUp
            // 
            this.buttonUp.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonUp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.buttonUp.Location = new System.Drawing.Point(149, 1);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(1);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(20, 15);
            this.buttonUp.TabIndex = 5;
            this.buttonUp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonUp.UseVisualStyleBackColor = false;
            this.buttonUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseDown);
            this.buttonUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseUp);
            // 
            // buttonRight
            // 
            this.buttonRight.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonRight.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRight.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Document, ((byte)(204)));
            this.buttonRight.Location = new System.Drawing.Point(169, 1);
            this.buttonRight.Margin = new System.Windows.Forms.Padding(1);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(15, 30);
            this.buttonRight.TabIndex = 6;
            this.buttonRight.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonRight.UseVisualStyleBackColor = false;
            this.buttonRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseDown);
            this.buttonRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseUp);
            // 
            // buttonLeft
            // 
            this.buttonLeft.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonLeft.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLeft.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.buttonLeft.Location = new System.Drawing.Point(134, 1);
            this.buttonLeft.Margin = new System.Windows.Forms.Padding(1);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(15, 30);
            this.buttonLeft.TabIndex = 7;
            this.buttonLeft.Text = "+";
            this.buttonLeft.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonLeft.UseVisualStyleBackColor = false;
            this.buttonLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseDown);
            this.buttonLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseUp);
            // 
            // buttonDown
            // 
            this.buttonDown.BackColor = System.Drawing.SystemColors.Highlight;
            this.buttonDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDown.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(204)));
            this.buttonDown.Location = new System.Drawing.Point(149, 16);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(1);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(20, 15);
            this.buttonDown.TabIndex = 8;
            this.buttonDown.Text = "+";
            this.buttonDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonDown.UseVisualStyleBackColor = false;
            this.buttonDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseDown);
            this.buttonDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonMoveMap_MouseUp);
            // 
            // buttonGoBack
            // 
            this.buttonGoBack.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonGoBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonGoBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonGoBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 34F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Document, ((byte)(204)));
            this.buttonGoBack.Location = new System.Drawing.Point(186, 1);
            this.buttonGoBack.Margin = new System.Windows.Forms.Padding(1);
            this.buttonGoBack.Name = "buttonGoBack";
            this.buttonGoBack.Size = new System.Drawing.Size(53, 30);
            this.buttonGoBack.TabIndex = 9;
            this.buttonGoBack.Text = "Назад";
            this.buttonGoBack.UseVisualStyleBackColor = false;
            this.buttonGoBack.Click += new System.EventHandler(this.buttonGoBack_Click);
            // 
            // FormMapNavigator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(252, 33);
            this.Controls.Add(this.buttonGoBack);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.buttonRight);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.labelZoom);
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.buttonZoomIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMapNavigator";
            this.Opacity = 0.75D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Навигация";
            this.Activated += new System.EventHandler(this.FormMapNavigator_Activated);
            this.Deactivate += new System.EventHandler(this.FormMapNavigator_Deactivate);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        /// <summary>
        /// надпись масштаба
        /// </summary>
        public System.Windows.Forms.Label labelZoom;

        private System.Windows.Forms.Button buttonZoomIn;
        private System.Windows.Forms.Button buttonZoomOut;

        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonDown;

        private System.Windows.Forms.Button buttonGoBack;
    }
}