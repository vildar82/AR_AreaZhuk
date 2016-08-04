using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_Schema.Scheme
{
    /// <summary>
    /// Вид торца сегмента
    /// </summary>
    public enum SegmentEnd
    {
        /// <summary>
        /// Прямой стык с соседней секцией
        /// </summary>
        Normal,
        /// <summary>
        /// Торец - начало или конец дома
        /// </summary>
        End,
        /// <summary>
        /// Угловой торец с поворотом на лево - от главного направления сегмента
        /// </summary>
        CornerLeft,
        /// <summary>
        /// Угловой сегмент с поворотом направо
        /// </summary>
        CornerRight
    }
}
