using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO
{
    /// <summary>
    /// базовый класс базы данных ETOPO
    /// </summary>
    internal abstract class BaseGrid: IDatabase
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
        public ETOPODBType Type { get; set; }

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
            double lat = coordinate.Latitude;
            double lon = coordinate.Longitude;
            double in1cell = this.cellSize;
            double ic = rows / 2;
            double jc = columns / 2;

            Point res = new Point
            {
                Y = (int)Math.Round(ic - lat / in1cell, 0),
                X = (int)Math.Round(jc + lon / in1cell, 0)
            };

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
        public static ETOPODBType ReadDBType(string headerFile)
        {
            if (Path.GetExtension(headerFile) == ".sq3")
                return ETOPODBType.SQLite;
            using (StreamReader sr = new StreamReader(headerFile))
            {
                do
                {
                    string line = sr.ReadLine();
                    if (line.Contains("NUMBERTYPE"))
                    {
                        string type = line.Substring(line.IndexOf(" ")).Trim();
                        if (type == "2_BYTE_INTEGER")
                            return ETOPODBType.Int16;
                        if (type == "4_BYTE_FLOAT")
                            return ETOPODBType.Float;
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
            SQLiteConnection.CreateFile(FileName);
            SQLiteConnection con = new SQLiteConnection("Data Source = " + FileName + "; Version = 3;");

            createTable(0, 1350, con, callback);
            createTable(1350, 1350, con, callback);
            createTable(2700, 1350, con, callback);
            createTable(4050, 1350, con, callback);
            createTable(5400, 1350, con, callback);
            createTable(6750, 1350, con, callback);
            createTable(8100, 1350, con, callback);
            createTable(9450, 1350, con, callback);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Не ликвидировать объекты несколько раз")]
        private void createTable(int startIndex, int length, SQLiteConnection connection, Action<string> callback = null)
        {
            //СОЗДАНИЕ БД
            string table_name = "tb" + startIndex;
            connection.Open();

            string create = @"CREATE TABLE " + table_name + @"
                (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,";

            for (int j = startIndex; j < startIndex + length; j++)
                create += "col" + j + " DOUBLE NOT NULL,";
            create = create.TrimEnd(new char[] { ',' });
            create += ");";

            SQLiteCommand commCreate = new SQLiteCommand(create, connection);
            commCreate.ExecuteNonQuery();
            connection.Close();

            //ЭКСПОРТ ДАННЫХ
            connection.Open();
            SQLiteTransaction trans = connection.BeginTransaction();
            double tNumber = startIndex / length + 1d;
            for (int i = 0; i < this.Rows; i++)
            {
                SQLiteCommand cm = connection.CreateCommand();

                string command = @"INSERT INTO '" + table_name + @"'";
                string cols = "(";
                string data = "(";
                for (int j = startIndex; j < startIndex + length; j++)
                {
                    cols += "'col" + j + "',";
                    data += this[i, j] + ",";
                }
                cols = cols.TrimEnd(new char[] { ',' });
                data = data.TrimEnd(new char[] { ',' });
                command += cols + ") VALUES " + data + ");";

                cm.CommandText = command;
                cm.ExecuteNonQuery();
                if (i % 200 == 0 && callback != null)
                    callback.Invoke("Создание базы данный SQLITE: таблица " + tNumber.ToString("0") + " из 8, завершено " + ((i / (double)this.Rows) * 100d).ToString("0.0"));
            }
            trans.Commit();
            connection.Close();
        }
    }
}
