using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    public static class LightingStringParser
    {
        public static List<int> GetLightings (string lightingString, out List<int> sideLightings)
        {            
            List<int> lightings = new List<int>();
            sideLightings = new List<int>();

            bool isRange = false;
            bool isLeaf = false; // окно в большой комнате с несколькими окнами

            for (int i = 0; i < lightingString.Length; i++)
            {
                char item = lightingString[i];
                if (char.IsDigit(item))
                {
                    AddLightingValue((int)char.GetNumericValue(item), lightings, isRange, isLeaf);
                    continue;                  
                }

                isRange = false;
                isLeaf = false;

                if (item == '-')
                {
                    isRange = true;
                    continue;
                }                

                if (item == '|')
                {
                    isLeaf = true;
                    // изменение знака предыдущего индекса
                    var lastLight = lightings.Last();
                    lightings[lightings.Count - 1] = lastLight * -1;
                    continue;
                }

                if (item == 'B')
                {
                    // Боковая освещенность
                    AddSideLightingValue(lightingString, sideLightings, ref i, isLeaf, isRange);
                }                              
            }

            return lightings;
        }        

        private static void AddLightingValue (int value, List<int> lightings, bool isRange, bool isLeaf)
        {
            int factorLeaf = isLeaf ? -1 : 1;
            if (isRange)
            {
                for (int i =Math.Abs(lightings.Last()) + 1; i <= value; i++)
                {
                    lightings.Add(i * factorLeaf);
                }
            }
            else
            {
                lightings.Add(value*factorLeaf);
            }
        }

        private static void AddSideLightingValue (string lightingString, List<int> sideLightings, 
            ref int iLightingString, bool isLeaf, bool isRange)
        {
            int factorLeaf = isLeaf ? -1 : 1;
            // индекс стороны
            int indexSide = GetSideIndex(ref iLightingString, lightingString);
            if (isRange)
            {
                for (int i = Math.Abs(sideLightings.Last())+1; i <= indexSide; i++)
                {
                    sideLightings.Add(i*factorLeaf);
                }
            }
            else
            {
                sideLightings.Add(indexSide*factorLeaf);
            }
        }

        private static int GetSideIndex (ref int i, string lightingString)
        {
            int resSideIndex = 1;
            if (i < lightingString.Length-1)
            {                
                var item = lightingString[i+1];
                if (char.IsDigit(item))
                {
                    resSideIndex = (int)char.GetNumericValue(item);
                    i++;
                }
            }
            return resSideIndex;
        }
    }
}
