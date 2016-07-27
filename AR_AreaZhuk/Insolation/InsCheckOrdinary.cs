using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Проверка инсоляции рядовой секции
    /// </summary>
    class InsCheckOrdinary : InsCheckBase
    {
        CellInsOrdinary cellInsStandart;
        CellInsOrdinary cellInsInvert;
        CellInsOrdinary cellInsCur;

        List<int> flatLightIndexCurSide;
        List<int> flatLightIndexOtherSide;
        List<int> flatLightIndexSideCurSide;
        List<int> flatLightIndexSideOtherSide;
        string[] insCurSide;
        string[] insOtherSide;
        bool isFirstFlatInSide;
        bool isLastFlatInSide;

        public InsCheckOrdinary (InsolationSpot insSpot,Section section,
            StartCellHelper startCellHelper, List<FlatInfo> sections, SpotInfo sp) 
            : base(insSpot, section, startCellHelper, sections, sp)
        {            
            // Данные по инсоляции секции в стандартном ее положении            
            cellInsStandart = new CellInsOrdinary(this);
            cellInsStandart.DefineIns();
            cellInsInvert = cellInsStandart.Invert();            
        }

        protected override bool CheckFlats ()
        {
            bool res = false;
            if (isRightOrTopLLu)
            {
                cellInsCur = cellInsStandart;
            }
            else
            {
                cellInsCur = cellInsInvert;
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

            insCurSide = null;
            insOtherSide = null;

            if (isTop)
            {
                insCurSide = cellInsCur.InsTop;
                insOtherSide = cellInsCur.InsBot.Reverse().ToArray(); // У нижних квартир не нужно проверять другую сторону                       
            }
            else
            {
                insCurSide = cellInsCur.InsBot;                
            }            

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

                    // Временно - подмена индекса освещенностим для боковых квартир!!!???
                    var sideFlat = SideFlatFake.GetSideFlat(flat.Type);
                    if (sideFlat != null)
                    {
                        lightingCurSide = sideFlat.LightingStringWithB;
                    }

                    flatLightIndexCurSide = LightingStringParser.GetLightings(lightingCurSide, out flatLightIndexSideCurSide);
                    flatLightIndexOtherSide = null;
                    // Для верхних крайних верхних квартир нужно проверить низ
                    if (isTop)
                    {
                        if (lightingOtherSide != null && (isFirstFlatInSide || isLastFlatInSide))
                        {
                            flatLightIndexOtherSide = LightingStringParser.GetLightings(lightingOtherSide, out flatLightIndexSideOtherSide);
                        }
                    }

                    var ruleInsFlat = insSpot.FindRule(flat);
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
            bool flatHasSide = flatLightIndexSideCurSide.Count != 0 || flatLightIndexSideOtherSide.Count != 0;
            if (!flatHasSide)
            {
                return;
            }

            // Квартира боковая по типу (заданы боковые индексы инсоляции)

            // Если это не крайняя квартира на стороне, то такую секцию нельзя пропускать дальше
            var endFlat = GetEndFlatSide();
            if (endFlat == EnumEndSide.None)
            {
                specialFail = true;
                return;
            }

            // Если сторона квартиры не соответствует стороне торца, такую секцию нельзя пропускать дальше 
            if (endFlat != cellInsCur.EndSide)
            {
                specialFail = true;
                return;
            }            

            // Если требования инсоляции уже удовлетворены, то не нужно проверять дальше
            if (RequirementsIsEmpty(requires))
            {
                return;
            }

            int flatLightingSide =0;
            int flatLightingSideOther =0;
            string insSideValue = null;
            string insSideOtherValue = null;

            if (endFlat == EnumEndSide.Right)
            {
                // Правый торец
                if (isTop)
                {
                    // Праввая верхняя ячейка инсоляции
                    insSideValue = cellInsCur.InsSideTopRight;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                    // для верхних квартир проверить нижнюю ячейку инсоляции
                    insSideOtherValue = cellInsCur.InsSideBotRight;
                    flatLightingSideOther= flatLightIndexSideOtherSide[0];
                }
                else
                {
                    // Праввая нижняя ячейка инсоляции
                    insSideValue = cellInsCur.InsSideBotRight;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                }
            }
            else if (endFlat == EnumEndSide.Left)
            {
                // Левый торец
                if (isTop)
                {
                    // Левая верхняя ячейка инсоляции
                    insSideValue = cellInsCur.InsSideTopLeft;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                    // для верхних квартир проверить нижнюю ячейку инсоляции
                    insSideOtherValue = cellInsCur.InsSideBotLeft;
                    flatLightingSideOther = flatLightIndexSideOtherSide[0];
                }
                else
                {
                    // Левая нижняя ячейка инсоляции
                    insSideValue = cellInsCur.InsSideBotLeft;
                    flatLightingSide = flatLightIndexSideCurSide[0];
                }
            }
            
            double lightWeight;
            int indexLighting = GetLightingValue(flatLightingSide, out lightWeight);
            CalcRequire(ref requires, lightWeight, insSideValue);
            
            indexLighting = GetLightingValue(flatLightingSideOther, out lightWeight);
            CalcRequire(ref requires, lightWeight, insSideOtherValue);
        }

        /// <summary>
        /// Определение с какого торца секции расположена квартира
        /// </summary>        
        private EnumEndSide GetEndFlatSide ()
        {
            EnumEndSide res = EnumEndSide.None;
            if (isFirstFlatInSide)
            {
                if (isTop)
                {
                    res = EnumEndSide.Right;
                }
                else
                {
                    res = EnumEndSide.Left;
                }
            }
            else if (isLastFlatInSide)
            {
                if (isTop)
                {
                    res = EnumEndSide.Left;
                }
                else
                {
                    res = EnumEndSide.Right;
                }
            }            
            return res;
        }
    }
}
