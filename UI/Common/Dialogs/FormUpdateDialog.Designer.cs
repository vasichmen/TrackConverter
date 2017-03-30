namespace TrackConverter.UI.Common.Dialogs
{
    partial class FormUpdateDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdateDialog));
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.textBoxChanges = new System.Windows.Forms.TextBox();
            this.checkBoxRemember = new System.Windows.Forms.CheckBox();
            this.buttonIgnore = new System.Windows.Forms.Button();
            this.labelСur = new System.Windows.Forms.Label();
            this.labelNew = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonUpdate.Location = new System.Drawing.Point(15, 255);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(91, 23);
            this.buttonUpdate.TabIndex = 0;
            this.buttonUpdate.Text = "Обновить";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // textBoxChanges
            // 
            this.textBoxChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxChanges.Enabled = false;
            this.textBoxChanges.Location = new System.Drawing.Point(15, 66);
            this.textBoxChanges.Multiline = true;
            this.textBoxChanges.Name = "textBoxChanges";
            this.textBoxChanges.Size = new System.Drawing.Size(278, 160);
            this.textBoxChanges.TabIndex = 1;
            // 
            // checkBoxRemember
            // 
            this.checkBoxRemember.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxRemember.AutoSize = true;
            this.checkBoxRemember.Location = new System.Drawing.Point(15, 232);
            this.checkBoxRemember.Name = "checkBoxRemember";
            this.checkBoxRemember.Size = new System.Drawing.Size(140, 17);
            this.checkBoxRemember.TabIndex = 2;
            this.checkBoxRemember.Text = "Запомнить мой выбор";
            this.checkBoxRemember.UseVisualStyleBackColor = true;
            // 
            // buttonIgnore
            // 
            this.buttonIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonIgnore.Location = new System.Drawing.Point(202, 255);
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.Size = new System.Drawing.Size(91, 23);
            this.buttonIgnore.TabIndex = 3;
            this.buttonIgnore.Text = "Игнорировать";
            this.buttonIgnore.UseVisualStyleBackColor = true;
            this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
            // 
            // labelСur
            // 
            this.labelСur.AutoSize = true;
            this.labelСur.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelСur.Location = new System.Drawing.Point(12, 9);
            this.labelСur.Name = "labelСur";
            this.labelСur.Size = new System.Drawing.Size(190, 17);
            this.labelСur.TabIndex = 4;
            this.labelСur.Text = "Изменения в новой версии:";
            // 
            // labelNew
            // 
            this.labelNew.AutoSize = true;
            this.labelNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelNew.Location = new System.Drawing.Point(12, 29);
            this.labelNew.Name = "labelNew";
            this.labelNew.Size = new System.Drawing.Size(190, 17);
            this.labelNew.TabIndex = 5;
            this.labelNew.Text = "Изменения в новой версии:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Изменения в новой версии:";
            // 
            // FormUpdateDialog
            // 
            this.AcceptButton = this.buttonUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 291);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelNew);
            this.Controls.Add(this.labelСur);
            this.Controls.Add(this.buttonIgnore);
            this.Controls.Add(this.checkBoxRemember);
            this.Controls.Add(this.textBoxChanges);
            this.Controls.Add(this.buttonUpdate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(324, 329);
            this.Name = "FormUpdateDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Доступна новая версия!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.TextBox textBoxChanges;
        private System.Windows.Forms.CheckBox checkBoxRemember;
        private System.Windows.Forms.Button buttonIgnore;
        private System.Windows.Forms.Label labelСur;
        private System.Windows.Forms.Label labelNew;
        private System.Windows.Forms.Label label1;
    }
}