namespace TrackConverter.UI.Map
{
    partial class FormSelectPoint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectPoint));
            this.listBoxPoints = new System.Windows.Forms.ListBox();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCycled = new System.Windows.Forms.Button();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxPoints
            // 
            this.listBoxPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPoints.FormattingEnabled = true;
            this.listBoxPoints.Location = new System.Drawing.Point(9, 52);
            this.listBoxPoints.Margin = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.listBoxPoints.Name = "listBoxPoints";
            this.listBoxPoints.Size = new System.Drawing.Size(238, 277);
            this.listBoxPoints.TabIndex = 0;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelect.Location = new System.Drawing.Point(252, 23);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new System.Drawing.Size(75, 23);
            this.buttonSelect.TabIndex = 1;
            this.buttonSelect.Text = "Сохранить";
            this.buttonSelect.UseVisualStyleBackColor = true;
            this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(252, 52);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Выберите начальную точку:";
            // 
            // buttonCycled
            // 
            this.buttonCycled.Location = new System.Drawing.Point(252, 81);
            this.buttonCycled.Name = "buttonCycled";
            this.buttonCycled.Size = new System.Drawing.Size(75, 40);
            this.buttonCycled.TabIndex = 4;
            this.buttonCycled.Text = "Круговой маршрут";
            this.buttonCycled.UseVisualStyleBackColor = true;
            this.buttonCycled.Click += new System.EventHandler(this.buttonCycled_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(9, 25);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(144, 20);
            this.textBoxSearch.TabIndex = 5;
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            // 
            // buttonFind
            // 
            this.buttonFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonFind.BackgroundImage")));
            this.buttonFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonFind.Location = new System.Drawing.Point(159, 23);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(23, 23);
            this.buttonFind.TabIndex = 6;
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // FormSelectPoint
            // 
            this.AcceptButton = this.buttonSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(339, 342);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.buttonCycled);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSelect);
            this.Controls.Add(this.listBoxPoints);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSelectPoint";
            this.Text = "Выбор точки";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxPoints;
        private System.Windows.Forms.Button buttonSelect;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCycled;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonFind;
    }
}