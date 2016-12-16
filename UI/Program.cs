#define Debug

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.Options;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.Local.ETOPO;
using TrackConverter.Lib.Mathematic.Geodesy.Systems;
using TrackConverter.Lib.Tracking;
using TrackConverter.Res.Properties;
using TrackConverter.UI.Common;
using TrackConverter.UI.Converter;
using TrackConverter.UI.Map;
using TrackConverter.UI.Tools;

namespace TrackConverter.UI
{
    /// <summary>
    /// основной класс программы
    /// </summary>
    static class Program
    {
        #region Окна

        /// <summary>
        /// основное окно конвертера
        /// </summary>
        public static FormConverter winConverter;

        /// <summary>
        /// окно с картой
        /// </summary>
        public static FormMap winMap;

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
        /// Окно графиков высот
        /// </summary>
        public static FormElevVisual winElevVisual;

        /// <summary>
        /// Основное окно программы
        /// </summary>
        public static FormContainer winMain;

        /// <summary>
        /// Окно редактирования точек
        /// </summary>
        public static FormPoints winPoints;

        #endregion

        #region Свойства окон

        /// <summary>
        /// Истина, если окно конвертера не содано или закрыто
        /// </summary>
        public static bool winConverterNullOrDisposed { get { return winConverter == null || winConverter.IsDisposed; } }

        /// <summary>
        /// Истина, если окно карты не содано или закрыто
        /// </summary>
        public static bool winMapNullOrDisposed { get { return winMap == null || winMap.IsDisposed; } }

        /// <summary>
        /// Истина, если окно объединения треков не содано или закрыто
        /// </summary>
        public static bool winJoinTrackNullOrDisposed { get { return winJoinTrack == null || winJoinTrack.IsDisposed; } }

        /// <summary>
        /// Истина, если окно настроек не содано или закрыто
        /// </summary>
        public static bool winOptionsNullOrDisposed { get { return winOptions == null || winOptions.IsDisposed; } }

        /// <summary>
        /// Истина, если окно навигации не содано или закрыто
        /// </summary>
        public static bool winNavigatorNullOrDisposed { get { return winNavigator == null || winNavigator.IsDisposed; } }

        /// <summary>
        /// Истина, если окно редактирования маршрута не содано или закрыто
        /// </summary>
        public static bool winRouteEditorNullOrDisposed { get { return winRouteEditor == null || winRouteEditor.IsDisposed; } }

        /// <summary>
        /// Истина, если окно сравнения маршрутов не содано или закрыто
        /// </summary>
        public static bool winCompareTrackNullOrDisposed { get { return winCompareTrack == null || winCompareTrack.IsDisposed; } }

        /// <summary>
        /// Истина, если окно ожидания не содано или закрыто
        /// </summary>
        public static bool winWaitingNullOrDisposed { get { return winWaiting == null || winWaiting.IsDisposed; } }

        /// <summary>
        /// истина, если окно редактирования точек не создано или закрыто
        /// </summary>
        public static bool winPointsNullOrDisposed { get { return winWaiting == null || winWaiting.IsDisposed; } }

        /// <summary>
        /// Истина, если окно графиков высот не содано или закрыто
        /// </summary>
        public static bool winElevVisualNullOrDisposed { get { return winElevVisual == null || winElevVisual.IsDisposed; } }

        #endregion

        /// <summary>
        /// иконка в панели задач
        /// </summary>
        static TrayIcon trayIcon = null;


        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
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
                Application.ApplicationExit += Application_ApplicationExit;

                //настройки программы
                Vars.Options = Options.Load(Application.StartupPath + Resources.options_folder);

                //проверка файлов программы
                CheckFiles();

                #endregion

                #region окна

                //создание основного окна
                winMain = new FormContainer()
                {
                    WindowState = Vars.Options.Container.WinState,
                    Size = Vars.Options.Container.WinSize,
                    Left = Vars.Options.Container.WinPosition.X,
                    Top = Vars.Options.Container.WinPosition.Y
                };

                //дочерние окна
                winMap = new FormMap() { MdiParent = winMain };
                winElevVisual = new FormElevVisual(null) { MdiParent = winMain };
                winPoints = new FormPoints() { MdiParent = winMain };
                winConverter = new FormConverter(args) { MdiParent = winMain };

