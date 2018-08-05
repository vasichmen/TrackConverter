using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Ext
{
    /// <summary>
    /// расширенный класс области карты  
    /// </summary>
    public class GMapControlExt : GMapControl
    {
        /// <summary>
        /// слой на карте для доп. слоя
        /// </summary>
        private GMapOverlay layersOverlay;

        /// <summary>
        /// загруженные объекты
        /// </summary>
        private Dictionary<int, VectorMapLayerObject> LayerObjects = new Dictionary<int, VectorMapLayerObject>();

        /// <summary>
        /// список объектов, выделенных на карте
        /// </summary>
        private List<int> SelectedPolygons = new List<int>();

        /// <summary>
        /// загруженные области с объектами
        /// </summary>
        private ConcurrentBag<RectLatLng> loadedAreas;

        /// <summary>
        /// ID слоя с объектами
        /// </summary>
        private string layersOverlayID = "layersOverlay";

        /// <summary>
        /// цвет границы объекта
        /// </summary>
        private Pen polygonStroke = Pens.Gray;

        /// <summary>
        /// цвет границы невидимого объекта
        /// </summary>
        private Pen invisPolygonStroke = Pens.Transparent;

        /// <summary>
        /// заливка объекта
        /// </summary>
        private Brush polygonBrush = Brushes.Transparent;

        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        private Brush selectedPolygonBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        /// <summary>
        /// всплывающие подсказки на объектах
        /// </summary>
        private ToolTip ToolTipPolygonTitles = new ToolTip();


        /// <summary>
        /// создаёт новый объект GMapControlExt
        /// </summary>
        public GMapControlExt() : base()
        {
            layersOverlay = new GMapOverlay(layersOverlayID);
            loadedAreas = new ConcurrentBag<RectLatLng>();
            this.Overlays.Add(layersOverlay);

            //добавляем событие обновления слоёв карты
            this.MouseUp += GMapControlExt_MouseUp;
            this.OnMapZoomChanged += GMapControlExt_OnMapZoomChanged;
            this.OnPolygonEnter += GMapControlExt_OnPolygonEnter;
        }

        /// <summary>
        /// вывод подсказки над картой при попадании курсора на объект
        /// </summary>
        /// <param name="item"></param>
        private void GMapControlExt_OnPolygonEnter(GMapPolygon item)
        {
            List<VectorMapLayerObject> objects = GetVectorObjectsUnderCursor();
            string text = "";
            foreach (VectorMapLayerObject obj in objects)
                text += obj.Name + "\r\n";

            ToolTipPolygonTitles.AutomaticDelay = 50;
            ToolTipPolygonTitles.AutoPopDelay = 10000;
            ToolTipPolygonTitles.InitialDelay = 50;
            ToolTipPolygonTitles.SetToolTip(this, text);
        }

        /// <summary>
        /// перерисовка слоёв  при изменении масштаба карты
        /// </summary>
        private void GMapControlExt_OnMapZoomChanged()
        {
            if (LayerProvider != VectorMapLayerProviders.None)
            {
                ClearLayers();
                RefreshLayers();
            }
        }

        /// <summary>
        /// событие обновления слоёв карты после перемещения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GMapControlExt_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsDragging)
                RefreshLayers();
        }

        /// <summary>
        /// работа с поставщиком слоя
        /// </summary>
        private VectorMapLayer layerProviderEngine;

        /// <summary>
        /// текущий поставщик слоёв для карты
        /// </summary>
        private VectorMapLayerProviders layerProvider;

        /// <summary>
        /// Поставщик слоя для карты
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VectorMapLayerProviders LayerProvider
        {
            get { return layerProvider; }
            set
            {
                layerProvider = value;
                ClearLayers();
                if (value != VectorMapLayerProviders.None)
                {
                    layerProviderEngine = new VectorMapLayer(value);
                    RefreshLayers();
                }
            }
        }

        /// <summary>
        /// обновление слоёв на карте (добавление незагруженных областей)
        /// </summary>
        private void RefreshLayers()
        {
            //если это первый вызов из конструктора или слой None, то надо выйти
            if (Program.winMain == null || this.LayerProvider == VectorMapLayerProviders.None)
                return;

            double resolution = this.MapProvider.Projection.GetGroundResolution((int)Zoom, Position.Lat); //сколько метров в 1 пикселе при таком масштабе (для выбора периметра объекта)
            double min_pixel_perimeter = 40; //минимальный периметр в пикселях для вывода на экран
            double minPerimeter = resolution * min_pixel_perimeter; //минимальный периметр в метрах
            List<VectorMapLayerObject> objects = new List<VectorMapLayerObject>();
            try
            {
                //загрузка слоя в новом потоке
                Task ts = new Task(() =>
                {
                    Program.winMain.BeginOperation();

                    //начальные пеераметры отображения
                    VectorMapLayerProviders provider = this.LayerProvider;
                    int zoom = (int)Zoom;
                    PointLatLng position = this.Position;

                    //области, которые необходимо загрузить
                    List<RectLatLng> tiles = this.GetVisibleTiles();

                    int i = 0;
                    Parallel.ForEach(tiles, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (tile) =>
                       {
                           try
                           {
                               string prc = (((double)i / tiles.Count) * 100d).ToString("0");
                               Program.winMain.setCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено " + prc + "%");
                               List<VectorMapLayerObject> nobj = this.layerProviderEngine.GetObjects(tile, minPerimeter, (int)Zoom);
                               loadedAreas.Add(tile);
                               this.Invoke(new Action(() =>
                               {
                                   foreach (VectorMapLayerObject obj in nobj)
                                   {
                                       //если во время выполнения операции изменились параметры отображения, то выходим
                                       if (zoom != (int)Zoom || this.LayerProvider != provider || position != this.Position)
                                           return;
                                       else
                                           ShowLayerObject(obj);
                                   }
                                   this.Refresh();
                               }));
                               i++;
                           }
                           catch (Exception e) //ловим ошибки при отсутствии интернета
                           { }
                       });
                    Program.winMain.EndOperation();
                });
                ts.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(Program.winMain, "Не удалось загрузить слои! Причина:\r\n" + e.Message, "Загрузка слоя " + this.LayerProvider.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Program.winMain.layerProviderToolStripMenuItem.DropDownItems[0].PerformClick();
                Program.winMain.EndOperation();
                return;
            }

        }

        /// <summary>
        /// возвращает тайлы, которые сейчас выдны на экране. При этом исключаются области, загруженные ранее.
        /// </summary>
        /// <returns></returns>
        private List<RectLatLng> GetVisibleTiles()
        {
            List<RectLatLng> res = new List<RectLatLng>();
            RectLatLng area = GetViewArea();
            double step_vert = (area.Top - area.Bottom) / 3d;
            double step_hor = (area.Right - area.Left) / 3d;
            for (double i = area.Top; i > area.Bottom; i -= step_vert)
                for (double j = area.Right; j > area.Left; j -= step_hor)
                {
                    RectLatLng nar = new RectLatLng(i, j - step_hor, step_hor, step_vert);
                    if (!isAreaLoaded(nar))
                        res.Add(nar);
                }
            return res;
        }

        /// <summary>
        /// возвращает истину, если заданная область nar полностью покрывается областями из списка this.loadedAreas
        /// </summary>
        /// <param name="nar">проверяемая область</param>
        /// <returns></returns>
        private bool isAreaLoaded(RectLatLng nar)
        {
            return false;
            //TODO: сделать проверку попададния в загруженную область 
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить область координат, видимых на экране
        /// </summary>
        /// <returns></returns>
        private RectLatLng GetViewArea()
        {
            //вычисление координат выдимой части карты
            PureProjection proj = this.MapProvider.Projection;
            GPoint center = proj.FromLatLngToPixel(Position, (int)Zoom);

            long lt_y = (long)(center.Y - this.Height / 2);
            long lt_x = (long)(center.X - this.Width / 2);
            PointLatLng lt = proj.FromPixelToLatLng(new GPoint(lt_x, lt_y), (int)Zoom);

            long rb_y = (long)(center.Y + this.Height / 2);
            long rb_x = (long)(center.X + this.Width / 2);
            PointLatLng rb = proj.FromPixelToLatLng(new GPoint(rb_x, rb_y), (int)Zoom);

            double deg_height = lt.Lat - rb.Lat;
            double deg_width = rb.Lng - lt.Lng;

            if (deg_height < 0 || deg_width < 0)
                throw new Exception("Размеры области меньше нуля!");

            RectLatLng area = new RectLatLng(lt.Lat, lt.Lng, deg_width, deg_height);
            return area;
        }

        /// <summary>
        /// прорисовка объекта слоя на карте
        /// </summary>
        /// <param name="obj"></param>
        private void ShowLayerObject(VectorMapLayerObject obj)
        {
            if (LayerObjects.ContainsKey(obj.ID))
                return;

            bool selected = SelectedPolygons.Contains(obj.ID);

            obj.Geometry.Fill = selected ? selectedPolygonBrush : polygonBrush;
            obj.Geometry.Stroke = obj.Invisible ? invisPolygonStroke : polygonStroke;
            obj.Geometry.Tag = obj;
            obj.Geometry.IsHitTestVisible = true;
            layersOverlay.Polygons.Add(obj.Geometry);
            LayerObjects.Add(obj.ID, obj);
        }

        /// <summary>
        /// возвращает список объектов слоя , на которых находится курсор
        /// </summary>
        /// <returns></returns>
        public List<VectorMapLayerObject> GetVectorObjectsUnderCursor()
        {
            List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();
            foreach (GMapPolygon pol in this.layersOverlay.Polygons)
                if (pol.IsMouseOver)
                    res.Add(pol.Tag as VectorMapLayerObject);
            return res;
        }

        /// <summary>
        /// выделение заданного полигона на карте (при показе доп. информации об объекте)
        /// </summary>
        /// <param name="geometry"></param>
        internal void SelectPolygon(GMapPolygon geometry)
        {
            geometry.Fill = this.selectedPolygonBrush;
            this.Refresh();
            this.SelectedPolygons.Add((geometry.Tag as VectorMapLayerObject).ID);
        }

        /// <summary>
        /// снятие выделение с объекта
        /// </summary>
        /// <param name="geometry"></param>
        internal void DisSelectPolygon(GMapPolygon geometry)
        {
            geometry.Fill = Brushes.Transparent;
            this.SelectedPolygons.Remove((geometry.Tag as VectorMapLayerObject).ID);
            this.Refresh();
        }

        /// <summary>
        /// очичтить слои карты
        /// </summary>
        private void ClearLayers()
        {
            layersOverlay.Polygons.Clear();
            LayerObjects.Clear();
            loadedAreas = new ConcurrentBag<RectLatLng>(); //очистка коллекции загруженных областей
            this.Refresh();
        }
    }
}
