using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Exceptions;

namespace TrackConverter.Lib.Data.Providers.Local.OS
{
    /// <summary>
    /// кэш в файловой системе
    /// </summary>
    public class FileSystemCache : IImagesCache, IWebCache
    {
        /// <summary>
        /// информация о файле в кэше
        /// </summary>
        private class FileInfo
        {
            /// <summary>
            /// ключ, по которому был добавлен файл
            /// </summary>
            public string url;

            /// <summary>
            /// имя в папке кэша
            /// </summary>
            public string path;

            /// <summary>
            /// дата добавления
            /// </summary>
            public DateTime date;
        }

        /// <summary>
        /// имя файла с данными о файлах в кэше
        /// </summary>
        private readonly string dataFileName = "data.txt";

        /// <summary>
        /// если истина, то файл загружен
        /// </summary>
        private bool isFileLoaded = false;

        /// <summary>
        /// базовая папка с кэшем
        /// </summary>
        private readonly string directory;

        /// <summary>
        /// блокировка многопоточного доступа к файлу информации о кэше
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// блокировка многопоточного доступа к файлам данных
        /// </summary>
        private readonly object lockerName = new object();

        /// <summary>
        /// информация о файлах в кэше
        /// </summary>
        private ConcurrentDictionary<string, FileInfo> files_data;

        /// <summary>
        /// создвёт новый объект кэша в указанной папке
        /// </summary>
        /// <param name="dir">базовая папка с файлами</param>
        public FileSystemCache(string dir, TimeSpan duration)
        {
            directory = dir;
            Directory.CreateDirectory(directory);
            files_data = new ConcurrentDictionary<string, FileInfo>();

            //заполнение информации о файлах
            loadDataFile(directory + "\\" + dataFileName);

            //удаление устаревших объектов
            deleteObjectsBefore(DateTime.Now - duration);

            //удаление файлов, которых нет в файле записей
            deleteMissedFiles();
        }



        #region IImagesCache

        /// <summary>
        /// провера существования изобржения в кэше
        /// </summary>
        /// <param name="url">url, по которому изображения было добавлено</param>
        /// <returns></returns>
        public bool CheckImage(string url)
        {
            return files_data.ContainsKey(url);
        }

