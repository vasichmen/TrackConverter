using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter
{


    #region InternetServices

    /// <summary>
    /// источник данных для геокодера
    /// </summary>
    public enum GeoCoderProvider
    {
        /// <summary>
        /// яндекс
        /// </summary>
        Yandex,

        /// <summary>
        /// гугл
        /// </summary>
        Google,

        /// <summary>
        /// Геокодер OpenStreetMaps
        /// </summary>
        Nominatim
    }

    /// <summary>
    /// источник прокладки маршрута
    /// </summary>
    public enum PathRouteProvider
    {
        /// <summary>
        /// Google
        /// </summary>
        Google,

        /// <summary>
        /// Яндекс
        /// </summary>
        Yandex
    }

    /// <summary>
    /// поставщик геоинформации
    /// </summary>
    public enum GeoInfoProvider
    {
        /// <summary>
        /// Гугл
        /// </summary>
        Google,

        /// <summary>
        /// GTOPO30. (только высоты над уровнем моря)
        /// </summary>
        GTOPO30,

        /// <summary>
        /// ETOPO
        /// </summary>
        ETOPO
    }

    /// <summary>
    /// поставщик сокращения ссылок
    /// </summary>
    public enum LinkShorterProvider
    {
        /// <summary>
        /// яндекс
        /// </summary>
        Clck,

        /// <summary>
        /// QPS
        /// </summary>
        Qps

    }

    /// <summary>
    /// тип прокладываемого сервисом маршрута 
    /// </summary>
    public enum PathRouteMode
    {
        /// <summary>
        /// пешком
        /// </summary>
        Walk,

        /// <summary>
        /// на автомобиле
        /// </summary>
        Driving

    }


    #endregion

    #region UI

    /// <summary>
    /// тип диалогового окна FormReadText
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        /// ввод текста
        /// </summary>
        ReadText,

        /// <summary>
        /// вывод текста
        /// </summary>
        ExportText,

        /// <summary>
        /// ввод числа
        /// </summary>
        ReadNumber,

        /// <summary>
        /// ввод расширения
        /// </summary>
        ReadExtension
    }

    /// <summary>
    /// типы маркеров на карте
    /// </summary>
    public enum MarkerTypes
    {
        /// <summary>
        /// маркер нового маршрута
        /// </summary>
        CreatingRoute,

        /// <summary>
        /// путевая точка
        /// </summary>
        Waypoint,

        /// <summary>
        /// Что здесь?
        /// </summary>
        WhatThere,

        /// <summary>
        /// Точки начала и конца прокладываемого машрута
        /// </summary>
        PathingRoute,

        /// <summary>
        /// выделенная точка
        /// </summary>
        SelectedPoint
    }

    /// <summary>
    /// Основное окно программы
    /// </summary>
    public enum MainWindowType
    {

        /// <summary>
        /// окно конвертера
        /// </summary>
        Converter,

        /// <summary>
        /// окно карты
        /// </summary>
        Map

    }

    /// <summary>
    /// тип измененной информации
    /// </summary>
    public enum EditInfoType
    {
        /// <summary>
        /// перемещение маркера
        /// </summary>
        MarkerMove,

        /// <summary>
        /// добавление маркера
        /// </summary>
        MarkerAdd,

        /// <summary>
        /// изменение информации о маркере
        /// </summary>
        MarkerEdit,

        /// <summary>
        /// удаление маркера
        /// </summary>
        MarkerDelete
    }

    #endregion

    #region Вычисления


    /// <summary>
    /// единицы измерения углов
    /// </summary>
    public enum AngleMeasure
    {
        /// <summary>
        /// радианы
        /// </summary>
        Radians,

        /// <summary>
        /// градусы
        /// </summary>
        Degrees
    }

    /// <summary>
    /// система координат
    /// </summary>
    public enum Geosystems
    {
        /// <summary>
        /// эллипсоид WGS84
        /// </summary>
        WGS84,

        /// <summary>
        /// эллипсоид ПЗ-90
        /// </summary>
        PZ90
    }

    /// <summary>
    /// способ вычисления оптимального маршрута через  точки
    /// </summary>
    public enum OptimalMethodType
    {
        /// <summary>
        /// Метод ветвей и границ
        /// </summary>
        BranchBounds,

        /// <summary>
        /// "Жадный" алгоритм
        /// </summary>
        Greedy,

        /// <summary>
        /// полный перебор
        /// </summary>
        FullSearch,

        /// <summary>
        /// рекурсивный полный перебор
        /// </summary>
        RecursiveEnum
    }



    #endregion

    #region Базы данных

    /// <summary>
    /// тип базы данных ETOPO
    /// </summary>
    public enum ETOPODBType
    {
        /// <summary>
        /// Float 
        /// </summary>
        Float,

        /// <summary>
        /// Int16
        /// </summary>
        Int16,

        /// <summary>
        /// SQLite
        /// </summary>
        SQLite
    }

    #endregion

    /// <summary>
    /// поставщик карты
    /// </summary>
    public enum MapProviders
    {
        /// <summary>
        /// яндекс спутник
        /// </summary>
        YandexSatelliteMap,

        /// <summary>
        /// яндекс схема
        /// </summary>
        YandexMap,

        /// <summary>
        /// яндекс гибрид
        /// </summary>
        YandexHybridMap,

        /// <summary>
        /// OpenCycleMap
        /// </summary>
        OpenCycleMap,

        /// <summary>
        /// гугл схема
        /// </summary>
        GoogleMap,

        /// <summary>
        /// гугл спутник
        /// </summary>
        GoogleSatelliteMap,

        /// <summary>
        /// гугл гибрид
        /// </summary>
        GoogleHybridMap
    }


    /// <summary>
    /// форматы представления маршрута
    /// </summary>
    public enum FileFormats
    {
        /// <summary>
        /// файл plt
        /// </summary>
        PltFile,

        /// <summary>
        /// файл rt2
        /// </summary>
        Rt2File,

        /// <summary>
        /// файл wpt
        /// </summary>
        WptFile,

        /// <summary>
        /// файл crd
        /// </summary>
        CrdFile,

        /// <summary>
        /// файл kml
        /// </summary>
        KmlFile,

        /// <summary>
        /// файл gpx
        /// </summary>
        GpxFile,

        /// <summary>
        /// файл rte
        /// </summary>
        RteFile,

        /// <summary>
        /// файл kmz
        /// </summary>
        KmzFile,

        /// <summary>
        /// файл csv
        /// </summary>
        CsvFile,

        /// <summary>
        /// файл osm
        /// </summary>
        OsmFile,

        /// <summary>
        /// файл nmea
        /// </summary>
        NmeaFile,

        /// <summary>
        /// файл txt
        /// </summary>
        TxtFile,

        /// <summary>
        /// Ссылка на маршрут Яндекс.Карт
        /// </summary>
        YandexLink,

        /// <summary>
        /// Ссылка на маршрут Wikimapia
        /// </summary>
        WikimapiaLink,

        /// <summary>
        /// последовательный список адресов
        /// </summary>
        AddressList,

        /// <summary>
        /// неопределенный формат файла
        /// </summary>
        Undefined
    }

    /// <summary>
    /// формат файла настроек
    /// </summary>
    public enum OptionsFormat
    {
        /// <summary>
        /// формат JSON
        /// </summary>
        JSON,

        /// <summary>
        /// формат XML
        /// </summary>
        XML
    }

}