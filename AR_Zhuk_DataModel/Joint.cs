using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_DataModel
{
    /// <summary>
    /// Вид торца секции
    /// </summary>
    public enum Joint
    {
        /// <summary>
        /// Обычный - когда соседняя секция тойже этажности
        /// </summary>
        None,
        /// <summary>
        /// Торец - когда соседней секции нет, или она ниже
        /// </summary>
        End,
        /// <summary>
        /// Деф.шов - когда соседняя секция выше
        /// </summary>
        Seam
    }
}
