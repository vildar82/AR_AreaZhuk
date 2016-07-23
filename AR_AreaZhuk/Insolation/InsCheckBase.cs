using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    abstract class InsCheckBase : IInsCheck
    {
        internal InsolationFrameWork insFramework = new InsolationFrameWork();
        internal readonly InsolationSpot insSpot;
        internal readonly Section section;

        internal FlatInfo checkSection;
        internal bool isRightOrTopLLu;

        internal readonly bool isVertical;

        internal readonly int indexRowStart;
        internal readonly int indexColumnStart;       

        public InsCheckBase (InsolationSpot insSpot, Section section, bool isVertical, int indexRowStart, int indexColumnStart)
        {
            this.section = section;
            this.insSpot = insSpot;
            this.isVertical = isVertical;
            this.indexRowStart = indexRowStart;
            this.indexColumnStart = indexColumnStart;
        }

        public virtual bool CheckSection (FlatInfo checkSect, bool isRightOrTopLLu)
        {
            Debug.Assert(checkSect.Flats.Any(f=>f.ShortType == "2KL2"));

            this.isRightOrTopLLu = isRightOrTopLLu;
            checkSection = checkSect;            
            return true;
        }
    }
}
