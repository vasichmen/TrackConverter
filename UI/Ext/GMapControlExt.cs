using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
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
        GMapOverlay layersOverlay;
        Dictionary<string, VectorMapLayerObject> LayerObjects = new Dictionary<string, VectorMapLayerObject>();
        string layersOverlayID = "layersOverlay";
        Pen polygonStroke = Pens.Black;
        Brush polygonBrush = Brushes.Transparent;
        ToolTip ToolTipPolygonTitles = new ToolTip();

        /// <summary>
        /// создаёт новый объект GMapControlExt
        /// </summary>
        public GMapControlExt() : base()
        {
            layersOverlay = new GMapOverlay(layersOverlayID);

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
                text += obj.Name+"\r\n";

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
                if (value != VectorMapLayerProviders.None)
                {
                    layerProviderEngine = new VectorMapLayer(value);
                    RefreshLayers();
                }
                else
                    ClearLayers();
            }
        }

        /// <summary>
        /// обновление слоёв на карте (полная перерисовка)
        /// </summary>
        private void RefreshLayers()
        {
            //если это первый вызов из конструктора или слой None, то надо выйти
            if (Program.winMain == null || this.LayerProvider == VectorMapLayerProviders.None)
                return;

            RectLatLng area = GetViewArea(); //область координат на экране

            double resolution = this.MapProvider.Projection.GetGroundResolution((int)Zoom, Position.Lat); //сколько метров в 1 пикселе при таком масштабе (для выбора периметра объекта)
            double min_pixel_perimeter = 40; //минимальный периметр в пикселях для вывода на экран
            double minPerimeter = resolution * min_pixel_perimeter; //минимальный периметр в метрах
            List<VectorMapLayerObject> objects = new List<VectorMapLayerObject>();
            try
            {
                //загрузка слоя в новом потоке
                Task ts = new Task(() =>
                {
                    VectorMapLayerProviders provider = this.LayerProvider;
                    int zoom = (int)Zoom;
                    List<RectLatLng> tiles = this.GetVisibleTiles();
                    foreach (RectLatLng tile in tiles)
                    {
                        List<VectorMapLayerObject> nobj = this.layerProviderEngine.GetObjects(tile, minPerimeter, (int)Zoom);
                        this.Invoke(new Action(() =>
                        {
                            foreach (VectorMapLayerObject obj in nobj)
                            {
                                if (zoom != (int)Zoom || this.LayerProvider != provider) //если во время выполнения операции изменились настройки, то выходим
                                        return;
                                ShowLayerObject(obj);
                            }
                            this.Refresh();
                        }));

                    }
                });
                ts.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(Program.winMain, "Не удалось загрузить слои! Причина:\r\n" + e.Message, "Загрузка слоя " + this.LayerProvider.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                Program.winMain.layerProviderToolStripMenuItem.DropDownItems[0].PerformClick();
                return;
            }

        }

        /// <summary>
        /// возвращает тайлы, которые сейчас выдны на экране
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
                    res.Add(nar);
                }
            return res;
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
            if (LayerObjects.ContainsKey(obj.hash))
                return;

            obj.Geometry.Fill = polygonBrush;
            obj.Geometry.Stroke = polygonStroke;
            obj.Geometry.Tag = obj;
            obj.Geometry.IsHitTestVisible = true;
            obj.Geometry.Tag = obj;
            layersOverlay.Polygons.Add(obj.Geometry);
            LayerObjects.Add(obj.hash, obj);
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
        /// очичтить слои карты
        /// </summary>
        private void ClearLayers()
        {
            layersOverlay.Polygons.Clear();
            LayerObjects.Clear();
            this.Refresh();
        }
    }
}
