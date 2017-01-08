using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;

namespace TrackConverter.UI.Tools
{

    /// <summary>
    /// окно выбора действий с выделенной областью карты
    /// </summary>
    public partial class FormSaveMap : Form
    {

        private Task worker;
        private bool isCancel = false;
        private int id = -1;

        /// <summary>
        /// создает окно операций с выделенной областью
        /// </summary>
        public FormSaveMap()
        {
            InitializeComponent();

            //поставщик карты
            foreach (MapProviderRecord mpr in Vars.Options.Map.AllProviders)
                comboBoxSaveMapProvider.Items.Add(mpr.Title);

            comboBoxSaveMapProvider.SelectedIndex = Vars.Options.Map.MapProvider.ID;
        }

        /// <summary>
        /// выбор места сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;

            if (sf.ShowDialog(this) == DialogResult.OK)
                textBoxSavePath.Text = sf.FileName;
        }

        /// <summary>
        /// начать операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            isCancel = false;
            id = comboBoxSaveMapProvider.SelectedIndex;
            Action saver = new Action(saver_action);
            worker = new Task(saver);
            worker.Start();
        }

        /// <summary>
        /// сохранение карты в файл
        /// </summary>
        private void saver_action()
        {
            #region подготовка исходных данных

            string fname = textBoxSavePath.Text;
            RectLatLng Area = Program.winMap.gmapControlMap.SelectedArea;
            int Zoom = (int)Vars.Options.Map.Zoom;
            GMapProvider Type;
            MapProviderRecord mpr = MapProviderRecord.FromID(id);
            switch (mpr.Enum)
            {
                case MapProviders.GoogleHybridMap:
                    Type = GMapProviders.GoogleHybridMap;
                    break;
                case MapProviders.GoogleMap:
                    Type = GMapProviders.GoogleMap;
                    break;
                case MapProviders.GoogleSatelliteMap:
                    Type = GMapProviders.GoogleSatelliteMap;
                    break;
                case MapProviders.OpenCycleMap:
                    Type = GMapProviders.OpenCycleMap;
                    break;
                case MapProviders.YandexHybridMap:
                    Type = GMapProviders.YandexHybridMap;
                    break;
                case MapProviders.YandexMap:
                    Type = GMapProviders.YandexMap;
                    break;
                case MapProviders.YandexSatelliteMap:
                    Type = GMapProviders.YandexSatelliteMap;
                    break;
                case MapProviders.WikimapiaMap:
                    Type = GMapProviders.WikiMapiaMap;
                    break;
                default:
                    throw new NotSupportedException("Этот поставщик карты не поддерживается " + Vars.Options.Map.MapProvider.Enum);
            }
            List<GPoint> tileArea = new List<GPoint>();
            tileArea.AddRange(Program.winMap.gmapControlMap.MapProvider.Projection.GetAreaTileList(Area, Zoom, 1));
            tileArea.TrimExcess();

            #endregion

            #region сохранение карты

            if (!Area.IsEmpty)
            {
                // current area
                GPoint topLeftPx = Type.Projection.FromLatLngToPixel(Area.LocationTopLeft, Zoom);
                GPoint rightButtomPx = Type.Projection.FromLatLngToPixel(Area.Bottom, Area.Right, Zoom);
                GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                GSize maxOfTiles = Type.Projection.GetTileMatrixMaxXY(Zoom);

                int padding = 22;
                using (Bitmap bmpDestination = new Bitmap((int)(pxDelta.X + padding * 2), (int)(pxDelta.Y + padding * 2)))
                {
                    using (Graphics gfx = Graphics.FromImage(bmpDestination))
                    {
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.SmoothingMode = SmoothingMode.HighQuality;

                        int i = 0;

                        #region карта

                        lock (tileArea)
                        {
                            foreach (var p in tileArea)
                            {
                                if (isCancel)
                                    return;

                                int pc = (int)(((double)++i / tileArea.Count) * 100);
                                this.Invoke(new Action(() => { progressBar1.Value = pc; Application.DoEvents(); }));

                                foreach (var tp in Type.Overlays)
                                {
                                    Exception ex;
                                    GMapImage tile;

                                    // tile number inversion(BottomLeft -> TopLeft) for pergo maps
                                    if (tp.InvertedAxisY)
                                        tile = GMaps.Instance.GetImageFrom(tp, new GPoint(p.X, maxOfTiles.Height - p.Y), Zoom, out ex) as GMapImage;
                                    else // ok
                                        tile = GMaps.Instance.GetImageFrom(tp, p, Zoom, out ex) as GMapImage;

                                    if (tile != null)
                                        using (tile)
                                        {
                                            long x = p.X * Type.Projection.TileSize.Width - topLeftPx.X + padding;
                                            long y = p.Y * Type.Projection.TileSize.Width - topLeftPx.Y + padding;
                                            gfx.DrawImage(tile.Img, x, y, Type.Projection.TileSize.Width, Type.Projection.TileSize.Height);
                                        }
                                }
                            }
                        }

                        #endregion

                        #region маршруты и маркеры

                        if (checkBoxSaveObjects.Checked)
                        {
                            foreach (GMapOverlay overlay in Program.winMap.gmapControlMap.Overlays)
                                foreach (GMapRoute r in overlay.Routes)
                                    if (r.IsVisible)
                                        using (GraphicsPath rp = new GraphicsPath())
                                        {
                                            for (int j = 0; j < r.Points.Count; j++)
                                            {
                                                var pr = r.Points[j];
                                                GPoint px = Type.Projection.FromLatLngToPixel(pr.Lat, pr.Lng, Zoom);

                                                px.Offset(padding, padding);
                                                px.Offset(-topLeftPx.X, -topLeftPx.Y);

                                                GPoint p2 = px;

                                                if (j == 0)
                                                    rp.AddLine(p2.X, p2.Y, p2.X, p2.Y);
                                                else
                                                {
                                                    System.Drawing.PointF p = rp.GetLastPoint();
                                                    rp.AddLine(p.X, p.Y, p2.X, p2.Y);
                                                }
                                            }

                                            if (rp.PointCount > 0)
                                                gfx.DrawPath(r.Stroke, rp);
                                        }



                            foreach (GMapOverlay overlay in Program.winMap.gmapControlMap.Overlays)
                                foreach (GMapMarker r in overlay.Markers)
                                    if (r.IsVisible)
                                    {
                                        var pr = r.Position;
                                        GPoint px = Type.Projection.FromLatLngToPixel(pr.Lat, pr.Lng, Zoom);

                                        px.Offset(padding, padding);
                                        px.Offset(-topLeftPx.X, -topLeftPx.Y);
                                        px.Offset(r.Offset.X, r.Offset.Y);

                                        gfx.ResetTransform();
                                        gfx.TranslateTransform(-r.LocalPosition.X, -r.LocalPosition.Y);
                                        gfx.TranslateTransform((int)px.X, (int)px.Y);

                                        r.OnRender(gfx);
                                    }
                        }

                        #endregion

                        gfx.ResetTransform();
                    }
                    bmpDestination.Save(fname, ImageFormat.Jpeg);
                }
            }
            else
            {
                MessageBox.Show(this, "Выделите район для сохранения (ALT+ПКМ)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            #endregion

            #region создание файла привязки

            if (checkBoxMapLinkingFile.Checked)
            {

            }

            #endregion
        }


        /// <summary>
        /// отмена действия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (worker.Status == TaskStatus.Running)
                isCancel = true;
            worker.Wait();
            this.Close();
        }
    }
}
