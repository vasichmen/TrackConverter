using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;

namespace TrackConverter.Lib.Tracking {

    /// <summary>
    /// файл геоинформации (представление kml )
    /// содержит маршруты, путевые точки
    /// </summary>
    public class GeoFile {


        /// <summary>
        /// создает новый экземпляр с готовым списком маршрутов
        /// </summary>
        /// <param name="routes">список маршрутов</param>
        public GeoFile( TrackFileList routes )
            : this( routes, null ) { }

        /// <summary>
        /// создает пустой экземпляр
        /// </summary>
        public GeoFile() {
            Routes = new TrackFileList();
            Waypoints = new TrackFile();
        }

        /// <summary>
        /// создает новый экземпляр с готовым списком маршрутов и путевых точек
        /// </summary>
        /// <param name="routes">маршруты</param>
        /// <param name="waypoints">путевые точки</param>
        public GeoFile( TrackFileList routes, IEnumerable<TrackPoint> waypoints )
            : this() {
            if ( routes != null )
                this.Routes = routes;

            if ( waypoints != null )
                this.Waypoints.Add( waypoints );
        }

        /// <summary>
        /// создает экземпляр с готовыми путевыми точками
        /// </summary>
        /// <param name="waypoints">путевые точки</param>
        public GeoFile( List<TrackPoint> waypoints )
            : this( null, waypoints ) { }



        /// <summary>
        /// список путевых точек
        /// </summary>
        public TrackFile Waypoints { get; set; }

        /// <summary>
        /// список маршрутов
        /// </summary>
        public TrackFileList Routes { get; set; }

    }
}
