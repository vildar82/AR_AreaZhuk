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
                insOtherSide = cellInsCur.InsBot.Reverse().ToArray(); 
            }
            else
            {                
                insCurSide = cellInsCur.InsBot;
                // У нижних квартир не нужно проверять верх, т.к. верха у них быть не может                
            }

            for (int i = 0; i < curSideFlats.Count; i++)
            {
                flat = curSideFlats[i];
                curFlatIndex = i;
                specialFail = false;
                bool flatPassed = false;
                string lightingCurSide = null;
                string lightingOtherSide = null;
                isFirstFlatInSide = IsFirstFlatInSide();
                isLastFlatInSide = IsLastFlatInSide();

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

                if (!flatPassed || specialFail)
                {
                    // квартира не прошла инсоляцию - вся секция не проходит
                    return false;
                }
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
            var countBalance = requires.Sum(s => Math.Ceiling(s.CountLighting)); 
            var res = countBalance <= 0;            
            return res;            
        }        

        

        /// <summary>
        /// Проверка инсоляции боковин
        /// </summary>        
        private void CheckLightingSide (ref List<InsRequired> requires)
        {
            // Если это не крайняя квартира на стороне, то точно боковой инсоляции нет.
            // И если не задана боковая инсоляция для квартиры
            // И если не задана боковая инсоляция в пятне инсоляции
            bool isSideFlat = isFirstFlatInSide || isLastFlatInSide;
            bool flatHasSide = flatLightIndexSideCurSide.Count != 0 || flatLightIndexSideOtherSide.Count != 0;            
            if (!isSideFlat || !flatHasSide || !cellInsCur.HasSideIns || requires.Sum(r=>r.CountLighting)<=0)
            {
                return;
            }

            if(isTop)
            {
                // Правая сторона
                if (isFirstFlatInSide)
                {
                    // Верхняя правая боковая ячейка  (InsSideTopRight)
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideTopRight) && flatLightIndexSideCurSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexSideCurSide[0], cellInsStandart.InsSideTopRight);
                    }
                    // Нижняя правая боковая ячейка (InsSideBotRight)
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideBotRight) && flatLightIndexOtherSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexOtherSide[0], cellInsStandart.InsSideBotRight);
                    }
                }           
                // Левая сторона     
                else if (isLastFlatInSide)
                {
                    // Верхняя левая боковая ячейка 
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideTopLeft) && flatLightIndexSideCurSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexSideCurSide[0], cellInsStandart.InsSideTopLeft);
                    }
                    // Нижняя левая боковая ячейка 
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideBotLeft) && flatLightIndexOtherSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexOtherSide[0], cellInsStandart.InsSideBotLeft);
                    }
                }                
            }
            else
            {
                if (isFirstFlatInSide)
                {
                    // Левая нижняя ячейка боковой инсоляции                    
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideBotRight) && flatLightIndexSideCurSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexSideCurSide[0], cellInsStandart.InsSideBotRight);
                    }
                }                
                else if (isLastFlatInSide)
                {
                    // Верхняя левая боковая ячейка 
                    if (!string.IsNullOrEmpty(cellInsStandart.InsSideTopLeft) && flatLightIndexSideCurSide.Count == 1)
                    {
                        CalcRequire(ref requires, flatLightIndexSideCurSide[0], cellInsStandart.InsSideTopLeft);
                    }
                }
            }
        }

        
    }
}
