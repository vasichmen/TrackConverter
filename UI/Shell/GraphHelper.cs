using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using TrackConverter.Lib;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using ZedGraph;

namespace TrackConverter.UI.Shell
{
    class GraphHelper
    {
        private FormMain formMain;

        public GraphHelper(FormMain formMain)
        {
            this.formMain = formMain;
        }
      
        
        #region вспомогательные методы

        /// <summary>
        /// обновление графика
        /// </summary>
        public void ConfigureGraph()
        {
            GraphPane gp = formMain.zedGraph.GraphPane;
            gp.Title.IsVisible = false;
            gp.XAxis.Title.Text = "Расстояние от старта в " + (Vars.Options.Graphs.isXKm ? "километрах" : "метрах");
            gp.YAxis.Title.Text = "Высота над у. м. в " + (Vars.Options.Graphs.isYKm ? "километрах" : "метрах");
            gp.CurveList.Clear();
            formMain.mainCurves = new List<LineItem>();

            //ДОБАВЛЕНИЕ ТРЕКОВ ИЗ СПИСКА
            if (formMain.tracks != null)
            {
                Parallel.ForEach(formMain.tracks, new Action<BaseTrack>((tf) =>
                {
                    PointPairList list = new PointPairList();
                    string name = tf.Name;
                    foreach (TrackPoint tt in tf)
                    {
                        //значения в километрах
                        double x = tt.StartDistance * 1000.000;
                        double y = tt.MetrAltitude;

                        //если настройки требуют, то перевод в метры
                        if (Vars.Options.Graphs.isXKm)
                            x /= 1000.000;
                        if (Vars.Options.Graphs.isYKm)
                            y /= 1000.000;

                        //добавление пары
                        list.Add(x, y);
                    }

                    LineItem curve = new LineItem(name, list, tf.Color, SymbolType.None);
                    curve.Line.IsSmooth = true;
                    formMain.mainCurves.Add(curve);
                    gp.CurveList.Add(curve);
                }));
            }

            //ДОБАВЛЕНИЕ ВЫДЕЛЕННОГО ТРЕКА
            //if (this == Program.winElevVisual) // если это окно - часть главного окна, то проверяем выделенный маршрут
                if (Vars.currentSelectedTrack != null)
                {
                    PointPairList list = new PointPairList();
                    BaseTrack selTrack = Vars.currentSelectedTrack;
                    string name = selTrack.Name;
                    foreach (TrackPoint tt in selTrack)
                    {
                        //значения в километрах
                        double x = tt.StartDistance * 1000.000;
                        double y = tt.MetrAltitude;

                        //если настройки требуют, то перевод в метры
                        if (Vars.Options.Graphs.isXKm)
                            x /= 1000.000;
                        if (Vars.Options.Graphs.isYKm)
                            y /= 1000.000;

                        //добавление пары
                        list.Add(x, y);
                    }

                formMain.curveSelectedTrack = new LineItem(name, list, selTrack.Color, SymbolType.None);
                formMain.curveSelectedTrack.Line.IsSmooth = true;
                formMain.curveSelectedTrack.Tag = "selectedTrack";

                formMain.mainCurves.Add(formMain.curveSelectedTrack);
                    gp.CurveList.Add(formMain.curveSelectedTrack);
                }
                else Clear();

            formMain.zedGraph.AxisChange();
            formMain.zedGraph.Invalidate();
        }

        /// <summary>
        /// удаляет все элементы графа (выделенные точки), не являющиеся линиями высот
        /// </summary>
        private void RemLastSelPoint()
        {
            GraphPane pane = formMain.zedGraph.GraphPane;

            //удаление предыдущей выделенной точки
            foreach (CurveItem ci in pane.CurveList)
                if (!formMain.mainCurves.Contains(ci))
                {
                    pane.CurveList.Remove(ci);
                    break;
                }
        }

        /// <summary>
        /// очистка графиков
        /// </summary>
        internal void Clear()
        {
            if (formMain.tracks != null)
                formMain.tracks.Clear();
            formMain.zedGraph.GraphPane.CurveList.Clear();
            formMain.zedGraph.AxisChange();
            formMain.zedGraph.Invalidate();
        }


        #region взаимодействие


        /// <summary>
        /// добавление нового трека в график
        /// </summary>
        /// <param name="track"></param>
        public void AddTrack(TrackFile track)
        {
            if (formMain.tracks == null) formMain.tracks = new TrackFileList();
            formMain.tracks.Add(track);
            ConfigureGraph();
        }

        /// <summary>
        /// удаление трека из графика
        /// </summary>
        /// <param name="track"></param>
        public void RemoveTrack(TrackFile track)
        {
            formMain.tracks.Remove(track);
            ConfigureGraph();
        }

