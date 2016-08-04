using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_Schema.Insolation
{
    /// <summary>
    /// Временно - для подмены индексов освещенности боковых квартир
    /// Квартира с боковым окном - выходящим на торец секции
    /// </summary>
    public class SideFlatFake
    {
        static List<SideFlatFake> SideFlats = new List<SideFlatFake>()
        {
            new SideFlatFake ("PIK1_2KL2_A0", "2|3,B", ""),
            new SideFlatFake ("PIK1_2KL2_Z0", "1|2,B", ""),
            new SideFlatFake ("PIK1_2KL3_A0", "1|B", "B|1"),
            new SideFlatFake ("PIK1_2KL3_Z0", "B|1", "2|B"),
            new SideFlatFake ("PIK1_3KL1_A0", "B|1", "1,3|B"),
            new SideFlatFake ("PIK1_3KL1_Z0", "1|B", "B|1,3"),
            new SideFlatFake ("PIK1_3KL2_A0", "B|1", "2,3|B"),
            new SideFlatFake ("PIK1_3KL2_Z0", "1|B", "B|1,2"),
            new SideFlatFake ("PIK1_3KL3_A0", "B|1,2", "2|B"),
            new SideFlatFake ("PIK1_3KL3_Z0", "1,2|B", "B|1"),
            new SideFlatFake ("PIK1_3NL1_A0", "1|B", "B|1,2|3"),
            new SideFlatFake ("PIK1_3NL1_Z0", "B|1", "1|2,3|B"),
            new SideFlatFake ("PIK1_4KL1_A0", "B|1,2", "2,3|B"),
            new SideFlatFake ("PIK1_4KL1_Z0", "1,2|B", "B|1-2"),
            new SideFlatFake ("PIK1_4KL2_A0", "B|1,2", "2,3|B"),
            new SideFlatFake ("PIK1_4KL2_Z0", "1,2|B", "B|1,2"), 
            new SideFlatFake ("PIK1_4NL2_A0", "1,2|B", "B|1,2|3"), 
            new SideFlatFake ("PIK1_4NL2_Z0", "B|1,2", "1|2,3|B")
        };

        /// <summary>
        /// Имя квартиры - полное
        /// </summary>
        public string Name { get; private set; }
        public string LightingTop { get; private set; }
        public string LightingBot { get; private set; }        

        public SideFlatFake (string name, string ligthingBot, string ligthingTop)
        {
            Name = name;
            LightingTop = ligthingTop;
            LightingBot = ligthingBot;
        }

        public static SideFlatFake GetSideFlat (string flatName)
        {
            var res = SideFlats. Find(f => f.Name == flatName);
            return res;
        }                
    }
}
