﻿using GMap.NET;
using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.ETOPO;
using TrackConverter.Res.Properties;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно настроек
    /// </summary>
    public partial class FormOptions : Form
    {
        /// <summary>
        /// создает новое окно настроек
        /// </summary>
        public FormOptions()
        {
            InitializeComponent();

            #region Общие

            checkBoxLoadPath.Checked = Vars.Options.Common.IsLoadDir;
            checkBoxSavePath.Checked = Vars.Options.Common.IsSaveDir;
            checkBoxLastExtension.Checked = Vars.Options.Common.IsExtension;
            checkBoxIsLoadETOPOOnStart.Checked = Vars.Options.Common.IsLoadETOPOOnStart;

            //формат настроек
            switch (Vars.Options.Format)
            {
                case OptionsFormat.XML:
                    comboBoxOptionsFormat.SelectedIndex = 0;
                    break;
                case OptionsFormat.JSON:
                    comboBoxOptionsFormat.SelectedIndex = 1;
                    break;
            }

            #endregion

            #region Конвертер

            textBoxMagnNPLat.Text = Vars.Options.Converter.NorthPoleLatitude.ToString();
            textBoxMagnNPLon.Text = Vars.Options.Converter.NorthPoleLongitude.ToString();
            textBoxMinimumRiseInterval.Text = Vars.Options.Converter.MinimumRiseInterval.ToString("00.00");
            checkBoxIsapproximate.Checked = Vars.Options.Converter.IsApproximateAltitudes;
            numericUpDownAmount.Value = Vars.Options.Converter.ApproximateAmount;

            //эллипсоид
            switch (Vars.Options.Converter.Geosystem)
            {
                case Geosystems.WGS84:
                    comboBoxEllipsoid.SelectedIndex = 0;
                    break;
                case Geosystems.PZ90:
                    comboBoxEllipsoid.SelectedIndex = 1;
                    break;
            }

            //нормализатор
            switch (Vars.Options.Converter.NormalizerBehavior)
            {
                case NormalizerBehavior.RemovePoint:
                    comboBoxNormalizeBehavior.SelectedIndex = 0;
                    break;
                case NormalizerBehavior.AddCritical:
                    comboBoxNormalizeBehavior.SelectedIndex = 1;
                    break;
            }

            textBoxMinimalNormalizeAngle.Text = Vars.Options.Converter.MinimalNormalizeAngle.ToString();

            #endregion

            #region Источники данных

            //геокодер
            switch (Vars.Options.DataSources.GeoCoderProvider)
            {
                case GeoCoderProvider.Google:
                    comboBoxGeocodingProvider.SelectedIndex = 1;
                    break;
                case GeoCoderProvider.Yandex:
                    comboBoxGeocodingProvider.SelectedIndex = 0;
                    break;
                case GeoCoderProvider.Nominatim:
                    comboBoxGeocodingProvider.SelectedIndex = 2;
                    break;
                case GeoCoderProvider.Arcgis:
                    comboBoxGeocodingProvider.SelectedIndex = 3;
                    break;
            }

            //поставщик высот
            switch (Vars.Options.DataSources.GeoInfoProvider)
            {
                case GeoInfoProvider.Google:
                    comboBoxGeoInfoProvider.SelectedIndex = 0;
                    break;
                case GeoInfoProvider.GTOPO30:
                    comboBoxGeoInfoProvider.SelectedIndex = 1;
                    break;
                case GeoInfoProvider.ETOPO:
                    comboBoxGeoInfoProvider.SelectedIndex = 2;
                    break;
            }

            //использовать кэш
            checkBoxUseCacheGeocoder.Checked = Vars.Options.DataSources.UseGeocoderCache;
            checkBoxUseMaplayerCache.Checked = Vars.Options.DataSources.UseMapLayerCache;
            checkBoxUseImageCache.Checked = Vars.Options.DataSources.UseImagesCache;
            checkBoxUseExtInfoCache.Checked = Vars.Options.DataSources.UseExtInfoCache;

            //длительность хранения кэша
            numericUpDownImagesCacheDuration.Value = Convert.ToDecimal( Vars.Options.DataSources.MaxImageCacheDays);
            numericUpDownMaplayersCacheDuration.Value = Convert.ToDecimal(Vars.Options.DataSources.MaxVectorLayersCacheDays);
            numericUpDownExtInfoCacheDuration.Value = Convert.ToDecimal(Vars.Options.DataSources.MaxExtInfoCacheDays);
            numericUpDownGeocoderCacheDuration.Value = Convert.ToDecimal(Vars.Options.DataSources.MaxGeocoderCacheDays);

            //папка ETOPO
            textBoxDBETOPOFolder.Text = Vars.Options.DataSources.ETOPODBFolder;
            new ToolTip().SetToolTip(textBoxDBETOPOFolder, Vars.Options.DataSources.ETOPODBFolder);

            #endregion

            #region Карта

            //сохранять точки и маршрутв при перезапуске
            checkBoxRestoreWptsRts.Checked = Vars.Options.Map.RestoreRoutesWaypoints;

            //окно настроек Wikimapia
            checkBoxIsFormWikimapiaToolbarShow.Checked = Vars.Options.Map.IsFormWikimpiaToolbarShow;

            //язык карты
            switch (Vars.Options.Map.MapLanguange)
            {
                case LanguageType.English:
                    comboBoxMapLanguange.SelectedIndex = 1;
                    break;
                case LanguageType.Russian:
                    comboBoxMapLanguange.SelectedIndex = 0;
                    break;
                case LanguageType.German:
                    comboBoxMapLanguange.SelectedIndex = 2;
                    break;
                default:
                    comboBoxMapLanguange.SelectedIndex = 0;
                    break;

            }

            //метод оптимального маршрута
            switch (Vars.Options.Map.OptimalRouteMethod)
            {
                case OptimalMethodType.Greedy:
                    trackBarOptimalMethodType.Value = 3;
                    break;
                case OptimalMethodType.BranchBounds:
                    trackBarOptimalMethodType.Value = 2;
                    break;
                case OptimalMethodType.FullSearch:
                    trackBarOptimalMethodType.Value = 0;
                    break;
                case OptimalMethodType.PolarSearch:
                    trackBarOptimalMethodType.Value = 1;
                    break;
                default:
                    trackBarOptimalMethodType.Value = 1;
                    break;
            }

            //глубина рекурсивного перебора
            numericUpDownRecursiveDepth.Value = Vars.Options.Map.MaxFullSearchNodes;

            //Не показывать маркеры азимутов
            checkBoxShowAziMarkers.Checked = Vars.Options.Map.ShowAziMarkers;

            //использование маршрутизации
            checkBoxUseRouter.Checked = Vars.Options.Map.UseRouterInOptimal;

            //метод ветвей и границ для рекурсивного перебора
            checkBoxUseBranchBounds.Checked = Vars.Options.Map.UseBranchBoundsInPolarSearch;

            #endregion

            #region Сервисы

            //поставщик прокладки маршрута
            switch (Vars.Options.Services.PathRouteProvider)
            {
                case PathRouteProvider.Yandex:
                    comboBoxRoutePathingProvider.SelectedIndex = 1;
                    break;
                case PathRouteProvider.Google:
                    comboBoxRoutePathingProvider.SelectedIndex = 0;
                    break;
            }

            //тип маршрута (пешком, на машине)
            switch (Vars.Options.Services.PathRouteMode)
            {
                case PathRouteMode.Driving:
                    comboBoxPathRouteMode.SelectedIndex = 1;
                    break;
                case PathRouteMode.Walk:
                    comboBoxPathRouteMode.SelectedIndex = 0;
                    break;
            }

            //поставщик сокращения ссылок
            switch (Vars.Options.Services.LinkShorterProvider)
            {
                case LinkShorterProvider.Clck:
                    comboBoxLinkShorter.SelectedIndex = 0;
                    break;
                case LinkShorterProvider.Qps:
                    comboBoxLinkShorter.SelectedIndex = 1;
                    break;
                case LinkShorterProvider.VK:
                    comboBoxLinkShorter.SelectedIndex = 2;
                    break;
                case LinkShorterProvider.Bitly:
                    comboBoxLinkShorter.SelectedIndex = 3;
                    break;
                default:
                    throw new ApplicationException("неизвестный поставщик поиска " + Vars.Options.Services.LinkShorterProvider);
            }

            //кэш ФС при построении
            checkBoxUseFSCacheForCreatingRoutes.Checked = Vars.Options.Services.UseFSCacheForCreatingRoutes;

            //изменять созданный маршрут
            checkBoxEditAfterPathing.Checked = Vars.Options.Services.ChangePathedRoute;

            #endregion

            #region Графики

            checkBoxXKm.Checked = Vars.Options.Graphs.isXKm;
            checkBoxYKm.Checked = Vars.Options.Graphs.isYKm;
            textBoxIntermediateDistance.Text = Vars.Options.Graphs.IntermediateDistance.ToString();
            checkBoxIsAddIntermediatePoints.Checked = Vars.Options.Graphs.IsAddIntermediatePoints;

            #endregion

            #region Авторизация

            bool isWikimapiaLoggedIn = Wikimapia.ServiceEngine.LoggedIn;
            buttonWikimapiaLogin.Enabled = !isWikimapiaLoggedIn;
            buttonWikimapiaLogout.Enabled = isWikimapiaLoggedIn;

            #endregion

            new ToolTip().SetToolTip(buttonAssociateFiles, "Ассоциировать с программой поддерживаемые форматы файлов");
            new ToolTip().SetToolTip(checkBoxLastExtension, "При сохранении файлов запоминать последний выбранный формат");
            new ToolTip().SetToolTip(checkBoxLoadPath, "При загрузке файлов запоминать последнюю папку");
            new ToolTip().SetToolTip(checkBoxSavePath, "При сохранении файлов запоминать последнюю папку");
            new ToolTip().SetToolTip(checkBoxEditAfterPathing, "После прокладки маршрута, проложенный маршрут будет открыт для редактирования");
        }

        /// <summary>
        /// сохраненеие изменений в настройках
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {

            #region Общие
            Vars.Options.Common.IsLoadDir = checkBoxLoadPath.Checked;
            Vars.Options.Common.IsSaveDir = checkBoxSavePath.Checked;
            Vars.Options.Common.IsExtension = checkBoxLastExtension.Checked;
            Vars.Options.Common.IsLoadETOPOOnStart = checkBoxIsLoadETOPOOnStart.Checked;

            //основное окно
            if (comboBoxOptionsFormat.SelectedIndex == 0)
                Vars.Options.Format = OptionsFormat.XML;
            if (comboBoxOptionsFormat.SelectedIndex == 1)
                Vars.Options.Format = OptionsFormat.JSON;

            #endregion

            #region Конвертер

            Vars.Options.Converter.NorthPoleLatitude = double.Parse(textBoxMagnNPLat.Text.Replace('.', Vars.DecimalSeparator));
            Vars.Options.Converter.NorthPoleLongitude = double.Parse(textBoxMagnNPLon.Text.Replace('.', Vars.DecimalSeparator));
            Vars.Options.Converter.MinimumRiseInterval = double.Parse(textBoxMinimumRiseInterval.Text.Replace('.', Vars.DecimalSeparator));

            //аппроксимация высот
            Vars.Options.Converter.IsApproximateAltitudes = checkBoxIsapproximate.Checked;
            Vars.Options.Converter.ApproximateAmount = numericUpDownAmount.Value;

            //эллипсоид
            if (comboBoxEllipsoid.SelectedIndex == 0)
                Vars.Options.Converter.Geosystem = Geosystems.WGS84;
            if (comboBoxEllipsoid.SelectedIndex == 1)
                Vars.Options.Converter.Geosystem = Geosystems.PZ90;

            //нормализатор
            if (comboBoxNormalizeBehavior.SelectedIndex == 0)
                Vars.Options.Converter.NormalizerBehavior = NormalizerBehavior.RemovePoint;
            if (comboBoxNormalizeBehavior.SelectedIndex == 1)
                Vars.Options.Converter.NormalizerBehavior = NormalizerBehavior.AddCritical;
            bool f = double.TryParse(textBoxMinimalNormalizeAngle.Text.Trim().Replace('.', Vars.DecimalSeparator), out double ang);
            if (!f || ang >= 180)
            {
                MessageBox.Show(this, "Ошибка ввода.\r\nМинимальный угол нормализации должен быть в пределах от 0 до 180 градусов", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Vars.Options.Converter.MinimalNormalizeAngle = ang;

            #endregion

            #region Карта

            //восстанавливать точки и машрруты при перезапуске
            Vars.Options.Map.RestoreRoutesWaypoints = checkBoxRestoreWptsRts.Checked;

            //окно настроек Wikimapia
            Vars.Options.Map.IsFormWikimpiaToolbarShow = checkBoxIsFormWikimapiaToolbarShow.Checked;

            //язык карты
            if (comboBoxMapLanguange.SelectedIndex == 0)
                Vars.Options.Map.MapLanguange = LanguageType.Russian;
            if (comboBoxMapLanguange.SelectedIndex == 1)
                Vars.Options.Map.MapLanguange = LanguageType.English;
            if (comboBoxMapLanguange.SelectedIndex == 2)
                Vars.Options.Map.MapLanguange = LanguageType.German;

            //метод отптимального маршрута
            if (trackBarOptimalMethodType.Value == 2)
                Vars.Options.Map.OptimalRouteMethod = OptimalMethodType.BranchBounds;
            if (trackBarOptimalMethodType.Value == 3)
                Vars.Options.Map.OptimalRouteMethod = OptimalMethodType.Greedy;
            if (trackBarOptimalMethodType.Value == 0)
                Vars.Options.Map.OptimalRouteMethod = OptimalMethodType.FullSearch;
            if (trackBarOptimalMethodType.Value == 1)
                Vars.Options.Map.OptimalRouteMethod = OptimalMethodType.PolarSearch;

            //глубина рекурсивного перебора
            Vars.Options.Map.MaxFullSearchNodes = (int)numericUpDownRecursiveDepth.Value;

            //использование маршрутизации
            Vars.Options.Map.UseRouterInOptimal = checkBoxUseRouter.Checked;

            //метод ветвей и границ для рекурсивного перебора
            Vars.Options.Map.UseBranchBoundsInPolarSearch = checkBoxUseBranchBounds.Checked;

            //Не показывать маркеры азимутов
            Vars.Options.Map.ShowAziMarkers = checkBoxShowAziMarkers.Checked;

            #endregion

            #region Источники данных

            //поставщик геокодера
            if (comboBoxGeocodingProvider.SelectedIndex == 0)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Yandex;
            if (comboBoxGeocodingProvider.SelectedIndex == 1)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Google;
            if (comboBoxGeocodingProvider.SelectedIndex == 2)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Nominatim;
            if (comboBoxGeocodingProvider.SelectedIndex == 3)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Arcgis;

            //поставщик  высоты
            if (comboBoxGeoInfoProvider.SelectedIndex == 0)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.Google;
            if (comboBoxGeoInfoProvider.SelectedIndex == 1)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.GTOPO30;
            if (comboBoxGeoInfoProvider.SelectedIndex == 2)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.ETOPO;

            //использовать кэш
            Vars.Options.DataSources.UseGeocoderCache = checkBoxUseCacheGeocoder.Checked;
            Vars.Options.DataSources.UseMapLayerCache = checkBoxUseMaplayerCache.Checked;
            Vars.Options.DataSources.UseImagesCache = checkBoxUseImageCache.Checked;
            Vars.Options.DataSources.UseExtInfoCache = checkBoxUseExtInfoCache.Checked;

            //длительность хранения кэша
            Vars.Options.DataSources.MaxImageCacheDays = Convert.ToDouble(numericUpDownImagesCacheDuration.Value);
            Vars.Options.DataSources.MaxVectorLayersCacheDays = Convert.ToDouble(numericUpDownMaplayersCacheDuration.Value);
            Vars.Options.DataSources.MaxExtInfoCacheDays = Convert.ToDouble(numericUpDownExtInfoCacheDuration.Value);
            Vars.Options.DataSources.MaxGeocoderCacheDays = Convert.ToDouble(numericUpDownGeocoderCacheDuration.Value);

            //папка ETOPO
            Vars.Options.DataSources.ETOPODBFolder = textBoxDBETOPOFolder.Text;

            #endregion

            #region Сервисы

            //поставщик прокладки маршрута
            if (comboBoxRoutePathingProvider.SelectedIndex == 0)
                Vars.Options.Services.PathRouteProvider = PathRouteProvider.Google;
            if (comboBoxRoutePathingProvider.SelectedIndex == 1)
                Vars.Options.Services.PathRouteProvider = PathRouteProvider.Yandex;

            //тип маршрута (пешком, на машине)
            if (comboBoxPathRouteMode.SelectedIndex == 0)
                Vars.Options.Services.PathRouteMode = PathRouteMode.Walk;
            if (comboBoxPathRouteMode.SelectedIndex == 1)
                Vars.Options.Services.PathRouteMode = PathRouteMode.Driving;

            //изменять созданный маршрут
            Vars.Options.Services.ChangePathedRoute = checkBoxEditAfterPathing.Checked;

            //кэш ФС при построении
            Vars.Options.Services.UseFSCacheForCreatingRoutes = checkBoxUseFSCacheForCreatingRoutes.Checked;

            //поставщик сокращения ссылок
            if (comboBoxLinkShorter.SelectedIndex == 0)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.Clck;
            if (comboBoxLinkShorter.SelectedIndex == 1)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.Qps;
            if (comboBoxLinkShorter.SelectedIndex == 2)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.VK;
            if (comboBoxLinkShorter.SelectedIndex == 3)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.Bitly;


            #endregion

            #region Графики

            Vars.Options.Graphs.isXKm = checkBoxXKm.Checked;
            Vars.Options.Graphs.isYKm = checkBoxYKm.Checked;
            Vars.Options.Graphs.IntermediateDistance = double.Parse(textBoxIntermediateDistance.Text.Replace('.', Vars.DecimalSeparator));
            Vars.Options.Graphs.IsAddIntermediatePoints = checkBoxIsAddIntermediatePoints.Checked;
            #endregion

            Vars.Options.Save(Application.StartupPath + Resources.options_folder);
            Program.AcceptOptions();

            this.Close();
        }

        /// <summary>
        /// отмена изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// выбор папки базы данных GTOPO2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectETOPOFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (Vars.Options.DataSources.ETOPODBFolder != null)
                fb.SelectedPath = Vars.Options.DataSources.ETOPODBFolder;
            else
                fb.SelectedPath = Application.StartupPath;

            //пока пользователь не выберет подходящую папку или пока не отменит выбор папки
            DialogResult dr = System.Windows.Forms.DialogResult.OK;
            while (dr == System.Windows.Forms.DialogResult.OK)
            {
                //вывод диалога
                dr = fb.ShowDialog(this);

                //если отмена выбора, то выход из цикла
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    break;
                //если ОК, то проверка папки
                if (dr == System.Windows.Forms.DialogResult.OK)
                    //если папка не подходит, то заново открываем диалог
                    if (!ETOPOProvider.DatabaseInstalled(fb.SelectedPath))
                    {
                        MessageBox.Show("В этой папке не найдено поддерживаемой базы данных или база данных повреждена");
                        continue;
                    }
                    else //если папка подходит, то выбор базы и выход из цикла
                    {
                        textBoxDBETOPOFolder.Text = fb.SelectedPath;
                        new ToolTip().SetToolTip(this.textBoxDBETOPOFolder, fb.SelectedPath);
                        break;
                    }
            }
        }

        /// <summary>
        /// очистка кэша
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearCache_Click(object sender, EventArgs e)
        {
            bool doRestart = false;
            if (checkBoxCacheMap.Checked)
            {
                Vars.clearMapCacheAfterExit = true;
                if (MessageBox.Show(this, "Кэш карт будет очищен только после перезагрузки программы. Перезагрузить сейчас?", "Очистка", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    doRestart = true;
            }
            if (checkBoxCacheGeocoder.Checked)
                Vars.dataCache.ClearGeocoder();
            if (checkBoxCacheAltitudes.Checked)
                Vars.dataCache.ClearAltitudes();
            if (checkBoxCacheHttp.Checked)
                Directory.Delete(Application.StartupPath + Resources.cache_directory + "\\http_cache", true);
            if (doRestart)
            {
                Vars.needRestart = true;
                Application.Exit();
            }
            else
                MessageBox.Show("Кэш очищен!");
        }

        /// <summary>
        /// добавление ярлыка на рабочий стол
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShortcutDesktop_Click(object sender, EventArgs e)
        {
            WshShell shell = new WshShell();

            //путь к ярлыку
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\TrackConverter.lnk";

            //создаем объект ярлыка
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            //задаем свойства для ярлыка
            //описание ярлыка в всплывающей подсказке
            shortcut.Description = "Редактор маршрутов";
            //горячая клавиша
            shortcut.Hotkey = "Ctrl+Shift+M";

            //путь к самой программе
            shortcut.TargetPath = Application.ExecutablePath;

            //Создаем ярлык
            shortcut.Save();
            MessageBox.Show("Ярлык создан!");
        }

        /// <summary>
        /// изменение описания оптимального метода маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarOptimalMethodType_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownRecursiveDepth.Enabled = trackBarOptimalMethodType.Value == 1;
            labelDepth.Enabled = trackBarOptimalMethodType.Value == 1;
            checkBoxUseBranchBounds.Enabled = trackBarOptimalMethodType.Value == 1;

            switch (trackBarOptimalMethodType.Value)
            {
                //полный перебор
                case 0:
                    textBoxOptimalMethodDescription.Text = Resources.DescriptionFullSearch;
                    break;

                //полярный перебор
                case 1:
                    textBoxOptimalMethodDescription.Text = Resources.DescriptionPolarSearch;
                    break;

                //метод ветвей и границ
                case 2:
                    textBoxOptimalMethodDescription.Text = Resources.DescriptionBranchBounds;
                    break;

                //жадный алгоритм
                case 3:
                    textBoxOptimalMethodDescription.Text = Resources.DescriptionGreedy;
                    break;
            }
        }

        /// <summary>
        /// ссылка на страницу загрузки ETOPO2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelDownloadETOPO2_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ngdc.noaa.gov/mgg/global/relief/ETOPO2/ETOPO2v2-2006/ETOPO2v2g/raw_binary/");
        }

        private void linkLabelDownloadETOPO1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ngdc.noaa.gov/mgg/global/relief/ETOPO1/data/");
        }

        private void linkLabelDownloadETOPO5_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ngdc.noaa.gov/mgg/global/relief/ETOPO5/");
        }

        /// <summary>
        /// авторизация на викимапии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWikimapiaLogin_Click(object sender, EventArgs e)
        {
            string login = textBoxWikimapiaLogin.Text;
            string password = textBoxWikimapiaPassword.Text;
            string success = Wikimapia.ServiceEngine.Login(login, password);
            if (success == "200")
            {
                buttonWikimapiaLogout.Enabled = true;
                buttonWikimapiaLogin.Enabled = false;
                Vars.Options.Auth.WikimapiaLogin = login;
                Vars.Options.Auth.WikimapiaPassword = password;
            }
            else
                MessageBox.Show(this, "Неверный логин или пароль", "Авторизация Wikimapia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        /// <summary>
        /// выход из викимапии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWikimapiaLogout_Click(object sender, EventArgs e)
        {
            Wikimapia.ServiceEngine.Logout();
            buttonWikimapiaLogout.Enabled = false;
            buttonWikimapiaLogin.Enabled = true;
            Vars.Options.Auth.WikimapiaLogin = "";
            Vars.Options.Auth.WikimapiaPassword = "";
        }

        /// <summary>
        /// переключение активности элементов настройки кэша на Источники данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxUseCache_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownGeocoderCacheDuration.Enabled = checkBoxUseCacheGeocoder.Checked;
            labelGeocoder.Enabled = checkBoxUseCacheGeocoder.Checked;

            numericUpDownExtInfoCacheDuration.Enabled = checkBoxUseExtInfoCache.Checked;
            labelExtInfos.Enabled = checkBoxUseExtInfoCache.Checked;

            numericUpDownImagesCacheDuration.Enabled = checkBoxUseImageCache.Checked;
            labelImages.Enabled = checkBoxUseImageCache.Checked;

            numericUpDownMaplayersCacheDuration.Enabled = checkBoxUseMaplayerCache.Checked;
            labelMaplayers.Enabled = checkBoxUseMaplayerCache.Checked;
        }
    }
}
