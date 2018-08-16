using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Работа с сервисом сокращения ссылок Qps.ru
    /// </summary>
    class Qps : BaseConnection, ILinkShorterProvider
    {
        public Qps() : base(null) { }

        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(100);
            }
        }

        /// <summary>
        /// максимальное число попыток подключения
        /// </summary>
        public override int MaxAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// сократить ссылку
        /// </summary>
        /// <param name="Link">длинная ссылка</param>
        /// <returns></returns>
        public string Short(string Link)
        {
            string l = HttpUtility.UrlEncode(Link);
            string url = string.Format(@"http://qps.ru/api?url={0}", l);
            string ans = base.SendStringGetRequest(url);
            return ans;
        }
    }
}
