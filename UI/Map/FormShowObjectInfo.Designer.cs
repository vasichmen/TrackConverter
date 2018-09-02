namespace TrackConverter.UI.Map
{
    partial class FormShowObjectInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormShowObjectInfo));
            this.flowLayoutPanelWrapper = new System.Windows.Forms.FlowLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.linkLabelLink = new System.Windows.Forms.LinkLabel();
            this.contextMenuStripLinkLabel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelImages = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxComments = new System.Windows.Forms.TextBox();
            this.copyLinkMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanelWrapper.SuspendLayout();
            this.contextMenuStripLinkLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelWrapper
            // 
            this.flowLayoutPanelWrapper.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.flowLayoutPanelWrapper.Controls.Add(this.labelName);
            this.flowLayoutPanelWrapper.Controls.Add(this.linkLabelLink);
            this.flowLayoutPanelWrapper.Controls.Add(this.flowLayoutPanelImages);
            this.flowLayoutPanelWrapper.Controls.Add(this.label2);
            this.flowLayoutPanelWrapper.Controls.Add(this.textBoxDescription);
            this.flowLayoutPanelWrapper.Controls.Add(this.label1);
            this.flowLayoutPanelWrapper.Controls.Add(this.textBoxComments);
            this.flowLayoutPanelWrapper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelWrapper.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelWrapper.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelWrapper.Name = "flowLayoutPanelWrapper";
            this.flowLayoutPanelWrapper.Size = new System.Drawing.Size(581, 653);
            this.flowLayoutPanelWrapper.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelName.Location = new System.Drawing.Point(3, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(126, 29);
            this.labelName.TabIndex = 6;
            this.labelName.Text = "Название";
            // 
            // linkLabelLink
            // 
            this.linkLabelLink.AutoSize = true;
            this.linkLabelLink.ContextMenuStrip = this.contextMenuStripLinkLabel;
            this.linkLabelLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkLabelLink.Location = new System.Drawing.Point(8, 29);
            this.linkLabelLink.Margin = new System.Windows.Forms.Padding(8, 0, 3, 0);
            this.linkLabelLink.Name = "linkLabelLink";
            this.linkLabelLink.Size = new System.Drawing.Size(63, 18);
            this.linkLabelLink.TabIndex = 12;
            this.linkLabelLink.TabStop = true;
            this.linkLabelLink.Text = "Ссылка";
            this.linkLabelLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLink_LinkClicked);
            // 
            // contextMenuStripLinkLabel
            // 
            this.contextMenuStripLinkLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyLinkToolStripMenuItem,
            this.copyLinkMapToolStripMenuItem});
            this.contextMenuStripLinkLabel.Name = "contextMenuStripLinkLabel";
            this.contextMenuStripLinkLabel.Size = new System.Drawing.Size(232, 70);
            // 
            // copyLinkToolStripMenuItem
            // 
            this.copyLinkToolStripMenuItem.Name = "copyLinkToolStripMenuItem";
            this.copyLinkToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.copyLinkToolStripMenuItem.Text = "Копировать ссылку";
            this.copyLinkToolStripMenuItem.Click += new System.EventHandler(this.copyLinkToolStripMenuItem_Click);
            // 
            // flowLayoutPanelImages
            // 
            this.flowLayoutPanelImages.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelImages.AutoScroll = true;
            this.flowLayoutPanelImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.flowLayoutPanelImages.Location = new System.Drawing.Point(3, 50);
            this.flowLayoutPanelImages.Name = "flowLayoutPanelImages";
            this.flowLayoutPanelImages.Size = new System.Drawing.Size(566, 120);
            this.flowLayoutPanelImages.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(3, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 22);
            this.label2.TabIndex = 11;
            this.label2.Text = "Описание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxDescription.Location = new System.Drawing.Point(3, 198);
            this.textBoxDescription.MaximumSize = new System.Drawing.Size(0, 204);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDescription.Size = new System.Drawing.Size(566, 204);
            this.textBoxDescription.TabIndex = 7;
            this.textBoxDescription.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 405);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 22);
            this.label1.TabIndex = 9;
            this.label1.Text = "Комментарии";
            // 
            // textBoxComments
            // 
            this.textBoxComments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComments.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxComments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxComments.Location = new System.Drawing.Point(3, 430);
            this.textBoxComments.MaximumSize = new System.Drawing.Size(0, 212);
            this.textBoxComments.Multiline = true;
            this.textBoxComments.Name = "textBoxComments";
            this.textBoxComments.ReadOnly = true;
            this.textBoxComments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxComments.Size = new System.Drawing.Size(566, 212);
            this.textBoxComments.TabIndex = 8;
            this.textBoxComments.TabStop = false;
            // 
            // copyLinkMapToolStripMenuItem
            // 
            this.copyLinkMapToolStripMenuItem.Name = "copyLinkMapToolStripMenuItem";
            this.copyLinkMapToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.copyLinkMapToolStripMenuItem.Text = "Копировать ссылку на карту";
            this.copyLinkMapToolStripMenuItem.Click += new System.EventHandler(this.copyLinkMapToolStripMenuItem_Click);
            // 
            // FormShowObjectInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(581, 653);
            this.Controls.Add(this.flowLayoutPanelWrapper);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormShowObjectInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormShowObjectInfo";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formShowObjectInfo_FormClosed);
            this.Shown += new System.EventHandler(this.formShowObjectInfo_Shown);
            this.flowLayoutPanelWrapper.ResumeLayout(false);
            this.flowLayoutPanelWrapper.PerformLayout();
            this.contextMenuStripLinkLabel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelWrapper;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelImages;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxComments;
        private System.Windows.Forms.LinkLabel linkLabelLink;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLinkLabel;
        private System.Windows.Forms.ToolStripMenuItem copyLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyLinkMapToolStripMenuItem;
    }
}