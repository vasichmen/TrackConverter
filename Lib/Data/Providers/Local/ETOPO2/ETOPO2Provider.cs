using GMap.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO2
{
    /// <summary>
    /// Данные о высоте из локальной базы данных ETOPO2. 
    /// </summary>
    public class ETOPO2Provider:IGeoInfoProvider
    {
        /// <summary>
        /// тип базы данных
        /// </summary>
        public ETOPO2DBType DBType { get { return database.Type; } }

        /// <summary>
        /// Точность в секундах
        /// </summary>
        public double Accuracy { get { return database.CellSize * 60; } }

        /// <summary>
        /// если истина, то это локальный источник данных
        /// </summary>
        public bool isLocal
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// база данных
        /// </summary>
        private IDatabase database;

        /// <summary>
        /// Создает новый экземпляр и загружает указанную базу данных
        /// </summary>
        /// <param name="databaseFolder"></param>
        /// <param name="callback">действие, выполняемой при загрузке БД</param>
        /// <exception cref="FileLoadException">Если при загрузке базы данных произошла ошибка</exception>
        public ETOPO2Provider(string databaseFolder, Action<string> callback = null)
        {
            if (callback != null)
                callback.Invoke("Идет загрузка базы данных ETOPO2");

            //проверка наличия заголовочного файла
            string[] hfiles = Directory.GetFiles(databaseFolder, "*.hdr", SearchOption.TopDirectoryOnly);
            if (hfiles.Length != 1)
            {
                string[] sqfile = Directory.GetFiles(databaseFolder,"*.sq3",SearchOption.TopDirectoryOnly);
                if (sqfile.Length > 0) {
                   hfiles = sqfile;
                }
                else
                    throw new FileLoadException ("В папке "+databaseFolder+" не обнаружено корректной базы данных");
            }
               
            string hfile = hfiles[0];

            //определение типа базы данных
            ETOPO2DBType dbt = BaseGrid.ReadDBType(hfile);

            //открытие базы данных в зависимости от типа
            switch (dbt)
            {
                case ETOPO2DBType.Float:
                    string[] ffiles = Directory.GetFiles(databaseFolder, "*.flt", SearchOption.TopDirectoryOnly);
                    if (ffiles.Length != 1)
                        throw new FileLoadException("Обнаружено несколько *.flt файлов данных или не найдено ни одного");
                    string ffile = ffiles[0];
                    this.database = new FloatDatabase(hfile, ffile);
                    break;
                case ETOPO2DBType.Int16:
                    string[] bfiles = Directory.GetFiles(databaseFolder, "*.bin", SearchOption.TopDirectoryOnly);
                    if (bfiles.Length != 1)
                        throw new FileLoadException("Обнаружено несколько *.bin файлов данных или не найдено ни одного");
                    string dfile = bfiles[0];
                    this.database = new Int16Database(hfile, dfile);
                    break;
                case ETOPO2DBType.SQLite:
                    this.database = new SQLiteDatabase(hfile);
                    break;
                default: throw new FileLoadException("Ошибка при открытии базы данных");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            //для сравнения
            //double control = new GeoInfo(GeoInfoProvider.Google).GetElevation(coordinate);

            double res = database[coordinate];
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double GetElevation(double lat, double lon)
        { return GetElevation(new Coordinate(lat, lon)); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double GetElevation(Coordinate.CoordinateRecord lat, Coordinate.CoordinateRecord lon)
        { return GetElevation(new Coordinate(lat, lon)); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GetElevation(PointLatLng point)
        { return GetElevation(new Coordinate(point)); }

        /// <summary>
        /// создает новый трек с высотами каждой точки
        /// </summary>
        /// <param name="track">трек</param>
        /// <returns>трек с высотами точек</returns>
        public TrackFile GetElevation(TrackFile track)
        {
            TrackFile res = new TrackFile();
            foreach (TrackPoint tp in track)
            {
                //Thread.Sleep(50);
                tp.MetrAltitude = GetElevation(tp.Coordinates);
                res.Add(tp);
            }
            return res;
        }

        /// <summary>
        /// возвращает истину, если в заданной папке есть поддерживаемая база данных
        /// </summary>
        /// <param name="databaseFolder"></param>
        /// <returns></returns>
        public static bool DatabaseInstalled(string databaseFolder)
        {
            //проверка наличия заголовочного файла
            string[] hfiles = Directory.GetFiles(databaseFolder, "*.hdr", SearchOption.TopDirectoryOnly);
            if (hfiles.Length != 1)
                return false;
            string hfile = hfiles[0];

            //определение типа базы данных
            ETOPO2DBType dbt = BaseGrid.ReadDBType(hfile);

            //открытие базы данных в зависимости от типа
            switch (dbt)
            {
                case ETOPO2DBType.Float:
                    string[] ffiles = Directory.GetFiles(databaseFolder, "*.flt", SearchOption.TopDirectoryOnly);
                    if (ffiles.Length != 1)
                        return false;
                    string ffile = ffiles[0];
                    break;
                case ETOPO2DBType.Int16:
                    string[] bfiles = Directory.GetFiles(databaseFolder, "*.bin", SearchOption.TopDirectoryOnly);
                    if (bfiles.Length != 1)
                        return false;
                    string dfile = bfiles[0];
                    break;
                default: return false;
            }
            return true;
        }
    }
}
