using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using TrackConverter.Lib.Data.Providers.Local.OS;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// базовый класс HTTP запросов к серверу
    /// </summary>
    public abstract class BaseConnection
    {
        /// <summary>
        /// создает новый экземпляр класса Base Connection
        /// </summary>
        private BaseConnection()
        {
            lastQuery = DateTime.MinValue;
        }

        /// <summary>
        /// создаёт новый объект с кэшем в указанной папке и заданной длительностью хранения
        /// </summary>
        /// <param name="cacheDirectory">папка с кэшем или null, если не надо использоать кэш</param>
        /// <param name="duration">длительность хранения в часах. По умолчанию - неделя</param>
        public BaseConnection(string cacheDirectory, int duration = 7 * 24) : this()
        {
            useCache = cacheDirectory != null;
            if (useCache)
            {
                this.cacheDirectory = cacheDirectory;
                this.duration = duration;
                this.cache = new FileSystemCache(cacheDirectory, TimeSpan.FromHours(duration));
            }
            else
            {

            }
        }

        /// <summary>
        /// время последнего запроса к сервису
        /// </summary>
        private DateTime lastQuery;
        private readonly bool useCache;
        private readonly string cacheDirectory;
        private readonly int duration;
        private FileSystemCache cache;

        /// <summary>
        /// загрузка изображения по заданной ссылке
        /// </summary>
        /// <param name="url"></param>
        /// <param name="operation">метод установки прогресса загрузки файла</param>
        /// <param name="afterLoadComplete">действие, выполняемое по окончании загрузки файла</param>
        /// <returns></returns>
        public static void GetFileAsync(string url, Action<string> operation = null, Action<string> afterLoadComplete = null)
        {
            int i = 0;
            string tmp_file = System.Windows.Forms.Application.StartupPath + Res.Properties.Resources.temp_directory + "\\" + i + ".tmp";
            Directory.CreateDirectory(Path.GetDirectoryName(tmp_file));
            while (File.Exists(tmp_file))
                tmp_file = System.Windows.Forms.Application.StartupPath + Res.Properties.Resources.temp_directory + "\\" + ++i + ".tmp";

            WebClient client = new WebClient();
            client.DownloadProgressChanged +=
                new DownloadProgressChangedEventHandler((sender, e) =>
                {
                    if (operation != null)
                    {
                        operation.Invoke("Загрузка изображения, завершено " + (e as DownloadProgressChangedEventArgs).ProgressPercentage + "%");
                    }
                }
                );
            client.DownloadFileCompleted += new AsyncCompletedEventHandler((sender, e) =>
            {
                if (afterLoadComplete != null)
                    afterLoadComplete.Invoke(tmp_file);
                client.Dispose();
            });
            client.DownloadFileAsync(new Uri(url), tmp_file);
        }

        /// <summary>
        /// получить изображение по ссылке
        /// </summary>
        /// <param name="url">ссылка на изображение</param>
        /// <returns></returns>
        public static Image GetImage(string url)
        {
            WebClient wc = new WebClient();
            try
            {
                Stream str = wc.OpenRead(url);
                if (wc.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                    return new Bitmap(256, 256);
                Image res = Image.FromStream(str);
                str.Close();
                wc.Dispose();
                return res;
            }
            catch (WebException ex)
            {
                Stream resp = ex.Response.GetResponseStream();
                StreamReader sr = new StreamReader(resp);
                string ans = sr.ReadToEnd();
                sr.Close();
                if (ex.Status == WebExceptionStatus.ProtocolError)
                    throw new ApplicationException(ans, ex);
                else
                    return new Bitmap(256, 256);
            }
        }



        /// <summary>
        /// Минимальное время между запросами к серверу. Значение по умолчанию 200 мс.
        /// Если время между запросами не прошло, SendStringRequest и SendXmlRequest будут ждать
        /// </summary>
        public abstract TimeSpan MinQueryInterval { get; }

        /// <summary>
        /// Максимальное число попыток переподключения
        /// </summary>
        public abstract int MaxAttempts { get; }

        /// <summary>
        /// отправка запроса с результатом в виде xml
        /// </summary>
        /// <param name="url">запрос</param>
        /// <exception cref="Exception">Если произошла ошибка при подключении</exception>
        /// <returns></returns>
        protected XmlDocument SendXmlGetRequest(string url)
        {
            XmlDocument res = new XmlDocument();
            string ans = SendStringGetRequest(url);
            res.LoadXml(ans);
            return res;
        }

        /// <summary>
        /// отправить запрос с результатом в виде HTML документа
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="code">код ошибки</param>
        /// <returns></returns>
        protected HtmlDocument SendHtmlGetRequest(string url, out HttpStatusCode code)
        {
            string ans = SendStringGetRequest(url, out code);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(ans);

            return doc;
        }

        /// <summary>
        /// отправка запроса с результатом в виде строки
        /// </summary>
        /// <param name="url">запрос</param>
        /// <returns></returns>
        /// <exception cref="WebException">Если произошла ошибка при подключении</exception>
        protected string SendStringGetRequest(string url)
        {
            string ans = SendStringGetRequest(url, out HttpStatusCode code);
            return ans;
        }

        /// <summary>
        /// отправка запроса с результатом в виде строки
        /// </summary>
        /// <param name="url">запрос</param>
        /// <param name="code">код ответа сервера</param>
        /// <returns></returns>
        /// <exception cref="WebException">Если произошла ошибка при подключении</exception>
        protected string SendStringGetRequest(string url, out HttpStatusCode code)
        {
            if (useCache)
                if (cache.ContainsWebUrl(url))
                {
                    code = HttpStatusCode.OK;
                    return cache.GetWebUrl(url);
                }
            try
            {
                //ожидание времени интервала между запросами
                while (DateTime.Now - lastQuery < MinQueryInterval)
                    Thread.Sleep(50);

                //Выполняем запрос к универсальному коду ресурса (URI).
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
                request.ContentType = "application/xml";
                request.Headers[HttpRequestHeader.AcceptLanguage] = "ru - RU,ru; q = 0.8,en - US; q = 0.6,en; q = 0.4";

                //Получаем ответ от интернет-ресурса.
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //string lng = response.Headers[HttpRequestHeader.var];

                //Экземпляр класса System.IO.Stream 
                //для чтения данных из интернет-ресурса.
                Stream dataStream = response.GetResponseStream();

                //Инициализируем новый экземпляр класса 
                //System.IO.StreamReader для указанного потока.
                StreamReader sreader = new StreamReader(dataStream);
                code = response.StatusCode;

                //Считывает поток от текущего положения до конца.            
                string responsereader = sreader.ReadToEnd();

                //Закрываем поток ответа.
                response.Close();

                //запоминание времени запроса
                lastQuery = DateTime.Now;

                //запись в кэш, если надо
                if (useCache)
                    cache.PutWebUrl(url, responsereader);

                return responsereader;
            }
            catch (WebException we) { throw new WebException("Ошибка подключения.\r\n" + url, we, we.Status, null); }
        }

        /// <summary>
        /// отправка запроса с результатом в виде объекта JSON
        /// </summary>
        /// <param name="url">запрос</param>
        /// <returns></returns>
        /// <exception cref="WebException">Если произошла ошибка при подключении</exception>
        protected JObject SendJsonGetRequest(string url)
        {
            JObject jobj;
            string json = SendStringGetRequest(url);
            json = json.Substring(json.IndexOf('{'));
            json = json.TrimEnd(new char[] { ';', ')' });
            try
            { jobj = JObject.Parse(json); }
            catch (Exception ex) { throw new ApplicationException("Ошибка в парсере JSON. Сервер вернул некорректный объект.", ex); }
            return jobj;
        }

        /// <summary>
        /// отправка POST запроса
        /// </summary>
        /// <param name="url">адрес</param>
        /// <param name="data">данные</param>
        /// <returns></returns>
        protected string SendStringPostRequest(string url, string data)
        {
            try
            {
                //ожидание времени интервала между запросами
                while (DateTime.Now - lastQuery < MinQueryInterval)
                    Thread.Sleep(50);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Timeout = 100000;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
                byte[] sentData = Encoding.GetEncoding(1251).GetBytes(data);
                req.ContentLength = sentData.Length;
                Stream sendStream = req.GetRequestStream();
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();
                WebResponse res = req.GetResponse();
                Stream ReceiveStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8);
                //Кодировка указывается в зависимости от кодировки ответа сервера
                Char[] read = new char[256];
                int count = sr.Read(read, 0, 256);
                string Out = string.Empty;
                while (count > 0)
                {
                    string str = new string(read, 0, count);
                    Out += str;
                    count = sr.Read(read, 0, 256);
                }
                return Out;
            }
            catch (WebException we) { throw new WebException("Ошибка подключения.\r\n" + url, we); }
        }
    }
}
