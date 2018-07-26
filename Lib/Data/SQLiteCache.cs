using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using GMap.NET;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Tracking;
using System.Threading;

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
        private struct RowGeocoder
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

        SQLiteConnection cache_connection;
        string directory;
        string cache_file;
        string cache_connectionString;
        string geocoder_table = "tb_geocoder";
        string maplayer_positions_table = "tb_maplayer_positions";
        string maplayer_data_table = "tb_maplayer_data";

        /// <summary>
        /// округление в таблице геокодера
        /// </summary>
        int decimal_digits = 4;

        /// <summary>
        /// максимальный масштаб карты
        /// </summary>
        private int max_zoom;

        /// <summary>
        /// открывает базу данных в указанной папке
        /// </summary>
        /// <param name="dbDirectory">папка с базой данных кэша</param>
        /// <param name="max_zoom">максимальный масштаб карты</param>
        public SQLiteCache(string dbDirectory, int max_zoom)
        {
            directory = dbDirectory;
            this.max_zoom = max_zoom;

            cache_file = dbDirectory + "\\cache.sqlite3";
            cache_connectionString = "Data Source = " + cache_file;

            if (!File.Exists(cache_file))
                CreateDB(cache_file);

            cache_connection = new SQLiteConnection();
            cache_connection.ConnectionString = cache_connectionString;
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

            //таблица кэша геокодера
            SQLiteCommand commCreate = new SQLiteCommand(
                @"CREATE TABLE " + geocoder_table + @"
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

            //таблица расположения векторных объектов
            commCreate = new SQLiteCommand(
               @"CREATE TABLE " + maplayer_positions_table + @"
                (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                object_id INTEGER NOT NULL,
                center_lat double NOT NULL,
                center_lon double NOT NULL,
                lat_min double NOT NULL,
                lat_max double NOT NULL,
                lon_min double NOT NULL,
                lon_max double NOT NULL,
                ins_date DATE NOT NULL,
                perimeter double NOT NULL,
                layer_provider TEXT NOT NULL
                );",
               con);
            commCreate.ExecuteNonQuery();

            //таблица данных векторных объектов
            commCreate = new SQLiteCommand(
               @"CREATE TABLE " + maplayer_data_table + @"
                (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
                object_id INTEGER NOT NULL,
                layer_provider TEXT NOT NULL,
                geometry TEXT NOT NULL,
                ins_date DATE NOT NULL,
                name TEXT NOT NULL,
                description TEXT,
                link TEXT
                );",
               con);
            commCreate.ExecuteNonQuery();

            SQLiteCommand commAddFirst = new SQLiteCommand(
                @"INSERT INTO '" + geocoder_table + @"' 
                ('latitude','longitude','altitude','address','tzid','tzname','tzoffset') 
                VALUES (55.755351,37.855511, 200, 'Россия, Реутов, Войтовича, 3', 'Moscow', 'GMT+3', '3');",
                con);
            int g = commAddFirst.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// выполнение запроса без результата
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private int ExecuteQuery(string query)
        {
            lock (this.cache_connection)
            {
                cache_connection.Open();
                SQLiteCommand com = cache_connection.CreateCommand();
                com.CommandText = query;
                int i = com.ExecuteNonQuery();
                cache_connection.Close();
                return i;
            }
        }



        #region Geocoder

        /// <summary>
        /// добавление новой строки в базу 
        /// </summary>
        /// <param name="lat">широта</param>
        /// <param name="lon">долгота</param>
        /// <param name="alt">высота</param>
        /// <param name="addr">адрес</param>
        /// <param name="tzone">временнАя зона</param>
        private void AddGeocoder(double lat, double lon, double alt, string addr, TimeZoneInfo tzone)
        {
            ExecuteQuery(CreateCommandGeocoder(lat, lon, alt, addr, tzone));
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
        private string CreateCommandGeocoder(double lat, double lon, double alt, string addr, TimeZoneInfo tzone)
        {
            string com = "";
            //if (double.IsNaN(lat) && double.IsNaN(lon))
            //    com = string.Format(@"INSERT OR REPLACE INTO '" + table_name + @"'('latitude','longitude','altitude','address') VALUES ({0},{1},{2},{3});",
            //               Math.Round(lat, decimal_digits).ToString().Replace(',', '.'),
            //               Math.Round(lon, decimal_digits).ToString().Replace(',', '.'),
            //               double.IsNaN(alt) ? "NULL" : alt.ToString().Replace(',', '.'),
            //               string.IsNullOrEmpty(addr) ? "NULL" : "'" + addr + "'");
            //else
            com = string.Format(@"INSERT OR REPLACE INTO '" + geocoder_table + @"'('latitude','longitude','altitude','address','tzid','tzname','tzoffset') VALUES ({0},{1},{2},{3},{4},{5},{6});",
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
        /// выполнение запроса с результатом
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        private List<RowGeocoder> ExecuteReaderGeocoder(string com)
        {
            lock (this.cache_connection)
            {
                cache_connection.Open();
                SQLiteCommand cmd = cache_connection.CreateCommand();
                cmd.CommandText = com;
                SQLiteDataReader dr = cmd.ExecuteReader();

                List<RowGeocoder> res = new List<RowGeocoder>();

                while (dr.Read())
                {
                    res.Add(new RowGeocoder()
                    {
                        id = Convert.ToInt32(dr["id"]),
                        lat = Convert.ToDouble(dr["latitude"]),
                        lon = Convert.ToDouble(dr["longitude"]),
                        alt = dr["altitude"] is DBNull ? double.NaN : Convert.ToDouble(dr["altitude"]),
                        adr = dr["address"] is DBNull ? null : Convert.ToString(dr["address"]),
                        TZid = dr["tzid"] is DBNull ? null : Convert.ToString(dr["tzid"]),
                        TZname = dr["tzname"] is DBNull ? null : Convert.ToString(dr["tzname"]),
                        TZoffset = dr["tzoffset"] is DBNull ? double.NaN : Convert.ToDouble(dr["tzoffset"]),
                    });
                }

                cache_connection.Close();
                return res;
            }
        }

        /// <summary>
        /// удаление записей, у которых адрес и высота NULL
        /// </summary>
        private void remNullsGeocoder()
        {
            string com = "DELETE FROM '" + geocoder_table + "' WHERE address IS NULL AND altitude IS NULL";
            int i = ExecuteQuery(com);
            string sel = "SELECT * FROM " + geocoder_table;
            List<RowGeocoder> all = ExecuteReaderGeocoder(sel);
        }

        /// <summary>
        /// добавление записи координат и высоты в кэш 
        /// </summary>
        /// <param name="Coordinate"></param>
        /// <param name="Altitude"></param>
        internal void Put(Coordinate Coordinate, double Altitude)
        {
            this.AddGeocoder(
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
            this.AddGeocoder(
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
            lock (this.cache_connection)
            {
                this.cache_connection.Open();
                SQLiteTransaction trans = this.cache_connection.BeginTransaction();
                double all = track.Count;
                for (int i = 0; i < track.Count; i++)
                {
                    SQLiteCommand cm = cache_connection.CreateCommand();

                    string command = CreateCommandGeocoder(
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
                this.cache_connection.Close();
            }
        }

        /// <summary>
        /// записать значения временной зоны в кэш
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="tzi"></param>
        internal void Put(Coordinate coordinates, TimeZoneInfo tzi)
        {
            this.AddGeocoder(
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
            string seln = "UPDATE '" + geocoder_table + "' SET address = NULL";
            int i = ExecuteQuery(seln);
            remNullsGeocoder();
        }

        /// <summary>
        /// удаление высот точек
        /// </summary>
        public void ClearAltitudes()
        {
            string seln = "UPDATE '" + geocoder_table + "' SET altitude = NULL";
            int i = ExecuteQuery(seln);
            remNullsGeocoder();
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


        #endregion

        //#region VectorMapLayer

        ///// <summary>
        ///// создаёт новую команду для добавления объекта
        ///// </summary>
        ///// <param name="obj">объект для добавления в кэш</param>
        ///// <param name="zoom">масштаб (в соответствующую таблицу будет записан объект)</param>
        ///// <returns></returns>
        //private string CreateCommandMaplayer(VectorMapLayerObject obj, int zoom)
        //{
        //    string res = string.Format(@"INSERT OR REPLACE INTO '" + maplayer_positions_table + "_z" + zoom + @"'('object_id','geometry','name','description','center_lat','center_lon','ins_date','link','perimeter') VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8});",
        //        obj.ID,
        //        obj.GeometryString,
        //        obj.Name,
        //        string.IsNullOrEmpty(obj.Description) ? "NULL" : "'" + obj.Description + "'",
        //        obj.GeometryCenter.Latitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        obj.GeometryCenter.Longitude.TotalDegrees.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        "NULL", //TODO: сделать сохранение времени добавления объекта
        //        string.IsNullOrEmpty(obj.Link) ? "NULL" : "'" + obj.Link + "'",
        //        obj.Perimeter.ToString().Replace(Vars.DecimalSeparator, '.')
        //        );
        //    return res;
        //}

        ///// <summary>
        ///// добавляет объект векторного слоя карты в кэш
        ///// </summary>
        ///// <param name="obj">векторный объект слоя</param>
        //public void AddMapLayerObject(VectorMapLayerObject obj, int zoom)
        //{
        //    ExecuteQuery(CreateCommandMaplayer(obj, zoom));
        //}

        ///// <summary>
        ///// добавляет коллекцию объектов в кэш
        ///// </summary>
        ///// <param name="objects">коллекция объектов для добавления</param>
        //public void AddMapLayerObjects(IEnumerable<VectorMapLayerObject> objects, int zoom)
        //{
        //    //TODO: сделать добавление объектов одной транзакцией
        //    foreach (VectorMapLayerObject obj in objects)
        //        AddMapLayerObject(obj, zoom);
        //}

        ///// <summary>
        ///// выполнение запроса с результатом
        ///// </summary>
        ///// <param name="com"></param>
        ///// <returns></returns>
        //private List<RowMaplayer> ExecuteReaderMaplayer(string com)
        //{
        //    lock (this.cache_connection)
        //    {
        //        cache_connection.Open();
        //        SQLiteCommand cmd = cache_connection.CreateCommand();
        //        cmd.CommandText = com;
        //        SQLiteDataReader dr = cmd.ExecuteReader();

        //        List<RowMaplayer> res = new List<RowMaplayer>();

        //        while (dr.Read())
        //        {
        //            res.Add(new RowMaplayer()
        //            {
        //                id = Convert.ToInt32(dr["id"]),
        //                object_id = Convert.ToInt32(dr["object_id"]),
        //                center_lat = Convert.ToDouble(dr["center_lat"]),
        //                center_lon = Convert.ToDouble(dr["center_lon"]),
        //                perimeter = Convert.ToDouble(dr["perimeter"]),
        //                description = dr["description"] is DBNull ? null : Convert.ToString(dr["description"]),
        //                link = dr["link"] is DBNull ? null : Convert.ToString(dr["link"]),
        //                geometry = dr["geometry"].ToString(),
        //                name = dr["name"].ToString(),
        //                ins_date = DateTime.Now //TODO: сделать запись времени добавления объекта
        //            });
        //        }

        //        cache_connection.Close();
        //        return res;
        //    }
        //}

        //#endregion


        #endregion

        #region  реализации интерфейсов TrackConverter

        //#region IVectorMapLayerProvider

        ///// <summary>
        ///// Получить список объектов в заданной области карты и с периметром не менее perimeter
        ///// </summary>
        ///// <param name="area">область карты</param>
        ///// <param name="perimeter">минимальный периметр объекта в метрах</param>
        ///// <returns></returns>
        //public List<VectorMapLayerObject> GetVectorMapLayerObjects(RectLatLng area, double perimeter)
        //{
        //    //координаты углов области
        //    double lon_max = Math.Max(area.LocationTopLeft.Lng, area.LocationRightBottom.Lng);
        //    double lat_max = Math.Max(area.LocationRightBottom.Lat, area.LocationTopLeft.Lat);
        //    double lon_min = Math.Min(area.LocationRightBottom.Lng, area.LocationTopLeft.Lng);
        //    double lat_min = Math.Min(area.LocationTopLeft.Lat, area.LocationRightBottom.Lat);

        //    string command = string.Format("SELECT * FROM " + maplayer_positions_table + @" WHERE 
        //        center_lat < {0} AND 
        //        center_lat > {1} AND 
        //        center_lon < {2} AND 
        //        center_lon > {3} AND 
        //        perimeter > {4}",
        //        lat_max.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        lat_min.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        lon_max.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        lon_min.ToString().Replace(Vars.DecimalSeparator, '.'),
        //        perimeter.ToString().Replace(Vars.DecimalSeparator, '.')
        //        );

        //    List<RowMaplayer> rows = ExecuteReaderMaplayer(command);

        //    if (rows == null || rows.Count == 0)
        //        return null;

        //    List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();
        //    foreach (RowMaplayer row in rows)
        //    {
        //        res.Add(new VectorMapLayerObject(row.geometry)
        //        {
        //            Description = row.description,
        //            ID = row.object_id,
        //            Link = row.link,
        //            Name = row.name
        //        });
        //    }
        //    return res;
        //}

        //#endregion

        #region IGeoCoderProvider

        /// <summary>
        /// пытается получить адрес из кэша. при неудаче результат null
        /// </summary>
        /// <param name="coordinate">координаты точки</param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            //            string all = "SELECT * FROM '" + table_name + @"'  ";
            //  List<Row> ar = ExecuteReader(all);

            string sel = string.Format(@"SELECT * FROM '" + geocoder_table + @"' WHERE latitude = '{0}' AND longitude = '{1}' AND address IS NOT NULL",
                Math.Round(coordinate.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
                Math.Round(coordinate.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
                );
            List<RowGeocoder> dr = ExecuteReaderGeocoder(sel);
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
            string sel = string.Format(@"SELECT * FROM '" + geocoder_table + @"' WHERE address = '{0}'", address);
            List<RowGeocoder> dr = ExecuteReaderGeocoder(sel);
            if (dr.Count == 0)
                return Coordinate.Empty;
            return new Coordinate(dr[0].lat, dr[0].lon);
        }

        /// <summary>
        /// получить веремнную зону из кэша
        /// </summary>
        /// <param name="coordinates">координаты</param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinates)
        {
            string sel = string.Format(@"SELECT * FROM '" + geocoder_table + @"' WHERE latitude = {0} AND longitude = {1} AND tzid IS NOT NULL AND tzname IS NOT NULL AND tzoffset IS NOT NULL",
           Math.Round(coordinates.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
           Math.Round(coordinates.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
           );
            List<RowGeocoder> dr = ExecuteReaderGeocoder(sel);
            if (dr.Count == 0)
                return null;
            if (Vars.Options.DataSources.UseSystemTimeZones)
                return TimeZoneInfo.FindSystemTimeZoneById(dr[0].TZid);
            else
                return TimeZoneInfo.CreateCustomTimeZone(dr[0].TZid, TimeSpan.FromHours(dr[0].TZoffset), dr[0].TZname, dr[0].TZname);

        }

        #endregion

        #region IGeoInfoProvider

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
        /// пытается получить высоту из кэша. При неудаче результат double.NaN
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public double GetElevation(Coordinate coordinate)
        {
            //string sel = "SELECT * FROM " + table_name;
            string sel = string.Format(@"SELECT * FROM " + geocoder_table + @" WHERE latitude = '{0}' AND longitude = '{1}' AND altitude IS NOT NULL",
            Math.Round(coordinate.Latitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.'),
            Math.Round(coordinate.Longitude.TotalDegrees, decimal_digits).ToString().Replace(',', '.')
            );
            List<RowGeocoder> dr = ExecuteReaderGeocoder(sel);
            if (dr.Count == 0)
                return double.NaN;
            return dr[0].alt;
        }

        #endregion

        #endregion

        #region реализации интерфейсов .NET

        /// <summary>
        /// закрывает подключение и освобождает ресурсы
        /// </summary>
        public void Dispose()
        {
            if (cache_connection != null)
            {
                cache_connection.Close();
                cache_connection.Dispose();
                GC.SuppressFinalize(this);
            }


        }

        #endregion

    }
}