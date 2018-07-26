using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Classes
{
    /// <summary>
    /// информация о поставщике слоя на карте 
    /// </summary>
    public class VectorMapLayerProviderRecord
    {
        /// <summary>
        /// значение перечисления
        /// </summary>
        public VectorMapLayerProviders Enum { get; set; }

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

        /// <summary>
        /// поставщик слоя по ID
        /// </summary>
        /// <param name="id">ID поставщика</param>
        /// <returns></returns>
        public static VectorMapLayerProviderRecord FromID(int id)
        {
            foreach (VectorMapLayerProviderRecord m in Vars.Options.Map.AllLayerProviders)
                if (m.ID == id)
                    return m;
            throw new ArgumentOutOfRangeException("Поставщика слоя с id " + id + " не обнаружено!");
        }
    }
}
