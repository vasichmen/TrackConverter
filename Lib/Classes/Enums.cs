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
        Qps,

        /// <summary>
        /// Bitly
        /// </summary>
        Bitly,

        /// <summary>
        /// сокращение ссылок вконтакте
        /// </summary>
        VK
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
        SelectedPoint,

        /// <summary>
        /// Результат поиска
        /// </summary>
        SearchResult
    }

    /// <summary>
    /// тип разделения маршрута (по ближайшей точке, по расстоянию)
    /// </summary>
    public enum SeparateRouteType
    {
        /// <summary>
        /// Разделение маршрута по ближайшей заданной точке
        /// </summary>
        Nearest,

        /// <summary>
        /// Разделение маршрута по заданной длине первого отрезка
        /// </summary>
        Length,

        /// <summary>
        /// Без типа (при отмене)
        /// </summary>
        None
    }

    /// <summary>
    /// тип точки при прокладке маршрута
    /// </summary>
    public enum PathingType
    {
        /// <summary>
        /// начальная точка
        /// </summary>
        Start,

        /// <summary>
        /// промеуточная точка
        /// </summary>
        Intermed,

        /// <summary>
        /// финишная точка
        /// </summary>
        Finish,

        /// <summary>
        /// Без типа
        /// </summary>
        None
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

    /// <summary>
    /// диалог обновления программы
    /// </summary>
    public enum UpdateDialogAnswer
    {
        /// <summary>
        /// всегда спрашивать
        /// </summary>
        AlwaysAsk,

        /// <summary>
        /// всегда игнорировать обновления
        /// </summary>
        AlwaysIgnore,

        /// <summary>
        /// всегда принимать обновления
        /// </summary>
        AlwaysAccept
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
        /// полярный перебор
        /// </summary>
        PolarSearch
    }

    /// <summary>
    /// поведение нормализатора при нормализации
    /// </summary>
    public enum NormalizerBehavior
    {
        /// <summary>
        /// удаляет точки, которые приводят к острым углам
        /// </summary>
        RemovePoint,

        /// <summary>
        /// ставит вместо "острых" точек такие точки, чтоб угол был минимально допустимый
        /// </summary>
        AddCritical
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
    /// тип точки в маршруте
    /// </summary>
    public enum RouteWaypointType
    {
        /// <summary>
        /// Старт
        /// </summary>
        Start,

        /// <summary>
        /// Достопримечательность
        /// </summary>
        Interest,

        /// <summary>
        /// Точка сбора
        /// </summary>
        CollectPoint,

        /// <summary>
        /// Привал
        /// </summary>
        Camp,

        /// <summary>
        /// Место для ночёвки
        /// </summary>
        Overnight,

        /// <summary>
        /// Финиш
        /// </summary>
        Finish,

        /// <summary>
        /// точка без типа
        /// </summary>
        None,

        /// <summary>
        /// магазин
        /// </summary>
        Shop,

        /// <summary>
        /// источник воды
        /// </summary>
        WaterSource
    }

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
        GoogleHybridMap,

        /// <summary>
        /// карта викимапии
        /// </summary>
        WikimapiaMap,

        /// <summary>
        /// карты ГГЦ
        /// </summary>
        GGC
    }

    /// <summary>
    /// поставщики слоёв на карте
    /// </summary>
    public enum MapLayerProviders
    {
        /// <summary>
        /// Wikimapia
        /// </summary>
        Wikimapia,

        /// <summary>
        /// карта треков OSM
        /// </summary>
        OSMGPSTracks,

        /// <summary>
        /// карта железных дорог
        /// </summary>
        OSMRailways,

        /// <summary>
        /// карта дорожного покрытия
        /// </summary>
        OSMRoadSurface,

        /// <summary>
        /// пробки яндекса
        /// </summary>
        YandexTraffic,

        /// <summary>
        /// отсутствует слой
        /// </summary>
        None
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
        /// файл microsoft word
        /// </summary>
        DocFile,

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
        /// Файл туристического маршрута TRR
        /// </summary>
        TrrFile,

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