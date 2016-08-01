using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Scheme.Cutting
{
    class CuttingOrdinary : ICutting
    {
        public static readonly List<int> SectionSteps = new List<int> { 6, 7, 8, 9, 10, 11, 12, 13, 14 };

        private HouseSpot houseSpot;

        public CuttingOrdinary (HouseSpot houseSpot)
        {
            this.houseSpot = houseSpot;
        }

        public void Cut()
        {
            // Все варианты домов по шагам секций
            var housesSteps = GetAllSteps();
            // Подстановка секций под каждый вариант
            foreach (var houseSteps in housesSteps)
            {
                var houseVar = GetHouseVariant(houseSteps);
            }
        }

        public static string GetSectionFloors(int countFloors)
        {
            string floors = "10-18";
            if (countFloors > 18 & countFloors <= 25)
                floors = "19-25";            
            if (countFloors < 9)
                floors = "9";
            return floors;
        }        

        public static string GetSectionType(SectionType sectionType)
        {
            switch (sectionType)
            {
                case SectionType.Ordinary:
                    return "Рядовая";
                case SectionType.CornerLeft:
                    return "Угловая лево";
                case SectionType.CornerRight:
                    return "Угловая право";
                case SectionType.Tower:
                    return "Башня";
            }
            return null;            
        }

        private List<int[]> GetAllSteps ()
        {            
            int houseSteps = 47;
            int sectMinStep = SectionSteps[0];
            int maxSectionsInHouse = houseSteps / sectMinStep;            
            int[] selectedSectionsStep = new int[maxSectionsInHouse];

            List<int[]> houses = new List<int[]>();

            bool isContinue = true;
            while (isContinue)
            {
                int countStepRest = houseSteps;
                for (int i = 0; i < maxSectionsInHouse; i++)
                {
                    countStepRest = countStepRest - SectionSteps[selectedSectionsStep[i]];
                    if (countStepRest == 0)
                    {
                        List<int> selSectSteps = new List<int>();
                        for (int k = 0; k <= i; k++)
                        {
                            selSectSteps.Add(selectedSectionsStep[k]);
                        }
                        houses.Add(selSectSteps.ToArray());

                        selectedSectionsStep[i]++;
                        if (selectedSectionsStep[i] >= SectionSteps.Count)
                        {
                            if (!SetIndexesSize(ref selectedSectionsStep, i, SectionSteps))
                                isContinue = false;
                        }
                        break;
                    }
                    else if (countStepRest < sectMinStep)
                    {
                        selectedSectionsStep[i]++;
                        if (selectedSectionsStep[i] >= SectionSteps.Count)
                        {
                            if (!SetIndexesSize(ref selectedSectionsStep, i, SectionSteps))
                                isContinue = false;
                        }
                        break;
                    }
                }
            }
            return houses;
        }

        public bool SetIndexesSize (ref int[] indexes, int index, List<int> masSizes)
        {
            bool res = true;
            if (index == 0)
            {
                return false;
            }
            indexes[index] = 0;
            indexes[index - 1]++;

            if (indexes[index - 1] >= masSizes.Count)
            {
                res = SetIndexesSize(ref indexes, index - 1, masSizes);
            }
            return res;
        }

        private Section GetHouseVariant (int[] houseSteps)
        {            
            Section section = null;
            int curStepInHouse = 1;
            // Перебор нарезанных секций в доме
            for (int numberSect = 0; numberSect < houseSteps.Length; numberSect++)
            {
                int sectCountStep = SectionSteps[houseSteps[numberSect]];
                section = houseSpot.GetSection(curStepInHouse, sectCountStep);

            }
            return section;
        }
    }
}