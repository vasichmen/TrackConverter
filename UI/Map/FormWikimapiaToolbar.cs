using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Shell;

namespace TrackConverter.UI.Map
{
    public partial class FormWikimapiaToolbar : Form
    {
        private FormMain frm;
        private GMapOverlay overlay;
        private string overlayId = "wikimapiaCategoriesOverlayId";
        private Wikimapia.CategoryInfo currentCategory = null;
        private Wikimapia wiki_engine = null;
        private ConcurrentDictionary<PointLatLng, object> visibleMarkers;

        /// <summary>
        /// текущая категория для отображения на карте или null
        /// </summary>
        public Wikimapia.CategoryInfo CurrentCategory
        {
            get { return currentCategory; }
            set
            {
                currentCategory = value;
                refreshCategories();
            }
        }

        /// <summary>
        /// создаёт новое окно настроек с заданным основным окном программы
        /// </summary>
        /// <param name="winMain">основное окно</param>
        public FormWikimapiaToolbar(FormMain winMain)
        {
            InitializeComponent();
            frm = winMain;
            wiki_engine = new Wikimapia(Application.StartupPath + Resources.cache_directory + "\\http_cache\\wikimapia\\categories");
            visibleMarkers = new ConcurrentDictionary<PointLatLng, object>();
        }

        /// <summary>
        /// при закрытии формы освобождение ресурсов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formWikimapiaToolbar_FormClosed(object sender, FormClosedEventArgs e)
        {
            //удаление слоя из gmapControl
            frm.gmapControlMap.Overlays.Remove(overlay);

            //удаление событий
            frm.gmapControlMap.MouseUp -= gmapControlMap_MouseUp;
            frm.gmapControlMap.OnMapZoomChanged -= gmapControlMap_OnMapZoomChanged;
            frm.gmapControlMap.SizeChanged -= gmapControlMap_SizeChanged;
        }

        /// <summary>
        /// создание слоя, подписка на события
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formWikimapiaToolbar_Shown(object sender, EventArgs e)
        {
            //добавление слоя поиска и категорий в  gmapControl
            overlay = new GMapOverlay(overlayId);
            frm.gmapControlMap.Overlays.Add(overlay);

            //получение списка категорий
            List<Wikimapia.CategoryInfo> cats;
            try
            {
                cats = new Wikimapia(null).BasicCategories;
            }
            catch (WebException)
            {
                cats = new List<Wikimapia.CategoryInfo>();
            }

            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add("Нет категории");
            comboBoxCategories.Items.AddRange(cats.ToArray());
            comboBoxCategories.SelectedIndex = 0;

            //подключение событий к карте
            frm.gmapControlMap.MouseUp += gmapControlMap_MouseUp;
            frm.gmapControlMap.OnMapZoomChanged += gmapControlMap_OnMapZoomChanged;
            frm.gmapControlMap.SizeChanged += gmapControlMap_SizeChanged;
        }

        #region Категории объектов

        /// <summary>
        /// обновление категорий при изменении выбранной категории из списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            overlay.Markers.Clear();
            if (comboBoxCategories.SelectedIndex == 0)
                CurrentCategory = null;
            else
            {
                Wikimapia.CategoryInfo cat = (Wikimapia.CategoryInfo)comboBoxCategories.Items[comboBoxCategories.SelectedIndex];
                CurrentCategory = cat;
            }
        }

        /// <summary>
        /// добавление категорий в список при изменении текста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxCategories_TextUpdate(object sender, EventArgs e)
        {
            if (comboBoxCategories.Text.Length < 2)
            {
                comboBoxCategories.DroppedDown = false;
                return;
            }
            List<Wikimapia.CategoryInfo> cats = wiki_engine.GetCategories(comboBoxCategories.Text);
            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add("Нет категории");
            comboBoxCategories.Items.AddRange(cats.ToArray());
            comboBoxCategories.SelectionStart = comboBoxCategories.Text.Length;
            comboBoxCategories.DroppedDown = true;
        }

