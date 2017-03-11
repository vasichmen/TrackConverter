using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// вспомогательные методв при работе с форматом KML
    /// </summary>
    static class KmlHelper
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
                TrackFile oneRoute = ParseLineString(routePoints);

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
                            TrackFile tf = ParseLineString(routePoints);

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
                            TrackPoint nv = ParsePoint(coords);
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
        /// распознает точки из строки координат с высотами
        /// </summary>
        /// <param name="lineString">строка координат, координаты разделены пробелами</param>
        /// <returns></returns>
        private static TrackFile ParseLineString(string lineString)
        {
            TrackFile res = new TrackFile();
            //переменная lineString содержит последовательность координат в формате:
            //долгота1,широта1,высота1 долгота2,широта2,высота2
            //разбиваем на отдельные точки
            string[] pts = Regex.Split(lineString.Trim(), "w* w*");
            foreach (string pt in pts)
                res += ParsePoint(pt);
            return res;
        }

        /// <summary>
        /// перевод точки из строкового представления.
        /// </summary>
        /// <param name="pointString">строка, содержаящая долготу, широту, высоту</param>
        /// <returns></returns>
        private static TrackPoint ParsePoint(string pointString)
        {
            MatchCollection arr = Regex.Matches(pointString, @"\d+\:\d+|\d+[.,]\d+");
            string al = "0";
            string lo = arr[0].ToString().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]).Trim();
            string la = arr[1].ToString().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]).Trim();
            if (arr.Count == 3)
                al = arr[2].ToString().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]).Trim();
            TrackPoint nv = new TrackPoint(la, lo)
            {
                MetrAltitude = Convert.ToDouble(al) //высота в метрах
            };
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
                cords += pt.Coordinates.Longitude.TotalDegrees.ToString("00.0000000000000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + ","
                    + pt.Coordinates.Latitude.TotalDegrees.ToString("00.0000000000000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + ","
                    + pt.MetrAltitude.ToString("00.000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + " ";

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
            string cords = tt.Coordinates.Longitude.TotalDegrees.ToString("00.0000000000000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + ","
                + tt.Coordinates.Latitude.TotalDegrees.ToString("00.0000000000000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + ","
                + tt.MetrAltitude.ToString("00.000").Replace(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0], '.').Trim() + " ";
            ppoint.InnerXml = string.Format(@"<extrude>1</extrude><coordinates>{0}</coordinates>", cords);
            placemark.AppendChild(ppoint);

            rootFold.AppendChild(placemark);
            return rootFold;
        }
    }
}
