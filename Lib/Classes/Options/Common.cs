using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// общие настройки программы (последние файлы, папки)
    /// </summary>
    public class Common
    {
        /// <summary>
        /// создает новый экземпляр общих настроек
        /// </summary>
        public Common()
        {
            this.IsExtension = true;
            this.IsLoadDir = true;
            this.IsSaveDir = true;
            this.IsLoadETOPO2OnStart = false;
            this.LastConsoleCommand = "help";
        }

        /// <summary>
        /// папка, куда последний раз сохраняли файл
        /// </summary>
        public string LastFileSaveDirectory { get; set; }

        /// <summary>
        /// папка,откуда последний раз загружали файл
        /// </summary>
        public string LastFileLoadDirectory { get; set; }

        /// <summary>
        /// номер расширения в фильтре файлов при сохранении одно трека
        /// </summary>
        public int LastSaveExtensionNumberSaveOneTrack { get; set; }

        /// <summary>
        /// номер расширения в фильтре файлов при сохранении нескольких треков в один файл
        /// </summary>
        public int LastSaveExtensionNumberSaveAllInOne { get; set; }

        /// <summary>
        /// последнее расширение при сохранении нескольких маршрутов в разные файлы
        /// </summary>
        public string LastSaveSeparateExtension { get; set; }

        /// <summary>
        /// сохранять последнее расширение
        /// </summary>
        public bool IsExtension { get; set; }

        /// <summary>
        /// сохранять последнюю папку сохранения файла
        /// </summary>
        public bool IsSaveDir { get; set; }

        /// <summary>
        /// сохранять последнюю папку открытия файла
        /// </summary>
        public bool IsLoadDir { get; set; }

        /// <summary>
        /// если истина, от при старте программы будет загружаться БД ETOPO2
        /// </summary>
        public bool IsLoadETOPO2OnStart { get; set; }

        /// <summary>
        ///  послденяя набранная команда в консоли
        /// </summary>
        public string LastConsoleCommand { get; set; }
    }
}
