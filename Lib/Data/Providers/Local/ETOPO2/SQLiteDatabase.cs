using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO2
{
    class SQLiteDatabase : IDatabase
    {
        private string dbFile;
        private int rows;
        private int cols;
        private SQLiteConnection connection;


        public SQLiteDatabase(string file)
        {
            this.dbFile = file;
            this.Load(file);
        }

        private void Load(string file)
        {
            string connectionString = "Data Source = " + file;
            connection = new SQLiteConnection();
            connection.ConnectionString = connectionString;
        }

        /// <summary>
        /// выполнение запроса с результатом
        /// </summary>
        /// <returns></returns>
        private double GetElev(int i, int j)
        {
            connection.Open();
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT data FROM tbData WHERE id=" + i.ToString();
            SQLiteDataReader dr = cmd.ExecuteReader();

            double[] data;
            double res=double.NaN;
            while (dr.Read())
            {

            }
            connection.Close();
            return res;
        }

        public double this[Coordinate coordinate]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double this[int i, int j]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double CellSize
        {
            get
            {
                return -1;
            }
        }

        public int Columns
        {
            get
            {
                return cols;
            }
        }

        public string DataFile
        {
            get
            {
                return dbFile;
            }
        }

        public string Folder
        {
            get
            {
                return Path.GetDirectoryName(dbFile);
            }
        }

        public string HeaderFile
        {
            get
            {
                return dbFile;
            }
        }

        public int Rows
        {
            get
            {
                return rows;
            }
        }

        public ETOPO2DBType Type
        {
            get
            {
                return ETOPO2DBType.SQLite;
            }
        }
    }
}
