using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AR_AreaZhuk.Model;
using AR_AreaZhuk.PIK1TableAdapters;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk
{
    static class Program
    {


        private static List<RoomInfo> GetTopFlatsInSection(List<RoomInfo> section, bool isTop)
        {
            List<RoomInfo> topFlats = new List<RoomInfo>();
            if (isTop)
            {
                int counter = section.Count - 1;
                while (section[counter].SelectedIndexTop > 0)
                {
                    topFlats.Add(section[counter]);
                    counter--;
                }
                counter = 0;
                while (section[counter].SelectedIndexTop > 0)
                {
                    topFlats.Add(section[counter]);
                    counter++;
                }
            }
            else
            {
                int counter = 0;
                foreach (RoomInfo t in section)
                {
                    if (t.SelectedIndexTop != 0)
                        continue;
                    if (t.SelectedIndexBottom == 0)
                        continue;
                    topFlats.Add(t);
                    counter++;
                }
            }
            return topFlats;
        }

        private static int[] GetLightingPosition(string[] lightStr)
        {
            int[] light = new int[5];
            if (lightStr[0].Contains('-'))
            {
                int counter = 0;
                for (int i = Convert.ToInt16(lightStr[0].Split('-')[0]);
                    i <= Convert.ToInt16(lightStr[0].Split('-')[1]);
                    i++)
                {
                    light[counter] = i;
                    counter++;
                }
            }
            else if (lightStr[0].Contains(','))
            {
                string[] mass = lightStr[0].Split(',');
                int counter = 0;
                for (int i = 0; i < mass.Length; i++)
                {
                    light[counter] = Convert.ToInt16(mass[i]);
                    counter++;
                }
            }
            else light[0] = Convert.ToInt16(lightStr[0]);
            return light;
        }
        public static int GetRandom(Random random, List<RoomInfo> roomInfo, RoomInfo lastRoom, List<RoomInfo> selectedRooms)
        {
            int r = 0;
            bool isValid = true;
            for (int i = 0; i < roomInfo.Count; i++)
            {
                r = random.Next(0, roomInfo.Count - 1);
                if (roomInfo[r].IndexLenghtNIZ.Split('/')[0].Equals("!") | roomInfo[r].SubZone.Equals("0"))
                {
                    continue;
                }
                foreach (var x in selectedRooms)
                {
                    if (!x.Type.Equals(roomInfo[r].Type))
                        continue;
                    if (roomInfo[r].Requirment.Equals("<=1"))
                        isValid = false;

                }
                if (isValid) break;
            }
            return r;
        }
        public static SpotInfo spotInfo = new SpotInfo();

        /// <summary>
        /// Главная точка входа для приложения.
        /// 
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //MainForm mf = new MainForm();
            //// MainForm mf = new MainForm();
            //Requirment requirment = new Requirment();
            //FrameWork fw = new FrameWork();
            //var roomInfo = fw.GetRoomData("");
            ////PIK1TableAdapters.C_Flats_PIK1TableAdapter flatsPIK1 = new C_Flats_PIK1TableAdapter();
            ////foreach (var rr in roomInfo)
            ////{
            ////    flatsPIK1.InsertFlat(rr.Type, rr.ShortType, rr.AreaLive, rr.AreaTotalStandart, rr.AreaTotal,
            ////        Convert.ToInt16(rr.AreaModules), 0, 0, rr.LinkageDO, rr.LinkagePOSLE,
            ////        rr.FactorSmoke, rr.LightingNiz, rr.LightingTop, rr.IndexLenghtTOP, rr.IndexLenghtNIZ, rr.SubZone);
            ////}
            //spotInfo = fw.GetSpotInformation(roomInfo);
            //MainForm.spotInfo = spotInfo;


            //var averageArea = spotInfo.SpotArea / 4;
            //var countSections = fw.GetCountSections(spotInfo);
            //var insulations = fw.GetInsulations("");
            //List<Section> dbSections = new List<Section>();
            //// var opa = fw.ExportSectionsToDB(roomInfo, 56, false, true, 18);
            //for (int i = 8; i < 14; i++)
            //{
            //    Section sec = new Section();
            //    sec.Sections = new List<FlatInfo>();
            //    sec.Floors = 18;
            //    sec.CountModules = i * 4;
            //    sec.Sections = fw.GetAllSections(roomInfo, i * 4, false, false, 18);
            //    dbSections.Add(sec);
            //}
            //for (int i = 8; i < 15; i++)
            //{
            //    Section sec = new Section();
            //    sec.Sections = new List<FlatInfo>();
            //    sec.Floors = 18;
            //    sec.IsLeftBottomCorner = true;
            //    sec.CountModules = i * 4;
            //    sec.Sections = fw.GetAllSections(roomInfo, i * 4, true, false, 18);
            //    dbSections.Add(sec);
            //}
            //for (int i = 8; i < 15; i++)
            //{
            //    Section sec = new Section();
            //    sec.Sections = new List<FlatInfo>();
            //    sec.Floors = 25;
            //    sec.CountModules = i * 4;
            //    sec.Sections = fw.GetAllSections(roomInfo, i * 4, false, false, 25);
            //    dbSections.Add(sec);
            //}



            //List<List<HouseInfo>> totalObject = new List<List<HouseInfo>>();
            //List<List<HouseInfo>> houses = new List<List<HouseInfo>>();
            //List<HouseInfo> listSections = new List<HouseInfo>();
            //List<List<FlatInfo>> sectionsInHouse = new List<List<FlatInfo>>();
            //bool isContinue1 = true;
            //bool isContinue2 = true;
            //int[] masSizes = new int[] { 32, 36, 40, 44, 48, 52, 56 };
            //int counterr = 1;
            //foreach (var insulation in insulations)
            //{
            //    counterr++;
            //    MainForm.isContinue = true;
            //    List<HouseInfo> housesTemp = new List<HouseInfo>();
            //    SpotInfo sp = new SpotInfo();
            //    int[] indexSelectedSize = new int[15];
            //    int[] indexSelectedSection = new int[15];
            //    bool isGo = true;

            //    isContinue1 = true;
            //    while (isContinue1)
            //    {
            //        int countModulesTotal = Convert.ToInt16(sp.SpotArea / 12.96);
            //        for (int i = 0; i < 10; i++)
            //        {
            //            if (i == 0)
            //            {
            //                indexSelectedSize[0] = 1;
            //            }
            //            countModulesTotal = countModulesTotal -
            //                                masSizes[indexSelectedSize[i]];
            //            if (countModulesTotal == 0)
            //            {
            //                bool isValid = true;
            //                for (int j = 0; j < listSections.Count; j++)
            //                {
            //                    if (listSections[j].Sections.Count > 0)
            //                        continue;
            //                    isValid = false;
            //                    break;
            //                }
            //                if (isValid)
            //                {
            //                    isContinue2 = true;
            //                    List<Section> sectionsGood = new List<Section>();
            //                    for (int j = 0; j <= i; j++)
            //                    {

            //                        if (j == 1)
            //                        {
            //                            var list = dbSections.Where(x => x.Floors.Equals(18))
            //                                .Where(x => x.IsLeftBottomCorner == true)
            //                                .Where(x => x.CountModules.Equals(masSizes[indexSelectedSize[j]]))
            //                                .ToList();
            //                            if (list.Count == 0)
            //                                break;
            //                            sectionsGood.Add(list[0]);
            //                        }
            //                        else if (j == i)
            //                        {
            //                            var list = dbSections.Where(x => x.Floors.Equals(25))
            //                                .Where(x => x.IsLeftBottomCorner == false
            //                                            & x.IsRightBottomCorner == false)
            //                                .Where(x => x.CountModules.Equals(masSizes[indexSelectedSize[j]]))
            //                                .ToList();
            //                            if (list.Count == 0)
            //                                break;
            //                            sectionsGood.Add(list[0]);

            //                        }
            //                        else
            //                        {
            //                            var list =
            //                                dbSections.Where(x => x.Floors.Equals(18))
            //                                    .Where(
            //                                        x => x.IsLeftBottomCorner == false & x.IsRightBottomCorner == false)
            //                                    .Where(x => x.CountModules.Equals(masSizes[indexSelectedSize[j]]))
            //                                    .ToList();
            //                            if (list.Count == 0)
            //                                break;
            //                            sectionsGood.Add(list[0]);
                                 
            //                        }
            //                    }
            //                    if (sectionsGood.Count != i + 1)
            //                    {
            //                        continue;
            //                    }

            //                    bool isGoGood = true;
            //                    for (int j = 0; j <= i; j++)
            //                    {
            //                        if (sectionsGood[j].Sections.Count == 0)
            //                        {
            //                            isGoGood = false;
            //                            break;
            //                        }
            //                    }
            //                    if (!isGoGood)
            //                    {
            //                        indexSelectedSize[i]++;
            //                        if (indexSelectedSize[i] >= masSizes.Length)
            //                        {
            //                            if (!mf.SetIndexesSize(indexSelectedSize, i, masSizes))
            //                                isContinue1 = false;
            //                        }
            //                        break;
            //                    }
            //                    bool isContinue = true;
            //                    bool isFirstSection = true;
            //                    bool isRightOrTopLLu = true;
            //                    bool isVertical = true;
            //                    bool isCorner = false;
            //                    int countEnter = 1;
            //                    int indexRowStart = 0; //10;
            //                    int indexColumnStart = 0; //9;
            //                    int heightSection = 0;
            //                    int cornerCountLinesFirstSection = 0;
            //                    if (insulation.IsLeftNizSection)
            //                    {
            //                        cornerCountLinesFirstSection = insulation.MaxLeftXY[1] - insulation.MinLeftXY[1] + 1 - 5;
            //                        indexRowStart = cornerCountLinesFirstSection + insulation.MinLeftXY[1];
            //                        indexColumnStart = insulation.MinLeftXY[0];
            //                    }
            //                    else if (insulation.IsRightNizSection)
            //                    {
            //                        cornerCountLinesFirstSection = insulation.MaxRightXY[1] - insulation.MinRightXY[1] + 1 - 5;
            //                        indexRowStart = cornerCountLinesFirstSection + insulation.MinRightXY[1];
            //                        indexColumnStart = insulation.MaxRightXY[0];
            //                    }

            //                    int tempColumnIndex = 0;
            //                    List<HouseInfo> sectionInfos = new List<HouseInfo>();
            //                    foreach (var sectionGood in sectionsGood)
            //                    {
                                   


            //                        isCorner = sectionGood.IsLeftBottomCorner | sectionGood.IsRightBottomCorner;
            //                            HouseInfo s = new HouseInfo();
            //                            s.Sections = new List<FlatInfo>();
            //                        if (countEnter == 2)
            //                            isVertical = false;
            //                                Section s1 = new Section();

            //                                s1.CountModules = sectionGood.CountModules;
            //                                if (countEnter > 1)
            //                                {
            //                                    //тут будут условия в зависимости от расположения угловой секции
            //                                    if (insulation.IsLeftNizSection)
            //                                    {
            //                                        indexRowStart = insulation.MaxLeftXY[1] - 3;
            //                                        indexColumnStart += sectionGood.CountModules/4;
            //                                        if (isCorner)
            //                                            indexColumnStart--;
            //                                    }
            //                                    if (insulation.IsRightNizSection)
            //                                    {
            //                                        if (indexColumnStart - sectionGood.CountModules / 4 < 0 ||
            //                                            insulation.Matrix[indexColumnStart - sectionGood.CountModules / 4, indexRowStart].Equals(""))
            //                                        {
            //                                            isContinue = false;
            //                                            break;
            //                                        }
            //                                    }
            //                                }
            //                                int countFloors = 18;
            //                                if (countEnter == 5)
            //                                {
            //                                    countFloors = 25;
            //                                }
            //                                List<FlatInfo> sections = new List<FlatInfo>();
            //                                 sections = sectionGood.Sections;
            //                                if (sections.Count > 0)
            //                                {
            //                                    var listSections1 = mf.GetInsulationSections(sections, isRightOrTopLLu, isVertical, indexRowStart,
            //                                        indexColumnStart, insulation, isCorner);
            //                                    s1.Sections = listSections1;
            //                                    if (!isCorner)
            //                                    {
            //                                        var listSections2 = mf.GetInsulationSections(sections, false, isVertical, indexRowStart,
            //                                            indexColumnStart, insulation, isCorner);
            //                                        foreach (var l in listSections2)
            //                                            s1.Sections.Add(l);

            //                                    }
            //                                }
            //                        s.Sections = s1.Sections;
            //                            countEnter++;
            //                            sectionInfos.Add(s);
            //                    }



                                
            //                    indexSelectedSection = new int[15];
            //                    while (isContinue2)
            //                    {
            //                        HouseInfo hi = new HouseInfo();
            //                        hi.Sections = new List<FlatInfo>();
            //                        try
            //                        {
            //                            List<FlatInfo> secs = new List<FlatInfo>();
            //                            int[] ids = new int[i+1];
            //                            for (int j = 0; j <= i; j++)
            //                            {
            //                                if (sectionInfos[j].Sections.Count == 0)
            //                                {
            //                                    isContinue2 = false;
            //                                    break;
            //                                }
            //                                hi.Sections.Add(sectionInfos[j].Sections[indexSelectedSection[j]]);
            //                                ids[j] = hi.Sections[j].IdSection;
            //                            }
            //                            if (ids.GroupBy(x => x).ToList().Count == hi.Sections.Count)
            //                            {
            //                                indexSelectedSection[i]++;
            //                                continue;
            //                            }
            //                            if (!isContinue2)
            //                                break;
            //                            mf.GetHousePercentage(ref hi,spotInfo);
            //                            housesTemp.Add(hi);
            //                            indexSelectedSection[i]++;
            //                            if (indexSelectedSection[i] >=
            //                                sectionInfos[i].Sections.Count)
            //                            {
            //                                if (!mf.SetIndexesSection(indexSelectedSection, indexSelectedSize, i, sectionInfos))
            //                                    isContinue2 = false;
            //                            }
            //                        }
            //                        catch
            //                        {
            //                            break;
            //                        }

            //                    }
            //                }
            //                indexSelectedSize[i]++;
            //                if (indexSelectedSize[i] >= masSizes.Length)
            //                {
            //                    if (!mf.SetIndexesSize(indexSelectedSize, i, masSizes))
            //                        isContinue1 = false;
            //                }
            //                break;
            //            }
            //            else if (countModulesTotal < 32)
            //            {
            //                indexSelectedSize[i]++;
            //                if (indexSelectedSize[i] >= masSizes.Length)
            //                {
            //                    if (!mf.SetIndexesSize(indexSelectedSize, i, masSizes))
            //                        isContinue1 = false;
            //                }
            //                break;
            //            }
            //        }
            //    }
            //    totalObject.Add(housesTemp);
            //}
            //mf.GetAllSectionPercentage(totalObject, requirment);
             Application.Run(new MainForm());
        }
    }
}
