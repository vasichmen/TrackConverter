using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.ProviderRecords;
using TrackConverter.Lib.Classes.StackEdits;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.OS;
using TrackConverter.Lib.Maping.GMap;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Map;
using static TrackConverter.Lib.Classes.StackEdits.Actions;

namespace TrackConverter.UI.Shell
{

    /// <summary>
    /// вспомогательные функции для работы интерфейса карты
    /// </summary>
    internal partial class MapHelper : MapHelperBase
    {

        /// <summary>
        /// Создает новый экземпляр с заданным главным окном программы
        /// </summary>
        /// <param name="formMain"></param>
        public MapHelper(FormMain formMain)
        {
            this.formMain = formMain;
        }

        /// <summary>
        /// настройки браузера карты
        /// </summary>
        public void ConfigureGMapControl()
        {

            #region системные настройки

            formMain.gmapControlMap.DragButton = MouseButtons.Left;

            //порядок получения данных 
            GMaps.Instance.Mode = Vars.Options.Map.AccessMode;
            GMaps.Instance.CacheOnIdleRead = true;
            GMaps.Instance.BoostCacheEngine = true;
            GMaps.Instance.MemoryCache.Capacity = 200; //максимальный размер кэша в МБ

            //zoom
            formMain.gmapControlMap.MaxZoom = Vars.Options.Map.MaximalZoom;
            formMain.gmapControlMap.Zoom = Vars.Options.Map.Zoom;
            OnMapZoomChanged();

            //информация о масштабе карты
            formMain.gmapControlMap.MapScaleInfoEnabled = false;

            //включение кэша карт, маршрутов итд
            formMain.gmapControlMap.Manager.UseDirectionsCache = true;
            formMain.gmapControlMap.Manager.UseGeocoderCache = true;
            formMain.gmapControlMap.Manager.UseMemoryCache = true;
            formMain.gmapControlMap.Manager.UsePlacemarkCache = true;
            formMain.gmapControlMap.Manager.UseRouteCache = true;
            formMain.gmapControlMap.Manager.UseUrlCache = true;

            //отключение черно-белого режима
            formMain.gmapControlMap.GrayScaleMode = false;

            //заполнение отсутствующих тайлов из меньшего масштаба
            formMain.gmapControlMap.FillEmptyTiles = true;

            //язык карты
            GMapProvider.Language = Vars.Options.Map.MapLanguange;

            //центральная точка
            if (Vars.Options.Map.LastCenterPoint != null)
                formMain.gmapControlMap.Position = Vars.Options.Map.LastCenterPoint;
            else
            {
                formMain.gmapControlMap.Position = new PointLatLng(37, 55);
                Vars.Options.Map.LastCenterPoint = formMain.gmapControlMap.Position;
            }

            //поставщик карты
            formMain.gmapControlMap.MapProvider = MapProviderRecord.MapProviderToClass(Vars.Options.Map.MapProvider.Enum);

            //поставщик слоя
            formMain.gmapControlMap.LayerProvider = Vars.Options.Map.LayerProvider.Enum;


            //Если вы используете интернет через прокси сервер,
            //указываем свои учетные данные.
            GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;


            //вид пустых тайлов
            formMain.gmapControlMap.EmptyMapBackground = Color.White;
            formMain.gmapControlMap.EmptyTileColor = Color.White;
            formMain.gmapControlMap.EmptyTileText = "Не удалось загрузить изображение \r\n Возможно, проблема с интернет-соединением или попробуйте уменьшить масштаб.";
            formMain.gmapControlMap.MapScaleInfoEnabled = true;

            //папка с кэшем
            Directory.CreateDirectory(Application.StartupPath + Resources.cache_directory);
            formMain.gmapControlMap.CacheLocation = Application.StartupPath + Resources.cache_directory;

            #endregion


            //обновление галочек в меню способа загрузки тайлов
            formMain.tmCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.CacheOnly;
            formMain.tmInternetCacheToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerAndCache;
            formMain.tmInternetToolStripMenuItem.Checked = Vars.Options.Map.AccessMode == AccessMode.ServerOnly;

            #region создание объектов

            //создание слоев на карте
            formMain.selectedPointsOverlay = new GMapOverlay(formMain.selectedPointsOverlayID);
            formMain.selectedRouteOverlay = new GMapOverlay(formMain.selectedRouteOverlayID);
            formMain.creatingRouteOverlay = new GMapOverlay(formMain.creatingRouteOverlayID);
            formMain.creatingPolygonOverlay = new GMapOverlay(formMain.creatingPolygonOverlayID);
            formMain.rulerRouteOverlay = new GMapOverlay(formMain.rulerRouteOverlayID);
            formMain.baseOverlay = new GMapOverlay(formMain.baseOverlayID);
            formMain.fromToOverlay = new GMapOverlay(formMain.fromToOverlayID);
            formMain.whatThereOverlay = new GMapOverlay(formMain.whatThereOverlayID);
            formMain.creatingTripOverlay = new GMapOverlay(formMain.creatingTripOverlayID);
            formMain.searchOverlay = new GMapOverlay(formMain.searchOverlayID);

            //добавление слоев на карту
            formMain.gmapControlMap.Overlays.Add(formMain.selectedPointsOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.selectedRouteOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.creatingRouteOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.creatingPolygonOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.rulerRouteOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.baseOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.fromToOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.creatingTripOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.whatThereOverlay);
            formMain.gmapControlMap.Overlays.Add(formMain.searchOverlay);

            #endregion

            //выставляем флаг, что загрузка настроек завершена
            formMain.gmapControlMap.isControlLoaded = true;
        }


        #region События карты


        internal void OnMapZoomChanged()
        {
            if (!Program.winNavigatorNullOrDisposed && Program.winNavigator.Visible)
                Program.winNavigator.labelZoom.Text = "Zoom: " + formMain.gmapControlMap.Zoom;
            formMain.toolStripLabelZoom.Text = formMain.gmapControlMap.Zoom.ToString();
            Vars.Options.Map.Zoom = formMain.gmapControlMap.Zoom;
        }

