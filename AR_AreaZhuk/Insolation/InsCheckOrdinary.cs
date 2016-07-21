using System;
using System.Collections.Generic;
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
            cellInsInvert.InsTop = cellInsStandart.InsBot;
            cellInsInvert.InsBot = cellInsStandart.InsTop;            
        }

        public override bool CheckSection (SectionInformation sect,bool isRightOrTopLLu)
        {
            bool res = false;     
            base.CheckSection(sect, isRightOrTopLLu);

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
            if (res)
            {
                // Проверка инсоляции квартир снизу
                res = CheckFlats(bottomFlats, cellIns, isTop: false);
            }            
            return res;
        }

        private bool CheckFlats (List<RoomInfo> flats, CellInsOrdinary cellIns,  bool isTop)
        {            
            int step = 0;
            foreach (var flat in flats)
            {
                string lightingCurSide;
                string lightingOtherSide;
                string[] insCurSide;
                string[] insOtherSide;
                if (isTop)
                {
                    lightingCurSide = flat.LightingTop;
                    lightingOtherSide = flat.LightingNiz;
                    insCurSide = cellIns.InsTop;
                    insOtherSide = cellIns.InsBot.Reverse().ToArray();
                }
                else
                {
                    lightingCurSide = flat.LightingNiz;
                    lightingOtherSide = flat.LightingTop;
                    insCurSide = cellIns.InsBot;
                    insOtherSide = cellIns.InsTop.Reverse().ToArray();
                }
                var lightCurSide = insFramework.GetLightingPosition(lightingCurSide, flat, sectionInfo.Flats);
                var lightOtherSide = insFramework.GetLightingPosition(lightingOtherSide, flat, sectionInfo.Flats);

                //var rule = insSpot.FindRule(flat);

                var rule = new RoomInsulation("Четырехкомнатная", 4, new List<string>() { "2=C", "2=D", "3=1C+2B" });

                if (rule != null)
                {
                    foreach (var ruleName in rule.Rules)
                    {
                        if (CheckRule(ruleName, lightCurSide, lightOtherSide, insCurSide, insOtherSide, step))
                        {
                            // Квартира прошла инсоляцию
                            return true;
                        }
                    }
                }
                // Сдвиг шага
                step += isTop? flat.SelectedIndexTop : flat.SelectedIndexBottom;                
            }
            return false;
        }        

        private bool CheckRule (InsRule rule, int[] lightCurSide, int[] lightOtherSide, 
            string[] insCurSide, string[] insOtherSide, int step)
        {
            var requires = rule.Requirements.ToList();

            // подходящие окна в квартиирах будут вычитаться из требований
            CheckLighting(ref requires, lightCurSide, insCurSide, step);
            CheckLighting(ref requires, lightOtherSide, insOtherSide, step);

            // Если все требуемые окно были вычтены, то сумма остатка будет <= 0
            var countBalance = requires.Sum(s => Math.Ceiling(s.CountLighting)); // Округление вниз - от окон внутри одного помещения
            var res = countBalance <= 0;            
            return res;            
        }

        private void CheckLighting (ref List<InsRequired> requires, int[] light, string [] ins, int step)
        {            
            foreach (var item in light)
            {
                double countLigth = 1;
                if (item.Equals(0)) break;

                int lightIndexInFlat;
                if (item > 0)
                {
                    lightIndexInFlat = item - 1;
                }
                else
                {
                    lightIndexInFlat = (-item) - 1;
                    countLigth = 0.5; // несколько окон в одном помещении в квартире (считается только одно окно в одном помещении)
                }

                var insValue = ins[step + lightIndexInFlat];

                if (!string.IsNullOrWhiteSpace(insValue))
                {
                    var require = requires.Find(f => f.InsIndex.Equals(insValue, StringComparison.OrdinalIgnoreCase));
                    if (requires != null)
                    {
                        require.CountLighting -= countLigth;
                    }
                }
            }            
        }                
    }
}
