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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Geodesy.MapCalibrations;

namespace TrackConverter.UI.Tools
{

    /// <summary>
    /// окно выбора действий с выделенной областью карты
    /// </summary>
    public partial class FormSaveMap : Form
    {
        private Task worker;
        private bool isCancel = false;
        private GMapControl gmc;

        /// <summary>
        /// создает окно операций с выделенной областью
        /// </summary>
        public FormSaveMap()
        {
            InitializeComponent();

            //поставщик карты
            foreach (MapProviderRecord mpr in Vars.Options.Map.AllMapProviders)
                comboBoxSaveMapProvider.Items.Add(mpr.Title);
            comboBoxSaveMapProvider.SelectedIndex = Vars.Options.Map.MapProvider.ID;
            numericUpDownZoom.Minimum = Program.winMain.gmapControlMap.MinZoom;
            numericUpDownZoom.Maximum = Program.winMain.gmapControlMap.MaxZoom;
            numericUpDownZoom.Value = (int)Program.winMain.gmapControlMap.Zoom;
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
            sf.AddExtension = true;
            sf.Filter = "Файл JPEG (*.jpg)|*.jpg";
            sf.Filter += "|Точечный рисунок BMP (*.bmp)|*.bmp";
            sf.Filter += "|Рисунок TIFF (*.tiff)|*.tiff";
            if (sf.ShowDialog(this) == DialogResult.OK)
            {
                textBoxSavePath.Text = sf.FileName;
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
            }
        }

        /// <summary>
        /// начать операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonCancelClose.Click -= buttonClose_Click;
            buttonCancelClose.Click += buttonCancel_Click;
            buttonCancelClose.Text = "Отмена";
            resetInterface();

            //поставщик карты
            int id = comboBoxSaveMapProvider.SelectedIndex;
            MapProviderRecord mpr = MapProviderRecord.FromID(id);
            GMapProvider provider;
            switch (mpr.Enum)
            {
                case MapProviders.GoogleHybridMap:
                    provider = GMapProviders.GoogleHybridMap;
                    break;
                case MapProviders.GoogleMap:
                    provider = GMapProviders.GoogleMap;
                    break;
                case MapProviders.GoogleSatelliteMap:
                    provider = GMapProviders.GoogleSatelliteMap;
                    break;
                case MapProviders.OpenCycleMap:
                    provider = GMapProviders.OpenCycleMap;
                    break;
                case MapProviders.YandexHybridMap:
                    provider = GMapProviders.YandexHybridMap;
                    break;
                case MapProviders.YandexMap:
                    provider = GMapProviders.YandexMap;
                    break;
                case MapProviders.YandexSatelliteMap:
                    provider = GMapProviders.YandexSatelliteMap;
                    break;
                case MapProviders.WikimapiaMap:
                    provider = GMapProviders.WikiMapiaMap;
                    break;
                default:
                    throw new NotSupportedException("Этот поставщик карты не поддерживается " + Vars.Options.Map.MapProvider.Enum);
            }

            GMapControl parent = Program.winMain.gmapControlMap;
            gmc = new GMapControl();
            gmc.CacheLocation = parent.CacheLocation;
            gmc.SelectedArea = parent.SelectedArea;
            gmc.MapProvider = provider;
            gmc.Zoom = (int)this.numericUpDownZoom.Value;
            foreach (GMapOverlay ov in parent.Overlays)
                gmc.Overlays.Add(ov);

            isCancel = false;
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

            if (string.IsNullOrWhiteSpace(textBoxSavePath.Text))
                return;

            //имя файла
            string fname = textBoxSavePath.Text;

            //масштаб
            int Zoom = (int)numericUpDownZoom.Value;

            //поставщик карты
            GMapProvider provider = gmc.MapProvider;

            //область сохранения
            RectLatLng Area = gmc.SelectedArea;
            List<GPoint> tileArea = new List<GPoint>();
            tileArea.AddRange(provider.Projection.GetAreaTileList(Area, Zoom, 1));
            tileArea.TrimExcess();

            //формат изображения
            ImageFormat format;
            switch (Path.GetExtension(fname).ToLower())
            {
                case ".jpeg":
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".tiff":
                    format = ImageFormat.Tiff;
                    break;
                default: throw new ApplicationException("Формат не поддерживатся");
            }



