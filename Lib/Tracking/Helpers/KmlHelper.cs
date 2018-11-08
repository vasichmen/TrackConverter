using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// вспомогательные методв при работе с форматом KML
    /// </summary>
    internal static class KmlHelper
    {
        /// <summary>
        /// разбор XML документа и выбор маршрутов и точек
        /// </summary>
        /// <param name="folder">начальные элемент  XML</param>
        /// <returns></returns>
        public static GeoFile CheckFolder(XmlNode folder)
        {
            //если в папке есть TimeSpan => в этой папке лежит один маршрут по частям
            //и части маршрута находятся в Placemark->LineString
            //
            //если в папке нет TimeSpan => в папке лежат отдельные элементы Placemark (маршруты, точки)
            //если у Placemark есть Point, то это точка, если у Placemark есть LineString, то это маршрут

            XmlNode timespan = XmlHelper.GetChild(folder, "TimeSpan", false);
            bool isFolderOneRoute = timespan != null;

            if (isFolderOneRoute)
            #region ЕСЛИ В ПАПКЕ ТОЛЬКО ОДИН МАРШРУТ ПО ЧАСТЯМ
            {
                string routePoints = "";
                foreach (XmlNode rtn in folder)
                    if (rtn.LocalName.ToLower() == "placemark")
                        routePoints += XmlHelper.GetChild(rtn, "coordinates", true).InnerText;
                routePoints = routePoints.TrimEnd(new char[] { ' ' });
                TrackFile oneRoute = parseLineString(routePoints);

                //#region получение времени маршрута
                //try
                //{
                //    string begin = XmlHelper.GetChild(timespan, "begin", false).InnerText;
                //    string end = XmlHelper.GetChild(timespan, "end", false).InnerText;
                //    TimeSpan rest = TimeSpan.Zero;
                //    DateTime bg = DateTime.Parse(begin);
                //    DateTime en = DateTime.Parse(end);
                //    rest = en - bg;
                //    oneRoute.Time = rest;
                //}
                //catch (NullReferenceException) { oneRoute.Time = TimeSpan.Zero; }
                //#endregion

                //название, описание
                try { oneRoute.Name = XmlHelper.GetChild(folder, "name", false).InnerText; }
                catch (NullReferenceException) { oneRoute.Name = "noname"; }
                try { oneRoute.Description = XmlHelper.GetChild(folder, "description", false).InnerText; }
                catch (NullReferenceException) { oneRoute.Description = ""; }

                oneRoute.CalculateAll();
                GeoFile res = new GeoFile(new TrackFileList(oneRoute));
                return res;
            }
            #endregion
            else
            #region ЕСЛИ В ПАПКЕ СМЕШАННОЕ СОДЕРЖИМОЕ (папки, точки, маршруты)
            {
                GeoFile res = new GeoFile();
                foreach (XmlNode node in folder)
                {
                    #region если это папка
                    if (node.LocalName.ToLower() == "folder" || node.LocalName.ToLower() == "document") //если это папка, то запускаем заново метод
                    {
                        GeoFile gf = CheckFolder(node);
                        res.Routes.Add(gf.Routes);
                        res.Waypoints.Add(gf.Waypoints);
                    }
                    #endregion

                    #region если это метка
                    //если метка, то смотрим на ее содержимое (маршрут или точка)
                    if (node.LocalName.ToLower() == "placemark")
                    {
                        bool isroute = false;
                        XmlNode str = XmlHelper.GetChild(node, "Point", false);
                        isroute = str == null;//если не нашлось тега Point => это маршрут
                        if (isroute) //если это маршрут
                        {
                            #region если метка - маршрут
                            //выделение точек маршрута
                            string routePoints = XmlHelper.GetChild(node, "coordinates", true).InnerText;
                            TrackFile tf = parseLineString(routePoints);

                            //время всегда ноль
                            //tf.Time = TimeSpan.Zero;

                            //название, описание
                            try { tf.Name = XmlHelper.GetChild(node, "name", false).InnerText; }
                            catch (NullReferenceException) { tf.Name = "noname"; }
                            try { tf.Description = XmlHelper.GetChild(node, "description", false).InnerText; }
                            catch (NullReferenceException) { tf.Description = ""; }

                            //оформление 
                            string color = XmlHelper.GetChild(node, "color", true).InnerText;
                            try { tf.Color = ColorTranslator.FromHtml("#" + color); }
                            catch (Exception) { tf.Color = Vars.Options.Converter.GetColor(); }

                            tf.CalculateAll();
                            res.Routes.Add(tf);
                            #endregion
                        }
                        else //если не маршрут, то точка
                        {
                            #region если метка - точка
                            string coords = XmlHelper.GetChild(node, "coordinates", true).InnerText;
                            TrackPoint nv = parsePoint(coords);
                            //название, описание
                            try { nv.Name = XmlHelper.GetChild(node, "name", false).InnerText; }
                            catch (NullReferenceException) { nv.Name = "noname"; }
                            try { nv.Description = XmlHelper.GetChild(node, "description", false).InnerText; }
                            catch (NullReferenceException) { nv.Description = ""; }
                            res.Waypoints.Add(nv);
                            #endregion
                        }

                    }
                    #endregion
                }
                return res;
            }
            #endregion
        }

        /// <summary>
        /// разбор ответа сайта Wikimapia с объектами в хаданной области
        /// </summary>
        /// <param name="kml">текстовый ответ в формате KML</param>
        /// <param name="perimeter">минимальный периметр объекта в метрах</param>
        /// <returns></returns>
        internal static List<VectorMapLayerObject> ParseWikimapiaObjectsAnswer(string kml, double perimeter)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(kml);
            XmlNode folder = doc.DocumentElement["Document"]["Folder"];


            List<VectorMapLayerObject> res = new List<VectorMapLayerObject>();
            foreach (XmlNode placemark in folder.ChildNodes)
            {
                //ID
                //string wid = placemark.Attributes["id"].InnerText.TrimStart(new[] { 'w', 'm' });
                string wid = placemark.Attributes["id"].InnerText.Substring(2); //удаление первых символов wm

                int id = int.Parse(wid);

                //LINK
                string link = "http://wikimapia.org/" + id;

                //NAME
                string raw_desc = placemark["description"].InnerText;
                raw_desc = raw_desc.Replace("![CDATA[", "").Replace("]]", "").Replace("View or update this place information at Wikimapia.", "").Replace("\n", "");
                string name = Regex.Replace(raw_desc, "<[^>]+>", string.Empty).Trim().Replace("&quot;", "\"").Replace("&amp;quot;", "\"").Replace("&#039;", "'");

                //Pixels
                //string maxLodPixels = placemark["Region"]["Lod"]["maxLodPixels"].InnerText;
                // string minLodPixels = placemark["Region"]["Lod"]["minLodPixels"].InnerText;

                //GEOMETRY
                string linestring = placemark["MultiGeometry"]["LineString"]["coordinates"].InnerText;
                //string linestring = placemark.SelectSingleNode(@"MultiGeometry/LineString/coordinates").InnerText;


                linestring = linestring.Replace("\n", " ").Trim().Trim();

                GMapPolygon geometry = parseLineString(linestring,name);
                bool perim = checkPerimeter(geometry, perimeter);

                //если периметр объекта больше заданного, то добавляем в результат
                if (perim)
                    res.Add(new VectorMapLayerObject(geometry) {Name=name, ID = id, LayerProvider = MapLayerProviders.Wikimapia, Invisible = false });
            }
            return res;
        }


        /// <summary>
        /// расчёт периметра полигона в метрах
        /// </summary>
        /// <param name="geometry">полигон</param>
        /// <returns></returns>
        private static double getPerimeter(TrackFile geometry)
        {
            double res = 0;
            for (int i = 1; i < geometry.Count; i++)
                res += Vars.CurrentGeosystem.CalculateDistance(geometry[i - 1], geometry[i]);
            return res;
        }

        /// <summary>
        /// возвращает истину, если периметр объекта geometry больше или равен minimal
        /// </summary>
        /// <param name="geometry">объект</param>
        /// <param name="minimal">минимальный периметр в метрах</param>
        /// <returns></returns>
        private static bool checkPerimeter(GMapPolygon geometry, double minimal)
        {
            double res = 0;
            for (int i = 1; i < geometry.Points.Count; i++)
            {
                res += Vars.CurrentGeosystem.CalculateDistance(geometry.Points[i - 1], geometry.Points[i]);
                if (minimal <= res)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// получить из строки только координаты без высот
        /// </summary>
        /// <param name="lineString">строка с координатами</param>
        /// <param name="name">название нового полигона</param>
        /// <returns></returns>
        private static GMapPolygon parseLineString(string lineString, string name)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            //переменная lineString содержит последовательность координат в формате:
            //долгота1,широта1,высота1 долгота2,широта2,высота2
            //разбиваем на отдельные точки
            string[] pts = lineString.Trim().Split(' ');
            foreach (string pt in pts)
            {
                string[] arr = pt.Split(',');

                string lo = arr[0].ToString().Replace('.', Vars.DecimalSeparator).Trim();
                string la = arr[1].ToString().Replace('.', Vars.DecimalSeparator).Trim();
                double lon = double.Parse(lo);
                double lat = double.Parse(la);
                PointLatLng point = new PointLatLng(lat, lon);
                points.Add(point);
            }
            return new GMapPolygon(points,name);
        }

        /// <summary>
        /// распознает точки из строки координат с высотами
        /// </summary>
        /// <param name="lineString">строка координат, координаты разделены пробелами</param>
        /// <returns></returns>
        private static TrackFile parseLineString(string lineString)
        {
            TrackFile res = new TrackFile();
            //переменная lineString содержит последовательность координат в формате:
            //долгота1,широта1,высота1 долгота2,широта2,высота2
            //разбиваем на отдельные точки
            string[] pts = lineString.Trim().Split(' ');
            foreach (string pt in pts)
                res += parsePoint(pt);
            return res;
        }

        /// <summary>
        /// перевод точки из строкового представления.
        /// </summary>
        /// <param name="pointString">строка, содержаящая долготу, широту, высоту</param>
        /// <returns></returns>
        private static TrackPoint parsePoint(string pointString)
        {
            string[] arr = pointString.Split(',');

            string lo = arr[0].ToString().Replace('.', Vars.DecimalSeparator).Trim();
            string la = arr[1].ToString().Replace('.', Vars.DecimalSeparator).Trim();
            double lon = double.Parse(lo);
            double lat = double.Parse(la);
            TrackPoint nv = new TrackPoint(lat, lon);
            if (arr[2] != null)
                if (arr[2] == "0")
                    nv.MetrAltitude = 0;
                else
                    nv.MetrAltitude = double.Parse(arr[2].ToString().Replace('.', Vars.DecimalSeparator));
            return nv;
        }


        /// <summary>
        /// запись трека в указанный xml узел 
        /// </summary>
        /// <param name="rootFold">узел, куда надо записать трек</param>
        /// <param name="tf">Трек для записи</param>
        /// <param name="parentDoc">родительский документ, от которого создаются элементы</param>
        /// <returns></returns>
        public static XmlNode WriteTrackToKMLNode(XmlDocument parentDoc, XmlNode rootFold, TrackFile tf)
        {
            XmlNode root = parentDoc.CreateNode(XmlNodeType.Element, "Folder", null);

            //параметры отображения и имя маршрута
            XmlNode name = parentDoc.CreateNode(XmlNodeType.Element, "name", null);
            name.InnerText = tf.Name;

            XmlNode style = parentDoc.CreateNode(XmlNodeType.Element, "Style", null);

            style.InnerXml = string.Format(@"<ListStyle><listItemType>check</listItemType><bgColor>00ffffff</bgColor></ListStyle>");

            root.AppendChild(name);
            root.AppendChild(style);


            //маршрут
            XmlNode placemark = parentDoc.CreateNode(XmlNodeType.Element, "Placemark", null);

            //color
            XmlNode styleL = parentDoc.CreateNode(XmlNodeType.Element, "Style", null);
            styleL.InnerXml = string.Format(@"<LineStyle><color>FF{0}</color><width>2</width></LineStyle>", ColorTranslator.ToHtml(tf.Color).TrimStart('#').ToLower());
            placemark.AppendChild(styleL);

            // еще имя маршрута
            XmlNode pmnm = parentDoc.CreateNode(XmlNodeType.Element, "name", null);
            pmnm.InnerText = tf.Name;
            placemark.AppendChild(pmnm);

            //описание
            XmlNode pmdes = parentDoc.CreateNode(XmlNodeType.Element, "description", null);
            pmdes.InnerText = tf.Description;
            placemark.AppendChild(pmdes);

            //координаты
            string cords = "";
            foreach (TrackPoint pt in tf)
                cords += pt.Coordinates.Longitude.ToString("00.0000000000000").Replace(Vars.DecimalSeparator, '.').Trim() + ","
                    + pt.Coordinates.Latitude.ToString("00.0000000000000").Replace(Vars.DecimalSeparator, '.').Trim() + ","
                    + pt.MetrAltitude.ToString("00.000").Replace(Vars.DecimalSeparator, '.').Trim() + " ";

            //LineString
            XmlNode pmlnstr = parentDoc.CreateNode(XmlNodeType.Element, "LineString", null);
            pmlnstr.InnerXml = "<extrude>1</extrude>";

            //coordinates
            XmlNode pmcrd = parentDoc.CreateNode(XmlNodeType.Element, "coordinates", null);
            pmcrd.InnerText = cords;
            pmlnstr.AppendChild(pmcrd);

            placemark.AppendChild(pmlnstr);


            root.AppendChild(placemark);
            rootFold.AppendChild(root);
            return rootFold;
        }

        /// <summary>
        /// запись точки в узел kml файла
        /// </summary>
        /// <param name="parentDoc">родительский документ, от которого создаются элементы</param>
        /// <param name="rootFold">узел, куда надо записать трек</param>
        /// <param name="tt">точка, которую необходимо записать</param>
        /// <returns></returns>
        public static XmlNode WritePlacemarkToKMLNode(XmlDocument parentDoc, XmlNode rootFold, TrackPoint tt)
        {
            XmlNode placemark = parentDoc.CreateNode(XmlNodeType.Element, "Placemark", null);

            //имя
            XmlNode pnm = parentDoc.CreateNode(XmlNodeType.Element, "name", null);
            pnm.InnerText = tt.Name;
            placemark.AppendChild(pnm);

            //описание
            XmlNode pdesc = parentDoc.CreateNode(XmlNodeType.Element, "description", null);
            pdesc.InnerText = tt.Description;
            placemark.AppendChild(pdesc);

            //точка
            XmlNode ppoint = parentDoc.CreateNode(XmlNodeType.Element, "Point", null);
            string cords = tt.Coordinates.Longitude.ToString("00.0000000000000").Replace(Vars.DecimalSeparator, '.').Trim() + ","
                + tt.Coordinates.Latitude.ToString("00.0000000000000").Replace(Vars.DecimalSeparator, '.').Trim() + ","
                + tt.MetrAltitude.ToString("00.000").Replace(Vars.DecimalSeparator, '.').Trim() + " ";
            ppoint.InnerXml = string.Format(@"<extrude>1</extrude><coordinates>{0}</coordinates>", cords);
            placemark.AppendChild(ppoint);

            rootFold.AppendChild(placemark);
            return rootFold;
        }
    }
}
