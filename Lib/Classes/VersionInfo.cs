using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Classes
{
    /// <summary>
    /// информация о версии программы
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// список изменений
        /// </summary>
        public string Changes { get; internal set; }

        /// <summary>
        /// дата публикации
        /// </summary>
        public DateTime ReleaseDate { get; internal set; }

        /// <summary>
        /// номер версии
        /// </summary>
        public int VersionInt { get; internal set; }

        /// <summary>
        /// текстовое представление весрии
        /// </summary>
        public string VersionText { get; internal set; }
    }
}
