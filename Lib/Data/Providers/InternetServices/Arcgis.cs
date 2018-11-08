using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Связь с сервисом ArcGis.com
    /// </summary>
    class Arcgis : BaseConnection, IGeoсoderProvider
    {
        public Arcgis(string folder, int d=24*7):base(folder,d) { }

        /// <summary>
        /// 
        /// </summary>
        public override TimeSpan MinQueryInterval { get { return TimeSpan.FromMilliseconds(100); } }
        public override int MaxAttempts { get { return 5; } }

        public string GetAddress(Coordinate coordinate)
        {
            //https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?outSR=4326&returnIntersection=false&location=37.715334892272956%2C55.759359885308086&f=json
            string url = "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer/reverseGeocode?outSR=4326&returnIntersection=false&location={0}%2C{1}&f=json";
            url = string.Format(url, coordinate.Longitude.ToString().Replace(Vars.DecimalSeparator,'.'), coordinate.Latitude.ToString().Replace(Vars.DecimalSeparator,'.'));
            JToken ans = SendJsonGetRequest(url);

            JToken err = ans["error"];
            if (err != null)
                throw new Exception(err["message"].ToString());

            string adr = ans["address"]["LongLabel"].ToString();
            return adr;

        }

        /// <summary>
        /// координаты адреса
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            throw new NotImplementedException();
        }

        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            throw new NotImplementedException();
        }
    }
}
