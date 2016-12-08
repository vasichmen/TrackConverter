using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO2
{
    /// <summary>
    /// базовый класс базы данных ETOPO2
    /// </summary>
    abstract class BaseGrid : IDatabase
    {
        protected int columns;
        protected int rows;
        protected string folder;
        protected string dataFile;
        protected string headerFile;
        protected double cellSize;
        protected Coordinate LLCorner;



        /// <summary>
        /// Папка с файлами базы данных
        /// </summary>
        public string Folder
        {
            get
            {
                if (this.folder != "difference")
                    return this.folder;
                else
                    throw new InvalidOperationException("Файлы базы данных находятся в разных папках");
            }
        }

        /// <summary>
        /// Путь к файлу данных
        /// </summary>
        public string DataFile { get { return this.dataFile; } }

        /// <summary>
        /// Путь к заголовочному файлу
        /// </summary>
        public string HeaderFile { get { return this.headerFile; } }

        /// <summary>
        /// количество Столбцов
        /// </summary>
        public int Columns { get { return this.columns; } }

        /// <summary>
        /// Количество строк
        /// </summary>
        public int Rows { get { return this.rows; } }

        /// <summary>
        /// Размер одной строки и одного столбца в градусах
        /// </summary>
        public double CellSize { get { return this.cellSize; } }

        /// <summary>
        /// тип базы данных
        /// </summary>
        public ETOPO2DBType Type { get; set; }

        /// <summary>
        /// Возвращает элемент по заданным столбцу и строке
        /// </summary>
        /// <param name="i">номер строки</param>
        /// <param name="j">номер столбца</param>
        /// <returns></returns>
        public abstract double this[int i, int j] { get; }

        /// <summary>
        /// возвращает высоту по заданным координатам точки
        /// </summary>
        /// <param name="coordinate">географические координаты точки</param>
        /// <returns></returns>
        public double this[Coordinate coordinate]
        {
            get
            {
                Point pt = GeoToLocal(coordinate);
                return this[pt.Y, pt.X];
            }
        }




        /// <summary>
        /// создает базу данных, но не загружает ее 
        /// </summary>
        /// <param name="fHeader"></param>
        /// <param name="fData"></param>
        protected BaseGrid(string fHeader, string fData)
        {
            string hFold = Path.GetDirectoryName(fHeader);
            string dFold = Path.GetDirectoryName(fData);
            if (hFold != dFold)
                this.folder = "difference";
            else
                this.folder = hFold;

            this.headerFile = fHeader;
            this.dataFile = fData;
        }

        /// <summary>
        /// преобразование географических координат в локальные. X - столбец, Y - строка
        /// </summary>
        /// <param name="coordinate">географические координаты точки</param>
        /// <returns></returns>
        protected Point GeoToLocal(Coordinate coordinate)
        {
            double lat = coordinate.Latitude.TotalDegrees;
            double lon = coordinate.Longitude.TotalDegrees;
            double in1cell = this.cellSize;
            double ic = rows / 2;
            double jc = columns / 2;

            Point res = new Point();

            res.Y = (int)Math.Round(ic - lat / in1cell, 0);
            res.X = (int)Math.Round(jc + lon / in1cell, 0);

            return res;
        }

        /// <summary>
        /// загружает базу данных
        /// </summary>
        protected abstract void LoadDatabase();



        /// <summary>
        /// Узнать тип базы данных из заголовочного файла
        /// </summary>
        /// <param name="headerFile">путь к заголовочному файлу базы данных</param>
        /// <returns></returns>
        public static ETOPO2DBType ReadDBType(string headerFile)
        {
            if (Path.GetExtension(headerFile) == ".sq3")
                return ETOPO2DBType.SQLite;
            using (StreamReader sr = new StreamReader(headerFile))
            {
                do
                {
                    string line = sr.ReadLine();
                    if (line.Contains("NUMBERTYPE"))
                    {
                        string type = line.Substring(line.IndexOf(" ")).Trim();
                        if (type == "2_BYTE_INTEGER")
                            return ETOPO2DBType.Int16;
                        if (type == "4_BYTE_FLOAT")
                            return ETOPO2DBType.Float;
                    }
                }
                while (!sr.EndOfStream);
            }
            throw new Exception("Не удалось определить тип базы данных. В файле " + headerFile + " нет информации о типе баы данных");
        }

        /// <summary>
        /// сохраняет базу данных в SQLite БД в указанном файле
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="callback">Действие, выполняемое во время сохранения</param>
        public void ExportToSQL(string FileName, Action<string> callback = null)
        {
            //СОЗДАНИЕ БД
            string table_name = "etopo2_data_table";
            File.Delete(FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            SQLiteConnection.CreateFile(FileName);
            SQLiteConnection con = new SQLiteConnection("Data Source = " + FileName + "; Version = 3; Synchronous = 0;");
            con.Open();
            SQLiteCommand commCreate = new SQLiteCommand(
                @"CREATE TABLE " + table_name + @"
                (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                row INTEGER NOT NULL,
                column INTEGER NOT NULL,
                altitude double
                );",
                con);
            commCreate.ExecuteNonQuery();
            con.Close();

            //ЭКСПОРТ ДАННЫХ
            double all = this.Columns * this.Rows;
            double c = 0;
            con.Open();

            for (int i = 0; i < this.Rows; i++)
            {
                SQLiteCommand cm = con.CreateCommand();
                SQLiteTransaction trans = con.BeginTransaction();
               
                for (int j = 0; j <this.Columns; j++)
                {
                    cm.CommandText = @"INSERT INTO '" + table_name + @"'('row','column','altitude') VALUES (" + i + "," + j + "," + this[i, j] + "); ";
                    cm.ExecuteNonQuery();
                    c++;
                    if (c % 10000 == 0 && callback != null)
                        callback.Invoke("Сохранение ETOPO2 в SQLite файл. Завершено: " + (c / all * 100d).ToString("0.0") + "%");
                }
                trans.Commit();
            }
            con.Close();
        }

    }
}
