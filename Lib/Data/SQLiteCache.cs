using System;
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
            public string TZname;
            public double TZoffset;
            public string TZid;
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

            //проверка версии БД
            geocoder_connection.Open();
            SQLiteCommand cm = new SQLiteCommand("SELECT * FROM " + table_name + " WHERE latitude != NULL LIMIT 1", geocoder_connection);
            SQLiteDataReader dr = cm.ExecuteReader();
            int version = 0;
            if (dr.FieldCount == 5)
                version = 1;
            else
            if (dr.FieldCount == 8)
                version = 2;
            geocoder_connection.Close();

            switch (version)
            {
                case 1:
                    geocoder_connection.Open();
                    SQLiteCommand cmac = new SQLiteCommand("ALTER TABLE " + table_name + " ADD COLUMN tzid TEXT", geocoder_connection);
                    cmac.ExecuteNonQuery();
                    cmac.CommandText = "ALTER TABLE " + table_name + " ADD COLUMN tzname TEXT";
                    cmac.ExecuteNonQuery();
                    cmac.CommandText = "ALTER TABLE " + table_name + " ADD COLUMN tzoffset DOUBLE";
                    cmac.ExecuteNonQuery();
                    geocoder_connection.Close();
                    break;
                case 2:
                    break;
                default: throw new SQLiteException("Неизвестная исходная версия БД: " + version);
            }

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
                null,
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
                Address,
                null
                );
        }

        /// <summary>
        /// записать большое количество высот одной транзакцией в кэш
        /// </summary>
        /// <param name="track"></param>
        /// <param name="els"></param>
        /// <param name="callback">действие, выполняемое во время операции</param>
        internal void Put(BaseTrack track, List<double> els, Action<string> callback = null)
        {
            //ЭКСПОРТ ДАННЫХ
            lock (this.geocoder_connection)
            {
                this.geocoder_connection.Open();
                SQLiteTransaction trans = this.geocoder_connection.BeginTransaction();
                double all = track.Count;
                for (int i = 0; i < track.Count; i++)
                {
                    SQLiteCommand cm = geocoder_connection.CreateCommand();

                    string command = CreateCommand(
                    track[i].Coordinates.Latitude.TotalDegrees,
                    track[i].Coordinates.Longitude.TotalDegrees,
                    els[i],
                    null,
                    null);

                    cm.CommandText = command;
                    cm.ExecuteNonQuery();
                    if (i % 200 == 0 && callback != null)
                        callback.Invoke("Запись данных в кэш: завершено " + ((i / all) * 100d).ToString("0.0"));
                }
                trans.Commit();
                this.geocoder_connection.Close();
            }
        }

        /// <summary>
        /// записать значения временной зоны в кэш
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="tzi"></param>
        public void Put(Coordinate coordinates, TimeZoneInfo tzi)
        {
            this.Add(
                coordinates.Latitude.TotalDegrees,
                coordinates.Longitude.TotalDegrees,
                double.NaN,
                null,
                tzi
                );
        }

        /// <summary>
        /// удаление данных геокодера
        /// </summary>
        public void ClearGeocoder()
        {
            string seln = "UPDATE '" + table_name + "' SET address = NULL";
            int i = ExecuteQuery(seln);
            remNulls();
        }

        /// <summary>
        /// удаление высот точек
        /// </summary>
        public void ClearAltitudes()
        {
            string seln = "UPDATE '" + table_name + "' SET altitude = NULL";
            int i = ExecuteQuery(seln);
            remNulls();
        }

        /// <summary>
        /// попробовать загрузить все высоты из кэша, если все высоты есть, то возвращает истину, а в track запишутся высоты
        /// </summary>
        /// <param name="track">маршрут, куда надо загрузить высоты</param>
        /// <returns></returns>
        internal bool TryGetElevations(ref BaseTrack track)
        {
            foreach (TrackPoint point in track)
            {
                double alt = this.GetElevation(point.Coordinates);
                if (double.IsNaN(alt)) //если такой точки в кэше нет, то выход false
                    return false;
                if (double.IsNaN(point.MetrAltitude))
                    point.MetrAltitude = alt; //если точка есть, от береём следующую
            }
            return true;
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
                address text,
                tzid text,
                tzname text,
                tzoffset double
                );",
                con);
            commCreate.ExecuteNonQuery();

            SQLiteCommand commAddFirst = new SQLiteCommand(
                @"INSERT INTO '" + table_name + @"' 
                ('latitude','longitude','altitude','address','tzid','tzname','tzoffset') 
                VALUES (55.755351,37.855511, 200, 'Россия, Реутов, Войтовича, 3', 'Moscow', 'GMT+3', '3');",
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
        private void Add(double lat, double lon, double alt, string addr, TimeZoneInfo tzone)
        {
            ExecuteQuery(CreateCommand(lat, lon, alt, addr, tzone));
        }

        /// <summary>
        /// генерирует команду на вставку в таблицу строки
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <param name="alt">высота</param>
        /// <param name="addr">адрес</param>
        /// <param name="tzone">временная зона</param>
        /// <returns></returns>
        private string CreateCommand(double lat, double lon, double alt, string addr, TimeZoneInfo tzone)
        {
            string com = "";
            //if (double.IsNaN(lat) && double.IsNaN(lon))
            //    com = string.Format(@"INSERT OR REPLACE INTO '" + table_name + @"'('latitude','longitude','altitude','address') VALUES ({0},{1},{2},{3});",
            //               Math.Round(lat, decimal_digits).ToString().Replace(',', '.'),
            //               Math.Round(lon, decimal_digits).ToString().Replace(',', '.'),
            //               double.IsNaN(alt) ? "NULL" : alt.ToString().Replace(',', '.'),
            //               string.IsNullOrEmpty(addr) ? "NULL" : "'" + addr + "'");
            //else
            com = string.Format(@"INSERT OR REPLACE INTO '" + table_name + @"'('latitude','longitude','altitude','address','tzid','tzname','tzoffset') VALUES ({0},{1},{2},{3},{4},{5},{6});",
                       Math.Round(lat, decimal_digits).ToString().Replace(',', '.'),
                       Math.Round(lon, decimal_digits).ToString().Replace(',', '.'),
                       double.IsNaN(alt) ? "NULL" : alt.ToString().Replace(',', '.'),
                       string.IsNullOrEmpty(addr) ? "NULL" : "'" + addr + "'",
                       tzone == null ? "NULL" : "'" + tzone.Id + "'",
                       tzone == null ? "NULL" : "'" + tzone.DisplayName + "'",
                        tzone == null ? "NULL" : tzone.BaseUtcOffset.Hours.ToString()
                       );
            return com;
        }

        /// <summary>
        /// выполнение запроса без результата
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private int ExecuteQuery(string query)
        {
            lock (this.geocoder_connection)
            {
                geocoder_connection.Open();
                SQLiteCommand com = geocoder_connection.CreateCommand();
                com.CommandText = query;
                int i = com.ExecuteNonQuery();
                geocoder_connection.Close();
                return i;
            }
        }

        /// <summary>
        /// выполнение запроса с результатом
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        private List<Row> ExecuteReader(string com)
        {
            lock (this.geocoder_connection)
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
                        adr = dr["address"].GetType() == typeof(DBNull) ? null : Convert.ToString(dr["address"]),
                        TZid = dr["tzid"].GetType() == typeof(DBNull) ? null : Convert.ToString(dr["tzid"]),
                        TZname = dr["tzname"].GetType() == typeof(DBNull) ? null : Convert.ToString(dr["tzname"]),
                        TZoffset = dr["tzoffset"].GetType() == typeof(DBNull) ? double.NaN : Convert.ToDouble(dr["tzoffset"]),
                    });
                }

                geocoder_connection.Close();
                return res;
            }
        }

        /// <summary>
        /// удаление записей, у которых адрес и высота NULL
        /// </summary>
        private void remNulls()
        {
            string com = "DELETE FROM '" + table_name + "' WHERE address IS NULL AND altitude IS NULL";
            int i = ExecuteQuery(com);
            string sel = "SELECT * FROM " + table_name;
            List<Row> all = ExecuteReader(sel);
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

            string sel = string.Format(@"SELECT * FROM '" + table_name + @"' WHERE latitude = '{0}' AND longitude = '{1}' AND address IS NOT NULL",
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
            //string sel = "SELECT * FROM " + table_name;
            string sel = string.Format(@"SELECT * FROM " + table_name + @" WHERE latitude = '{0}' AND longitude = '{1}' AND altitude IS NOT NULL",
            Math.Round(coordinate.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
            Math.Round(coordinate.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
            );
            List<Row> dr = ExecuteReader(sel);
            if (dr.Count == 0)
                return double.NaN;
            return dr[0].alt;
        }

        /// <summary>
        /// получить веремнную зону из кэша
        /// </summary>
        /// <param name="coordinates">координаты</param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinates)
        {
            string sel = string.Format(@"SELECT * FROM '" + table_name + @"' WHERE latitude = {0} AND longitude = {1} AND tzid IS NOT NULL AND tzname IS NOT NULL AND tzoffset IS NOT NULL",
           Math.Round(coordinates.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
           Math.Round(coordinates.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
           );
            List<Row> dr = ExecuteReader(sel);
            if (dr.Count == 0)
                return null;
            if (Vars.Options.DataSources.UseSystemTimeZones)
                return TimeZoneInfo.FindSystemTimeZoneById(dr[0].TZid);
            else
                return TimeZoneInfo.CreateCustomTimeZone(dr[0].TZid, TimeSpan.FromHours(dr[0].TZoffset), dr[0].TZname, dr[0].TZname);

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
                GC.SuppressFinalize(this);
            }


        }


        #endregion

    }
}