using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    public class WebSite : BaseConnection
    {
        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }

        public void AttachGuid(string guid, string options)
        {
            string site = Vars.Options.Common.SiteAddress;
            string url = string.Format("{0}/receiver.php?mode=attach&program_guid={1}", site, guid);
            string ans = this.SendStringRequest(url);
            //MessageBox.Show(ans);
        }

        public void SendStatistic()
        {
            string guid = Vars.Options.Common.ApplicationGuid;
            string options = Vars.Options.GetXmlText();
            AttachGuid(guid, options);
        }
    }
}
