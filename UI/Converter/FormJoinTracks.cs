using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Converter
{
    /// <summary>
    /// окно объединения треков  в один
    /// </summary>
    public partial class FormJoinTracks : Form
    {
        /// <summary>
        /// список мкршрутов для объединения
        /// </summary>
        private TrackFileList tracks;

        /// <summary>
        /// создает новое окно объединеиния треков
        /// </summary>
        public FormJoinTracks()
        {
            InitializeComponent();
            tracks = new TrackFileList();
        }

        /// <summary>
        /// добавление трека в список
        /// </summary>
        /// <param name="trackFile">трек для добавления</param>
        public void AddTrack(TrackFile trackFile)
        {
            if (!tracks.Contains(trackFile))
            {
                tracks.Add(trackFile);
                listBoxTracks.Items.Add(trackFile.Name);
            }
            else
                if (MessageBox.Show(this, "Этот маршрут уже добавлен. Добавить повторно?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    tracks.Add(trackFile);
                    listBoxTracks.Items.Add(trackFile.Name);
                }
            listBoxTracks.SelectedIndex = 0;
        }

        /// <summary>
        /// сдвиг трека вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (listBoxTracks.SelectedIndex == -1) return;
            if (listBoxTracks.SelectedIndex == 0) return;
            TrackFile tm = tracks[listBoxTracks.SelectedIndex];
            tracks.Remove(tm);
            listBoxTracks.SelectedIndex--;
            tracks.Insert(listBoxTracks.SelectedIndex, tm);
            RefreshList(listBoxTracks.SelectedIndex);
        }

        /// <summary>
        /// сдвиг трека вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (listBoxTracks.SelectedIndex == -1) return;
            if (listBoxTracks.SelectedIndex == tracks.Count - 1) return;
            TrackFile tm = tracks[listBoxTracks.SelectedIndex];
            tracks.Remove(tm);
            listBoxTracks.SelectedIndex++;
            tracks.Insert(listBoxTracks.SelectedIndex, tm);
            RefreshList(listBoxTracks.SelectedIndex);
        }

        /// <summary>
        /// сохранение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            TrackFile tf = tracks.JoinTracks();
            tf.CalculateAll();
            tf.Color = Vars.Options.Converter.GetColor();
            FormReadText rt = new FormReadText(DialogType.ReadText, "Введите имя нового маршрута", "", false, false, false, false);
            if (rt.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                tf.Name = rt.Result;
            else
                return;

            Program.winConverter.AddRouteToList(tf);
            Close();
        }

        /// <summary>
        /// закрытие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.tracks.Clear();
            this.Close();
        }

        /// <summary>
        /// обновление списка треков
        /// </summary>
        private void RefreshList(int newselect)
        {
            listBoxTracks.Items.Clear();
            foreach (TrackFile tf in tracks)
                listBoxTracks.Items.Add(tf.Name);
            listBoxTracks.SelectedIndex = newselect;
        }

    }
}
