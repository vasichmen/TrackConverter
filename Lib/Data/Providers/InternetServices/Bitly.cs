using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    class Bitly : BaseConnection, ILinkShorterProvider
    {
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

        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(500);
            }
        }

        public string Short(string Link)
        {
            string url = "https://bitly.com/data/shorten";
            string ans = this.SendStringPostRequest(url, Link);
            return ans;
        }
    }
}
