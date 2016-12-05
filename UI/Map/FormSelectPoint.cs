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

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно выбора точки из списка
    /// </summary>
    public partial class FormSelectPoint : Form
    {
        /// <summary>
        /// структура, добавляемая в список
        /// </summary>
        private struct ListItem
        {
            /// <summary>
            /// точка
            /// </summary>
            public TrackPoint point;

            /// <summary>
            /// строковое представление информации о точке
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return point.Name + point.Coordinates.ToString(" ({lat},{lon})", "00.000000");
            }
        }

        /// <summary>
        /// результат работы диалога
        /// </summary>
        public TrackPoint Result { get; set; }

        /// <summary>
        /// точки, которые в данный момент выводятся в списке
        /// </summary>
        private TrackFile points;

        /// <summary>
        /// полный список точек, за исключением точки, переданной параметром withoutPoint
        /// </summary>
        private TrackFile AllPoints;

        #region конструкторы

        /// <summary>
        /// выводит окно со списком точек для выбора. 
        /// Если диалог успешно завершен, то результирующая точка будет в свойстве Result
        /// </summary>
        /// <param name="points">точки для выбора</param>
        public FormSelectPoint(TrackFile points) : this(points, true) { }

        /// <summary>
        /// выводит окно со списком точек для выбора. 
        /// Если диалог успешно завершен, то результирующая точка будет в свойстве Result
        /// </summary>
        /// <param name="points">точки для выбора</param>
        /// <param name="isCycledEnable">если истина, то будет активна точка кругового маршрута</param>
        public FormSelectPoint(TrackFile points, bool isCycledEnable) : this(points, isCycledEnable, null) { }

        /// <summary>
        ///  выводит окно со списком точек для выбора. 
        /// Если диалог успешно завершен, то результирующая точка будет в свойстве Result
        /// </summary>
        /// <param name="points">список  точек на выбор</param>
        /// <param name="isCycledEnable">если истина, то будет активна точка кругового маршрута</param>
        /// <param name="labelText">текст надписи (какую точку выбирать)</param>
        public FormSelectPoint(TrackFile points, bool isCycledEnable, string labelText)
            : this(points, null, isCycledEnable, labelText) { }

        /// <summary>
        /// выводит окно со списком точек для выбора. 
        /// Если диалог успешно завершен, то результирующая точка будет в свойстве Result
        /// </summary>
        /// <param name="points">список  точек на выбор</param>
        /// <param name="withoutPoint">точка, исключаемая из списка</param>
        /// <param name="isCycledEnable">если истина, то будет активна точка кругового маршрута</param>
        /// <param name="labelText">текст надписи (какую точку выбирать)</param>
        public FormSelectPoint(TrackFile points, TrackPoint withoutPoint, bool isCycledEnable, string labelText)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentNullException("Список точек пуст или не существует");

            this.AllPoints = points.Clone();
            InitializeComponent();
            AllPoints.Remove(withoutPoint);
            FillList(AllPoints);

            listBoxPoints.SelectedIndex = 0;
            buttonCycled.Enabled = isCycledEnable;
            label1.Text = labelText != null ? labelText : "Выберите точку";
        }

        #endregion

        #region события

        /// <summary>
        /// выбор точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelect_Click(object sender, EventArgs e)
        {
            if (listBoxPoints.SelectedIndex < 0)
                return;

            Result = points[listBoxPoints.SelectedIndex];

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        /// <summary>
        /// отмена действия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// круговой маршрут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCycled_Click(object sender, EventArgs e)
        {
            Result = null;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// поиск при изменении текста в строке поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            //если запрос пустой
            if (string.IsNullOrWhiteSpace(textBoxSearch.Text))
                FillList(AllPoints);

            //отсев нужных точек
            TrackFile selected = new TrackFile();
            foreach (TrackPoint tt in AllPoints)
                if (tt.Name.ToLower().Contains(textBoxSearch.Text.ToLower()))
                    selected.Add(tt);
            FillList(selected);
        }

        /// <summary>
        /// поиск по списку 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFind_Click(object sender, EventArgs e)
        {
            textBoxSearch_TextChanged(null, null);
        }

        #endregion

        #region вспомогательные методы


        /// <summary>
        /// заполнение списка указанными элементами
        /// </summary>
        /// <param name="points"></param>
        private void FillList(TrackFile points)
        {
            this.points = points;
            listBoxPoints.Items.Clear();

            points.Sort();

            foreach (TrackPoint tt in points)
                listBoxPoints.Items.Add(new ListItem() { point = tt });
        }

        #endregion

    }
}
