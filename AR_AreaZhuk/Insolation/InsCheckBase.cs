﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    abstract class InsCheckBase : IInsCheck
    {
        protected InsolationFrameWork insFramework = new InsolationFrameWork();
        internal readonly InsolationSpot insSpot;
        internal readonly Section section;

        internal SectionInformation sectionInfo;
        internal bool isRightOrTopLLu;

        internal readonly bool isVertical;

        internal readonly int indexRowStart;
        internal readonly int indexColumnStart;       
        
        protected List<RoomInfo> topFlats;
        protected List<RoomInfo> bottomFlats;      

        public InsCheckBase (InsolationSpot insSpot, Section section, bool isVertical, int indexRowStart, int indexColumnStart)
        {
            this.section = section;
            this.insSpot = insSpot;
            this.isVertical = isVertical;
            this.indexRowStart = indexRowStart;
            this.indexColumnStart = indexColumnStart;
        }

        public virtual bool CheckSection (SectionInformation sect, bool isRightOrTopLLu)
        {
            this.isRightOrTopLLu = isRightOrTopLLu;
            sectionInfo = sect;
            topFlats = insFramework.GetTopFlatsInSection(sect.Flats, true, false);
            bottomFlats = insFramework.GetTopFlatsInSection(sect.Flats, false, false);
            return true;
        }
    }
}