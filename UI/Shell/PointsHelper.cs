using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI.Shell
{
    class PointsHelper
    {
        private FormMain formMain;

        public PointsHelper(FormMain formMain)
        {
            this.formMain = formMain;
        }

        internal void RefreshData()
        {
            if (Vars.currentSelectedTrack != null)
            {
                if (Vars.currentSelectedTrack is TripRouteFile)
                {
                    formMain.Points.Clear();
                }
                else
                    formMain.Points = Vars.currentSelectedTrack;
            }
            else
                formMain.Points.Clear();

            FillDGV(formMain.Points.Source);
        }

        private void refreshAzimuths(object p1, object p2)
        {
            formMain.Validate();
            formMain.dataGridViewPoints.Update();
            formMain.dataGridViewPoints.EndEdit();
            try
            {
                DataTable dt = (DataTable)formMain.dataGridViewPoints.DataSource;
                formMain.Points.Source = dt;
                FillDGV(formMain.Points.Source);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        /// <summary>
        /// заполнение таблицы
        /// </summary>
        /// <param name="source"></param>
        public void FillDGV(object source)
        {
            formMain.dataGridViewPoints.DataSource = null;
            formMain.dataGridViewPoints.DataSource = source;
            formMain.dataGridViewPoints.Refresh();
            foreach (DataGridViewColumn column in formMain.dataGridViewPoints.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        /// <summary>
        /// выделение заданной точки в списке
        /// </summary>
        /// <param name="pt"></param>
        internal void SelectPoint(TrackPoint pt)
        {
            try
            {
                if (formMain.Points.Contains(pt))
                {
                    int ind = formMain.Points.IndexOf(pt);
                    formMain.dataGridViewPoints.Rows[ind].Selected = true;
                    formMain.dataGridViewPoints.Rows[ind].Visible = true;
                    formMain.dataGridViewPoints.CurrentCell = formMain.dataGridViewPoints[0, ind];
                }
            }
            catch (NullReferenceException) { }
        }

        #region События контекстного меню списка точек

        internal void toolStripAddPoint(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;

            FormEditPoint fep = new FormEditPoint();

            if (fep.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                formMain.Points.Insert(row, fep.Result);
                formMain.Points.CalculateAll();
                FillDGV(formMain.Points.Source);
                formMain.isEditedPoints = true;

            }
        }

        internal void toolStripEditPoint(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            TrackPoint tt = formMain.Points[row];
            FormEditPoint fep = new FormEditPoint(tt);
            if (fep.ShowDialog() == DialogResult.OK)
            {
                formMain.Points[row] = fep.Result;
                formMain.Points.CalculateAll();
                FillDGV(formMain.Points.Source);
                formMain.isEditedPoints = true;

                Vars.currentSelectedTrack = formMain.Points;
                formMain.converterHelper.RefreshData();
                formMain.mapHelper.RefreshData();
                formMain.graphHelper.RefreshData();
            }
        }

        internal void toolStripOpenWithConverter(object sender, EventArgs e)
        {
            if (formMain.Points == null || formMain.Points.Count == 0)
                return;
            formMain.Points.Source = (DataTable)formMain.dataGridViewPoints.DataSource;
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            new FormTransformCoordinate(formMain.Points[row]).Show();
        }

        internal void toolStripShowPointOnMap(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            TrackPoint tt = formMain.Points[row];
            formMain.mapHelper.ShowWaypoint(tt, false);
        }

        internal void toolStripShowYandex(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            TrackPoint tt = formMain.Points[row];
            Process.Start(tt.Coordinates.ExportYandex());
        }

        internal void toolStripShowGoogle(object sender, EventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            TrackPoint tt = formMain.Points[row];
            Process.Start(tt.Coordinates.ExportGoogle());
        }

        internal void toolStripRemovePoint(object sender, EventArgs e)
        {
            if (formMain.dataGridViewPoints.SelectedRows.Count == 0) return;

            int first = int.MaxValue;
            List<int> inds = new List<int>();
            foreach (DataGridViewRow r in formMain.dataGridViewPoints.SelectedRows)
            {
                inds.Add(r.Index);
                if (r.Index < first)
                    first = r.Index;
            }
            inds.Sort((a, b) => { if (a > b) return -1; else if (b > a) return 1; return 0; });
            for (int i = 0; i < inds.Count; i++)
                formMain.Points.Remove(inds[i]);


            FillDGV(formMain.Points.Source);

            //if (this == Program.winPoints)
            {
                formMain.Points.Source = (DataTable)this.formMain.dataGridViewPoints.DataSource;
                Vars.currentSelectedTrack = formMain.Points;
                formMain.converterHelper.UpdateSelectedTrack();
                formMain.converterHelper.RefreshData();
                formMain.mapHelper.RefreshData();
                formMain.graphHelper.RefreshData();
            }

            formMain.isEditedPoints = true;

            if (formMain.dataGridViewPoints.Rows.Count != 0)
            {
                if (first == 0)
                    first = 1;
                formMain.dataGridViewPoints.ClearSelection();
                formMain.dataGridViewPoints.Rows[first - 1].Selected = true;
                formMain.dataGridViewPoints.FirstDisplayedScrollingRowIndex = first - 1;
            }
        }

        #endregion

        #region События списка точек


        internal void dataGridCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            formMain.dataGridViewPoints.Rows[e.RowIndex].Selected = true;
        }

        internal void dataGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = formMain.dataGridViewPoints.SelectedCells[0].RowIndex;
            TrackPoint tt = formMain.Points[row];
            FormEditPoint fep = new FormEditPoint(tt);
            if (fep.ShowDialog() == DialogResult.OK)
            {
                formMain.Points[row] = fep.Result;
                Vars.currentSelectedTrack = formMain.Points;
                formMain.converterHelper.UpdateSelectedTrack();
                formMain.converterHelper.RefreshData();
                formMain.mapHelper.RefreshData();
                formMain.graphHelper.RefreshData();
                formMain.Points.CalculateAll();
                FillDGV(formMain.Points.Source);
                formMain.dataGridViewPoints.Rows[e.RowIndex].ErrorText = null;
                formMain.isEditedPoints = true;
            }
        }

        internal void dataGridCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                formMain.dataGridViewPoints[e.ColumnIndex, e.RowIndex].Selected = true;
            }
        }

        internal void dataGridCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (formMain.dataGridViewPoints.Rows[e.RowIndex].IsNewRow) return;
            if (formMain.dataGridViewPoints.IsCurrentCellDirty) //если текущая ячейка редактируется
                if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3) //если это широта, долгота, высота
                    if (((string)e.FormattedValue).Trim() != "") //если не пустое значение
                        if (((string)e.FormattedValue).Trim() != "NaN") //если это не NotAvailableNumber
                            if (!Regex.IsMatch((string)e.FormattedValue, @"^[0-9\.\,\-]+$")) //если есть что-то кроме цифр
                            {
                                //то отменяем сохранение изменений и выводим сообщение
                                e.Cancel = true;
                                formMain.dataGridViewPoints[e.ColumnIndex, e.RowIndex].Value = DBNull.Value;
                                formMain.dataGridViewPoints.Rows[e.RowIndex].ErrorText = "Значение должно быть числом или NaN";
                                return;
                            }
        }

        internal void dataGridCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            formMain.isEditedPoints = true;
            formMain.Points.Source = (DataTable)this.formMain.dataGridViewPoints.DataSource;
            Vars.currentSelectedTrack = formMain.Points;
            formMain.converterHelper.UpdateSelectedTrack();
            formMain.converterHelper.RefreshData();
            formMain.mapHelper.RefreshData();
            formMain.graphHelper.RefreshData();
            if (e.ColumnIndex == 1)
                if ((string)formMain.dataGridViewPoints[2, e.RowIndex].FormattedValue != string.Empty)
                    refreshAzimuths(null, null);
            if (e.ColumnIndex == 2)
                if ((string)formMain.dataGridViewPoints[1, e.RowIndex].FormattedValue != string.Empty)
                    refreshAzimuths(null, null);
            //сброс ошибок строк
            formMain.dataGridViewPoints.Rows[e.RowIndex].ErrorText = null;
        }


        internal void dataGridKeyDown(object sender, KeyEventArgs e)
        {
            if (formMain.dataGridViewPoints.RowCount == 0) return;
            if (e.KeyData != Keys.Delete) return;
            toolStripRemovePoint(null, null);
        }

        internal void dataGridSelectionChanged(object sender, EventArgs e)
        {
            if (formMain.dataGridViewPoints.SelectedRows.Count == 1)
            {
                int ind = formMain.dataGridViewPoints.SelectedRows[0].Index;

                if (ind >= formMain.Points.Count)
                    return;

                TrackPoint tt = formMain.Points[ind];
                formMain.mapHelper.SelectPoint(tt);
                formMain.graphHelper.SelectPoint(tt);
            }
        }

        #endregion
    }
}