                //создание окна ожидания
                winWaiting = new FormWaiting();

                //открытие окна навигации, если требуется
                if (Vars.Options.Map.IsFormNavigatorShow)
                {
                    winNavigator = new FormMapNavigator();
                    winNavigator.Show(winMain);
                }

                winMap.Show();
                winConverter.Show();
                winElevVisual.Show();
                winPoints.Show();

                #endregion

                #region настройки объектов

                //открытие БД кэша геокодера
                Vars.dataCache = new SQLiteCache(Application.StartupPath + Resources.cache_directory + "\\geocoder");

                //метод загрузки базы данных ETOPO
                Vars.TaskLoadingETOPO = GetETOPOLoadingTask();

                //применение настроек
                AcceptOptions();

                #endregion

                //запуск основного окна
                Application.Run(winMain);

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
        }

        /// <summary>
        /// обновление окон после изменния выделенного маршрута
        /// </summary>
        public static void RefreshWindows(Form sender)
        {
            if (sender != winMap)
                winMap.RefreshData();
            if (sender != winElevVisual)
                winElevVisual.RefreshData();
            if (sender != winPoints)
                winPoints.RefreshData();
            if (sender.GetType() != typeof(FormConverter))
                winConverter.RefreshSelectedTrack();
        }

        /// <summary>
        /// выход из приложения и сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Vars.Options.Save(Application.StartupPath + Resources.options_folder);
            if (Vars.dataCache != null)
                Vars.dataCache.Dispose();
            trayIcon.UnShow();
        }

        /// <summary>
        /// проверка файлов программы
        /// <param name="opts">настройки программы</param>
        /// </summary>
        private static void CheckFiles()
        {
            if (!File.Exists(Application.StartupPath + Resources.lib_dll_file))
            {
                MessageBox.Show(null, "Библиотека " + Resources.lib_dll_file + " не была найдена в папке программы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.gmapcore_dll_file))
            {
                MessageBox.Show(null, "Библиотека " + Resources.gmapcore_dll_file + " не была найдена в папке программы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.gmapwf_dll_file))
            {
                MessageBox.Show(null, "Библиотека " + Resources.gmapwf_dll_file + " не была найдена в папке программы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.help_doc_file))
            {
                MessageBox.Show(null, "Файл справки " + Resources.help_doc_file + "  не был найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.gpxschema_scheme_file))
            {
                MessageBox.Show(null, "Файл формата GPX " + Resources.gpxschema_scheme_file + " не был найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.kmlschema_scheme_file))
            {
                MessageBox.Show(null, "Файл формата KML " + Resources.kmlschema_scheme_file + "  не был найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.osmxschema_scheme_file))
            {
                MessageBox.Show(null, "Файл формата OSM " + Resources.osmxschema_scheme_file + " не был найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(Application.StartupPath + Resources.ziplib_dll_file))
            {
                MessageBox.Show(null, "Библиотека " + Resources.ziplib_dll_file + " не была найдена в папке программы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (MapProviderRecord mpr in Vars.Options.Map.AllProviders)
                if (!File.Exists(Application.StartupPath + mpr.IconName))
                {
                    MessageBox.Show(null, "Файл карты " + mpr.IconName + " не был найден в папке " + Application.StartupPath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Vars.TaskLoadingETOPO = GetETOPOLoadingTask();
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
        private static Task GetETOPOLoadingTask()
        {
            return new Task(new Action(() =>
             {
                 try
                 {
                     if (GeoInfo.IsETOPOReady)
                         GeoInfo.ETOPOProvider = new ETOPOProvider(Vars.Options.DataSources.ETOPODBFolder, winMain.setCurrentOperation);
                     else throw new ApplicationException("База данных ETOPO не установлена. Укажите в настройках путь к файлам базы данных");
                 }
                 catch (Exception exc)
                 {
                     Vars.Options.DataSources.ETOPODBFolder = "";
                     Vars.Options.DataSources.GeoInfoProvider = GeoInfoProvider.Google;
                     MessageBox.Show(null, "Ошибка при загрузке базы данных ETOPO: " + exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     throw exc;
                 }
                 finally
                 {
                     winMain.EndOperation();
                 }
             }));
        }
    }
}

