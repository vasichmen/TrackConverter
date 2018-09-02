using System;
using System.Windows.Forms;
using TrackConverter.Res.Properties;

namespace TrackConverter.UI.Common
{

    /// <summary>
    /// класс значка программы в трее
    /// </summary>
    public class TrayIcon: IDisposable
    {
        private NotifyIcon notify;
        private readonly Action onDoubleClick;

        /// <summary>
        /// возвращает значение, показывающее, виден ли значо в трее
        /// </summary>
        public bool IsShowing { get { return notify.Visible; } }

        /// <summary>
        /// создает новый экземпляр класса
        /// </summary>
        /// <param name="onDoubleClick">событие при двойном щелчке мыши</param>
        /// <param name="contextMenu">контекстное меню</param>
        public TrayIcon(Action onDoubleClick, ContextMenu contextMenu)
        {
            notify = new NotifyIcon();
            this.notify.ContextMenu = contextMenu;
            this.onDoubleClick = onDoubleClick;
            notify.Icon = Resources.main_icon;
            notify.DoubleClick += new EventHandler((f, r) => { onDoubleClick.Invoke(); });
        }


        /// <summary>
        /// отображает значок в панели
        /// </summary>
        public void Show()
        {
            notify.Visible = true;
        }

        /// <summary>
        /// прекращет отображение значка
        /// </summary>
        public void UnShow()
        {
            notify.Visible = false;
        }

        /// <summary>
        /// очищает ресурсы
        /// </summary>
        /// <param name="disposing">с</param>
        protected virtual void Dispose(bool disposing)
        {
            notify.Dispose();
        }

        /// <summary>
        /// очищает ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

        }
    }
}
