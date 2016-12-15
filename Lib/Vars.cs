﻿using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Classes.Options;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Tracking;
using TrackConverter.Lib.Mathematic.Geodesy.Systems;

namespace TrackConverter
{
    /// <summary>
    /// глобальные переменные
    /// </summary>
    public class Vars
    {  
        /// <summary>
        /// настройки программы
        /// </summary>
        public static Options Options { get; set; }

        /// <summary>
        /// смещения в градусах за одно нажатие на cтрелку при перемещении карты в различных масштабах.
        /// Ключ - масштаб от 2 до 20, значение - смещение в градусах
        /// </summary>
        public static Dictionary<int, double> MapMoveSteps
        {
            get
            {
                return new Dictionary<int, double>() { 
                    {2, 1.112 } ,
                    {3, 0.4448},
                    {4, 0.2224},
                    {5, 0.1112},
                    {6, 0.0556},
                    {7, 0.0278},
                    {8, 0.01668},
                    {9, 0.00556},
                    {10, 0.00334},
                    {11, 0.00167},
                    {12, 0.00111},
                    {13, 0.00044},
                    {14, 0.00022},
                    {15, 0.00011},
                    {16, 0.00006},
                    {17, 0.00003},
                    {18, 0.00002},
                    {19, 0.0000056},
                    {20, 0.0000033}
                };
            }
        }

        /// <summary>
        /// текущий выделенный трек в списке маршрутов для обновления информации во всех окнах
        /// </summary>
        public static TrackFile currentSelectedTrack { get; set; }

        /// <summary>
        /// кэш данных из интернет-сервисов
        /// </summary>
        public static SQLiteCache dataCache = null;

        /// <summary>
        /// задача загрузки базы данных ETOPO2
        /// </summary>
        public static Task TaskLoadingETOPO2;

        /// <summary>
        /// текущая система координат
        /// </summary>
        public static IGeosystem CurrentGeosystem;
    }
}
