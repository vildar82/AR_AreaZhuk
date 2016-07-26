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
        internal readonly InsolationSpot insSpot;
        internal readonly Section section;
        internal List<FlatInfo> sections;
        internal SpotInfo sp;

        internal FlatInfo checkSection;
        internal bool isRightOrTopLLu;

        internal readonly bool isVertical;

        internal readonly StartCellHelper startCellHelper;        

        protected List<RoomInfo> topFlats;
        protected List<RoomInfo> bottomFlats;

        // Текущие проверяемые значения
        protected RoomInfo flat;        
        protected bool isTop;
        protected bool isCurSide;
        protected int curFlatIndex;
        protected List<RoomInfo> curSideFlats;
        protected bool specialFail; // Спец. условия не прохождения инсоляции - например торцевая квартира в средней секции

        protected abstract bool CheckFlats ();

        public InsCheckBase (InsolationSpot insSpot, Section section, 
            StartCellHelper startCellHelper, List<FlatInfo> sections, SpotInfo sp)
        {
            this.section = section;
            this.sections = sections;
            this.sp = sp;
            this.insSpot = insSpot;
            this.isVertical = section.IsVertical;
            this.startCellHelper = startCellHelper;            
        }

        public bool CheckSection (FlatInfo sect, bool isRightOrTopLLu)
        {
            bool res = false;
            this.isRightOrTopLLu = isRightOrTopLLu;
            checkSection = sect;

            // !!!??? Может быть мало квартир в секции?            
            if (sect.Flats.Count <= 3)
            {
                Debug.Assert(false, "Меньше 3 квартир в секции.");
                return false;
            }

            var topFlats = insSpot.insFramework.GetTopFlatsInSection(sect.Flats, isTop: true, isRight: false);
            var bottomFlats = insSpot.insFramework.GetTopFlatsInSection(sect.Flats, isTop:false, isRight:false);

            // Проверка инсоляции квартир сверху
            isTop = true;
            curSideFlats = topFlats;
            res = CheckFlats();
            if (res) // прошла инсоляция верхних квартир
            {
                // Проверка инсоляции квартир снизу                
                isTop = false;
                curSideFlats = bottomFlats;
                res = CheckFlats();
            }
            return res;
        }

        /// <summary>
        /// Проверка - это концевая секция (1 или последняя)
        /// </summary>        
        public bool IsEndSection ()
        {
            var res = section.NumberInSpot == 1 ||
                section.NumberInSpot == sp.TotalSections;
            return res;
        }
    }
}