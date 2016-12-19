﻿using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TrackConverter.Lib;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Map;
using System.Text.RegularExpressions;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно редактирования путевых точек
    /// </summary>
    public partial class FormPoints : Form
    {

        /// <summary>
        /// список загруженных точек
        /// </summary>
        private TrackFile Points;

        /// <summary>
        /// если истина, то при закрытии окна будет вызван метод map.EndEditWaypoints
        /// </summary>
        private Action<TrackFile> endEditWaypointsAction;

        /// <summary>
        /// если истина, то есть несохраненные изменения
        /// </summary>
        private bool isEdited = false;

        #region конструкторы

        /// <summary>
        /// создает новое окно списка точек
        /// </summary>
        public FormPoints()
            : this(null, null) { }

        /// <summary>
        /// создает экземпляр окна и загружает указанный трек
        /// </summary>
        /// <param name="trackFile">трек для загрузки</param>
        public FormPoints(TrackFile trackFile)
            : this(trackFile, null) { }

        /// <summary>
        /// создает экземпляр окна с заданным списком точек
        /// </summary>
        /// <param name="waypoints">списк путевых точет для открытия</param>
        /// <param name="callEndEditWaypointsOnMap">если истина, то при закрытии окна будет вызван метод map.EndEditWaypoints</param>
        public FormPoints(TrackFile waypoints, Action<TrackFile> callEndEditWaypointsOnMap)
        {
            InitializeComponent();

            if (waypoints == null)
                waypoints = new TrackFile();

            this.Points = waypoints;
            this.endEditWaypointsAction = callEndEditWaypointsOnMap;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Points.Source;
            dataGridView1.Refresh();
        }

        #endregion

        #region события

        #region глвное меню

        /// <summary>
        /// загрузка файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Points = new TrackFile();
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.kml, *.gpx, *.kmz,*.rte)|*.crd; *.wpt; *.plt; *rt2; *.kml;*.kmz; *.gpx; *.rte";
                of.Filter += "|Треки Androzic (*.plt)|*.plt";
                of.Filter += "|Маршрут Androzic (*.rt2)|*.rt2";
                of.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
                of.Filter += "|Файл координат(*.crd)|*.crd";
                of.Filter += "|Файл Google Earth(*.kml)|*.kml";
                of.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
                of.Filter += "|Файл маршрутов OziExplorer(*.rte)|*.rte";
                of.Filter += "|Файл Google Earth(*.kmz)|*.kmz";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = false;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Points = Serializer.DeserializeTrackFile(of.FileName);

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = Points.Source;
                    dataGridView1.Refresh();
                    Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(of.FileName);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }

        /// <summary>
        /// сохранение файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Points.Source = (DataTable)dataGridView1.DataSource;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Файл маршрута Androzic (*.rt2)|*.rt2";
            sf.Filter += "|Треки Androzic (*.plt)|*.plt";
            sf.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
            sf.Filter += "|Файл координат(*.crd)|*.crd";
            sf.Filter += "|Файл GPS координат(*.gpx)|*.gpx";


            sf.AddExtension = true;
            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;


            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (sf.FilterIndex)
                {
                    case 1:
                        Serializer.Serialize(sf.FileName, Points, FileFormats.Rt2File);
                        break;
                    case 2:
                        Serializer.Serialize(sf.FileName, Points, FileFormats.PltFile);
                        break;
                    case 3:
                        Serializer.Serialize(sf.FileName, Points, FileFormats.WptFile);
                        break;
                    case 4:
                        Serializer.Serialize(sf.FileName, Points, FileFormats.CrdFile);
                        break;
                    case 5:
                        Serializer.Serialize(sf.FileName, Points, FileFormats.GpxFile);
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                this.isEdited = false;
                MessageBox.Show("Файл сохранен!");
            }
        }

        /// <summary>
        /// загрузка з ссылок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormReadText of = new FormReadText(DialogType.ReadText, "Введите ссылку на маршрут: ", "", false, false, false, false);
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TrackFile tf = new TrackFile();
                    tf = Serializer.DeserializeTrackFile(of.Result);
                    Points = tf;
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = Points.Source;
                    dataGridView1.Refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary>
        /// открытие точек на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshAzimuthsToolStripMenuItem_Click(null, null);
            if (Program.winMapNullOrDisposed)
            {
                Program.winMap = new FormMap();
            }
            if (!Program.winMap.Visible)
                Program.winMap.Show();
            Program.winMap.ShowPoints(Points,false, false);
            Program.winMap.Activate();
        }

        /// <summary>
        /// выделение заданной точки в списке
        /// </summary>
        /// <param name="pt"></param>
        internal void SelectPoint(TrackPoint pt)
        {
            try
            {
                if (Points.Contains(pt))
                {
                    int ind = Points.IndexOf(pt);
                    dataGridView1.Rows[ind].Selected = true;
                    dataGridView1.Rows[ind].Visible = true;
                    dataGridView1.CurrentCell = dataGridView1[0, ind];
                }
            }
            catch (NullReferenceException) { }
        }

        /// <summary>
        /// обновление полей и сохранение изменений из dataGridView в this.Points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshAzimuthsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Validate();
            dataGridView1.Update();
            dataGridView1.EndEdit();
            try
            {
                DataTable dt = (DataTable)dataGridView1.DataSource;
                Points.Source = dt;

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Points.Source;
                dataGridView1.Refresh();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #endregion

        #region  меню DGV

        /// <summary>
        /// открыть точку в конвертере координат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Points.Source = (DataTable)dataGridView1.DataSource;
            int row = dataGridView1.SelectedCells[0].RowIndex;
            new FormTransformCoordinate(Points[row]).Show();
        }

        /// <summary>
        /// удалить точку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.SuspendLayout();

            int row = dataGridView1.SelectedCells[0].RowIndex;
            Points.Remove(row);


            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Points.Source;
            dataGridView1.Refresh();

            Vars.currentSelectedTrack = Points;
            Program.RefreshWindows(this);

            //этот кусок удаляет почему-то еще одну строку перед выделенной 
            //if (dataGridView1.Rows.Count != 0)
            //    if (row - 1 > 0)
            //        dataGridView1.Rows[row - 1].Selected = true;
            //    else
            //        dataGridView1.Rows[0].Selected = true;

            dataGridView1.ResumeLayout();
            this.isEdited = true;
        }

        /// <summary>
        /// открытие точки в яндекс картах
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showYandexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            TrackPoint tt = Points[row];
            Process.Start(tt.Coordinates.ExportYandex());
        }

        /// <summary>
        /// открытие точки в гугл картах
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showGoogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            TrackPoint tt = Points[row];
            Process.Start(tt.Coordinates.ExportGoogle());
        }

        /// <summary>
        /// добавление новой точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;

            FormEditPoint fep = new FormEditPoint();

            if (fep.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Points.Insert(row, fep.Result);
                Points.CalculateAll();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Points.Source;
                dataGridView1.Refresh();
                this.isEdited = true;

            }
        }

        /// <summary>
        /// редактирование точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            TrackPoint tt = Points[row];
            FormEditPoint fep = new FormEditPoint(tt);
            if (fep.ShowDialog() == DialogResult.OK)
            {
                Points[row] = fep.Result;
                Points.CalculateAll();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Points.Source;
                dataGridView1.Refresh();
                this.isEdited = true;

                Vars.currentSelectedTrack = this.Points;
                Program.RefreshWindows(this);
            }
        }

        /// <summary>
        /// показать точку на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winMapNullOrDisposed)
                Program.winMap = new FormMap();
            if (!Program.winMap.Visible)
                Program.winMap.Show();
            int row = dataGridView1.SelectedCells[0].RowIndex;
            TrackPoint tt = Points[row];
            Program.winMap.ShowPoint(tt, false);
            Program.winMap.Activate();
        }

        #endregion

        #region события DGV

        /// <summary>
        /// удаление элемента из списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.RowCount == 0) return;
            if (e.KeyData != Keys.Delete) return;
            removeToolStripMenuItem_Click(null, null);
        }

        /// <summary>
        /// загрузка файлов перетаскиванием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.UnicodeText))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// загрузка файлов перетаскиванием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            bool loaded = false;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string ss in s)
                    Points = Serializer.DeserializeTrackFile(ss);
                loaded = true;
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string s = (string)e.Data.GetData(DataFormats.UnicodeText, false);
                Points = Serializer.DeserializeTrackFile(s);
                loaded = true;
            }
            if (loaded)
            {
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Points.Source;
                dataGridView1.Refresh();
            }
        }

        /// <summary>
        /// перерасчет неизменяемых полей (азимут, склонение)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// выделение нажатой ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Selected = true;
            }
        }

        /// <summary>
        /// обновление столбцов и сохранение в this.Points при изменении значений координат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            isEdited = true;

            if (e.ColumnIndex == 1)
                if ((string)dataGridView1[2, e.RowIndex].FormattedValue != string.Empty)
                    refreshAzimuthsToolStripMenuItem_Click(null, null);
            if (e.ColumnIndex == 2)
                if ((string)dataGridView1[1, e.RowIndex].FormattedValue != string.Empty)
                    refreshAzimuthsToolStripMenuItem_Click(null, null);
            //сброс ошибок строк
            dataGridView1.Rows[e.RowIndex].ErrorText = null;
        }

        /// <summary>
        /// проверка правильного ввода значений долготы, широты, высоты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow) return;
            if (dataGridView1.IsCurrentCellDirty) //если текущая ячейка редактируется
                if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3) //если это широта, долгота, высота
                    if (((string)e.FormattedValue).Trim() != "") //если не пустое значение
                        if (!Regex.IsMatch((string)e.FormattedValue, @"^[0-9\.\,\-]+$")) //если есть что-то кроме цифр
                        {//то отменяем сохранение изменений и выводим сообщение
                            e.Cancel = true;
                            dataGridView1[e.ColumnIndex, e.RowIndex].Value = DBNull.Value;
                            //MessageBox.Show("В это поле можно вводить только цифры");
                            dataGridView1.Rows[e.RowIndex].ErrorText = "Значение должно быть числом";
                            return;
                        }

        }

        /// <summary>
        /// открытие точки для редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            TrackPoint tt = Points[row];
            FormEditPoint fep = new FormEditPoint(tt);
            if (fep.ShowDialog() == DialogResult.OK)
            {
                Points[row] = fep.Result;
                Points.CalculateAll();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = Points.Source;
                dataGridView1.Refresh();
            }
        }

        /// <summary>
        /// нажатие на ячейке и выделение ее на графике и карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            dataGridView1.Rows[e.RowIndex].Selected = true;

        }

        /// <summary>
        /// обновление окон при выделении точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 1)
                return;
            int ind = dataGridView1.SelectedRows[0].Index;

            if (ind >= Points.Count)
                return;

            TrackPoint tt = Points[ind];
            Program.winMap.SelectPoint(tt);
            Program.winElevVisual.SelectPoint(tt);
        }

        #endregion


        /// <summary>
        /// закрытие окна и вызов метода завершения редактирования точек на карте, если необходимо
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPoints_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this == Program.winPoints) //если это часть основного окна, то подтверждение выхода
                if (isEdited)
                    if (MessageBox.Show(this, "Точки не сохранены, вы действительно хотите выйти?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }

            if (this != Program.winPoints) //если это не часть основного окна, то выполняем действие
                if (endEditWaypointsAction != null)
                {
                    Points.Source = (DataTable)dataGridView1.DataSource;
                    endEditWaypointsAction.Invoke(Points);
                }
        }

        #endregion

        #region взаимодействие

        /// <summary>
        /// установить трек для отображения точек
        /// </summary>
        /// <param name="tf"></param>
        internal void SetTrack(TrackFile tf)
        {
            this.Points = tf;
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Points.Source;
            dataGridView1.Refresh();
        }

        /// <summary>
        /// очистка списка точек
        /// </summary>
        internal void Clear()
        {
            this.Points.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Points.Source;
            dataGridView1.Refresh();
        }

        /// <summary>
        /// обновление списка точек в выделенном маршруте
        /// </summary>
        internal void RefreshData()
        {
            if (Vars.currentSelectedTrack != null)
                this.Points = Vars.currentSelectedTrack;
            else
                this.Points.Clear();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Points.Source;
            dataGridView1.Refresh();
        }


        #endregion
    }
}
