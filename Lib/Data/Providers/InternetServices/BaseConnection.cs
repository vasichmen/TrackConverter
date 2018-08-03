using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

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
        public BaseConnection()
        {
            InternetReachable = CheckInternet();
            lastQuery = DateTime.MinValue;
            lastCheckInet = DateTime.MinValue;
        }

        /// <summary>
        /// время последнего запроса к сервису
        /// </summary>
        DateTime lastQuery;

        /// <summary>
        /// время последней проверки подключения к сети
        /// </summary>
        DateTime lastCheckInet;

        [DllImport("wininet.dll")]
        static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved);

        [Flags]
        enum InternetConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        /// <summary>
        /// загрузка изображения по заданной ссылке
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Image GetImage(string url)
        {
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(url);
            request1.Method = "GET";
            request1.KeepAlive = true;
            HttpWebResponse response1 = (HttpWebResponse)request1.GetResponse();
            Image res = Image.FromStream(response1.GetResponseStream());
            request1.Abort();
            return res;
        }

        /// <summary>
        /// провекрка подключения к интернету
        /// </summary>
        /// <returns></returns>
        public bool CheckInternet()
        {
            if (DateTime.Now - this.lastCheckInet < TimeSpan.FromMinutes(1))
                return InternetReachable;
            try
            {
                InternetConnectionState flags = InternetConnectionState.INTERNET_CONNECTION_CONFIGURED | 0;
                bool checkStatus = InternetGetConnectedState(ref flags, 0);
                if (checkStatus)
                {
                    string[] serverList = new string[] { @"google.com" };
                    bool haveAnInternetConnection = false;
                    Ping ping = new Ping();
                    for (int i = 0; i < serverList.Length; i++)
                    {
                        PingReply pingReply = ping.Send(serverList[i], 5000);
                        haveAnInternetConnection = (pingReply.Status == IPStatus.Success);
                        if (haveAnInternetConnection)
                            break;
                    }
                    return haveAnInternetConnection;
                }
                return checkStatus;
            }
            catch { return false; }
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
        /// проверка подключения к интернет
        /// </summary>
        public bool InternetReachable { get; set; }

        /// <summary>
        /// отправка запроса с результатом в виде xml
        /// </summary>
        /// <param name="url">запрос</param>
        /// <exception cref="Exception">Если произошла ошибка при подключении</exception>
        /// <returns></returns>
        protected XmlDocument SendXmlGetRequest(string url)
        {
            XmlDocument res = new XmlDocument();
            HttpStatusCode code;
            string ans = SendStringGetRequest(url, out code);
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
            
            //StreamReader sr = new StreamReader("f.html");
            //code = HttpStatusCode.OK;
            //var ans = sr.BaseStream;

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
            HttpStatusCode code;
            string ans = SendStringGetRequest(url, out code);
            return ans;
        }

        /// <summary>
        /// отправка запроса с результатом в виде строки
        /// </summary>
        /// <param name="url">запрос</param>
        /// <returns></returns>
        /// <exception cref="WebException">Если произошла ошибка при подключении</exception>
        protected string SendStringGetRequest(string url, out HttpStatusCode code)
        {
            //проверка подключения
            InternetReachable = CheckInternet();
            if (!(bool)InternetReachable)
                throw new WebException("Подключение к интернет не установлено!");

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

                return responsereader;
            }
            catch (WebException we) { throw new WebException("Ошибка подключения.\r\n" + url, we); }
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
            HttpStatusCode code;
            string json = SendStringGetRequest(url, out code);
            json = json.Substring(json.IndexOf('{'));
            json = json.TrimEnd(new char[] { ';', ')' });
            try { jobj = JObject.Parse(json); }
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
            //проверка подключения
            InternetReachable = CheckInternet();
            if (!(bool)InternetReachable)
                throw new WebException("Подключение к интернет не установлено!");

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
