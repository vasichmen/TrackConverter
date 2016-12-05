using System;
namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик сокращения ссылок
    /// </summary>
    interface ILinkShorterProvider
    {
        /// <summary>
        /// сократить ссылку. Используется выбранный сервис
        /// </summary>
        /// <param name="Link"></param>
        /// <returns></returns>
        string Short(string Link);
    }
}
