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

        private bool CheckRule (string rule, int[] lightCurSide, int[] lightOtherSide, 
            string[] insCurSide, string[] insOtherSide, int step)
        {
            string[] masRule = rule.Split('=', '|');

            var intLightings = CheckLighting(masRule, lightCurSide, insCurSide, step);
            intLightings += CheckLighting(masRule, lightOtherSide, insOtherSide, step);

            int lightingRequirement = Convert.ToInt16(masRule[0]);
            var res = intLightings >= lightingRequirement;
            return res;            
        }

        private int CheckLighting (string[] masRule, int[] light, string [] ins, int step)
        {
            double countValidCell = 0;           
            
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
                    countLigth = 0.5; // Условие или (одно из окон в одном помещении)
                }

                var insValue = ins[step + lightIndexInFlat];

                if (!string.IsNullOrWhiteSpace(insValue))
                {
                    if (masRule[1].Equals(insValue, StringComparison.OrdinalIgnoreCase))
                    {
                        countValidCell += countLigth;
                    }
                }
            }
            var res = Convert.ToInt32(countValidCell);
            return res;
        }                
    }
}
