using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// методы для работы с форматом Trip Route
    /// </summary>
    static class TrrHelper
    {
        /// <summary>
        /// разделитель разделов информации, точек, маршртов
        /// </summary>
        public static readonly string separatorMain = "<section>";

        /// <summary>
        /// разделитель массива маршрутов и массива информации о маршрутах
        /// </summary>
        public static readonly string separatorDays = "<day_infos>";

        /// <summary>
        /// заголовок с версией для экспорта
        /// </summary>
        public static readonly string header = "Track Converter Trip File 1.0";

        /// <summary>
        /// структура для сериализации информации
        /// </summary>
        class info
        {
            /// <summary>
            /// для Json
            /// </summary>
            public info()
            { }

            /// <summary>
            /// создает структуру из объекта  TrackFile
            /// </summary>
            /// <param name="trip"></param>
            public info(TripRouteFile trip)
            {
                this.Name = trip.Name;
                this.Description = trip.Description;
                this.Distance = trip.Distance;
                this.KmphSpeed = trip.KmphSpeed;
                this.Color = trip.Color;
                this.Time = trip.Time;
            }

            /// <summary>
            /// преобразует структуру в TripRouteFile
            /// </summary>
            /// <returns></returns>
            public TripRouteFile ToTripRoute()
            {
                TripRouteFile res = new TripRouteFile();
                res.Name = this.Name;
                res.Description = this.Description;
                res.Color = this.Color;
                return res;
            }

            public Color Color { get; set; }
            public string Description { get; set; }
            public double Distance { get; set; }
            public double KmphSpeed { get; set; }
            public string Name { get; set; }
            public TimeSpan Time { get; set; }
        }




        #region экспорт в строки

        public static string GetInformation(TripRouteFile trip)
        {
            info info = new info(trip);
            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            return json;
        }

        public static string GetRoutes(TripRouteFile trip)
        {
            string array = JsonConvert.SerializeObject(trip.DaysRoutes, Formatting.Indented);
            List<info> infs = new List<info>();
            foreach (TrackFile tf in trip.DaysRoutes)
                infs.Add(new info()
                {
                    Color = tf.Color,
                    Time = tf.Time,
                    Description = tf.Description,
                    KmphSpeed = tf.KmphSpeed,
                    Distance = tf.Distance,
                    Name = tf.Name
                });
            string array_info = JsonConvert.SerializeObject(infs, Formatting.Indented);

            return array + "\r\n" + separatorDays + "\r\n" + array_info;
        }

        public static string GetWaypoints(TripRouteFile trip)
        {
            string json = JsonConvert.SerializeObject(trip.Waypoints, Formatting.Indented);
            return json;
        }

        #endregion

        #region десериализация из строк

        public static TripRouteFile GetInformation(string jsonInformation)
        {
            info info = JsonConvert.DeserializeObject<info>(jsonInformation.Trim(new char[] { '\r', '\n', ' ' }));
            return info.ToTripRoute();
        }

        public static TrackFileList GetRoutes(string jsonRoutes)
        {
            string[] data = Regex.Split(jsonRoutes, "w*" + separatorDays + "w*");

            if (data.Length != 2)
                throw new ApplicationException("Файл поврежден");

            //маршурты
            List<TrackFile> array = JsonConvert.DeserializeObject<List<TrackFile>>(data[0]);

            //информация
            List<info> infs = JsonConvert.DeserializeObject<List<info>>(data[1]);

            if (array.Count != infs.Count)
                throw new ApplicationException("В файле не фватает информации о дневных маршрутах");
            for (int i = 0; i < array.Count; i++)
            {
                array[i].Color = infs[i].Color;
                array[i].Name = infs[i].Name;
                array[i].Description = infs[i].Description;
                array[i].CalculateAll();
            }
            return new TrackFileList(array);
        }

        public static TrackFile GetWaypoints(string jsonWaypoints)
        {
            TrackFile wpts = JsonConvert.DeserializeObject<TrackFile>(jsonWaypoints);
            return wpts;
        }

        #endregion
    }
}