        /// <summary>
        /// получить изображение из кэша
        /// </summary>
        /// <param name="url">url, по которому изображения было добавлено</param>
        /// <returns></returns>
        public Image GetImage(string url)
        {
            if (files_data.ContainsKey(url))
            {
                try
                {
                    string fname = directory + "\\" + files_data[url].path;
                    if (File.Exists(fname))
                    {
                        Image res = Image.FromFile(fname);
                        return res;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// добавить изображение в кэш
        /// </summary>
        /// <param name="url">url изображения (используется как ключ для поиска)</param>
        /// <param name="data">изображение</param>
        /// <returns></returns>
        public bool PutImage(string url, Image data)
        {
            if (!CheckImage(url))
            {
                Image copy = (Image)data.Clone();

                string fname = getFileName(url);
                copy.Save(directory + "\\" + fname);
                FileInfo info = new FileInfo() { date = DateTime.Now, url = url, path = fname };
                files_data.TryAdd(url, info);

                //дописываем файл
                lock (locker)
                {
                    StreamWriter sw = new StreamWriter(directory + "\\" + dataFileName, true, Encoding.UTF8);
                    string line = info.url + "*" + info.path + "*" + info.date.ToString();
                    sw.WriteLine(line);
                    sw.Close();
                    return true;
                }
            }
            else
                return false;
        }

        #endregion

        #region IWebCache

        /// <summary>
        /// добавить ответ сервера в кэш
        /// </summary>
        /// <param name="url">url  запроса</param>
        /// <param name="data">ответ сервера</param>
        /// <returns></returns>
        public bool PutWebUrl(string url, string data)
        {
            if (!ContainsWebUrl(url))
            {
                //сохраняем файл с данными

                string fname;
                StreamWriter sw;
                lock (lockerName)
                {
                    fname = getFileNameFromUrl(url);
                    sw = new StreamWriter(directory + "\\" + fname, false, Encoding.UTF8);
                    sw.Write(data);
                    sw.Close();

                }
                //while (File.Exists(fname))
                //  fname = getFileNameFromUrl(url);

                //сохраняем информацию для проверки
                FileInfo info = new FileInfo() { date = DateTime.Now, url = url, path = fname };
                files_data.TryAdd(url, info);

                //дописываем файл
                string line = info.url + "*" + info.path + "*" + info.date.ToString();
                append(line);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// проверка существования ответа сервера в кэше
        /// </summary>
        /// <param name="url">запрос </param>
        /// <returns></returns>
        public bool ContainsWebUrl(string url)
        {
            return files_data.ContainsKey(url);
        }

        /// <summary>
        /// получить ответ сервера из кэша
        /// </summary>
        /// <param name="url">запрос к серверу</param>
        /// <returns></returns>
        public string GetWebUrl(string url)
        {
            if (files_data.ContainsKey(url))
            {
                try
                {
                    string fname = directory + "\\" + files_data[url].path;
                    if (File.Exists(fname))
                    {
                        string res = null;
                        StreamReader sr = new StreamReader(fname, Encoding.UTF8, true);
                        res = sr.ReadToEnd();
                        sr.Close();
                        return res;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
                return null;
        }

        private void append(string line)
        {
            lock (locker)
            {
                StreamWriter sw;
                sw = new StreamWriter(directory + "\\" + dataFileName, true, Encoding.UTF8);
                sw.WriteLine(line);
                sw.Close();
            }
        }

        #endregion

        #region вспомогательные методы

        /// <summary>
        /// получить имя файла (на основе url), которое можно использовать в этой папке
        /// </summary>
        /// <param name="url">url файла, на основе которого будет выбрано имя файла в кэше</param>
        /// <returns></returns>
        private string getFileName(string url)
        {
            string ext = Path.GetExtension(url);
            string name = Path.GetFileNameWithoutExtension(url);

            string res = name + ext;
            int i = 0;
            while (File.Exists(directory + "\\" + res))
            {
                res = name + "_" + i + ext;
                i++;
            }
            return res;
        }

        /// <summary>
        /// получить имя файла из строки запроса. (оставляет имя сервера и параметры запроса)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string getFileNameFromUrl(string url)
        {
            url = url.TrimStart(new char[] { 'h', 't', 'p', 's', ':', '/' });
            int ind = url.IndexOf('/');
            string serv = url.Substring(0, ind);
            int ind2 = url.LastIndexOf("?");
            string pars = url.Substring(ind2 + 1);

            //удаление знаков, которые нельзя использовать в именах файлов
            pars = pars.Replace(":", "").Replace("<", "").Replace(">", "").Replace("*", "")
                       .Replace("?", "").Replace(@"/", "").Replace(@"\", "").Replace("|", "");
            if (pars.Length > 10)
                pars = pars.Substring(0, 10);
            string res = serv + "_" + pars + ".urldata";
            int i = 0;
            while (File.Exists(directory + "\\" + res))
            {
                res = serv + "_" + pars + "_" + i + ".urldata";
                i++;
            }
            return res;
        }

        /// <summary>
        /// удалить файлы в кэше, созданные раньше заданной даты и записей, для которых отсутствуют файлы
        /// </summary>
        /// <param name="dateTime">файлы, созданные ранее, будут удалены</param>
        private void deleteObjectsBefore(DateTime dateTime)
        {
            //список ключей, которые будут удалены из словаря информации
            List<string> urls = new List<string>();

            //удаление файлов
            foreach (var kv in files_data)
            {
                if (kv.Value.date < dateTime || !File.Exists(directory + "\\" + kv.Value.path)) //если файл старше или отсутствует на диске, то удаляем
                {
                    File.Delete(directory + "\\" + kv.Value.path);
                    urls.Add(kv.Value.url);
                }
            }

            //удаление ключей
            foreach (string url in urls)
                files_data.TryRemove(url, out FileInfo info);

            //перезапись файла информации
            exportDataFile(directory + "\\" + dataFileName);
        }

        /// <summary>
        /// удаление файлов, которых нет в файле кэша и записей из файла, для которых нет файлов
        /// </summary>
        private void deleteMissedFiles()
        {
            if (!isFileLoaded)
                throw new TrackConverterException("Перед удалением отсутствующих файлов необходимо загрузить файл данных");

            foreach (var file in Directory.EnumerateFiles(directory))
            {
                bool f = false;
                foreach (FileInfo fi in files_data.Values)
                    if (fi.path == Path.GetFileName(file))
                    {
                        f = true;
                        break;
                    }
                if (!f && Path.GetFileName(file) != dataFileName)
                    try
                    { File.Delete(file); }
                    catch (Exception) { }
            }
        }

        /// <summary>
        /// записать информацию из files_data в файл информации (перед выходом, например)
        /// </summary>
        /// <param name="fname"></param>
        private void exportDataFile(string fname)
        {
            StreamWriter sw = new StreamWriter(fname, false, Encoding.UTF8);
            foreach (var kv in files_data)
            {
                string line = kv.Value.url + "*" + kv.Value.path + "*" + kv.Value.date.ToString();
                sw.WriteLine(line);
            }
            sw.Close();
        }

        /// <summary>
        /// загрузить информацию о файлах в кэше из заданного файла 
        /// </summary>
        /// <param name="fname">адрес файла информации о файлах в кэше</param>
        private void loadDataFile(string fname)
        {
            StreamReader sr;
            if (!File.Exists(fname))
                sr = new StreamReader(File.Create(fname));
            else
                sr = new StreamReader(fname, Encoding.UTF8);

            string t = sr.ReadToEnd();
            sr.Close();
            string[] files = t.Split('\n');
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo();
                string[] fields = file.Split('*');
                if (fields.Length != 3)
                    continue;
                fi.url = fields[0];
                fi.path = fields[1];
                fi.date = DateTime.Parse(fields[2]);
                files_data.TryAdd(fi.url, fi);
            }
            isFileLoaded = true;
        }

        #endregion
    }
}
