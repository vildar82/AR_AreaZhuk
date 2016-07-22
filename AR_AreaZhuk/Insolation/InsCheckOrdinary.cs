using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Insolation
{
    class InsCheckOrdinary : InsCheckBase
    {        
        CellInsOrdinary cellInsStandart;
        CellInsOrdinary cellInsInvert;

        public InsCheckOrdinary (InsolationSpot insSpot,Section section, bool isVertical, int indexRowStart, int indexColumnStart) 
            : base(insSpot, section, isVertical, indexRowStart, indexColumnStart)
        {            
            // Данные по инсоляции секции в стандартном ее положении            
            cellInsStandart = new CellInsOrdinary(this);
            cellInsStandart.DefineIns();
            cellInsInvert = new CellInsOrdinary(this);
            cellInsInvert.InsTop = cellInsStandart.InsBot.Reverse().ToArray();
            cellInsInvert.InsBot = cellInsStandart.InsTop.Reverse().ToArray();            
        }

        public override bool CheckSection (FlatInfo sect,bool isRightOrTopLLu)
        {
            bool res = false;     
            base.CheckSection(sect, isRightOrTopLLu);

            // !!!??? Может быть мало квартир в секции?            
            if (sect.Flats.Count <= 3)
            {
                Debug.Assert(false, "Меньше 3 квартир в секции.");
                return false;
            }

            topFlats = insFramework.GetTopFlatsInSection(sect.Flats, true, false);
            bottomFlats = insFramework.GetTopFlatsInSection(sect.Flats, false, false);

            CellInsOrdinary cellIns;
            if (isRightOrTopLLu)
            {
                cellIns = cellInsStandart;
            }
            else
            {
                cellIns = cellInsInvert;
            }

            // Проверка инсоляции квартир сверху
            res = CheckFlats(topFlats, cellIns, isTop: true);
            if (res) // прошла инсоляция верхних квартир
            {
                // Проверка инсоляции квартир снизу
                // отступ шагов снизу от последней верхней квартиры
                var startStep = topFlats.Last().SelectedIndexBottom; 
                res = CheckFlats(bottomFlats, cellIns, isTop: false, startStep: startStep);
            }            
            return res;
        }

        private bool CheckFlats (List<RoomInfo> flatsSide, CellInsOrdinary cellIns,  bool isTop, int startStep = 0)
        {            
            int step = startStep;

            string[] insCurSide = null;
            string[] insOtherSide = null;

            if (isTop)
            {                
                insCurSide = cellIns.InsTop;
                insOtherSide = cellIns.InsBot.Reverse().ToArray(); 
            }
            else
            {                
                insCurSide = cellIns.InsBot;
                //insOtherSide = cellIns.InsTop.Reverse().ToArray();
            }

            foreach (var flat in flatsSide)
            {
                bool flatPassed = false;
                string lightingCurSide = null;
                string lightingOtherSide = null;                
                if (isTop)
                {
                    lightingCurSide = flat.LightingTop;
                    lightingOtherSide = flat.LightingNiz;                    
                }
                else
                {
                    lightingCurSide = flat.LightingNiz;
                    //lightingOtherSide = flat.LightingTop;                    
                }
                var lightCurSide = insFramework.GetLightingPosition(lightingCurSide, flat, sectionInfo.Flats);
                var lightOtherSide = insFramework.GetLightingPosition(lightingOtherSide, flat, sectionInfo.Flats);

                var rule = insSpot.FindRule(flat);

                if (rule == null)
                {
                    // без правил инсоляции может быть ЛЛУ
                    flatPassed = true;
                }
                else
                {
                    foreach (var ruleName in rule.Rules)
                    {
                        if (CheckRule(ruleName, lightCurSide, lightOtherSide, insCurSide, insOtherSide, step))
                        {
                            // Правило удовлетворено
                            flatPassed = true;
                            break;
                        }
                    }
                }

                if (!flatPassed)
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

        private bool CheckRule (InsRule rule, int[] lightCurSide, int[] lightOtherSide, 
            string[] insCurSide, string[] insOtherSide, int step)
        {
            var requires = rule.Requirements.ToList();

            // подходящие окна в квартиирах будут вычитаться из требований
            CheckLighting(ref requires, lightCurSide, insCurSide, step);
            CheckLighting(ref requires, lightOtherSide, insOtherSide, step);

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
                    // несколько окон в одном помещении в квартире (считается только одно окно в одном помещении)
                    lightIndexInFlat = (-item) - 1;
                    countLigth = 0.5; 
                }

                var insIndexProject = ins[step + lightIndexInFlat];

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
    }
}
