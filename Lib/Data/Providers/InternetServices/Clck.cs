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
    /// работа с сервисом сокращения ссылок Clck.ru
    /// </summary>
    class Clck :BaseConnection, ILinkShorterProvider
    {
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(100);
            }
        }

        /// <summary>
        /// сократить ссылку. Возвращает сокращенную ссылку
        /// </summary>
        /// <param name="Link">длинная ссылка</param>
        /// <returns></returns>
        public string Short(string Link)
        {
            string enc = HttpUtility.UrlEncode(Link); //кодирование ссылки. Без него точки не сохранятся
            string url = string.Format(@"http://clck.ru/--?url={0}", enc);
            string ans = base.SendStringRequest(url);
            return ans;
        }
    }
}