        internal void MouseClick(MouseEventArgs e)
        {
            #region добавление точки к новому маршруту если 

            //происходит создание маршрута, 
            //не происходит перемещение маркера,
            //указатель не находится на другом маркере
            if (e.Button == MouseButtons.Left && //если левая кнопка
                (formMain.isCreatingRoute || formMain.isRuling) && //создание маршрута или линейка
                !formMain.isMarkerMoving && //не происходит движение маркера
                !formMain.gmapControlMap.IsMouseOverMarker)
            {
                //обновление информации в списке точек после перемещения
                refreshWaypoints();

                if (formMain.isCreatingRoute)
                {
                    PointLatLng pt = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                    //если выделена последняя точка, то добавляем
                    if (formMain.selectedRoutePointIndex == formMain.creatingRoute.Count)
                    {
                        formMain.creatingRoute.Add(new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedRoutePointIndex = formMain.creatingRoute.Count - 1;
                    }
                    //если выделена точка в середине маршрута, то вставляем после нее
                    else
                    {
                        formMain.creatingRoute.Insert(formMain.selectedRoutePointIndex + 1, new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedRoutePointIndex++;
                    }

                    //вывод нового маршрута на экран
                    ShowCreatingRoute(formMain.creatingRouteOverlay, formMain.creatingRoute);
                }
                if (formMain.isRuling)
                {
                    PointLatLng pt = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                    //если выделена последняя точка, то добавляем
                    if (formMain.selectedRoutePointIndex == formMain.rulerRoute.Count)
                    {
                        formMain.rulerRoute.Add(new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedRoutePointIndex = formMain.rulerRoute.Count - 1;
                    }
                    //если выделена точка в середине маршрута, то вставляем после нее
                    else
                    {
                        formMain.rulerRoute.Insert(formMain.selectedRoutePointIndex + 1, new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedRoutePointIndex++;
                    }

                    //вывод нового маршрута на экран
                    ShowCreatingRoute(formMain.rulerRouteOverlay, formMain.rulerRoute);
                }
            }
            #endregion

            #region добавление точки к новому периметру если 

            //происходит создание маршрута, 
            //не происходит перемещение маркера,
            //указатель не находится на другом маркере
            if (e.Button == MouseButtons.Left && //если левая кнопка
                (formMain.isCreatingPolygon) && //создание периметра
                !formMain.isMarkerMoving && //не происходит движение маркера
                !formMain.gmapControlMap.IsMouseOverMarker)
            {
                if (formMain.isCreatingPolygon)
                {
                    PointLatLng pt = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                    //если выделена последняя точка, то добавляем
                    if (formMain.selectedPolygonPointIndex == formMain.creatingPolygon.Count)
                    {
                        formMain.creatingPolygon.Add(new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedPolygonPointIndex = formMain.creatingPolygon.Count - 1;
                    }
                    //если выделена точка в середине маршрута, то вставляем после нее
                    else
                    {
                        formMain.creatingPolygon.Insert(formMain.selectedPolygonPointIndex + 1, new TrackPoint(pt) { Icon = IconOffsets.CREATING_ROUTE_MARKER });
                        formMain.selectedPolygonPointIndex++;
                    }

                    //вывод нового маршрута на экран
                    ShowCreatingPolygon(formMain.creatingPolygonOverlay, formMain.creatingPolygon);
                }

                return;
            }
            #endregion

            #region нажатие на полигон при выборе векторного объекта

            //если не идёт создание маршрута, не выбор точки, не линейка, не перемещение маркера, не перемещение карты, ЛКМ, то обрабатываем нажатие
            if (!formMain.isCreatingRoute && //не создание маршрута
                !formMain.isSelectingPoint &&  //не выбор точки
                !formMain.isRuling && //не линейка
                !formMain.isMarkerMoving && //не перемещение маркера
                !formMain.gmapControlMap.IsDragging && //если не перемещение карты
                e.Button == MouseButtons.Left && //левая кнопка мыши
                !formMain.gmapControlMap.IsMouseOverMarker //если курсор не на маркере
                )
            {
                OnPolygonClick(e);
            }

            #endregion

            #region выбор точки при редактировании путешествия

            //если идёт выбор точки, не идёт создание маршрута и не линейка и клик не на маркере
            if (formMain.isSelectingPoint && !(formMain.isCreatingRoute || formMain.isRuling) && !formMain.isMarkerClicked && !formMain.gmapControlMap.IsDragging)
            {
                PointLatLng pt = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);
                TrackPoint newpt = new TrackPoint(pt);
                if (formMain.AfterSelectPointAction != null)
                    formMain.AfterSelectPointAction.Invoke(newpt);
            }

            #endregion

            formMain.isMarkerClicked = false;
        }

        internal void OnPositionChanged(PointLatLng point)
        {
            Vars.Options.Map.LastCenterPoint = point;
        }

        /// <summary>
        /// событие при нажатии на маркер
        /// </summary>
        /// <param name="itm"></param>
        /// <param name="e"></param>
        internal void OnMarkerClick(GMapMarker itm, MouseEventArgs e)
        {
            MapMarker item = itm as MapMarker;

            //изменение инормаци по лкм
            //если не происходило перемещение маркера и не "что здесь"  и не происходит создание маршрута и не линейка
            if (!formMain.isMarkerMoving && !formMain.isCreatingRoute && !formMain.isRuling && item.Tag.Type != MarkerTypes.WhatThere && e.Button == MouseButtons.Left)
            {
                if (item.Clickable) //если на маркер можно нажимать, то обработка нажатия
                {
                    formMain.markerClicked = item;
                    formMain.isMarkerClicked = true; //для того,чтобы при активных слоях викимапии не открывалось окно инфо. об объекте
                    ToolStripEditMarker(e);
                    return;
                }
            }

            //открытие информации об объекте при нажатии на маркере категории Wikimapia
            if (!formMain.isMarkerMoving && !formMain.isCreatingRoute && !formMain.isRuling && item.Tag.Type == MarkerTypes.WikimpiaCategoryItem && e.Button == MouseButtons.Left)
            {
                formMain.isMarkerClicked = true;
                FormShowObjectInfo fsoi = new FormShowObjectInfo((VectorMapLayerObject)item.Tag.Tag);
                fsoi.Show(this.formMain);
                return;
            }

            //открытие соответствующего окна Что Здесь по маркеру
            //если не происходило перемещение маркера и не происходит создание маршрута и не линейка и "что здесь"
            if (!formMain.isMarkerMoving && !formMain.isCreatingRoute && !formMain.isRuling && item.Tag.Type == MarkerTypes.WhatThere && e.Button == MouseButtons.Left)
            {
                //если в словаре уже есть такая точка, то открываем окно
                if (formMain.ActiveWhatThereForms.ContainsKey(item.Tag.Info))
                {
                    FormWhatsthere fw = formMain.ActiveWhatThereForms[item.Tag.Info];
                    if (!fw.Visible)
                        fw.Show(Program.winMain);
                    else
                        fw.Activate();
                }

                //если точки нет, то создаем новое окно и заносим в словарь
                else
                {
                    FormWhatsthere fw = new FormWhatsthere(item.Tag.Info);
                    fw.Show(Program.winMain);
                    formMain.ActiveWhatThereForms.Add(item.Tag.Info, fw);
                }
            }

            //открытие контекстного меню по пкм
            //если не передвижение маркера и не создание маршрута и правая кнопка мыши то вывод меню маркера
            if (!formMain.isMarkerMoving && item.Tag.Type != MarkerTypes.CreatingRoute && e.Button == MouseButtons.Right)
            {
                formMain.markerClicked = item;
                formMain.contextMenuStripMarker.Show(new Point(e.X, e.Y));
                return;
            }

            //выделение нажатого маркера при создании маршрута
            if (e.Button == MouseButtons.Left && (formMain.isCreatingRoute || formMain.isRuling) && !formMain.isMarkerMoving && formMain.gmapControlMap.IsMouseOverMarker)
            {
                if (formMain.isCreatingRoute)
                {
                    if (formMain.creatingRoute.Contains(item.Tag.Info))
                    {
                        formMain.selectedRoutePointIndex = formMain.creatingRoute.IndexOf(item.Tag.Info);
                        ShowCreatingRoute(formMain.creatingRouteOverlay, formMain.creatingRoute);
                    }
                }
                if (formMain.isRuling)
                {
                    if (formMain.rulerRoute.Contains(item.Tag.Info))
                    {
                        formMain.selectedRoutePointIndex = formMain.rulerRoute.IndexOf(item.Tag.Info);
                        ShowCreatingRoute(formMain.rulerRouteOverlay, formMain.rulerRoute);
                    }
                }
            }

            //выделение нажатого маркера при создании периметра
            if (e.Button == MouseButtons.Left && formMain.isCreatingPolygon && !formMain.isMarkerMoving && formMain.gmapControlMap.IsMouseOverMarker)
            {
                if (formMain.creatingPolygon.Contains(item.Tag.Info))
                {
                    formMain.selectedPolygonPointIndex = formMain.creatingPolygon.IndexOf(item.Tag.Info);
                    ShowCreatingPolygon(formMain.creatingPolygonOverlay, formMain.creatingPolygon);
                }

            }
        }

        /// <summary>
        /// нажатие на полигон на карте. Этот метод нельзя подписывать напрямую на событие OnPolygonClick, иначе будут проблемы при выборе точки на карте
        /// </summary>
        /// <param name="e"></param>
        internal void OnPolygonClick(MouseEventArgs e)
        {
            //выбор дополнительной информации об объекте 
            if (e.Button == MouseButtons.Left)
            {
                formMain.currentMarker = null;
                List<VectorMapLayerObject> objs = formMain.gmapControlMap.GetVectorObjectsUnderCursor();
                if (objs.Count == 0)
                    return;
                VectorMapLayerObject obj;
                if (objs.Count == 1)
                    obj = objs[0];
                else
                {
                    FormChooseVariant fcv = new FormChooseVariant(objs, "Выбор объекта", SelectionMode.One);
                    if (fcv.ShowDialog(Program.winMain) == DialogResult.OK)
                    {
                        if (fcv.Result.Count != 1) //выход, если выбрано более одного объекта
                            return;
                        obj = objs[fcv.Result[0]];
                    }
                    else //выход, если выбор объекта отменён
                        return;
                }
                new FormShowObjectInfo(obj).Show();
                return;
            }
        }

        internal void OnMarkerEnter(GMapMarker itm)
        {
            MapMarker item = itm as MapMarker;
            if (!formMain.isMarkerMoving)
                formMain.currentMarker = item as MapMarker;

            //если маркер нового маршрута, то вывод расстояия от начала пути
            if (formMain.isCreatingRoute && item.Tag.Type == MarkerTypes.CreatingRoute)
            {
                TrackFile tf = new TrackFile(formMain.creatingRoute.Take(formMain.creatingRoute.IndexOf(item.Tag.Info) + 1));
                tf.CalculateAll();
                double lg = tf.Distance;
                formMain.toolStripLabelFromStart.Text = "От начала пути: " + lg;
            }
            //вывод подсказки (долгота широта имя)
            if (!formMain.isCreatingRoute && (item.Tag.Type != MarkerTypes.WhatThere))
            {
                item.ToolTipText = item.Tag.Info.Name;
                //string.Format(
                //    "{2}\r\n{0},{1}",
                //    item.Position.Lat.ToString("00.000"),
                //    item.Position.Lng.ToString("00.000"),
                //    item.Tag.Info.Name
                //);
            }
            else
            {

            }
        }

        /// <summary>
        /// при уходе мыши с маркера
        /// </summary>
        /// <param name="item"></param>
        internal void OnMarkerLeave(GMapMarker item)
        {
            if (item.Tag != null)
                if ((item.Tag as MapMarker).Tag.Type == MarkerTypes.CreatingRoute)
                    formMain.toolStripLabelFromStart.Text = "";
        }

        internal void MapTypeChanged(GMapProvider type)
        {
            //очистка кэша
            GMaps.Instance.MemoryCache.Clear();

            if (type.MaxZoom == null)
                formMain.gmapControlMap.MaxZoom = 21;
            else
                formMain.gmapControlMap.MaxZoom = (int)type.MaxZoom;
            formMain.gmapControlMap.MinZoom = type.MinZoom;
        }

        /// <summary>
        /// событие движения карты
        /// </summary>
        internal void OnMapDrag()
        {
            formMain.currentMarker = null;
        }

        internal void MouseUp(MouseEventArgs e)
        {
            //Выполняем проверку, какая клавиша мыши была отпущена,
            //если левая то устанавливаем переменной значение false.
            if (e.Button == MouseButtons.Left)
            {
                formMain.isLeftButtonDown = false;

                if (formMain.isMarkerMoving)
                {
                    //если это маркер построения маршрута, то перестраиваем маршрут
                    if (formMain.currentMarker.Tag.Type == MarkerTypes.PathingRoute)
                    {
                        tryPathRoute(formMain.currentMarker.Tag.Info.Name, false);
                    }

                    //добавление информации для отмены действия
                    formMain.LastEditsStack.Push(
                            new StackItem(
                                new MarkerMoveInfo(
                                    formMain.currentMarker.Tag.Info,
                                    formMain.oldMarker, new Action<TrackPoint, TrackPoint>(
                                        (newPt, oldPt) =>
                                        {
                                            if (formMain.creatingRoute != null && formMain.creatingRoute.Contains(newPt))
                                            {
                                                formMain.creatingRoute[formMain.creatingRoute.IndexOf(newPt)] = oldPt;
                                                ShowCreatingRoute(formMain.creatingRouteOverlay, formMain.creatingRoute);
                                            }
                                            if (formMain.rulerRoute != null && formMain.rulerRoute.Contains(newPt))
                                            {
                                                formMain.rulerRoute[formMain.rulerRoute.IndexOf(newPt)] = oldPt;
                                                ShowCreatingRoute(formMain.rulerRouteOverlay, formMain.rulerRoute);
                                            }
                                            if (formMain.waypoints != null && formMain.waypoints.Contains(newPt))
                                            {
                                                formMain.waypoints[formMain.waypoints.IndexOf(newPt)] = oldPt;
                                                ShowWaypoints(formMain.waypoints, formMain.baseOverlay, true);
                                            }
                                        }
                                    )
                                )
                            )
                        );
                    updateUndoButton();
                    formMain.isMarkerMoving = false;
                }

                //обновление информации в списке точек после перемещения
                refreshWaypoints();
                return;
            }

            //вывод контекстного меню если не перемещается карта и мышь не на маршруте
            if (e.Button == MouseButtons.Right && !formMain.gmapControlMap.IsDragging && !formMain.gmapControlMap.IsMouseOverRoute)
            {
                if (!formMain.gmapControlMap.IsMouseOverMarker) //если курсор не на маркере, то открываем контекстное окно карты
                {
                    formMain.pointClicked = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);
                    formMain.contextMenuStripMap.Show(formMain.gmapControlMap, new Point(e.X, e.Y));
                }
                return;
            }
        }

        internal void MouseMove(MouseEventArgs e)
        {
            //если курсор за пределами карты, то выход.
            PointLatLng ll = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);
            if (Math.Abs(ll.Lat) > 90 || Math.Abs(ll.Lng) > 180)
                return;

            //вывод информации о координатах мыши
            Coordinate cr = new Coordinate(formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y));
            formMain.toolStripLabelPosition.Text = cr.ToString("{lat} {lon}", "ddºmm'ss.s\"H");

            //передвижение маркера
            //Проверка, что нажата левая клавиша мыши и не происходит перемещение карты 
            if (formMain.currentMarker != null && //маркер выбран
                e.Button == MouseButtons.Left && formMain.isLeftButtonDown && //левая кнопка мыши нажата
                formMain.currentMarker.Tag.Type != MarkerTypes.WhatThere && //маркер не "что здесь"
                formMain.currentMarker.Tag.Type != MarkerTypes.SearchResult && // маркер не результат поиска
                formMain.currentMarker.Tag.Type != MarkerTypes.WikimpiaCategoryItem //маркер - не категория викимапии
                )
            {

                if (!formMain.isMarkerMoving) //если движение маркера только начинается, то запоминаемего положение для отмены
                    formMain.oldMarker = formMain.currentMarker.Tag.Info.Clone();
                formMain.isMarkerMoving = true;

                PointLatLng point = formMain.gmapControlMap.FromLocalToLatLng(e.X, e.Y);

                //Получение координат маркера.
                formMain.currentMarker.Position = point;
                formMain.currentMarker.Tag.Info.Coordinates = new Coordinate(point);

                //Вывод координат маркера в подсказке.
                formMain.currentMarker.ToolTipText = string.Format("{0},{1}", point.Lat.ToString("00.000"), point.Lng.ToString("00.000"));


                //если создается маршрут, то обновляем длину маршрута
                if (formMain.isCreatingRoute)
                    ShowCreatingRoute(formMain.creatingRouteOverlay, formMain.creatingRoute);
                if (formMain.isRuling)
                    ShowCreatingRoute(formMain.rulerRouteOverlay, formMain.rulerRoute);
                if (formMain.isCreatingPolygon)
                    ShowCreatingPolygon(formMain.creatingPolygonOverlay, formMain.creatingPolygon);
            }

        }

        internal void MouseDown(MouseEventArgs e)
        {
            //Выполняем проверку, какая клавиша мыши была нажата,
            //если левая, то устанавливаем переменной значение true.
            if (e.Button == MouseButtons.Left)
            {
                formMain.isLeftButtonDown = true;
            }

            //сброс текущего маркера
            if (!formMain.gmapControlMap.IsMouseOverMarker)
                formMain.currentMarker = null;
        }


        internal void ButtonClearSearchMarks(object sender, EventArgs e)
        {
            formMain.searchOverlay.Clear();
            //formMain.gmapControlMap.Refresh();
        }

        internal void ButtonFindClick(object sender, EventArgs e)
        {
            //поиск объектов и вывод на карту
            //выход и очистка окна, если пустой запрос
            if (formMain.toolStripComboBoxSearch.Text.Length == 0)
            {
                formMain.toolStripComboBoxSearch.DroppedDown = false;
                formMain.toolStripComboBoxSearch.Items.Clear();
                return;
            }

            //выход, если запрос меньше 4 символов
            if (formMain.toolStripComboBoxSearch.Text.Length < 4)
            {
                MessageBox.Show(formMain, "Длина запроса должна быть более 4-х символов!", "Поиск адресов", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            if (!Vars.Options.Map.LastSearchRequests.Contains(formMain.toolStripComboBoxSearch.Text.ToLower()))
                Vars.Options.Map.LastSearchRequests.Add(formMain.toolStripComboBoxSearch.Text.ToLower());

            //формирование списка точек
            Dictionary<string, Coordinate> adrs = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetAddresses(formMain.toolStripComboBoxSearch.Text);

            //если не нашлось ничего
            if (adrs.Count == 0)
            {
                MessageBox.Show(formMain, "Поиск не дал результатов!", "Поиск адресов", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }

            //добавление точек на карту
            foreach (KeyValuePair<string, Coordinate> kv in adrs)
            {
                bool contains = false;
                foreach (GMapMarker m in formMain.searchOverlay.Markers)
                    if (m.Position == kv.Value.GMap)
                    { contains = true; break; }

                if (!contains) //если такой точки нет, то добавляем
                {
                    TrackPoint t = new TrackPoint(kv.Value)
                    {
                        Name = kv.Key,
                        Icon = IconOffsets.SEARCH_RESULT_ICON
                    };
                    this.ShowWaypoint(t, formMain.searchOverlay, Resources.search_result_icon, MarkerTypes.SearchResult, MarkerTooltipMode.OnMouseOver, false);
                }
            }
            formMain.gmapControlMap.ZoomAndCenterMarkers(formMain.searchOverlayID);
        }

        /// <summary>
        /// Перемещение цетра карты на местоположение устройста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ToolstripButtonLocateDevice(object sender, EventArgs e)
        {
            try
            {
                Coordinate pos = new Geolocation().GetLocation();
                formMain.gmapControlMap.Position = pos.GMap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(formMain, "ПРи определении местоположения произошла ошибка.\r\nПричина: " + ex.Message, "Определение местоположения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        internal void ToolStripComboBoxSearch_DropDown(object sender, EventArgs e)
        {
            formMain.toolStripComboBoxSearch.Items.Clear();
            formMain.toolStripComboBoxSearch.Items.AddRange(Vars.Options.Map.LastSearchRequests.ToArray());
            formMain.toolStripComboBoxSearch.MaxDropDownItems = 6;
        }

        internal void BoxSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                ButtonFindClick(sender, null);
        }

        internal void MouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                formMain.gmapControlMap.Zoom++;
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                formMain.gmapControlMap.Zoom--;
        }

        internal void OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            //выделение по лкм если не линейка и не создание маршрута
            if (e.Button == MouseButtons.Left && !formMain.isCreatingRoute && !formMain.isRuling)
            {
                if (item.Tag != null)
                {
                    TrackFile tf = item.Tag as TrackFile;
                    Vars.currentSelectedTrack = tf;
                    formMain.converterHelper.RefreshData();
                    formMain.pointsHelper.RefreshData();
                    formMain.graphHelper.RefreshData();
                }
                return;
            }

            //контекстное меню по правой кнопке если не линейка и не создание маршрута
            if (e.Button == MouseButtons.Right && !formMain.isCreatingRoute && !formMain.isRuling)
            {
                formMain.routeClicked = item;
                formMain.contextMenuStripRoute.Show(new Point(e.X, e.Y));
                return;
            }

        }

        #endregion

        #region Кнопки

        internal void UndoClick(EventArgs e)
        {
            if (formMain.LastEditsStack.Count > 0)
                formMain.LastEditsStack.Pop().Data.Undo();
            updateUndoButton();
        }

        internal void ZoomIn_Click(EventArgs e)
        {
            formMain.gmapControlMap.Zoom++;
        }

        internal void RulerClick(EventArgs e)
        {
            if (formMain.isCreatingRoute)
            {
                MessageBox.Show("Идет создание маршрута. Завершите Создание перед тем, как использовать этот инструмент");
                return;
            }
            formMain.gmapControlMap.DragButton = System.Windows.Forms.MouseButtons.Right;
            formMain.isRuling = true;
            formMain.rulerRouteOverlay.Clear();
            formMain.rulerRoute = new TrackFile();
            Program.winRouteEditor = new FormEditRoute(
                "Измерениe расстояния",
                formMain.rulerRoute,
                formMain.rulerRouteOverlay,
                null,
                null
                );
            Program.winRouteEditor.Show(Program.winMain);
            ;
        }



        internal void ZoomOutClick(EventArgs e)
        {
            formMain.gmapControlMap.Zoom--;
        }

        /// <summary>
        /// переключение поставщиков карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void MpProvider_Click(object sender, EventArgs e)
        {
            MapProviderRecord mpr = (MapProviderRecord)((ToolStripMenuItem)sender).Tag;
            GMapProvider newProvider = MapProviderRecord.MapProviderToClass(mpr.Enum);

            //обновление провайдера карты независимо от доступности в этом регионе
            formMain.gmapControlMap.MapProvider = newProvider;

            //проверка доступности карты в этом регионе
            //Vars.Options.Map.MapProvider = mpr;
            //GMapProvider provider;
            //if (mpr.MapProviderClass == MapProviderClasses.Retromap)
            //{
            //    Retromap.BaseRetromap nmap = ((Retromap.BaseRetromap)(newProvider));
            //    string MapID = nmap.MapID;
            //    bool exist = Retromap.ServiceEngine.Exist(MapID, formMain.gmapControlMap.Position);
            //    if (!exist)
            //    {
            //        DialogResult dr = MessageBox.Show(formMain, "Этой карты нет в видимой области. Показать весь регион выбранной карты?", "Выбор карты", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            //        switch (dr)
            //        {
            //            case DialogResult.Yes: //центр карты на центр выбранной карты
            //                RectLatLng rect = nmap.Rectangle;
            //                formMain.gmapControlMap.SetZoomToFitRect(rect);
            //                formMain.gmapControlMap.Position = rect.LocationMiddle;
            //                provider = newProvider;
            //                break;
            //            case DialogResult.No: //оставить всё как есть
            //                provider = newProvider;
            //                break;
            //            case DialogResult.Cancel: //отменить выбор карты
            //                provider = formMain.gmapControlMap.MapProvider;
            //                break;
            //            default: throw new Exception("Диалоговое окно вернуло некорректный ответ: " + dr.ToString());
            //        }

            //    }
            //    else
            //        provider = newProvider;
            //}
            //else
            //    provider = newProvider;
            //formMain.gmapControlMap.MapProvider = provider;

            selectDropDownItems<MapProviderRecord>(formMain.toolStripDropDownButtonMapProvider, mpr.ID);
            selectDropDownItems<MapProviderRecord>(formMain.mapProviderToolStripMenuItem, mpr.ID);
        }


        /// <summary>
        /// выделяет пункты меню, карта или слой которого выбран
        /// </summary>
        /// <param name="menu">пункт мен с кнопками карт</param>
        /// <param name="id">id карты, которая сейчас открыта</param>
        /// <typeparam name="T">тип (MapLayerProviderRecord или MapProviderRecord)</typeparam>
        private void selectDropDownItems<T>(ToolStripDropDownItem menu, int id)
            where T : BaseProviderRecord
        {
            ToolStripMenuItem item = null;
            foreach (ToolStripMenuItem ti in menu.DropDownItems)
                if (ti.HasDropDownItems)
                {
                    ti.Checked = false;
                    foreach (ToolStripMenuItem ti2 in ti.DropDownItems)
                    {
                        bool check = (((T)(ti2.Tag)).ID == id);
                        if (check)
                            item = ti2;
                        ti2.Checked = false; //сама кнопка карты                        
                    }
                }
                else
                    ti.Checked = (((T)(ti.Tag)).ID == id);
            if (item != null)
            {
                item.Checked = true;
                (item.OwnerItem as ToolStripMenuItem).Checked = true;
            }
        }

        /// <summary>
        /// событие нажатия на копку переключения слоёв карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void LrProvider_Click(object sender, EventArgs e)
        {
            MapLayerProviderRecord lpr = (MapLayerProviderRecord)((ToolStripMenuItem)sender).Tag;
            switch (lpr.Enum)
            {
                case MapLayerProviders.None:
                case MapLayerProviders.Wikimapia:
                case MapLayerProviders.YandexTraffic:
                case MapLayerProviders.OSMGPSTracks:
                case MapLayerProviders.OSMRailways:
                case MapLayerProviders.OSMRoadSurface:
                case MapLayerProviders.RosreestrCadaster:
                    formMain.gmapControlMap.LayerProvider = lpr.Enum;
                    break;
                default:
                    throw new NotSupportedException("Этот поставщик слоя не поддерживается " + lpr.Enum);
            }
            Vars.Options.Map.LayerProvider = lpr;
            selectDropDownItems<MapLayerProviderRecord>(formMain.layerProviderToolStripMenuItem, lpr.ID);
        }

        /// <summary>
        /// обновление списка карт для этой точки при открытии меню списка поставщиков карт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void MapProviderDropDownOpening(object sender, EventArgs e)
        {
            PointLatLng pos = formMain.gmapControlMap.Position;
            int zoom = (int)(formMain.gmapControlMap.Zoom);
            ToolStripDropDownItem menu = sender as ToolStripDropDownItem;
            bool inet_reach = true;
            foreach (ToolStripMenuItem item in menu.DropDownItems)

                if (item.HasDropDownItems)
                    foreach (ToolStripMenuItem item_map in item.DropDownItems)
                    {
                        MapProviderRecord mpr = item_map.Tag as MapProviderRecord;
                        switch (mpr.MapProviderClass)
                        {
                            case MapProviderClasses.Retromap:
                                try
                                {
                                    bool exist = true;
                                    if (inet_reach)
                                    {
                                        string id = (MapProviderRecord.MapProviderToClass(mpr.Enum) as Retromap.BaseRetromap).MapID;
                                        exist = Retromap.ServiceEngine.Exist(id, pos, zoom);
                                    }
                                    item_map.Visible = exist;
                                }
                                catch (WebException we)
                                {
                                    inet_reach = false;
                                    item_map.Visible = true;
                                }
                                break;
                            case MapProviderClasses.Genshtab:
                            case MapProviderClasses.Google:
                            case MapProviderClasses.OSM:
                            case MapProviderClasses.Yandex:
                                break;
                            case MapProviderClasses.None:
                                break;
                            default: throw new Exception("этот класс карт не реализован");
                        }
                    }
        }

        #endregion

        #region Контекстные меню карты

        internal void ContextMenuMapOpening(object sender, CancelEventArgs e)
        {
            //кнопка очистки маркеров построения маршрутов
            formMain.clearFromtoMarkersToolStripMenuItem.Visible = formMain.fromToOverlay.Markers.Count != 0;

            //кнопка редактирования викимапии
            formMain.wikimapiaToolStripMenuItem.Visible = formMain.gmapControlMap.LayerProvider == MapLayerProviders.Wikimapia;
            if (formMain.gmapControlMap.LayerProvider == MapLayerProviders.Wikimapia)
            {
                formMain.editObjectToolStripMenuItem.Visible = formMain.gmapControlMap.IsMouseOverPolygon;
                formMain.removeObjectToolStripMenuItem.Visible = formMain.gmapControlMap.IsMouseOverPolygon;
            }
        }

        internal void ContextMenuMarkerOpening(object sender, CancelEventArgs e)
        {
            bool canEdit = formMain.markerClicked.Tag.Type != MarkerTypes.WhatThere & formMain.markerClicked.Tag.Type != MarkerTypes.PathingRoute;
            formMain.editMarkerToolStripMenuItem.Visible = canEdit;
        }

        internal void ToolStripDeleteRoute(EventArgs e)
        {
            formMain.converterHelper.DeleteRoute(formMain.routeClicked.Tag as TrackFile);
        }

        internal void ToolStripDeleteMarker(EventArgs e)
        {
            if (MessageBox.Show(formMain, "Вы действительно хотите удалить этот маркер?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                formMain.baseOverlay.Markers.Remove(formMain.markerClicked);
                if (formMain.waypoints != null)
                    formMain.waypoints.Remove(formMain.markerClicked.Tag.Info);

                //если маркер прокладки маршрута, то удаляем его из соответствующей точки
                if (formMain.markerClicked.Tag.Type == MarkerTypes.PathingRoute)
                {
                    formMain.fromToOverlay.Markers.Remove(formMain.markerClicked);
                    switch (formMain.markerClicked.Tag.PathingType)
                    {
                        case PathingType.Start:
                            formMain.fromPoint = null;
                            break;
                        case PathingType.Finish:
                            formMain.toPoint = null;
                            break;
                        case PathingType.Intermed:
                            formMain.IntermediatePoints.Remove(formMain.markerClicked.Tag.Info);
                            break;
                    }
                }
                else
                {
                    updateUndoButton();
                    formMain.LastEditsStack.Push(
                        new StackItem(
                            new MarkerDeleteInfo(formMain.markerClicked.Tag.Info, new Action<TrackPoint>(
                                (oldP) =>
                                {
                                    formMain.waypoints.Add(oldP);
                                    ShowWaypoints(formMain.waypoints, formMain.baseOverlay, false);
                                }
                                )
                                )
                            )
                        );
                    updateUndoButton();
                }
            }
        }

        internal void ToolStripEditRoute(EventArgs e)
        {

            if (formMain.routeClicked.Tag != null)
            {
                TrackFile tf = formMain.routeClicked.Tag as TrackFile;
                if (formMain.Tracks.Contains(tf))
                    formMain.converterHelper.BeginEditRoute(tf);
            }
        }

        internal void ToolStripEditMarker(EventArgs e)
        {
            //запоминание предыдущего тега
            MarkerTag tag = formMain.markerClicked.Tag;


            FormEditPoint fe = new FormEditPoint(tag.Info);
            fe.ShowDialog(formMain);

            //если информация изменилась, то записываем новую
            if (fe.DialogResult == DialogResult.OK)
            {
                formMain.markerClicked.Tag.Info = fe.Result; //запись нофой информации в тег
                DeleteWaypoint(formMain.markerClicked.Tag.Info, formMain.baseOverlay); //удаление маркера со слоя
                ShowWaypoint(formMain.markerClicked.Tag.Info, formMain.baseOverlay, false); //добавление маркера в слой
                updateUndoButton();
                formMain.LastEditsStack.Push(
                    new StackItem(
                        new MarkerEditInfo(fe.Result, tag.Info, new Action<TrackPoint, TrackPoint>(
                            (newP, oldP) =>
                            {
                                if (formMain.waypoints.Contains(newP))
                                {
                                    formMain.waypoints[formMain.waypoints.IndexOf(newP)] = oldP;
                                    ShowWaypoints(formMain.waypoints, formMain.baseOverlay, true);
                                }
                            }
                            )
                    )
                    )

               );
                updateUndoButton();

            }
            //если нет - старую
            else
                formMain.markerClicked.Tag = tag;
        }

        internal void ToolStripCopyCoordinates(EventArgs e)
        {
            Coordinate cr = new Coordinate(formMain.pointClicked);
            Clipboard.SetText(cr.ToString("{lat} {lon}", "ddºmm'ss.s\"H"));
        }

        internal void ToolStripWhatThere(EventArgs e)
        {
            TrackPoint point = new TrackPoint(formMain.pointClicked)
            {
                Icon = IconOffsets.WHAT_THERE
            };
            try
            {
                FormWhatsthere wt = new FormWhatsthere(point);
                wt.Show(Program.winMain);

                //добавление в словарь окон
                if (formMain.ActiveWhatThereForms == null)
                    formMain.ActiveWhatThereForms = new Dictionary<TrackPoint, FormWhatsthere>();
                formMain.ActiveWhatThereForms.Add(point, wt);

                //вывод значка на экран
                ShowWaypoint(point, formMain.whatThereOverlay, Resources.what_there, MarkerTypes.WhatThere, MarkerTooltipMode.OnMouseOver, true);
            }
            catch (Exception we)
            {
                MessageBox.Show(formMain, "Не удалось получить информацию.\r\n" + we.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void ToolStripClearFromToMarkers(EventArgs e)
        {
            formMain.fromToOverlay.Clear();
            formMain.fromPoint = null;
            formMain.toPoint = null;
            formMain.IntermediatePoints = null;
        }

        internal void ToolStripCreateRoute(object sender, EventArgs e)
        {
            tryPathRoute((string)((ToolStripMenuItem)sender).Tag, true);
        }

        internal void ToolStripAddWaypoint(EventArgs e)
        {
            if (formMain.waypoints == null)
                formMain.waypoints = new TrackFile();
            TrackPoint tt = new TrackPoint(formMain.pointClicked);
            FormEditPoint fe = new FormEditPoint(tt);
            if (fe.ShowDialog(formMain) == DialogResult.OK)
            {
                ShowWaypoint(fe.Result, formMain.baseOverlay, true);
                formMain.LastEditsStack.Push(
                    new StackItem(
                        new MarkerAddInfo(fe.Result, new Action<TrackPoint>(
                            (newP) =>
                            {
                                if (formMain.waypoints.Contains(newP))
                                {
                                    formMain.waypoints.Remove(newP);
                                    ShowWaypoints(formMain.waypoints, formMain.baseOverlay, true);
                                }
                            }
                            )
                        )
                    )
                    );
                updateUndoButton();

            }
        }

        /// <summary>
        /// создать новый объект викимапии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ToolStripCreateWikimapiaObject(object sender, EventArgs e)
        {
            //нарисовать периметр объекта
            TrackFile tf = new TrackFile();
            BeginEditPolygon(tf, (track) =>
            {
                //открыть окно редактирования
                Wikimapia.ObjectEditInfo inf = new Wikimapia.ObjectEditInfo()
                {
                    Geometry = track
                };
                new FormEditWikimapiaObject(inf, (obj) =>
                {
                    try
                    {
                        int id = Wikimapia.ServiceEngine.CreateObject(obj);
                        var f = new Wikimapia(null).GetExtInfo(new VectorMapLayerObject(null, "n") { ID = id });
                        VectorMapLayerObject vo = new VectorMapLayerObject(obj.Geometry, f.Title) { ID = id, LayerProvider = MapLayerProviders.Wikimapia };
                        formMain.gmapControlMap.ShowVectorLayerObject(vo);
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(formMain, "Произошла ошибка:\r\n" + ee.Message, "Создание объекта Wikimapia", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }

                }).Show(formMain);
                formMain.isCreatingPolygon = false;
            });

        }

        internal void ToolStripRemoveWikimapiaObject(object sender, EventArgs e)
        {
            formMain.currentMarker = null;
            List<VectorMapLayerObject> objs = formMain.gmapControlMap.GetVectorObjectsUnderCursor();
            if (objs.Count == 0)
                return;
            VectorMapLayerObject obj;
            if (objs.Count == 1)
                obj = objs[0];
            else
            {
                FormChooseVariant fcv = new FormChooseVariant(objs, "Выбор объекта", SelectionMode.One);
                if (fcv.ShowDialog(Program.winMain) == DialogResult.OK)
                {
                    if (fcv.Result.Count != 1) //выход, если выбрано более одного объекта
                        return;
                    obj = objs[fcv.Result[0]];
                }
                else //выход, если выбор объекта отменён
                    return;
            }
            if (MessageBox.Show(formMain, "Вы действительно хотите удалить объект \"" + obj.Name + "\"?", "Удаление объекта", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                Wikimapia.ServiceEngine.RemoveObject(obj.ID);
            return;
        }

        internal void ToolStripEditWikimapiaObject(object sender, EventArgs e)
        {
            formMain.currentMarker = null;
            List<VectorMapLayerObject> objs = formMain.gmapControlMap.GetVectorObjectsUnderCursor();
            if (objs.Count == 0)
                return;
            VectorMapLayerObject obj;
            if (objs.Count == 1)
                obj = objs[0];
            else
            {
                FormChooseVariant fcv = new FormChooseVariant(objs, "Выбор объекта", SelectionMode.One);
                if (fcv.ShowDialog(Program.winMain) == DialogResult.OK)
                {
                    if (fcv.Result.Count != 1) //выход, если выбрано более одного объекта
                        return;
                    obj = objs[fcv.Result[0]];
                }
                else //выход, если выбор объекта отменён
                    return;
            }
            Wikimapia.ObjectEditInfo inf = new Wikimapia.ObjectEditInfo()
            {
                ID = obj.ID
            };
            new FormEditWikimapiaObject(inf, (nobj) =>
            {
                Wikimapia.ServiceEngine.EditObject(nobj.ID, nobj);
            }).Show();
            return;
        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// обновление списка точек после перемещения точки
        /// </summary>
        private void refreshWaypoints()
        {
            if (formMain.waypoints == null)
                formMain.waypoints = new TrackFile();
            formMain.waypoints.Clear();
            foreach (MapMarker mm in formMain.baseOverlay.Markers)
            {
                TrackPoint tt = mm.Tag.Info;
                formMain.waypoints.Add(tt);
            }
        }

        /// <summary>
        /// вывод создаваемого маршрута и его маркеров 
        /// <param name="overlay">слой для вывода (ruler,creating)</param>
        /// <param name="track">маршрут для вывода</param>
        /// </summary>
        public void ShowCreatingRoute(GMapOverlay overlay, BaseTrack track)
        {
            if (overlay.Id != formMain.rulerRouteOverlayID && overlay.Id != formMain.creatingRouteOverlayID)
                throw new ArgumentException("Попытка вывода создаваемого маршрута на чужой слой: " + overlay.Id, "overlay");

            track.CalculateAll();
            overlay.Clear();
            int i = 0;
            foreach (TrackPoint tt in track)
            {
                Icon ic;
                if (i == formMain.selectedRoutePointIndex)
                    ic = Resources.route_point_selected;
                else
                    ic = Resources.route_point;
                ShowWaypoint(tt, overlay, ic, MarkerTypes.CreatingRoute, MarkerTooltipMode.OnMouseOver, false);
                i++;
            }
            track.Color = Color.DarkBlue;
            ShowRoute(track, overlay, false);
            refreshToolTipsCreatingRoute(overlay);
            formMain.toolStripLabelInfo.Text = "Расстояние: " + track.Distance + " км, количество точек: " + track.Count;
        }

        /// <summary>
        /// вывод создаваемого многоугольника
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="track"></param>
        public void ShowCreatingPolygon(GMapOverlay overlay, TrackFile track)
        {
            if (overlay.Id != formMain.creatingPolygonOverlayID)
                throw new ArgumentException("Попытка вывода создаваемого периметра на чужой слой: " + overlay.Id, "overlay");

            if (track.Count == 0)
                return;

            overlay.Clear();
            int i = 0;
            foreach (TrackPoint tt in track)
            {
                Icon ic;
                if (i == formMain.selectedPolygonPointIndex)
                    ic = Resources.route_point_selected;
                else
                    ic = Resources.route_point;
                ShowWaypoint(tt, overlay, ic, MarkerTypes.CreatingRoute, MarkerTooltipMode.OnMouseOver, false);
                i++;
            }
            ShowPolygon(track, overlay, formMain.gmapControlMap.selectedPolygonBrush);
            TrackFile tf = track + track[0]; //добавляем последнюю точку, чтоб полигон был замкнут
            tf.Color = Color.DarkBlue;
            ShowRoute(tf, overlay, false);
        }

        /// <summary>
        /// обновление состояния кнопки отмены действия
        /// </summary>
        private void updateUndoButton()
        {
            formMain.toolStripButtonUndo.Enabled = formMain.LastEditsStack.Count > 0;
            if (formMain.LastEditsStack.Count > 0)
                formMain.toolStripButtonUndo.ToolTipText = "Отменить " + formMain.LastEditsStack.Peek().Text;
            else
                formMain.toolStripButtonUndo.ToolTipText = "Нет доступных действий для отмены";
        }

        /// <summary>
        /// попытка построить маршрут. На вход передается тег нажатой кнопки контекстного меню 
        /// (конец, начало, промежуточная точка) 
        /// если достаточно данных для построения, строится маршрут. Перед этим очищается слой fromToOverlay
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="isAddedNewPoint">если истина, то была добавлена новая точка и надо определить ее тип, 
        /// если ложь, то значит были только передвижения точек  добавлять их не надо
        /// </param>
        private void tryPathRoute(string tag, bool isAddedNewPoint)
        {
            //если была добавлена новая точка, то проверяем, что за точка, добавляем, и пробуем строить
            if (isAddedNewPoint)
            {
                TrackPoint tt = new TrackPoint(formMain.pointClicked);

                if (tag == "from")
                {
                    tt.Icon = IconOffsets.MARKER_START;
                    DeleteWaypoint(formMain.fromPoint, formMain.fromToOverlay);
                    ShowWaypoint(tt, formMain.fromToOverlay, Resources.marker_start, MarkerTypes.PathingRoute, PathingType.Start, MarkerTooltipMode.Never, false, null);
                    tt.Name = "Начало маршрута";
                    formMain.fromPoint = tt;
                }
                if (tag == "to")
                {
                    tt.Icon = IconOffsets.MARKER_FINISH;
                    DeleteWaypoint(formMain.toPoint, formMain.fromToOverlay);
                    ShowWaypoint(tt, formMain.fromToOverlay, Resources.marker_finish, MarkerTypes.PathingRoute, PathingType.Finish, MarkerTooltipMode.Never, false, null);
                    tt.Name = "Конец маршрута";
                    formMain.toPoint = tt;
                }
                if (tag == "intermediate")
                {
                    tt.Icon = IconOffsets.MARKER_INTERMEDIATE;
                    ShowWaypoint(tt, formMain.fromToOverlay, Resources.intermed_point, MarkerTypes.PathingRoute, PathingType.Intermed, MarkerTooltipMode.Never, false, null);
                    if (formMain.IntermediatePoints == null)
                        formMain.IntermediatePoints = new TrackFile();
                    tt.Name = "Промежуточная точка";
                    formMain.IntermediatePoints.Add(tt);
                }
            }

            if (formMain.toPoint != null && formMain.fromPoint != null) //если достаточно точек для построения маршрута, то строим
            {
                if (!isAddedNewPoint) //если новых точек нет, а только передвигали старые, то очищаем старые маршруты
                    formMain.fromToOverlay.Routes.Clear();

                //построение маршрута
                GeoRouter gr = new GeoRouter(Vars.Options.Services.PathRouteProvider);
                TrackFile rt = null;
                try
                { rt = gr.CreateRoute(formMain.fromPoint.Coordinates, formMain.toPoint.Coordinates, formMain.IntermediatePoints); }
                catch (Exception e)
                {
                    MessageBox.Show(formMain, e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    formMain.fromToOverlay.Clear();
                    formMain.fromPoint = null;
                    formMain.toPoint = null;
                    formMain.IntermediatePoints = null;
                    return;
                }
                rt.Color = Vars.Options.Converter.GetColor();
                rt.CalculateAll();
                rt.Name = "Новый маршрут";
                if (Vars.Options.Services.ChangePathedRoute)
                {
                    //если надо открыть маршрут для редактирования
                    BeginEditRoute(rt, (tf) => { formMain.converterHelper.EndEditRouteOrTrip(tf); }, () =>
                    {
                        for (int i = 0; i < formMain.fromToOverlay.Markers.Count;)
                        {
                            object o = formMain.fromToOverlay.Markers[i];
                            MapMarker m = o as MapMarker;
                            if (m.Tag.Type == MarkerTypes.PathingRoute)
                                formMain.fromToOverlay.Markers.Remove(o as GMapMarker);
                            else
                                i++;
                        }
                    });
                }
                else
                {
                    //если не надо открывать маршрут
                    formMain.converterHelper.AddRouteToList(rt);
                    Vars.currentSelectedTrack = rt;
                    formMain.converterHelper.RefreshData();
                    formMain.mapHelper.RefreshData();
                    formMain.pointsHelper.RefreshData();
                    formMain.graphHelper.RefreshData();
                }
                // fromToOverlay.Clear();
                // fromPoint = null;
                // toPoint = null;
                // IntermediatePoints = null;
            }
        }

        /// <summary>
        /// обновление списка маршрутов и выделенного трека и показываемы маршрутов
        /// </summary>
        public void RefreshData()
        {
            DeselectPoints();
            formMain.selectedRouteOverlay.Clear();
            formMain.baseOverlay.Routes.Clear();

            foreach (BaseTrack tf in formMain.showingRoutesList)
                ShowRoute(tf, formMain.baseOverlay, false);

            if (Vars.currentSelectedTrack != null)
                ShowRoute(Vars.currentSelectedTrack, formMain.selectedRouteOverlay, true);

        }


        /// <summary>
        /// отменить выделение всех точек
        /// </summary>
        public void DeselectPoints()
        {
            if (formMain.selectedPointsOverlay != null)
                formMain.selectedPointsOverlay.Markers.Clear();
        }

        /// <summary>
        /// обновление подсказок над маркерами создаваемого маршрута
        /// </summary>
        private void refreshToolTipsCreatingRoute(GMapOverlay overlay)
        {
            if (Vars.Options.Map.ShowAziMarkers)
            {
                foreach (MapMarker item in overlay.Markers)
                {
                    item.ToolTip = new GMapToolTip(item);
                    item.ToolTipMode = MarkerTooltipMode.Always;
                    item.ToolTip.Fill = new SolidBrush(Color.Transparent);
                    item.ToolTip.Foreground = new SolidBrush(Color.Black);
                    item.ToolTip.Stroke = new Pen(Color.Black, 1);
                    item.ToolTip.Font = new Font("Times New Roman", 10, FontStyle.Bold);
                    item.ToolTipText = item.Tag.Info.Distance.ToString("00.000") + " км";
                    item.ToolTipText += !double.IsNaN(item.Tag.Info.TrueAzimuth) ? "\r\nАзимут: " + item.Tag.Info.TrueAzimuth + "º" : "";
                }
            }
        }


        /// <summary>
        /// очистка карты
        /// </summary>
        public void Clear()
        {
            formMain.mainHelper.ToolStripClearAll(null, null);
        }

        /// <summary>
        /// редактирование маршрута
        /// </summary>
        /// <param name="trackFile">редактируемый маршрут</param>
        /// <param name="afterAction">Действие, выполняемое после нажатия кнопки сохранить</param>
        /// <param name="cancelAction">Действие, выполняемое после нажатия кнопки отменить или закрытии окна</param>
        public void BeginEditRoute(TrackFile trackFile, Action<TrackFile> afterAction, Action cancelAction = null)
        {
            if (trackFile == null)
                throw new ArgumentNullException("trackFile не может быть null в FormMap.BeginEditRoute()");

            formMain.gmapControlMap.DragButton = MouseButtons.Right;


            //если идет сздание маршрута, то прерываем
            if (formMain.isCreatingRoute)
                if (MessageBox.Show(formMain, "Уже идет создание или редактирование маршрута, прервать?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    Program.winRouteEditor.Close();
                else
                {
                    if (cancelAction != null)
                        cancelAction.Invoke();
                    return;
                }

            //открываем маршрут для редактирования
            formMain.creatingRoute = trackFile;
            formMain.creatingRouteOverlay.Clear();
            formMain.selectedRouteOverlay.Clear();
            formMain.isCreatingRoute = true;
            Program.winRouteEditor = new FormEditRoute(
                "Редактирование маршрута",
                formMain.creatingRoute,
                formMain.creatingRouteOverlay,
                afterAction,
                cancelAction
                );
            Program.winRouteEditor.Show(formMain);
            ShowCreatingRoute(formMain.creatingRouteOverlay, formMain.creatingRoute);
            refreshToolTipsCreatingRoute(formMain.creatingRouteOverlay);
            formMain.gmapControlMap.ZoomAndCenterRoute(formMain.creatingRouteOverlay.Routes[0]);
        }

        /// <summary>
        /// редактирование маршрута
        /// </summary>
        /// <param name="trackFile">редактируемый маршрут</param>
        /// <param name="afterAction">Действие, выполняемое после нажатия кнопки сохранить</param>
        /// <param name="cancelAction">Действие, выполняемое после нажатия кнопки отменить или закрытии окна</param>
        public void BeginEditTrip(TripRouteFile trackFile, Action<TripRouteFile> afterAction, Action cancelAction = null)
        {
            if (trackFile == null)
                throw new ArgumentNullException("trackFile не может быть null в FormMap.BeginEditTrip()");

            //если идет сздание маршрута, то прерываем
            if (formMain.isCreatingRoute)
                if (MessageBox.Show(formMain, "Уже идет создание или редактирование маршрута, прервать?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    Program.winRouteEditor.Close();
                else
                {
                    if (cancelAction != null)
                        cancelAction.Invoke();
                    return;
                }

            //открываем маршрут для редактирования
            Program.winTripEditor = new FormEditTrip(
                "Редактирование путешествия",
                trackFile,
                afterAction,
                cancelAction
                );
            Program.winTripEditor.Show(Program.winMain);
        }

        /// <summary>
        /// редактирование периметра объекта
        /// </summary>
        /// <param name="trackFile">редактируемый полигон</param>
        /// <param name="afterAction">Действие, выполняемое после нажатия кнопки сохранить</param>
        /// <param name="cancelAction">Действие, выполняемое после нажатия кнопки отменить или закрытии окна</param>
        public void BeginEditPolygon(TrackFile trackFile, Action<TrackFile> afterAction, Action cancelAction = null)
        {
            if (trackFile == null)
                throw new ArgumentNullException("trackFile не может быть null в FormMap.BeginEditPolygon()");

            formMain.gmapControlMap.DragButton = MouseButtons.Right;


            //если идет сздание маршрута, то прерываем
            if (formMain.isCreatingRoute)
                if (MessageBox.Show(formMain, "Уже идет создание или редактирование маршрута, прервать?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    Program.winRouteEditor.Close();
                else
                {
                    if (cancelAction != null)
                        cancelAction.Invoke();
                    return;
                }

            //открываем маршрут для редактирования
            formMain.creatingPolygon = trackFile;
            formMain.creatingPolygonOverlay.Clear();
            formMain.isCreatingPolygon = true;
            Program.winPolygonEditor = new FormEditPolygon(
                "Редактирование периметра объекта",
                formMain.creatingPolygon,
                afterAction,
                cancelAction
                );
            Program.winPolygonEditor.Show(formMain);
            ShowCreatingPolygon(formMain.creatingPolygonOverlay, formMain.creatingPolygon);
            if (formMain.creatingPolygonOverlay.Routes.Count > 0)
                formMain.gmapControlMap.ZoomAndCenterRoute(formMain.creatingPolygonOverlay.Routes[0]);
        }

        /// <summary>
        /// начало выбора точки на карте
        /// </summary>
        /// <param name="after"></param>
        internal void BeginSelectPoint(Action<TrackPoint> after)
        {
            formMain.isSelectingPoint = true;
            formMain.gmapControlMap.DragButton = MouseButtons.Right;
            formMain.gmapControlMap.Cursor = Cursors.Cross;
            if (formMain.isCreatingRoute)
                Program.winRouteEditor.Close();
            formMain.AfterSelectPointAction = after;
        }

        /// <summary>
        /// завершение редактирования путевых точек
        /// </summary>
        /// <param name="wpts"></param>
        public void EndEditWaypoints(BaseTrack wpts)
        {
            formMain.waypoints = wpts as TrackFile;
            this.DeselectPoints();
            this.ShowWaypoints(formMain.waypoints, true, false);
        }

        /// <summary>
        /// выделить определенную точку на карте. 
        /// Перед выделением новой точки предыдущая выделенная точка будет удалена
        /// </summary>
        /// <param name="point">точка, которую надо выделить</param>
        public void SelectPoint(TrackPoint point)
        {
            if (point == null)
                throw new ArgumentNullException("point");
            if (formMain.selectedPointsOverlay == null)
            {
                formMain.selectedPointsOverlay = new GMapOverlay();
            }

            formMain.selectedPointsOverlay.Markers.Clear();
            TrackPoint cop = point.Clone();
            cop.Icon = IconOffsets.SELECTED_POINT_ICON;
            ShowWaypoint(cop, formMain.selectedPointsOverlay, Resources.selected_point, MarkerTypes.SelectedPoint, false);

            //центр на точку
            formMain.gmapControlMap.Position = cop.Coordinates.GMap;
        }
        #endregion
    }
}
