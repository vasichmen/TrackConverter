using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;

namespace TrackConverter.UI.Shell
{
    internal partial class MapHelper
    {


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
            ShowWaypoint(point, formMain.baseOverlay, addToWaypoint);
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
                if (formMain.waypoints == null)
                    formMain.waypoints = new TrackFile();
                if (!formMain.waypoints.Contains(point))
                    formMain.waypoints.Add(point);
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
                if (formMain.waypoints == null)
                    formMain.waypoints = new TrackFile();
                formMain.waypoints.Add(points);
            }
            ShowWaypoints(points, formMain.baseOverlay, isClearBefore);
            formMain.gmapControlMap.ZoomAndCenterMarkers(formMain.baseOverlay.Id);
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
            ShowRoute(route, formMain.baseOverlay);
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
                formMain.gmapControlMap.ZoomAndCenterRoute(rr);
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
                RectLatLng? rect = formMain.gmapControlMap.GetRectOfRoute(route.GetTotalMapRoute());
                if (rect != null)
                    formMain.gmapControlMap.SetZoomToFitRect((RectLatLng)rect);
            }
        }

        /// <summary>
        /// добавление точек на карту
        /// </summary>
        /// <param name="waypts">список точек</param>
        /// <param name="lay">слой для добавления</param>
        /// <param name="clearBefore">очистить слой перед добавлением</param>
        /// <param name="addToWaypoints">если истина, то точки будут добавлены в список точек</param>
        public void ShowWaypoints(BaseTrack waypts, GMapOverlay lay, bool clearBefore, bool addToWaypoints)
        {
            if (waypts == null)
                return;
            if (clearBefore)
                lay.Markers.Clear();
            formMain.gmapControlMap.SuspendLayout();
            if (waypts.GetType() == typeof(TrackFile))
                foreach (TrackPoint tt in waypts)
                    ShowWaypoint(tt, lay, addToWaypoints);
            else
                foreach (TrackPoint tt in ((TripRouteFile)waypts).Waypoints)
                    ShowWaypoint(tt, lay, addToWaypoints);
            formMain.gmapControlMap.ResumeLayout();
        }

        /// <summary>
        /// удаление заданных точек с карты
        /// </summary>
        /// <param name="tf"></param>
        public void DeleteWaypoints(BaseTrack tf)
        {
            foreach (TrackPoint tt in tf)
                formMain.waypoints.Remove(tt);
            ShowWaypoints(formMain.waypoints, true, true);
        }


        #endregion


        #endregion
    }
}