namespace TrackConverter.Lib.Classes.ProviderRecords
{
    /// <summary>
    /// базовый класс для записей поставщиков
    /// </summary>
    public class BaseProviderRecord
    {

        /// <summary>
        /// заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// идентификатор
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Иконка слоя
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// максимальное число параллельных запросов к серверу
        /// </summary>
        public int MaxParallelPool { get; set; }

    }
}