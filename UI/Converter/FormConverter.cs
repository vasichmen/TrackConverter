﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Mathematic.Assessment;
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
            this.Size = Vars.Options.Converter.WinSize;
            RefreshRecentFiles(); //обновление писка последних загруженных файлов
        }

        /// <summary>
        /// Запуск с загрузкой файла
        /// </summary>
        /// <param name="args"></param>
        public FormConverter(string[] args)
            : this()
        {
            try
            {
                if (args.Length != 0)
                    if (File.Exists(args[0]))
                    {
                        if (Path.GetExtension(args[0]) == ".kml" || Path.GetExtension(args[0]) == ".kmz")
                        {
                            GeoFile gf = Serializer.DeserializeGeoFile(args[0]);
                            this.Tracks = gf.Routes;
                            Program.winMap.Clear();
                            Program.winMap.ShowPoints(gf.Waypoints, true);
                            OpenFile(null, true);
                        }
                        else
                        {
                            if (Path.GetExtension(args[0]) == ".adrs")
                                openingFile = args[0];
                            else
                                OpenFile(args[0], false);
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


        #region события

        #region форма

        /// <summary>
        /// закрытие окна и сохранение настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormConverter_FormClosed(object sender, FormClosedEventArgs e)
        {
            Vars.Options.Converter.IsFormMapShow = Program.winMap.Visible;
            Vars.Options.Converter.LastLoadedTracks = this.Tracks.FilePaths;
            Vars.Options.Converter.WinSize = this.Size;
        }

        /// <summary>
        /// перевод фокуса на форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormConverter_Load(object sender, EventArgs e)
        {
            //открытие окна карты, если оно было открыто при последнем закрытии
            if (Program.winMapNullOrDisposed)
                Program.winMap = new FormMap();
            if (!Program.winMap.Visible && Vars.Options.Converter.IsFormMapShow)
                Program.winMap.Show();

            //открытие последних файлов
            if (this.Tracks == null || this.Tracks.Count == 0)
            {
                this.Tracks = Serializer.DeserializeTrackFileList(Vars.Options.Converter.LastLoadedTracks);
                this.OpenFile(null, true);
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
                    TrackFile pts = null;
                    Program.winMain.BeginOperation();
                    //метод обновления информации о выполняемой операции

                    pts = Serializer.DeserializeTrackFile(openingFile, Program.winMain.setCurrentOperation);
                    openingFile = null;
                    //обработка результатов
                    Program.winMap.Invoke(new Action(() =>
                    {
                        Program.winMain.EndOperation();
                        Program.winMap.Clear();
                        Program.winMap.ShowPoints(pts, true);
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
                of.Filter = "Все поддерживаемые форматы(*.crd, *.wpt, *.plt, *.rt2, *.kml, *.kmz, *.gpx, *.rte, *.osm, *.nmea, *.csv, *.txt)|*.crd; *.wpt; *.plt; *rt2; *.kml; *.kmz; *.gpx; *.rte; *.osm; *.nmea; *.csv; *.txt";
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


                if (Vars.Options.Common.IsLoadDir)
                    of.InitialDirectory = Vars.Options.Common.LastFileLoadDirectory;
                of.Multiselect = true;
                of.RestoreDirectory = false;
                if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    foreach (string fn in of.FileNames)
                        OpenFile(fn, false);
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
                    TrackFile tf = new TrackFile();
                    tf = Serializer.DeserializeTrackFile(of.Result);
                    Tracks.Add(tf);
                    OpenFile(null, true);
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
            TrackFile tf = null;

            //если кнопка контекстного меню
            if ((string)(((ToolStripMenuItem)sender).Name) == "saveFileContextToolStripMenuItem1")
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
            sf.Filter = "Файл маршрута Androzic (*.rt2)|*.rt2";
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

            if (Vars.Options.Common.IsSaveDir)
                sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
            if (Vars.Options.Common.IsExtension)
                sf.FilterIndex = Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack;
            sf.FileName = tf.FileName;


            if (sf.ShowDialog() == DialogResult.OK)
            {
                tf.FileName = sf.FileName;
                switch (sf.FilterIndex)
                {
                    case 1:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.Rt2File);
                        break;
                    case 2:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.PltFile);
                        break;
                    case 3:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.WptFile);
                        break;
                    case 4:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CrdFile);
                        break;
                    case 5:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmlFile);
                        break;
                    case 6:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.GpxFile);
                        break;
                    case 7:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.KmzFile);
                        break;
                    case 8:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.OsmFile);
                        break;
                    case 9:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.NmeaFile);
                        break;
                    case 10:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.CsvFile);
                        break;
                    case 11:
                        Serializer.Serialize(sf.FileName, new TrackFileList() { tf }, FileFormats.RteFile);
                        break;
                    case 12:
                        Serializer.Serialize(sf.FileName, tf, FileFormats.TxtFile);
                        break;
                    case 13:
                        Program.winMain.BeginOperation();
                        Task ts = new Task(new Action(() =>
                            {
                                Serializer.Serialize(sf.FileName, tf, FileFormats.AddressList, Program.winMain.setCurrentOperation);
                                Program.winMain.EndOperation();
                            }));
                        ts.Start();
                        break;
                }
                Vars.Options.Common.LastFileSaveDirectory = Path.GetDirectoryName(sf.FileName);
                Vars.Options.Common.LastSaveExtensionNumberSaveOneTrack = sf.FilterIndex;
            }
        }

        /// <summary>
        /// Сохранение маршрута в яндекс- ссылку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveYandexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrackFile tf = null;

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
            TrackFile tf = null;

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

            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Program.winWaitingNullOrDisposed)
                    Program.winWaiting = new FormWaiting();

                Program.winWaiting.Show(this);
                Program.winWaiting.Focus();
                try
                {
                    Task pr = new Task(new Action(() =>
                    {
                        switch (sf.FilterIndex)
                        {
                            case 1:
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.RteFile);
                                break;
                            case 2:
                                Serializer.Serialize(sf.FileName, tfs, FileFormats.KmlFile);
                                break;
                            case 3:
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
            FormReadText rt = new FormReadText(DialogType.ReadExtension, "Bведите формат файла (wpt,crd,plt,rt2,gpx,kml,kmz,osm,nmea,txt,csv)", Vars.Options.Common.IsExtension ? Vars.Options.Common.LastSaveSeparateExtension : "", false, false, false, false);
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
                    ParallelLoopResult pr = Parallel.ForEach<TrackFile>(Tracks, new Action<TrackFile>((TrackFile tf) =>
                    {
                        string basename = fb.SelectedPath + "\\" + tf.FileName + "_" + Guid.NewGuid().ToString();
                        switch (rt.Result)
                        {
                            case "wpt":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.WptFile);
                                break;
                            case "crd":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.CrdFile);
                                break;
                            case "plt":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.PltFile);
                                break;
                            case "rt2":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.Rt2File);
                                break;
                            case "gpx":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.GpxFile);
                                break;
                            case "kml":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.KmlFile);
                                break;
                            case "kmz":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.KmzFile);
                                break;
                            case "osm":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.OsmFile);
                                break;
                            case "nmea":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.NmeaFile);
                                break;
                            case "csv":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.CsvFile);
                                break;
                            case "txt":
                                Serializer.Serialize(basename + ".wpt", tf, FileFormats.TxtFile);
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
            dataGridView1.DataSource = null;
            Tracks.Clear();
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
            //если элемент - первое контектное меню, то выбираем пункты на основе тегов
            if (sender.GetType() == typeof(ContextMenuStrip))
            {
                ContextMenuStrip menu = (ContextMenuStrip)sender;

                //если не выделено ни одного элемента, то не выводим меню
                if (dataGridView1.SelectedRows.Count < 1)
                    e.Cancel = true;

                foreach (ToolStripMenuItem item in menu.Items)
                {
                    if (item.HasDropDownItems) //если у пункта есть дочерние элементы
                        this.contextMenuStripDGW_Opening(item, null);
                    if ((string)item.Tag == "single")
                        item.Visible = dataGridView1.SelectedRows.Count == 1;
                }
            }

            //если элемент - пункт с дочерними элементами, то выбираем пункты
            if (sender.GetType() == typeof(ToolStripMenuItem))
            {
                ToolStripMenuItem menu = (ToolStripMenuItem)sender;
                foreach (ToolStripMenuItem item in menu.DropDownItems)
                {
                    if (item.HasDropDownItems)  //если у пункта есть дочерние элементы
                        this.contextMenuStripDGW_Opening(item, null);
                    if ((string)item.Tag == "single")
                        item.Visible = dataGridView1.SelectedRows.Count == 1;
                }
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
            new FormPoints(Tracks[row]).Show(Program.winMain);
        }

        /// <summary>
        /// удаление трека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
            {
                int row = dgvr.Index;
                Tracks.Remove(Tracks[row]);
            }
            OpenFile(null, true);
            if (dataGridView1.SelectedRows.Count > 0)
                Vars.currentSelectedTrack = Tracks[dataGridView1.SelectedRows[0].Index];
            else
                Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
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
                OpenFile(null, true);

                Vars.currentSelectedTrack = Tracks[row];
                Program.RefreshWindows(this);
            }
        }

        /// <summary>
        /// редактирование мкршрута на карте
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

            int row = dataGridView1.SelectedRows[0].Index;
            TrackFile backup = Tracks[row].Clone(); //запоминаем старый маршрут
            TrackFile ed = Tracks[row].Clone(); //новый маршрут для редактирования
            Tracks.RemoveAt(row); //удаление из списка
            OpenFile(null, true);
            //начало редактирования
            Program.winMap.BeginEditRoute(ed,
                (tf) =>
                {
                    EndEditRoute(tf);
                },
                () =>
                {
                    backup.CalculateAll();
                    Tracks.Add(backup);
                    OpenFile(null, true);
                }
                );
            Vars.currentSelectedTrack = null;
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// показать маршрут на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showOnMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
            {
                int row = dgvr.Index;
                TrackFile tf = Tracks[row];
                if (tf.Color.IsEmpty)
                    tf.Color = Vars.Options.Converter.GetColor();

                new FormMap(new TrackFileList(Tracks[row])).Show(Program.winMain);
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

                if (Program.winWaitingNullOrDisposed)
                    Program.winWaiting = new FormWaiting();

                Program.winWaiting.Focus();
                Program.winWaiting.Show(this);
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    int rowI = row.Index;
                    TrackFile tf = Tracks[rowI];

                    double lg = double.Parse(frt.Result);
                    tf = tf.AddIntermediatePoints(lg);

                    Task pr = new Task(new Action(() =>
                    {
                        try
                        {
                            tf = new GeoInfo(GeoInfoProvider.ETOPO).GetElevation(tf);
                        }
                        catch (ApplicationException eeeee)
                        { MessageBox.Show("Ошибка запроса"); }
                    }));
                    pr.Start();
                    pr.Wait();
                    tfl.Add(tf);
                }
                Program.winWaiting.Visible = false;
                new FormElevVisual(tfl).Show(Program.winMain);
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
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                int rowI = row.Index;
                TrackFile tf = Tracks[rowI];
                tfl.Add(tf);
            }
            new FormElevVisual(tfl).Show(Program.winMain);
        }

        /// <summary>
        /// показать все точки на карте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showWaypointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                Program.winMap.ShowPoints(Tracks[r.Index], true);
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
                Program.winJoinTrack.Visible = true;

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
                Program.winCompareTrack.Show();
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
                        TrackFile tf = Tracks[row];
                        tf = new GeoInfo(Vars.Options.DataSources.GeoInfoProvider).GetElevation(tf, Program.winMain.setCurrentOperation);
                        er = 0;
                    }
                }
                catch (Exception ex)
                {
                    er = -1;
                    this.Invoke(new Action(() => MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error)));
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
                    TrackFile tf = Tracks[row];
                    tf = Approximator.Approximate(tf, amount);
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
            TrackFile tf = Tracks[row];


            if (tf.FilePath != null && tf.FilePath.Length > 1)
                Process.Start(Path.GetDirectoryName(tf.FilePath));
            else
                MessageBox.Show(this, "Этот маршрут еще не сохранен на диске", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// перезапись открытого файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedRows[0].Index;
            TrackFile tf = Tracks[row];


            if (tf.FilePath != null && tf.FilePath.Length > 1)
            {
                if (MessageBox.Show(this, "Вы действительно хотите перезаписать файл " + tf.FilePath, "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (Path.GetExtension(tf.FilePath) == ".kml" || Path.GetExtension(tf.FilePath) == ".kmz")
                    {
                        string msg = "Данный формат (" + tf.Format.ToString() + ") поддерживает хранение маршрутов и путевых точек." +
                            "\r\nЕсли продолжить сохранение, информация о путевых точках в этом файле может быть утеряна." +
                            "\r\nДля сохранения маршрутов и точек в один файл используйте соответствующий пункт меню карты." +
                            "\r\nПродолжить?";
                        if (MessageBox.Show(this,
                            msg,
                            "Внимание!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                            return;
                    }
                    Serializer.Serialize(tf.FilePath, tf, tf.Format);
                    MessageBox.Show("Файл сохранен!");
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
            //dataGridView1.BackgroundColor = Color.DarkGreen;
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
                    OpenFile(ss, false);
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
                    TrackFile tf = Tracks[i];
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
                TrackFile tf = Tracks[e.RowIndex];
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

            TrackFile tf = Tracks[ind];

            Vars.currentSelectedTrack = tf;
            Program.RefreshWindows(this);
        }

        /// <summary>
        /// вывод полной информации о маршруте по двойному щелчку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
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
                OpenFile((string)item.Tag, false);
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
        public void AddRouteToList(TrackFile trackFile)
        {
            if (trackFile == null)
                return;
            if (trackFile.Color.IsEmpty)
                trackFile.Color = Vars.Options.Converter.GetColor();
            Tracks.Add(trackFile);
            OpenFile(null, true);
        }

        /// <summary>
        /// завершение редактирования маршрута
        /// </summary>
        /// <param name="track"></param>
        public void EndEditRoute(TrackFile track)
        {
            if (track == null)
                return;
            Tracks.Add(track);
            OpenFile(null, true);
        }

        /// <summary>
        /// обновление информации выделенного трека на Vars.currentSelectedTrack
        /// </summary>
        internal void RefreshSelectedTrack()
        {
            if (dataGridView1.SelectedRows.Count == 1 && Vars.currentSelectedTrack != null)
            {
                Vars.currentSelectedTrack.CalculateAll();
                int ind = dataGridView1.SelectedRows[0].Index;
                Tracks[ind] = Vars.currentSelectedTrack;
                OpenFile(null, true);
            }
        }

        /// <summary>
        /// открытие файла 
        /// </summary>
        /// <param name="FileName"> имя файла</param>
        /// <param name="OnlyRefreshUI"> Если истина, то будет обновлен только внешний вид, настройки не будут затронуты.
        /// (при загрузке ссылок)</param>
        public void OpenFile(string FileName, bool OnlyRefreshUI)
        {
            if (!OnlyRefreshUI)
            {
                TrackFile tf = new TrackFile();
                Tracks.AddRange(Serializer.DeserializeTrackFileList(FileName));
                Vars.Options.Converter.AddRecentFile(FileName);//добавление последнего файла
                Vars.Options.Common.LastFileLoadDirectory = Path.GetDirectoryName(FileName); //послденяя открытая папка
            }

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Tracks.Source;
            dataGridView1.Refresh();

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            SaveToolStripMenuItem.Enabled = true;
            RefreshRecentFiles(); //обновление списка последних файлов
            dataGridView1.ClearSelection();

            if (!OnlyRefreshUI)
                if (dataGridView1.Rows.Count != 0)
                    dataGridView1.Rows[Tracks.Count - 1].Selected = true;

        }

        /// <summary>
        /// открытие ссылки
        /// </summary>
        /// <param name="link"></param>
        public void OpenLink(string link)
        {
            TrackFile tf = new TrackFile();
            tf = Serializer.DeserializeTrackFile(link);
            Tracks.Add(tf);
            OpenFile(null, true);
        }

        #endregion

    }
}
