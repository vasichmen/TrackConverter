using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.MapProviders;

namespace TrackConverter.Lib.Classes {

    /// <summary>
    /// структура поставщика карты для настроек и тега в кнопке выбора источника карты
    /// </summary>
    public class MapProviderRecord {

        /// <summary>
        /// поставщик
        /// </summary>
        public MapProviders Enum { get; set; }

        /// <summary>
        /// заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// идентификатор
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Изображение карты
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// максимальное число параллельных запросов к серверу
        /// </summary>
        public int MaxParallelPool { get; set; }

        /// <summary>
        /// поставщик карты по ID
        /// </summary>
        /// <param name="id">ID поставщика</param>
        /// <returns></returns>
        public static MapProviderRecord FromID( int id ) {
           foreach ( MapProviderRecord m in Vars.Options.Map.AllMapProviders )
               if ( m.ID == id )
                   return m;
           throw new ArgumentOutOfRangeException( "Поставщика карты с id " + id + " не обнаружено!" );
       }
    }
}
