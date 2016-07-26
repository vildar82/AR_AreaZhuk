using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    static class InsCheckFactory
    {
        public static IInsCheck CreateInsCheck (InsolationSpot insSpot, Section section, 
            StartCellHelper startCellHelper,  List<FlatInfo> sections, SpotInfo sp)
        {
            IInsCheck insCheck = null;

            if(section.IsCorner)
            {
                insCheck = new InsCheckCorner(insSpot, section, startCellHelper, sections, sp);
            }
            else
            {
                insCheck = new InsCheckOrdinary(insSpot, section, startCellHelper, sections, sp);
            }

            return insCheck;
        }        
    }
}
