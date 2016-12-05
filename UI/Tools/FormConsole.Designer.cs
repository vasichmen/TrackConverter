namespace TrackConverter.UI.Tools
{
    partial class FormConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConsole));
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxResult
            // 
            this.textBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxResult.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxResult.Location = new System.Drawing.Point(0, 0);
            this.textBoxResult.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.Size = new System.Drawing.Size(575, 293);
            this.textBoxResult.TabIndex = 1;
            this.textBoxResult.Text = "com>";
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxCommand.Location = new System.Drawing.Point(0, 296);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(566, 20);
            this.textBoxCommand.TabIndex = 2;
            this.textBoxCommand.Text = "com>";
            this.textBoxCommand.TextChanged += new System.EventHandler(this.textBoxCommand_TextChanged);
            this.textBoxCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCommand_KeyDown);
            // 
            // FormConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 316);
            this.Controls.Add(this.textBoxCommand);
            this.Controls.Add(this.textBoxResult);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormConsole";
            this.Text = "Консоль";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBoxCommand;
    }
}