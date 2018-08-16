using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;

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
        class FileInfo
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
        string dataFileName = "data.txt";

        /// <summary>
        /// базовая папка с кэшем
        /// </summary>
        string directory;

        static object locker = new object();

        /// <summary>
        /// информация о файлах в кэше
        /// </summary>
        Dictionary<string, FileInfo> files_data;

        /// <summary>
        /// создвёт новый объект кэша в указанной папке
        /// </summary>
        /// <param name="dir">базовая папка с файлами</param>
        public FileSystemCache(string dir, TimeSpan duration)
        {
            directory = dir;
            Directory.CreateDirectory(directory);
            files_data = new Dictionary<string, FileInfo>();

            //заполнение информации о файлах
            LoadDataFile(directory + "\\" + dataFileName);

            //удаление устаревших объектов
            DeleteObjectsBefore(DateTime.Now - duration);
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
                string fname = getFileName(url);
                data.Save(directory + "\\" + fname);
                FileInfo info = new FileInfo() { date = DateTime.Now, url = url, path = fname };
                files_data.Add(url, info);

                //дописываем файл
                StreamWriter sw = new StreamWriter(directory + "\\" + dataFileName, true, Encoding.UTF8);
                string line = info.url + "*" + info.path + "*" + info.date.ToString();
                sw.WriteLine(line);
                sw.Close();
                return true;
            }
            else return false;
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
                string fname = getFileNameFromUrl(url);
                StreamWriter sw = new StreamWriter(directory + "\\" + fname, false, Encoding.UTF8);
                sw.Write(data);
                sw.Close();

                //сохраняем информацию для проверки
                FileInfo info = new FileInfo() { date = DateTime.Now, url = url, path = fname };
                files_data.Add(url, info);

                //дописываем файл
                lock (locker)
                {
                    sw = new StreamWriter(directory + "\\" + dataFileName, true, Encoding.UTF8);
                    string line = info.url + "*" + info.path + "*" + info.date.ToString();
                    sw.WriteLine(line);
                    sw.Close();
                }
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
            url = url.TrimStart(new char[] { 'h', 't', 'p', ':', '/' });
            int ind = url.IndexOf('/');
            string serv = url.Substring(0, ind);
            int ind2 = url.LastIndexOf("?");
            string pars = url.Substring(ind2 + 1);

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
        /// удалить файлы в кэше, созданные раньше заданной даты
        /// </summary>
        /// <param name="dateTime">файлы, созданные ранее, будут удалены</param>
        private void DeleteObjectsBefore(DateTime dateTime)
        {
            List<string> urls = new List<string>();

            //удаление файлов
            foreach (var kv in files_data)
            {
                if (kv.Value.date < dateTime) //если файл старше, то удаляем
                {
                    File.Delete(directory + "\\" + kv.Value.path);
                    urls.Add(kv.Value.url);
                }
            }

            //удаление ключей
            foreach (string url in urls)
                files_data.Remove(url);

            //перезапись файла информации
            ExportDataFile(directory + "\\" + dataFileName);
        }

        /// <summary>
        /// записать информацию из files_data в файл информации (перед выходом, например)
        /// </summary>
        /// <param name="fname"></param>
        private void ExportDataFile(string fname)
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
        private void LoadDataFile(string fname)
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
                files_data.Add(fi.url, fi);
            }
        }

        #endregion
    }
}
