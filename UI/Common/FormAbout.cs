using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.UI.Common.Dialogs;

namespace TrackConverter.UI.Common
{
    /// <summary>
    /// окно о программе
    /// </summary>
    internal partial class FormAbout : Form
    {
        /// <summary>
        /// создает новое окно
        /// </summary>
        public FormAbout()
        {
            InitializeComponent();
            this.Text = string.Format("О программе {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = string.Format("Версия {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.linkLabelSite.Text = Vars.Options.Common.SiteAddress;
            this.linkLabelTlg.Text = "Telegram: @vasichmen";
            this.linkLabelGithub.Text = "Репозиторий GitHub";
            this.textBoxDescription.Text = AssemblyDescription;
        }

        #region Методы доступа к атрибутам сборки

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        /// <summary>
        /// закрывание окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// переход на сайт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Vars.Options.Common.SiteAddress);
        }

        /// <summary>
        /// прверка новой версии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Velomapa site = new Velomapa();
                VersionInfo vi = site.GetVersion();
                //действие при проверке версии

                float curVer = Vars.Options.Common.VersionInt;
                if (vi.VersionInt > curVer)
                {
                    FormUpdateDialog fud = new FormUpdateDialog(vi);
                    this.Invoke(new Action(() => fud.ShowDialog()));
                }
                else
                    MessageBox.Show(this, "Обновлений нет!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (WebException we)
            {
                MessageBox.Show(this, "Ошибка подключения!\r\n" + we.Message, "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// нажатие на ссылку связи в телеграме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelTlg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Process.Start(Vars.Options.Common.TelegramAddress);
        }

        /// <summary>
        /// нажатие ссылки на гитхаб
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Vars.Options.Common.GitHubRepository);
        }
    }
}
