using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Tracking.Helpers;
using TrackConverter.Res;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Tracking
{

    /// <summary>
    /// класс сериализации структур в различные форматы
    /// </summary>
    public static class Serializer
    {

        /// <summary>
        /// сериализация одного трека в файл или ссылку
        /// </summary>
        /// <param name="FileName">имя файла (если ссылка то в этом поле количество точек)</param>
        /// <param name="track">трек</param>
        /// <param name="format">формат</param>
        /// <param name="callback">действие, выполняемое при сохранении файла</param>
        /// <returns></returns>
        public static string Serialize(string FileName, BaseTrack track, FileFormats format, Action<string> callback = null)
        {
            switch (format)
            {
                case FileFormats.PltFile:
                    ExportPLT(FileName, track);
                    return null;
                case FileFormats.CrdFile:
                    ExportCRD(FileName, track);
                    return null;
                case FileFormats.WptFile:
                    ExportWPT(FileName, track);
                    return null;
                case FileFormats.Rt2File:
                    ExportRT2(FileName, track);
                    return null;
                case FileFormats.GpxFile:
                    ExportGPX(FileName, track);
                    return null;
                case FileFormats.DocFile:
                    exportDOC(FileName, track, callback);
                    return null;
                case FileFormats.KmlFile:
                    if (track is TrackFile)
                        exportKML(FileName, new TrackFileList(track));
                    if (track is TripRouteFile)
                        exportKML(FileName, (track as TripRouteFile).DaysRoutes);
                    return null;
                case FileFormats.KmzFile:
                    if (track is TrackFile)
                        exportKMZ(FileName, new TrackFileList(track));
                    if (track is TripRouteFile)
                        exportKMZ(FileName, (track as TripRouteFile).DaysRoutes);
                    return null;
                case FileFormats.OsmFile:
                    ExportOSM(FileName, track);
                    return null;
                case FileFormats.TxtFile:
                    ExportTXT(FileName, track);
                    return null;
                case FileFormats.NmeaFile:
                    ExportNMEA(FileName, track);
                    return null;
                case FileFormats.CsvFile:
                    ExportCSV(FileName, track);
                    return null;
                case FileFormats.CsvYandexFile:
                    ExportCSV(FileName, track,true);
                    return null;
                case FileFormats.AddressList:
                    exportADRS(FileName, track, callback);
                    return null;
                case FileFormats.TrrFile:
                    exportTrr(FileName, track);
                    return null;
                case FileFormats.RteFile:
                    if (track is TrackFile)
                        exportRTE(FileName, new TrackFileList(track));
                    if (track is TripRouteFile)
                        exportRTE(FileName, (track as TripRouteFile).DaysRoutes);
                    return null;
                case FileFormats.YandexLink:
                    return ExportYandex(int.Parse(FileName), track);
                case FileFormats.WikimapiaLink:
                    return ExportWikimapia(int.Parse(FileName), track);
                default:
                    throw new NotSupportedException("Неподдерживаемый формат " + format.ToString());
            }

        }

        /// <summary>
        /// сериализация списка треков в файл
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="tracks">список треков</param>
        /// <param name="format">формат</param>
        public static void Serialize(string FileName, TrackFileList tracks, FileFormats format)
        {
            switch (format)
            {
                case FileFormats.RteFile:
                    exportRTE(FileName, tracks);
                    break;
                case FileFormats.KmlFile:
                    exportKML(FileName, tracks);
                    break;
                case FileFormats.KmzFile:
                    exportKMZ(FileName, tracks);
                    break;
                default:
                    throw new NotSupportedException("Неподдерживаемый формат " + format.ToString());
            }

        }

        /// <summary>
        /// сериализация списка треков и списка точек в файл
        /// </summary>
        /// <param name="FileName">имя файал</param>
        /// <param name="geo">объект, представляющий списки</param>
        /// <param name="format">формат</param>
        public static void Serialize(string FileName, GeoFile geo, FileFormats format)
        {
            switch (format)
            {
                case FileFormats.KmlFile:
                    ExportKml(FileName, geo);
                    break;
                case FileFormats.KmzFile:
                    exportKmz(FileName, geo);
                    break;
                default:
                    throw new NotSupportedException("Этот формат не поддерживается: " + format.ToString());

            }
        }

        /// <summary>
        /// экспорт файлов TRR
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="geo">маршрут</param>
        /// <param name="format">формат файла</param>
        public static void Serialize(string FileName, TripRouteFile geo, FileFormats format)
        {
            switch (format)
            {
                case FileFormats.TrrFile:
                    exportTrr(FileName, geo);
                    break;
                default:
                    throw new NotSupportedException("Этот формат не поддерживается: " + format.ToString());

            }
        }

        /// <summary>
        /// загрузка трека из файла или ссылки
        /// </summary>
        /// <param name="FileNameOrLink">имя файла или адрес ссылки</param>
        /// <param name="callback">действие при обработке файла</param>
        /// <returns></returns>
        public static BaseTrack DeserializeTrackFile(string FileNameOrLink, Action<string> callback = null)
        {
            if (string.IsNullOrWhiteSpace(FileNameOrLink))
                throw new ArgumentException("Пустое имя файла");

            FileNameOrLink = FileNameOrLink.ToLower();
            BaseTrack res = null;
            if (FileNameOrLink.Contains("yandex"))
                res = loadYandexRoute(FileNameOrLink);
            else
                if (FileNameOrLink.Contains("wikimapia"))
                res = loadWikimapia(FileNameOrLink);
            else
                    if (!File.Exists(FileNameOrLink))
                throw new Exception("Файл не существует или неправильный формат ссылки:  \r\n" + FileNameOrLink);
            if (res == null)
            {
                string extension = Path.GetExtension(FileNameOrLink);
                switch (extension.ToLower())
                {
                    case ".plt":
                        res = loadPLT(FileNameOrLink);
                        break;
                    case ".crd":
                        res = loadCRD(FileNameOrLink);
                        break;
                    case ".wpt":
                        res = loadWPT(FileNameOrLink);
                        break;
                    case ".rt2":
                        res = loadRT2(FileNameOrLink);
                        break;
                    case ".gpx":
                        res = loadGPX(FileNameOrLink);
                        break;
                    case ".kml":
                        res = loadKMLwaypoints(FileNameOrLink);
                        break;
                    case ".kmz":
                        res = loadKMZwaypoints(FileNameOrLink);
                        break;
                    case ".osm":
                        res = loadOSM(FileNameOrLink);
                        break;
                    case ".txt":
                        res = loadTXT(FileNameOrLink);
                        break;
                    case ".nmea":
                        res = loadNMEA(FileNameOrLink);
                        break;
                    case ".csv":
                        res = loadCSV(FileNameOrLink);
                        break;
                    case ".adrs":
                        res = loadADRS(FileNameOrLink, callback);
                        break;
                    case ".trr":
                        res = loadTRR(FileNameOrLink);
                        break;
                    case ".rte":
                        res = loadRTE(FileNameOrLink)[0];
                        break;
                    default:
                        throw new Exception("Данный файл не поддерживается: " + FileNameOrLink);
                }
            }
            if (res.Color.IsEmpty || res.Color.A == 0)
                res.Color = Vars.Options.Converter.GetColor();
            return res;
        }

        /// <summary>
        /// загрузка списка треков из фйла нескольких треков. Если это один трек, то будет загружен как один и добавлен в список
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <returns></returns>
        public static TrackFileList DeserializeTrackFileList(string FileName)
        {
            TrackFileList res = new TrackFileList();
            if (!File.Exists(FileName))
                throw new Exception("Файл не существует:  " + FileName);

            string extension = FileName.Substring(FileName.LastIndexOf('.') + 1);

            switch (extension)
            {

                case "rte":
                    res = loadRTE(FileName);
                    break;
                case "kml":
                    res = loadKML(FileName);
                    break;
                case "kmz":
                    res = loadKMZ(FileName);
                    break;
                case "trr":
                    res.Add(loadTRR(FileName));
                    break;
                default:
                    res = new TrackFileList
                    {
                        DeserializeTrackFile(FileName)
                    };
                    break;
            }

            foreach (TrackFile tf in res)
                if (tf.Color.IsEmpty || tf.Color.A == 0)
                    tf.Color = Vars.Options.Converter.GetColor();

            return res;
        }

        /// <summary>
        /// Загрузить список несколько файлов 
        /// </summary>
        /// <param name="list">список имен файлов</param>
        /// <returns></returns>
        public static TrackFileList DeserializeTrackFileList(List<string> list)
        {
            if (list == null)
                return new TrackFileList();
            TrackFileList res = new TrackFileList();
            foreach (string fn in list)
            {
                if (File.Exists(fn))
                    res.Add(Serializer.DeserializeTrackFile(fn));
            }
            return res;
        }

        /// <summary>
        /// загрузка списка треков и списка точек из файла
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <returns></returns>
        public static GeoFile DeserializeGeoFile(string FileName)
        {
            GeoFile res;
            if (!File.Exists(FileName))
                throw new Exception("Файл не существует:  " + FileName);

            string extension = Path.GetExtension(FileName).ToLower();

            switch (extension)
            {
                case ".kml":
                    res = loadKml(FileName);
                    break;
                case ".kmz":
                    res = loadKmz(FileName);
                    break;
                default:
                    throw new NotSupportedException("Формат не пoддерживается: " + extension);
            }

            foreach (TrackFile tf in res.Routes)
                if (tf.Color.IsEmpty || tf.Color.A == 0)
                    tf.Color = Vars.Options.Converter.GetColor();
            return res;
        }

        /// <summary>
        /// загрузка файла TripRouteFile
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <returns></returns>
        public static TripRouteFile DeserializeTripRouteFile(string FileName)
        {
            string ext = Path.GetExtension(FileName).ToLower();
            switch (ext)
            {
                case ".trr":
                    return loadTRR(FileName);
                default:
                    throw new ApplicationException("расширение не поддерживается " + ext);
            }
        }


        #region TrackFile

        #region загрузка разных форматов маршрутов

        #region импорт из файлов

        /// <summary>
        /// загрузка точек из файла kmz. Ели надо загружать маршрут, 
        /// то использовать метод списка маршрутов. TrackFileList.LoadKMZ
        /// </summary>
        /// <param name="FilePath"></param>
        private static TrackFile loadKMZwaypoints(string FilePath)
        {
            TrackFile res = new TrackFile();
            FastZip fz = new FastZip();
            string tmpFolder = Vars.Options.Converter.TempFolder + Guid.NewGuid();
            fz.ExtractZip(FilePath, tmpFolder, "-y");
            res = loadKMLwaypoints(tmpFolder + "\\doc.kml");
            Directory.Delete(tmpFolder, true);
            return res;
        }

        /// <summary>
        /// загрузка точек из файла kml. Ели надо загружать маршрут, 
        /// то использовать метод  LoadKML()
        /// </summary>
        /// <param name="FilePath"></param>
        private static TrackFile loadKMLwaypoints(string FilePath)
        {
            TrackFile res = new TrackFile();
            GeoFile gf = Serializer.DeserializeGeoFile(FilePath);
            res.Add(gf.Waypoints);
            return res;
        }

        /// <summary>
        /// Загрузка фала CSV
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private static TrackFile loadCSV(string FilePath)
        {
            StreamReader inputS = new StreamReader(FilePath, new UTF8Encoding(false), true);
            string type = inputS.ReadLine();
            if (type.Contains("\"Широта\";\"Долгота\";\"Описание\";\"Подпись\";\"Номер метки\"")) // если это файл яндекс точек
            {
                TrackFile res = new TrackFile();
                
                //"Широта";"Долгота";"Описание";"Подпись";"Номер метки"
                while (!inputS.EndOfStream)
                {
                    string[] line = Regex.Split(inputS.ReadLine(), "w*;w*");
                    if (line.Length < 4)
                        continue;
                    TrackPoint pt = new TrackPoint(line[0], line[1])
                    {
                        Description = line[2],
                        Name = line[3].Trim('"')             
                    };
                    pt.Icon = IconOffsets.MARKER;
                    res.Add(pt);
                }

                res.Name = Path.GetFileName(FilePath);
                res.FilePath = FilePath;
                return res;
            }
            else //если что-то другое
            {
                TrackFile res = new TrackFile();

                //заголовок и информация
                //<название трека>;<описание трека>;<длина, км>;<время>;<скорость>;<количчество точек>
                while (!inputS.EndOfStream)
                {
                    string[] line = Regex.Split(inputS.ReadLine(), "w*,w*");
                    TrackPoint pt = new TrackPoint(line[1], line[2])
                    {
                        MetrAltitude = Convert.ToDouble(line[3].Replace('.', Vars.DecimalSeparator)),
                        Time = DateTime.Parse(line[4])
                    };
                    TimeSpan tod = TimeSpan.Parse(line[5]);
                    pt.Time = pt.Time.Add(tod);
                    pt.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                    res.Add(pt);
                }

                res.CalculateAll();
                res.Name = Path.GetFileName(FilePath);
                res.FilePath = FilePath;
                return res;
            }
            inputS.Close();

        }

        /// <summary>
        /// загрузка файла списка адресов
        /// </summary>
        /// <param name="FilePath">адрес файла</param>
        /// <param name="callback">действие, выполняемое при обработке файла</param>
        /// <returns></returns>
        private static TrackFile loadADRS(string FilePath, Action<string> callback = null)
        {
            if (callback != null)
                callback.Invoke("Открытие файла " + Path.GetFileName(FilePath));
            StreamReader sr = new StreamReader(FilePath, true);
            List<string> ads = new List<string>();
            string line = "";
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!string.IsNullOrWhiteSpace(line) && !ads.Contains(line))
                    ads.Add(line);
            }

            TrackFile res = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider).GetCoordinates(ads, callback);
            res.FilePath = FilePath;
            res.Name = res.FileName;
            return res;
        }

        /// <summary>
        /// Загрузка файла NMEA
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private static TrackFile loadNMEA(string FilePath)
        {
            TrackFile res = new TrackFile();
            StreamReader inputS = new StreamReader(FilePath, new UTF8Encoding(false), true);
            inputS.ReadLine();
            //$GPRMC,000000.000,V,5949.908,N,03021.021,E,0.00,0.00,010070,,*11
            string[] line;
            while (!inputS.EndOfStream)
            {
                try
                {
                    line = Regex.Split(inputS.ReadLine(), "w*,w*");
                    string title = line[0];
                    switch (title.ToLower())
                    {

                        case "$gprmc":
                            TrackPoint pt = null;
                            string lat = string.Concat(line[3], ',', line[4]);
                            string lon = string.Concat(line[5], ',', line[6]);
                            string date = line[9];
                            string time = line[1];
                            Coordinate cord = Coordinate.Parse(string.Concat(lat, "#", lon), "lat#lon", "ddmm.mmm,H");
                            pt = new TrackPoint(cord)
                            {
                                Icon = IconOffsets.CREATING_ROUTE_MARKER
                            };
                            res.Add(pt);
                            break;
                    }
                }
                catch (Exception) { }
            }
            inputS.Close();
            res.CalculateAll();
            res.Name = Path.GetFileName(FilePath);
            res.FilePath = FilePath;
            return res;
        }

        /// <summary>
        /// Загрузка файла TXT
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private static TrackFile loadTXT(string FilePath)
        {
            TrackFile res = new TrackFile();
            StreamReader inputS = new StreamReader(FilePath, new UTF8Encoding(false), true);
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            string title = inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            inputS.ReadLine();
            string[] ln = Regex.Split(title, "w*\tw*");
            res.Name = ln[1];
            while (!inputS.EndOfStream)
            {
                string[] line = Regex.Split(inputS.ReadLine(), "w*\tw*");
                string name = line[0];
                string pos = line[1];

                Coordinate cd = Coordinate.Parse(pos, "lon lat", "Hdd mm.mmm");
                TrackPoint pt = new TrackPoint(cd)
                {
                    Name = name,
                    Icon = IconOffsets.CREATING_ROUTE_MARKER
                };
                res.Add(pt);
            }
            inputS.Close();
            res.CalculateAll();
            res.FilePath = FilePath;
            return res;
        }

        /// <summary>
        /// Загрузка файла OSM
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private static TrackFile loadOSM(string FilePath)
        {
            TrackFile res = new TrackFile();
            XmlDocument doc = new XmlDocument();
            doc.Load(FilePath);
            foreach (XmlNode nd in doc.DocumentElement.ChildNodes)
            {
                if (nd.LocalName.ToLower() == "node")
                {
                    string lat = nd.Attributes["lat"].Value;
                    string lon = nd.Attributes["lon"].Value;
                    string alt = nd.Attributes["alt"] == null ? "-777" : nd.Attributes["alt"].Value;
                    double Alt = double.Parse(alt.Replace('.', Vars.DecimalSeparator));
                    TrackPoint tp = new TrackPoint(lat, lon)
                    {
                        MetrAltitude = Alt,
                    };
                    tp.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                    res.Add(tp);
                }
                if (nd.LocalName.ToLower() == "way")
                {
                    foreach (XmlNode xn in nd.ChildNodes)
                        if (xn.LocalName.ToLower() == "tag")
                        {
                            if (xn.Attributes["k"].Value.ToLower() == "name")
                                res.Name = xn.Attributes["v"].Value;
                        }
                }
            }
            res.FilePath = FilePath;
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// Загрузка файла *.gpx
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private static TrackFile loadGPX(string FilePath)
        {
            TrackFile res = new TrackFile();
            XmlDocument xml = new XmlDocument();
            xml.Load(FilePath);
            string routeName = xml.GetElementsByTagName("name")[0].InnerText;


            XmlNode trackNode = null;

            //выбор корневого элемента трека
            foreach (XmlNode root in xml.DocumentElement.ChildNodes)
                if (root.Name.ToLower() == "trk")
                    trackNode = root;

            if (trackNode == null)
                throw new InvalidOperationException("Файл gpx поврежден или имеет неизвестный формат");

            foreach (XmlNode nd in trackNode.ChildNodes)
                if (nd.Name.ToLower() == "trkseg") // выбор секции
                    foreach (XmlNode pt in nd.ChildNodes)
                        if (pt.Name.ToLower() == "trkpt")
                        {
                            string lat, lon, alt, tim;
                            alt = "";
                            tim = "";

                            lat = pt.Attributes.GetNamedItem("lat").Value;
                            lon = pt.Attributes.GetNamedItem("lon").Value;
                            foreach (XmlNode cd in pt.ChildNodes)
                            {
                                alt = (cd.Name.ToLower() == "ele") ? cd.InnerText : alt;
                                tim = (cd.Name.ToLower() == "time") ? cd.InnerText : tim;
                            }

                            double altD = alt == "" ? -777 : Convert.ToDouble(alt.Replace('.', Vars.DecimalSeparator));
                            DateTime dt = tim == "" || tim == "01.01.0001 0:00:00" ? DateTime.MinValue : Convert.ToDateTime(tim) - Vars.Options.Converter.UTCDifferrent;
                            TrackPoint nv = new TrackPoint(lat, lon)
                            {
                                MetrAltitude = altD, //высота в метрах
                                Time = dt//время установки точки
                            };
                            nv.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                            res.Add(nv);
                        }

            res.Name = routeName;
            res.FilePath = FilePath;
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// Загрузка файла *.rt2
        /// </summary>
        /// <param name="FilePath">имя файла</param>
        private static TrackFile loadRT2(string FilePath)
        {
            TrackFile res = new TrackFile();
            StreamReader sr = new StreamReader(FilePath, new UTF8Encoding(false), true);
            //переходим на начало списка точек
            string st = sr.ReadLine(); //заголовок и версия
            st = sr.ReadLine(); //формат координат
            if (!st.ToLower().Contains("wgs 84"))
                throw new Exception("Неверный формат значений координат в файле");

            //H3,описание,,цвет
            st = sr.ReadLine(); //описание трека

            string[] arrd = Regex.Split(st, "w*,w*");
            res.Description = arrd[1]; //записываем описание
            if (arrd[3] != "0")
                res.Color = Color.FromArgb(int.Parse(arrd[3])); //цвет
            res.FilePath = FilePath;
            res.Name = res.FileName;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line.Replace(" ", "");
                string[] arr = Regex.Split(line, "w*,w*");
                TrackPoint nv = new TrackPoint(arr[2], arr[3]);

                //высота в футах
                double f = double.NaN;
                bool ff = double.TryParse(arr[4].Replace('.', Vars.DecimalSeparator), out f);
                nv.FeetAltitude = ff ? f : double.NaN;
                nv.Icon = IconOffsets.CREATING_ROUTE_MARKER;

                res.Add(nv);
            }
            sr.Close();
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// Загрузка файла PLT и вычисление длины трека
        /// </summary>
        /// <param name="FilePath">Путь к файлу PLT</param>
        private static TrackFile loadPLT(string FilePath)
        {
            /*СТРОКА ФАЙЛА *.PLT
           широта,долгота,код(0 - обычная точка, 1 - пауза),высота в футах ( -777 высота недоступна),время
           */

            TrackFile res = new TrackFile();
            if (!Regex.IsMatch(FilePath, "w*.plt"))
                throw new ArgumentException("Поддерживаются файлы только формата plt");
            StreamReader sr = new StreamReader(FilePath, new UTF8Encoding(false), true);
            //переходим на начало списка точек
            float version = 1;
            string st = sr.ReadLine(); //заголовок и версия формата файла
            if (st.Contains("1.0"))
                version = 1;
            else if (st.Contains("2.0"))
                version = 2;
            else if (st.Contains("2.1"))
                version = 2.1f;
            st = sr.ReadLine(); //формат значений координат
            if (!st.ToLower().Contains("wgs 84"))
                throw new Exception("Неподдерживаемый эллипсоид в файле: " + st + ".\r\n(Требуется эллипсоид WGS 84)");
            if (version >= 2)
            {
                st = sr.ReadLine(); //напоминание про футы
                st = sr.ReadLine(); //зарезервированная строка
            }

            //0,2,255,OziCE Track Log File,1

            st = sr.ReadLine(); //параметры трека
            string[] arrd = Regex.Split(st, "w*,w*");
            res.Description = arrd[3]; //записываем описание трека
            res.FilePath = FilePath; //путь к файлу
            if (arrd[2] != "0")
                res.Color = Color.FromArgb(int.Parse(arrd[2]));
            if (version >= 2)
                sr.ReadLine(); //количество точек в треке
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                line.Replace(" ", "");
                string[] arr = Regex.Split(line, "w*,w*");
                TrackPoint nv = new TrackPoint(arr[0], arr[1])
                {
                    Time = arr[4] == "" ? DateTime.MinValue : DateTime.FromOADate(Convert.ToDouble(arr[4].Replace('.', Vars.DecimalSeparator).Trim())) + Vars.Options.Converter.UTCDifferrent
                };

                //высота в футах
                double f = double.NaN;
                bool ff = double.TryParse(arr[3].Replace('.', Vars.DecimalSeparator), out f);
                nv.FeetAltitude = ff ? f : double.NaN;
                nv.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                res.Add(nv);
            }
            sr.Close();
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// Загрузка файла WPT и вычисление длины трека
        /// </summary>
        /// <param name="FilePath">Путь к файлу WPT</param>
        private static TrackFile loadWPT(string FilePath)
        {
            TrackFile res = new TrackFile();
            StreamReader inputS = new StreamReader(FilePath, new UTF8Encoding(false), true);
            inputS.ReadLine(); //заголовок файла  и информация о версии
            string st = inputS.ReadLine(); //датум
            if (!st.ToLower().Contains("wgs 84"))
                throw new Exception("Неизвестный датум: " + st);

            st = inputS.ReadLine(); //параметры трека
            if (st.ToLower() != "Reserved 2")
            {
                //0,описание,,цвет
                string[] arrd = Regex.Split(st, "w*,w*");
                res.Description = arrd[1]; //описание трека
                if (arrd[3] != "0")
                    res.Color = Color.FromArgb(int.Parse(arrd[3])); //цвет
            }
            inputS.ReadLine(); //зарезервированная строка
            while (!inputS.EndOfStream)
            {
                string[] line = Regex.Split(inputS.ReadLine(), "w*,w*");
                TrackPoint pt = new TrackPoint(line[2], line[3])
                {
                    Name = line[1], //имя точки
                                    //время
                    Time = line[4] == "" ? DateTime.MinValue : DateTime.FromOADate(Convert.ToDouble(line[4].Replace('.', Vars.DecimalSeparator).Trim())) + Vars.Options.Converter.UTCDifferrent,
                    //описание точки
                    Description = line[10]
                };
                //высота в метрах
                double f = double.NaN;
                bool ff = double.TryParse(line[14].Replace('.', Vars.DecimalSeparator), out f);
                pt.MetrAltitude = ff ? f : double.NaN;
                //иконка
                pt.Icon = int.Parse(line[5]);
                res.Add(pt);
            }
            inputS.Close();
            res.CalculateAll();
            res.Name = Path.GetFileName(FilePath);
            res.FilePath = FilePath;
            return res;
        }

        /// <summary>
        /// Загрузка файла CRD и вычисление длины трека
        /// </summary>
        /// <param name="FilePath">Путь к файлу CRD</param>
        private static TrackFile loadCRD(string FilePath)
        {
            TrackFile res = new TrackFile();
            StreamReader inputS = new StreamReader(FilePath, new UTF8Encoding(false), true);
            inputS.ReadLine();
            int version = 0;
            if (inputS.ReadLine().Contains("1.0"))
                version = 1;
            else if (inputS.ReadLine().Contains("2.0"))
                version = 2;
            //заголовок и информация
            //<название трека>;<описание трека>;<длина, км>;<время>;<скорость>;<количчество точек>
            if (version == 1)
            {
                string str = inputS.ReadLine();
                string[] arrd = str.Split(';');
                res.Name = arrd[0];
                res.Description = arrd[1];
            }

            inputS.ReadLine(); // зарезирвированная строка
            while (!inputS.EndOfStream)
            {
                string[] line = Regex.Split(inputS.ReadLine(), "w*;w*");
                TrackPoint pt = new TrackPoint(line[0], line[1]);

                //высота в футах
                double f = double.NaN;
                bool ff = double.TryParse(line[2].Replace('.', Vars.DecimalSeparator), out f);
                pt.MetrAltitude = ff ? f : double.NaN;

                pt.Time = DateTime.Parse(line[3]) + Vars.Options.Converter.UTCDifferrent;
                pt.Name = line[4];
                pt.Description = line[5];
                pt.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                res.Add(pt);
            }

            inputS.Close();
            res.CalculateAll();
            res.Name = Path.GetFileName(FilePath);
            res.FilePath = FilePath;
            return res;
        }

        #endregion

        #region импорт из ссылок

        /// <summary>
        /// Загрузка маршрута яндекс карт
        /// </summary>
        /// <param name="route">Ссылка на маршрут</param>
        private static TrackFile loadYandexRoute(string route)
        {
            TrackFile res = new TrackFile();
            //http://maps.yandex.ru/?ll=37.859722%2C55.752218&spn=0.080681%2C0.038967&z=14&l=map&rl=37.84993730%2C55.75729500~0.01613617%2C-0.00522755~0.02205849%2C0.00861540~-0.01999855%2C0.00440376
            //https://yandex.ru/maps/21621/reutov/?source=serp_navig&text=%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%82%D0%B8%D1%80%D1%83%D0%B5%D1%82%D1%81%D1%8F%20%D0%BB%D0%B8%20%D1%8F%D1%87%D0%B5%D0%B9%D0%BA%D0%B0%20%D0%B2%20datagridview&sll=37.859651%2C55.758281&sspn=0.074329%2C0.038187&rl=37.86025139%2C55.77272534~0.00420570%2C-0.01679174~0.01012802%2C-0.00348508~0.01390457%2C0.01355132~-0.01596451%2C0.00425801~-0.04454613%2C-0.00488707~0.01264555%2C-0.01156691~0.01590301%2C-0.00183941~0.02011744%2C-0.00464731&ll=37.859651%2C55.758281&z=14&sctx=CAAAAAEAaRmp91TuQkDOFhBaD%2BFLQGISLuQR3Kg%2FJV0z%2BWaboz8FAAAAAAECAwUUAAAAAAAAAAAAAAAAAAAAAHVUAAABAACAvwAAAAAAAAAA
            //удаление ненужной информации в начале ссылки,
            string alpts;
            if (route.LastIndexOf("rl=") > 0)
                alpts = route.Remove(0, route.LastIndexOf("rl=") + 3);
            else
                throw new ArgumentException("Неверный формат ссылки!");

            //...в конце ссылки,...
            if (alpts.IndexOf("&ll=") > 0)
                alpts = alpts.Remove(alpts.IndexOf("&ll="));


            //...остается список точек в формате
            //долгота0%2Cширота0~приращение долготы1%2Cприращение широты1~приращение долготы2%2Cприращение широты2
            //разбиваем список на пары приращений долгота - широта
            string[] points = Regex.Split(alpts, "S*~S*");
            bool first = true;
            foreach (string pt in points)
            {
                TrackPoint npt = null;
                //разбиваем пары долгота%2Cширота
                string[] pair = Regex.Split(pt.ToLower(), "%2c");
                if (first)
                {
                    npt = new TrackPoint(pair[1], pair[0]);
                    first = false;
                }
                else
                {
                    //долгота, широта
                    double nlon = Convert.ToDouble(pair[0].Replace('.', Vars.DecimalSeparator));
                    double nlat = Convert.ToDouble(pair[1].Replace('.', Vars.DecimalSeparator));
                    npt = new TrackPoint(nlat + res[res.Count - 1].Coordinates.Latitude, nlon + res[res.Count - 1].Coordinates.Longitude);
                }
                npt.Icon = IconOffsets.CREATING_ROUTE_MARKER;
                res.Add(npt);
            }
            res.Name = "Yandex";
            res.FilePath = "Yandex\\yandexRoute" + DateTime.Now.ToString();
            res.CalculateAll();
            return res;
        }

        /// <summary>
        /// Загрузка маршрута викимапии
        /// </summary>
        /// <param name="route">Ссылка на маршрут</param>
        private static TrackFile loadWikimapia(string route)
        {
            TrackFile res = new TrackFile();
            //http://wikimapia.org/#lang=ru&lat=55.735374&lon=37.861032&z=14&m=ys&gz=0;378486728;557230014;123596;77576;0;0;478076;235824
            //удаление ненужной информации в начале ссылки,
            string alpts;
            if (route.LastIndexOf("gz=") > 0)
                alpts = route.Remove(0, route.LastIndexOf("gz=") + 5);
            else
                throw new ArgumentException("Неверный формат ссылки!");
            //остается список точек в формате:
            // [координаты нулевой точки  долгота;широта];[координаты вектора к 1 точке долгота;широта];[координаты вектора к 2 точке долгота;широта]; итд
            //выбирается система отсчета (нулевая точка) и относительно нее строятся вектора к точкам маршрута
            //координаты нулевой точки в новой системе координат: 0;0
            //координаты нулевой точки в земной системе: выбираются как минимальные значения 
            //  из всех координат маршрута lat=min(lat[]), lon=min(lon[])
            //разбиваем список на последовательность координат
            string[] routeArray = Regex.Split(alpts, "S*;S*");
            //список точек
            List<int[]> points = new List<int[]>();
            for (int i = 0; i < routeArray.Length; i += 2)
            {
                //в массив points записываем по 2 числа из общего массива, отвечающие за каждую точку на карте
                int[] pt = new int[] { Convert.ToInt32(routeArray[i]), Convert.ToInt32(routeArray[i + 1]) };
                points.Add(pt);
            }

            int lonBase = Convert.ToInt32(points[0][0]);
            int latBase = Convert.ToInt32(points[0][1]);
            for (int i = 1; i < points.Count; i++)
            {
                //долгота
                int lon = points[i][0];
                //широта
                int lat = points[i][1];
                TrackPoint npt = new TrackPoint((latBase + lat) / 10000000.0000000, (lonBase + lon) / 10000000.0000000)
                {
                    Icon = IconOffsets.CREATING_ROUTE_MARKER
                };
                res.Add(npt);
            }
            res.Name = "Wikimapia";
            res.FilePath = "Wikimapia\\wikimapiaRoute" + DateTime.Now.ToString();
            ;
            res.CalculateAll();
            return res;
        }
        #endregion

        #endregion

        #region экспорт в разные форматы маршрутов

        #region экспорт в файлы

        /// <summary>
        /// Экспорт в RT2
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportRT2(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            outputS.WriteLine("H1,OziExplorer CE Route2 File Version 1.0\nH2,WGS 84\nH3,{0},,{1}", FileName.Substring(FileName.LastIndexOf("\\") + 1), Track.Color.ToArgb());
            int i = 0;
            foreach (TrackPoint pt in Track)
            {
                string f0 = "W";
                string f1 = "RWPT" + i.ToString();
                string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string f4 = pt.FeetAltitude.ToString().Replace(",", ".");
                outputS.WriteLine("{0},{1},{2},{3},{4}", f0, f1, lat, lon, f4);
                i++;
            }

            outputS.Close();
        }

        /// <summary>
        /// Экспорт в фармат GPX
        /// </summary>
        /// <param name="FileName">Путь к файлу</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportGPX(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            XmlDocument xmDoc = new XmlDocument();
            xmDoc.LoadXml(Resources.gpxschema_scheme_file);
            XmlNode root = xmDoc.GetElementsByTagName("trkseg")[0];
            XmlNode name = xmDoc.GetElementsByTagName("name")[0];
            name.InnerText = Track.Name;
            foreach (TrackPoint pt in Track)
            {
                //основной узел точки
                XmlNode nd = xmDoc.CreateNode(XmlNodeType.Element, "trkpt", null);
                //атрибуты основного узла
                XmlAttribute lat = xmDoc.CreateAttribute("lat");
                lat.Value = pt.Coordinates.Latitude.ToString();
                XmlAttribute lon = xmDoc.CreateAttribute("lon");
                lon.Value = pt.Coordinates.Longitude.ToString();
                nd.Attributes.Append(lat);
                nd.Attributes.Append(lon);
                //внутренииe узлы (высота, время)
                XmlNode elev = xmDoc.CreateNode(XmlNodeType.Element, "ele", null);
                elev.InnerText = pt.MetrAltitude.ToString();
                XmlNode tim = xmDoc.CreateNode(XmlNodeType.Element, "time", null);
                //tim.InnerText = (pt.Time + Vars.Options.UTCDifferrent).ToString("yyyy-MM-dd hh:mm:ss");
                tim.InnerText = pt.Time.ToUniversalTime().ToString();
                //добавление внутрених узлов к основному узлу точки
                nd.AppendChild(elev);
                nd.AppendChild(tim);
                //добевление к основному документу
                root.AppendChild(nd);
            }

            StreamWriter sw = new StreamWriter(FileName, false, new UTF8Encoding(false));
            XmlWriter xmw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            root.OwnerDocument.WriteTo(xmw);
            xmw.Close();

            //sw.Close();
        }

        /// <summary>
        /// экспорт списка адресов
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="track">список точек</param>
        /// <param name="callback">действие, выполняемое при сохранении файла</param>
        private static void exportADRS(string fileName, BaseTrack track, Action<string> callback = null)
        {
            List<string> ads = new List<string>();
            GeoCoder coder = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider);
            foreach (TrackPoint pt in track)
            {
                string adr = coder.GetAddress(pt.Coordinates);
                ads.Add(adr);
                if (callback != null)
                    callback.Invoke("Идет сохранение  файла. Обрабатывается адрес: " + adr);
            }

            StreamWriter sw = new StreamWriter(fileName, false, new UTF8Encoding(false));
            foreach (string a in ads)
                sw.WriteLine(a);

            sw.Close();
        }

        /// <summary>
        /// Экспорт в PLT
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportPLT(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            outputS.WriteLine("OziExplorer Track Point File Version 2.0\nWGS 84\nAltitude is in Feet\nReserved 3\n0,1,{2},{0},0\n{1}", Track.Description, Track.Count, Track.Color.ToArgb());
            int i = 0;
            foreach (TrackPoint pt in Track)
            {
                string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string f4 = "0";
                string al = ((int)(pt.FeetAltitude)).ToString();
                DateTime NullDate = pt.Time.ToUniversalTime();
                string tm = (pt.Time.Ticks != 0) ? NullDate.ToOADate().ToString() : "0";
                tm = tm.Replace(Vars.DecimalSeparator, '.');
                string date = NullDate.Date.ToString("ddMMyy");
                string time = NullDate.TimeOfDay.ToString("hhmmss") + "." + NullDate.TimeOfDay.Milliseconds;
                outputS.WriteLine("{0},{1},{2},{3},{4},{5},{6}", lat, lon, f4, al, tm, date, time);
                i++;
            }
            outputS.Close();
        }

        /// <summary>
        /// экспорт в  microsoft word 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="track"></param>
        private static void exportDOC(string fileName, BaseTrack track, Action<string> callback)
        {
            OfficeHelper.WriteTrack(track, fileName, callback);
        }

        /// <summary>
        /// Экспорт в WPT
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportWPT(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            outputS.WriteLine("OziExplorer Waypoint File Version 1.1\nWGS 84\n0,{0},,{1}\nReserved 3", Track.Description, Track.Color.ToArgb());
            int i = 0;
            foreach (TrackPoint pt in Track)
            {
                string f0 = i.ToString(); //номер
                string f1 = pt.Name; //имя точки
                string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.'); //широта
                string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');//долгота
                string f4 = (pt.Time.Ticks != 0) ? pt.Time.AddHours(-4).ToOADate().ToString() : "0"; //дата
                f4 = f4.Replace(Vars.DecimalSeparator, '.');
                string f5 = pt.Icon.ToString(); //иконка
                string f6 = "1"; //статус
                string f7 = "4"; //формат
                string f8 = "0"; //цвет символа
                string f9 = "0"; //цвет фона
                string f10 = pt.Description; //описание точки
                string f11 = "0"; //Положение символа от имени
                string f12 = "0"; //формат отображения
                string f13 = "0"; //дистанция приближения
                string f14 = pt.MetrAltitude.ToString(); //высота в метрах
                string f15 = "6"; //размер шрифта
                string f16 = "0"; //стиль шрифта (0 - обычный, 1 - жирный)
                string f17 = "17"; //размер символа на карте

                outputS.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                                   f0, f1, lat, lon, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14, f15, f16, f17);
                i++;
            }
            outputS.Close();
        }

        /// <summary>
        /// Экспорт в CRD
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportCRD(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            outputS.WriteLine("Coordinates Route File");
            outputS.WriteLine("Version 1.0");
            outputS.WriteLine("{0};{1};{2};{3};{4};{5}",
                Track.Name,
                Track.Description,
                Track.Distance + ", km",
                Track.Time,
                Track.KmphSpeed + ", km/h",
                Track.Count);
            outputS.WriteLine("Reserved 3");

            int i = 0;
            foreach (TrackPoint pt in Track)
            {
                string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string alt = pt.MetrAltitude.ToString().Replace(Vars.DecimalSeparator, '.');
                outputS.WriteLine("{0};{1};{2};{3};{4};{5}", lat, lon, alt, pt.Time.ToUniversalTime(), pt.Name, pt.Description);
                i++;
            }
            outputS.Close();
        }

        /// <summary>
        /// Экспорт в KML
        /// </summary>
        /// <param name="FilePath">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportKML(string FilePath, BaseTrack Track)
        {
            GeoFile gf = new GeoFile();
            gf.Routes.Add(Track);
            Serializer.Serialize(FilePath, gf, FileFormats.KmlFile);
        }

        /// <summary>
        /// Экспорт в OSM
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">Трек для экспорта</param>
        public static void ExportOSM(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Resources.osmxschema_scheme_file);
            int i = -1;
            foreach (TrackPoint tp in Track)
            {
                //основной узел точки
                XmlNode nd = doc.CreateNode(XmlNodeType.Element, "node", null);
                //атрибуты основного узла
                XmlAttribute lat = doc.CreateAttribute("lat");
                lat.Value = tp.Coordinates.Latitude.ToString();
                XmlAttribute lon = doc.CreateAttribute("lon");
                lon.Value = tp.Coordinates.Longitude.ToString();
                XmlAttribute alt = doc.CreateAttribute("alt");
                alt.Value = tp.MetrAltitude.ToString();
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = i.ToString();
                nd.Attributes.Append(id);
                nd.Attributes.Append(lat);
                nd.Attributes.Append(lon);
                nd.Attributes.Append(alt);
                doc.DocumentElement.AppendChild(nd);
                i--;
            }

            //добавление пути (тег way)
            XmlNode way = doc.CreateNode(XmlNodeType.Element, "way", null);
            XmlAttribute id1 = doc.CreateAttribute("id");
            id1.Value = (-Track.Count - 1).ToString();
            way.Attributes.Append(id1);
            XmlAttribute vis = doc.CreateAttribute("visible");
            vis.Value = "true";
            way.Attributes.Append(vis);
            i = -1;
            foreach (TrackPoint pt in Track)
            {
                XmlNode nd = doc.CreateNode(XmlNodeType.Element, "nd", null);
                XmlAttribute re = doc.CreateAttribute("ref");
                re.Value = i.ToString();
                nd.Attributes.Append(re);
                way.AppendChild(nd);
                i--;
            }
            doc.DocumentElement.AppendChild(way);
            StreamWriter sw = new StreamWriter(FileName, false, new UTF8Encoding(false));
            XmlWriter xmw = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            doc.WriteTo(xmw);
            xmw.Close();
            //sw.Close();
        }

        /// <summary>
        /// Экспорт в NMEA
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportNMEA(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            foreach (TrackPoint pt in Track)
            {
                string position = pt.Coordinates.ToString("{lon},{lat}", "ddmm.mmm,H");
                string date;
                string time;
                try
                {
                    date = (pt.Time - Vars.Options.Converter.UTCDifferrent).Date.ToString("ddMMyy");
                    time = (pt.Time - Vars.Options.Converter.UTCDifferrent).TimeOfDay.ToString("hhmmss");
                }
                catch (Exception)
                {
                    date = "000000";
                    time = "000000";
                }
                //$GPRMC,000000.000,V,5955.154,N,03023.668,E,0.00,0.00,010070,,*14
                outputS.WriteLine("$GPRMC,{0},A,{1},0.00,0.00,{2}",
                    time, position, date);
            }
            outputS.Close();
        }

        /// <summary>
        /// Экспорт в CSV
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportCSV(string FileName, BaseTrack Track, bool isYandexFormat = false)
        {
            if (isYandexFormat) {
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
                outputS.WriteLine("\"Широта\";\"Долгота\";\"Описание\";\"Подпись\";\"Номер метки\"");
                int i = 1;
                foreach (TrackPoint pt in Track)
                {
                    string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.');
                    string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');
                    string desc = pt.Description.Replace(";",",");
                    string name = pt.Name.Replace(";",",");
                    outputS.WriteLine("{0};{1};{2};{3};{4}", lat, lon, desc,name,i );
                    i++;
                }
                outputS.Close();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));
                StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
                outputS.WriteLine("No,Latitude,Longitude,Altitude,Date,Time");
                int i = 1;
                foreach (TrackPoint pt in Track)
                {
                    string lat = pt.Coordinates.Latitude.ToString().Replace(Vars.DecimalSeparator, '.');
                    string lon = pt.Coordinates.Longitude.ToString().Replace(Vars.DecimalSeparator, '.');
                    string alt = pt.MetrAltitude.ToString().Replace(Vars.DecimalSeparator, '.');
                    string date = pt.Time.Date.ToString("yyyy/MM/dd").Replace('.', '/');
                    string time = pt.Time.TimeOfDay.ToString();
                    outputS.WriteLine("{0},{1},{2},{3},{4},{5}", i, lat, lon, alt, date, time);
                    i++;
                }
                outputS.Close();
            }
        }

        /// <summary>
        /// Экспорт в TXT
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportTXT(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));
            outputS.WriteLine("Grid	Lat/Lon hddd°mm.mmm'");
            outputS.WriteLine("Datum	WGS 84");
            outputS.WriteLine();
            outputS.WriteLine();
            outputS.WriteLine();
            outputS.WriteLine("Header	Name	Start Time	Elapsed Time	Length	Average Speed	Link");
            outputS.WriteLine();
            outputS.WriteLine("Track\t{0}\t{1}\t{2} km\t{3} km/h", Track.Name, Track[0].Time, Track.Distance, Track.KmphSpeed);
            outputS.WriteLine();
            outputS.WriteLine("Header	Position	Time	Altitude	Depth	Temperature	Leg Length	Leg Time	Leg Speed	Leg Course");
            outputS.WriteLine();
            foreach (TrackPoint pt in Track)
            {
                string position = pt.Coordinates.ToString("{lon} {lat}", "Hdd mm.mmm");
                string alt = pt.MetrAltitude.ToString().Replace(Vars.DecimalSeparator, '.');
                string tm = pt.Time.TimeOfDay.ToString();
                outputS.WriteLine("{0}\t{1}\t{2}\t{3}", pt.Name, position, tm, alt);
            }
            outputS.Close();
        }

        /// <summary>
        /// Экспорт в KMZ
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Track">трек для экспорта</param>
        public static void ExportKMZ(string FileName, BaseTrack Track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            string tmpf = Vars.Options.Converter.TempFolder + "\\" + Guid.NewGuid();
            ExportKML(tmpf + "\\doc.kml", Track);
            FastZip fz = new FastZip();
            fz.CreateZip(FileName, tmpf, false, "-y", "-y");
            Directory.Delete(tmpf, true);
        }
        #endregion

        #region экспорт в ссылки

        /// <summary>
        /// Экспорт как ссылку яндекса
        /// </summary>
        public static string ExportYandex(BaseTrack Track)
        {
            if (Track.Count > 100)
                return ExportYandex(100, Track);

            string lon, lat;
            lon = Track[0].Coordinates.Longitude.ToString("00.00000000").Replace(Vars.DecimalSeparator, '.');
            lat = Track[0].Coordinates.Latitude.ToString("00.00000000").Replace(Vars.DecimalSeparator, '.');
            string res = string.Format("http://maps.yandex.ru/?ll={0}%2C{1}&spn=0.322723%2C0.156041&z=10&l=map&", lon, lat);
            res += string.Format("rl={0}%2C{1}", lon, lat);
            for (int i = 1; i < Track.Count; i++)
            {
                string ndata = "";
                double dlon = Math.Round(Track[i].Coordinates.Longitude - Track[i - 1].Coordinates.Longitude, 8);
                double dlat = Math.Round(Track[i].Coordinates.Latitude - Track[i - 1].Coordinates.Latitude, 8);
                ndata += "~" + dlon.ToString("0.00000000").Replace(Vars.DecimalSeparator, '.') + "%2C" + dlat.ToString("0.00000000").Replace(Vars.DecimalSeparator, '.');
                res += ndata;
            }
            return res;
        }

        /// <summary>
        /// Экспорт в ссылку для карты яндекса с указанием количества точек
        /// </summary>
        /// <param name="count">количество точек в маршруте</param>
        /// <param name="Track">трек для экспорта</param>
        /// <returns></returns>
        public static string ExportYandex(int count, BaseTrack Track)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Количество точек меньше или равно нулю");
            if (count > Track.Count)
                count = Track.Count;
            int step = Track.Count / count;

            string lon, lat;
            lon = Track[0].Coordinates.Longitude.ToString("00.00000000").Replace(Vars.DecimalSeparator, '.');
            lat = Track[0].Coordinates.Latitude.ToString("00.00000000").Replace(Vars.DecimalSeparator, '.');
            string res = string.Format("http://maps.yandex.ru/?ll={0}%2C{1}&spn=0.322723%2C0.156041&z=10&l=map&", lon, lat);
            res += string.Format("rl={0}%2C{1}", lon, lat);
            for (int i = 1; i < Track.Count; i += step)
            {
                string ndata = "";
                int prev = (i - step < 0) ? 1 : (i - step);
                double dlon = Math.Round(Track[i].Coordinates.Longitude - Track[prev].Coordinates.Longitude, 8);
                double dlat = Math.Round(Track[i].Coordinates.Latitude - Track[prev].Coordinates.Latitude, 8);
                ndata += "~" + dlon.ToString("0.00000000").Replace(Vars.DecimalSeparator, '.') + "%2C" + dlat.ToString("#0.00000000").Replace(Vars.DecimalSeparator, '.');
                res += ndata;
            }
            //добавление последней точки
            string ndata1 = "";
            int j = Track.Count - 1;
            int prev1 = (j - step < 0) ? 1 : (j - step);
            double dlon1 = Math.Round(Track[j].Coordinates.Longitude - Track[prev1].Coordinates.Longitude, 8);
            double dlat1 = Math.Round(Track[j].Coordinates.Latitude - Track[prev1].Coordinates.Latitude, 8);
            ndata1 += "~" + dlon1.ToString("0.00000000").Replace(Vars.DecimalSeparator, '.') + "%2C" + dlat1.ToString("#0.00000000").Replace(Vars.DecimalSeparator, '.');
            res += ndata1;
            return res;
        }

        /// <summary>
        /// экспорт как ссылка викимапии
        /// </summary>
        public static string ExportWikimapia(BaseTrack Track)
        {
            if (Track.Count > 100)
                return ExportWikimapia(100, Track);

            //http://wikimapia.org/#lang=ru&lat=55.735374&lon=37.861032&z=14&m=ys&gz=0;378486728;557230014;123596;77576;0;0;478076;235824
            string lon, lat;
            lon = (Track[0].Coordinates.Longitude).ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
            lat = (Track[0].Coordinates.Latitude).ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
            //при формировании ссылки в части, отвечающей за установку курсора, сначала  широта,  потом долгота
            //в части, где записываются точки наоборот - сначала долгота, потом широта
            string res = string.Format("http://wikimapia.org/#lang=ru&lat={1}&lon={0}&z=12&m=ys&gz=0;", lon, lat);
            int latBase, lonBase;
            latBase = (int)(Track.AllLatitudes.Min<double>() * 10000000);
            lonBase = (int)(Track.AllLongitudes.Min<double>() * 10000000);
            res += string.Format("{1};{0}", latBase, lonBase);
            for (int i = 0; i < Track.Count; i++)
            {
                string ndata = "";
                int dlonA = (int)(Track[i].Coordinates.Longitude * 10000000);
                int dlatA = (int)(Track[i].Coordinates.Latitude * 10000000);
                int dlon = dlonA - lonBase;
                int dlat = dlatA - latBase;
                ndata += ";" + dlon.ToString() + ";" + dlat.ToString();
                res += ndata;
            }
            return res;
        }

        /// <summary>
        /// Экспорт в ссылку для викимапии с определнным количествои точек
        /// </summary>
        /// <param name="count">количество точек в маршруте</param>
        /// <param name="Track">маршрут для экспорта</param>
        /// <returns>ссылка на машрурт на картах Wikimapia</returns>
        public static string ExportWikimapia(int count, BaseTrack Track)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Количество точек меньше или равно нулю");
            if (count > Track.Count)
                count = Track.Count;

            int step = Track.Count / count;
            string lon, lat;
            lon = (Track[0].Coordinates.Longitude).ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
            lat = (Track[0].Coordinates.Latitude).ToString("00.000000").Replace(Vars.DecimalSeparator, '.');
            //при формировании ссылки в части, отвечающей за установку курсора, сначала  широта,  потом долгота
            //в части, где записываются точки наоборот - сначала долгота, потом широта
            string res = string.Format("http://wikimapia.org/#lang=ru&lat={1}&lon={0}&z=12&m=ys&gz=0;", lon, lat);

            int latBase, lonBase;
            latBase = (int)(Track.AllLatitudes.Min<double>() * 10000000);
            lonBase = (int)(Track.AllLongitudes.Min<double>() * 10000000);
            res += string.Format("{1};{0};", latBase, lonBase);
            res += Wikimapia.GetPolygon(Track, step);

            return res;
        }

        #endregion

        #endregion

        #endregion

        #region TrackFileFist

        #region загрузка разных форматов маршрутов

        /// <summary>
        /// Загрузка нескольких маршрутов из файла *.rte
        /// </summary>
        /// <param name="FileName"></param>
        private static TrackFileList loadRTE(string FileName)
        {
            //            <тип строки*>,<информация**>
            //            * - R (маршрут), W (путевая точка)
            //            ** - данные в строке: 
            //              для типа R:
            //              <номер маршрута>,<имя маршрута>,<описание маршрута>,<цвет маршрута>
            //              для типа W
            //<номер маршрута>,<номер точки в маршруте>,<имя точки>,<описание точки>,<широта>,<долгота>,<время>,<>,<>,<>,<>,<65535>,<описание>,<0>,<0>,<сходное расстояние>,<высота в футах>
            TrackFileList res = new TrackFileList();

            StreamReader sr = new StreamReader(FileName, new UTF8Encoding(false), true);
            TrackFile ntf = new TrackFile();

            //пропуск строк в начале файла, в которых нет данных
            string str = "";
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                str = sr.ReadLine();
                string[] arr = Regex.Split(str, "w*,w*");

                switch (arr[0])
                {
                    case "R":
                        if (ntf.Count > 0)
                            res.Add(ntf);

                        ntf = new TrackFile
                        {
                            Name = arr[2],
                            Description = arr[3]
                        };
                        if (arr[4] != "0")
                            ntf.Color = Color.FromArgb(int.Parse(arr[4]));
                        break;
                    case "W":
                        TrackPoint np = new TrackPoint(arr[5], arr[6])
                        {
                            Description = arr[13],
                            Name = arr[3],
                            FeetAltitude = Convert.ToDouble(arr[17].Replace('.', Vars.DecimalSeparator)),
                            Time = DateTime.FromOADate(Convert.ToDouble(arr[7].Replace('.', Vars.DecimalSeparator).Trim()))
                        };
                        ntf += np;
                        break;
                }
            }
            if (ntf.Count > 0)
                res.Add(ntf);

            foreach (TrackFile tf in res)
                tf.CalculateAll();
            sr.Close();
            return res;
        }

        /// <summary>
        /// загрузка файла kmz
        /// </summary>
        /// <param name="FilePath"></param>
        private static TrackFileList loadKMZ(string FilePath)
        {
            TrackFileList res = new TrackFileList();
            FastZip fz = new FastZip();
            string tmpFolder = Vars.Options.Converter.TempFolder + "\\" + Guid.NewGuid();
            fz.ExtractZip(FilePath, tmpFolder, "-y");
            res = loadKML(tmpFolder + "\\doc.kml");
            Directory.Delete(tmpFolder, true);

            return res;
        }

        /// <summary>
        /// Загрузка маршрута из файла *.kml
        /// </summary>
        /// <param name="FilePath">имя файла</param>
        private static TrackFileList loadKML(string FilePath)
        {
            TrackFileList res = new TrackFileList();
            GeoFile gf = Serializer.DeserializeGeoFile(FilePath);
            res.AddRange(gf.Routes);
            return res;
        }

        #endregion

        #region экспорт в разные форматы маршрутов

        /// <summary>
        /// Сохранение маршрутов в файл rte
        /// </summary>
        /// <param name="FileName">путь к файлу</param>
        /// <param name="track">трек для экспорта</param>
        private static void exportRTE(string FileName, TrackFileList track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            StreamWriter outputS = new StreamWriter(FileName, false, new UTF8Encoding(false));

            outputS.WriteLine("OziExplorer Route File Version 1.0\nWGS 84\nReserved 1\nReserved 2");
            outputS.WriteLine("R,  0,R0              ,,255");

            int i = 1;
            foreach (BaseTrack tf in track)
            {
                RteHelper.WriteTrackToRTEFile(tf, i, outputS);
                i++;
            }
            outputS.Close();
        }

        /// <summary>
        /// Сохранение маршрутов в файл kml
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        /// <param name="track">трек для экспорта</param>
        private static void exportKML(string FileName, TrackFileList track)
        {
            GeoFile gf = new GeoFile(track);
            Serializer.Serialize(FileName, gf, FileFormats.KmlFile);
        }

        /// <summary>
        /// Экспорт маршрутов в файл kmz
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="track">трек для экспорта</param>
        private static void exportKMZ(string FileName, TrackFileList track)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            string tmpf = Vars.Options.Converter.TempFolder + "\\" + Guid.NewGuid();
            exportKML(tmpf + "\\doc.kml", track);
            FastZip fz = new FastZip();
            fz.CreateZip(FileName, tmpf, false, "-y", "-y");
            Directory.Delete(tmpf, true);
        }

        #endregion

        #endregion

        #region TripRouteFile

        #region загрузка

        /// <summary>
        /// загрузка файла Trr
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns></returns>
        private static TripRouteFile loadTRR(string fileName)
        {
            //формат файла:
            //Заголовок с информацией о версии
            //***
            //информация 
            //***
            //массив с маршрутами по дням
            //***
            //массив с путевыми точками

            StreamReader sr = new StreamReader(fileName, Encoding.UTF8, true);
            string header = sr.ReadLine();
            string version = Regex.Split(header, "w* w*")[4];

            string allData = sr.ReadToEnd();
            sr.Close();

            switch (version)
            {
                case "1.0":
                {
                    string[] parts = Regex.Split(allData, "w*" + TrrHelper.separatorMain + "w*");

                    string info = parts[1];
                    string routes = parts[2];
                    string wpts = parts[3];

                    TripRouteFile res = TrrHelper.GetInformation(info);
                    res.DaysRoutes = TrrHelper.GetRoutes10(routes);
                    res.Waypoints = TrrHelper.GetWaypoints10(wpts);
                    res.FilePath = fileName;
                    return res;
                }
                case "2.0":
                {
                    string[] parts = Regex.Split(allData, "w*" + TrrHelper.separatorMain + "w*");

                    string info = parts[1];
                    string routes = parts[2];
                    string wpts = parts[3];

                    TripRouteFile res = TrrHelper.GetInformation(info);
                    res.DaysRoutes = TrrHelper.GetRoutes(routes);
                    res.Waypoints = TrrHelper.GetWaypoints(wpts);
                    res.FilePath = fileName;
                    return res;
                }
                default:
                    throw new ApplicationException("Версия файла " + version + " не поддерживается в этой версии программы. Обновите Track Converter!");
            }
        }

        #endregion

        #region экспорт

        /// <summary>
        /// экспорт в формат туристического маршрта TRR
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="tripIns">маршрут</param>
        private static void exportTrr(string fileName, BaseTrack tripIns)
        {
            TripRouteFile trip;
            if (tripIns is TrackFile)
                trip = ((TrackFile)tripIns).ToTripRoute();
            else
                trip = tripIns as TripRouteFile;

            //формат файла:
            //Заголовок с информацией о версии
            //***
            //информация 
            //***
            //массив с маршрутами по дням
            //***
            //массив с путевыми точками

            trip.CalculateAll(); //вычисление всех параметров путешествия
            string header = TrrHelper.header; //загловок файла с версией
            string information = TrrHelper.GetInformation(trip); //информация о путешествии
            string wpts = TrrHelper.GetWaypoints(trip); //путевые точки
            string rts = TrrHelper.GetRoutes(trip); //маршруты по дням

            StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8);
            sw.WriteLine(header);
            sw.WriteLine(TrrHelper.separatorMain);
            sw.WriteLine(information);
            sw.WriteLine(TrrHelper.separatorMain);
            sw.WriteLine(rts);
            sw.WriteLine(TrrHelper.separatorMain);
            sw.WriteLine(wpts);
            sw.Close();
        }

        #endregion

        #endregion

        #region GeoFile

        #region загрузка разных форматов маршрутов

        /// <summary>
        /// загрузка файла kmz
        /// </summary>
        /// <param name="FilePath">имя файла</param>
        private static GeoFile loadKmz(string FilePath)
        {
            FastZip fz = new FastZip();
            string tmpFolder = Vars.Options.Converter.TempFolder + Guid.NewGuid();
            fz.ExtractZip(FilePath, tmpFolder, "-y");
            GeoFile res = loadKml(tmpFolder + "\\doc.kml");
            Directory.Delete(tmpFolder, true);
            return res;
        }

        /// <summary>
        /// загрузк путевых точек и маршрутов из файла KML
        /// </summary>
        /// <param name="FileName">имя файла</param>
        private static GeoFile loadKml(string FileName)
        {
            GeoFile res = new GeoFile();
            XmlDocument xml = new XmlDocument();
            xml.Load(FileName);
            res = KmlHelper.CheckFolder(xml.DocumentElement);
            return res;
        }

        #endregion

        #region экспорт в разные форматы маршрутов

        /// <summary>
        /// сохранение путевых точек и маршрутов в файл KML
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="file">объект GeoFile для экспорта</param>
        public static void ExportKml(string FileName, GeoFile file)
        {
            //ПОДГОТОВКА ДОКУМЕНТА
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            XmlWriter xw = XmlWriter.Create(FileName, new XmlWriterSettings() { Indent = true, CloseOutput = true, Encoding = new UTF8Encoding(false) });
            xw.WriteStartDocument();

            XmlDocument doc = new XmlDocument();
            XmlNode kml = doc.CreateNode(XmlNodeType.Element, "kml", null);

            XmlNode document = doc.CreateNode(XmlNodeType.Element, "Document", null);
            kml.AppendChild(document);

            //группа 
            XmlNode folder = doc.CreateNode(XmlNodeType.Element, "Folder", null);
            document.AppendChild(folder);

            //имя группы 
            XmlNode nm = doc.CreateNode(XmlNodeType.Element, "name", null);
            nm.InnerText = Path.GetFileNameWithoutExtension(FileName);
            folder.AppendChild(nm);

            //ЗАПИСЬ МАРШРУТОВ
            foreach (TrackFile tf in file.Routes)
                folder = KmlHelper.WriteTrackToKMLNode(doc, folder, tf);

            //ЗАПИСЬ ТОЧЕК
            foreach (TrackPoint tt in file.Waypoints)
                folder = KmlHelper.WritePlacemarkToKMLNode(doc, folder, tt);

            //ЗАКРЫТИЕ ФАЙЛА
            kml.WriteTo(xw);
            xw.Close();
        }

        /// <summary>
        /// экспорт в файл kmz
        /// </summary>
        /// <param name="FileName">имя файла</param>
        /// <param name="file">объект для сериализации</param>
        private static void exportKmz(string FileName, GeoFile file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            string tmpf = Vars.Options.Converter.TempFolder + "\\" + Guid.NewGuid();
            ExportKml(tmpf + "\\doc.kml", file);
            FastZip fz = new FastZip();
            fz.CreateZip(FileName, tmpf, false, "-y", "-y");
            Directory.Delete(tmpf, true);
        }

        #endregion

        #endregion
    }
}