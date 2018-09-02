using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// методы для работы с форматом Trip Route
    /// </summary>
    internal static class TrrHelper
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
        private class Info
        {
            /// <summary>
            /// для Json
            /// </summary>
            public Info()
            { }

            /// <summary>
            /// создает структуру из объекта  TrackFile
            /// </summary>
            /// <param name="trip"></param>
            public Info(TripRouteFile trip)
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
                TripRouteFile res = new TripRouteFile
                {
                    Name = this.Name,
                    Description = this.Description,
                    Color = this.Color
                };
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
            Info info = new Info(trip);
            string json = JsonConvert.SerializeObject(info, Formatting.Indented);
            return json;
        }

        public static string GetRoutes(TripRouteFile trip)
        {
            string array = JsonConvert.SerializeObject(trip.DaysRoutes, Formatting.Indented);
            List<Info> infs = new List<Info>();
            foreach (TrackFile tf in trip.DaysRoutes)
                infs.Add(new Info()
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
            Info info = JsonConvert.DeserializeObject<Info>(jsonInformation.Trim(new char[] { '\r', '\n', ' ' }));
            return info.ToTripRoute();
        }

        public static TrackFileList GetRoutes(string jsonRoutes)
        {
            string[] data = Regex.Split(jsonRoutes, "w*" + separatorDays + "w*");

            if (data.Length != 2)
                throw new ApplicationException("Файл поврежден");

            //маршруты
            List<TrackFile> array = JsonConvert.DeserializeObject<List<TrackFile>>(data[0]);

            //информация
            List<Info> infs = JsonConvert.DeserializeObject<List<Info>>(data[1]);

            if (array.Count != infs.Count)
                throw new ApplicationException("В файле не хватает информации о дневных маршрутах");
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
