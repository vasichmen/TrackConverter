﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
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
    public class FormMapBase : Form
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
        /// область карты
        /// </summary>
        public GMapControl gmapControlMap=new GMapControl();

        /// <summary>
        /// список загруженных треков
        /// </summary>
        public TrackFileList tracks;

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
        /// измерение расстояния
        /// </summary>
        public TrackFile rulerRoute;

        #endregion

        #region навигация, взаимодействие

        /// <summary>
        /// стек последних позиций при переходах по карте
        /// </summary>
        public Stack<KeyValuePair<string, Coordinate>> PositionsStack { get; set; }

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
        protected void DeleteWaypoint(TrackPoint trackPoint, GMapOverlay lay)
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
        protected void ShowWaypoint(TrackPoint point, bool addToWaypoint)
        {
            ShowWaypoint(point, baseOverlay, addToWaypoint);
        }

        /// <summary>
        /// покзать одну точку
        /// </summary>
        /// <param name="point">точка</param>
        /// <param name="lay">слой в GMapControl для отображения маркера</param>
        /// <param name="addToWaypoint">если истина, то точка будет добавлена в список путевых точек</param>
        protected void ShowWaypoint(TrackPoint point, GMapOverlay lay, bool addToWaypoint)
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
        protected void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes type)
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
        protected void ShowWaypoint(TrackPoint point, GMapOverlay lay, Icon icon, MarkerTypes mType, MarkerTooltipMode ttMode)
        {
            Point offsets = IconOffsets.GetOffset(point.Icon);
            MapMarker mar = new MapMarker(point.Coordinates.GMap, icon, offsets);

            if (string.IsNullOrWhiteSpace(point.Name))
                point.Name = point.Coordinates.ToString("{lat},{lon}", "00.000");

            mar.Tag.Type = mType;
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
        protected void ShowWaypoints(TrackFile points, bool isClearBefore, bool addToWayPoints)
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
        /// выод на карту списка точек. Новые точки будут добавлены в список
        /// </summary>
        /// <param name="isClearBefore">если истина, то перед выводом слой будет очищен от мареров</param>
        /// <param name="wpts">список точек для вывода</param>
        /// <param name="lay">слой, на котором будут выведены точки</param>
        protected void ShowWaypoints(TrackFile wpts, GMapOverlay lay, bool isClearBefore)
        {
            if (isClearBefore)
                lay.Markers.Clear();
            gmapControlMap.SuspendLayout();
            foreach (TrackPoint tt in wpts)
                ShowWaypoint(tt, lay, true);
            gmapControlMap.ResumeLayout();
        }

        #endregion

        #region маршруты

        /// <summary>
        /// показать заданный маршрут. Если маршрута нет в списке машрутов карты то добавляет в общий список
        /// </summary>
        /// <param name="route">маршрут</param>
        protected void ShowRoute(TrackFile route)
        {
            ShowRoute(route, baseOverlay);
        }

        /// <summary>
        /// показать маршрут на указанном слое и добавить в список маршрутов
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="overlay">слой</param>
        protected void ShowRoute(TrackFile route, GMapOverlay overlay)
        {
            if (tracks == null)
                tracks = new TrackFileList();
            if (!tracks.Contains(route))
                tracks.Add(route);
            ShowRoute(route, overlay);
        }

        /// <summary>
        /// отрисовка маршрута на заданном слое, но не добавляет в список маршрутов
        /// </summary>
        /// <param name="route">маршрут</param>
        /// <param name="lay">слой</param>
        /// <param name="centring">если истина, то после отрисовки карта будет подогнана под маршрут по центру и масштбу</param>
        protected void ShowRoute(TrackFile route, GMapOverlay lay, bool centring = true)
        {
            if (route == null)
                return;
            if (route.Color.IsEmpty)
                route.Color = Vars.Options.Converter.GetColor();
            GMapRoute rr = new GMapRoute(route.GMapPoints, route.Name);
            rr.Stroke = new Pen(new SolidBrush(route.Color));
            rr.Stroke.Width = 3;

            lay.Routes.Add(rr);
            if (centring)
                gmapControlMap.ZoomAndCenterRoute(rr);
        }

        /// <summary>
        /// удаление маршрута с карты
        /// </summary>
        /// <param name="route"></param>
        protected void DeleteRoute(TrackFile route)
        {
            if (this.tracks == null || route == null) return;
            this.tracks.Remove(route);
            ShowRoutes(tracks, true);
        }

        /// <summary>
        /// показать на карте маршруты в базовом слое и добавить в список которых нет
        /// </summary>
        /// <param name="tracksList">маршруты</param>
        /// <param name="isClearBefore">если истина, то перед добавление будут удалены предыдущие маршруты</param>
        protected void ShowRoutes(TrackFileList tracksList, bool isClearBefore)
        {
            if (isClearBefore)
                baseOverlay.Routes.Clear();

            if (tracks == null)
                tracks = new TrackFileList();

            foreach (TrackFile tf in tracksList)
            {
                if (!tracks.Contains(tf))
                    tracks.Add(tf);
                ShowRoute(tf, baseOverlay, false);
            }
        }

        #endregion


        #endregion

        #region конфигурация GMapControlMap



        #endregion
    }
}