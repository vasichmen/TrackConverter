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
            ETOPODBFolder = "";
            GeoCoderProvider = GeoCoderProvider.Yandex;
            GeoInfoProvider = GeoInfoProvider.Google;
            UseGeocoderCache = true;
            UseSystemTimeZones = false;
            UseMapLayerCache = true;
            UseExtInfoCache = true;
            UseImagesCache = true;

            MaxImageCacheDays = 30;
            MaxVectorLayersCacheDays = 0.5;
            MaxGeocoderCacheDays = 30;
            MaxExtInfoCacheDays = 1;

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
        /// папка базы данных ETOPO
        /// </summary>
        public string ETOPODBFolder { get; set; }

        /// <summary>
        /// Использовать кэш геокодера и высот в интернет-сервисах
        /// </summary>
        public bool UseGeocoderCache { get; set; }

        /// <summary>
        /// Использовать системные часовые пояса. (в противном случае используются данные геокодера)
        /// </summary>
        public bool UseSystemTimeZones { get;  set; }

        /// <summary>
        /// если истина, то будет использоваться кэш объектов слоя карты
        /// </summary>
        public bool UseMapLayerCache { get;  set; }

        /// <summary>
        /// максимальное время хранения кэша картинок в днях (по умолчанию 30 дней)
        /// </summary>
        public double MaxImageCacheDays { get;  set; }

        /// <summary>
        /// максимальное время хранения кэша векторных слоёв в днях (по умолчанию 7 дней)
        /// </summary>
        public double MaxVectorLayersCacheDays { get; set; }

        /// <summary>
        /// время хранения кэша геокодера в днях
        /// </summary>
        public double MaxGeocoderCacheDays { get; set; }

        /// <summary>
        /// время хранения кэша информации об объектах в днях
        /// </summary>
        public double MaxExtInfoCacheDays { get; set; }

        /// <summary>
        /// использовать кэш для информации об объектах в днях
        /// </summary>
        public bool UseExtInfoCache { get; set; }

        /// <summary>
        /// использовать кэш картинок в днях
        /// </summary>
        public bool UseImagesCache { get; set; }
    }
}
