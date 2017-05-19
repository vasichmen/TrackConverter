using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.StackEdits;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// основные действия, выполняемые с картой
    /// </summary>
    partial class FormMap
    {

        #region public поля

        #region флаги, передача параметров в методы

        /// <summary>
        /// индекс активного маркера при создании маршрута
        /// </summary>
        public int selectedPointIndex;

        /// <summary>
        /// если истина, значит создается маршрут
        /// </summary>
        public bool isCreatingRoute;

        /// <summary>
        /// если истина, то при нажатии на карту ЛКМ надо открыть меню редактирования точки и вызвать this.AfterSelectPointAction
        /// </summary>
        public bool isSelectingPoint = false;

        /// <summary>
        /// если истина, то идет измерение расстояния
        /// </summary>
        public bool isRuling;

        /// <summary>
        /// нажата ли кнопка стрелок. Исользкется в методе таймера передвижения карты
        /// </summary>
        public bool canMoveMap = false;

        /// <summary>
        /// при нажатии стрелок здесь информация, какая стрелка нажата
        /// </summary>
        public Keys canMoveMapDirection;

        #endregion

        #region карта, слои, маршруты


        /// <summary>
        /// список загруженных путевых точек
        /// </summary>
        public TrackFile waypoints;

        /// <summary>
        /// создаваемый маршрут
        /// </summary>
        public TrackFile creatingRoute;

        /// <summary>
        /// слой с маркерами и маршрутами на карте
        /// </summary>
        public GMapOverlay baseOverlay;

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        public GMapOverlay creatingRouteOverlay;

        /// <summary>
        /// слой измерения расстояний
        /// </summary>
        public GMapOverlay rulerRouteOverlay;

        /// <summary>
        /// слой вывода создаваемого путешествия
        /// </summary>
        public GMapOverlay creatingTripOverlay;

        /// <summary>
        /// измерение расстояния
        /// </summary>
        public TrackFile rulerRoute;

        #endregion

        #region навигация, взаимодействие

        /// <summary>
        /// стек последних позиций при переходах по карте
        /// </summary>
        public Stack<KeyValuePair<string, Coordinate>> PositionsStack { get; set; }

        /// <summary>
        /// словарь с соответствиями точек и окон "что здесь"
        /// </summary>
        public Dictionary<TrackPoint, FormWhatsthere> ActiveWhatThereForms { get; set; }

        #endregion

        #endregion

        #region protected поля

        #region карта, слои, маршруты

        /// <summary>
        /// текущий выбранный маркер, на котором находится мышь
        /// </summary>
        protected MapMarker currentMarker;

        /// <summary>
        /// точка, на которой произошло нажатие (для вывода ContextMenu на пустой карте)
        /// </summary>
        protected PointLatLng pointClicked;

        /// <summary>
        /// маркер, на котором произошло нажатие(для вывода контекстного меню маркера)
        /// </summary>
        protected MapMarker markerClicked;

        /// <summary>
        /// маршрут, для которого произошло нажатие (для вывода контекстного меню)
        /// </summary>
        protected GMapRoute routeClicked;

        /// <summary>
        /// слой выделенной точки
        /// </summary>
        protected GMapOverlay selectedPointsOverlay;

        /// <summary>
        /// слой, выделенного в списке маршрутов, маршрута
        /// </summary>
        protected GMapOverlay selectedRouteOverlay;

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        protected GMapOverlay fromToOverlay;

        /// <summary>
        /// слой точек "что здесь"
        /// </summary>
        protected GMapOverlay whatThereOverlay;

        /// <summary>
        /// начало маршрута
        /// </summary>
        protected TrackPoint fromPoint;

        /// <summary>
        /// конец маршрута
        /// </summary>
        protected TrackPoint toPoint;

        /// <summary>
        /// промежуточные точки при построении маршрутов
        /// </summary>
        protected TrackFile IntermediatePoints;

        #endregion

        #region состояние карты

        /// <summary>
        /// состояние нажатия левой кнопки мыши. Для перетаскивания маркера
        /// </summary>
        protected bool isLeftButtonDown = false;

        /// <summary>
        /// если истина, то значит, идет перемещаение маркера
        /// </summary>
        protected bool isMarkerMoving;

        /// <summary>
        /// если истина, то последнее нажатие мыши было на маркере.
        /// Используется при создании маршрута при выделении маркера
        /// </summary>
        protected bool isMarkerClicked;

        #endregion

        #region флаги, передача параметров

        /// <summary>
        /// действие, которое выполняется после выбора и редактирования точки (при редактировании путешествия)
        /// </summary>
        protected Action<TrackPoint> AfterSelectPointAction;

        /// <summary>
        /// действие, выполняемое при отмене выбора точки на карте при создании путешествия
        /// </summary>
        protected Action CancelSelectPointAction;

        /// <summary>
        /// координаты маркера до перемещения. (для отмены действия)
        /// </summary>
        protected TrackPoint oldMarker = null;

        #endregion

        #region идентификаторы

        /// <summary>
        /// базовый слой
        /// </summary>
        protected readonly string baseOverlayID = "baseOverlay";

        /// <summary>
        /// слой выделенного маршрут
        /// </summary>
        protected readonly string selectedRouteOverlayID = "selectedRouteOverlay";

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        protected readonly string creatingRouteOverlayID = "creatingRouteOverlay";

        /// <summary>
        /// слой создаваемого маршрута
        /// </summary>
        protected readonly string creatingTripOverlayID = "creatingTripOverlay";

        /// <summary>
        /// линейка
        /// </summary>
        protected readonly string rulerRouteOverlayID = "rulerRouteOverlay";

        /// <summary>
        /// слой выделенных точек через метод SelectPoint
        /// </summary>
        protected readonly string selectedPointsOverlayID = "selectedPointsOverlay";

        /// <summary>
        /// слой маркеров построения маршрута From To
        /// </summary>
        protected readonly string fromToOverlayID = "fromToOverlay";

        /// <summary>
        /// слой маркеров "что здесь"
        /// </summary>
        protected readonly string whatThereOverlayID = "whatThereOverlay";
        #endregion

        #region работа интерфейса

        /// <summary>
        /// стек последних изменений на карте
        /// </summary>
        protected Stack<StackItem> LastEditsStack { get; set; }



        /// <summary>
        /// таймер перемещения карты
        /// </summary>
        protected System.Timers.Timer moveMapTimer;

        /// <summary>
        /// таймер обновления списка поиска 
        /// </summary>
        protected System.Timers.Timer refreshGoToTimer;

        /// <summary>
        /// последнее изменение текста в комбобоксе поиска адреса
        /// </summary>
        protected DateTime lastCBGoToChanged = DateTime.Now;

        /// <summary>
        /// последний запрос в поиске мест
        /// </summary>
        protected string lastGoToQuery = "";

        #endregion

        #endregion

        #region отрисовка карты

        #region точки


        /// <summary>
        /// удаление отображаемой точки точки с слоя 
        /// </summary>
        /// <param name="trackPoint">точка</param>
        /// <param name="lay">слой</param>
        public void DeleteWaypoint(TrackPoint trackPoint, GMapOverlay lay)
        {
            if (trackPoint == null) return;

            for (int i = 0; i < lay.Markers.Count; i++)
            {
                MapMarker it = lay.Markers[i] as MapMarker;
                if (it.Tag.Info == trackPoint)
                {
                    lay.Markers.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// показать заданную точку на базовом слое
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="addToWaypoint">еси истина, то точка будет добавлена к путевым точкам</param>
        public void ShowWaypoint(TrackPoint point, bool addToWaypoint)
        {
            ShowWaypoint(point, baseOverlay, addToWaypoint);
        }

        /// <summary>
        /// покзать одну точку
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="lay">слой в GMapControl для отображения маркера</param>
        /// <param name="addToWaypoint">если истина, то точка будет добавлена в список путевых точек</param>
        public void ShowWaypoint(TrackPoint point, GMapOverlay lay, bool addToWaypoint)
        {
            if (point == null)
                return;
            if (addToWaypoint)
            {
                if (waypoints == null)
                    waypoints = new TrackFile();
                if (!waypoints.Contains(point))
                    waypoints.Add(point);
            }
            //если иконки нет, то базовую иконку
            Icon ic;
            if (point.Icon == IconOffsets.marker)
                ic = Resources.marker;
            else
            {
                //если есть файл такой иконки, то его, если нет - базовую иконку
                string fn = Application.StartupPath + Resources.icons_directory + "\\" + point.Icon.ToString("000") + ".ico";
                if (File.Exists(fn))
                    ic = new Icon(fn);
                else
                    ic = Resources.marker;
            }
            ShowWaypoint(point, lay, ic, MarkerTypes.Waypoint);
        }

        /// <summary>
        /// показать одну точку с заданной картинкой
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="lay">слой</param>
        /// <param name="icon">относительный адрес картинки (из Resources)</param>
        /// <param name="type">тип маркера</param>
        public void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes type)
        {
            ShowWaypoint(point, lay, icon, type, MarkerTooltipMode.OnMouseOver);
        }

        /// <summary>
        /// показать одну точку с заданной картинкой, на указанном слое, с заданными типами подсказки и маркера
        /// </summary>
        /// <param name="point">информация о точке</param>
        /// <param name="lay">слой</param>
        /// <param name="icon">картинка</param>
        /// <param name="mType">тип  маркера</param>
        /// <param name="ttMode">тип всплывающей подсказки</param>
        public void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes mType, MarkerTooltipMode ttMode)
        {
            ShowWaypoint(point, lay, icon, mType, PathingType.None, ttMode);
        }

        /// <summary>
        /// показать одну точку с заданной картинкой, на указанном слое, с заданными типами подсказки и маркера
        /// </summary>
        /// <param name="point">информация о точке</param>
        /// <param name="lay">слой</param>
        /// <param name="icon">картинка</param>
        /// <param name="mType">тип  маркера</param>
        /// <param name="pType">тип точки при прокладке маршрута</param>
        /// <param name="ttMode">тип всплывающей подсказки</param>
        public void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes mType, PathingType pType, MarkerTooltipMode ttMode)
        {
            Point offsets = IconOffsets.GetOffset(point.Icon);
            MapMarker mar = new MapMarker(point.Coordinates.GMap, icon, offsets);

            if (string.IsNullOrWhiteSpace(point.Name))
                point.Name = point.Coordinates.ToString("{lat},{lon}", "00.000");

            mar.Tag.Type = mType;
            mar.Tag.PathingType = pType;
            mar.ToolTipText = point.Name;
            mar.ToolTipMode = ttMode;
            mar.Tag.Info = point;

            lay.Markers.Add(mar);
        }

        /// <summary>
        /// показать путевые точки на карте в базовом слое
        /// </summary>
        /// <param name="points">точки для показа</param>
        /// <param name="addToWayPoints">если true , то точки будут добавлены в список путевых точек</param>
        /// <param name="isClearBefore">если истина, перед добавлением будет произведена очитска слоя от точек</param>
        public void ShowWaypoints(BaseTrack points, bool isClearBefore, bool addToWayPoints)
        {
            if (points == null)
                return;
            if (addToWayPoints)
            {
                if (waypoints == null)
                    waypoints = new TrackFile();
                waypoints.Add(points);
            }
            ShowWaypoints(points, baseOverlay, isClearBefore);
            gmapControlMap.ZoomAndCenterMarkers(baseOverlay.Id);
        }

        /// <summary>
        /// выод на карту списка точек. Новые точки не будут добавлены в список
        /// </summary>
        /// <param name="isClearBefore">если истина, то перед выводом слой будет очищен от мареров</param>
        /// <param name="wpts">список точек для вывода</param>
        /// <param name="lay">слой, на котором будут выведены точки</param>
        public void ShowWaypoints(BaseTrack wpts, GMapOverlay lay, bool isClearBefore)
        {
            ShowWaypoints(wpts, lay, isClearBefore, false);
        }

        #endregion

        #region маршруты

        /// <summary>
        /// показать заданный маршрут. Если маршрута нет в списке машрутов карты то добавляет в общий список
        /// </summary>
        /// <param name="route">маршрут</param>
        public void ShowRoute(BaseTrack route)
        {
            ShowRoute(route, baseOverlay);
        }

        /// <summary>
        /// показать маршрут на указанном слое и добавить в список маршрутов
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="overlay">слой</param>
        public void ShowRoute(BaseTrack route, GMapOverlay overlay)
        {
            ShowRoute(route, overlay, true);
        }

        /// <summary>
        /// отрисовка маршрута на заданном слое, но не добавляет в список маршрутов
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="lay">слой</param>
        /// <param name="centring">если истина, то после отрисовки карта будет подогнана под маршрут по центру и масштбу</param>
        public void ShowRoute(BaseTrack route, GMapOverlay lay, bool centring)
        {
            if (route.GetType() == typeof(TrackFile))
                ShowTrackFile(route as TrackFile, lay, centring);
            else
                ShowTripRoute(route as TripRouteFile, lay, centring);
        }

        /// <summary>
        /// вывод на карту заданного маршрута
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="lay">слой для вывода</param>
        /// <param name="centring">если истина, то карты будет по центру</param>
        public void ShowTrackFile(TrackFile route, GMapOverlay lay, bool centring = true)
        {
            if (route == null)
                return;
            if (route.Color.IsEmpty)
                route.Color = Vars.Options.Converter.GetColor();
            GMapRoute rr = new GMapRoute(route.GMapPoints, route.Name);
            rr.IsHitTestVisible = true;
            rr.IsVisible = true;
            rr.Stroke = new Pen(new SolidBrush(route.Color));
            rr.Stroke.Width = 3;
            rr.Tag = route;

            lay.Routes.Add(rr);
            if (centring)
                gmapControlMap.ZoomAndCenterRoute(rr);
        }

        /// <summary>
        /// вывод на экран путешествия по частям(кажды день своим цветом) и его путевых точек
        /// </summary>
        /// <param name="route"></param>
        /// <param name="lay"></param>
        /// <param name="centring"></param>
        public void ShowTripRoute(TripRouteFile route, GMapOverlay lay, bool centring)
        {
            //вывод маршрутов
            foreach (TrackFile tf in route.DaysRoutes)
                ShowTrackFile(tf, lay, false);

            //вывод путевых точек
            ShowWaypoints(route.Waypoints, lay, false, false);

            //если требуется центровка
            if (centring)
            {
                RectLatLng? rect = gmapControlMap.GetRectOfRoute(route.GetTotalMapRoute());
                if (rect != null)
                    gmapControlMap.SetZoomToFitRect((RectLatLng)rect);
            }
        }

        /// <summary>
        /// добавление точек на карту
        /// </summary>
        /// <param name="waypoints">список точек</param>
        /// <param name="lay">слой для добавления</param>
        /// <param name="clearBefore">очистить слой перед добавлением</param>
        /// <param name="addToWaypoints">если истина, то точки будут добавлены в список точек</param>
        public void ShowWaypoints(BaseTrack waypoints, GMapOverlay lay, bool clearBefore, bool addToWaypoints)
        {
            if (clearBefore)
                lay.Markers.Clear();
            gmapControlMap.SuspendLayout();
            if (waypoints.GetType() == typeof(TrackFile))
                foreach (TrackPoint tt in waypoints)
                    ShowWaypoint(tt, lay, addToWaypoints);
            else
                foreach (TrackPoint tt in ((TripRouteFile)waypoints).Waypoints)
                    ShowWaypoint(tt, lay, addToWaypoints);
            gmapControlMap.ResumeLayout();
        }

        /// <summary>
        /// удаление заданных точек с карты
        /// </summary>
        /// <param name="tf"></param>
        public void DeleteWaypoints(BaseTrack tf)
        {
            foreach (TrackPoint tt in tf)
                waypoints.Remove(tt);
            ShowWaypoints(waypoints, true, true);
        }


        #endregion


        #endregion
    }
}
