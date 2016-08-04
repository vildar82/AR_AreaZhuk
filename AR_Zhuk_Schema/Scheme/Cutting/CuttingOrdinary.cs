using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema.DB;
using AR_Zhuk_Schema.Insolation;

namespace AR_Zhuk_Schema.Scheme.Cutting
{
    class CuttingOrdinary : ICutting
    {
        public static readonly List<int> SectionSteps = new List<int> { 7, 8, 9, 10, 11, 12, 13, 14 };

        private List<string> failedSections;

        private HouseSpot houseSpot;
        private IDBService dbService;
        private IInsolation insService;
        private SpotInfo sp;
        private int maxSectionBySize;

        public CuttingOrdinary (HouseSpot houseSpot, IDBService dbService, IInsolation insService, SpotInfo sp)
        {
            this.houseSpot = houseSpot;
            this.dbService = dbService;
            this.insService = insService;
            this.sp = sp;
        }

        public List<HouseInfo> Cut (int maxSectionBySize)
        {
            this.maxSectionBySize = maxSectionBySize;
            failedSections = new List<string>();

            List<HouseInfo> resHouses = new List<HouseInfo>();
            // Все варианты домов по шагам секций
            var housesSteps = GetAllSteps();
            // Подстановка секций под каждый вариант
            for (int h = 0; h < housesSteps.Count; h++)
            {
                var houseSteps = housesSteps[h];

#if TEST
                var sisexTest = string.Join(".", houseSteps);
#endif

                var houseVar = GetHouseVariant(houseSteps);

                if (houseVar != null)
                {
                    HouseInfo hi = new HouseInfo();
                    hi.SpotInf = sp;
                    hi.SectionsBySize = houseVar;
                    resHouses.Add(hi);
                }                
            }
            return resHouses;
        }

        private static string GetSectionDataKey (int sectCountStep, int numberFailSect, int startStepFailedSect)
        {            
            string key = "n" + numberFailSect + "z" + sectCountStep + "s" + startStepFailedSect;
            return key;
        }

        private List<Section> GetHouseVariant (int[] houseSteps)
        {
            Debug.WriteLine("Размерность дома: " + string.Join(",", houseSteps));

            List<Section> resSections = new List<Section>();            
            int curStepInHouse = 1;
            int sectionsInHouse = houseSteps.Length;

            string key = string.Empty;
            bool fail = false;
            bool addToFailed = true;

            // Перебор нарезанных секций в доме
            for (int numberSect = 1; numberSect <= sectionsInHouse; numberSect++)
            {
                fail = false;
                Section section = null;   
                             
                // Размер секции - шагов
                var sectCountStep = SectionSteps[houseSteps[numberSect - 1]];

                // ключ размерности секции
                key = GetSectionDataKey(sectCountStep, numberSect, curStepInHouse);
                if (failedSections.Contains(key))
                {
                    Debug.WriteLine("failedSection - " + key);

                    fail = true;
                    addToFailed = false;
                    break;
                }            

                // Отрезка секции из дома
                section = houseSpot.GetSection(curStepInHouse, sectCountStep);
                if (section == null)
                {
                    Debug.WriteLine("fail нарезки - curStepInHouse=" + curStepInHouse + "; sectCountStep=" + sectCountStep);

                    fail = true;
                    break;
                }
                curStepInHouse += sectCountStep;

                // Этажность секции, тип
                var type = GetSectionType(section.SectionType);                
                section.Floors = GetSectionFloors(numberSect, sectionsInHouse, section.IsCorner);
                var levels = GetSectionLevels(section.Floors);

                section.NumberInSpot = numberSect;
                section.SpotOwner = houseSpot.SpotName;

                section.IsStartSectionInHouse = numberSect == 1;
                section.IsEndSectionInHouse = numberSect == sectionsInHouse;

                // Запрос секций из базы
                section.Sections = dbService.GetSections(section, type, levels, sp, maxSectionBySize);
                if (section.Sections.Count == 0)
                {
                    Debug.WriteLine("fail no in db - шаг=" + section.CountStep + "; type=" + type + "; levels=" + levels);

                    fail = true;
                    break;
                }

                // Проверка инсоляции секции
                List<FlatInfo> flatsCheckedIns = insService.GetInsolationSections(section);
                if (flatsCheckedIns.Count == 0)
                {
                    Debug.WriteLine("fail ins");

                    fail = true;
                    break;
                }
                section.Sections = flatsCheckedIns;
                resSections.Add(section);
            }

            if (fail)
            {
                resSections = null;
                if (addToFailed)
                    failedSections.Add(key);
            }

            // Определение торцов секций
            DefineSectionsEnds(resSections);

            return resSections;
        }       

        private int GetSectionFloors (int numberSect, int sectionsInHouse, bool isCorner)
        {
            int floors = houseSpot.HouseOptions.CountFloorsMain;            
            if (!isCorner)
            {
                bool isDominant = false;
                if (numberSect < 4)
                {
                    isDominant = houseSpot.HouseOptions.DominantPositions[numberSect - 1];
                }
                else if (numberSect == sectionsInHouse)
                {
                    isDominant = houseSpot.HouseOptions.DominantPositions.Last();
                }
                else if (numberSect == sectionsInHouse -1)
                {
                    isDominant = houseSpot.HouseOptions.DominantPositions[3];
                }
                if (isDominant)
                {
                    floors = houseSpot.HouseOptions.CountFloorsDominant;
                }
            }
            return floors;
        }

        public static string GetSectionLevels(int countFloors)
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
            int houseSteps = houseSpot.CountSteps;
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

        /// <summary>
        /// Определение торцов в секциях
        /// </summary>
        /// <param name="sections"></param>
        private void DefineSectionsEnds (List<Section> sections)
        {
            if (sections == null) return;

            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                Section sectionPrev = null;
                if (i!=0)
                    sectionPrev = sections.ElementAt(i-1);
                Section sectionNext = null;
                if (i!= sections.Count-1)
                    sectionNext = sections.ElementAt(i + 1);
                                
                section.JointStart = GetJoint(section, sectionPrev);
                section.JointEnd = GetJoint(section, sectionNext);
            }
        }

        private Joint GetJoint(Section section, Section sectionJoint)
        {
            if (sectionJoint == null)
                return Joint.End;
            
            if (section.Floors> sectionJoint.Floors)
            {
                return Joint.End;
            }
            else if (section.Floors == sectionJoint.Floors)
            {
                return Joint.None;
            }
            else
            {
                return Joint.Seam;
            }
        }        
    }
}