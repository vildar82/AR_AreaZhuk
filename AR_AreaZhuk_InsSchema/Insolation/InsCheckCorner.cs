using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_InsSchema.Insolation
{
    /// <summary>
    /// Проверка инсоляции угловой секции
    /// </summary>
    class InsCheckCorner : InsCheckBase
    {
        int indexBot =0;
        string[] insTop;
        string[] insBot;

        public InsCheckCorner (IInsolation insService, Section section)
            : base(insService, section)
        {
            insTop = section.InsTop.Select(s => s.InsValue).ToArray();
            insBot = section.InsBot.Select(s => s.InsValue).ToArray();
        }

        protected override bool CheckFlats ()
        {
            bool res = false;
            if (isTop)
            {
                curSideFlats = topFlats;
                res = CheckSideFlats(insTop);
            }
            else
            {
                curSideFlats = bottomFlats;
                res = CheckSideFlats(insBot);
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
                    // прыжок на ширину ЛЛУ без 1 шага (уходит внутрь угла и не занимает шаг секции)
                    step += flat.SelectedIndexTop - 1;
                    continue;
                }

                string lightingFlat = isTop ? flat.LightingTop : flat.LightingNiz;

                List<int> sideLighting;
                Side flatEndSide;
                var lightingFlatIndexes = LightingStringParser.GetLightings(lightingFlat, out sideLighting, isTop, out flatEndSide);

                var ruleInsFlat = insService.FindRule(flat);
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
                            CheckLighting(ref requires, flatLightIndexBot, insBot.Reverse().ToArray(), 0);
                        }
                        // Для последней - проверка низа
                        else if (IsEndLastFlatInSide())
                        {
                            Side end;
                            var flatLightIndexBot = LightingStringParser.GetLightings(flat.LightingNiz, out sideLighting, false, out end);
                            CheckLighting(ref requires, flatLightIndexBot, insBot, 0);
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
