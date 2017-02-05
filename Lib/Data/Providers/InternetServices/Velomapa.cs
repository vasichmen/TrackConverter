using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    /// <summary>
    /// Взаимодействие с сайтом TrackConverter
    /// </summary>
    public class Velomapa : BaseConnection
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
        private void AttachGuid(string guid, string name)
        {
            string site = Vars.Options.Common.SiteAddress;
            string url = string.Format("{0}/receiver.php?mode=attach&program_guid={1}&user_name={2}", site, guid, name);
            string ans = this.SendStringRequest(url);
            if (ans != "OK")
                throw new WebException(ans);
        }

        /// <summary>
        /// узнать последнюю версию на сайте
        /// </summary>
        /// <returns></returns>
        private float GetVersion()
        {
            try
            {
                string site = Vars.Options.Common.SiteAddress;
                string url = string.Format("{0}/receiver.php?mode=version", site);
                string ver = this.SendStringRequest(url);
                return Convert.ToSingle(ver.Replace(".", ""));
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// узнать последнюю версию на сайте
        /// </summary>
        /// <returns></returns>
        public void GetVersionAsync()
        {
            Action act = new Action(() =>
            {
                bool f = true;
                int i = 0;
                while (f && i < 3)
                {
                    try
                    {
                        string guid = Vars.Options.Common.ApplicationGuid;
                        string name = Environment.UserName;
                        float actVer = GetVersion();
                        float curVer = Vars.Options.Common.Version;
                        if (actVer > curVer)
                            if (MessageBox.Show(null, "Текущая версия " + curVer + ", новая версия " + actVer + ". Загрузить новую версию?", "Доступна новая версия", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                Process.Start(Vars.Options.Common.SiteAddress + "/programs.php?item=TrackConverter");
                                return;
                            }
                        f = false;
                        i++;
                    }
                    catch (WebException wex)
                    {
                        f = true;
                        Thread.Sleep(2000);
                    }
                }
            });
            Task ts = new Task(act);
            ts.Start();
        }

        /// <summary>
        /// отправить статистику на сервер
        /// </summary>
        public void SendStatisticAsync()
        {
            Action act = new Action(() =>
            {
                bool f = true;
                int i = 0;
                while (f && i < 3)
                {
                    try
                    {
                        string guid = Vars.Options.Common.ApplicationGuid;
                        string name = Environment.UserName;
                        AttachGuid(guid, name);
                        f = false;
                        i++;
                    }
                    catch (WebException wex)
                    {
                        f = true;
                        Thread.Sleep(2000);
                    }
                }


            });
            Task ts = new Task(act);
            ts.Start();
        }
    }
}
