using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackConverter.Lib.Data.Interfaces;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.Lib.Data
{
    /// <summary>
    /// сокращение ссылок
    /// </summary>
    public class LinkShorter
    {

        private LinkShorterProvider provider;
        private ILinkShorterProvider shorter;

        /// <summary>
        /// создает новый эксемпляр с указанным сервисом сокращения ссылок
        /// </summary>
        /// <param name="provider"></param>
        public LinkShorter(LinkShorterProvider provider)
        {
            this.provider = provider;
            switch (provider)
            {
                case LinkShorterProvider.Clck:
                    shorter = new Clck();
                    break;
                case LinkShorterProvider.Qps:
                    shorter = new Qps();
                    break;
                case LinkShorterProvider.Bitly:
                    shorter = new Bitly();
                    break;
                case LinkShorterProvider.VK:
                    shorter = new VK();
                    break;
                default:
                    throw new ApplicationException("Данный поставщик не поддерживается: " + this.provider);
            }
        }

        /// <summary>
        /// сократить ссылку
        /// </summary>
        /// <param name="Link">ссылка для сокращения</param>
        /// <returns></returns>
        public string Short(string Link)
        {
            return shorter.Short(Link);
        }
    }
}
