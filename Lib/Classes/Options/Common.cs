using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

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
            this.IsLoadETOPOOnStart = false;
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
        /// если истина, от при старте программы будет загружаться БД ETOPO
        /// </summary>
        public bool IsLoadETOPOOnStart { get; set; }

        /// <summary>
        ///  послденяя набранная команда в консоли
        /// </summary>
        public string LastConsoleCommand { get; set; }

        /// <summary>
        /// адрес сайта программы
        /// </summary>
        public string SiteAddress { get { return "http://velomapa.ru"; } }

        /// <summary>
        /// GUID экземпляра программы
        /// </summary>
        public string ApplicationGuid
        {
            get
            {
#if (DEBUG)
                return "debug";
#else
                RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\TrackConverter");
                object guido = key.GetValue("Guid");
                string guid;
                if (guido == null)
                {
                    guid = Guid.NewGuid().ToString();
                    key.SetValue("Guid", guid);
                }
                else
                    guid = (string)guido;
                return guid;
#endif
            }
        }
    }
}
