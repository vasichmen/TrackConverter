using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Res.Properties;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// взаимодействие с сайтом вконтакте
    /// </summary>
    class VK : BaseConnection, ILinkShorterProvider
    {
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromSeconds(0.5);
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
        /// сократить ссылку vk.cc
        /// </summary>
        /// <param name="Link"></param>
        /// <returns></returns>
        public string Short(string Link)
        {
            string token = Resources.VK_access_token;
            string enc = HttpUtility.UrlEncodeUnicode(Link); //кодирование ссылки. Без него точки не сохранятся
            //enc = enc.Replace("amp%3B", "");
            string url = string.Format(@"https://api.vk.com/method/utils.getShortLink?url={0}&v=5.62&access_token={1}", enc,token);
            string ans = SendStringGetRequest(url);
            JObject jo = JObject.Parse(ans);
            string res = jo["response"]["short_url"].ToString();
            return res;
        }
    }
}
