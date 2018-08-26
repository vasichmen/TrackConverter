using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TrackConverter.Lib.Data.Interfaces
{
    /// <summary>
    /// поставщик растрового слоя карты
    /// </summary>
    interface IRastrMapLayerProvider
    {
        /// <summary>
        /// Получить картинку по тайловым координатам
        /// </summary>
        /// <param name="x">координата х</param>
        /// <param name="y">координата у</param>
        /// <param name="z">масштаб</param>
        /// <returns></returns>
        Image GetRastrTile(long x, long y, int z);
    }
}
