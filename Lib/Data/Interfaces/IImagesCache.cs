using System.Drawing;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// взаимодействие с кэшем картинок
    /// </summary>
    public interface IImagesCache
    {
        /// <summary>
        /// проверка существования картинки в кэше. Возвращает true, если картинка есть
        /// </summary>
        /// <param name="url">url картинки, с которым она была добавлена в кэш</param>
        /// <returns></returns>
        bool CheckImage(string url);

        /// <summary>
        /// получить картинку из кэша
        /// </summary>
        /// <param name="url">url картинки, с которым она была добавлена в кэш</param>
        /// <returns></returns>
        Image GetImage(string url);

        /// <summary>
        /// добавить картинку в кэш
        /// </summary>
        /// <param name="url">адрес картинки в интернете</param>
        /// <param name="data">картинка</param>
        /// <returns></returns>
        bool PutImage(string url, Image data);
        
    }
}