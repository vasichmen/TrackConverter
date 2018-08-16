using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик механизма кэширования веб-запросов
    /// </summary>
    interface IWebCache
    {
        /// <summary>
        /// добавить ответ сервера в кэш
        /// </summary>
        /// <param name="url">url запроса</param>
        /// <param name="data">ответ сервера</param>
        /// <returns></returns>
        bool PutWebUrl(string url, string data);

        /// <summary>
        /// проверка существования запроса в кэше
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool ContainsWebUrl(string url);

        /// <summary>
        /// получить ответ сервера из кэша
        /// </summary>
        /// <param name="url">запрос к серверу</param>
        /// <returns></returns>
        string GetWebUrl(string url);

    }
}
