using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO
{
    /// <summary>
    /// Работа с базой данных ETOPO в формате SQLite
    /// </summary>
    class SQLiteDatabase : IDatabase
    {
        private string dbFile;
        private int rows;
        private int cols;
        private SQLiteConnection connection;

        /// <summary>
        /// создает новый экземпляр и открывает базу данных
        /// </summary>
        /// <param name="file"></param>
        public SQLiteDatabase(string file)
        {
            this.dbFile = file;
            this.Load(file);
        }

        /// <summary>
        /// открывает базу данных
        /// </summary>
        /// <param name="file">файл с базой данных</param>
        private void Load(string file)
        {
            string connectionString = "Data Source = " + file;
            connection = new SQLiteConnection();
            connection.ConnectionString = connectionString;
            cols = 10800;
            rows = 5400;
        }

        /// <summary>
        /// выполнение запроса с результатом
        /// </summary>
        /// <returns></returns>
        private double GetElev(int i, int j)
        {
            //выбор нужной таблицы
            int ind = -1;
            for (int inn = 7; inn >= 0; inn--)
                if (j >= inn * 1350)
                { ind = inn; break; }

            string table_name = "tb" + ind*1350;
            connection.Open();
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM " + table_name + " WHERE id = " + i.ToString();
            SQLiteDataReader dr = cmd.ExecuteReader();

            double res = double.NaN;
            if (dr.Read())
                res = (double)dr["col"+j];
            connection.Close();
            return res;
        }

        /// <summary>
        /// всегда приводит к возникновению InvalidOperationException
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="callback"></param>
        public void ExportToSQL(string FileName, Action<string> callback = null)
        {
            throw new InvalidOperationException("Попытка преобразовать базу данных SQLite в тот же формат");
        }

        /// <summary>
        /// возвращае высоту по координатам точки
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public double this[Coordinate coordinate]
        {
            get
            {
                Point pt = this.GeoToLocal(coordinate);
                return this[pt.Y, pt.X];
            }
        }

        /// <summary>
        /// преобразование географических координат в локальные. X - столбец, Y - строка
        /// </summary>
        /// <param name="coordinate">географические координаты точки</param>
        /// <returns></returns>
        private Point GeoToLocal(Coordinate coordinate)
        {
            double lat = coordinate.Latitude.TotalDegrees;
            double lon = coordinate.Longitude.TotalDegrees;
            double in1cell = this.CellSize;
            double ic = rows / 2;
            double jc = Columns / 2;

            Point res = new Point();

            res.Y = (int)Math.Round(ic - lat / in1cell, 0);
            res.X = (int)Math.Round(jc + lon / in1cell, 0);

            return res;
        }

        /// <summary>
        /// возвращает высоту по указанным строке и столбцу из таблицы
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public double this[int i, int j]
        {
            get
            {
                return GetElev(i, j);
            }
        }

        /// <summary>
        /// размер ячейки градусов
        /// </summary>
        public double CellSize
        {
            get
            {
                return 180d / (Rows + 0.0d);
            }
        }

        /// <summary>
        /// количество столбцов
        /// </summary>
        public int Columns
        {
            get
            {
                return cols;
            }
        }

        /// <summary>
        /// адрес файла данных
        /// </summary>
        public string DataFile
        {
            get
            {
                return dbFile;
            }
        }

        /// <summary>
        /// папка с файлами базы данных
        /// </summary>
        public string Folder
        {
            get
            {
                return Path.GetDirectoryName(dbFile);
            }
        }

        /// <summary>
        /// адрес заголовочного файла
        /// </summary>
        public string HeaderFile
        {
            get
            {
                return dbFile;
            }
        }

        /// <summary>
        /// количетсво строк
        /// </summary>
        public int Rows
        {
            get
            {
                return rows;
            }
        }

        /// <summary>
        /// тип базы данных
        /// </summary>
        public ETOPODBType Type
        {
            get
            {
                return ETOPODBType.SQLite;
            }
        }
    }
}
