using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// базовый класс HTTP запросов к серверу
    /// </summary>
   public abstract class BaseConnection {

        /// <summary>
        /// время последнего запроса к сервису
        /// </summary>
        DateTime lastQuery = DateTime.MinValue;

        /// <summary>
        /// Минимальное время между запросами к серверу. Значение по умолчанию 200 мс.
        /// Если время между запросами не прошло, SendStringRequest и SendXmlRequest будут ждать
        /// </summary>
        public abstract TimeSpan MinQueryInterval { get; }

        /// <summary>
        /// отправка запроса с результатом в виде xml
        /// </summary>
        /// <param name="url">запрос</param>
        /// <exception cref="Exception">Если произошла ошибка при подключении</exception>
        /// <returns></returns>
        protected XmlDocument SendXmlRequest( string url ) {
            XmlDocument res = new XmlDocument();
            string ans = SendStringRequest( url );
            res.LoadXml( ans );
            return res;
        }

        /// <summary>
        /// отправка запроса с результатом в виде строки
        /// </summary>
        /// <param name="url">запрос</param>
        /// <returns></returns>
        /// <exception cref="Exception">Если произошла ошибка при подключении</exception>
        protected string SendStringRequest( string url ) {
            try {

                //ожидание времени интервала между запросами
                while (DateTime.Now - lastQuery < MinQueryInterval)
                    Thread.Sleep(50);

                //Выполняем запрос к универсальному коду ресурса (URI).
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create( url );
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
                request.ContentType = "application/xml";
                request.Headers[HttpRequestHeader.AcceptLanguage] = "ru - RU,ru; q = 0.8,en - US; q = 0.6,en; q = 0.4";

                //Получаем ответ от интернет-ресурса.
                WebResponse response =
                    request.GetResponse();

                //string lng = response.Headers[HttpRequestHeader.var];

                //Экземпляр класса System.IO.Stream 
                //для чтения данных из интернет-ресурса.
                Stream dataStream =
                    response.GetResponseStream();

                //Инициализируем новый экземпляр класса 
                //System.IO.StreamReader для указанного потока.
                StreamReader sreader =
                    new StreamReader( dataStream );

                //Считывает поток от текущего положения до конца.            
                string responsereader = sreader.ReadToEnd();

                //Закрываем поток ответа.
                response.Close();

                //запоминание времени запроса
                lastQuery = DateTime.Now;

                return responsereader;
            } catch ( WebException we ) { throw new Exception( "Ошибка подключения.\r\n"+url, we ); }
        }
    }
}
