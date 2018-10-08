using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.ProviderRecords;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Exceptions;
using TrackConverter.Lib.Mathematic.Geodesy.Projections.GMapImported;
using TrackConverter.UI.Map;

namespace TrackConverter.UI.Ext
{
    /// <summary>
    /// область карты с возможностью отображения слоёв
    /// </summary>
    public class GMapControlExt : GMapControl
    {
        #region поля

        /// <summary>
        /// если истина, то вся первичная настройка завершена
        /// </summary>
        public bool isControlLoaded = false;

        /// <summary>
        /// слой на карте для доп. слоя
        /// </summary>
        public GMapOverlay vectorLayersOverlay;

        /// <summary>
        /// загруженные объекты (для проверки существования этого объекта на экране)
        /// </summary>
        private Dictionary<int, VectorMapLayerObject> VectorLayerObjects = new Dictionary<int, VectorMapLayerObject>();

        /// <summary>
        /// список объектов, выделенных на карте
        /// </summary>
        private List<int> SelectedPolygons = new List<int>();


        /// <summary>
        /// загруженные области слоя карты (координаты в тайловых координатах)
        /// </summary>
        private ConcurrentDictionary<object, Image> loadedAreas;

        /// <summary>
        /// ID слоя с объектами
        /// </summary>
        private readonly string vectorLayersOverlayID = "vectorLayersOverlay";

        /// <summary>
        /// цвет границы объекта
        /// </summary>
        private  Pen polygonStroke = Pens.Gray;

        /// <summary>
        /// цвет границы невидимого объекта
        /// </summary>
        private readonly Pen invisPolygonStroke = Pens.Transparent;

        /// <summary>
        /// заливка объекта
        /// </summary>
        private readonly Brush polygonBrush = Brushes.Transparent;

        /// <summary>
        /// цвет выделенного объекта
        /// </summary>
        private readonly Brush selectedPolygonBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        /// <summary>
        /// всплывающие подсказки на объектах
        /// </summary>
        private ToolTip ToolTipPolygonTitles = new ToolTip() { AutomaticDelay = 50, AutoPopDelay = 10000, InitialDelay = 50 };

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
        /// блокировка объекта Graphics для рисования растровых слоёв
        /// </summary>
        private static readonly object lockGraphics = new object();

        /// <summary>
        /// если истина, то все тайлы карты загружены
        /// </summary>
        private bool isTilesLoaded = false;

        /// <summary>
        /// текущий поставщик слоёв для карты
        /// </summary>
        private MapLayerProviders layerProvider;

        #endregion

        #region cвойства

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
                clearLayers();

                if (value == MapLayerProviders.Wikimapia)
                {
                    if (Vars.Options.Map.IsFormWikimpiaToolbarShow)
                    {
                        if (Program.winWikimapiaToolbarNullOrDisposed)
                            Program.winWikimapiaToolbar = new FormWikimapiaToolbar(Program.winMain);
                        Program.winWikimapiaToolbar.Show(Program.winMain);
                    }
                }
                else
                {
                    if (!Program.winWikimapiaToolbarNullOrDisposed)
                        Program.winWikimapiaToolbar.Close();
                }

