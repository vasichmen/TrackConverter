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

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно вывода профиля высот трека
    /// http://jenyay.net/Programming/ZedGraph
    /// </summary>
    public partial class FormElevVisual : Form
    {
        /// <summary>
        /// список выводящих треков, не включая выделенный трек
        /// </summary>
        private TrackFileList tracks;

        /// <summary>
        /// основные линии на графике, не включая линию выдленного трека
        /// </summary>
        private List<LineItem> mainCurves;
        
        /// <summary>
        /// создает новый объект FormElevVisual с заданным списком треков
        /// </summary>
        /// <param name="fl"></param>
        public FormElevVisual(TrackFileList fl)
        {
            tracks = fl;
            InitializeComponent();
            Task pr = new Task(new Action(() =>
            {
                Program.winMain.BeginOperation();
                Program.winMain.setCurrentOperation("Построение профиля...");
                ConfigureGraph();
                Program.winMain.EndOperation();
            }));
            pr.Start();
        }


        #region события 

        #region события zedGraph

        /// <summary>
        /// выделение точки , ближайшей к курсору и выделение этой точки на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool zedGraph_MouseMoveEvent(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
        {
            //если  нажата кнопка Ctrl, то выход
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return false;

            // Сюда будет сохранена кривая, рядом с которой был произведен клик
            CurveItem curve;

            // Сюда будет сохранен номер точки кривой, ближайшей к точке клика
            int index;

            GraphPane pane = zedGraph.GraphPane;

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

                zedGraph.Invalidate();

                #endregion

                #region выделение точки на карте

                int curveIndex = zedGraph.GraphPane.CurveList.IndexOf(curve);
                int pointIndex = index;
                BaseTrack tf = Vars.currentSelectedTrack;
                TrackPoint pt = tf[pointIndex];

                //if (Program.winMap.tracks.Contains(tf)) //если идет выбор точки из всех треков

                Program.winMain.mapHelper.SelectPoint(pt);

                #endregion

                #region выделение точки в списке точек


                Program.winMain.pointsHelper.SelectPoint(pt);

                #endregion


            }
            return false;
        }

        /// <summary>
        /// отмена выделения всех точек на карте и графике
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zedGraph_MouseLeave(object sender, EventArgs e)
        {
            RemLastSelPoint(); //очистка графика
            Program.winMain.mapHelper.DeselectPoints(); //очитска карты
        }

        /// <summary>
        /// вывод всплывающих подсказок при наведении на точку
        /// </summary>
        /// <param name="sender">Отправитель сообщения</param>
        /// <param name="pane">Панель для рисования</param>
        /// <param name="curve">Кривая, около которой находится курсор</param>
        /// <param name="iPt">Номер точки в кривой</param>
        /// <returns>Нужно вернуть отображаемую строку</returns>
        private string zedGraph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];
            // Сформируем строку
            string result = string.Format("Высота: {0:F1} м\nРасстояние: {1:F1} км", point.Y, point.X);
            return result;
        }

        #endregion

        #endregion

        #region взаимодействие


        /// <summary>
        /// добавление нового трека в график
        /// </summary>
        /// <param name="track"></param>
        public void AddTrack(TrackFile track)
        {
            if (tracks == null) tracks = new TrackFileList();
            tracks.Add(track);
            ConfigureGraph();
        }

        /// <summary>
        /// удаление трека из графика
        /// </summary>
        /// <param name="track"></param>
        public void RemoveTrack(TrackFile track)
        {
            tracks.Remove(track);
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

            GraphPane pane = zedGraph.GraphPane;

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

            zedGraph.Invalidate();

        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// обновление графика
        /// </summary>
        private void ConfigureGraph()
        {
            GraphPane gp = zedGraph.GraphPane;
            gp.Title.IsVisible = false;
            gp.XAxis.Title.Text = "Расстояние от старта в " + (Vars.Options.Graphs.isXKm ? "километрах" : "метрах");
            gp.YAxis.Title.Text = "Высота над у. м. в " + (Vars.Options.Graphs.isYKm ? "километрах" : "метрах");
            gp.CurveList.Clear();
            mainCurves = new List<LineItem>();

            //ДОБАВЛЕНИЕ ТРЕКОВ ИЗ СПИСКА
            if (tracks != null)
            {
                Parallel.ForEach(tracks, new Action<BaseTrack>((tf) =>
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
                    mainCurves.Add(curve);
                    gp.CurveList.Add(curve);
                }));
            }

            //ДОБАВЛЕНИЕ ВЫДЕЛЕННОГО ТРЕКА
            //if (this == Program.winElevVisual) // если это окно - часть главного окна, то проверяем выделенный маршрут
            //    if (Vars.currentSelectedTrack != null)
            //    {
            //        PointPairList list = new PointPairList();
            //        BaseTrack selTrack = Vars.currentSelectedTrack;
            //        string name = selTrack.Name;
            //        foreach (TrackPoint tt in selTrack)
            //        {
            //            //значения в километрах
            //            double x = tt.StartDistance * 1000.000;
            //            double y = tt.MetrAltitude;

            //            //если настройки требуют, то перевод в метры
            //            if (Vars.Options.Graphs.isXKm)
            //                x /= 1000.000;
            //            if (Vars.Options.Graphs.isYKm)
            //                y /= 1000.000;

            //            //добавление пары
            //            list.Add(x, y);
            //        }

            //        curveSelectedTrack = new LineItem(name, list, selTrack.Color, SymbolType.None);
            //        curveSelectedTrack.Line.IsSmooth = true;
            //        curveSelectedTrack.Tag = "selectedTrack";

            //        mainCurves.Add(curveSelectedTrack);
            //        gp.CurveList.Add(curveSelectedTrack);
            //    }
            //    else
                    Clear();

            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }

        /// <summary>
        /// удаляет все элементы графа (выделенные точки), не являющиеся линиями высот
        /// </summary>
        private void RemLastSelPoint()
        {
            GraphPane pane = zedGraph.GraphPane;

            //удаление предыдущей выделенной точки
            foreach (CurveItem ci in pane.CurveList)
                if (!mainCurves.Contains(ci))
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
            if (tracks != null)
                tracks.Clear();
            zedGraph.GraphPane.CurveList.Clear();
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }


        #endregion

    }
}
