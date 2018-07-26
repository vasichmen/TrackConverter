using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// окно выбора вариантов
    /// </summary>
    public partial class FormChooseVariant : Form
    {
        /// <summary>
        /// список индексов вариантов, выбранных пользователем
        /// </summary>
        public List<int> Result { get; set; }

        /// <summary>
        /// создаёт окно выбора варианта
        /// </summary>
        /// <param name="variants">Список вариантов</param>
        /// <param name="selectionMode">тип выбора вариантов</param>
        /// <param name="caption">Заголовок окна</param>
        public FormChooseVariant(IEnumerable<object> variants, string caption, SelectionMode selectionMode)
        {
            InitializeComponent();
            Text = caption;
            listBoxVariants.Items.AddRange(variants.ToArray());
            listBoxVariants.SelectionMode = selectionMode;
            if (selectionMode == SelectionMode.One)
                this.listBoxVariants.SelectedIndexChanged += buttonOK_Click;
        }
        

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Result = new List<int>();
            foreach (int i in listBoxVariants.SelectedIndices)
                Result.Add(i);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
