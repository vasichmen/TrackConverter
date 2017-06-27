using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;

namespace TrackConverter.UI.Common.Dialogs
{
    /// <summary>
    /// Окно информации о новой версии
    /// </summary>
    public partial class FormUpdateDialog : Form
    {
        private VersionInfo vi;

        private FormUpdateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// создает окно новой версии с указанной информацией
        /// </summary>
        /// <param name="vi">Информация о новой версии</param>
        public FormUpdateDialog(VersionInfo vi) : this()
        {
            this.vi = vi;
            Text = "Доступна новая версия " + vi.VersionText;
            labelСur.Text = "Текущая версия: " + Vars.Options.Common.VersionText;
            labelNew.Text = "Новая версия: " + vi.VersionText;
            textBoxChanges.Text = vi.Changes + "\r\nДата: " + vi.ReleaseDate.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// переход на сайт для обновления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (checkBoxRemember.Checked)
                Vars.Options.Common.UpdateMode = UpdateDialogAnswer.AlwaysAccept;
            Process.Start(Vars.Options.Common.SiteAddress + "/files/downloads/TrackConverter_"+vi.VersionText.Substring(0,5)+".zip"); 
            this.Close();
        }

        /// <summary>
        /// отмена обновления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            if (checkBoxRemember.Checked)
                Vars.Options.Common.UpdateMode = UpdateDialogAnswer.AlwaysIgnore;
            Close();
        }
    }
}
