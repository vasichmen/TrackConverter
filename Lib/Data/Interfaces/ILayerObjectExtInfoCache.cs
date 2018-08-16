using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// кэш дополнительных данных об объектах слоя
    /// </summary>
    internal interface ILayerObjectExtInfoCache
    {
        /// <summary>
        /// проверить, содержится ли объект в кэше
        /// </summary>
        /// <param name="id">ID объекта</param>
        /// <returns></returns>
        bool ContainsLayerObjectExtInfo(int id);

        /// <summary>
        /// получить информацию об объекте из кэша
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns></returns>
        Wikimapia.ExtInfo GetLayerObjectExtInfo(int id);

        /// <summary>
        /// добавить объект в кэш
        /// </summary>
        /// <param name="obj">информация об объекте</param>
        /// <returns></returns>
        bool PutLayerObjectExtInfo(Wikimapia.ExtInfo obj);
    }
}