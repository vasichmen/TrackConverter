﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// кэш данных геокодера, высот итд  в файловой системе. 
    /// </summary>
    public class SQLiteCache : IGeoInfoProvider, IGeoсoderProvider, IDisposable
    {
        /// <summary>
        /// строка в таблице кэша геокодера
        /// </summary>
        private struct Row
        {
            public int id;
            public double lat;
            public double lon;
            public double alt;
            public string adr;
        }

        SQLiteConnection geocoder_connection;
        string directory;
        string geocoder_file;
        string geocoder_connectionString;
        string table_name = "tb_cache";
        int decimal_digits = 4;


        /// <summary>
        /// открывает базу данных в указанной папке
        /// </summary>
        /// <param name="dbDirectory">папка с базой данных кэша</param>
        public SQLiteCache(string dbDirectory)
        {
            directory = dbDirectory;

            geocoder_file = dbDirectory + "\\geocoder.gcdb";
            geocoder_connectionString = "Data Source = " + geocoder_file;

            if (!File.Exists(geocoder_file))
                CreateDB(geocoder_file);

            geocoder_connection = new SQLiteConnection();
            geocoder_connection.ConnectionString = geocoder_connectionString;
        }

        /// <summary>
        /// добавление записи координат и высоты в кэш 
        /// </summary>
        /// <param name="Coordinate"></param>
        /// <param name="Altitude"></param>
        internal void Put(Coordinate Coordinate, double Altitude)
        {
            this.Add(
                Coordinate.Latitude.TotalDegrees,
                Coordinate.Longitude.TotalDegrees,
                Altitude,
                null
                );
        }

        /// <summary>
        /// добавление записи координат и адреса в кэш
        /// </summary>
        /// <param name="Coordinate"></param>
        /// <param name="Address"></param>
        internal void Put(Coordinate Coordinate, string Address)
        {
            this.Add(
                Coordinate.Latitude.TotalDegrees,
                Coordinate.Longitude.TotalDegrees,
                double.NaN,
                Address
                );
        }

        #region работа с базой данных

        /// <summary>
        /// создает новую базу данных
        /// </summary>
        /// <param name="baseName"></param>
        private void CreateDB(string baseName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(baseName));
            SQLiteConnection.CreateFile(baseName);
            SQLiteConnection con = new SQLiteConnection("Data Source = " + baseName);
            con.Open();
            SQLiteCommand commCreate = new SQLiteCommand(
                @"CREATE TABLE " + table_name + @"
                (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                latitude double NOT NULL,
                longitude double NOT NULL,
                altitude double,
                address text
                );",
                con);
            commCreate.ExecuteNonQuery();

            SQLiteCommand commAddFirst = new SQLiteCommand(
                @"INSERT INTO '" + table_name + @"' 
                ('latitude','longitude','altitude','address') 
                VALUES (55.755351,37.855511, NULL, 'Россия, Реутов, Войтовича, 3');",
                con);
            int g = commAddFirst.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// добавление новой строки в базу 
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <param name="alt">высота</param>
        /// <param name="addr">адрес</param>
        private void Add(double lat, double lon, double alt, string addr)
        {
            string com = string.Format(@"INSERT INTO '" + table_name + @"'('latitude','longitude','altitude','address') VALUES ({0},{1},{2},{3});",
                    Math.Round(lat, decimal_digits).ToString().Replace(',', '.'),
                    Math.Round(lon, decimal_digits).ToString().Replace(',', '.'),
                    double.IsNaN(alt) ? "NULL" : alt.ToString().Replace(',', '.'),
                    string.IsNullOrEmpty(addr) ? "NULL" : "'" + addr + "'");
            ExecuteQuery(com);
        }

        /// <summary>
        /// выполнение запроса без результата
        /// </summary>
        /// <param name="query"></param>
        private void ExecuteQuery(string query)
        {
            geocoder_connection.Open();
            SQLiteCommand com = geocoder_connection.CreateCommand();
            com.CommandText = query;
            com.ExecuteNonQuery();
            geocoder_connection.Close();
        }

        /// <summary>
        /// выполнение запроса с результатом
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        private List<Row> ExecuteReader(string com)
        {
            geocoder_connection.Open();
            SQLiteCommand cmd = geocoder_connection.CreateCommand();
            cmd.CommandText = com;
            SQLiteDataReader dr = cmd.ExecuteReader();

            List<Row> res = new List<Row>();

            while (dr.Read())
            {
                res.Add(new Row()
                {
                    id = Convert.ToInt32(dr["id"]),
                    lat = Convert.ToDouble(dr["latitude"]),
                    lon = Convert.ToDouble(dr["longitude"]),
                    alt = dr["altitude"].GetType() == typeof(DBNull) ? double.NaN : Convert.ToDouble(dr["altitude"]),
                    adr = dr["address"].GetType() == typeof(DBNull) ? null : Convert.ToString(dr["address"])
                });
            }

            geocoder_connection.Close();
            return res;
        }

        #endregion

        #region  реализации интерфейсов TrackConverter

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
        /// пытается получить адрес из кэша. при неудаче результат null
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            //            string all = "SELECT * FROM '" + table_name + @"'  ";
            //  List<Row> ar = ExecuteReader(all);

            string sel = string.Format(@"SELECT * FROM '" + table_name + @"' WHERE latitude = {0} AND longitude = {1}",
                Math.Round(coordinate.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
                Math.Round(coordinate.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
                );
            List<Row> dr = ExecuteReader(sel);
            if (dr.Count == 0)
                return null;
            return dr[0].adr;
        }

        /// <summary>
        /// пытается получить кординаты адреса из кэша. При неудаче результат Coordinate.Empty
        /// </summary>
        /// <param name="address">адрес </param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            string sel = string.Format(@"SELECT * FROM '" + table_name + @"' WHERE address = '{0}'", address);
            List<Row> dr = ExecuteReader(sel);
            if (dr.Count == 0)
                return Coordinate.Empty;
            return new Coordinate(dr[0].lat, dr[0].lon);
        }

        /// <summary>
        /// пытается получить высоту из кэша. При неудаче результат double.NaN
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            string sel = string.Format(@"SELECT * FROM '" + table_name + @"' WHERE latitude = {0} AND longitude = {1}",
            Math.Round(coordinate.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
            Math.Round(coordinate.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
            );
            List<Row> dr = ExecuteReader(sel);
            if (dr.Count == 0)
                return double.NaN;
            return dr[0].alt;
        }

        #endregion

        #region реализации интерфейсов .NET

        /// <summary>
        /// закрывает подключение и освобождает ресурсы
        /// </summary>
        public void Dispose()
        {
            if (geocoder_connection != null)
            {
                geocoder_connection.Close();
                geocoder_connection.Dispose();
            }
        }


        #endregion

    }
}