                if (value != MapLayerProviders.None)
                {
                    switch (value)
                    {
                        case MapLayerProviders.Wikimapia:
                            vectorLayerProviderEngine = new VectorMapLayer(value);
                            break;
                        case MapLayerProviders.YandexTraffic:
                        case MapLayerProviders.OSMGPSTracks:
                        case MapLayerProviders.OSMRailways:
                        case MapLayerProviders.OSMRoadSurface:
                        case MapLayerProviders.RosreestrCadaster:
                            rastrLayerProviderEngine = new RastrMapLayer(value);
                            break;
                    }

                }
                RefreshLayers();
            }
        }

        /// <summary>
        /// проекция поставщика текущего слоя 
        /// </summary>
        /// <exception cref="TrackConverterException">Если для слоя не определена проекция</exception>
        public PureProjection LayerProjection
        {
            get
            {

                //выбор проекции слоя зависит от поставщика
                PureProjection proj;
                switch (layerProvider)
                {
                    case MapLayerProviders.YandexTraffic:
                        proj = new MercatorProjectionYandex();
                        break;
                    case MapLayerProviders.OSMGPSTracks:
                    case MapLayerProviders.OSMRailways:
                    case MapLayerProviders.OSMRoadSurface:
                    case MapLayerProviders.RosreestrCadaster: //TODO: проверить проекцию (Spatial Reference: 102100  (3857) )
                        proj = new MercatorProjection();
                        break;
                    case MapLayerProviders.Wikimapia:
                        proj = new MercatorProjection();
                        break;
                    case MapLayerProviders.None:
                        return MapProvider.Projection;
                    default:
                        throw new TrackConverterException("Для этого слоя не определена проекция карты!");
                }
                return proj;
            }
        }

        #endregion

        /// <summary>
        /// создаёт новый объект GMapControlExt
        /// </summary>
        public GMapControlExt() : base()
        {
            vectorLayersOverlay = new GMapOverlay(vectorLayersOverlayID);
            loadedAreas = new ConcurrentDictionary<object, Image>();
            this.Overlays.Add(vectorLayersOverlay);

            //добавляем событие обновления слоёв карты
            this.MouseUp += gMapControlExt_MouseUp;
            this.OnMapZoomChanged += gMapControlExt_OnMapZoomChanged;
            this.OnPolygonEnter += gMapControlExt_OnPolygonEnter;
            this.OnPolygonLeave += gMapControlExt_OnPolygonLeave;
            this.SizeChanged += gMapControlExt_SizeChanged;
            this.OnTileLoadComplete += gMapControlExt_OnTileLoadComplete;
            this.OnTileLoadStart += gMapControlExt_OnTileLoadStart;
            this.Paint += gMapControlExt_Paint;
            this.OnMapTypeChanged += gMapControlExt_OnMapTypeChanged;
        }


        #region события карты 

        /// <summary>
        /// при перерисовке контрола надо перерисовывать астровые слои
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gMapControlExt_Paint(object sender, PaintEventArgs e)
        {
            lock (loadedAreas)
            {
                switch (LayerProvider)
                {
                    case MapLayerProviders.YandexTraffic:
                    case MapLayerProviders.OSMGPSTracks:
                    case MapLayerProviders.OSMRailways:
                    case MapLayerProviders.OSMRoadSurface:
                    case MapLayerProviders.RosreestrCadaster:
                        foreach (var kv in this.loadedAreas)
                        {
                            showRastrLayerTile(kv.Value, (GPoint)kv.Key, e.Graphics);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// изменение цвета контура объектов при смене карты
        /// </summary>
        /// <param name="type"></param>
        private void gMapControlExt_OnMapTypeChanged(GMapProvider type)
        {
            MapProviders provider = Vars.Options.Map.MapProvider.Enum;
            polygonStroke = new Pen(MapLayerProviderRecord.GetObjectBorderColor(provider));
            foreach (var obj in vectorLayersOverlay.Polygons)
                obj.Stroke = polygonStroke;
        }

        /// <summary>
        /// устанавливает флаг начала загрузки карты
        /// </summary>
        private void gMapControlExt_OnTileLoadStart()
        {
            isTilesLoaded = false;
        }

        /// <summary>
        /// устанавливает флаг окончания загрузки карты
        /// </summary>
        /// <param name="ElapsedMilliseconds"></param>
        private void gMapControlExt_OnTileLoadComplete(long ElapsedMilliseconds)
        {
            isTilesLoaded = true;
        }

        /// <summary>
        /// обновление слоёв при изменении размеров окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gMapControlExt_SizeChanged(object sender, EventArgs e)
        {
            RefreshLayers();
        }

        /// <summary>
        /// вывод подсказки над картой при попадании курсора на объект
        /// </summary>
        /// <param name="item"></param>
        private void gMapControlExt_OnPolygonEnter(GMapPolygon item)
        {
            List<VectorMapLayerObject> objects = GetVectorObjectsUnderCursor();
            refreshToolTipObjects(objects);

            //при попадании в объект все остальные под курсором прозрачные, а сам объект потом желтым
            //foreach (var obj in objects)
            //    obj.Geometry.Fill = Brushes.Transparent;
            //item.Fill = Brushes.Yellow;
        }

        /// <summary>
        /// при уходе курсора с объекта обновить подсказку
        /// </summary>
        /// <param name="item"></param>
        private void gMapControlExt_OnPolygonLeave(GMapPolygon item)
        {
            List<VectorMapLayerObject> objects = GetVectorObjectsUnderCursor();
            refreshToolTipObjects(objects);

            //при уходе с объекта сделать его прозрачным, а самый маленький под курсором( по периметру) - желтым
            //VectorMapLayerObject minOb = null;
            //double minPer = double.MaxValue;
            //foreach (var obj in objects)
            //{
            //    if (obj.Perimeter < minPer)
            //    {
            //        minPer = obj.Perimeter;
            //        minOb = obj;
            //    }
            //}
            //    item.Fill = Brushes.Transparent;
            //if (minOb != null)
            //    minOb.Geometry.Fill = Brushes.Yellow;
        }

        /// <summary>
        /// обновитьь всплывающую подсказку для объекта
        /// </summary>
        private void refreshToolTipObjects(List<VectorMapLayerObject> objects)
        {

            string text = "";
            foreach (VectorMapLayerObject obj in objects)
                text += obj.Name + "\r\n";
            ToolTipPolygonTitles.SetToolTip(this, text);
        }

        /// <summary>
        /// перерисовка слоёв  при изменении масштаба карты
        /// </summary>
        private void gMapControlExt_OnMapZoomChanged()
        {
            if (LayerProvider != MapLayerProviders.None)
            {
                clearLayers();
                RefreshLayers();
            }
        }

        /// <summary>
        /// событие обновления слоёв карты после перемещения 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gMapControlExt_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsDragging)
                RefreshLayers();
        }

        #endregion

        #region растровые слои



        /// <summary>
        /// добавление на карту тайла растрового слоя 
        /// </summary>
        /// <param name="tile">картинка - тайл</param>
        /// <param name="position">тайловые координаты</param>
        /// <param name="gr">если null, то используется площадь для рисования по умолчанию rastrLayersGraphics</param>
        private void showRastrLayerTile(Image tile, GPoint position, Graphics gr = null)
        {
            //ожидание завершения загрузки карты
            while (!isTilesLoaded)
                Thread.Sleep(30);

            RectLatLng viewArea = GetViewArea();
            //if (projection == null) projection = LayerProjection;

            GPoint va_lt_pix = LayerProjection.FromLatLngToPixel(viewArea.LocationTopLeft, (int)Zoom);


            long x = 0, y = 0;
            x = position.X * 256 - va_lt_pix.X;
            y = position.Y * 256 - va_lt_pix.Y;
            if (gr == null)
                gr = rastrLayersGraphics;
            lock (lockGraphics)
            {
                try
                {
                    gr.DrawImage(tile, (int)x, (int)y);
                }
                catch (Exception) { }
            }

        }


        #endregion

        #region векторные слои


        /// <summary>
        /// прорисовка объекта слоя на карте (потокобезопасный)
        /// </summary>
        /// <param name="obj"></param>
        private void showVectorLayerObject(VectorMapLayerObject obj)
        {
            if (VectorLayerObjects.ContainsKey(obj.ID))
                return;
            else
                VectorLayerObjects.Add(obj.ID, obj);

            bool selected = SelectedPolygons.Contains(obj.ID);

            if (LayerProvider == MapLayerProviders.None)
                return;
            obj.Geometry.Fill = selected ? selectedPolygonBrush : polygonBrush;
            obj.Geometry.Stroke = obj.Invisible ? invisPolygonStroke : polygonStroke;
            obj.Geometry.Tag = obj;
            obj.Geometry.IsHitTestVisible = true;
            Action act = new Action(() => { vectorLayersOverlay.Polygons.Add(obj.Geometry); });

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
        private void showVectorLayerObjects(List<VectorMapLayerObject> list)
        {
            foreach (VectorMapLayerObject obj in list)
                showVectorLayerObject(obj);
        }

        /// <summary>
        /// возвращает список объектов слоя , на которых находится курсор
        /// </summary>
        /// <returns></returns>
        public List<VectorMapLayerObject> GetVectorObjectsUnderCursor()
        {
            List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();
            foreach (GMapPolygon pol in this.vectorLayersOverlay.Polygons)
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
            foreach (GMapPolygon pol in this.vectorLayersOverlay.Polygons)
                if ((pol.Tag as VectorMapLayerObject).ID == id)
                {
                    this.SelectedPolygons.Add(id);
                    pol.Fill = this.selectedPolygonBrush;
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
            foreach (GMapPolygon pol in this.vectorLayersOverlay.Polygons)
                if ((pol.Tag as VectorMapLayerObject).ID == id)
                {
                    pol.Fill = this.polygonBrush;
                    break;
                }
            this.Refresh();
        }

        #endregion

        #region вспомогательные методы

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
            double min_pixel_perimeter = 25; //минимальный периметр в пикселях для вывода на экран
            double minPerimeter = resolution * min_pixel_perimeter; //минимальный периметр в метрах
            List<VectorMapLayerObject> objects = new List<VectorMapLayerObject>();
            try
            {
                //загрузка слоя в новом потоке
                Task ts = new Task(() =>
                {
                    Program.winMain.BeginOperation();
                    Program.winMain.SetCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено 0%");

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
                            List<RectLatLng> tiles = this.getVisibleLatLngTiles();

                            int i = 0; //количество загруженных тайлов
                            Parallel.ForEach(tiles, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, (tile) =>
                               {
                                   try
                                   {
                                       //если во время выполнения операции изменились параметры отображения, то выходим
                                       if (zoom != (int)Zoom || //масштаб карты
                                                this.LayerProvider != provider || //источник данных
                                                position != this.Position || //центр карты
                                                height != this.Height || //высота элемента
                                                width != this.Width) //ширина элемента
                                           return;

                                       List<VectorMapLayerObject> nobj = this.vectorLayerProviderEngine.GetObjects(tile, minPerimeter, (int)Zoom);
                                       if (!this.IsDisposed)
                                       {
                                           loadedAreas.TryAdd(tile.LocationMiddle, null);
                                           showVectorLayerObjects(nobj);
                                           string prc = (((double)i++ / tiles.Count) * 100d).ToString("0");
                                           Program.winMain.SetCurrentOperation("Загрузка слоёв " + provider.ToString() + ", завершено " + prc + "%");
                                       }
                                   }
                                   catch (Exception e) //ловим ошибки при отсутствии интернета
                                   { }
                               });
                            break;
                        #endregion

                        case MapLayerProviders.YandexTraffic:
                        case MapLayerProviders.OSMGPSTracks:
                        case MapLayerProviders.OSMRailways:
                        case MapLayerProviders.OSMRoadSurface:

                            #region ДЛЯ РАСТРОВЫХ СЛОЁВ ПО ТАЙЛОВЫМ КООРДИНАТАМ

                            //области, которые необходимо загрузить

                            List<GPoint> tilesR = this.GetVisiblePixelTiles(LayerProjection);

                            int iR = 0; //количество загруженных тайлов
                            Parallel.ForEach(tilesR, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (tile) =>
                            {
                                try
                                {
                                    //если во время выполнения операции изменились параметры отображения, то выходим
                                    if (zoom != (int)Zoom || //масштаб карты
                                             this.LayerProvider != provider || //источник данных
                                             position != this.Position || //центр карты
                                             height != this.Height || //высота элемента
                                             width != this.Width) //ширина элемента
                                        return;

                                    Image tl = rastrLayerProviderEngine.GetRastrTile(tile.X, tile.Y, (int)Zoom);
                                    if (!this.IsDisposed)
                                    {
                                        loadedAreas.TryAdd(tile, tl);
                                        showRastrLayerTile(tl, tile);
                                        string prc = (((double)iR++ / tilesR.Count) * 100d).ToString("0");
                                        Program.winMain.SetCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено " + prc + "%");
                                    }
                                }
                                catch (Exception e) //ловим ошибки при отсутствии интернета
                                { }
                            });
                            break;
                        #endregion

                        case MapLayerProviders.RosreestrCadaster:

                            #region ДЛЯ РАСТРОВЫХ СЛОЁВ ПО МЕТРИЧЕСКИМ КООРДИНАТАМ

                            //области, которые необходимо загрузить
                            List<RectLatLng> llTiles = this.getVisibleLatLngTiles();

                            int im = 0; //количество загруженных тайлов
                            Parallel.ForEach(llTiles, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (tile) =>
                            {
                                try
                                {
                                    //если во время выполнения операции изменились параметры отображения, то выходим
                                    if (zoom != (int)Zoom || //масштаб карты
                                             this.LayerProvider != provider || //источник данных
                                             position != this.Position || //центр карты
                                             height != this.Height || //высота элемента
                                             width != this.Width) //ширина элемента
                                        return;

                                    Image tl = rastrLayerProviderEngine.GetRastrTile(tile, LayerProjection);
                                    if (!this.IsDisposed)
                                    {
                                        GPoint tileXY = LayerProjection.FromPixelToTileXY(LayerProjection.FromLatLngToPixel(tile.LocationTopLeft, zoom));
                                        bool f = loadedAreas.TryAdd(tileXY, tl);
                                        showRastrLayerTile(tl, tileXY);
                                        string prc = (((double)im++ / llTiles.Count) * 100d).ToString("0");
                                        Program.winMain.SetCurrentOperation("Загрузка слоёв " + LayerProvider.ToString() + ", завершено " + prc + "%");
                                    }
                                }
                                catch (Exception e) //ловим ошибки при отсутствии интернета
                                { }
                            });
                            break;
                        #endregion

                        case MapLayerProviders.None:
                            return;

                        default:
                            throw new Exception("Нет обработчика этого поставщика слоя");

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
        /// Получить область координат, видимых на экране
        /// </summary>
        /// <returns></returns>
        public RectLatLng GetViewArea()
        {
            //вычисление координат выдимой части карты
            PureProjection proj = MapProvider.Projection;
            GPoint center = proj.FromLatLngToPixel(Position, (int)Zoom);

            long lt_y = center.Y - Height / 2;
            long lt_x = center.X - Width / 2;
            PointLatLng lt = proj.FromPixelToLatLng(new GPoint(lt_x, lt_y), (int)Zoom);

            long rb_y = center.Y + Height / 2;
            long rb_x = center.X + Width / 2;
            PointLatLng rb = proj.FromPixelToLatLng(new GPoint(rb_x, rb_y), (int)Zoom);

            double deg_height = lt.Lat - rb.Lat;
            double deg_width = rb.Lng - lt.Lng;

            if (deg_height < 0 || deg_width < 0)
                throw new Exception("Размеры области меньше нуля!");

            RectLatLng area = new RectLatLng(lt.Lat, lt.Lng, deg_width, deg_height);
            return area;
        }

        /// <summary>
        /// возвращает координаты верхних левых углов тайлов, которые сейчас видны на экране, исключая уже загруженные
        /// </summary>
        /// <returns></returns>
        public List<GPoint> GetVisiblePixelTiles(PureProjection proj = null, int tileSize = 256)
        {
            int zoom = (int)Zoom;
            RectLatLng viewArea = GetViewArea();
            if (proj == null)
                proj = this.MapProvider.Projection;

            GPoint lt_pix = proj.FromLatLngToPixel(viewArea.LocationTopLeft, zoom);
            GPoint rb_pix = proj.FromLatLngToPixel(viewArea.LocationRightBottom, zoom);

            long x_from = ((int)(lt_pix.X / tileSize)); //верхние границы с округлнием в меньшую сторону
            long y_from = ((int)(lt_pix.Y / tileSize));
            long x_step = 1;
            long y_step = 1;
            long y_to = (long)Math.Ceiling(rb_pix.Y / (double)tileSize); //нижние границы с округлением в большую сторону
            long x_to = (long)Math.Ceiling(rb_pix.X / (double)tileSize);

            List<GPoint> result = new List<GPoint>();
            for (long y = y_from; y <= y_to; y += y_step)
                for (long x = x_from; x <= x_to; x += x_step)
                {
                    GPoint rll = new GPoint(x, y);
                    if (!loadedAreas.ContainsKey(rll))
                        result.Add(rll);
                }

            return result;
        }


        /// <summary>
        /// возвращает области-тайлы, которые сейчас выдны на экране. При этом исключаются области, загруженные ранее.
        /// </summary>
        /// <returns></returns>
        private List<RectLatLng> getVisibleLatLngTiles()
        {
            //тайловый способ
            //https://tech.yandex.ru/maps/doc/jsapi/2.1/theory/index-docpage/#tile_coordinates
            int zoom = (int)Zoom;
            RectLatLng viewArea = GetViewArea();
            PureProjection proj = this.MapProvider.Projection;

            GPoint lt_pix = proj.FromLatLngToPixel(viewArea.LocationTopLeft, zoom);
            GPoint rb_pix = proj.FromLatLngToPixel(viewArea.LocationRightBottom, zoom);

            GSize tile_size = proj.TileSize;

            long x_from = ((int)(lt_pix.X / 256)) * 256; //верхние границы с округлением в меньшую сторону
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
                    GPoint tileXY = LayerProjection.FromPixelToTileXY(LayerProjection.FromLatLngToPixel(rll.LocationTopLeft, zoom));
                    if (!loadedAreas.ContainsKey(tileXY))
                        result.Add(rll);
                }

            return result;
        }

        /// <summary>
        /// очичтить слои карты
        /// </summary>
        private void clearLayers()
        {
            switch (layerProvider)
            {
                case MapLayerProviders.OSMGPSTracks:
                case MapLayerProviders.OSMRailways:
                case MapLayerProviders.OSMRoadSurface:
                case MapLayerProviders.RosreestrCadaster:
                case MapLayerProviders.YandexTraffic:
                    if (rastrLayersGraphics != null)
                        try
                        {
                            rastrLayersGraphics.Clear(Color.Transparent); //очистка слоя рисования растровых слоёв
                        }
                        catch (Exception) { }
                    loadedAreas.Clear(); //очистка растрового слоя
                    break;
                case MapLayerProviders.Wikimapia:
                    ToolTipPolygonTitles.RemoveAll(); //удаление подсказок на объектах
                    vectorLayersOverlay.Polygons.Clear(); //очистка карты
                    VectorLayerObjects.Clear(); //удаляем векторные объекты на экране
                    break;
                case MapLayerProviders.None:
                    if (rastrLayersGraphics != null)
                        try
                        {
                            rastrLayersGraphics.Clear(Color.Transparent); //очистка слоя рисования растровых слоёв
                        }
                        catch (Exception) { }
                    loadedAreas.Clear(); //очистка растрового слоя
                    ToolTipPolygonTitles.RemoveAll(); //удаление подсказок на объектах
                    vectorLayersOverlay.Polygons.Clear(); //очистка карты
                    VectorLayerObjects.Clear(); //удаляем векторные объекты на экране
                    break;
                default: throw new Exception("Этот провайдер слоя не реализован");

            }
        }

        #endregion

    }
}
