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
        public static IInsCheck CreateInsCheck (InsolationSpot insSpot, Section section, bool isCorner,
            bool isVertical, StartCellHelper startCellHelper,  List<FlatInfo> sections, SpotInfo sp)
        {
            IInsCheck insCheck = null;

            if(isCorner)
            {
                insCheck = new InsCheckCorner(insSpot, section, isVertical, startCellHelper, sections, sp);
            }
            else
            {
                insCheck = new InsCheckOrdinary(insSpot, section, isVertical, startCellHelper, sections, sp);
            }

            return insCheck;
        }        
    }
}
