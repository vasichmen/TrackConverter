namespace TrackConverter.UI.Tools
{
    partial class FormElevVisual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormElevVisual));
            this.zedGraph = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraph
            // 
            this.zedGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraph.EditModifierKeys = System.Windows.Forms.Keys.None;
            this.zedGraph.IsShowPointValues = true;
            this.zedGraph.Location = new System.Drawing.Point(0, 0);
            this.zedGraph.Name = "zedGraph";
            this.zedGraph.ScrollGrace = 0D;
            this.zedGraph.ScrollMaxX = 0D;
            this.zedGraph.ScrollMaxY = 0D;
            this.zedGraph.ScrollMaxY2 = 0D;
            this.zedGraph.ScrollMinX = 0D;
            this.zedGraph.ScrollMinY = 0D;
            this.zedGraph.ScrollMinY2 = 0D;
            this.zedGraph.Size = new System.Drawing.Size(758, 486);
            this.zedGraph.TabIndex = 1;
            this.zedGraph.UseExtendedPrintDialog = true;
            this.zedGraph.PointValueEvent += new ZedGraph.ZedGraphControl.PointValueHandler(this.zedGraph_PointValueEvent);
            this.zedGraph.MouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(this.zedGraph_MouseMoveEvent);
            this.zedGraph.MouseLeave += new System.EventHandler(this.zedGraph_MouseLeave);
            // 
            // FormElevVisual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 486);
            this.Controls.Add(this.zedGraph);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormElevVisual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Профиль высот";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraph;
    }
}