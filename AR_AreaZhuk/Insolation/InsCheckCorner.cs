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

        public override bool CheckSection (FlatInfo sect, bool isRightOrTopLLu)
        {
            bool resCheck = false;
            base.CheckSection(sect, isRightOrTopLLu);

            // Определить это правая или левая угловая секция            
            bool isRight = true;
            //topFlats = insFramework.GetTopFlatsInSection(sect.Flats, true, isRight);
            //bottomFlats = insFramework.GetTopFlatsInSection(sect.Flats, false, isRight);
            resCheck = true;

            return resCheck;
        }
    }
}
