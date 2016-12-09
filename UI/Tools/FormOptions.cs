using GMap.NET;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.Local.ETOPO2;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common;

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
            checkBoxIsLoadETOPO2OnStart.Checked = Vars.Options.Common.IsLoadETOPO2OnStart;

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


            switch (Vars.Options.Converter.DistanceMethodType)
            {
                case DistanceMethodType.GaverSin:
                    radioButtonGaversin.Checked = true;
                    break;
                case DistanceMethodType.PifagorTeory:
                    radioButtonPifagor.Checked = true;
                    break;
                case DistanceMethodType.SphereSin:
                    radioButtonSphere.Checked = true;
                    break;
                case DistanceMethodType.ModGaverSin:
                    radioButtonModGaver.Checked = true;
                    break;
            }

            textBoxMagnNPLat.Text = Vars.Options.Converter.NorthPoleLatitude.ToString();
            textBoxMagnNPLon.Text = Vars.Options.Converter.NorthPoleLongitude.ToString();
            textBoxMinimumRiseInterval.Text = Vars.Options.Converter.MinimumRiseInterval.ToString("00.00");
            checkBoxIsapproximate.Checked = Vars.Options.Converter.IsApproximateAltitudes;
            numericUpDownAmount.Value = Vars.Options.Converter.ApproximateAmount;

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
                case GeoInfoProvider.ETOPO2:
                    comboBoxGeoInfoProvider.SelectedIndex = 2;
                    break;
            }

            //использовать кэш
            checkBoxUseCacheGeocoder.Checked=Vars.Options.DataSources.UseGeocoderCache ;

            //папка ETOPO2
            textBoxDBETOPOFolder.Text = Vars.Options.DataSources.ETOPO2DBFolder;
            new ToolTip().SetToolTip(textBoxDBETOPOFolder, Vars.Options.DataSources.ETOPO2DBFolder);

            #endregion

            #region Карта

            //сохранять точки и маршрутв при перезапуске
            checkBoxRestoreWptsRts.Checked = Vars.Options.Map.RestoreRoutesWaypoints;

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
                case OptimalMethodType.RecursiveEnum:
                    trackBarOptimalMethodType.Value = 1;
                    break;
                default:
                    trackBarOptimalMethodType.Value = 1;
                    break;
            }

            //глубина рекурсивного перебора
            numericUpDownRecursiveDepth.Value = Vars.Options.Map.MaxFullSearchNodes;


            //использование маршрутизации
           checkBoxUseRouter.Checked =Vars.Options.Map.UseRouterInOptimal;

            //метод ветвей и границ для рекурсивного перебора
            checkBoxUseBranchBounds.Checked = Vars.Options.Map.UseBranchBoundsInRecurEnum;

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
            }



            //изменять созданный маршрут
            checkBoxEditAfterPathing.Checked = Vars.Options.Services.ChangePathedRoute;

            #endregion

            #region Графики

            checkBoxXKm.Checked = Vars.Options.Graphs.isXKm;
            checkBoxYKm.Checked = Vars.Options.Graphs.isYKm;
            textBoxIntermediateDistance.Text = Vars.Options.Graphs.IntermediateDistance.ToString();
            checkBoxIsAddIntermediatePoints.Checked = Vars.Options.Graphs.IsAddIntermediatePoints;

            #endregion


            new ToolTip().SetToolTip(radioButtonGaversin, "Подходит для любых расстояний, если точки не лежат на концах диаметра");
            new ToolTip().SetToolTip(radioButtonModGaver, "Подходит для любых расстояний");
            new ToolTip().SetToolTip(radioButtonPifagor, "Подходит для маленьких расстояний. При вычислении используется значение высоты, если оно известно");
            new ToolTip().SetToolTip(radioButtonSphere, "Подходит для больших расстояний");
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
            Vars.Options.Common.IsLoadETOPO2OnStart = checkBoxIsLoadETOPO2OnStart.Checked;

            //основное окно
            if (comboBoxOptionsFormat.SelectedIndex == 0)
                Vars.Options.Format = OptionsFormat.XML;
            if (comboBoxOptionsFormat.SelectedIndex == 1)
                Vars.Options.Format = OptionsFormat.JSON;

            #endregion

            #region Конвертер

            if (radioButtonGaversin.Checked)
                Vars.Options.Converter.DistanceMethodType = DistanceMethodType.GaverSin;
            if (radioButtonPifagor.Checked)
                Vars.Options.Converter.DistanceMethodType = DistanceMethodType.PifagorTeory;
            if (radioButtonSphere.Checked)
                Vars.Options.Converter.DistanceMethodType = DistanceMethodType.SphereSin;
            if (radioButtonModGaver.Checked)
                Vars.Options.Converter.DistanceMethodType = DistanceMethodType.ModGaverSin;

            Vars.Options.Converter.NorthPoleLatitude = double.Parse(textBoxMagnNPLat.Text.Replace('.', ','));
            Vars.Options.Converter.NorthPoleLongitude = double.Parse(textBoxMagnNPLon.Text.Replace('.', ','));
            Vars.Options.Converter.MinimumRiseInterval = double.Parse(textBoxMinimumRiseInterval.Text.Replace('.', ','));
            Vars.Options.Converter.IsApproximateAltitudes = checkBoxIsapproximate.Checked;
            Vars.Options.Converter.ApproximateAmount = numericUpDownAmount.Value;

            #endregion

            #region Карта

            //восстанавливать точки и машрруты при перезапуске
            Vars.Options.Map.RestoreRoutesWaypoints = checkBoxRestoreWptsRts.Checked;

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
                Vars.Options.Map.OptimalRouteMethod = OptimalMethodType.RecursiveEnum;

            //глубина рекурсивного перебора
            Vars.Options.Map.MaxFullSearchNodes = (int)numericUpDownRecursiveDepth.Value;

            //использование маршрутизации
            Vars.Options.Map.UseRouterInOptimal = checkBoxUseRouter.Checked;

            //метод ветвей и границ для рекурсивного перебора
            Vars.Options.Map.UseBranchBoundsInRecurEnum = checkBoxUseBranchBounds.Checked;


            #endregion

            #region Источники данных

            //поставщик геокодера
            if (comboBoxGeocodingProvider.SelectedIndex == 0)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Yandex;
            if (comboBoxGeocodingProvider.SelectedIndex == 1)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Google;
            if (comboBoxGeocodingProvider.SelectedIndex == 2)
                Vars.Options.DataSources.GeoCoderProvider = GeoCoderProvider.Nominatim;

            //поставщик  высоты
            if (comboBoxGeoInfoProvider.SelectedIndex == 0)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.Google;
            if (comboBoxGeoInfoProvider.SelectedIndex == 1)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.GTOPO30;
            if (comboBoxGeoInfoProvider.SelectedIndex == 2)
                Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.ETOPO2;

            //использовать кэш
            Vars.Options.DataSources.UseGeocoderCache = checkBoxUseCacheGeocoder.Checked;

            //папка ETOPO2
            Vars.Options.DataSources.ETOPO2DBFolder = textBoxDBETOPOFolder.Text;

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

            //поставщик сокращения ссылок
            if (comboBoxLinkShorter.SelectedIndex == 0)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.Clck;
            if (comboBoxLinkShorter.SelectedIndex == 1)
                Vars.Options.Services.LinkShorterProvider = LinkShorterProvider.Qps;


            #endregion

            #region Графики

            Vars.Options.Graphs.isXKm = checkBoxXKm.Checked;
            Vars.Options.Graphs.isYKm = checkBoxYKm.Checked;
            Vars.Options.Graphs.IntermediateDistance = double.Parse(textBoxIntermediateDistance.Text.Replace('.', ','));
            Vars.Options.Graphs.IsAddIntermediatePoints = checkBoxIsAddIntermediatePoints.Checked;
            #endregion

            Vars.Options.Save(Application.StartupPath + Resources.options_folder);

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
            if (Vars.Options.DataSources.ETOPO2DBFolder != null)
                fb.SelectedPath = Vars.Options.DataSources.ETOPO2DBFolder;
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
                    if (!ETOPO2Provider.DatabaseInstalled(fb.SelectedPath))
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
            try
            {
                Directory.Delete(Application.StartupPath + Resources.cache_directory, true);
            }
            catch (Exception ex)
            { }
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
        }

        /// <summary>
        /// перевключение 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxIsapproximate_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownAmount.Enabled = checkBoxIsapproximate.Checked;
            labelApproxPower.Enabled = checkBoxIsapproximate.Checked;
        }

        /// <summary>
        /// изменение активности поля расстояния промежуточных точек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxIsAddIntermediatePoints_CheckedChanged(object sender, EventArgs e)
        {
            textBoxIntermediateDistance.Enabled = checkBoxIsAddIntermediatePoints.Checked;
            labelIntermedDist.Enabled = checkBoxIsAddIntermediatePoints.Enabled;
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

                //рекурсивный перебор
                case 1:
                    textBoxOptimalMethodDescription.Text = Resources.DescriptionRecursiveEnum;
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
    }
}
