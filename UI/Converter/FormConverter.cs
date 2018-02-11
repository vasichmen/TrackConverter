using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Mathematic.Assessment;
using TrackConverter.Lib.Mathematic.Astronomy;
using TrackConverter.Lib.Tracking;
using TrackConverter.UI.Common;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Map;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI.Converter
{
    /// <summary>
    /// основное окно
    /// </summary>
    public partial class FormConverter : Form
    {
        /// <summary>
        /// список загруженных маршрутов
        /// </summary>
        public TrackFileList Tracks;

        /// <summary>
        /// список маршрутов, выводимый на карте постоянной
        /// </summary>
        public TrackFileList showingRoutesList;

        /// <summary>
        /// список путевый точек, выводимых на карте постоянно
        /// </summary>
        private TrackFileList showingWaypointsList;


        /// <summary>
        /// открываемый файл при запуске приложения (для асинхронной загрузки файла)
        /// </summary>
        private string openingFile = null;

        /// <summary>
        /// создает объект FormConverter
        /// </summary>
        public FormConverter()
        {
            InitializeComponent();
            Tracks = new TrackFileList();
            showingRoutesList = new TrackFileList();
            showingWaypointsList = new TrackFileList();
            RefreshRecentFiles(); //обновление писка последних загруженных файлов
        }


        #region события

        #region форма

        /// <summary>
        /// закрытие окна и сохранение настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormConverter_FormClosed(object sender, FormClosedEventArgs e)
        {
            Vars.Options.Converter.LastLoadedTracks = this.Tracks.FilePaths;
        }

        /// <summary>
        /// перевод фокуса на форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormConverter_Load(object sender, EventArgs e)
        {
#if (DEBUG)
            this.button1.Visible = true;
#endif

            //открытие последних файлов
            if (this.Tracks == null || this.Tracks.Count == 0)
            {
                this.Tracks = Serializer.DeserializeTrackFileList(Vars.Options.Converter.LastLoadedTracks);
                RefreshData();
            }


            if (this.Tracks.Count != 0)
            {
                Vars.currentSelectedTrack = Tracks[0];
                Program.RefreshWindows(this);
            }

            //завершение открытия файлов, при запуске программы
            if (openingFile != null)
            {
                Task ts = new Task(new Action(() =>
                {
                    BaseTrack pts = null;
                    Program.winMain.BeginOperation();
                    //метод обновления информации о выполняемой операции

                    pts = Serializer.DeserializeTrackFile(openingFile, Program.winMain.setCurrentOperation);
                    openingFile = null;
                    //обработка результатов
                    Program.winMap.Invoke(new Action(() =>
                    {
                        Program.winMain.EndOperation();
                        Program.winMap.Clear();
                        Program.winMap.ShowWaypoints(pts, false, false);
                    }));
                }));
                ts.Start();
            }
        }

        #endregion

        #region меню

        #region загрузка маршрутов
        /// <summary>
        /// загрузка файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRouteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog of = new OpenFileDialog();
                of.Filter = "Все поддерживаемые форматы(*.trr, *.crd, *.wpt, *.plt, *.rt2, *.kml, *.kmz, *.gpx, *.rte, *.osm, *.nmea, *.csv, *.txt)| *.trr; *.crd; *.wpt; *.plt; *rt2; *.kml; *.kmz; *.gpx; *.rte; *.osm; *.nmea; *.csv; *.txt";
                of.Filter += "|Треки Androzic (*.plt)|*.plt";
                of.Filter += "|Маршрут Androzic (*.rt2)|*.rt2";
                of.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
                of.Filter += "|Файл координат(*.crd)|*.crd";
                of.Filter += "|Файл Google Earth(*.kml)|*.kml";
                of.Filter += "|Файл Google Earth(*.kmz)|*.kmz";
                of.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
                of.Filter += "|Файл маршрутов OziExplorer(*.rte)|*.rte";
                of.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
                of.Filter += "|Файл NMEA(*.nmea)|*.nmea";
                of.Filter += "|Файл Excel(*.csv)|*.csv";
                of.Filter += "|Текстовый файл(*.txt)|*.txt";
                of.Filter += "|Маршрут Track Converter(*.trr)|*.trr";

                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = true;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == DialogResult.OK)
                    foreach (string fn in of.FileNames)
                        OpenFile(fn);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        /// <summary>
        /// загрузка по ссылке Яндекс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLinkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormReadText of = new FormReadText(DialogType.ReadText, "Введите ссылку на маршрут: ", "", false, false, false, false);
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenLink(of.Result);
                }
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        #endregion

        #region экспорт маршрутов

        /// <summary>
        /// Сохранение маршрута в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            //если кнопка контекстного меню
            if (((ToolStripMenuItem)sender).Name == "saveFileContextToolStripMenuItem1" || ((ToolStripMenuItem)sender).Name == "saveToolStripMenuItem1")
                if (dataGridView1.SelectedRows.Count > 1) //если выделено больше одного маршрута, то сохранение в один файл
                {
                    saveAllInOneFileToolStripMenuItem_Click(sender, e);
                    return;
                }
                else
                    tf = Tracks[dataGridView1.SelectedRows[0].Index];
            else
            {//если нажата кнопка главного меню
                if (Tracks.Count > 1)
                {
                    MessageBox.Show(this, "Для сохранения в один файл должен быть загружен только один маршрут! Если требуется сохранить несколько маршрутов в один файл, используйте соответствующий пункт меню.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Tracks.Count == 0)
                {
                    MessageBox.Show(this, "Для сохранения сначала надо загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = Tracks[0];
            }


            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Файл маршрута TrackConverter(*.trr)|*.trr";
            sf.Filter += "|Файл маршрута Androzic (*.rt2)|*.rt2";
            sf.Filter += "|Треки Androzic (*.plt)|*.plt";
            sf.Filter += "|Путевые точки Ozi(*.wpt)|*.wpt";
            sf.Filter += "|Файл координат(*.crd)|*.crd";
            sf.Filter += "|Файл координат(*.kml)|*.kml";
            sf.Filter += "|Файл GPS координат(*.gpx)|*.gpx";
            sf.Filter += "|Файл координат(*.kmz)|*.kmz";
            sf.Filter += "|Файл OpenStreetMaps(*.osm)|*.osm";
            sf.Filter += "|Файл NMEA(*.nmea)|*.nmea";
            sf.Filter += "|Файл Excel(*.csv)|*.csv";
            sf.Filter += "|Файл нескольких маршрутов Ozi(*.rte)|*.rte";
            sf.Filter += "|Текстовый файл(*.txt)|*.txt";
            sf.Filter += "|Список адресов(*.adrs)|*.adrs";

            sf.AddExtension = true;
            sf.FileName = tf.Name;
            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack;

            //sf.FileName = tf.Name; //при сохранении как файл не записывать сохранённый путь


            if (sf.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(tf.FilePath))
                    tf.FilePath = sf.FileName;
                switch (Path.GetExtension(sf.FileName).ToLower())
                {
                    case ".rt2":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.Rt2File);
                        break;
                    case ".plt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.PltFile);
                        break;
                    case ".wpt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.WptFile);
                        break;
                    case ".crd":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CrdFile);
                        break;
                    case ".kml":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmlFile);
                        break;
                    case ".gpx":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.GpxFile);
                        break;
                    case ".kmz":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmzFile);
                        break;
                    case ".osm":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.OsmFile);
                        break;
                    case ".nmea":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.NmeaFile);
                        break;
                    case ".csv":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CsvFile);
                        break;
                    case ".rte":
                        if (tf.GetType() == typeof(TripRouteFile))
                            Serializer.Serialize(sf.FileName, (tf as TripRouteFile).DaysRoutes, FileFormats.RteFile);
                        else
                            Serializer.Serialize(sf.FileName, new TrackFileList() { tf }, FileFormats.RteFile);
                        break;
                    case ".txt":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.TxtFile);
                        break;
                    case ".adrs":
                        Program.winMain.BeginOperation();
                        Task ts = new Task(new Action(() =>
                            {
                                Serializer.Serialize(sf.FileName, tf, FileFormats.AddressList, Program.winMain.setCurrentOperation);
                                Program.winMain.EndOperation();
                            }));
                        ts.Start();
                        break;
                    case ".trr":
                        Serializer.Serialize(sf.FileName, tf, FileFormats.TrrFile);
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack = sf.FilterIndex;
            }
        }

        /// <summary>
        /// загрузка файлов при запусеке программы из параметров
        /// </summary>
        /// <param name="args">параметры командной строки</param>
        internal void LoadFiles(string[] args)
        {
            try
            {
                if (args.Length != 0)
                    foreach (string arg in args)
                        if (File.Exists(arg))
                        {
                            string ext = Path.GetExtension(arg).ToLower();
                            switch (ext)
                            {
                                case ".trr":
                                    TripRouteFile trr = Serializer.DeserializeTripRouteFile(arg);
                                    this.AddRouteToList(trr);
                                    RefreshData();
                                    break;
                                case ".kml":
                                case ".kmz":
                                    GeoFile gf = Serializer.DeserializeGeoFile(arg);
                                    this.Tracks = gf.Routes;
                                    Program.winMap.Clear();
                                    Program.winMap.ShowWaypoints(gf.Waypoints, false, false);
                                    RefreshData();
                                    break;
                                case ".wpt":
                                case ".adrs":
                                    openingFile = arg;
                                    break;
                                default:
                                    OpenFile(arg);
                                    break;
                            }

                            Vars.currentSelectedTrack = this.Tracks.Count > 0 ? Tracks[0] : null;
                            Program.RefreshWindows(this);
                        }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit(new CancelEventArgs(true));
            }
        }

        /// <summary>
        /// Сохранение маршрута в яндекс- ссылку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveYandexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            if ((string)(((ToolStripMenuItem)sender).Name) == "saveYandexContextToolStripMenuItem")
                tf = Tracks[dataGridView1.SelectedCells[0].RowIndex];
            else
            {
                if (Tracks.Count > 1)
                {
                    MessageBox.Show(this, "Для сохранения в ссылку должен быть загружен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Tracks.Count == 0)
                {
                    MessageBox.Show(this, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = Tracks[0];
            }
            FormReadText of = new FormReadText(DialogType.ReadNumber, "Введите количество точек в результате, всего: " + tf.Count, "", true, false, false, false);

            int count = 0;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                count = Convert.ToInt32(of.Result);
            }
            else
            {
                MessageBox.Show(this, "Надо указать количество точек в ссылке!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string res = Serializer.Serialize(count.ToString(), tf, FileFormats.YandexLink);

            if (!(res.Count<char>() > 64000))
                new FormReadText(DialogType.ExportText, "Ссылка на маршрут", res, false, true, true, true).ShowDialog();
            else
            {
                MessageBox.Show(this, "Слишком длинный маршрут для вывода в формате ссылки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }



        /// <summary>
        /// сохранение как ссылка на викимапию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveWikimapiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseTrack tf = null;

            if ((string)(((ToolStripMenuItem)sender).Name) == "saveWikimapiaContextToolStripMenuItem")
                tf = Tracks[dataGridView1.SelectedCells[0].RowIndex];
            else
            {
                if (Tracks.Count > 1)
                {
                    MessageBox.Show(this, "Для сохранения в ссылку должен быть загружен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Tracks.Count == 0)
                {
                    MessageBox.Show(this, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tf = Tracks[0];
            }

            FormReadText of = new FormReadText(DialogType.ReadNumber, "Введите количество точек в результате, всего: " + tf.Count, "", true, false, false, false);

            int count = 0;
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                count = Convert.ToInt32(of.Result);
            }
            else
            {
                MessageBox.Show(this, "Надо указать количество точек в ссылке!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string res = Serializer.Serialize(count.ToString(), tf, FileFormats.WikimapiaLink);

            if (!(res.Count<char>() > 64000))
                new FormReadText(DialogType.ExportText, "Ссылка на маршрут", res, false, true, true, true).ShowDialog();
            else
            {
                MessageBox.Show(this, "Слишком длинный маршрут для вывода в формате ссылки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// сохранение всех загруженных саршрутов в один файл RTE, KML, KMZ
        /// </summary>
        /// <param name="sender">кнопка основного меню или контекстного</param>
        /// <param name="e"></param>
        private void saveAllInOneFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Tracks.Count == 0)
            {
                MessageBox.Show(this, "Для сохранения сначала загрузить маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //выбор элементов для сохранения
            TrackFileList tfs;

            //если контекстное меню, то выбираем выделенные маршруты
            if (sender.GetType() == typeof(ToolStripMenuItem))
            {
                if (dataGridView1.SelectedRows.Count < 1)
                {
                    MessageBox.Show(this, "Для сохранения сначала выделите маршруты!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                tfs = new TrackFileList();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    tfs.Add(Tracks[row.Index]);
            }
            //если главное меню, выбираем все загруженные маршруты
            else
                tfs = Tracks;

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter += "Файл маршрутов OziExplorer(*.rte)|*.rte";
            sf.Filter += "| Файл Google Earth (*.kml)|*.kml";
            sf.Filter += "| Архивированный файл Google Earth (*.kmz)|*.kmz";
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveAllInOne;
            sf.AddExtension = true;
            sf.InitialDirectory = Application.StartupPath;
            sf.FileName = tfs.Count > 0 ? tfs[0].Name : "";

            if (sf.ShowDialog() == DialogResult.OK)
            {
                if (Program.winWaitingNullOrDisposed)
                    Program.winWaiting = new FormWaiting();

                Program.winWaiting.Show(this);
                Program.winWaiting.Focus();
                try
                {
                    Task pr = new Task(new Action(() =>
                    {
                        switch (Path.GetExtension(sf.FileName).ToLower())
                        {
                            case ".rte":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.RteFile);
                                break;
                            case ".kml":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.KmlFile);
                                break;
                            case ".kmz":
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.KmzFile);
                                break;
                        }


                    }));
                    pr.Start();
                    pr.Wait();
                    Vars.Options.Common.LastSaveExtensionNumberSaveAllInOne = sf.FilterIndex;
                    MessageBox.Show("Файл сохранен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    Program.winWaiting.Visible = false;
                }

            }
        }

        /// <summary>
        /// сохранение всех машрутов в отдельные файлы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAllSeparateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReadText rt = new FormReadText(DialogType.ReadExtension, "Bведите формат файла (wpt,crd,plt,rt2,gpx,kml,kmz,osm,nmea,txt,csv,rte)", Vars.Options.Common.IsExtension ? Vars.Options.Common.LastSaveSeparateExtension : "", false, false, false, false);
            if (rt.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.ShowNewFolderButton = true;

                if (Vars.Options.Common.IsSaveDir)
                    fb.SelectedPath = Vars.Options.Common.LastFileSaveDirectory;
                if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FormWaiting fw = new FormWaiting();
                    fw.Show(this);
                    ParallelLoopResult pr = Parallel.ForEach<BaseTrack>(Tracks, new Action<BaseTrack>((BaseTrack tf) =>
                    {
                        string nm = "";
                        if (string.IsNullOrEmpty(tf.Name))
                            if (string.IsNullOrEmpty(tf.FileName))
                                nm = Guid.NewGuid().ToString();
                            else
                                nm = tf.FileName;
                        else
                            nm = tf.Name;

                        string basename = fb.SelectedPath + "\\" + nm;
                        switch (rt.Result)
                        {
                            case "wpt":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.WptFile);
                                break;
                            case "crd":
                                Serializer.Serialize(basename + ".crd", tf, FileFormats.CrdFile);
                                break;
                            case "plt":
                                Serializer.Serialize(basename + ".plt", tf, FileFormats.PltFile);
                                break;
                            case "rt2":
                                Serializer.Serialize(basename + ".rt2", tf, FileFormats.Rt2File);
                                break;
                            case "gpx":
                                Serializer.Serialize(basename + ".gpx", tf, FileFormats.GpxFile);
                                break;
                            case "kml":
                                Serializer.Serialize(basename + ".kml", tf, FileFormats.KmlFile);
                                break;
                            case "kmz":
                                Serializer.Serialize(basename + ".kmz", tf, FileFormats.KmzFile);
                                break;
                            case "osm":
                                Serializer.Serialize(basename + ".osm", tf, FileFormats.OsmFile);
                                break;
                            case "nmea":
                                Serializer.Serialize(basename + ".nmea", tf, FileFormats.NmeaFile);
                                break;
                            case "csv":
                                Serializer.Serialize(basename + ".csv", tf, FileFormats.CsvFile);
                                break;
                            case "txt":
                                Serializer.Serialize(basename + ".txt", tf, FileFormats.TxtFile);
                                break;
                            case "rte":
                                Serializer.Serialize(basename + ".rte", tf, FileFormats.RteFile);
                                break;
                            default:
                                MessageBox.Show(this, "Данный формат не поддерживается: " + rt.Result, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                        }
                        Vars.Options.Common.LastSaveSeparateExtension = rt.Result;
                    }));
                    while (!pr.IsCompleted)
                        Thread.Sleep(200);
                    fw.Close();
                }
            }
            MessageBox.Show("Файлы сoхранены");
        }

        #endregion

        /// <summary>
        /// очистка списка загруженных машрутов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tracks.Clear();
            dataGridView1.DataSource = Tracks.Source;
            dataGridView1.Refresh();
            SaveToolStripMenuItem.Enabled = false;
            Program.winMap.Clear();
            Program.winPoints.Clear();
            Program.winElevVisual.Clear();
        }


        /// <summary>
        /// Выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region  datagridview

        #region контекстное меню

        /// <summary>
        /// отмена вывода меню, если не выделено ни одного маршрута.
        /// Выбор подходящих пунктов меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStripDGW_Opening(object sender, CancelEventArgs e)
        {
            //если все выделенные маршрты - TrackFile, то добавление пункта "преобразовать в путешествие и объединить в путешествие"
            bool allTF = true;
            foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
                allTF &= Tracks[dr.Index].GetType() == typeof(TrackFile);
            if (allTF)
            {
                toTripRouteFileToolStripMenuItem.Visible = true;
                joinToTripRouteToolStripMenuItem.Visible = dataGridView1.SelectedRows.Count > 1; //объединение только если больше одного маршрута
            }
            else
            {
                toTripRouteFileToolStripMenuItem.Visible = false;
                joinToTripRouteToolStripMenuItem.Visible = false;
            }

            //если один выделенный маршрут - путешествие, то переименовываем кнопку редактирования
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int ind = dataGridView1.SelectedRows[0].Index;
                BaseTrack bt = Tracks[ind];
                if (bt.GetType() == typeof(TripRouteFile))
                    editRouteMapToolStripMenuItem.Text = "Редактировать путешествие";
                else
                    editRouteMapToolStripMenuItem.Text = "Редактировать на карте";
            }

            //подготовка кнопки "открыть в проводнике"
            openRouteFolderToolStripMenuItem.Visible = true;
            foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
                openRouteFolderToolStripMenuItem.Visible &= File.Exists(Tracks[dr.Index].FilePath);


            //если элемент - первое контектное меню, то выбираем пункты на основе тегов
            if (sender.GetType() == typeof(ContextMenuStrip))
            {
                ContextMenuStrip menu = (ContextMenuStrip)sender;
                //если не выделено ни одного элемента, то не выводим меню
                if (dataGridView1.SelectedRows.Count < 1)
                {
                    e.Cancel = true;
                    return;
                }
                prepareToolStripMenuItemList(menu.Items);
            }

            //если элемент - пункт с дочерними элементами, то выбираем пункты
            if (sender.GetType() == typeof(ToolStripMenuItem))
            {
                ToolStripMenuItem menu = (ToolStripMenuItem)sender;
                prepareToolStripMenuItemList(menu.DropDownItems);
            }


        }

        /// <summary>
        /// подготовка списка пунктов контекстного меню маршрута перед открытием
        /// </summary>
        /// <param name="items"></param>
        private void prepareToolStripMenuItemList(ToolStripItemCollection items)
        {
            foreach (ToolStripMenuItem item in items)
            {
                if (item.HasDropDownItems)  //если у пункта есть дочерние элементы
                {
                    prepareToolStripMenuItemList(item.DropDownItems);
                    continue;
                }

                if ((string)item.Tag == "single")
                    item.Visible = dataGridView1.SelectedRows.Count == 1;

                if (item.Name == "showWaypointsToolStripMenuItem" || item.Name == "showOnMapToolStripMenuItem")
                    if (dataGridView1.SelectedRows.Count == 1)
                    {
                        BaseTrack tf = Tracks[dataGridView1.SelectedRows[0].Index];
                        if (item.Name == "showWaypointsToolStripMenuItem")
                            item.Checked = showingWaypointsList.Contains(tf);
                        if (item.Name == "showOnMapToolStripMenuItem")
                            item.Checked = showingRoutesList.Contains(tf);
                    }
                    else
                        item.CheckState = CheckState.Indeterminate;
            }
        }

        /// <summary>
        /// открытие трека как путевые точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = dataGridView1.SelectedCells[0].RowIndex;
            new FormPoints(Tracks[row]) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain);
        }

        /// <summary>
        /// удаление трека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackFileList list = new TrackFileList();
            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
            {
                int row = dgvr.Index;
                BaseTrack tttt = Tracks[row];
                list.Add(tttt);
            }
            DeleteRoute(list);

        }

        /// <summary>
        /// подробная информация о маршруте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = dataGridView1.SelectedCells[0].RowIndex;
            FormTrackInformation fti = new FormTrackInformation(Tracks[row]);
            if (fti.ShowDialog() == DialogResult.OK)
            {
                Tracks[row] = fti.Result;
                RefreshData();

                Vars.currentSelectedTrack = Tracks[row];
                Program.RefreshWindows(this);
            }
        }

        /// <summary>
        /// редактирование маршрута или путешествия на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editRouteMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BaseTrack bt = Tracks[dataGridView1.SelectedRows[0].Index];
            if (bt.GetType() == typeof(TrackFile))
                BeginEditRoute(bt as TrackFile);
            else
                BeginEditTrip(bt as TripRouteFile);
        }

        /// <summary>
        /// нормализация трека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void normalizeTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int min = int.MaxValue;
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {
                if (Tracks[r.Index].GetType() == typeof(TrackFile))
                {
                    if (min > r.Index)
                        min = r.Index;
                    Tracks[r.Index] = TrackHandler.Normalize(Tracks[r.Index] as TrackFile, Vars.Options.Converter.MinimalNormalizeAngle, Vars.Options.Converter.NormalizerBehavior);
                }
            }
            Program.RefreshWindows(this);
            dataGridView1.ClearSelection();
            dataGridView1.Rows[min].Selected = true;
        }

        /// <summary>
        /// преобразование обычного маршрута в отдельные путешествие
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toTripRouteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
            {
                int ind = dr.Index;
                TrackFile tf = (TrackFile)Tracks[ind];
                TripRouteFile tr = tf.ToTripRoute();
                Tracks.Remove(tf);
                Tracks.Insert(ind, tr);
            }
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
            RefreshData();
        }

        /// <summary>
        /// объединение маршуртов в одно путешествие
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinToTripRouteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int first = dataGridView1.SelectedRows[0].Index;
            TrackFileList tfl = new TrackFileList();
            foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
            {
                int ind = dr.Index;
                TrackFile tf = (TrackFile)Tracks[ind - tfl.Count];
                tfl.Add(tf);
                Tracks.Remove(tf);
            }
            TripRouteFile trr = tfl.ToTripRoute();
            Tracks.Insert(first, trr);
            Vars.currentSelectedTrack = trr;
            Program.RefreshWindows(this);
            RefreshData();
        }

        /// <summary>
        /// показать маршрут на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showRouteOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {

                BaseTrack tf = Tracks[r.Index];
                if (showingRoutesList.Contains(tf))
                    showingRoutesList.Remove(tf);
                else
                {
                    if (tf.Color.IsEmpty)
                        tf.Color = Vars.Options.Converter.GetColor();
                    showingRoutesList.Add(tf);
                }
                tf.IsVisible = !tf.IsVisible;
                Program.winMap.RefreshData();
            }
        }

        /// <summary>
        /// построение профиля высот с дополнительными точками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elevgraphWithIntermediatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReadText frt = new FormReadText(
                DialogType.ReadNumber,
                "Bведите минимальное расстояние в метрах между точками",
                "",
                false,
                false,
                false,
                false);
            if (frt.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                TrackFileList tfl = new TrackFileList();
                Task pr = new Task(new Action(() =>
                    {
                        Program.winMain.BeginOperation();
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            int rowI = row.Index;
                            BaseTrack tf = Tracks[rowI].Clone();

                            double lg = double.Parse(frt.Result);
                            tf = tf.AddIntermediatePoints(lg);

                            tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.setCurrentOperation);
                            tf.CalculateAll();
                            tfl.Add(tf);
                        }
                        this.Invoke(new Action(() => new FormElevVisual(tfl) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain)));
                        Program.winMain.EndOperation();
                    }));
                pr.Start();
            }
        }

        /// <summary>
        /// профиль высот по данным маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elevgraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackFileList tfl = new TrackFileList();
            Task pr = new Task(new Action(() =>
            {
                Program.winMain.BeginOperation();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int rowI = row.Index;
                    BaseTrack tf = Tracks[rowI];

                    tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.setCurrentOperation);
                    tf.CalculateAll();
                    tfl.Add(tf);
                }
                this.Invoke(new Action(() => new FormElevVisual(tfl) { FormBorderStyle = FormBorderStyle.Sizable }.Show(Program.winMain)));
                Program.winMain.EndOperation();
            }));
            pr.Start();
        }

        /// <summary>
        /// показать все точки на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showWaypointsOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
            {
                BaseTrack tf = Tracks[r.Index];
                if (showingWaypointsList.Contains(tf))
                {
                    showingWaypointsList.Remove(tf);
                    Program.winMap.DeleteWaypoints(tf);
                }
                else
                {
                    showingWaypointsList.Add(tf);
                    Program.winMap.ShowWaypoints(tf, false, true);
                }
            }
        }

        /// <summary>
        /// добавление трека к объединению
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addToJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winJoinTrackNullOrDisposed)
                Program.winJoinTrack = new FormJoinTracks();
            if (!Program.winJoinTrack.Visible)
                Program.winJoinTrack.Show(Program.winMain);

            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                Program.winJoinTrack.AddTrack(Tracks[dgvr.Index]);

            Program.winJoinTrack.Activate();
        }

        /// <summary>
        /// добавление маршрута в сравнение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.winCompareTrackNullOrDisposed)
                Program.winCompareTrack = new FormTrackComparison();
            if (!Program.winCompareTrack.Visible)
                Program.winCompareTrack.Show(Program.winMain);
            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                Program.winCompareTrack.AddTrack(Tracks[dgvr.Index]);
            Program.winCompareTrack.CalculateComparison();
            Program.winCompareTrack.Activate();
        }

        /// <summary>
        /// загрузка высот для выделенного машрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.winMain.BeginOperation();
            Task ts = new Task(new Action(() =>
            {
                int er = 0;
                try
                {
                    foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                    {
                        int row = dgvr.Index;
                        BaseTrack tf = Tracks[row];
                        tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.setCurrentOperation);
                        er = 0;
                    }
                }
                catch (Exception ex)
                {
                    er = -1;
                    this.Invoke(new Action(() =>
                        MessageBox.Show(this, "Не удалось получить информацию из-за проблем с соединением.\r\n" + ex.Message + "\r\nПроверьте соединение с Интернет", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    ));
                }
                this.Invoke(new Action(() =>
                {
                    if (er == 0)
                    {
                        //если выделен один маршрут, то обновлям данные в окнах
                        if (dataGridView1.SelectedRows.Count == 1)
                        {
                            int ind = dataGridView1.SelectedRows[0].Index;
                            Vars.currentSelectedTrack = this.Tracks[ind];
                            Program.RefreshWindows(this);
                        }
                    }
                    Program.winMain.EndOperation();
                }));
            }));
            ts.Start();
        }

        /// <summary>
        /// удалить высоты точек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeElevationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы действительно хотите очистить высоты точек у выбранных маршрутов?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                {
                    int row = dgvr.Index;
                    BaseTrack tf = Tracks[row];
                    tf.ClearAltitudes();
                }

                //если выделен один маршрут, то обновлям данные в окнах
                if (dataGridView1.SelectedRows.Count == 1)
                {
                    int ind = dataGridView1.SelectedRows[0].Index;
                    Vars.currentSelectedTrack = this.Tracks[ind];
                    Program.RefreshWindows(this);
                }
            }


        }

        /// <summary>
        /// аппроксимация высот
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void approximateAltitudesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormReadText frt = new FormReadText(
                DialogType.ReadNumber,
                "Введите степень полинома",
                Vars.Options.Converter.ApproximateAmount.ToString(),
                false,
                false,
                false,
                false);

            if (frt.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                int amount = (int)double.Parse(frt.Result);
                foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
                {
                    int row = dgvr.Index;
                    BaseTrack tf = Tracks[row];
                    tf = TrackHandler.Approximate(tf, amount);
                }
                MessageBox.Show(this, "Обработка завершена", "Выполнено!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //если выделен один маршрут, то одновлям данные в окнах
                if (dataGridView1.SelectedRows.Count == 1)
                {
                    int ind = dataGridView1.SelectedRows[0].Index;
                    Vars.currentSelectedTrack = this.Tracks[ind];
                    Program.RefreshWindows(this);
                }
            }
        }

        /// <summary>
        /// открытие папки с маршрутом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openRouteFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show(this, "Для этого действия должен быть выделен только один маршрут!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int row = dataGridView1.SelectedRows[0].Index;
            BaseTrack tf = Tracks[row];


            if (tf.FilePath != null && tf.FilePath.Length > 1)
                Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + tf.FilePath));
            else
                MessageBox.Show(this, "Этот маршрут еще не сохранен на диске", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// перезапись открытого файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overwriteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedRows[0].Index;
            BaseTrack tf = Tracks[row];


            if (!string.IsNullOrEmpty(tf.FilePath))
            {
                if (MessageBox.Show(this, "Вы действительно хотите перезаписать файл\r\n " + tf.FilePath, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (Path.GetExtension(tf.FilePath) == ".kml" || Path.GetExtension(tf.FilePath) == ".kmz")
                    {
                        string msg = "Данный формат (" + tf.Format.ToString() + ") поддерживает хранение маршрутов и путевых точек." +
                            "\r\nЕсли продолжить сохранение, информация о путевых точках в этом файле может быть утеряна." +
                            "\r\nДля сохранения маршрутов и точек в один файл используйте соответствующий пункт меню карты." +
                            "\r\nПродолжить?";
                        if (MessageBox.Show(this, msg, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.No)
                            return;
                    }
                    Serializer.Serialize(tf.FilePath, tf, tf.Format);
                }
            }
            else
                SaveFileToolStripMenuItem_Click(sender, e);

        }

        #endregion

        /// <summary>
        /// загрузка файлов перетаскиванием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.UnicodeText))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// загрузка файлов перетаскиванием
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string ss in s)
                    OpenFile(ss);
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string s = (string)e.Data.GetData(DataFormats.UnicodeText, false);
                this.OpenLink(s);
            }
        }


        /// <summary>
        /// выделение нажатой ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            Keyboard ss = new Keyboard();
            if (!ss.CtrlKeyDown && e.Button != MouseButtons.Right)
                dataGridView1.ClearSelection();
            if (e.Button == MouseButtons.Right)
                dataGridView1.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// выделение всей строки и вывод информации на другие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1)
                return;
            dataGridView1.Rows[e.RowIndex].Selected = true;
        }

        /// <summary>
        /// рисование цветов маршрутов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            if (dataGridView1.RowCount > 0 && dataGridView1.ColumnCount > 0 && Tracks.Count > 0)
            {
                int i = 0;
                foreach (DataGridViewRow dgvr in dataGridView1.Rows)
                {
                    BaseTrack tf = Tracks[i];
                    DataGridViewCell cell = dgvr.Cells["Цвет"];
                    cell.Style = new DataGridViewCellStyle() { BackColor = tf.Color };
                    i++;
                }
            }
        }

        /// <summary>
        /// вывод всплывающей подсказки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (Tracks.Count == 0 || e.RowIndex >= Tracks.Count) return;
            if ((e.ColumnIndex == this.dataGridView1.Columns[0].Index) && e.Value != null)
            {
                DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                BaseTrack tf = Tracks[e.RowIndex];
                string text = "Название: " + tf.Name + "\r\n";
                text += "Длина: " + tf.Distance.ToString("00.00") + " км\r\n";
                text += "Количество точек: " + tf.Count + "\r\n";
                text += "Средняя скорость: " + tf.KmphSpeed + " км/ч\r\n";
                text += "Описание: " + tf.Description + "\r\n";
                text += "Общее время: " + tf.Time + "\r\n";
                text += "Имя файла: " + tf.FileName + "\r\n";
                cell.ToolTipText = text;
            }
        }

        /// <summary>
        /// изменение выделенной строки и обновление окон
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count != 1)
                return;
            int ind = dataGridView1.SelectedRows[0].Index;

            BaseTrack tf = Tracks[ind];

            Vars.currentSelectedTrack = tf.Clone();
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// вывод полной информации о маршруте по двойному щелчку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows[e.RowIndex].Selected = true;
            if (Tracks[e.RowIndex].GetType() == typeof(TripRouteFile))
                editRouteMapToolStripMenuItem_Click(sender, new EventArgs());
            else
                informationToolStripMenuItem_Click(sender, new EventArgs());
        }

        /// <summary>
        /// удаление трека клавиатурой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.RowCount == 0) return;
            if (e.KeyData != Keys.Delete) return;
            removeToolStripMenuItem_Click(null, null);
        }

        #endregion

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// нажатие на пункт меню последний открытый файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void nm_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            try
            {
                OpenFile((string)item.Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Vars.Options.Converter.RecentFiles.Remove((string)item.Tag);
                RefreshRecentFiles();
            }
        }


        /// <summary>
        /// обновление списка последних загруженных файлов
        /// </summary>
        private void RefreshRecentFiles()
        {
            for (int i = 0; i < FileToolStripMenuItem.DropDownItems.Count; i += 0)
            {
                if (FileToolStripMenuItem.DropDownItems[i].Name.Contains("tsmiRecentButt"))
                {
                    FileToolStripMenuItem.DropDownItems.RemoveAt(i);
                    continue;
                }
                i++;
            }

            if (Vars.Options.Converter.RecentFiles != null)
            {
                int i = 0;
                foreach (string str in Vars.Options.Converter.RecentFiles)
                {
                    ToolStripMenuItem nm = new ToolStripMenuItem(str);
                    nm.Click += nm_Click;
                    nm.Tag = str;
                    nm.ToolTipText = str;
                    nm.Text = Path.GetFileName(str);
                    nm.Name = "tsmiRecentButt" + i;
                    FileToolStripMenuItem.DropDownItems.Add(nm);
                    i++;
                }
            }
        }

        #endregion

        #region взаимодействие

        /// <summary>
        /// добавление маршрута в список маршрутов
        /// </summary>
        /// <param name="trackFile"></param>
        public void AddRouteToList(BaseTrack trackFile)
        {
            if (trackFile == null)
                return;
            if (trackFile.Color.IsEmpty)
                trackFile.Color = Vars.Options.Converter.GetColor();
            Tracks.Add(trackFile);
            RefreshData();
        }

        /// <summary>
        /// добавление нескольких маршрутов в список
        /// </summary>
        /// <param name="routes"></param>
        internal void AddRouteToList(TrackFileList routes)
        {
            foreach (BaseTrack tf in routes)
                AddRouteToList(tf);
        }

        /// <summary>
        /// удаление маршрута из списка
        /// </summary>
        /// <param name="route"></param>
        public void DeleteRoute(BaseTrack route)
        {
            TrackFileList tl = new TrackFileList() { route };
            DeleteRoute(tl);
        }

        /// <summary>
        /// удаление нескольких  маршрутов из списка
        /// </summary>
        /// <param name="list"></param>
        private void DeleteRoute(TrackFileList list)
        {
            foreach (BaseTrack tf in list)
                Tracks.Remove(tf);

            if (Tracks.Count > 0)
                Vars.currentSelectedTrack = Tracks[0];
            else
                Vars.currentSelectedTrack = null;
            this.RefreshData();
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// начало редактирования маршута , который уже есть в списке маршутов
        /// </summary>
        /// <param name="route"></param>
        public void BeginEditRoute(TrackFile route)
        {
            if (!this.Tracks.Contains(route))
                throw new ApplicationException("Маршрута нет в списке");

            BaseTrack backup = route.Clone(); //запоминаем старый маршрут
            TrackFile ed = route.Clone() as TrackFile; //новый маршрут для редактирования
            Tracks.Remove(route); //удаление из списка
            RefreshData();
            //начало редактирования
            Program.winMap.BeginEditRoute(ed,
                (tf) =>
                {
                    EndEditRouteOrTrip(tf);
                },
                () =>
                {
                    backup.CalculateAll();
                    AddRouteToList(backup);
                }
                );
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// начало редактирования путешествия, которое уже есть в списке маршутов
        /// </summary>
        /// <param name="tripRouteFile">путешествие для редактирования</param>
        private void BeginEditTrip(TripRouteFile tripRouteFile)
        {
            if (!this.Tracks.Contains(tripRouteFile))
                throw new ApplicationException("Маршрута нет в списке");

            BaseTrack backup = tripRouteFile.Clone(); //запоминаем старый маршрут
            TripRouteFile ed = tripRouteFile.Clone() as TripRouteFile; //новый маршрут для редактирования
            Tracks.Remove(tripRouteFile); //удаление из списка
            RefreshData();
            //начало редактирования
            Program.winMap.BeginEditTrip(ed,
                (tf) =>
                {
                    EndEditRouteOrTrip(tf);
                },
                () =>
                {
                    backup.CalculateAll();
                    AddRouteToList(backup);
                }
                );
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// завершение редактирования маршрута
        /// </summary>
        /// <param name="route"></param>
        public void EndEditRouteOrTrip(BaseTrack route)
        {
            if (route == null)
                return;
            Vars.currentSelectedTrack = route;
            AddRouteToList(route);
        }

        /// <summary>
        /// обновление информации
        /// </summary>
        internal void RefreshData()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Tracks.Source;
            dataGridView1.Refresh();

            foreach (DataGridViewColumn column in dataGridView1.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            RefreshRecentFiles(); //обновление списка последних файлов
            dataGridView1.ClearSelection();

            if (Vars.currentSelectedTrack != null)
            {
                int ind = Tracks.IndexOf(Vars.currentSelectedTrack);
                if (ind == -1) return;
                dataGridView1.Rows[ind].Selected = true;
            }
        }

        /// <summary>
        /// открытие файла 
        /// </summary>
        /// <param name="FileName"> имя файла</param>
        public void OpenFile(string FileName)
        {
            BaseTrack toShow = null;
            if (Path.GetExtension(FileName).ToLower() == ".trr")
            {
                TripRouteFile tr = Serializer.DeserializeTripRouteFile(FileName);
                AddRouteToList(tr);
                Program.winMap.Clear();
                toShow = tr;
            }
            else
            {
                TrackFileList tfl = Serializer.DeserializeTrackFileList(FileName);
                if (tfl.Count > 0)
                {
                    toShow = tfl[0];
                    AddRouteToList(tfl);
                }
            }
            Vars.Options.Converter.AddRecentFile(FileName); //добавление последнего файла
            Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(FileName); //послденяя открытая папка
            if (this.Tracks.Count > 0)
                Vars.currentSelectedTrack = this.Tracks[0];
            RefreshData();
        }

        /// <summary>
        /// открытие ссылки
        /// </summary>
        /// <param name="link"></param>
        public void OpenLink(string link)
        {
            BaseTrack tf = Serializer.DeserializeTrackFile(link);
            AddRouteToList(tf);
        }

        /// <summary>
        /// присваивает текущему выделенному треку значение из Vars.currentSelectedTrack
        /// </summary>
        internal void UpdateSelectedTrack()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                Tracks[index] = Vars.currentSelectedTrack;
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {


            string one = "vVg_AmdVUgM="; //length 12
            //55.727464,37.705918

            string two = "KEc-ApFnUgNoLAEA6GkAAA==";
            //55.732114,37.635880
            //55.759226,37.712784   27112   76904    

            string three = "vVg_AmdVUgPKiAAAUiQAAPqoAABdKgAA";
            //55.727464,37.705918
            //55.736763,37.740937   9299    35019 
            //55.747609,37.784196   10846   43259

            string four = "vVg_AmdVUgNoLAEAPhsAAKG___8iO___J7v8_6Hb__8=";
            //55.727464,37.705918
            //55.734438,37.782822
            //55.684040,37.766343
            //55.674729,37.552109

            string vlong = "VKRBArXMUgPQ_P__zQUAALz____7AAAACAAAAFkAAAAbAAAASgAAAHvv__-5AQAAzv7__ysAAABb____LwAAACD___94AAAAYf___yUAAAB2-v__oAAAAKb-__8UAAAAkPj__wYAAAAhAAAAVwIAACkAAACSAAAAVgAAAJoAAADdAAAA4AAAACYBAADoAAAAfwMAAIYCAAB1AAAAvv___zcAAACj____SwAAAPL8__8=";
            string v2 = "zaRBAubLUgNx_P__aAYAAMr___96AAAA2f___7YAAAAUAAAAgAAAAIEAAABCAQAAJwIAABAFAABQAAAAhAEAAN4AAACZAAAAagIAADsBAAA__f__hwEAADT-___iAAAACfb__0UFAACW5f__Pg4AAIn-___nAAAACP___9sAAACb____gQAAAL7-__8IAgAASP___5wAAAD6_v__fgAAAGD___8sAAAAyP7__ycAAACy_v__-v___1D8__80____-vz__0T___-d_f__V____2n___-u____q____7z____Y____xv___8____8N____x_____D7___3____zv___8T___-n____mP___8P____0_v__vf___3D-___t____vf3________O____vP7__9j____U_f__lv___7r2__8=";

            Yandex yand = new Yandex();

            DateTime start = DateTime.Now;
            var x1 = yand.CreateRoute(new Lib.Classes.Coordinate(55.739298, 37.815652), new Lib.Classes.Coordinate(54.974780, 73.317424), null);
            TimeSpan ts = DateTime.Now - start;


        }

    }
}
