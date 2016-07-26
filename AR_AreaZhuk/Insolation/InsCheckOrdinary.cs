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

        protected int[] flatLightIndexCurSide;
        protected int[] flatLightIndexOtherSide;
        protected string[] insCurSide;
        protected string[] insOtherSide;

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

                    flatLightIndexCurSide = insSpot.insFramework.GetLightingPosition(lightingCurSide, flat, checkSection.Flats);
                    flatLightIndexOtherSide = null;
                    // Для крайних верхних квартир нужно проверить низ
                    if (lightingOtherSide != null && curFlatIndex ==0 || curFlatIndex == curSideFlats.Count-1)
                    {
                        flatLightIndexOtherSide = insSpot.insFramework.GetLightingPosition(lightingOtherSide, flat, checkSection.Flats);
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
                step += isTop? flat.SelectedIndexTop : flat.SelectedIndexBottom;                
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

            // Если все требуемые окно были вычтены, то сумма остатка будет <= 0
            // Округление вниз - от окон внутри одного помещения
            var countBalance = requires.Sum(s => Math.Ceiling(s.CountLighting)); 
            var res = countBalance <= 0;            
            return res;            
        }

        private void CheckLighting (ref List<InsRequired> requires, int[] light, string [] ins, int step)
        {
            if (light == null || ins == null) return;

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
                    lightIndexInFlat = (-item) - 1;
                    countLigth = 0.5;
                }                

                // проверка квартиры с окном выходящим на торец секции   
                string insIndexProject;
                if (!CheckSideFlat(step, lightIndexInFlat, ins, out insIndexProject))
                {                    
                    return;
                }                

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
        }

        /// <summary>
        /// Проверка боковой квартиры - у которой окна выходят на торец секции
        /// Т.е. такая квартира может быть только в торцевой секции
        /// Если такая квартира в средней секции, то такая секция не пропускается
        /// </summary>
        /// <returns>false если квартира не пропущена</returns>
        private bool CheckSideFlat (int step, int flatLightIndex, string[] ins, out string insIndexProject)
        {
            bool res = false;
            insIndexProject = null;

            var sideFlat = insSpot.SideFlats.Find(f => f.Name == flat.Type);

            // Это боковая квартира с окном на торце секции
            // и текущий индекс освещенности равен торцевому индексу в этой квартире - то проверяем боковую инсоляцию
            if (sideFlat != null &&
                sideFlat.IndexLightSide == flatLightIndex + 1)
            {
                if (isTop)
                {
                    // Верх (на стороне ЛЛУ)
                    // если это первая квартира сверху - то торец справа                    
                    if (curFlatIndex == 0)
                    {
                        // Торец справа
                        if (isCurSide)
                        {
                            // Сверху
                            insIndexProject = cellInsCur.InsSideTopRight;
                        }
                        else
                        {
                            // Снизу
                            insIndexProject = cellInsCur.InsSideBotRight;
                        }
                    }
                    else if (curFlatIndex == curSideFlats.Count-1)
                    {
                        // Торец слева
                        if (isCurSide)
                        {
                            // Сверху
                            insIndexProject = cellInsCur.InsSideTopLeft;
                        }
                        else
                        {
                            // Снизу
                            insIndexProject = cellInsCur.InsSideBotLeft;
                        }
                    }                    
                }
                else
                {
                    // Нижняя сторона от ЛЛУ
                    // если это первая квартира снизу - то торец справа
                    if (curFlatIndex == 0)
                    {
                        // Торец справа
                        if (isCurSide)
                        {
                            // Снизу
                            insIndexProject = cellInsCur.InsSideBotRight;
                        }
                        else
                        {
                            // Сверху
                            insIndexProject = cellInsCur.InsSideTopRight;
                        }
                    }
                    else if (curFlatIndex == curSideFlats.Count - 1)
                    {
                        // Торец слева
                        if (isCurSide)
                        {
                            // Снизу
                            insIndexProject = cellInsCur.InsSideBotLeft;
                        }
                        else
                        {
                            // Сверху
                            insIndexProject = cellInsCur.InsSideTopLeft;
                        }
                    }
                }

                if (string.IsNullOrEmpty(insIndexProject))
                {
                    specialFail = true;
                }
            }
            else
            {
                // определение инсоляции по текущему шагу в секции и индексу окна в квартире
                var indexStepLight = step + flatLightIndex;
                insIndexProject = ins[indexStepLight];
                res = true;
            }
            return res;
        }
    }
}
