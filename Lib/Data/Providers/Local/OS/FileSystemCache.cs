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
    //TODO: описания методов
    public class FileSystemCache : IImagesCache
    {
        class FileInfo
        {
            public string url;
            public string path;
            public DateTime date;
        }

        string dataFileName = "data.txt";
        string directory;
        Dictionary<string, FileInfo> images_data;

        public FileSystemCache(string dir)
        {
            directory = dir;
            Directory.CreateDirectory(directory);
            images_data = new Dictionary<string, FileInfo>();

            //заполнение информации о файлах
            LoadDataFile(directory + "\\" + dataFileName);

            //удаление устаревших объектов
            DeleteObjectsBefore(DateTime.Now - TimeSpan.FromDays( Vars.Options.DataSources.MaxImageCacheDays));
        }


        #region IImageCache

        public bool CheckImage(string url)
        {
            return images_data.ContainsKey(url);
        }

        public Image GetImage(string url)
        {
            if (images_data.ContainsKey(url))
            {
                try
                {
                    string fname = directory + "\\" + images_data[url].path;
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

        public bool PutImage(string url, Image data)
        {
            if (!CheckImage(url))
            {
                string fname = getFileName(url);
                data.Save(directory + "\\" + fname);
                FileInfo info = new FileInfo() { date = DateTime.Now, url = url, path = fname };
                images_data.Add(url, info);

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

        #region вспомогательные методы

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


        private void DeleteObjectsBefore(DateTime dateTime)
        {
            List<string> urls = new List<string>();

            //удаление файлов
            foreach (var kv in images_data)
            {
                if (kv.Value.date < dateTime) //если файл старше, то удаляем
                {
                    File.Delete(directory + "\\" + kv.Value.path);
                    urls.Add(kv.Value.url);
                }
            }

            //удаление ключей
            foreach (string url in urls)
                images_data.Remove(url);

            //перезапись файла информации
            ExportDataFile(directory + "\\" + dataFileName);
        }

        private void ExportDataFile(string fname)
        {
            StreamWriter sw = new StreamWriter(fname, false, Encoding.UTF8);
            foreach (var kv in images_data)
            {
                string line = kv.Value.url + "*" + kv.Value.path + "*" + kv.Value.date.ToString();
                sw.WriteLine(line);
            }
            sw.Close();
        }

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
                images_data.Add(fi.url, fi);
            }
        }

        #endregion
    }
}
