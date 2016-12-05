using TrackConverter.Res.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Classes.Options {
    /// <summary>
    /// настройки конвертера
    /// </summary>
    public class Converter {

        /// <summary>
        /// список уже используемых цветов
        /// </summary>
        private List<Color> usedColors = new List<Color>();


        /// <summary>
        /// создает новый экземпляр с настройками по умолчанию
        /// </summary>
        public Converter() {
            this.DistanceMethodType = DistanceMethodType.ModGaverSin;
            this.NorthPoleLatitude = 85.90000;
            this.NorthPoleLongitude = -147.00000;
            this.MaxRecentFiles = 5;
            this.IsFormMapShow = false;
            this.TempFolder = Application.StartupPath + Resources.temp_directory;
            this.IsApproximateAltitudes = false;
            this.ApproximateAmount = 60;
            this.MinimumRiseInterval = 2000;
            this.WinSize = new Size(300,300);
        }

        /// <summary>
        /// разница с универсальным временем
        /// </summary>
        public TimeSpan UTCDifferrent { get { return TimeZone.CurrentTimeZone.GetUtcOffset( DateTime.Now ); } }

        /// <summary>
        /// папка временного хранения файлов
        /// </summary>
        public string TempFolder { get; set; }

        /// <summary>
        /// Если истина, то при закрытии окна было открыто окно карты
        /// </summary>
        public bool IsFormMapShow { get; set; }

        /// <summary>
        /// список последних открытых файлов
        /// </summary>
        public List<string> RecentFiles { get; set; }

        /// <summary>
        /// максимальный размер массива послдених открытых файлов
        /// </summary>
        public int MaxRecentFiles { get; set; }

        /// <summary>
        /// координата широты северного магнитного полюса
        /// </summary>
        public double NorthPoleLatitude { get; set; }

        /// <summary>
        /// координата долготы серверного магнитного полюса
        /// </summary>
        public double NorthPoleLongitude { get; set; }

        /// <summary>
        /// Тип метода для расчета расстояний
        /// </summary>
        public DistanceMethodType DistanceMethodType { get; set; }

        /// <summary>
        /// степень аппроксимации высот
        /// </summary>
        public decimal ApproximateAmount { get; set; }

        /// <summary>
        /// использовать аппроксимацию высот
        /// </summary>
        public bool IsApproximateAltitudes { get; set; }

        /// <summary>
        /// Список файлов, загруженных при выходе из программы
        /// </summary>
        public List<string> LastLoadedTracks { get; set; }


        /// <summary>
        /// ассоциируемые с программой расширения файлов
        /// </summary>
        public List<KeyValuePair<string, string>> Extensions {
            get {
                return new List<KeyValuePair<string, string>>() {  
                    new KeyValuePair<string,string>(".rt2", "Маршрут OziExplorer "),  
                    new KeyValuePair<string,string>(".crd", "Файл координат "),  
                    new KeyValuePair<string,string>(".plt", "Трек OziExplorer "),  
                    new KeyValuePair<string,string>(".wpt", "Путевые точки OziExplorer "),  
                    new KeyValuePair<string,string>(".kml", "Файл Google Earth "),  
                    new KeyValuePair<string,string>(".rte", "Файл нескольких маршрутов OziExplorer "),  
                    new KeyValuePair<string,string>(".gpx", "Файл GPS координат "),  
                    new KeyValuePair<string,string>(".kmz", "Архивированный файл Google Earth "),  
                    new KeyValuePair<string,string>(".osm", "Файл OpenStreetMaps "),  
                    new KeyValuePair<string,string>(".nmea", "Файл обмена навигационными данными "),  
                };
            }
            set { }
        }

        /// <summary>
        /// минимальная длина горки в метрах при поиске точек экстремума
        /// </summary>
        public double MinimumRiseInterval { get; set; }

        /// <summary>
        /// Размеры окна
        /// </summary>
        public Size WinSize { get; set; }

        /// <summary>
        /// добавление последнего открытого файла
        /// </summary>
        /// <param name="FileName"></param>
        public void AddRecentFile( string FileName ) {
            if ( RecentFiles == null ) //создание нового списка, если необходимо
                RecentFiles = new List<string>();

            if ( RecentFiles.Contains( FileName ) )//если такой файл уже есть, то удаляем и добавляем в начало
                RecentFiles.Remove( FileName );

            RecentFiles.Insert( 0, FileName ); //добавление файла в начало

            if ( RecentFiles.Count > this.MaxRecentFiles ) // если количество выходит за пределы, то образаем массив
            {
                RecentFiles.RemoveRange( this.MaxRecentFiles - 1, RecentFiles.Count - this.MaxRecentFiles );
            }
        }

        /// <summary>
        /// получить неиспользуемый цвет
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            DateTime start = DateTime.Now;
        getnc:
            Random rn = new Random();
            Color nc = Color.FromArgb(255, rn.Next(0, 255), rn.Next(0, 128), rn.Next(0, 128));

            if (DateTime.Now - start > TimeSpan.FromSeconds(2))
                goto ret;
            if (usedColors.Contains(nc))
                goto getnc;

        ret:
            usedColors.Add(nc);
            return nc;
        }






        
    }
}
