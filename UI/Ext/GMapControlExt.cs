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
using TrackConverter.Lib.Mathematic.Geodesy.Projections.GMapImported;

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
        /// загруженные объекты (для проверки существования этого объекта на экране)
        /// </summary>
        private Dictionary<int, VectorMapLayerObject> LayerObjects = new Dictionary<int, VectorMapLayerObject>();

        /// <summary>
        /// список объектов, выделенных на карте
        /// </summary>
        private List<int> SelectedPolygons = new List<int>();

        /// <summary>
        /// загруженные области с векторными объектами
        /// </summary>
        private ConcurrentBag<PointLatLng> loadedVectorAreas;

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
        /// работа с поставщиком векторного слоя
        /// </summary>
        private VectorMapLayer vectorLayerProviderEngine;

        /// <summary>
        /// графический слой растровых слоёв
        /// </summary>
        private Graphics rastrLayersGraphics = null;

        /// <summary>
        /// работа с поставщиком растрового слоя
        /// </summary>
        private RastrMapLayer rastrLayerProviderEngine;

        /// <summary>
        /// если истина, то вся первичная настройка завершена
        /// </summary>
        public bool isControlLoaded = false;

        /// <summary>
        /// если истина, то все тайлы карты загружены
        /// </summary>
        private bool isTilesLoaded = false;

        /// <summary>
        /// создаёт новый объект GMapControlExt
        /// </summary>
        public GMapControlExt() : base()
        {
            layersOverlay = new GMapOverlay(layersOverlayID);
            loadedVectorAreas = new ConcurrentBag<PointLatLng>();
            this.Overlays.Add(layersOverlay);

            //добавляем событие обновления слоёв карты
            this.MouseUp += GMapControlExt_MouseUp;
            this.OnMapZoomChanged += GMapControlExt_OnMapZoomChanged;
            this.OnPolygonEnter += GMapControlExt_OnPolygonEnter;
            this.SizeChanged += GMapControlExt_SizeChanged;
            this.OnTileLoadComplete += GMapControlExt_OnTileLoadComplete;
            this.OnTileLoadStart += GMapControlExt_OnTileLoadStart;
        }

        private void GMapControlExt_OnTileLoadStart()
        {
            isTilesLoaded = false;
        }

        private void GMapControlExt_OnTileLoadComplete(long ElapsedMilliseconds)
        {
            isTilesLoaded = true;
        }

        /// <summary>
        /// обновление слоёв при изменении размеров окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GMapControlExt_SizeChanged(object sender, EventArgs e)
        {
            RefreshLayers();
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
            if (LayerProvider != MapLayerProviders.None)
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
        /// текущий поставщик слоёв для карты
        /// </summary>
        private MapLayerProviders layerProvider;

        /// <summary>
        /// Поставщик слоя для карты
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MapLayerProviders LayerProvider
        {
            get { return layerProvider; }
            set
            {
                layerProvider = value;
                ClearLayers();
                if (value != MapLayerProviders.None)
                {
                    switch (value)
                    {
                        case MapLayerProviders.Wikimapia:
                            vectorLayerProviderEngine = new VectorMapLayer(value);
                            break;
                        case MapLayerProviders.YandexTraffic:
                            rastrLayerProviderEngine = new RastrMapLayer(value);
                            break;
                    }
                }
                RefreshLayers();
            }
        }

        /// <summary>
        /// обновление слоёв на карте (добавление незагруженных областей)
        /// </summary>
        public void RefreshLayers()
        {
            //если это первый вызов из конструктора или слой None, то надо выйти
            if (Program.winMain == null || this.LayerProvider == MapLayerProviders.None)
                return;

            //если ещё не завершена загрузка настроек контрола, то выход
            if (!isControlLoaded)
                return;

            //если нет слоя рисования растровых слоёв, то получаем его
            if (rastrLayersGraphics == null)
            {
                rastrLayersGraphics = CreateGraphics();
            }

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
                    Program.winMain.setCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено 0%");

                    //начальные параметры отображения
                    MapLayerProviders provider = this.LayerProvider;
                    int zoom = (int)Zoom;
                    PointLatLng position = this.Position;
                    int height = this.Height;
                    int width = this.Width;

                    switch (this.LayerProvider)
                    {
                        case MapLayerProviders.Wikimapia:
                            #region ДЛЯ ВЕКТОРНЫХ СЛОЁВ

                            //области, которые необходимо загрузить
                            List<RectLatLng> tiles = this.GetVisibleLatLngTiles();

                            int i = 0; //количество загруженных тайлов
                            Parallel.ForEach(tiles, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, (tile) =>
                               {
                                   try
                                   {
                                       List<VectorMapLayerObject> nobj = this.vectorLayerProviderEngine.GetObjects(tile, minPerimeter, (int)Zoom);
                                       loadedVectorAreas.Add(tile.LocationMiddle);
                                       if (!this.IsDisposed)
                                       {
                                           //если во время выполнения операции изменились параметры отображения, то выходим
                                           if (zoom != (int)Zoom || //масштаб карты
                                                    this.LayerProvider != provider || //источник данных
                                                    position != this.Position || //центр карты
                                                    height != this.Height || //высота элемента
                                                    width != this.Width) //ширина элемента
                                               return;
                                           ShowVectorLayerObjects(nobj);
                                       }
                                       string prc = (((double)i++ / tiles.Count) * 100d).ToString("0");
                                       Program.winMain.setCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено " + prc + "%");


                                   }
                                   catch (Exception e) //ловим ошибки при отсутствии интернета
                                   { }
                               });
                            break;
                        #endregion

                        case MapLayerProviders.YandexTraffic:

                            #region ДЛЯ РАСТРОВЫХ СЛОЁВ

                            //области, которые необходимо загрузить
                            List<GPoint> tilesR = this.GetVisiblePixelTiles();

                            int iR = 0; //количество загруженных тайлов
                            Parallel.ForEach(tilesR, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (tile) =>
                            {
                                try
                                {
                                    Image tl = rastrLayerProviderEngine.GetRastrTile(tile.X, tile.Y, (int)Zoom);
                                    ShowRastrLayerTile(tl, tile);
                                    string prc = (((double)iR++ / tilesR.Count) * 100d).ToString("0");
                                    Program.winMain.setCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено " + prc + "%");
                                }
                                catch (Exception e) //ловим ошибки при отсутствии интернета
                                { }
                            });
                            break;
                        #endregion

                        default: throw new Exception("Нет обработчика этого поставщика слоя");

                    }


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
        /// возвращает координаты верхних левых углов тайлов, которые сейчас виды на экране
        /// </summary>
        /// <returns></returns>
        private List<GPoint> GetVisiblePixelTiles()
        {
            int zoom = (int)Zoom;
            RectLatLng viewArea = GetViewArea();
            PureProjection proj = this.MapProvider.Projection;

            GPoint lt_pix = proj.FromLatLngToPixel(viewArea.LocationTopLeft, zoom);
            GPoint rb_pix = proj.FromLatLngToPixel(viewArea.LocationRightBottom, zoom);

            long x_from = ((int)(lt_pix.X / 256)); //верхние границы с округлнием в меньшую сторону
            long y_from = ((int)(lt_pix.Y / 256));
            long x_step = 1;
            long y_step = 1;
            long y_to = (long)Math.Ceiling(rb_pix.Y / 256d); //нижние границы с округлением в большую сторону
            long x_to = (long)Math.Ceiling(rb_pix.X / 256d);

            List<GPoint> result = new List<GPoint>();
            for (long y = y_from + y_step; y <= y_to; y += y_step)
                for (long x = x_from; x < x_to; x += x_step)
                {
                    GPoint rll = new GPoint(x, y);
                    //if (!isAreaLoaded(rll))
                    result.Add(rll);
                }

            return result;
        }

        /// <summary>
        /// возвращает области-тайлы, которые сейчас выдны на экране. При этом исключаются области, загруженные ранее.
        /// </summary>
        /// <returns></returns>
        private List<RectLatLng> GetVisibleLatLngTiles()
        {
            //тайловый способ
            //https://tech.yandex.ru/maps/doc/jsapi/2.1/theory/index-docpage/#tile_coordinates
            int zoom = (int)Zoom;
            RectLatLng viewArea = GetViewArea();
            PureProjection proj = this.MapProvider.Projection;

            GPoint lt_pix = proj.FromLatLngToPixel(viewArea.LocationTopLeft, zoom);
            GPoint rb_pix = proj.FromLatLngToPixel(viewArea.LocationRightBottom, zoom);

            GSize tile_size = proj.TileSize;

            long x_from = ((int)(lt_pix.X / 256)) * 256; //верхние границы с округлнием в меньшую сторону
            long y_from = ((int)(lt_pix.Y / 256)) * 256;
            long x_step = tile_size.Width;
            long y_step = tile_size.Height;
            long y_to = (long)Math.Ceiling(rb_pix.Y / 256d) * 256; //нижние границы с округлением в большую сторону
            long x_to = (long)Math.Ceiling(rb_pix.X / 256d) * 256;


            List<RectLatLng> result = new List<RectLatLng>();
            for (long y = y_from + y_step; y <= y_to; y += y_step)
                for (long x = x_from; x < x_to; x += x_step)
                {
                    PointLatLng lt = proj.FromPixelToLatLng(x, y, zoom);
                    PointLatLng rb = proj.FromPixelToLatLng(x + x_step, y + y_step, zoom);

                    SizeLatLng sz = new SizeLatLng(rb.Lat - lt.Lat, rb.Lng - lt.Lng);

                    RectLatLng rll = new RectLatLng(lt, sz);
                    if (!isVectorAreaLoaded(rll))
                        result.Add(rll);
                }

            return result;

            ////деление экрана на части
            //List<RectLatLng> res = new List<RectLatLng>();
            //RectLatLng area = GetViewArea();
            //double step_vert = (area.Top - area.Bottom) / 5d;
            //double step_hor = (area.Right - area.Left) / 5d;
            //for (double i = area.Top; i > area.Bottom; i -= step_vert)
            //    for (double j = area.Right; j > area.Left; j -= step_hor)
            //    {
            //        RectLatLng nar = new RectLatLng(i, j - step_hor, step_hor, step_vert);
            //        if (!isAreaLoaded(nar))
            //            res.Add(nar);
            //    }
            //return res;
        }

        /// <summary>
        /// возвращает истину, если заданная область nar полностью покрывается областями из списка this.loadedAreas
        /// </summary>
        /// <param name="nar">проверяемая область</param>
        /// <returns></returns>
        private bool isVectorAreaLoaded(RectLatLng nar)
        {
            foreach (var area in loadedVectorAreas)
            {
                if (area == nar.LocationMiddle)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Получить область координат, видимых на экране
        /// </summary>
        /// <returns></returns>
        private RectLatLng GetViewArea(PureProjection proj = null)
        {
            //вычисление координат выдимой части карты
            if (proj == null)
                proj = this.MapProvider.Projection;
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
        /// прорисовка объекта слоя на карте (потокобезопасный)
        /// </summary>
        /// <param name="obj"></param>
        private void ShowVectorLayerObject(VectorMapLayerObject obj)
        {
            if (LayerObjects.ContainsKey(obj.ID))
                return;
            else
                LayerObjects.Add(obj.ID, obj);

            bool selected = SelectedPolygons.Contains(obj.ID);

            Action act = new Action(() =>
                {
                    if (LayerProvider == MapLayerProviders.None)
                        return;
                    obj.Geometry.Fill = selected ? selectedPolygonBrush : polygonBrush;
                    obj.Geometry.Stroke = obj.Invisible ? invisPolygonStroke : polygonStroke;
                    obj.Geometry.Tag = obj;
                    obj.Geometry.IsHitTestVisible = true;
                    layersOverlay.Polygons.Add(obj.Geometry);
                });

            if (!this.IsDisposed)
            {
                if (this.InvokeRequired)
                    this.Invoke(act);
                else
                    act.Invoke();
            }
        }

        /// <summary>
        /// быстрое добавление нескольких объектов
        /// </summary>
        /// <param name="list">коллекция объектов</param>
        private void ShowVectorLayerObjects(List<VectorMapLayerObject> list)
        {
            this.SuspendLayout();
            foreach (VectorMapLayerObject obj in list)
                ShowVectorLayerObject(obj);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// добавление на карту тайла растрового слоя 
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="position"></param>
        private void ShowRastrLayerTile(Image tile, GPoint position)
        {
            //ожидание завершения загрузки карты
            while (!isTilesLoaded)
                Thread.Sleep(30);


            //выбор проекции слоя зависит от поставщика
            PureProjection proj;
            switch (layerProvider)
            {
                case MapLayerProviders.YandexTraffic:
                    proj = new MercatorProjectionYandex();
                    break;
                case MapLayerProviders.Wikimapia:
                    return;
                default: throw new Exception("Для этого слоя не определена проекция карты!");
            }
            RectLatLng viewArea = GetViewArea(proj);
            GPoint va_lt_pix =proj.FromLatLngToPixel(viewArea.LocationTopLeft, (int)Zoom);


            long x = 0, y = 0;
            x = position.X * 256 - va_lt_pix.X;
            y = position.Y * 256 - va_lt_pix.Y;

            lock (rastrLayersGraphics)
            {
                rastrLayersGraphics.DrawImage(tile, (int)x, (int)y);
            }

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
        internal void SelectPolygon(int id)
        {
            foreach (GMapPolygon pol in this.layersOverlay.Polygons)
                if ((pol.Tag as VectorMapLayerObject).ID == id)
                {
                    this.SelectedPolygons.Add(id);
                    pol.Fill = this.selectedPolygonBrush;
                    this.Refresh();
                    break;
                }
        }

        /// <summary>
        /// снятие выделение с объекта
        /// </summary>
        /// <param name="geometry"></param>
        internal void DisSelectPolygon(int id)
        {
            this.SelectedPolygons.Remove(id);
            foreach (GMapPolygon pol in this.layersOverlay.Polygons)
                if ((pol.Tag as VectorMapLayerObject).ID == id)
                {
                    pol.Fill = this.polygonBrush;
                    this.Refresh();
                    break;
                }
        }

        /// <summary>
        /// очичтить слои карты
        /// </summary>
        private void ClearLayers()
        {
            layersOverlay.Polygons.Clear(); //очистка карты
            LayerObjects.Clear(); //удаляем объекты на экране
            loadedVectorAreas = new ConcurrentBag<PointLatLng>(); //очистка коллекции загруженных областей
            if (rastrLayersGraphics != null)
                rastrLayersGraphics.Clear(Color.Transparent); //очистка слоя рисования растровых слоёв
            this.Refresh();
        }
    }
}
