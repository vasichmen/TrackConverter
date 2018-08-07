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
            StreamReader sr;
            if (!File.Exists(directory + "\\" + dataFileName))
                sr = new StreamReader(File.Create(directory + "\\" + dataFileName));
            else
                sr = new StreamReader(directory + "\\" + dataFileName, Encoding.UTF8);
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

        public bool CheckImage(string url)
        {
            return images_data.ContainsKey(url);
        }

        public Image GetImage(string url)
        {
            if (images_data.ContainsKey(url))
            {
                string fname = directory + "\\" + images_data[url].path;
                Image res = Image.FromFile(fname);
                return res;
            }
            else
                return null;
        }

        public bool PutImage(string url, Image data)
        {
            if (!CheckImage(url))
            {
                string fname = getFileName(url);
                data.Save(directory+"\\"+ fname);
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

        private string getFileName(string url)
        {
            string ext = Path.GetExtension(url);
            string name = Path.GetFileNameWithoutExtension(url);

            string res =  name + ext;
            int i = 0;
            while (File.Exists( directory + "\\" +res))
            {
                res = name + "_" + i + ext;
                i++;
            }
            return res;
        }

        public void Close()
        {
            //StreamWriter sw = new StreamWriter(dataFileName, false, Encoding.UTF8);
            //string js =data.ToString(Newtonsoft.Json.Formatting.Indented);
            //sw.WriteLine(js);
            //sw.Close();
        }
    }
}
