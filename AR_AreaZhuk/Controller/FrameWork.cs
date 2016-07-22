using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AR_AreaZhuk.PIK1TableAdapters;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using AR_Zhuk_DataModel;
using AR_AreaZhuk.Insolation;

namespace AR_AreaZhuk
{
    public class FrameWork
    {
        public static bool IsExit = false;
        public double GetCountLines(double area)
        {
            return area / 14.4 / 3.6;
        }

        public int GetIndexSection(RoomInfo selectedRoom, RoomInfo preRoom, bool isNizIndex)
        {
            if (selectedRoom.ShortType.Equals("2KL2"))
            {
                if (preRoom.LinkagePOSLE.Contains("J"))
                {
                    int a = 1 + 1;
                    int b = a + 1;
                }
                if (preRoom.LinkagePOSLE.Contains("N"))
                {
                    int a = 1 + 1;
                    int b = a + 1;
                }
            }
            int index = -100;
            string[] masSelected = null;
            string[] masPre = null;
            if (isNizIndex)
            {
                masSelected = selectedRoom.IndexLenghtNIZ.Split(';');
                masPre = preRoom.IndexLenghtNIZ.Split(';');
            }
            else
            {
                masSelected = selectedRoom.IndexLenghtTOP.Split(';');
                masPre = preRoom.IndexLenghtTOP.Split(';');
            }

            if (preRoom == null)
            {
                index = Convert.ToInt16(masSelected[0]);
            }

            else if (masSelected.Length > 1)
            {
                for (int i = 0; i < masSelected.Length; i++)
                {
                    string[] selected = masSelected[i].Split('|');
                    if (selected.Length > 1 && selected[0].Contains(">"))
                    {
                        if (preRoom.LinkagePOSLE.Contains(selected[0].Trim().Substring(0, 1)) &
                            selectedRoom.LinkageDO.Contains(selected[0].Trim().Substring(2, 1)))
                        {
                            index = Convert.ToInt16(selected[1]);
                            break;
                        }
                    }
                    else if (selected.Length > 1 && selectedRoom.LinkagePOSLE.Contains(selected[0].Trim()) |
                            selectedRoom.LinkageDO.Contains(selected[0].Trim()))
                    {
                        index = Convert.ToInt16(selected[1]);
                        break;
                    }
                    else { int.TryParse(masSelected[0], out index); }
                }
                //string symbolCode = masSelected[1].Trim();
                //if (preRoom.LinkageDO.Contains(symbolCode))
                //    index = Convert.ToInt16(masSelected[2]);
                //else index = Convert.ToInt16(masSelected[0]);
            }
            //else if (masPre.Length > 1)
            //{
            //    string symbolCode = masPre[1].Trim();
            //    int remaining = Convert.ToInt16(masPre[2]) - Convert.ToInt16(masPre[0]);
            //    if (preRoom.LinkageDO.Contains(symbolCode))
            //        index = Convert.ToInt16(masSelected[0]) + remaining;
            //    else index = Convert.ToInt16(masSelected[0]);
            //}
            else
            {
                int.TryParse(masSelected[0], out index);
            }
            return index;
        }

        public List<InsolationSpot> GetInsulations(string path)
        {
            List<InsolationSpot> insulations = new List<InsolationSpot>();
            insulations.Add(GetInsulationSpot("P1|", path));
            //insulations.Add(GetInsulationSpot("P2|", path));
            return insulations;
        }

