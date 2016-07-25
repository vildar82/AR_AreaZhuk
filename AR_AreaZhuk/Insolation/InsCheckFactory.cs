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
            bool isVertical, Cell cellStart,  List<FlatInfo> sections, SpotInfo sp)
        {
            IInsCheck insCheck = null;

            if(isCorner)
            {
                insCheck = new InsCheckCorner(insSpot, section, isVertical, cellStart, sections, sp);
            }
            else
            {
                insCheck = new InsCheckOrdinary(insSpot, section, isVertical, cellStart, sections, sp);
            }

            return insCheck;
        }        
    }
}
