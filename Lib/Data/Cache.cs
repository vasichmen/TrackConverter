using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.Local.OS;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data
{
    //TODO: описания методов
    public class Cache : IGeoсoderProvider, IGeoInfoProvider, IDisposable, IGeocoderCache, IGeoInfoCache, IImagesCache
    {
        string sqliteDir;
        string imagesDir;

        SQLiteCache sqlite;
        FileSystemCache fs;

        public Cache(string baseDir)
        {
            sqliteDir = baseDir + "\\geocoder";
            imagesDir = baseDir + "\\images";

            sqlite = new SQLiteCache(sqliteDir);
            fs = new FileSystemCache(imagesDir);
        }


        #region реализации интерфейсов TrackConverter

        public bool isLocal
        {
            get
            {
                return ((IGeoInfoProvider)sqlite).isLocal;
            }
        }


        public string GetAddress(Coordinate coordinate)
        {
            return ((IGeoсoderProvider)sqlite).GetAddress(coordinate);
        }

        public Coordinate GetCoordinate(string address)
        {
            return ((IGeoсoderProvider)sqlite).GetCoordinate(address);
        }

        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            return ((IGeoсoderProvider)sqlite).GetTimeZone(coordinate);
        }

        public double GetElevation(Coordinate coordinate)
        {
            return ((IGeoInfoProvider)sqlite).GetElevation(coordinate);
        }

        public void ClearGeocoder()
        {
            ((IGeocoderCache)sqlite).ClearGeocoder();
        }

        public void PutGeocoder(Coordinate Coordinate, string Address)
        {
            ((IGeocoderCache)sqlite).PutGeocoder(Coordinate, Address);
        }

        public void ClearAltitudes()
        {
            ((IGeoInfoCache)sqlite).ClearAltitudes();
        }

        public void PutGeoInfo(Coordinate coordinates, TimeZoneInfo tzi)
        {
            ((IGeoInfoCache)sqlite).PutGeoInfo(coordinates, tzi);
        }

        public void PutGeoInfo(Coordinate Coordinate, double Altitude)
        {
            ((IGeoInfoCache)sqlite).PutGeoInfo(Coordinate, Altitude);
        }

        public void PutGeoInfo(BaseTrack track, List<double> els, Action<string> callback = null)
        {
            ((IGeoInfoCache)sqlite).PutGeoInfo(track, els, callback);
        }

        public bool TryGetElevations(ref BaseTrack track)
        {
            return ((IGeoInfoCache)sqlite).TryGetElevations(ref track);
        }

        public bool CheckImage(string url)
        {
            return ((IImagesCache)fs).CheckImage(url);
        }

        public Image GetImage(string url)
        {
            return ((IImagesCache)fs).GetImage(url);
        }

        public bool PutImage(string url, Image data)
        {
            return ((IImagesCache)fs).PutImage(url, data);
        }


        #endregion

        #region реализации интерфейсов .NET

        public void Dispose()
        {
            sqlite.Dispose();
            fs.Close();
        }

        #endregion


    }
}