            #endregion

            #region сохранение карты

            if (!Area.IsEmpty)
            {
                GPoint topLeftPx = provider.Projection.FromLatLngToPixel(Area.LocationTopLeft, Zoom);
                GPoint rightButtomPx = provider.Projection.FromLatLngToPixel(Area.LocationRightBottom, Zoom);
                GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                GSize maxOfTiles = provider.Projection.GetTileMatrixMaxXY(Zoom);

                int padding = 0;
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
                                this.Invoke(new Action(() => { progressBarProgress.Value = pc; Application.DoEvents(); }));

                                foreach (var tp in provider.Overlays)
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
                                            long x = p.X * provider.Projection.TileSize.Width - topLeftPx.X + padding;
                                            long y = p.Y * provider.Projection.TileSize.Height - topLeftPx.Y + padding;
                                            gfx.DrawImage(tile.Img, x, y, provider.Projection.TileSize.Width, provider.Projection.TileSize.Height);
                                        }
                                }
                            }
                        }

                        #endregion

                        #region маршруты и маркеры

                        if (checkBoxSaveObjects.Checked)
                        {
                            foreach (GMapOverlay overlay in gmc.Overlays)
                                foreach (GMapRoute r in overlay.Routes)
                                    if (r.IsVisible)
                                        using (GraphicsPath rp = new GraphicsPath())
                                        {
                                            for (int j = 0; j < r.Points.Count; j++)
                                            {
                                                var pr = r.Points[j];
                                                GPoint px = provider.Projection.FromLatLngToPixel(pr.Lat, pr.Lng, Zoom);

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



                            foreach (GMapOverlay overlay in gmc.Overlays)
                                foreach (GMapMarker r in overlay.Markers)
                                    if (r.IsVisible)
                                    {
                                        var pr = r.Position;
                                        GPoint px = provider.Projection.FromLatLngToPixel(pr.Lat, pr.Lng, Zoom);

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
                    bmpDestination.Save(fname, format);
                }


                #region создание файла привязки

                if (checkBoxMapLinkingFile.Checked)
                    MapCalibrationOzi.CreateMapCalibrationFile(fname, gmc.MapProvider.Projection, Area, pxDelta, Zoom, topLeftPx);
                if (checkBoxWLinkingFile.Checked)
                    MapCalibrationWorld.CreateMapCalibrationFile(fname, Area, pxDelta);

                #endregion

            }
            else
            {
                MessageBox.Show(this, "Выделите район для сохранения (ALT + ПКМ)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            #endregion

            this.Invoke(new Action(() =>
            {
                progressBarProgress.Value = 0;
                resetInterface();
                this.buttonCancelClose.Text = "Закрыть";
                this.buttonCancelClose.Click -= buttonCancel_Click;
                this.buttonCancelClose.Click += buttonClose_Click;
            }));
            MessageBox.Show("Карта сохранена!");
        }

        /// <summary>
        /// отмена действия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (worker != null && worker.Status == TaskStatus.Running)
            {
                isCancel = true;
                worker.Wait();
            }
            this.buttonCancelClose.Text = "Закрыть";
            this.buttonCancelClose.Click -= buttonCancel_Click;
            this.buttonCancelClose.Click += buttonClose_Click;
            resetInterface();
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// переключение интерфейса
        /// </summary>
        private void resetInterface()
        {
            this.textBoxSavePath.Enabled = !textBoxSavePath.Enabled;
            this.comboBoxSaveMapProvider.Enabled = !comboBoxSaveMapProvider.Enabled;
            this.numericUpDownZoom.Enabled = !numericUpDownZoom.Enabled;
            this.buttonSelectSaveFile.Enabled = !buttonSelectSaveFile.Enabled;
            this.checkBoxMapLinkingFile.Enabled = !checkBoxMapLinkingFile.Enabled;
            this.checkBoxSaveObjects.Enabled = !checkBoxSaveObjects.Enabled;
            this.checkBoxWLinkingFile.Enabled = !checkBoxWLinkingFile.Enabled;
            this.buttonStart.Enabled = !buttonStart.Enabled;
        }

    }
}
