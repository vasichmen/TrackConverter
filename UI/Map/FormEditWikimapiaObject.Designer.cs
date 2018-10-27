namespace TrackConverter.UI.Map
{
    partial class FormEditWikimapiaObject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditWikimapiaObject));
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonEditPhotos = new System.Windows.Forms.Button();
            this.buttonEditGeometry = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxBuilding = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxIsBuilding = new System.Windows.Forms.CheckBox();
            this.textBoxWikiLink = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRemoveCategory = new System.Windows.Forms.Button();
            this.buttonAddCategory = new System.Windows.Forms.Button();
            this.listBoxCategories = new System.Windows.Forms.ListBox();
            this.listBoxCatSearch = new System.Windows.Forms.ListBox();
            this.textBoxCatSearch = new System.Windows.Forms.TextBox();
            this.comboBoxStreet = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(15, 76);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(427, 20);
            this.textBoxTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Название";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Описание";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(15, 115);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(427, 160);
            this.textBoxDescription.TabIndex = 2;
            // 
            // buttonEditPhotos
            // 
            this.buttonEditPhotos.Location = new System.Drawing.Point(15, 12);
            this.buttonEditPhotos.Name = "buttonEditPhotos";
            this.buttonEditPhotos.Size = new System.Drawing.Size(168, 23);
            this.buttonEditPhotos.TabIndex = 4;
            this.buttonEditPhotos.Text = "Редактировать фоготрафии";
            this.buttonEditPhotos.UseVisualStyleBackColor = true;
            this.buttonEditPhotos.Click += new System.EventHandler(this.buttonEditPhotos_Click);
            // 
            // buttonEditGeometry
            // 
            this.buttonEditGeometry.Location = new System.Drawing.Point(189, 12);
            this.buttonEditGeometry.Name = "buttonEditGeometry";
            this.buttonEditGeometry.Size = new System.Drawing.Size(162, 23);
            this.buttonEditGeometry.TabIndex = 5;
            this.buttonEditGeometry.Text = "Редактировать контур";
            this.buttonEditGeometry.UseVisualStyleBackColor = true;
            this.buttonEditGeometry.Click += new System.EventHandler(this.buttonEditGeometry_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxStreet);
            this.groupBox1.Controls.Add(this.textBoxBuilding);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(15, 320);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 66);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Адрес";
            // 
            // textBoxBuilding
            // 
            this.textBoxBuilding.Location = new System.Drawing.Point(293, 32);
            this.textBoxBuilding.Name = "textBoxBuilding";
            this.textBoxBuilding.Size = new System.Drawing.Size(94, 20);
            this.textBoxBuilding.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(290, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Номер дома";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Улица";
            // 
            // checkBoxIsBuilding
            // 
            this.checkBoxIsBuilding.AutoSize = true;
            this.checkBoxIsBuilding.Location = new System.Drawing.Point(15, 392);
            this.checkBoxIsBuilding.Name = "checkBoxIsBuilding";
            this.checkBoxIsBuilding.Size = new System.Drawing.Size(185, 17);
            this.checkBoxIsBuilding.TabIndex = 7;
            this.checkBoxIsBuilding.Text = "Этот объект является зданием";
            this.checkBoxIsBuilding.UseVisualStyleBackColor = true;
            // 
            // textBoxWikiLink
            // 
            this.textBoxWikiLink.Location = new System.Drawing.Point(15, 294);
            this.textBoxWikiLink.Name = "textBoxWikiLink";
            this.textBoxWikiLink.Size = new System.Drawing.Size(427, 20);
            this.textBoxWikiLink.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Ссылка на Википедию";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(15, 415);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(118, 44);
            this.buttonSave.TabIndex = 12;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(324, 415);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(118, 44);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonRemoveCategory);
            this.groupBox2.Controls.Add(this.buttonAddCategory);
            this.groupBox2.Controls.Add(this.listBoxCategories);
            this.groupBox2.Controls.Add(this.listBoxCatSearch);
            this.groupBox2.Controls.Add(this.textBoxCatSearch);
            this.groupBox2.Location = new System.Drawing.Point(475, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 399);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Категории объекта";
            // 
            // buttonRemoveCategory
            // 
            this.buttonRemoveCategory.Location = new System.Drawing.Point(104, 361);
            this.buttonRemoveCategory.Name = "buttonRemoveCategory";
            this.buttonRemoveCategory.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveCategory.TabIndex = 5;
            this.buttonRemoveCategory.Text = "Удалить";
            this.buttonRemoveCategory.UseVisualStyleBackColor = true;
            this.buttonRemoveCategory.Click += new System.EventHandler(this.buttonRemoveCategory_Click);
            // 
            // buttonAddCategory
            // 
            this.buttonAddCategory.Location = new System.Drawing.Point(104, 198);
            this.buttonAddCategory.Name = "buttonAddCategory";
            this.buttonAddCategory.Size = new System.Drawing.Size(75, 23);
            this.buttonAddCategory.TabIndex = 4;
            this.buttonAddCategory.Text = "Добавить";
            this.buttonAddCategory.UseVisualStyleBackColor = true;
            this.buttonAddCategory.Click += new System.EventHandler(this.buttonAddCategory_Click);
            // 
            // listBoxCategories
            // 
            this.listBoxCategories.FormattingEnabled = true;
            this.listBoxCategories.Location = new System.Drawing.Point(6, 234);
            this.listBoxCategories.Name = "listBoxCategories";
            this.listBoxCategories.Size = new System.Drawing.Size(290, 121);
            this.listBoxCategories.TabIndex = 3;
            // 
            // listBoxCatSearch
            // 
            this.listBoxCatSearch.FormattingEnabled = true;
            this.listBoxCatSearch.Location = new System.Drawing.Point(6, 45);
            this.listBoxCatSearch.Name = "listBoxCatSearch";
            this.listBoxCatSearch.Size = new System.Drawing.Size(290, 147);
            this.listBoxCatSearch.TabIndex = 2;
            // 
            // textBoxCatSearch
            // 
            this.textBoxCatSearch.Location = new System.Drawing.Point(6, 19);
            this.textBoxCatSearch.Name = "textBoxCatSearch";
            this.textBoxCatSearch.Size = new System.Drawing.Size(290, 20);
            this.textBoxCatSearch.TabIndex = 0;
            this.textBoxCatSearch.TextChanged += new System.EventHandler(this.textBoxCatSearch_TextChanged);
            // 
            // comboBoxStreet
            // 
            this.comboBoxStreet.FormattingEnabled = true;
            this.comboBoxStreet.Location = new System.Drawing.Point(31, 32);
            this.comboBoxStreet.Name = "comboBoxStreet";
            this.comboBoxStreet.Size = new System.Drawing.Size(215, 21);
            this.comboBoxStreet.TabIndex = 10;
            // 
            // FormEditWikimapiaObject
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(789, 468);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxWikiLink);
            this.Controls.Add(this.checkBoxIsBuilding);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonEditGeometry);
            this.Controls.Add(this.buttonEditPhotos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormEditWikimapiaObject";
            this.Text = "FormEditWikimapiaObject";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.formEditWikimapiaObject_FormClosed);
            this.Shown += new System.EventHandler(this.formEditWikimapiaObject_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonEditPhotos;
        private System.Windows.Forms.Button buttonEditGeometry;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxBuilding;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxIsBuilding;
        private System.Windows.Forms.TextBox textBoxWikiLink;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonRemoveCategory;
        private System.Windows.Forms.Button buttonAddCategory;
        private System.Windows.Forms.ListBox listBoxCategories;
        private System.Windows.Forms.ListBox listBoxCatSearch;
        private System.Windows.Forms.TextBox textBoxCatSearch;
        private System.Windows.Forms.ComboBox comboBoxStreet;
    }
}