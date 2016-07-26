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

            topFlats = insSpot.insFramework.GetTopFlatsInSection(sect.Flats, isTop: true, isRight: false);
            bottomFlats = insSpot.insFramework.GetTopFlatsInSection(sect.Flats, isTop:false, isRight:false);

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

        /// <summary>
        /// Требования инсоляции удовлетворены
        /// Сумма остатка требуемых помещений равна 0
        /// </summary>
        /// <param name="requires">требования инсоляции</param>        
        protected bool RequirementsIsEmpty (List<InsRequired> requires)
        {
            var balance = requires.Sum(s => Math.Floor(s.CountLighting));
            var res = balance <= 0;
            return res;
        }


        /// <summary>
        /// Проверка правила инсоляции
        /// </summary>
        /// <param name="requires">требования инсоляции</param>
        /// <param name="light">индексы освещенности квартиры</param>
        /// <param name="ins">инсоляция по матрице</param>
        /// <param name="step">шаг в секции до этой квартиры</param>
        protected void CheckLighting (ref List<InsRequired> requires, List<int> light, string[] ins, int step)
        {
            if (light == null || ins == null || requires.Sum(r => r.CountLighting) <= 0) return;

            foreach (var item in light)
            {
                if (item.Equals(0)) break;
                double countLigth = 1;

                int lightIndexInFlat;
                if (item > 0)
                {
                    lightIndexInFlat = item - 1;
                }
                else
                {
                    // несколько окон в одном помещении в квартире (для инсоляции считается только одно окно в одном помещении)
                    lightIndexInFlat = -item - 1;
                    countLigth = 0.5;
                }

                string insIndexProject = ins[step + lightIndexInFlat];

                CalcRequire(ref requires, countLigth, insIndexProject);
            }
        }

        protected static void CalcRequire (ref List<InsRequired> requires, double countLigth, string insIndexProject)
        {
            if (!string.IsNullOrWhiteSpace(insIndexProject))
            {
                for (int i = 0; i < requires.Count; i++)
                {
                    var require = requires[i];
                    if (require.CountLighting > 0 && require.IsPassed(insIndexProject))
                    {
                        require.CountLighting -= countLigth;
                        requires[i] = require;
                    }
                }
            }
        }

        protected bool IsFirstFlatInSide ()
        {
            return curFlatIndex == 0;
        }

        protected bool IsLastFlatInSide ()
        {
            return curFlatIndex == curSideFlats.Count - 1;
        }
    }
}