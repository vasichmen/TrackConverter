using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Взаимодействие с сайтом TrackConverter
    /// </summary>
    public class WebSite : BaseConnection
    {
        /// <summary>
        /// минимальное время между запросами
        /// </summary>
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromSeconds(0.01);
            }
        }

        /// <summary>
        /// отправить отчет о запуске программы
        /// </summary>
        /// <param name="guid">guid экземпляра</param>
        /// <param name="name">имя пользователя</param>
        private void AttachGuid(string guid,string name)
        {
            string site = Vars.Options.Common.SiteAddress;
            string url = string.Format("{0}/receiver.php?mode=attach&program_guid={1}&user_name={2}", site, guid,name);
            string ans = this.SendStringRequest(url);
        }

        /// <summary>
        /// отправить статистику на сервер
        /// </summary>
        public void SendStatisticAsync()
        {
            Action act = new Action(() =>
            {
                string guid = Vars.Options.Common.ApplicationGuid;
                string name = Environment.UserName;
                AttachGuid(guid,name);
            });
            Task ts = new Task(act);
            ts.Start();
        }
    }
}
