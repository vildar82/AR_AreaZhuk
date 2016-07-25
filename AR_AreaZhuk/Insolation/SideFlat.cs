using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Квартира с боковым окном - выходящим на торец секции
    /// </summary>
    public class SideFlat
    {
        /// <summary>
        /// Имя квартиры - полное
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Индекс освещенного модуля на торцевой стороне        
        /// </summary>
        public int IndexLightSide { get; private set; }
        
        public SideFlat(string name, int indexLightSide)
        {
            Name = name;
            IndexLightSide = indexLightSide;            
        }
    }
}
