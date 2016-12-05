﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// настройки источников данных(геокодер, высоты)
    /// </summary>
    public class DataSources
    {
        /// <summary>
        /// создает новый экземпляр настроек источников данных
        /// </summary>
        public DataSources()
        {
            ETOPO2DBFolder = Application.StartupPath+"\\Data\\ETOPO2";
        }

        /// <summary>
        /// поставщик геокодера
        /// </summary>
        public GeoCoderProvider GeoCoderProvider { get; set; }

        /// <summary>
        /// поставщик геоинформации
        /// </summary>
        public GeoInfoProvider GeoInfoProvider { get; set; }

        /// <summary>
        /// папка базы данных ETOPO2
        /// </summary>
        public string ETOPO2DBFolder { get; set; }
    }
}