using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Insolation
{
    /// <summary>
    /// Проверка инсоляции рядовой секции
    /// </summary>
    class InsCheckOrdinary : InsCheckBase
    {
        readonly string[] insTopStandart;
        readonly string[] insBotStandart;
        readonly string[] insTopInvert;
        readonly string[] insBotInvert;
        readonly string insSideTopStandart;
        readonly string insSideBotStandart;

        List<int> flatLightIndexCurSide;
        List<int> flatLightIndexOtherSide;
        List<int> flatLightIndexSideCurSide;
        List<int> flatLightIndexSideOtherSide;
        string[] insCurSide;
        string[] insOtherSide;
        string insSideTop;
        string insSideBot;
        bool isFirstFlatInSide;
        bool isLastFlatInSide;
        Side flatEndSide;

        public InsCheckOrdinary (IInsolation insService, Section section) 
            : base(insService, section)
        {
            insTopStandart = section.InsTop.Select(m => m.InsValue).ToArray();
            insBotStandart = section.InsBot.Select(m => m.InsValue).ToArray();            
            insTopInvert = insBotStandart;
            insBotInvert = insTopStandart;
            if (section.InsSide!= null)
            {
                insSideTopStandart = section.Direction > 0 ? section.InsSide[0].InsValue: section.InsSide[1].InsValue;
                insSideBotStandart = section.Direction > 0 ? section.InsSide[1].InsValue : section.InsSide[0].InsValue;
            }
        }

        protected override bool CheckFlats ()
        {
            bool res = false;
            if (isRightOrTopLLu)
            {
                if (isTop)
                {
                    insCurSide = insTopStandart;
                    insOtherSide = insBotStandart;                    
                }
                else
                {
                    insCurSide = insBotStandart;
                    insOtherSide = insTopStandart;
                }
                insSideTop = insSideTopStandart;
                insSideBot = insSideBotStandart;
            }
            else
            {
                if (isTop)
                {
                    insCurSide = insTopInvert;
                    insOtherSide = insBotInvert;
                }
                else
                {
                    insCurSide = insBotInvert;
                    insOtherSide = insTopInvert;
                }
                insSideTop = insSideBotStandart;
                insSideBot = insSideTopStandart; 
            }

            if (isTop)
            {
                res = CheckCellIns();
            }
            else
            {
                var startStep = topFlats.Last().SelectedIndexBottom;                
                res = CheckCellIns(startStep);
            }
            return res;
        }

        private bool CheckCellIns (int startStep = 0)
        {            
            int step = startStep;            

            for (int i = 0; i < curSideFlats.Count; i++)
            {
                specialFail = false;
                flat = curSideFlats[i];
                curFlatIndex = i;                
                bool flatPassed = false;
                string lightingCurSide = null;
                string lightingOtherSide = null;
                isFirstFlatInSide = IsEndFirstFlatInSide();
                isLastFlatInSide = IsEndLastFlatInSide();

                if (flat.SubZone == "0")
                {
                    // без правил инсоляции может быть ЛЛУ
                    flatPassed = true;
                }
                else
                {
                    if (isTop)
                    {
                        lightingCurSide = flat.LightingTop;
                        lightingOtherSide = flat.LightingNiz;
                    }
                    else
                    {
                        lightingCurSide = flat.LightingNiz;
                    }

                    flatLightIndexCurSide = LightingStringParser.GetLightings(lightingCurSide, 
                                out flatLightIndexSideCurSide, isTop, out flatEndSide);
                    flatLightIndexOtherSide = null;
                    flatLightIndexSideOtherSide = null;
                    // Для верхних крайних верхних квартир нужно проверить низ
                    if (isTop)
                    {
                        if (lightingOtherSide != null && (isFirstFlatInSide || isLastFlatInSide))
                        {
                            flatLightIndexOtherSide = LightingStringParser.GetLightings(lightingOtherSide, 
                                out flatLightIndexSideOtherSide, false, out flatEndSide);
                        }
                    }

                    var ruleInsFlat = insService.FindRule(flat);
                    if (ruleInsFlat == null)
                    {
                        // Атас, квартира не ЛЛУ, но без правил инсоляции
                        throw new Exception("Не определено правило инсоляции для квартиры - " + flat.Type);
                    }

                    foreach (var rule in ruleInsFlat.Rules)
                    {
                        if (CheckRule(rule, step))
                        {
                            // Правило удовлетворено, оставшиеся правила можно не проверять
                            // Евартира проходит инсоляцию
                            flatPassed = true;
                            break;
                        }
                    }
                }
#if TEST
                flat.IsInsPassed = flatPassed;
                if (specialFail)
                {
                    flat.IsInsPassed = false;
                }
                //Trace.WriteLine(checkSection.IdSection + "_flat=" + flat.Type + ", flatPassed=" + flatPassed + ", specialFail=" + specialFail + ", isTop=" + isTop);
#else
                if (!flatPassed || specialFail)
                {
                    // квартира не прошла инсоляцию - вся секция не проходит                    
                    return false;
                }                
#endif
                // Сдвиг шага
                step += isTop ? flat.SelectedIndexTop : flat.SelectedIndexBottom;
            }
            // Все квартиры прошли инсоляцию
            return true;
        }

        private bool CheckRule (InsRule rule, int step)
        {
            // подходящие окна в квартиирах будут вычитаться из требований
            var requires = rule.Requirements.ToList();                       

            // Проверка окон с этой строны
            isCurSide = true;
            CheckLighting(ref requires, flatLightIndexCurSide, insCurSide, step);           

            // Проверка окон с другой стороны
            isCurSide = false;
            CheckLighting(ref requires, flatLightIndexOtherSide, insOtherSide, step);

            // проверка боковин            
            CheckLightingSide(ref requires);

            // Если все требуемые окно были вычтены, то сумма остатка будет <= 0
            // Округление вниз - от окон внутри одного помещения
            var isPassed = RequirementsIsEmpty(requires);                    
            return isPassed;            
        }   

        /// <summary>
        /// Проверка инсоляции боковин
        /// </summary>        
        private void CheckLightingSide (ref List<InsRequired> requires)
        {
            // Если это не боковая квартра по типу (не заданы боковые индексы инсоляции), то у такой квартиры не нужно проверять боковую инсоляцию
            bool flatHasSide = flatEndSide != Side.None; //flatLightIndexSideCurSide.Count != 0 || flatLightIndexSideOtherSide.Count != 0;
            if (!flatHasSide)
            {
                return;
            }

            // Квартира боковая по типу (заданы боковые индексы инсоляции)

            // Если это не крайняя квартира на стороне, то такую секцию нельзя пропускать дальше
            var endFlat = flatEndSide; //GetEndFlatSide();
            if (endFlat == Side.None)
            {
                specialFail = true;
                return;
            }
            bool isStoppor = IsStoppor();
            var endSideSection = GetSectionEndSide();
            if (endFlat != endSideSection && isStoppor)
            {
                specialFail = true;
                return;
            }

            // Если требования инсоляции уже удовлетворены, то не нужно проверять дальше
            if (RequirementsIsEmpty(requires))
            {
                return;
            }

            int flatLightingSide = 0;
            int flatLightingSideOther = 0;
            string insSideValue = null;
            string insSideOtherValue = null;

            if (endFlat == Side.Right)
            {
                // Правый торец
                if (isTop)
                {
                    // Праввая верхняя ячейка инсоляции
                    insSideValue = insSideTop;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                    // для верхних квартир проверить нижнюю ячейку инсоляции
                    insSideOtherValue = insSideBot;
                    flatLightingSideOther = flatLightIndexSideOtherSide[0];
                }
                else
                {
                    // Праввая нижняя ячейка инсоляции
                    insSideValue = insSideBot;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                }
            }
            else if (endFlat == Side.Left)
            {
                // Левый торец
                if (isTop)
                {
                    // Левая верхняя ячейка инсоляции
                    insSideValue = insSideTop;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                    // для верхних квартир проверить нижнюю ячейку инсоляции
                    insSideOtherValue = insSideBot;
                    flatLightingSideOther = flatLightIndexSideOtherSide[0];
                }
                else
                {
                    // Левая нижняя ячейка инсоляции
                    insSideValue = insSideBot;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                }
            }

            double lightWeight;
            int indexLighting = GetLightingValue(flatLightingSide, out lightWeight);
            CalcRequire(ref requires, lightWeight, insSideValue);

            indexLighting = GetLightingValue(flatLightingSideOther, out lightWeight);
            CalcRequire(ref requires, lightWeight, insSideOtherValue);
        }

        private Side GetSectionEndSide ()
        {
            Side res = Side.None;
            // Определение стороны торца секции
            if (section.IsStartSectionInHouse)
            {
                if (isRightOrTopLLu)                
                    res = section.Direction > 0 ? Side.Left : Side.Right;                                    
                else                
                    res = section.Direction > 0 ? Side.Right : Side.Left;                                    
            }
            else if (section.IsEndSectionInHouse)
            {
                if (isRightOrTopLLu)
                    res = section.Direction > 0 ? Side.Right : Side.Left;
                else
                    res = section.Direction > 0 ? Side.Left : Side.Right;
            }
            return res;
        }

        private bool IsStoppor ()
        {
            // Если боковое окно единственное в помещени, то такую квартиру нельзя ставить в глухой торец (без окна с торца на улицу)
            // Если сторона квартиры не соответствует стороне торца, такую секцию нельзя пропускать дальше 
            // Только если индекс боковины не половинчатый - если не половинчатый, то боковое окно - будет заткнуто торцом и в комнате не останется окон
            var res = (flatLightIndexSideCurSide!=null && flatLightIndexSideCurSide.Count != 0 && flatLightIndexSideCurSide[0] == 1);
            if (res) return res;
            res = (flatLightIndexSideOtherSide!=null && flatLightIndexSideOtherSide.Count != 0 && flatLightIndexSideOtherSide[0] == 1);
            return res;
        }

        ///// <summary>
        ///// Определение с какого торца секции расположена квартира
        ///// </summary>        
        //private EnumEndSide GetEndFlatSide ()
        //{
        //    EnumEndSide res = EnumEndSide.None;
        //    if (isFirstFlatInSide)
        //    {
        //        if (isTop)
        //        {
        //            res = EnumEndSide.Right;
        //        }
        //        else
        //        {
        //            res = EnumEndSide.Left;
        //        }
        //    }
        //    else if (isLastFlatInSide)
        //    {
        //        if (isTop)
        //        {
        //            res = EnumEndSide.Left;
        //        }
        //        else
        //        {
        //            res = EnumEndSide.Right;
        //        }
        //    }            
        //    return res;
        //}
    }
}