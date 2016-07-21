using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    class InsCheckCorner : InsCheckBase
    {
        public InsCheckCorner (InsolationSpot insSpot, Section section, bool isVertical, int indexRowStart, int indexColumnStart)
            : base(insSpot, section, isVertical, indexRowStart, indexColumnStart)
        {
        }
    }
}
