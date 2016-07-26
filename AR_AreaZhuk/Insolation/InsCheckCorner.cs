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
                 res = CheckSideFlats(cellIns.InsTop);
            }
            else
            {
                res = CheckSideFlats(cellIns.InsBot);
            }
            return res;
        }        

        /// <summary>
        /// Проверка инсоляции верхних квартир
        /// </summary>
        private bool CheckSideFlats (string[] ins)
        {
            int step = 0;            
            for (int i = 0; i < curSideFlats.Count; i++)
            {
                flat = curSideFlats[i];
                curFlatIndex = i;
                specialFail = false;
                bool flatPassed = false;

                if (flat.SubZone == "0")
                {
                    // ЛЛУ
                    continue;
                }

                string lightingFlat = isTop ? flat.LightingTop : flat.LightingNiz;
                List<int> sideLighting;
                var lightingFlatIndexes = LightingStringParser.GetLightings(lightingFlat, out sideLighting);

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
                    // Для первой квартиры проверить низ
                    if  (isTop && curFlatIndex==0)
                    {
                        var flatLightIndexBot = LightingStringParser.GetLightings(flat.LightingNiz, out sideLighting);
                        CheckLighting(ref requires, flatLightIndexBot, cellIns.InsBot, 0);
                        indexBot = flat.SelectedIndexBottom;
                    }

                    // Если все требуемые окно были вычтены, то сумма остатка будет <= 0
                    // Округление вниз - от окон внутри одного помещения
                    var countBalance = requires.Sum(s => Math.Ceiling(s.CountLighting));
                    flatPassed = countBalance <= 0;
                }

                if (!flatPassed || specialFail)
                {
                    // квартира не прошла инсоляцию - вся секция не проходит
                    return false;
                }
                step += isTop ? flat.SelectedIndexTop : flat.SelectedIndexBottom;
            }
            return true;
        }               

        private void CheckLighting (ref List<InsRequired> requires, List<int> light, string[] ins, int step)
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
                
                // Нужно проверить торец секции
                // При условии, что угловая секция может быть последней!!!???
                int indexStepLight = step + lightIndexInFlat;
                string insIndexProject = ins[indexStepLight];

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

        private bool CheckSideFlat (int step, int lightIndexInFlat, string[] ins, out string insIndexProject)
        {
            throw new NotImplementedException();
        }
    }
}
