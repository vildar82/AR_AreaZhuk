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
    public class SideFlatFake
    {
        static List<SideFlatFake> SideFlats = new List<SideFlatFake>()
            {
                new SideFlatFake ("PIK1_2KL2_A0", "2|3,B"),
                new SideFlatFake ("PIK1_2KL2_Z0", "1|2,B")
            };

        /// <summary>
        /// Имя квартиры - полное
        /// </summary>
        public string Name { get; private set; }
        public string LightingStringWithB { get; private set; }       

        public SideFlatFake (string name, string ligthingWithB)
        {
            Name = name;
            LightingStringWithB = ligthingWithB;            
        }

        public static SideFlatFake GetSideFlat (string flatName)
        {
            var res = SideFlats. Find(f => f.Name == flatName);
            return res;
        }                
    }
}
