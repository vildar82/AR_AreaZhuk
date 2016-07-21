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
            bool isVertical, int indexRowStart, int indexColumnStart)
        {
            IInsCheck insCheck = null;

            if(isCorner)
            {
                insCheck = new InsCheckCorner(insSpot, section, isVertical, indexRowStart, indexColumnStart);
            }
            else
            {
                insCheck = new InsCheckOrdinary(insSpot, section, isVertical, indexRowStart, indexColumnStart);
            }

            return insCheck;
        }        
    }
}
