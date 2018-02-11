using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// работа с геокодером OSM
    /// http://wiki.openstreetmap.org/wiki/Nominatim
    /// </summary>
    class Nominatim : BaseConnection, IGeoсoderProvider
    {
        /// <summary>
        /// минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }

        /// <summary>
        /// максимальное число попыток подключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// получить адрес по координатам
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public string GetAddress(Coordinate coordinate)
        {
            // http://nominatim.openstreetmap.org/reverse?format=xml&lat=52.5487429714954&lon=-1.81602098644987

            //string g = base.SendStringRequest("http://nominatim.openstreetmap.org/reverse?lat=56.1269950639321&lon=35.7268531545079&format=xml");


            string url = string.Format("http://nominatim.openstreetmap.org/reverse?format=xml&lat={0}&lon={1}&accept-language={2}",
                coordinate.Latitude.TotalDegrees.ToString().Replace(',', '.'),
                coordinate.Longitude.TotalDegrees.ToString().Replace(',', '.'),
                "ru-Ru");
            XmlDocument xml = SendXmlGetRequest(url);

            //если ошибка
            XmlNodeList ers = xml.GetElementsByTagName("error");
            if (ers.Count == 1)
                throw new ApplicationException("Nominatim error: "+ers[0].InnerText);

            //приведение адреса к стандартному виду
            XmlNode parts = xml.GetElementsByTagName("addressparts")[0];
            string res = "";
            for (int i = parts.ChildNodes.Count - 1; i >= 0; i--)
                if (parts.ChildNodes[i].LocalName != "country_code" &&
                    parts.ChildNodes[i].LocalName != "postcode" &&
                    parts.ChildNodes[i].LocalName != "suburb" &&
                    parts.ChildNodes[i].LocalName != "county" &&
                    parts.ChildNodes[i].LocalName != "building" &&
                    parts.ChildNodes[i].LocalName != "residential")
                    res += parts.ChildNodes[i].InnerText + ", ";
            res = res.TrimEnd(new char[] { ' ', ',' });
            res = res.Replace("РФ", "Россия");
            return res;
        }

        /// <summary>
        /// получить координаты по адресу
        /// </summary>
        /// <param name="address">адрес</param>
        /// <returns></returns>
        public Coordinate GetCoordinate(string address)
        {
            // http://nominatim.openstreetmap.org/search?q=135+pilkington+avenue,+birmingham&format=xml

            string url = string.Format("http://nominatim.openstreetmap.org/search?q={0}&format=xml", address);
            XmlDocument xml = SendXmlGetRequest(url);

            XmlNodeList places = xml.GetElementsByTagName("place");
            if (places.Count == 0)
                throw new ApplicationException("Геокодер Nominatim не может найти адрес " + address + ".\r\nПопробуйте использовать другой геокодер");
            XmlNode place = places[0];

            string lat = place.Attributes["lat"].Value;
            string lon = place.Attributes["lon"].Value;

            Coordinate res = new Coordinate(lat, lon);
            return res;
        }

        /// <summary>
        /// получить информацию о временной зоне
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public TimeZoneInfo GetTimeZone(Coordinate coordinate)
        {
            throw new NotImplementedException();
        }
    }
}
