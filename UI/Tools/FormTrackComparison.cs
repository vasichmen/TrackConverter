using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Mathematic.Assessment;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Converter;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно сравнения треков 
    /// </summary>
    public partial class FormTrackComparison : Form
    {

        /// <summary>
        /// список треков для сравнения
        /// </summary>
        private TrackFileList Tracks;


        /// <summary>
        /// создает окно сравнения треков с заданным списком
        /// </summary>
        public FormTrackComparison()
        {
            InitializeComponent();
            this.Tracks = new TrackFileList();
        }

        /// <summary>
        /// добавление нового трека в сравнение
        /// </summary>
        /// <param name="trackFile">добавляемый трек</param>
        public void AddTrack(BaseTrack trackFile)
        {
            this.Tracks.Add(trackFile);
        }

        /// <summary>
        /// окончание добавления треков  и вычисление результатов
        /// </summary>
        public void CalculateComparison()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (Tracks.TotalPoints > 10000 && !Vars.Options.Converter.IsApproximateAltitudes)
                    MessageBox.Show(this, "Обработка больших маршрутов без использования аппроксимации может занять длительное время", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DataTable dt = ElevationAnalysis.AnalyzeTracks(Tracks);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dt;
                dataGridView1.Refresh();
            }
            catch (ApplicationException e)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Refresh();
                MessageBox.Show(this, e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// закрашивание красным и зеленым худших и лучших параметров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            if (dataGridView1.RowCount > 1 && dataGridView1.ColumnCount > 1)
            {
                for (int i = 0; i < Tracks.Count; i++)
                    dataGridView1["Название", i].Style = new DataGridViewCellStyle() { BackColor = Tracks[i].Color };

                FillColumn("Длина, км", Color.LightGreen, Color.Red);
                FillColumn("Перепад высот, м", Color.LightGreen, Color.Red);
                FillColumn("Отклонение по высоте, м", Color.LightGreen, Color.Red);
                FillColumn("Общая длина подъемов,км", Color.LightGreen, Color.Red);
                FillColumn("Подъемы, %", Color.LightGreen, Color.Red);
                FillColumn("Общая длина спусков,км", Color.Red, Color.LightGreen);
                FillColumn("Спуски, %", Color.Red, Color.LightGreen);
                FillColumn("Максимальная крутизна подъема, º", Color.LightGreen, Color.Red);
                FillColumn("Максимальная крутизна спуска, º", Color.Red, Color.LightGreen);
            }
        }

        /// <summary>
        /// закрашивание минимальных и максимальных элементов в заданной колонке заданным цветом
        /// </summary>
        /// <param name="colName">название колонки</param>
        /// <param name="minColor">цвет для минимального элемента</param>
        /// <param name="maxColor">цвет для максимального элемента</param>
        private void FillColumn(string colName, Color minColor, Color maxColor)
        {
            int Min = GetMin(colName);
            int Max = GetMax(colName);
            dataGridView1[colName, Min].Style = new DataGridViewCellStyle() { BackColor = minColor };
            dataGridView1[colName, Max].Style = new DataGridViewCellStyle() { BackColor = maxColor };
        }

        /// <summary>
        /// находит индекс минимального значение в заданной колонке
        /// </summary>
        /// <param name="colName">имя колонки</param>
        /// <returns></returns>
        private int GetMin(string colName)
        {
            int res = -1;
            double min = double.MaxValue;
            DataTable dt = (DataTable)dataGridView1.DataSource;
            DataColumn dc = dt.Columns[colName];

            int ctn = 0;
            foreach (DataRow dr in dt.Rows)
            {
                double val = dr.Field<double>(dc);
                if (val < min)
                {
                    min = val;
                    res = ctn;
                }
                ctn++;
            }
            return res;
        }

        /// <summary>
        /// находит индекс максимального значение в заданной колонке
        /// </summary>
        /// <param name="colName">имя колонки</param>
        /// <returns></returns>
        private int GetMax(string colName)
        {
            int res = -1;
            double max = double.MinValue;
            DataTable dt = (DataTable)dataGridView1.DataSource;
            DataColumn dc = dt.Columns[colName];

            int ctn = 0;
            foreach (DataRow dr in dt.Rows)
            {
                double val = dr.Field<double>(dc);
                if (val > max)
                {
                    max = val;
                    res = ctn;
                }
                ctn++;
            }
            return res;
        }
    }
}