        #endregion

        #region Поиск

        /// <summary>
        /// Нажатие на кнопку поиска объектов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string q = textBoxSearch.Text;
            List<Wikimapia.SearchObjectItemInfo> list = wiki_engine.Search(q, frm.gmapControlMap.Position, (int)frm.gmapControlMap.Zoom, 0);

        }
        

        #endregion

        #region подписанные события GmapControl на основной форме

        /// <summary>
        /// обновление при изменении масштаба
        /// </summary>
        private void gmapControlMap_OnMapZoomChanged()
        {
            this.refreshCategories();
        }

        /// <summary>
        /// обновление пир изменении размера контрола
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_SizeChanged(object sender, EventArgs e)
        {
            this.refreshCategories();
        }

        /// <summary>
        /// обновление при перемещении карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gmapControlMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (frm.gmapControlMap.IsDragging)
                this.refreshCategories();
        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// обновление маркеров категорий на карте
        /// </summary>
        private void refreshCategories()
        {
            if (currentCategory == null)
            {
                overlay.Markers.Clear();
                visibleMarkers.Clear();
                return;
            }

            //удаление маркеров котегорий, которых не видно сейчас на карте
            RectLatLng view = frm.gmapControlMap.GetViewArea();
            removeInvisibleMarkers(view);

            int zoom = (int)frm.gmapControlMap.Zoom;
            Size size = frm.gmapControlMap.Size;
            Wikimapia.CategoryInfo ccat = currentCategory;
            PointLatLng pos = frm.gmapControlMap.Position;

            Action act = new Action(() =>
            {
                List<GPoint> tiles = frm.gmapControlMap.GetVisiblePixelTiles(null, 1024);
                Parallel.ForEach(tiles, (tile) =>
                {
                    List<VectorMapLayerObject> objects = wiki_engine.GetCategoryTile(currentCategory.ID, (int)tile.X, (int)tile.Y, (int)frm.gmapControlMap.Zoom);

                    foreach (var obj in objects)
                    {
                        if (zoom != (int)frm.gmapControlMap.Zoom ||
                            size != frm.gmapControlMap.Size ||
                            ccat != currentCategory ||
                            pos != frm.gmapControlMap.Position) //проверка, что предыдущие параметры остались неизменны
                            return;
                        TrackPoint t = new TrackPoint(obj.GeometryCenter);
                        t.Name = obj.Name;
                        t.Icon = IconOffsets.CATEGORY_MARKER;
                        bool cont = visibleMarkers.ContainsKey(t.GMap);
                        if (view.Contains(t.GMap) && !cont) //если эта точка видна на карте и она ещё не добавлена, то добавляем на карту в базовом потоке
                        {
                            visibleMarkers.TryAdd(t.GMap, null);
                            this.Invoke(new Action(() =>
                           {
                               frm.mapHelper.ShowWaypoint(t, overlay, Resources.category_marker, MarkerTypes.WikimpiaCategoryItem, PathingType.None, MarkerTooltipMode.OnMouseOver, false, obj);
                           }));
                        }
                    }
                }
                );
            });
            new Task(act).Start();
        }

        /// <summary>
        /// удаление всех маркеров со слоя, которые не видны сейчас на карте
        /// </summary>
        /// <param name="view"></param>
        private void removeInvisibleMarkers(RectLatLng view)
        {
            var poss = visibleMarkers.Keys;
            foreach (var pos in poss)
                if (!view.Contains(pos))
                    visibleMarkers.TryRemove(pos, out object val);

            for (int i = 0; i < overlay.Markers.Count;)
            {
                var mark = overlay.Markers[i];
                if (!view.Contains(mark.Position))
                    overlay.Markers.RemoveAt(i);
                else i++;
            }
        }

        #endregion

    }
}
