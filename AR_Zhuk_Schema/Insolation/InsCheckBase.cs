using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Insolation
{
    abstract class InsCheckBase : IInsCheck
    {
        protected readonly IInsolation insService;
        internal readonly Section section;
        internal List<FlatInfo> sections;        

        internal FlatInfo checkSection;
        internal bool isRightOrTopLLu;
        internal readonly bool isVertical;        

        protected List<RoomInfo> topFlats;
        protected List<RoomInfo> bottomFlats;

        // Текущие проверяемые значения
        protected RoomInfo flat;        
        protected bool isTop;
        protected bool isCurSide;
        protected int curFlatIndex;
        protected List<RoomInfo> curSideFlats;
        protected bool specialFail;               

        protected abstract bool CheckFlats ();

        public InsCheckBase (IInsolation insService, Section section)
        {
            this.insService = insService;
            this.section = section;            
            this.isVertical = section.IsVertical;            
        }

        public bool CheckSection (FlatInfo sect, bool isRightOrTopLLu)
        {
            bool res = false;
            this.isRightOrTopLLu = isRightOrTopLLu;
            checkSection = sect;

            topFlats = insService.GetSideFlatsInSection(sect.Flats, isTop: true);
            bottomFlats = insService.GetSideFlatsInSection(sect.Flats, isTop: false);

            // Проверка инсоляции квартир сверху
            isTop = true;
            curSideFlats = topFlats;
            res = CheckFlats();
#if TEST
            // Проверка инсоляции квартир снизу                
            isTop = false;
            curSideFlats = bottomFlats;
            res = CheckFlats();
#else
            if (res) // прошла инсоляция верхних квартир
            {
                // Проверка инсоляции квартир снизу                
                isTop = false;
                curSideFlats = bottomFlats;
                res = CheckFlats();
            }
#endif
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

                double ligthWeight;
                int lightIndexInFlat = GetLightingValue(item, out ligthWeight);

                string insIndexProject = ins[step + lightIndexInFlat];

                CalcRequire(ref requires, ligthWeight, insIndexProject);
            }
        }

        /// <summary>
        /// Определение индекса окна инсоляции и веса инсоляции
        /// Определение веса инсоляции окна 1 - одно инсолируемое окно в помещении
        /// 0,5 - когда два окна в одном помещении
        /// </summary>
        /// <param name="item">Индекс инсоляционного окна в квартире - может быть с минусом, когда 2 окна в одном помещении</param>
        /// <param name="ligthWeight">Вес инсоляционнного окна - 1, или 0,5 если два окна в одном помещении</param>
        /// <returns>Индекс инсоляционного окна (без знака минус)</returns>
        protected static int GetLightingValue (int item, out double ligthWeight)
        {
            ligthWeight = 1;
            int lightIndexInFlat;
            if (item > 0)
            {
                lightIndexInFlat = item - 1;
            }
            else
            {
                // несколько окон в одном помещении в квартире (для инсоляции считается только одно окно в одном помещении)
                lightIndexInFlat = -item - 1;
                ligthWeight = 0.5;
            }
            return lightIndexInFlat;
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

        /// <summary>
        /// Это первая торцевая квартира, на текущей стророне
        /// </summary>        
        protected bool IsEndFirstFlatInSide ()
        {
            bool res = false;
            if (isTop)
            {
                if (curFlatIndex == 0)
                {
                    res = true;
                }
            }
            else
            {
                if (curFlatIndex == 0 && topFlats.Last().SelectedIndexBottom ==0)
                {
                    res = true;
                }
            }
            return res;
        }

        /// <summary>
        /// Это последняя торцевая квартра на стороне
        /// </summary>
        /// <returns></returns>
        protected bool IsEndLastFlatInSide ()
        {
            bool res = false;
            if (isTop)
            {
                if (curFlatIndex == curSideFlats.Count - 1)
                {
                    res = true;
                }
            }
            else
            {
                if (curFlatIndex == (curSideFlats.Count - 1) && topFlats.First().SelectedIndexBottom==0)
                {
                    res = true;
                }
            }   
            return res;
        }
    }
}