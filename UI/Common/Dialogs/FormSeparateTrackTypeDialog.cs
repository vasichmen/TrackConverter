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
    /// окно выбора типа разделения маршрута (по расстоянию, по ближайшей точке)
    /// </summary>
    public partial class FormSeparateTrackTypeDialog : Form
    {
        /// <summary>
        /// Результат выполнения диалога (тип разделения)
        /// </summary>
        public SeparateRouteType Result = SeparateRouteType.None;

        /// <summary>
        /// Результат выполнения диалога (добавлять граничные в оба трека)
        /// </summary>
        public bool ResultAddCommon = true;

        /// <summary>
        /// Диалог выбора способа разделения маршрута на 2 части
        /// </summary>
        public FormSeparateTrackTypeDialog(string label, string caption)
        {
            InitializeComponent();
            textBoxLabel.Text = label;
            Text = caption;
        }

        private void buttonNearest_Click(object sender, EventArgs e)
        {
            Result = SeparateRouteType.Nearest;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonLength_Click(object sender, EventArgs e)
        {
            Result = SeparateRouteType.Length;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Result = SeparateRouteType.None;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormSeparateTrackTypeDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                ResultAddCommon = checkBoxAddCommonPoint.Checked;
        }
    }
}
