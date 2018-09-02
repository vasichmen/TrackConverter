using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Classes.Options
{
    /// <summary>
    /// Настройки программы
    /// </summary>
    [Serializable]
    public class Options
    {
        /// <summary>
        /// создает новый экземпляр
        /// </summary>
        public Options()
        {
            this.Map = new Map();
            this.Converter = new Converter();
            this.Common = new Common();
            this.DataSources = new DataSources();
            this.Services = new Services();
            this.Graphs = new Graphs();
            this.Container = new Container();
            this.Format = OptionsFormat.XML;
        }

        /// <summary>
        /// настройки карты
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// настройки конвертера
        /// </summary>
        public Converter Converter { get; set; }

        /// <summary>
        /// сервисы
        /// </summary>
        public Services Services { get; set; }

        /// <summary>
        /// Общие
        /// </summary>
        public Common Common { get; set; }

        /// <summary>
        /// Источники данных
        /// </summary>
        public DataSources DataSources { get; set; }

        /// <summary>
        /// настройки построения графиков и рпрофиля высот
        /// </summary>
        public Graphs Graphs { get; set; }

        /// <summary>
        /// окно-контейнер
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// формат сохранения настроек
        /// </summary>
        public OptionsFormat Format { get; set; }

        /// <summary>
        /// адрес файла
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// сохранение настроек в файл
        /// </summary>
        /// <param name="Directory">адрес папки, куда сохранить файл</param>
        public void Save(string Directory)
        {
            switch (this.Format)
            {
                case OptionsFormat.JSON:
                    File.Delete(Application.StartupPath + Resources.options_folder + "\\options.xml");
                    JSONSerialize(Directory + "options.json");
                    break;
                case OptionsFormat.XML:
                    File.Delete(Application.StartupPath + Resources.options_folder + "\\options.json");
                    XMLSerialize(Directory + "options.xml");
                    break;
            }
        }

        /// <summary>
        /// Загрузка файла настроек
        /// </summary>
        /// <param name="Directory">адрес файла</param>
        /// <returns></returns>
        public static Options Load(string Directory)
        {
            string[] arr = System.IO.Directory.GetFiles(Directory);
            List<string> files = new List<string>(arr);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                switch (name)
                {
                    case "options.xml":
                        return XMLDeserialize(file);
                    case "options.json":
                        return JSONDeserialize(file);
                    default:
                        continue;
                }
            }

            return new Options();

        }

        #region сериализация

        /// <summary>
        /// сериализация в XML
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private void XMLSerialize(string FilePath)
        {
            File.Delete(FilePath);
            XmlSerializer se = new XmlSerializer(typeof(Options));
            FileStream fs = new FileStream(FilePath, FileMode.Create);
            se.Serialize(fs, this);
            fs.Close();
        }

        /// <summary>
        /// Сериализация в JSON
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        private void JSONSerialize(string FilePath)
        {
            File.Delete(FilePath);
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Options));
            Directory.CreateDirectory(Resources.temp_directory);
            FileStream fs = new FileStream(Resources.temp_directory + "\\stream.tmp", FileMode.Create);
            json.WriteObject(fs, this);
            fs.Close();
            StreamReader sr = new StreamReader(Resources.temp_directory + "\\stream.tmp");
            string js = sr.ReadToEnd();
            js = FormatJson(js);
            FileStream file = new FileStream(FilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(file);
            sw.WriteLine(js);
            sw.Close();
        }

        /// <summary>
        /// десериализация XML
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        /// <returns></returns>
        private static Options XMLDeserialize(string FilePath)
        {
            if (!File.Exists(FilePath))
                return new Options();


            FileStream fs = new FileStream(FilePath, FileMode.Open);
            XmlSerializer se = new XmlSerializer(typeof(Options));
            try
            {
                Options res = (Options)se.Deserialize(fs);
                res.Format = OptionsFormat.XML;
                res.FilePath = FilePath;
                return res;
            }
            catch (Exception)
            {
                return new Options();
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// десериализация JSON
        /// </summary>
        /// <param name="FilePath">путь к файлу</param>
        /// <returns></returns>
        private static Options JSONDeserialize(string FilePath)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Options));
                string fileContent = File.ReadAllText(FilePath);
                Options res = (Options)json.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
                res.Format = OptionsFormat.JSON;
                return res;
            }
            catch (Exception)
            {
                return new Options();
            }

        }

        /// <summary>
        /// Форматирование JSON строки
        /// Источник: https://wcoder.github.io/notes/format-json-csharp
        /// </summary>
        /// <param name="str">строка JSON без символов новой строки</param>
        /// <returns></returns>
        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            string indentString = "\t";
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, ++indent))
                                sb.Append(indentString);
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, --indent))
                                sb.Append(indentString);
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            foreach (var e in Enumerable.Range(0, indent))
                                sb.Append(indentString);
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        #endregion

    }
}
