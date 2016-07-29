using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Проверка инсоляции угловой секции
    /// </summary>
    class InsCheckCorner : InsCheckBase
    {
        CellInsCorner cellIns;
        int indexBot =0;      

        public InsCheckCorner (InsolationSpot insSpot, Section section,
            StartCellHelper startCellHelper, List<FlatInfo> sections, SpotInfo sp)
            : base(insSpot, section, startCellHelper, sections, sp)
        {
            cellIns = new CellInsCorner(this);
            cellIns.DefineIns(); 
        }

        protected override bool CheckFlats ()
        {
            bool res = false;
            if (isTop)
            {
                curSideFlats = topFlats;
                res = CheckSideFlats(cellIns.InsTop);
            }
            else
            {
                curSideFlats = bottomFlats;
                res = CheckSideFlats(cellIns.InsBot);
            }
            return res;
        }

        /// <summary>
        /// Проверка инсоляции верхних квартир
        /// </summary>
        private bool CheckSideFlats (string[] ins)
        {
            int step = isTop ? 0 : indexBot;
            for (int i = 0; i < curSideFlats.Count; i++)
            {
                flat = curSideFlats[i];
                curFlatIndex = i;                
                bool flatPassed = false;

                if (flat.SubZone == "0")
                {
                    // ЛЛУ
                    continue;
                }

                string lightingFlat = isTop ? flat.LightingTop : flat.LightingNiz;                

                List<int> sideLighting;
                Side flatEndSide;
                var lightingFlatIndexes = LightingStringParser.GetLightings(lightingFlat, out sideLighting, isTop, out flatEndSide);

                var ruleInsFlat = insSpot.FindRule(flat);
                if (ruleInsFlat == null)
                {
                    // Атас, квартира не ЛЛУ, но без правил инсоляции
                    throw new Exception("Не определено правило инсоляции для квартиры - " + flat.Type);
                }

                foreach (var rule in ruleInsFlat.Rules)
                {
                    // подходящие окна в квартиирах будут вычитаться из требований
                    var requires = rule.Requirements.ToList();

                    CheckLighting(ref requires, lightingFlatIndexes, ins, step);

                    // Для верхних квартир проверить низ
                    if (isTop)
                    {
                        if (IsEndFirstFlatInSide())
                        {
                            // проверка низа для первой верхней квартиры
                            Side end;
                            var flatLightIndexBot = LightingStringParser.GetLightings(flat.LightingNiz, out sideLighting, true, out end);
                            CheckLighting(ref requires, flatLightIndexBot, cellIns.InsBot.Reverse().ToArray(), 0);
                        }
                        // Для последней - проверка низа
                        else if (IsEndLastFlatInSide())
                        {
                            Side end;
                            var flatLightIndexBot = LightingStringParser.GetLightings(flat.LightingNiz, out sideLighting, false, out end);
                            CheckLighting(ref requires, flatLightIndexBot, cellIns.InsBot, 0);
                            // начальный отступ шагов для проверки нижних квартир
                            indexBot = flat.SelectedIndexBottom;
                        }
                    }

                    // Если все требуемые окно были вычтены, то сумма остатка будет <= 0
                    // Округление вниз - от окон внутри одного помещения
                    flatPassed = RequirementsIsEmpty(requires);                    
                    if (flatPassed)
                    {
                        break;
                    }
                }
#if TEST
                flat.IsInsPassed = flatPassed;
#else
                if (!flatPassed)
                {
                    // квартира не прошла инсоляцию - вся секция не проходит
                    return false;
                }               
#endif
                step += isTop ? flat.SelectedIndexTop : flat.SelectedIndexBottom;
            }
            return true;
        }  
    }
}