        /// <summary>
        /// добавление нескольких треков
        /// </summary>
        /// <param name="tfl"></param>
        public void AddTrack(TrackFileList tfl)
        {
            foreach (TrackFile tf in tfl)
                this.AddTrack(tf);
        }

        /// <summary>
        /// обновление вывода выделенного трека
        /// </summary>
        public void RefreshData()
        {
            ConfigureGraph();
        }

        /// <summary>
        /// выделение точки на графике
        /// </summary>
        /// <param name="tt"></param>
        public void SelectPoint(TrackPoint tt)
        {
            RemLastSelPoint();

            GraphPane pane = formMain.zedGraph.GraphPane;

            //значения в километрах
            double x = tt.StartDistance * 1000.000;
            double y = tt.MetrAltitude;

            //если настройки требуют, то перевод в метры
            if (Vars.Options.Graphs.isXKm)
                x /= 1000.000;
            if (Vars.Options.Graphs.isYKm)
                y /= 1000.000;


            // Кривая, состоящая из одной точки. Точка будет отмечена красным кругом
            LineItem curvePount = pane.AddCurve("",
                new double[] { x },
                new double[] { y },
                Color.Red,
                SymbolType.Circle);

            // 
            curvePount.Line.IsVisible = false;

            // Цвет заполнения круга - колубой
            curvePount.Symbol.Fill.Color = Color.Red;

            // Тип заполнения - сплошная заливка
            curvePount.Symbol.Fill.Type = FillType.Solid;

            // Размер круга
            curvePount.Symbol.Size = 7;

            formMain.zedGraph.Invalidate();

        }

        #endregion


        #endregion

        #region события графика


        internal void MouseLeave(object sender, EventArgs e)
        {
            RemLastSelPoint(); //очистка графика
            formMain.mapHelper.DeselectPoints(); //очитска карты
        }

        internal bool MoseMoveEvent(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
        {
            //если  нажата кнопка Ctrl, то выход
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return false;

            // Сюда будет сохранена кривая, рядом с которой был произведен клик
            CurveItem curve;

            // Сюда будет сохранен номер точки кривой, ближайшей к точке клика
            int index;

            GraphPane pane = formMain.zedGraph.GraphPane;

            // Максимальное расстояние от точки клика до кривой в пикселях, 
            // при котором еще считается, что клик попал в окрестность кривой.
            GraphPane.Default.NearestTol = 30;

            bool result = pane.FindNearestPoint(e.Location, out curve, out index);

            if (result && (string)curve.Tag == "selectedTrack")
            {
                #region выделение точки на графике

                //очистка графика от предыдущих точек
                RemLastSelPoint();

                // Максимально расстояние от точки клика до кривой не превысило NearestTol
                // Добавим точку на график, вблизи которой произошел клик

                // Кривая, состоящая из одной точки. Точка будет отмечена красным кругом
                LineItem curvePount = pane.AddCurve("",
                    new double[] { curve[index].X },
                    new double[] { curve[index].Y },
                    Color.Red,
                    SymbolType.Circle);

                // 
                curvePount.Line.IsVisible = false;

                // Цвет заполнения круга - колубой
                curvePount.Symbol.Fill.Color = Color.Red;

                // Тип заполнения - сплошная заливка
                curvePount.Symbol.Fill.Type = FillType.Solid;

                // Размер круга
                curvePount.Symbol.Size = 7;

                formMain.zedGraph.Invalidate();

                #endregion

                #region выделение точки на карте

                int curveIndex = formMain.zedGraph.GraphPane.CurveList.IndexOf(curve);
                int pointIndex = index;
                BaseTrack tf = Vars.currentSelectedTrack;
                TrackPoint pt = tf[pointIndex];

                //if (Program.winMap.tracks.Contains(tf)) //если идет выбор точки из всех треков

                formMain.mapHelper.SelectPoint(pt);

                #endregion

                #region выделение точки в списке точек


                formMain.pointsHelper.SelectPoint(pt);

                #endregion


            }
            return false;
        }

        /// <summary>
        /// вывод всплывающих подсказок при наведении на точку
        /// </summary>
        /// <param name="sender">Отправитель сообщения</param>
        /// <param name="pane">Панель для рисования</param>
        /// <param name="curve">Кривая, около которой находится курсор</param>
        /// <param name="iPt">Номер точки в кривой</param>
        /// <returns>Нужно вернуть отображаемую строку</returns>
        internal string PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];
            // Сформируем строку
            string result = string.Format("Высота: {0:F1} м\nРасстояние: {1:F1} км", point.Y, point.X);
            return result;
        }

        #endregion

    }
}