        private static InsolationSpot GetInsulationSpot(string nameSpot, string path)
        {
            InsolationSpot insulation = new InsolationSpot();
            insulation.Name = nameSpot;
            insulation.Matrix = new string[100, 100];
            insulation.MaxLeftXY = new List<int>();
            insulation.MinLeftXY = new List<int>();

            insulation.MaxRightXY = new List<int>();
            insulation.MinRightXY = new List<int>();
            List<RoomInfo> roomsInfo = new List<RoomInfo>();
           // path = @"E:\Задание по инсоляции ПИК1.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(path.ToString())))
            {
                int firstRow = 1;
                int firstColumn = 1;
                bool isAllBreak = false;
                // string nameSpot = "P2|";
                int minColumn = 5000;
                int maxColumn = -5000;

                var workSheet = xlPackage.Workbook.Worksheets[1];

                for (int column = 1; column < 100; column++)
                {
                    for (int row = 1; row < 100; row++)
                    {
                        var cellVal = Convert.ToString(workSheet.Cells[row, column].Value);
                        if (cellVal.Contains(nameSpot))
                        {
                            if (minColumn > column - 1)
                                minColumn = column - 1;
                            if (maxColumn < column - 1)
                                maxColumn = column - 1;
                            insulation.Matrix[column - 1, row - 1] = cellVal;
                        }
                        else insulation.Matrix[column - 1, row - 1] = string.Empty;
                    }
                }


                int minRow = 5000;
                int maxRow = -5000;
                for (int i = 0; i < 100; i++)
                {
                    if (string.IsNullOrEmpty(insulation.Matrix[minColumn, i]))
                        continue;
                    if (!insulation.Matrix[0, i].Contains(nameSpot))
                        continue;
                    if (minRow > i)
                        minRow = i;
                    if (maxRow < i)
                        maxRow = i;
                }
                insulation.MinLeftXY.Add(minColumn);
                insulation.MinLeftXY.Add(minRow);
                insulation.MaxLeftXY.Add(minColumn);
                insulation.MaxLeftXY.Add(maxRow);

                minRow = 5000;
                maxRow = -500;
                for (int i = 0; i < 100; i++)
                {
                    if (string.IsNullOrEmpty(insulation.Matrix[maxColumn - 1, i]))
                        continue;
                    if (!insulation.Matrix[maxColumn - 1, i].Contains(nameSpot))
                        continue;
                    if (minRow > i)
                        minRow = i;
                    if (maxRow < i)
                        maxRow = i;
                }
                insulation.MinRightXY.Add(maxColumn);
                insulation.MinRightXY.Add(minRow);
                insulation.MaxRightXY.Add(maxColumn);
                insulation.MaxRightXY.Add(maxRow);
                string info = "";
                if (insulation.MaxLeftXY[1] - insulation.MinLeftXY[1] == insulation.MaxRightXY[1] - insulation.MinRightXY[1])
                {
                    info += "П образная.";
                    string s = Convert.ToString(insulation.Matrix[insulation.MinLeftXY[0], insulation.MinLeftXY[1] + 8]);
                    if (string.IsNullOrEmpty(s))
                        info += " Низ.";
                    else info += " Верх.";
                }
                else
                {
                    if (insulation.MaxLeftXY[1] - insulation.MinLeftXY[1] > insulation.MaxRightXY[1] - insulation.MinRightXY[1])
                    {
                        info += "Угловая. Левый угол. ";
                    }
                    else if (insulation.MaxLeftXY[1] - insulation.MinLeftXY[1] <
                             insulation.MaxRightXY[1] - insulation.MinRightXY[1])
                    {
                        info += "Угловая. Правый угол. ";
                    }
                    if (insulation.MinLeftXY[1].Equals(insulation.MinRightXY[1]))
                        info += "Верх.";
                    else info += "Низ.";
                    switch (info)
                    {
                        case "Угловая. Левый угол. Верх.":
                            insulation.IsLeftTopSection = true;
                            break;
                        case "Угловая. Правый угол. Низ.":
                            insulation.IsRightNizSection = true;
                            break;
                        case "Угловая. Правый угол. Верх.":
                            insulation.IsRightTopSection = true;
                            break;
                        case "Угловая. Левый угол. Низ.":
                            insulation.IsLeftNizSection = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            return insulation;
        }

        public
             List<RoomInfo> GetRoomData(string path)
        {
            List<RoomInfo> roomsInfo = new List<RoomInfo>();
            path = @"E:\__ROM_Типы квартир.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(path.ToString())))
            {
                int counter = 2;
                while (xlPackage.Workbook.Worksheets[1].Cells[counter, 1].Value != null)
                {

                    string shortType = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 1].Value);
                    string subzone = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 2].Value);
                    string type = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 3].Value);
                    string liveArea = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 4].Value);
                    string totalAreaStandart = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 5].Value);
                    string totalArea = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 6].Value);
                    string moduleArea = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 7].Value);
                    string indexNIZ = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 8].Value);
                    string indexTOP = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 9].Value);
                    string linkageBefore = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 10].Value);
                    string linkageAfter = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 11].Value);
                    string linkageOr = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 12].Value);
                    string req = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 13].Value);
                    string factorSmoke = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 14].Value);
                    string typeSection = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 15].Value);
                    string typeHouse = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 16].Value);
                    string levels = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 17].Value);
                    string order = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 18].Value);
                    string lightNiz = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 19].Value);
                    string lightTop = Convert.ToString(xlPackage.Workbook.Worksheets[1].Cells[counter, 20].Value);
                    string[] linkMasBefore = linkageBefore.Split(';');
                    string[] linkMasAfter = linkageAfter.Split(';');

                    for (int i = 0; i < linkMasBefore.Length; i++)
                    {
                        for (int j = 0; j < linkMasAfter.Length; j++)
                        {
                            RoomInfo ri = new RoomInfo(shortType, subzone, type, liveArea, totalAreaStandart, totalArea,
                                moduleArea, indexNIZ, indexTOP, linkMasBefore[i], linkMasAfter[j], linkageOr,
                                req, typeSection, levels, typeHouse, order, lightNiz, lightTop, factorSmoke);
                            roomsInfo.Add(ri);
                        }
                    }



                    counter++;
                }
            }
            return roomsInfo;
        }

        //public int GetCountSections(SpotInfo spotInfo)
        //{
        //    //HouseInfo si = new HouseInfo();
        //    //double count = spotInfo.SpotArea / si.MaxArea;
        //    //int intCount = (int)count;
        //    //if (count - intCount > 0)
        //    //    return intCount + 1;
        //    //return intCount;
        //    return 0;
        //}

        public SpotInfo GetSpotInformation()
        {
            SpotInfo spotInfo = new SpotInfo();
            spotInfo.requirments.Add(new Requirment("01", 22, 23, 14, 0, 3, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("1", 33, 35, 21, 0, 4, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("1", 45, 47, 6, 0, 4, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("2", 45, 47, 17, 0, 3, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("2", 53, 56, 12, 0, 3, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("2", 68, 70, 8, 0, 3, 0, 0, 5));
            spotInfo.requirments.Add(new Requirment("3", 85, 95, 22, 0, 2, 0, 0, 5));
            return spotInfo;
        }


        public List<FlatInfo> GenerateSections(List<RoomInfo> roomInfo, int countModulesInSection,
          bool isCornerLeftNiz, bool isCornerRightNiz, int countFloors)
        {
            // FrameWork fw = new FrameWork();
            //  double averageAreaSection = spotInfo.SpotArea / countSections;
            // List<HouseInfo> sectionsInfo = new List<HouseInfo>();
            int countEqual = 0;

            // double currentSum = 569.95 + 51.84;//spotInfo.SpotArea / 4; //569.95+51.84;//571.5;//+51.84;
            int countIndexes = countModulesInSection / 2;

            int limit = 5000000;
            int iteration = 0;
            Random random = new Random();
            // bool isContinue = true;
            List<FlatInfo> listSections = new List<FlatInfo>();
            int indexSummNiz = 0;
            int indexSummTop = 0;
            double summ = 0;
            List<RoomInfo> flatsISection = new List<RoomInfo>();
            RoomInfo lastRoom = null;

            if (isCornerLeftNiz)
            {
                var rr = roomInfo.Where(x => x.Type.Equals("PIK1U_BS_A_10-17_A_2")).ToList();
                summ = rr.Max(x => x.AreaModules);
                lastRoom = rr.Where(x => x.AreaModules.Equals(summ)).ToList()[0];
                indexSummNiz = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                indexSummTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                lastRoom.SelectedIndexTop = indexSummTop;
                lastRoom.SelectedIndexBottom = indexSummNiz;
                flatsISection.Add(lastRoom);
                lastRoom = roomInfo.Where(x => x.Type.Equals("PIK1_3NL2_Z0")).ToList()[0];
                flatsISection.Add(lastRoom);
                summ += lastRoom.AreaModules;
                indexSummNiz += Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                indexSummTop += Convert.ToInt16(lastRoom.IndexLenghtTOP);
                lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
            }
            else if (isCornerRightNiz | !isCornerLeftNiz)
            {
                if (isCornerRightNiz)
                {
                    var rr = roomInfo.Where(x => x.Type.Equals("PIK1U_BS_L_10-17_A_2")).ToList();
                    summ = rr.Max(x => x.AreaModules);
                    lastRoom = rr.Where(x => x.AreaModules.Equals(summ)).ToList()[0];
                    indexSummNiz = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                    indexSummTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                    lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                    lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);

                    var tempRoom = roomInfo.Where(x => x.Type.Equals("PIK1_3NL2_A0")).ToList()[0];
                    flatsISection.Add(tempRoom);
                    flatsISection.Add(lastRoom);
                }
                else
                {
                    if (countFloors == 25)
                    {
                        var rr = roomInfo.First(x => x.Type.Equals("PIK1U_BS_L_18-25_A_3"));//PIK1U_BS_L_18-25_A_3

                        summ = rr.AreaModules;
                        lastRoom = rr;
                        indexSummNiz = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                        indexSummTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                        lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                        lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                        flatsISection.Add(lastRoom);
                    }
                    else
                    {
                        var rr = roomInfo.First(x => x.Type.Equals("PIK1U_BS_A_10-17_Z_2"));//PIK1U_BS_L_18-25_A_3

                        summ = rr.AreaModules;
                        lastRoom = rr;//PIK1U_BS_A_10-17_Z_2
                        indexSummNiz = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                        indexSummTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                        lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
                        lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
                        flatsISection.Add(lastRoom);
                    }

                }
            }

            while (limit >= iteration)
            {
                int indexSummNizTemp = indexSummNiz;
                int indexSummTopTemp = indexSummTop;
                double summTemp = summ;
                List<RoomInfo> flatsInSectionTemp = new List<RoomInfo>();
                foreach (var f in flatsISection)
                {
                    flatsInSectionTemp.Add(f);
                }
                RoomInfo lastRoomTemp = new RoomInfo(lastRoom, lastRoom.LinkageDO, lastRoom.LinkagePOSLE);
                bool isTopNull = false;
                for (int i = 0; i < 11; i++)
                {
                    string[] lastRoomConfig = lastRoomTemp.LinkagePOSLE.Split('/')[0].Split('|');
                    int[] intMass = new int[10];

                    var tempRoomInfo = TempRoomInfo(roomInfo, isCornerLeftNiz, isCornerRightNiz, lastRoomConfig);
                    bool isGo = true;
                    List<List<RoomInfo>> rrrrr = new List<List<RoomInfo>>();
                    int lastIndex = 9;
                    List<List<RoomInfo>> alllrooms = new List<List<RoomInfo>>();
                    if (tempRoomInfo.Count == 0)
                        continue;
                    int indexRandom = GetRandom(random, tempRoomInfo, lastRoomTemp, flatsInSectionTemp, isCornerRightNiz,
                        isCornerLeftNiz);
                    if (indexRandom == -100)
                        continue;
                    double randomValue = tempRoomInfo[indexRandom].AreaModules;
                    if (tempRoomInfo[indexRandom].IndexLenghtNIZ.Split('/')[0].Equals("!"))
                        continue;

                    int indexNiz = GetIndexSection(tempRoomInfo[indexRandom], lastRoomTemp, true);
                    int indexTop = GetIndexSection(tempRoomInfo[indexRandom], lastRoomTemp, false);
                    if (indexTop == 0)
                        isTopNull = true;
                   

                    indexSummNizTemp += indexNiz;
                    indexSummTopTemp += indexTop;
                    

                    if (indexSummNizTemp < 0 | indexSummTopTemp < 0)
                        continue;
                    if (isTopNull & indexNiz == 0)
                    {
                        if (indexSummNizTemp != countIndexes/2)
                            break;
                    }
                    summTemp += randomValue;
                    lastRoomTemp = tempRoomInfo[indexRandom];
                    lastRoomTemp.SelectedIndexTop = indexTop;
                    lastRoomTemp.SelectedIndexBottom = indexNiz;
                    flatsInSectionTemp.Add(tempRoomInfo[indexRandom]);
                    if (i >= 0)
                    {
                        if (isCornerRightNiz & lastRoomTemp.LinkagePOSLE.Contains("P"))
                        {
                            var tempRoom = roomInfo.Where(x => x.Type.Equals("PIK1_3NL2_A0")).ToList()[0];
                            // listRooms1.Add(lastRoom);
                            if (summTemp + tempRoom.AreaModules - countModulesInSection > 0)
                                continue;
                            if (Math.Abs(summTemp + tempRoom.AreaModules - countModulesInSection).Equals(0))
                            {
                                summTemp += tempRoom.AreaModules;
                                indexSummNizTemp += Convert.ToInt16(tempRoom.IndexLenghtNIZ);
                                indexSummTopTemp += Convert.ToInt16(tempRoom.IndexLenghtTOP);

                                flatsInSectionTemp.RemoveAt(0);
                                flatsInSectionTemp.Add(tempRoom);
                                lastRoomTemp = tempRoom;
                                lastRoomTemp.SelectedIndexTop = Convert.ToInt16(lastRoomTemp.IndexLenghtTOP);
                                lastRoomTemp.SelectedIndexBottom = Convert.ToInt16(lastRoomTemp.IndexLenghtNIZ);
                            }
                        }
                        if (summTemp - countModulesInSection > 0)
                            break;
                        if (Math.Abs(summTemp - countModulesInSection).Equals(0))
                        {
                            //if (flatsInSectionTemp[1].SubZone.Equals("2"))
                            //{

                            //}

                            if (indexSummNizTemp + indexSummTopTemp == countIndexes & indexSummNizTemp == countIndexes / 2 &
                                !isCornerLeftNiz & !isCornerRightNiz) //рядовая
                                AddToListSections(listSections, flatsInSectionTemp, roomInfo, isCornerLeftNiz, countFloors);

                            else if ((summTemp / 4 - 4) == indexSummTopTemp & (summTemp / 4 - 4) + 7 == indexSummNizTemp &
                                     (isCornerLeftNiz | isCornerRightNiz)) //угловая
                            {
                                if (isCornerRightNiz & lastRoomTemp.Type.Equals("PIK1_3NL2_A0"))
                                    AddToListSections(listSections, flatsInSectionTemp, roomInfo, isCornerLeftNiz, countFloors);
                                else if (isCornerLeftNiz)
                                    AddToListSections(listSections, flatsInSectionTemp, roomInfo, isCornerLeftNiz, countFloors);
                            }

                            break;
                        }
                    }
                }
                iteration++;
            }
            return listSections;
        }





        //public List<List<RoomInfo>> GetSections(List<RoomInfo> roomInfo, int countModulesInSection,
        //    bool isCornerLeftNiz, bool isCornerRightNiz, int countFloors)
        //{
        //    bool isNizLeft = isCornerLeftNiz;
        //    bool isNizRight = isCornerRightNiz;

        //    string[] lastRoomConfig1 = null;
        //    int[] intMass = new int[12];
        //    bool isGo = true;
        //    countFloors = 18;

        //    List<List<RoomInfo>> rrrrr = new List<List<RoomInfo>>();



        //    List<RoomInfo> roomInfoVariants = new List<RoomInfo>();
        //    List<RoomInfo> flatsISection = new List<RoomInfo>();
        //    RoomInfo lastRoom = new RoomInfo();
        //    int indexSummNiz1 = 0;
        //    int indexSummTop1 = 0;
        //    int summ1 = 0;
        //    if (isNizLeft)
        //    {
        //        var rr = roomInfo.Where(x => x.Type.Equals("PIK1U_BS_A_10-17_A_2")).ToList();
        //        summ1 = Convert.ToInt16(rr.Max(x => x.AreaModules));
        //        lastRoom = rr.Where(x => x.AreaModules.Equals(summ1)).ToList()[0];
        //        indexSummNiz1 = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //        indexSummTop1 = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //        lastRoom.SelectedIndexTop = indexSummTop1;
        //        lastRoom.SelectedIndexBottom = indexSummNiz1;
        //        // tempRoomInfo1 = TempRoomInfo(roomInfo, isCornerLeftNiz, isCornerRightNiz, lastRoomConfig1);
        //        flatsISection.Add(lastRoom);
        //        lastRoom = roomInfo.Where(x => x.Type.Equals("PIK1_3NL2_Z0")).ToList()[0];
        //        flatsISection.Add(lastRoom);
        //        summ1 += Convert.ToInt16(lastRoom.AreaModules);
        //        indexSummNiz1 += Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //        indexSummTop1 += Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //        lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //        lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //        lastRoomConfig1 = lastRoom.LinkagePOSLE.Split('/')[0].Split('|');
        //        roomInfoVariants = TempRoomInfo(roomInfo, isNizLeft, isNizRight, lastRoomConfig1);
        //    }
        //    else if (isNizRight)
        //    {
        //        var rr = roomInfo.Where(x => x.Type.Equals("PIK1_BS_A_10-17_A")).ToList();
        //        summ1 = Convert.ToInt16(rr.Max(x => x.AreaModules));
        //        lastRoom = rr.Where(x => x.AreaModules.Equals(summ1)).ToList()[0];
        //        indexSummNiz1 = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //        indexSummTop1 = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //        lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //        lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);

        //        //  var tempRoom = roomInfo.Where(x => x.Type.Equals("PIK1_3NL2_A0")).ToList()[0];
        //        //  listRooms1.Add(tempRoom);
        //        flatsISection.Add(lastRoom);
        //    }
        //    else if (!(isNizLeft & isNizRight))
        //    {
        //        if (countFloors == 25)
        //        {
        //            var rr = roomInfo.Where(x => x.SubZone.Equals("0"))
        //                .Where(x => x.LevelsSection.Equals("19-25"))
        //                .Where(x => x.TypeSection.Equals("Рядовая"))
        //                .Where(x => x.TypeHouse.Equals("Секционный"))
        //                .Where(x => x.FactorSmoke.Equals("")).Where(x => x.Requirment != "0")
        //                .ToList();

        //            summ1 = Convert.ToInt16(rr.Max(x => x.AreaModules));
        //            lastRoom = rr.Where(x => x.AreaModules.Equals(summ1)).ToList()[0];
        //            indexSummNiz1 = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //            indexSummTop1 = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //            lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //            lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //            lastRoomConfig1 = lastRoom.LinkagePOSLE.Split('/')[0].Split('|');
        //            roomInfoVariants = TempRoomInfo(roomInfo, isNizRight, isNizRight, lastRoomConfig1);
        //            flatsISection.Add(lastRoom);
        //        }
        //        else
        //        {
        //            var rr = roomInfo.Where(x => x.SubZone.Equals("0"))
        //           .Where(x => x.LevelsSection.Equals("10-18"))
        //           .Where(x => x.TypeSection.Equals("Рядовая"))
        //           .Where(x => x.TypeHouse.Equals("Секционный"))
        //           .Where(x => x.FactorSmoke.Equals(""))
        //           .ToList();

        //            summ1 = Convert.ToInt16(rr.Max(x => x.AreaModules));
        //            lastRoom = rr.Where(x => x.AreaModules.Equals(summ1)).ToList()[0];
        //            indexSummNiz1 = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //            indexSummTop1 = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //            lastRoom.SelectedIndexTop = Convert.ToInt16(lastRoom.IndexLenghtTOP);
        //            lastRoom.SelectedIndexBottom = Convert.ToInt16(lastRoom.IndexLenghtNIZ);
        //            lastRoomConfig1 = lastRoom.LinkagePOSLE.Split('/')[0].Split('|');
        //            roomInfoVariants = TempRoomInfo(roomInfo, isNizRight, isNizRight, lastRoomConfig1);
        //            flatsISection.Add(lastRoom);
        //        }

        //    }








        //    while (isGo)
        //    {

        //        int indexSummNizTemp = indexSummNiz1;
        //        int indexSummTopTemp = indexSummTop1;
        //        int summTemp = summ1;
        //        List<RoomInfo> flatsInSectionTemp = new List<RoomInfo>();
        //        foreach (var f in flatsISection)
        //        {
        //            flatsInSectionTemp.Add(f);
        //        }
        //        RoomInfo lastRoomTemp = new RoomInfo(lastRoom, lastRoom.LinkageDO, lastRoom.LinkagePOSLE);


        //        IsExit = false;

        //        var sectionVariant = GetRoomInfo(lastRoomTemp, roomInfo, flatsInSectionTemp, intMass, lastRoomTemp, indexSummNizTemp, indexSummTopTemp, summTemp, isNizLeft, isNizRight, 36);
        //        if (sectionVariant == null || sectionVariant.Count == 0)
        //            continue;
        //        if (roomInfoVariants.Count - 1 == intMass[1] & isNizLeft)
        //            break;
        //        else if (roomInfoVariants.Count - 1 == intMass[0])
        //            break;

        //        if (!sectionVariant[sectionVariant.Count - 1].LinkagePOSLE.Split('|')[2].Equals(
        //    sectionVariant[0].LinkageDO.Split('|')[2]))
        //            continue;
        //        if (isNizRight)
        //        {
        //            bool isEx = false;
        //            foreach (var r in rrrrr)
        //            {
        //                if (r.Count != sectionVariant.Count)
        //                    continue;
        //                isEx = false;
        //                for (int i = 0; i < r.Count; i++)
        //                {
        //                    if (r[i].Type != sectionVariant[i].Type)
        //                    {
        //                        isEx = false;
        //                        break;
        //                    }

        //                    isEx = true;
        //                }
        //                if (isEx)
        //                {
        //                    break;
        //                }
        //            }
        //            if (!isEx && IsValidSmoke(sectionVariant, roomInfo, false, countFloors, rrrrr))
        //                rrrrr.Add(sectionVariant);
        //            rrrrr.Add(sectionVariant);
        //        }
        //        else
        //        {

        //            if (IsValidSmoke(sectionVariant, roomInfo, isNizLeft, countFloors, rrrrr))
        //                rrrrr.Add(sectionVariant);
        //        }

        //    }
        //   return rrrrr;
        //}

        public List<FlatInfo> GetAllSectionsFromDB (int countModulesInSection,
            bool isCornerLeftNiz, bool isCornerRightNiz, int countFloors)
        {
            List<FlatInfo> sectionsBySyze = new List<FlatInfo>();
            FrameWork fw = new FrameWork();
            List<HouseInfo> sectionsInfo = new List<HouseInfo>();
            string countFl = "10-18";
            if (countFloors > 18 & countFloors <= 25)
                countFl = "19-25";
            //if (countFloors > 9 & countFloors <= 18)
            //    countFl = "10-18";
            if (countFloors < 9)
                countFl = "9";
            PIK1TableAdapters.C_SectionsTableAdapter sects = new C_SectionsTableAdapter();
            PIK1TableAdapters.FlatsInSectionsTableAdapter flatsIsSection = new FlatsInSectionsTableAdapter();
            string levels = "Рядовая";
            if (isCornerLeftNiz)
                levels = "Угловая лево";
            if (isCornerRightNiz)
                levels = "Угловая право";
            var sections = sects.GetSectionByID(countFl, levels, countModulesInSection / 4);
            var flats = flatsIsSection.GetFlatsInTypeSection(countModulesInSection / 4, countFl, levels);
            int counter = 0;
            int lastIdSection = 0;
            FlatInfo fl = new FlatInfo();
            for (int i = 0; i < flats.Count; i++)
            {
                var f = flats[i];
                if (f.ID_Section != lastIdSection)
                {
                    if (counter != 0)
                        sectionsBySyze.Add(fl);
                    fl = new FlatInfo();
                    fl.IdSection = f.ID_Section;
                    fl.Floors = countFloors;
                    fl.CountStep = countModulesInSection / 4;
                    fl.Flats = new List<RoomInfo>();
                    fl.IsCorner = isCornerLeftNiz | isCornerRightNiz;
                    lastIdSection = f.ID_Section;
                }
                var fflat = new RoomInfo(f.ShortType, f.SubZone, f.TypeFlat, f.AreaLive.ToString(),
                       f.AreaTotalStandart.ToString(),
                       f.AreaTotalStrong.ToString(), f.CountModules.ToString(), "",
                       "", f.LinkageBefore, f.LinkageAfter, "", "", "", f.Levels, "", "", f.LightBottom, f.LightTop,
                       "");
                fflat.SelectedIndexTop = f.SelectedIndexTop;
                fflat.SelectedIndexBottom = f.SelectedIndexBottom;
                fl.Flats.Add(fflat);
                //if (counter == 0 | lastIdSection != f.ID_Section)
                //{
                //    fl.IdSection = f.ID_Section;
                //    fl.Floors = countFloors;
                //    fl.CountStep = countModulesInSection / 4;
                //    fl.Flats = new List<RoomInfo>();
                //    fl.IsCorner = isCornerLeftNiz | isCornerRightNiz;
                //}

                // List<RoomInfo> secs = new List<RoomInfo>();
                //try
                //{
                // var flatsInSection = flats.Where(x => x.ID_Section.Equals(s.ID_Section)).OrderBy(x => x.ID_FlatInSection).ToList();
                //bool isValid = true;
                //bool is2KL2 = false;
                //foreach (var f in flatsInSection)
                //{


                //}
                //if (!isValid) continue;

                counter++;
                if (counter < 1000) continue;
                break;
            }
            //foreach (var s in sections)
            //{
            //    FlatInfo fl = new FlatInfo();
            //    fl.IdSection = s.ID_Section;
            //    fl.Floors = countFloors;
            //    fl.CountStep = countModulesInSection / 4;
            //    fl.Flats = new List<RoomInfo>();
            //    fl.IsCorner = isCornerLeftNiz | isCornerRightNiz;

            //    List<RoomInfo> secs = new List<RoomInfo>();
            //    //try
            //    //{
            //    var flatsInSection = flats.Where(x => x.ID_Section.Equals(s.ID_Section)).OrderBy(x => x.ID_FlatInSection).ToList();
            //    //bool isValid = true;
            //    //bool is2KL2 = false;
            //    foreach (var f in flatsInSection)
            //    {

            //        var fflat = new RoomInfo(f.ShortType, f.SubZone, f.TypeFlat, f.AreaLive.ToString(),
            //            f.AreaTotalStandart.ToString(),
            //            f.AreaTotalStrong.ToString(), f.CountModules.ToString(), "",
            //            "", f.LinkageBefore, f.LinkageAfter, "", "", "", f.Levels, "", "", f.LightBottom, f.LightTop,
            //            "");
            //        fflat.SelectedIndexTop = f.SelectedIndexTop;
            //        fflat.SelectedIndexBottom = f.SelectedIndexBottom;
            //        fl.Flats.Add(fflat);
            //    }
            //    //if (!isValid) continue;
            //    sectionsBySyze.Add(fl);
            //    counter++;
            //if (counter < 3000) continue;
            //break;
            //}
            return sectionsBySyze;
        }


        private void AddToListSections(List<FlatInfo> listSections, List<RoomInfo> listRooms1, List<RoomInfo> allRooms, bool isLeftCorner, int countFloors)
        {
            bool isExist = false;
            foreach (var section in listSections)
            {
                isExist =
                    section.Flats.OrderBy(x => x.Type).SequenceEqual(listRooms1.OrderBy(x => x.Type));
                if (!isExist) continue;
                break;
            }
            if (!listRooms1[listRooms1.Count - 1].LinkagePOSLE.Split('|')[2].Equals(
                listRooms1[0].LinkageDO.Split('|')[2]) & !listRooms1[listRooms1.Count - 1].LinkagePOSLE.Split('|')[2].Equals("E"))
                return;
            if (!isExist)
            {
                if (IsValidSmoke(listRooms1, allRooms, isLeftCorner, countFloors, listSections))
                {
                    if (listRooms1.Where(x => x.ShortType.Equals("2KL2")).ToList().Count > 1)
                    {
                        int a = 0;
                        int b = a+1;
                    }
                    FlatInfo fi = new FlatInfo();
                    fi.Flats = listRooms1;
                    //if (fi.Flats[1].ShortType == "1KS1" && fi.Flats[2].ShortType == "2KL3" && fi.Flats[fi.Flats.Count-2].ShortType == "2KL3")
                    //{
                    //    listSections.Add(fi);
                    //    int b = 0;
                    //}
                    listSections.Add(fi);
                }
            }
            return;
        }

        private List<RoomInfo> TempRoomInfo(List<RoomInfo> roomInfo, bool isCornerLeftNiz, bool isCornerRightNiz, string[] lastRoomConfig)
        {
            List<RoomInfo> tempRoomInfo = new List<RoomInfo>();
            int counter11 = 0;
            foreach (var ri in roomInfo)
            {
                counter11++;
                if (ri.IndexLenghtNIZ.Split('/')[0].Equals("!") | ri.SubZone.Equals("0"))
                    continue;
                if (ri.SubZone.Equals("4"))
                    continue;
                if (ri.TypeSection.Contains("Угл") & !isCornerLeftNiz)
                    continue;
                if (ri.TypeHouse.Contains("Баш"))
                    continue;
                string[] tempRoomConfig = ri.LinkageDO.Split('/')[0].Split('|');

                //if (ri.TypeSection.Equals("Право") & !isCornerRightNiz)
                //    continue;
                //if (ri.TypeSection.Equals("Лево") & !isCornerLeftNiz)
                //    continue;

                if (tempRoomConfig.Length < 3)
                    continue;
                if (lastRoomConfig[2].Equals("C"))
                {
                    if ((!tempRoomConfig[2].Equals("C") & !tempRoomConfig[2].Equals("D") &
                         !tempRoomConfig[2].Equals("P") & !tempRoomConfig[2].Equals("H")))
                        continue;
                    if (tempRoomConfig[2].Equals("D"))
                    {
                        if (Convert.ToInt16(lastRoomConfig[0]) < Convert.ToInt16(tempRoomConfig[0]))
                            continue;
                    }
                    else if ((!tempRoomConfig[2].Equals("C") & !tempRoomConfig[2].Equals("P")) &
                             Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("D"))
                {
                    if (!tempRoomConfig[2].Equals("C"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("H"))
                {
                    if (!tempRoomConfig[2].Equals("C") & !tempRoomConfig[2].Equals("N"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("F"))
                {
                    if (!tempRoomConfig[2].Equals("G") & !tempRoomConfig[2].Equals("K"))
                        continue;

                    if (!tempRoomConfig[2].Equals("K") &
                        Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("L"))
                {
                    if (!tempRoomConfig[2].Equals("M"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("K"))
                {
                    if (!tempRoomConfig[2].Equals("M") & !isCornerRightNiz)
                        continue;
                }
                else if (lastRoomConfig[2].Equals("J"))
                {
                    if (!tempRoomConfig[2].Equals("P"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("N"))
                {
                    if ((!tempRoomConfig[2].Equals("R") & !tempRoomConfig[2].Equals("H")))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("P"))
                {
                    if ((!tempRoomConfig[2].Equals("P") & !tempRoomConfig[2].Equals("J") &
                         !tempRoomConfig[2].Equals("C")))
                        continue;
                    if ((!tempRoomConfig[2].Equals("C") & !tempRoomConfig[2].Equals("P")) &
                        Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("E"))
                {
                    if (!tempRoomConfig[2].Equals("Q"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("Q"))
                {
                    if (!tempRoomConfig[2].Equals("Q") & !tempRoomConfig[2].Equals("E"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals("R"))
                {
                    if (!tempRoomConfig[2].Equals("N"))
                        continue;
                    if (Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0]))
                        continue;
                }
                else if (lastRoomConfig[2].Equals(tempRoomConfig[2]))
                {
                    if ((!tempRoomConfig[2].Equals("E") &
                         Convert.ToInt16(lastRoomConfig[0]) > Convert.ToInt16(tempRoomConfig[0])))
                        continue;
                }

                else
                {
                    continue;
                }
                tempRoomInfo.Add(ri);
            }
            return tempRoomInfo;
        }


        public int[] GetIndexes(int[] intMass, List<RoomInfo> tempInfo, List<RoomInfo> roomInfo1, int index)
        {
            if (index == -1)
                index = 0;
            string[] lastRoomConfig = tempInfo[index].LinkagePOSLE.Split('/')[0].Split('|');
            var variants = TempRoomInfo(roomInfo1, false, false, lastRoomConfig);
            if (intMass[index] >= variants.Count - 1)
            {
                intMass[index] = 0;
                GetIndexes(intMass, tempInfo, roomInfo1, index - 1);
            }
            else //if (intMass[index-1] <= variants.Count)
            {
                intMass[index]++;
                // ReSharper disable once RedundantCheckBeforeAssignment

                intMass[index + 1] = 0;
                intMass[index + 2] = 0;
            }
            //for (int i = tempInfo.Count - 2; i >= 0; i++)
            //{
            //    string[] lastRoomConfig = tempInfo[i].LinkagePOSLE.Split('/')[0].Split('|');
            //    var variants = TempRoomInfo(roomInfo1, false, false, lastRoomConfig);
            //    if (intMass[i] >= variants.Count)
            //    {
            //        intMass[i - 1] = 0;
            //    }
            //    else
            //    {
            //        intMass[i-1]++;
            //    }
            //}
            return intMass;
        }

        public bool IsValidRoom(RoomInfo selectedRoom, RoomInfo preRoom, List<RoomInfo> tempInfo, bool isLeftNiz, bool isRightNiz)
        {
            //if (selectedRoom.ShortType.Equals("2KL2"))
            //{
            //    int a = 1;
            //    int b = a + 1;
            //}
            if (selectedRoom.SubZone.Contains('!'))
                return false;
            if (selectedRoom.Requirment.Equals("0"))
                return false;
            if (preRoom == null)
                return true;
            if (selectedRoom.ShortType.Equals("2KL2") & preRoom.ShortType.Equals("2KL2"))
                return false;
            if (selectedRoom.IndexLenghtNIZ.Split('/')[0].Equals("!") | selectedRoom.SubZone.Equals("0"))
            {
                return false;
            }
            //if (selectedRoom.ShortType.Equals("2NM2"))
            //{
            //    return false;
            //}
            if (preRoom.ShortType.Equals(selectedRoom.ShortType))
            {
                string codeReal = selectedRoom.Type.Substring(selectedRoom.Type.Length - 2, 1);
                for (int i = tempInfo.Count - 1; i >= 0; i--)
                {
                    if (!tempInfo[i].ShortType.Equals(selectedRoom.ShortType))
                        break;
                    codeReal = tempInfo[i].Type.Substring(selectedRoom.Type.Length - 2, 1) + codeReal;
                }
                if (codeReal.Length > selectedRoom.OrderBuild.Length & !selectedRoom.OrderBuild.Equals(""))
                    return false;
                //if (selectedRoom.ShortType.Equals("1NS1"))
                //{
                //    if (codeReal.Equals("ZA"))
                //    { }
                //}
                if (!selectedRoom.OrderBuild.Equals(""))
                {
                    if (!codeReal.Substring(0, codeReal.Length)
                            .Equals(selectedRoom.OrderBuild.Substring(0, codeReal.Length)))
                        return false;
                }
                //string code = preRoom.Type.Substring(preRoom.Type.Length - 2, 1) + selectedRoom.Type.Substring(selectedRoom.Type.Length - 2, 1);
                //if (!code.Equals(selectedRoom.OrderBuild))
                //{
                //    var count = tempInfo.Where(x => x.ShortType.Equals(selectedRoom.ShortType)).ToList().Count;
                //}

            }
            bool isKM = preRoom.LinkagePOSLE.Contains("K") & selectedRoom.LinkageDO.Contains("M");
            bool isFk = preRoom.LinkagePOSLE.Contains("F") & selectedRoom.LinkageDO.Contains("K");
            // bool isFk = preRoom.LinkagePOSLE.Contains("F") & selectedRoom.LinkageDO.Contains("K");
            if (isKM & isLeftNiz)
            {
                var kms = tempInfo.Where(x => x.LinkagePOSLE.Contains("J")).ToList();
                if (kms.Count == 0)
                    return false;

            }
            if (isFk & isRightNiz)
            {
                var kms = tempInfo.Where(x => x.LinkageDO.Contains("J")).ToList();
                if (kms.Count == 0)
                    return false;

            }
            if (selectedRoom.Requirment.Equals("0"))
                return false;
            else if (!selectedRoom.Requirment.Equals(""))
            {
                string requirment = selectedRoom.Requirment;
                string[] reqs = Regex.Split(requirment, "<=");
                string firstSybmol = reqs[0].Trim();
                string secondSybmol = reqs[1].Trim();
                string[] doubleReq = firstSybmol.Split('+');
                if (doubleReq.Length == 2)   //например PIK1_2KL3_A0+PIK1_2KL3_Z0 <=1
                {
                    if (tempInfo.Where(y => y.Type.Equals(doubleReq[0]) | y.Type.Equals(doubleReq[1])).ToList

().Count > Convert.ToInt16(secondSybmol) - 1)
                        return false;
                }
                else if (firstSybmol.Equals(""))  //<=1
                {
                    if (tempInfo.Where(y => y.Type.Equals(selectedRoom.Type)).ToList().Count > Convert.ToInt16(secondSybmol) - 1)
                        return false;
                }
                else if (firstSybmol.Length == 1)   //H<=1
                {
                    if (tempInfo.Where(y => y.Type.Equals(selectedRoom.Type) & (selectedRoom.LinkageDO.Contains(firstSybmol) |
                        selectedRoom.LinkagePOSLE.Contains(firstSybmol))).ToList()
                        .Count > Convert.ToInt16(secondSybmol) - 1)
                        return false; ;
                }
            }

            return true;
        }

        public List<RoomInfo> GetRoomInfo(RoomInfo lastRoomInfo, List<RoomInfo> roomInfo1, List<RoomInfo> tempInfo, int[] intMass, RoomInfo preRoomInfo,
            int indexSummNiz, int indexSummTop, int summ, bool isLeftNiz, bool isRightNiz, int countLines)
        {
            FrameWork fw = new FrameWork();
            //try
            //{
            string[] lastRoomConfig = lastRoomInfo.LinkagePOSLE.Split('/')[0].Split('|');
            var variants = TempRoomInfo(roomInfo1, isLeftNiz, isRightNiz, lastRoomConfig);
            int indexRoom = tempInfo.Count;
            if (intMass[indexRoom - 1] > variants.Count - 1)
            {
                GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                IsExit = true;
            }
            lastRoomInfo = variants[intMass[indexRoom - 1]];


            if (tempInfo.Count > 1)
                preRoomInfo = tempInfo[tempInfo.Count - 1];

            if (!IsValidRoom(lastRoomInfo, preRoomInfo, tempInfo, isLeftNiz, isRightNiz))
            {
                if (isLeftNiz | !isRightNiz)
                    tempInfo.Add(lastRoomInfo);

                GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                IsExit = true;
            }

            if (!IsExit)
            {
                tempInfo.Add(lastRoomInfo);
                int indexNiz = fw.GetIndexSection(lastRoomInfo, preRoomInfo, true);
                int indexTop = fw.GetIndexSection(lastRoomInfo, preRoomInfo, false);
                lastRoomInfo.SelectedIndexTop = indexTop;
                lastRoomInfo.SelectedIndexBottom = indexNiz;
                indexSummNiz += indexNiz;
                indexSummTop += indexTop;
                summ += Convert.ToInt16(lastRoomInfo.AreaModules);

                if (isRightNiz)
                {
                    var finishRoom = roomInfo1.Where(x => x.Type.Equals("PIK1_3NL2_A0")).ToList()[0];
                    if (Math.Abs(summ + finishRoom.AreaModules - countLines).Equals(0))
                    {
                        if (!lastRoomInfo.LinkagePOSLE.Split('|')[2].Equals("P"))
                        {
                            IsExit = true;
                            GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 3);
                        }
                        else
                        {
                            lastRoomInfo = finishRoom;
                            tempInfo.Add(lastRoomInfo);
                            indexNiz = fw.GetIndexSection(lastRoomInfo, preRoomInfo, true);
                            indexTop = fw.GetIndexSection(lastRoomInfo, preRoomInfo, false);
                            lastRoomInfo.SelectedIndexTop = indexTop;
                            lastRoomInfo.SelectedIndexBottom = indexNiz;
                            indexSummNiz += indexNiz;
                            indexSummTop += indexTop;
                            summ += Convert.ToInt16(lastRoomInfo.AreaModules);
                        }

                    }
                    else if (summ + finishRoom.AreaModules - countLines > 0)
                    {
                        GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 3);
                        IsExit = true;
                    }
                }

                //  listRooms1.Add(tempRoom);


                if (!IsExit)
                {
                    if (tempInfo.Count > 10)
                    {
                        GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                        IsExit = true;
                    }
                    if (!IsExit)
                    {
                        if (countLines - summ > 0)
                        {
                            GetRoomInfo(lastRoomInfo, roomInfo1, tempInfo, intMass, preRoomInfo, indexSummNiz,
                                indexSummTop,
                                summ, isLeftNiz, isRightNiz, countLines);
                        }
                        else if (Math.Abs(summ - countLines).Equals(0))
                        {
                            if (!isLeftNiz & !isRightNiz)
                            {
                                if (!(indexSummNiz + indexSummTop == countLines / 2 & indexSummNiz == countLines / 2 / 2))
                                    IsExit = true;
                                GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                            }
                            else if (isLeftNiz | isRightNiz)
                            {
                                if (!((summ / 4 - 4) == indexSummTop & (summ / 4 - 4) + 7 == indexSummNiz)) //угловая
                                    IsExit = true;

                                GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                                //проверить правую угловую тут


                            }
                            else if (isRightNiz)
                            {

                            }
                        }
                        else if (summ - countLines > 0)
                        {
                            GetIndexes(intMass, tempInfo, roomInfo1, tempInfo.Count - 2);
                            IsExit = true;
                        }

                    }
                }
            }
            //}
            //catch
            //{
            //    return null;
            //}
            if (IsExit)
                tempInfo.Clear();
            return tempInfo;

        }


        private bool IsValidSmoke(List<RoomInfo> listRooms1, List<RoomInfo> allRooms, bool isCorner, int countFloors, List<FlatInfo> allsections)
        {

            if (listRooms1.Count > 7)
            {
                if (listRooms1[3].ShortType.Equals("2NM2"))
                {

                }

                RoomInfo rr = allRooms.Where(x => x.SubZone.Equals("0"))
                          .Where(x => x.LevelsSection.Equals("10-18")).Where(x=>x.Requirment!="0")
                          .Where(x => x.TypeSection.Equals("Рядовая")).Where(x => x.TypeHouse.Equals("Секционный")).Where(x => !x.FactorSmoke.Equals(""))
                          .ToList()[0];
                if (countFloors == 25)
                {
                    rr = allRooms.Where(x => x.SubZone.Equals("0")).Where(x => x.Requirment != "0")
                           .Where(x => x.LevelsSection.Equals("19-25"))
                           .Where(x => x.TypeSection.Equals("Рядовая")).Where(x => x.TypeHouse.Equals("Секционный")).Where(x => !x.FactorSmoke.Equals(""))
                           .ToList()[0];
                }
                if (isCorner)
                {
                    rr = allRooms.Where(x => x.SubZone.Equals("0")).Where(x => x.Requirment != "0")
                          .Where(x => x.LevelsSection.Equals("10-18"))
                          .Where(x => x.TypeSection.Equals("Угловая-лево")).Where(x => x.TypeHouse.Equals("Секционный")).Where(x => x.Requirment != "0").Where(x => !x.FactorSmoke.Equals(""))
                          .ToList()[0];
                }
                listRooms1[0] = rr;
                listRooms1[0].SelectedIndexTop = Convert.ToInt16(rr.IndexLenghtTOP);
                listRooms1[0].SelectedIndexBottom = Convert.ToInt16(rr.SelectedIndexBottom);
                int countTop = 0;
                int counter = 0;
                if (isCorner)
                {
                    while (listRooms1[counter].SelectedIndexTop > 0)
                    {
                        if (counter == 0)
                        {
                            string indexSmoke = listRooms1[counter].FactorSmoke;
                            string[] masSmoke = indexSmoke.Split('|');
                            if (masSmoke.Length == 2)
                                countTop += Convert.ToInt16(masSmoke[0]);
                            counter++;
                            continue;
                        }
                        countTop += listRooms1[counter].SelectedIndexTop;
                        counter++;
                    }
                    counter--;
                }
                int countBottom = 0;
                while (listRooms1[counter].SelectedIndexBottom >= 0)
                {

                    string indexSmoke = listRooms1[counter].FactorSmoke;
                    if (indexSmoke.Equals(""))
                    {
                        countBottom += listRooms1[counter].SelectedIndexBottom;
                        counter++;
                        if (listRooms1.Count == counter)
                            break;
                        continue;
                    }
                    countBottom += Convert.ToInt16(indexSmoke.Split('|')[0]);
                    if (isCorner)
                        countTop = 7 + Convert.ToInt16(listRooms1[0].FactorSmoke.Split('|')[0]);//только ЛЛУ
                    if (countBottom == countTop)
                    {
                        string indexSmokeLLU = listRooms1[0].FactorSmoke;
                        if (indexSmokeLLU.Split('|')[1] == indexSmoke.Split('|')[1])
                            return false;
                    }
                    else if (countBottom > countTop)
                        break;
                    else countBottom -= Convert.ToInt16(indexSmoke.Split('|')[0]);
                    countBottom += listRooms1[counter].SelectedIndexBottom;
                    counter++;
                    if (counter == listRooms1.Count)
                        break;

                }
            }
            return true;
        }

        int GetRandom(Random random, List<RoomInfo> roomInfo, RoomInfo lastRoom, List<RoomInfo> selectedRooms, bool isRightNiz, bool isLeftNiz)
        {
            int r = 0;
            bool isValid = false;
            for (int i = 0; i < roomInfo.Count; i++)
            {
                r = random.Next(0, roomInfo.Count);
                if (!IsValidRoom(roomInfo[r], lastRoom, selectedRooms, isLeftNiz, isRightNiz))
                    continue;
                return r;
            }
            if (!isValid)
            {
                return -100;
            }
            return r;
        }
    }
}
