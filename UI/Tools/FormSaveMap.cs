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
            numericUpDownZoom.Minimum = Program.winMap.gmapControlMap.MinZoom;
            numericUpDownZoom.Maximum = Program.winMap.gmapControlMap.MaxZoom;
            numericUpDownZoom.Value = (int)Program.winMap.gmapControlMap.Zoom;
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
            if (sf.ShowDialog(this) == DialogResult.OK)
                textBoxSavePath.Text = sf.FileName;
            Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
        }

        /// <summary>
        /// начать операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
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
            GMapProvider provider = Program.winMap.gmapControlMap.MapProvider;

            //область сохранения
            RectLatLng Area = Program.winMap.gmapControlMap.SelectedArea;
            List<GPoint> tileArea = new List<GPoint>();
            tileArea.AddRange(Program.winMap.gmapControlMap.MapProvider.Projection.GetAreaTileList(Area, Zoom, 1));
            tileArea.TrimExcess();

            //формат изображения
            ImageFormat format;
            switch (Path.GetExtension(fname))
            {
                case ".jpeg":
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                default: throw new ApplicationException("Формат не поддерживатся");
            }



            #endregion

            #region сохранение карты

            if (!Area.IsEmpty)
            {
                GPoint topLeftPx = provider.Projection.FromLatLngToPixel(Area.LocationTopLeft, Zoom);
                GPoint rightButtomPx = provider.Projection.FromLatLngToPixel(Area.Bottom, Area.Right, Zoom);
                GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                GSize maxOfTiles = provider.Projection.GetTileMatrixMaxXY(Zoom);

                int padding = 22;
                using (Bitmap bmpDestination = new Bitmap((int)(pxDelta.X + padding * 2), (int)(pxDelta.Y + padding * 2)))
                {
                    using (Graphics gfx = Graphics.FromImage(bmpDestination))
                    {
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.SmoothingMode = SmoothingMode.HighQuality;
                        gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

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
                                            long y = p.Y * provider.Projection.TileSize.Width - topLeftPx.Y + padding;
                                            gfx.DrawImage(tile.Img, x, y, provider.Projection.TileSize.Width, provider.Projection.TileSize.Height);
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



                            foreach (GMapOverlay overlay in Program.winMap.gmapControlMap.Overlays)
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
                    makeMapLinkFile(fname, Area, pxDelta);
                if (checkBoxWLinkingFile.Checked)
                    makeWLinkFile(fname, Area, pxDelta);

                #endregion

            }
            else
            {
                MessageBox.Show(this, "Выделите район для сохранения (ALT + ПКМ)", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            #endregion

            this.Invoke(new Action(() => progressBarProgress.Value = 0));
            MessageBox.Show("Карта сохранена!");
        }

        /// <summary>
        /// создание файла привязки .w
        /// </summary>
        /// <param name="fname">адрес файла карты</param>
        /// <param name="Area">область сохраняемой карты</param>
        /// <param name="pxDelta">размер области карты ( X на Y точек)</param>
        private void makeWLinkFile(string fname, RectLatLng Area, GPoint pxDelta)
        {
            string wf = fname + "w";
            using (StreamWriter world = File.CreateText(wf))
            {
                world.WriteLine("{0:0.000000000000}", (Area.WidthLng / pxDelta.X));
                world.WriteLine("0.0000000");
                world.WriteLine("0.0000000");
                world.WriteLine("{0:0.000000000000}", (-Area.HeightLat / pxDelta.Y));
                world.WriteLine("{0:0.000000000000}", Area.Left);
                world.WriteLine("{0:0.000000000000}", Area.Top);
                world.Close();
            }
        }

        /// <summary>
        /// создание файла привязки .map
        /// Описание формата:
        /// http://www.oziexplorer3.com/eng/help/map_file_format.html
        /// </summary>
        /// <param name="fname">адрес файла карты</param>
        /// <param name="Area">область сохраняемой карты</param>
        /// <param name="pxDelta">размер области карты ( X на Y точек)</param>
        private void makeMapLinkFile(string fname, RectLatLng Area, GPoint pxDelta)
        {
            //исходные данные привязки
            string fold = Path.GetDirectoryName(fname);
            string nm = Path.GetFileNameWithoutExtension(fname);
            string mapFile = fold + "\\" + nm + ".map";
            StreamWriter sw = new StreamWriter(mapFile);
            string VMapName = Path.GetFileName(fname);

            string VProjection = "Mercator";
            PureProjection projection = Program.winMap.gmapControlMap.MapProvider.Projection;
            string cCalibrationEmptyStrFormat = "Point{0:00},xy,     ,     ,in, deg,    ,        ,N,    ,        ,W, grid,   ,           ,           ,N\r\n";

            GRect VLocalRect = new GRect();
            VLocalRect.Height = pxDelta.Y;
            VLocalRect.Width = pxDelta.X;


            PointLatLng VLL1 = Area.LocationTopLeft;
            PointLatLng VLL2 = Area.LocationRightBottom;
            PointLatLng VLL = Area.LocationMiddle;

            //заголовок и точки привязки карты
            string text = "OziExplorer Map Data File Version 2.2\r\n" +
                VMapName + "\r\n" +
                VMapName + "\r\n" +
                "1 ,Map Code,\r\n" +
                "WGS 84,,   0.0000,   0.0000,WGS 84\r\n" +
                "Reserved 1\r\n" +
                "Reserved 2\r\n" +
                "Magnetic Variation,,,E\r\n" +
                "Map Projection," + VProjection + ",PolyCal,No,AutoCalOnly,No,BSBUseWPX,No\r\n" +
                GetPointCalibrationStr(1, VLocalRect.Left, VLocalRect.Top, VLL1.Lng, VLL1.Lat) +
                GetPointCalibrationStr(2, (VLocalRect.Right - VLocalRect.Left) / 2, VLocalRect.Top, VLL.Lng, VLL1.Lat) +
                GetPointCalibrationStr(3, VLocalRect.Right, VLocalRect.Top, VLL2.Lng, VLL1.Lat) +
                GetPointCalibrationStr(4, VLocalRect.Left, ((VLocalRect.Bottom - VLocalRect.Top) / 2), VLL1.Lng, VLL.Lat) +
                GetPointCalibrationStr(5, (VLocalRect.Right - VLocalRect.Left) / 2, ((VLocalRect.Bottom - VLocalRect.Top) / 2), VLL.Lng, VLL.Lat) +
                GetPointCalibrationStr(6, VLocalRect.Right, ((VLocalRect.Bottom - VLocalRect.Top) / 2), VLL2.Lng, VLL.Lat) +
                GetPointCalibrationStr(7, VLocalRect.Left, VLocalRect.Bottom, VLL1.Lng, VLL2.Lat) +
                GetPointCalibrationStr(8, (VLocalRect.Right - VLocalRect.Left) / 2, VLocalRect.Bottom, VLL.Lng, VLL2.Lat) +
                GetPointCalibrationStr(9, VLocalRect.Right, VLocalRect.Bottom, VLL2.Lng, VLL2.Lat);

            //пустые точки привязки
            for (int i = 10; i <= 30; i++)
                text += string.Format(cCalibrationEmptyStrFormat, i);

            //масштаб 
            Coordinate lb = new Coordinate(VLL1.Lng, VLL2.Lng);
            Coordinate rb = new Coordinate(VLL2);
            double dist = Vars.CurrentGeosystem.CalculateDistance(lb, rb);
            double pix = pxDelta.X;
            double scale = dist / pix;

            //информация о карте
            text += "Projection Setup,,,,,,,,,,\r\n" +
                 "Map Feature = MF ; Map Comment = MC     These follow if they exist\r\n" +
                 "Track File = TF      These follow if they exist\r\n" +
                 "Moving Map Parameters = MM?    These follow if they exist\r\n" +
                 "MM0,Yes\r\n" +
                 "MMPNUM,4\r\n" +
                 "MMPXY,1," + VLocalRect.Left + "," + VLocalRect.Top + "\r\n" +
                 "MMPXY,2," + VLocalRect.Right + "," + VLocalRect.Top + "\r\n" +
                 "MMPXY,3," + VLocalRect.Right + "," + VLocalRect.Bottom + "\r\n" +
                 "MMPXY,4," + VLocalRect.Left + "," + VLocalRect.Bottom + "\r\n" +
                 "MMPLL,1, " + VLL1.Lng.ToString("00.000000") + ", " + VLL1.Lat.ToString("00.000000") + "\r\n" +
                 "MMPLL,2, " + VLL2.Lng.ToString("00.000000") + ", " + VLL1.Lat.ToString("00.000000") + "\r\n" +
                 "MMPLL,3, " + VLL2.Lng.ToString("00.000000") + ", " + VLL2.Lat.ToString("00.000000") + "\r\n" +
                 "MMPLL,4, " + VLL1.Lng.ToString("00.000000") + ", " + VLL2.Lat.ToString("00.000000") + "\r\n" +
                 "MM1B," + scale.ToString().Replace(',', '.') + "\r\n" +
                 "MOP,Map Open Position,0,0\r\n" +
                 "IWH,Map Image Width/Height," + VLocalRect.Right + "," + VLocalRect.Bottom + "\r\n";

            sw.Write(text);
            sw.Close();
        }

        private string GetPointCalibrationStr(int APointID, long X, long Y, double longi, double lati)
        {
            Coordinate coord = new Coordinate(lati, longi);
            string cCalibrationStrFormat = "Point{0:00},xy,    {1}, {2},in, deg, {3}, {4}, grid,   ,           ,           ,N\r\n";
            string lon = coord.Longitude.ToString("dd, mm.mmmm,H");
            string lat = coord.Latitude.ToString("dd, mm.mmmm,H");
            return string.Format(cCalibrationStrFormat, APointID, X, Y, lat, lon);
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
            this.Close();
        }

        private void comboBoxSaveMapProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            Program.winMap.gmapControlMap.MapProvider = provider;
        }
    }
}
