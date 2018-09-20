/* Copyright 2017 vasich

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#define Debug

using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.Options;
using TrackConverter.Lib.Classes.ProviderRecords;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Data.Providers.Local.ETOPO;
using TrackConverter.Lib.Mathematic.Geodesy.Models;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common;
using TrackConverter.UI.Common.Dialogs;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Map;
using TrackConverter.UI.Shell;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI
{
    /// <summary>
    /// основной класс программы
    /// </summary>
    internal static class Program
    {
        #region Окна


        /// <summary>
        /// окно редактирования путешествия
        /// </summary>
        public static FormEditTrip winTripEditor;

        /// <summary>
        /// окно объединения треков
        /// </summary>
        public static FormJoinTracks winJoinTrack;

        /// <summary>
        /// окно настроек
        /// </summary>
        public static FormOptions winOptions;

        /// <summary>
        /// сравнение маршрутов
        /// </summary>
        public static FormTrackComparison winCompareTrack;

        /// <summary>
        /// окно навигатора
        /// </summary>
        public static FormMapNavigator winNavigator;

        /// <summary>
        /// окно инструментов редактора маршрута
        /// </summary>
        public static FormEditRoute winRouteEditor;

        /// <summary>
        /// окно ожидания
        /// </summary>
        public static FormWaiting winWaiting;

        /// <summary>
        /// Основное окно программы
        /// </summary>
        public static FormMain winMain;

        /// <summary>
        /// Окно сохранения карты
        /// </summary>
        public static FormSaveMap winSaveMap;

        #endregion


        /// <summary>
        /// иконка в панели задач
        /// </summary>
        private static TrayIcon trayIcon = null;


        internal static bool winNavigatorNullOrDisposed { get { return winNavigator == null || winNavigator.IsDisposed; } }
        public static bool winJoinTrackNullOrDisposed { get { return winJoinTrack == null || winJoinTrack.IsDisposed; } }
        public static bool winCompareTrackNullOrDisposed { get { return winJoinTrack == null || winJoinTrack.IsDisposed; } }
        public static bool winWaitingNullOrDisposed { get { return winWaiting == null || winWaiting.IsDisposed; } }
        public static bool winSaveMapNullOrDisposed { get { return winSaveMap == null || winSaveMap.IsDisposed; } }
        public static bool winOptionsNullOrDisposed { get { return winOptions == null || winOptions.IsDisposed; } }


        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
#if (!DEBUG)
            try
            {
#endif

            #region система

            //установка параметров отображения
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            //активация иконки в панели задач
            trayIcon = new TrayIcon(
                //действие при двойном нажатии
                new Action(() =>
                {
                    winMain.WindowState = FormWindowState.Normal;
                }),

                //контекстное меню
                new ContextMenu(
                    new MenuItem[] { //элементы меню
                            new MenuItem( //кнопка выход
                                "Выход", //заголовок
                                new EventHandler( //действие при нажатии
                                    new Action<object, EventArgs>((f1, f2) => { winMain.Close(); })
                                    )
                                )
                    }
                    )
                );
            trayIcon.Show();

            //обработчик выхода из приложения
            Application.ApplicationExit += application_ApplicationExit;

            //настройки программы
            Vars.Options = Options.Load(Application.StartupPath + Resources.options_folder);

            //проверка файлов программы
            checkFiles();

            //открытие БД кэша геокодера
            new Task(new Action(() => { Vars.dataCache = new Cache(Application.StartupPath + Resources.cache_directory); })).Start();


            //метод загрузки базы данных ETOPO
            Vars.TaskLoadingETOPO = getETOPOLoadingTask();



            #endregion

            #region настройки объектов

            //применение настроек
            AcceptOptions();

            #endregion

            #region создание окон

            //создание основного окна
            winMain = new FormMain(args)
            {
                WindowState = Vars.Options.Container.WinState,
                Size = Vars.Options.Container.WinSize,
                Left = Vars.Options.Container.WinPosition.X,
                Top = Vars.Options.Container.WinPosition.Y
            };

            //winMain.splitContainerHorizontalLeft.SplitterDistance  = Vars.Options.Container.HorizontalLeftSplitter;
            //winMain.splitContainerHorizontalRight.SplitterDistance = Vars.Options.Container.HorizontalRightSplitter;
            //winMain.splitContainerVertical.SplitterDistance = Vars.Options.Container.VerticalSplitter;

            //создание окна ожидания
            winWaiting = new FormWaiting();


            #endregion

            #region запись статистики, проверка версии

            new Task(new Action(() =>
            {
                Velomapa site = new Velomapa(); //связь с сайтом
                site.SendStatisticAsync(); //статистика

                //действие при проверке версии
                Action<VersionInfo> action = new Action<VersionInfo>((vi) =>
                {
                    float curVer = Vars.Options.Common.VersionInt;
                    if (vi.VersionInt > curVer)
                    {
                        FormUpdateDialog fud = new FormUpdateDialog(vi);
                        if (Vars.Options.Common.UpdateMode != UpdateDialogAnswer.AlwaysIgnore)
                            winMain.Invoke(new Action(() => fud.ShowDialog()));
                    }
                });
                site.GetVersionAsync(action); //проверка версии
            })
            ).Start();

            #endregion


            #region открытие окон

            //открытие окна навигации, если требуется
            if (Vars.Options.Map.IsFormNavigatorShow)
            {
                winNavigator = new FormMapNavigator();
                winNavigator.Show(winMain);
            }

            #endregion


            //запуск основного окна
            //Application.Run(winMain);
            Application.Run(winMain);

#if (!DEBUG)
            }
            catch (Exception ex) //запись ошибки в лог
            {
                MessageBox.Show(null, ex.Message, "Ошибка при запуске или неисправимая ошибка приложения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string file = Application.StartupPath + Resources.crash_file;
                StreamWriter sw = new StreamWriter(file, true);
                sw.WriteLine(ex.Message);
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine("Дополнительная информация: " + ex.HelpLink);
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine();
                sw.WriteLine("-------------------------------");
                sw.Close();
                return;
            }
#endif
        }


        /// <summary>
        /// выход из приложения и сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void application_ApplicationExit(object sender, EventArgs e)
        {
            GMaps.Instance.CancelTileCaching();
            Debug.Print("Application_ApplicationExit");
            Vars.Options.Save(Application.StartupPath + Resources.options_folder); //сохранение настроек
            Debug.Print("Option saved");

            if (Vars.dataCache != null)
                Vars.dataCache.Dispose(); //закрытие кэша
            Debug.Print("Cache closed");

            trayIcon.UnShow(); //иконка трея
            Debug.Print("Tray icon off");

            //очистка временных файлов
            try
            {
                if (Directory.Exists(Application.StartupPath + Resources.temp_directory))
                    Directory.Delete(Application.StartupPath + Resources.temp_directory, true);
            }
            catch (Exception exxx) { Debug.Print(exxx.Message); }
            finally { Debug.Print("Temp directory removed"); }

            //если требуется - очистка кэша карт
            if (Vars.clearMapCacheAfterExit)
                Directory.Delete(Application.StartupPath + Resources.cache_directory + "\\TileDBv5", true);
            else
                GMaps.Instance.OptimizeMapDb(null);

            //если требуется перезапуск
            if (Vars.needRestart)
                Process.Start(Application.ExecutablePath);

        }

        /// <summary>
        /// проверка файлов программы
        /// </summary>
        private static void checkFiles()
        {
            List<string> dlls = new List<string>() {
                Resources.dll_lib,
                Resources.dll_gmapcore,
                Resources.dll_gmapwf,
                Resources.dll_html_agility_pack,
                Resources.dll_newtonsoft_json,
                Resources.dll_res,
                Resources.dll_runtime,
                Resources.dll_SQLite,
                Resources.dll_zed_graph,
                Resources.dll_sqliteInterop64,
                Resources.dll_sqliteInterop86,
                Resources.dll_res_trackconverter,
                Resources.dll_res_zedgraph
            };



            if (!File.Exists(Application.StartupPath + Resources.help_doc_file))
            {
                MessageBox.Show(null, "Файл справки " + Resources.help_doc_file + "  не был найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            foreach (var n in dlls)
                if (!File.Exists(Application.StartupPath + n))
                {
                    MessageBox.Show(null, "Библиотека " + n + " не была найдена в папке программы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

            foreach (MapProviderRecord mpr in Vars.Options.Map.AllMapProviders)
                if (!File.Exists(Application.StartupPath + mpr.IconName))
                {
                    MessageBox.Show(null, "Файл карты " + mpr.IconName + " не был найден в папке " + Application.StartupPath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
            foreach (MapLayerProviderRecord lpr in Vars.Options.Map.AllLayerProviders)
                if (!File.Exists(Application.StartupPath + lpr.IconName))
                {
                    MessageBox.Show(null, "Файл слоя " + lpr.IconName + " не был найден в папке " + Application.StartupPath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }
        }

        /// <summary>
        /// применение новыx настроек программы
        /// </summary>
        internal static void AcceptOptions()
        {
            //эллипсоид
            //установка системы координат
            switch (Vars.Options.Converter.Geosystem)
            {
                case Geosystems.PZ90:
                    Vars.CurrentGeosystem = new PZ90();
                    break;
                case Geosystems.WGS84:
                    Vars.CurrentGeosystem = new WGS84();
                    break;
                default:
                    MessageBox.Show("Ошибка при создании системы координат: " + Vars.Options.Converter.Geosystem);
                    break;
            }

            //база данных ETOPO
            if (Vars.Options.DataSources.GeoInfoProvider == GeoInfoProvider.ETOPO)
            {
                if (Vars.TaskLoadingETOPO.Status == TaskStatus.RanToCompletion || Vars.TaskLoadingETOPO.IsCompleted)
                    Vars.TaskLoadingETOPO = getETOPOLoadingTask();
                winMain.BeginOperation();
                Vars.TaskLoadingETOPO.Start();
            }

            //язык карты
            GMapProvider.Language = Vars.Options.Map.MapLanguange;

        }

        /// <summary>
        /// получить задачу загрузки БД ЕТОРО2
        /// </summary>
        /// <returns></returns>
        private static Task getETOPOLoadingTask()
        {
            return new Task(new Action(() =>
             {
                 try
                 {
                     if (GeoInfo.IsETOPOReady)
                         GeoInfo.ETOPOProvider = new ETOPOProvider(Vars.Options.DataSources.ETOPODBFolder, winMain.SetCurrentOperation);
                     else throw new ApplicationException("База данных ETOPO не установлена. Укажите в настройках путь к файлам базы данных");
                 }
                 catch (Exception exc)
                 {
                     Vars.Options.DataSources.ETOPODBFolder = "";
                     Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.Google;
                     MessageBox.Show(null, "Ошибка при загрузке базы данных ETOPO: " + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 finally
                 {
                     winMain.EndOperation();
                 }
             }));
        }

    }
}