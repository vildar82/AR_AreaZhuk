using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AR_AreaZhuk.Controller;
using AR_AreaZhuk.Model;
using AR_AreaZhuk.PIK1TableAdapters;
using OfficeOpenXml.Drawing.Chart;
using AR_Zhuk_DataModel;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using OfficeOpenXml;
using AR_AreaZhuk.Insolation;
using AR_AreaZhuk.Scheme;

namespace AR_AreaZhuk
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public bool IsRemainingDominants { get; set; }
        public int DominantOffSet { get; set; }

        public static List<SpotInfo> spinfos = new List<SpotInfo>();
        public string PathToFileInsulation { get; set; }
        public static int offset = 5;
        public static bool isSave = false;
        public static int countGood = 0;
        public static bool IsExit = false;
        public static bool isContinue = true;
        public static bool isContinue2 = true;
        public static SpotInfo spotInfo = new SpotInfo();
        public static List<List<HouseInfo>> houses = new List<List<HouseInfo>>();
        public static List<GeneralObject> ob = new List<GeneralObject>();

        public void ViewProgress()
        {
            FormProgress fp = new FormProgress();
            fp.ShowDialog();
        }
        private void MainForm_Load (object sender, EventArgs e)
        {
            //btnStartScan.Enabled = true;
            //PathToFileInsulation = @"c:\Задание по инсоляции ПИК1.xlsx";
            chkEnableDominant.Checked = true;
            chkListP1.SetItemChecked(chkListP1.Items.Count - 1, true);
            chkListP2.SetItemChecked(chkListP2.Items.Count - 1, true);

            btnMenuGroup1.Image = Properties.Resources.up;
            btnMenuGroup2.Image = Properties.Resources.up;
            btnMenuGroup3.Image = Properties.Resources.up;
            //  pnlMenuGroup2.Height = 25;
            //  pnlMenuGroup3.Height = 25;
            //  Exporter.ExportFlatsToSQL();
            //   Exporter.ExportSectionsToSQL(56, "Рядовая", 25, false, false);
            //  Requirment requirment = new Requirment();
            // this.pb.Image = global::AR_AreaZhuk.Properties.Resources.объект;
            FrameWork fw = new FrameWork();
            //var roomInfo = fw.GetRoomData("");
            spotInfo = fw.GetSpotInformation();
            foreach (var r in spotInfo.requirments)
            {
                dg.Rows.Add();
                dg[0, dg.RowCount - 1].Value = r.SubZone;
                dg[1, dg.RowCount - 1].Value = r.MinArea + "-" + r.MaxArea;
                dg[2, dg.RowCount - 1].Value = r.Percentage;
                dg[3, dg.RowCount - 1].Value = r.OffSet;
            }

            int per = 0;
            for (int i = 0; i < dg.RowCount; i++)
            {
                per += Convert.ToInt16(dg[2, i].Value);
            }
            dg.Rows.Add();
            dg[1, dg.RowCount - 1].Value = "Всего:";
            dg[2, dg.RowCount - 1].Value = per;
            isEvent = true;
        }

        public bool IsEqualSections(List<RoomInfo> section1, List<RoomInfo> section2)
        {
            if (section1.Count != section2.Count) return false;
            foreach (var flat1 in section1)
            {
                int countInSection1 = section1.Where(x => x.ShortType.Equals(flat1.ShortType)).ToList().Count();
                int countInSection2 = section2.Where(x => x.ShortType.Equals(flat1.ShortType)).ToList().Count();
                if (countInSection1 != countInSection2)
                    return false;

            }
            return true;
        }

        
        public void GetAllSectionPercentage(List<List<HouseInfo>> listSections, Requirment requirment)
        {
            int counter = 0;
            int allCounter = 0;
            bool isContinue = true;
            houses = listSections;
            //   label3.Text = DateTime.Now.ToString();
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 15;
            Parallel.For(0, houses[0].Count, po, GetGeneralObjects);
            // GetGeneralObjects(0);

        }

        private void GetGeneralObjects(int startIndex)
        {

            var house1 = houses[0][startIndex];
            var spot1 = house1.SpotInf;
            for (int j = 0; j < houses[1].Count; j++)
            {
                // countGood++;
                var house2 = houses[1][j];
                if (IsRemainingDominants)
                {
                    int offsetDom = DominantOffSet;
                    int remaining =
                        Math.Abs(house1.Sections[house1.Sections.Count - 1].CountStep -
                                 house2.Sections[house2.Sections.Count - 1].CountStep);
                    if (remaining > offsetDom)
                        continue;
                }
                bool isValid = false;

                var spot2 = house2.SpotInf;
                int allCountFlats = 0;
                for (int k = 0; k < spotInfo.requirments.Count; k++)
                {
                    allCountFlats += spot1.requirments[k].RealCountFlats;
                    allCountFlats += spot2.requirments[k].RealCountFlats;
                }
                SpotInfo spGo = new SpotInfo();
                spGo = spot1.CopySpotInfo(spotInfo);
                // dg2.Rows.Add();
                int countValid = 0;
                for (int k = 0; k < spotInfo.requirments.Count; k++)
                {
                    //if ()
                    //{
                    //    isValid = true;
                    //    continue;
                    //}

                    int currentCountFlats = spot1.requirments[k].RealCountFlats;
                    currentCountFlats += spot2.requirments[k].RealCountFlats;
                    double percentOb = Convert.ToDouble(currentCountFlats) / Convert.ToDouble(allCountFlats) * 100;
                    spGo.requirments[k].RealCountFlats = currentCountFlats;
                    spGo.requirments[k].RealPercentage = percentOb;
                    if (spotInfo.requirments[k].OffSet == 0)
                        continue;
                    if (Math.Abs(percentOb - spotInfo.requirments[k].Percentage) > spotInfo.requirments[k].OffSet &
                        spotInfo.requirments[k].Percentage != 0)
                    {
                        // countValid++;
                        isValid = false;
                        break;
                    }
                    isValid = true;
                    continue;

                }


                //  dg2[dg2.RowCount - 1, 0].Value = infoPercent;
                if (!isValid) continue;
                string guid = Guid.NewGuid().ToString();
                spGo.GUID = guid;
                spGo.TotalFlats = allCountFlats;
                spinfos.Add(spGo);
                int countS = house1.Sections.Count + house2.Sections.Count;
                int[] ss = new int[countS];
                for (int i = 0; i < house1.Sections.Count; i++)
                {
                    ss[i] = house1.Sections[i].IdSection;
                }
                for (int i = 0; i < house2.Sections.Count; i++)
                {
                    ss[i + house1.Sections.Count] = house2.Sections[i].IdSection;
                }
                string typicalSect = "";
                int countSovp = countS - ss.GroupBy(x => x).ToList().Count + 1;
                for (int i = 0; i < ss.Length; i++)
                {

                    int v = ss[i];
                    if (v == 0)
                        continue;
                    int t = 1;

                    for (int m = i + 1; m < ss.Length; m++)
                    {
                        if (v != ss[m]) continue;
                        t++;
                        ss[m] = 0;
                    }
                    if (t == 1) continue;
                    typicalSect += t + ";";
                }
                typicalSect = typicalSect.Remove(typicalSect.Length - 1, 1);

                GeneralObject go = new GeneralObject();
                spGo.TotalSections = countS;
                spGo.TypicalSections = typicalSect;
                go.SpotInf = spGo;
                double area = GetTotalArea(house1);
                area += GetTotalArea(house2);
                go.Houses.Add(house1);
                go.Houses.Add(house2);
                go.SpotInf.RealArea = area;
                go.GUID = guid;
                ob.Add(go);
            }

        }

        private double GetTotalArea(HouseInfo house1)
        {
            double totalArea = 0;

            foreach (var section in house1.Sections)
            {
                double sectionArea = 0;
                int countFloors = 18;
                if (section.Flats[0].ShortType.Contains("25"))
                    countFloors = 25;
                foreach (var f in section.Flats)
                {
                    if (countFloors == 25)
                    {
                        sectionArea += 9 * f.AreaTotal;
                        sectionArea += 15 * f.AreaTotalStandart;
                    }
                    else
                    {
                        // sectionArea += 4 * f.AreaTotal;
                        sectionArea += 14 * f.AreaTotalStandart;
                    }
                }
                totalArea += sectionArea;
                section.Area = sectionArea;
            }
            return totalArea;
        }
        
        public bool SetIndexesSection(int[] indexes, int[] sizes, int index, List<HouseInfo> listSections)
        {
            if (index == 0)
            {
                isContinue2 = false;
                return false;
            }
            indexes[index] = 0;
            indexes[index - 1]++;

            if (indexes[index - 1] >= listSections[index - 1].Sections.Count)
            {
                SetIndexesSection(indexes, sizes, index - 1, listSections);
            }
            return true;
        }

        public bool SetIndexesSize(int[] indexes, int index, int[] masSizes)//List<HouseInfo> listSections)
        {
            if (index == 0)
            {
                isContinue = false;
                return false;
            }
            indexes[index] = 0;
            indexes[index - 1]++;

            if (indexes[index - 1] >= masSizes.Length)
            {
                SetIndexesSize(indexes, index - 1, masSizes);
            }
            return isContinue;
        }

        public void GetHousePercentage(ref HouseInfo houseInfo, SpotInfo sp1, InsolationSpot insulation)
        {
            sp1 = sp1.CopySpotInfo(spotInfo);
            for (int k = 0; k < houseInfo.Sections.Count; k++) //Квартиры
            {                
                FlatInfo section = houseInfo.Sections[k];
                double areaSection = 0;
                for (int l = 0; l < section.Flats.Count; l++) //Квартиры
                {
                    if (section.Flats[l].SubZone.Equals("0")) continue;
                    var reqs =
                        sp1.requirments.Where(
                            x => x.SubZone.Equals(section.Flats[l].SubZone))
                            .Where(x => x.MaxArea + 5 >= section.Flats[l].AreaTotal & x.MinArea - 5 <= section.Flats[l].AreaTotal)
                            .ToList();
                    if (reqs.Count == 0) continue;
                    reqs[0].RealCountFlats += section.Floors - 1;

                }
            }
            int countFlats = 0;
            foreach (var r in sp1.requirments)
            {
                countFlats += r.RealCountFlats;
            }
            foreach (var r in sp1.requirments)
            {
                double percentage = r.RealCountFlats * 100 / countFlats;
                r.RealPercentage = percentage;
            }
            houseInfo.SpotInf = sp1;
        }
        private static int GetCountHouses(DataGridView dg, bool isSave)
        {
            ob = new List<GeneralObject>();
            FrameWork fw = new FrameWork();
            //  var roomInfo = fw.GetRoomData("");
            //  spotInfo = fw.GetSpotInformation(roomInfo);

            int countHouses = 0;
            for (int i = 0; i < dg.RowCount; i++)
            {
                string[] parse = dg[1, i].Value.ToString().Split('-');
                spotInfo.requirments.Where(x => x.SubZone.Equals(dg[0, i].Value.ToString()))
                    .Where(x => x.MinArea.ToString().Equals(parse[0]))
                    .ToList()[0].Percentage =
                    Convert.ToInt16(dg[2, i].Value);

            }

            for (int l = 0; l < houses[0].Count; l++)
            {
                var house1 = houses[0][l];
                var spot1 = house1.SpotInf;
                for (int j = 0; j < houses[1].Count; j++)
                {
                    countGood++;
                    var house2 = houses[1][j];
                    bool isValid = false;

                    var spot2 = house2.SpotInf;
                    for (int k = 0; k < spotInfo.requirments.Count; k++)
                    {
                        double realPercentage = spot1.requirments[k].RealPercentage +
                                                spot2.requirments[k].RealPercentage;
                        if (Math.Abs(spotInfo.requirments[k].Percentage - realPercentage / 2) > offset)
                        {
                            isValid = false;
                            break;
                        }
                        isValid = true;
                        continue;

                    }
                    if (!isValid) continue;
                    int countS = house1.Sections.Count + house2.Sections.Count;
                    int[] ss = new int[countS];
                    for (int i = 0; i < house1.Sections.Count; i++)
                    {
                        ss[i] = house1.Sections[i].IdSection;
                    }
                    for (int i = 0; i < house2.Sections.Count; i++)
                    {
                        ss[i + house1.Sections.Count] = house2.Sections[i].IdSection;
                    }
                    int countSovp = countS - ss.GroupBy(x => x).ToList().Count + 1;
                    GeneralObject go = new GeneralObject();
                    go.Houses.Add(house1);
                    go.Houses.Add(house2);
                    ob.Add(go);
                    if (isSave)
                    {
                        Serializer ser = new Serializer();
                        ser.SerializeList(go,
                            house1.Sections.Count.ToString() + "-" + house2.Sections.Count.ToString() + "- (" +
                            countSovp.ToString() + ") - " + countGood.ToString());
                    }
                }
            }

            return ob.Count;
        }

        private List<Requirment> GetRequirments(List<Requirment> requirments)
        {
            return requirments.ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spinfos.Clear();
            ob.Clear();
            FormManager.GetSpotTaskFromDG(spotInfo, dg);
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 15;
            Parallel.For(0, houses[0].Count, po, GetGeneralObjects);
            // GetGeneralObjects(0);
            FormManager.ViewDataProcentage(dg2, spinfos);
            lblCountObjects.Text = ob.Count.ToString();
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            List<string> guids = (from DataGridViewRow row in dg2.SelectedRows select dg2[dg2.Columns.Count - 1, row.Index].Value.ToString()).ToList();
            foreach (var g in guids)
            {
                GeneralObject go = ob.First(x => x != null && x.SpotInf.GUID.Equals(g));
                if (go == null) break;
                Serializer ser = new Serializer();
                ser.SerializeList(go, go.SpotInf.RealArea + "m2 (" + go.SpotInf.TotalFlats.ToString() + ")");

            }

        }

        private static bool isEvent = false;
        private void dg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isEvent) return;
            int per = 0;
            for (int i = 0; i < dg.RowCount - 1; i++)
            {
                per += Convert.ToInt16(dg[2, i].Value);
            }
            //if (per != 0)
            dg[2, dg.RowCount - 1].Value = per;
        }

        private void btnStartScan_Click(object sender, EventArgs e)
        {
            btnStartScan.Enabled = false;
            btnViewPercentsge.Enabled = true;
            Requirment requirment = new Requirment();
            FrameWork fw = new FrameWork();
            //  var roomInfo = fw.GetRoomData("");
            spotInfo = fw.GetSpotInformation();
            FormManager.GetSpotTaskFromDG(spotInfo, dg);
            var insulations = fw.GetInsulations(PathToFileInsulation);
            for (int i = 0; i < insulations.Count; i++)
            {
                insulations[i].CountFloorsDominant = Convert.ToInt16(numDomCountFloor.Value);
                insulations[i].CountFloorsMain = Convert.ToInt16(numMainCountFloor.Value);
                for (int j = 0; j < 5; j++)
                {
                    insulations[i].DominantPositions.Add(((CheckedListBox)this.Controls.Find("chkListP" + (i + 1).ToString(), true)[0]).GetItemChecked(j));
                }
            }

            List<Section> dbSections = new List<Section>();
            Thread th = new Thread(ViewProgress);
            th.Start();
            for (int k = 0; k < insulations.Count; k++)
            {

                if (!dbSections.Any(x => x.Floors.Equals(insulations[k].CountFloorsMain) & x.IsLeftBottomCorner == false & x.IsRightBottomCorner == false))
                    Parallel.For(7, 15, (q) => GetDBSections(q, insulations[k], fw, dbSections, insulations[k].CountFloorsMain, false, false));
                if (!dbSections.Any(x => x.Floors.Equals(insulations[k].CountFloorsDominant) & x.IsLeftBottomCorner == false & x.IsRightBottomCorner == false))
                    if (chkEnableDominant.Checked & insulations[k].CountFloorsDominant != insulations[k].CountFloorsMain)
                        Parallel.For(7, 15, (q) => GetDBSections(q, insulations[k], fw, dbSections, insulations[k].CountFloorsDominant, false, false));


                if (!dbSections.Any(x => x.Floors.Equals(insulations[k].CountFloorsMain) & x.IsLeftBottomCorner == insulations[k].IsLeftNizSection
                    & x.IsRightBottomCorner == insulations[k].IsRightNizSection))
                    if (insulations[k].IsLeftNizSection | insulations[k].IsRightNizSection)
                        Parallel.For(7, 15, (q) => GetDBSections(q, insulations[k], fw, dbSections,
                            insulations[k].CountFloorsMain, insulations[k].IsLeftNizSection, insulations[k].IsRightNizSection));

                // fw.GetAllSectionsFromDB(36,false,false,18);


            }

            List<List<HouseInfo>> totalObject = new List<List<HouseInfo>>();
            List<List<HouseInfo>> houses = new List<List<HouseInfo>>();
            List<HouseInfo> listSections = new List<HouseInfo>();
            List<List<FlatInfo>> sectionsInHouse = new List<List<FlatInfo>>();
            bool isContinue1 = true;
            bool isContinue2 = true;
            int[] masSizes = new int[] { 28, 32, 36, 40, 44, 48, 52, 56 };
            int counterr = 0;
            foreach (var insulation in insulations)
            {
                counterr++;
                int minimalCountSection = 10;
                List<HouseInfo> variantHouses = new List<HouseInfo>();
                MainForm.isContinue = true;
                List<HouseInfo> housesTemp = new List<HouseInfo>();
                SpotInfo sp = new SpotInfo();
                int[] indexSelectedSize = new int[15];
                int[] indexSelectedSection = new int[15];
                
                indexSelectedSize = new int[15];
                isContinue1 = true;
                while (isContinue1)
                {
                    int countModulesTotal = Convert.ToInt16(sp.SpotArea / 12.96);
                    List<HouseInfo> sectionInfos = new List<HouseInfo>();
                    for (int i = 0; i < 10; i++)
                    {
                        //if (i == 0)
                        //{
                        //    //indexSelectedSize[0] = 2;
                        //    //if()
                        //}
                        countModulesTotal = countModulesTotal -
                                            masSizes[indexSelectedSize[i]];
                        if (countModulesTotal == 0)
                        {
                            HouseInfo variantHouse = new HouseInfo();
                            variantHouse.Sections = new List<FlatInfo>();
                            bool isValid = true;
                            for (int j = 0; j < listSections.Count; j++)
                            {
                                if (listSections[j].Sections.Count > 0)
                                    continue;
                                isValid = false;
                                break;
                            }
                            if (isValid)
                            {
                                isContinue2 = true;
                                List<Section> sectionsGood = new List<Section>();
                                for (int j = 0; j <= i; j++)
                                {
                                    bool isDominant = false;
                                    //  bool isDominant = insulation.DominantPositions[j];
                                    if (j <= 3 | (i - j) < 2)
                                    {
                                        if (j <= 3)
                                            isDominant = insulation.DominantPositions[j];
                                        if (i - j == 1)
                                            isDominant = insulation.DominantPositions[3];
                                        if (i - j == 0)
                                            isDominant = insulation.DominantPositions[4];
                                    }
                                    int countF = 0;
                                    if (isDominant)
                                        countF = insulation.CountFloorsDominant;
                                    else countF = insulation.CountFloorsMain;
                                    if ((insulation.IsLeftNizSection | insulation.IsRightNizSection) & j == 1)
                                    {
                                        var list =
                                            dbSections.Where(
                                                x =>
                                                    x.Floors.Equals(insulation.CountFloorsMain) &
                                                    x.IsLeftBottomCorner == insulation.IsLeftNizSection &
                                                    x.IsRightBottomCorner == insulation.IsRightNizSection &
                                                    x.CountModules.Equals(masSizes[indexSelectedSize[j]]))
                                                .ToList();
                                        if (list.Count == 0)
                                            break;
                                        sectionsGood.Add(list[0]);
                                    }
                                    else
                                    {
                                        var list =
                                          dbSections.Where(
                                              x =>
                                                  x.Floors.Equals(countF) &
                                                  x.IsLeftBottomCorner == false &
                                                  x.IsRightBottomCorner == false &
                                                  x.CountModules.Equals(masSizes[indexSelectedSize[j]]))
                                              .ToList();
                                        if (list.Count == 0)
                                            break;
                                        sectionsGood.Add(list[0]);
                                    }

                                }
                                if (sectionsGood.Count != i + 1)
                                {
                                    continue;
                                }

                                bool isGoGood = true;
                                for (int j = 0; j <= i; j++)
                                {
                                    if (sectionsGood[j].Sections.Count == 0)
                                    {
                                        isGoGood = false;
                                        break;
                                    }
                                }
                                if (!isGoGood)
                                {
                                    indexSelectedSize[i]++;
                                    if (indexSelectedSize[i] >= masSizes.Length)
                                    {
                                        if (!SetIndexesSize(indexSelectedSize, i, masSizes))
                                            isContinue1 = false;
                                    }
                                    break;
                                }
                                bool isContinue = true;
                                bool isFirstSection = true;
                                bool isRightOrTopLLu = true;
                                bool isVertical = true;
                                bool isCorner = false;
                                int countEnter = 1;
                                int indexRowStart = 0; //10;
                                int indexColumnStart = 0; //9;
                                int heightSection = 0;
                                int cornerCountLinesFirstSection = 0;
                                if (insulation.IsLeftNizSection)
                                {
                                    cornerCountLinesFirstSection = insulation.MaxLeftXY[1] - insulation.MinLeftXY[1] + 1 - 5;
                                    if (masSizes[indexSelectedSize[0]] / 4 != cornerCountLinesFirstSection)
                                    {
                                        indexSelectedSize[i]++;
                                        if (indexSelectedSize[i] >= masSizes.Length)
                                        {
                                            if (!SetIndexesSize(indexSelectedSize, i, masSizes))
                                                isContinue1 = false;
                                        }
                                        break;
                                    }
                                    indexRowStart = cornerCountLinesFirstSection + insulation.MinLeftXY[1];
                                    indexColumnStart = insulation.MinLeftXY[0];
                                }
                                else if (insulation.IsRightNizSection)
                                {
                                    cornerCountLinesFirstSection = insulation.MaxRightXY[1] - insulation.MinRightXY[1] + 1 - 5;
                                    if (masSizes[indexSelectedSize[0]] / 4 != cornerCountLinesFirstSection)
                                    {
                                        indexSelectedSize[i]++;
                                        if (indexSelectedSize[i] >= masSizes.Length)
                                        {
                                            if (!SetIndexesSize(indexSelectedSize, i, masSizes))
                                                isContinue1 = false;
                                        }
                                        break;
                                    }
                                    indexRowStart = cornerCountLinesFirstSection + insulation.MinRightXY[1];
                                    indexColumnStart = insulation.MaxRightXY[0];
                                }

                                int tempColumnIndex = 0;

                                //foreach (var sectionGood in sectionsGood)
                                //{
                                List<Section> secInHouse = new List<Section>();
                                HouseInfo hhh = new HouseInfo();
                                bool isEmptySection = false;
                                for (int m = 0; m < sectionsGood.Count; m++)
                                {
                                    var sectionGood = sectionsGood[m];
                                    // Section sssss = new Section();
                                    //    sssss.Sections= new List<FlatInfo>();


                                    isCorner = sectionGood.IsLeftBottomCorner | sectionGood.IsRightBottomCorner;
                                    HouseInfo s = new HouseInfo();
                                    s.Sections = new List<FlatInfo>();
                                    if (countEnter == 2)
                                        isVertical = false;
                                    Section s1 = new Section();

                                    s1.CountModules = sectionGood.CountModules;
                                    if (countEnter > 1)
                                    {
                                        //тут будут условия в зависимости от расположения угловой секции
                                        if (insulation.IsLeftNizSection)
                                        {
                                            indexRowStart = insulation.MaxLeftXY[1] - 3;
                                            indexColumnStart += sectionGood.CountModules / 4;
                                            if (isCorner)
                                                indexColumnStart--;
                                        }
                                        if (insulation.IsRightNizSection)
                                        {
                                            indexRowStart = insulation.MaxRightXY[1] - 3;
                                            if (countEnter > 2)
                                                indexColumnStart -= sectionGood.CountModules / 4 + 1;
                                            if (indexColumnStart - sectionGood.CountModules / 4 < 0 ||
                                                insulation.Matrix[indexColumnStart - sectionGood.CountModules / 4, indexRowStart].Equals(""))
                                            {
                                                isContinue = false;
                                                break;
                                            }
                                        }
                                    }
                                    List<FlatInfo> sections = new List<FlatInfo>();
                                    sections = sectionGood.Sections;
                                    if (sections.Count > 0)
                                    {
                                        Cell cellStart;
                                        if (insulation.Name == "P1|")
                                        {
                                            cellStart = new Cell(0, 0);
                                        }
                                        else
                                        {
                                            cellStart = new Cell(16, 0);
                                        }
                                        var listSections1 = insulation.GetInsulationSections(sections, isVertical, isCorner, m + 1, i + 1,
                                            spotInfo, cellStart);
                                        s1 = listSections1;
                                    }

                                    countEnter++;
                                    if (s1.Sections.Count == 0)
                                    {
                                        isEmptySection = true;
                                        break;
                                    }
                                    s.Sections = s1.Sections;
                                    sectionInfos.Add(s);
                                    // sssss = s1;
                                    secInHouse.Add(s1);
                                    // variantHouses.Add(s);

                                }

                                //indexSelectedSection = new int[15];
                                //while (isContinue2)
                                //{
                                //if (sectionInfos.Count > 4 & counterr == 1)
                                //    break;
                                //if (sectionInfos.Count < i + 1)
                                //    break;                                    
                                try
                                {
                                    int sectBySize = sectionInfos.Max(s => s.Sections.Count);
                                    for (int s = 0; s < sectBySize; s++)
                                    {
                                        HouseInfo hi = new HouseInfo();
                                        hi.Sections = new List<FlatInfo>();

                                        for (int j = 0; j <= i; j++)
                                        {
                                            int index = s;
                                            var sect = sectionInfos[j];
                                            if (index > sect.Sections.Count - 1)
                                            {
                                                index = sect.Sections.Count - 1;
                                            }
                                            hi.Sections.Add(sectionInfos[j].Sections[index]);

                                            //if (sectionInfos[j].Sections.Count == 0 || indexSelectedSection[j] >= sectionInfos[j].Sections.Count)
                                            //{
                                            //    isContinue2 = false;
                                            //    break;
                                            //}
                                            ////  if ()
                                            //hi.Sections.Add(sectionInfos[j].Sections[indexSelectedSection[j]]);
                                            //  else continue;
                                            // if (i != hi.Sections.Count)
                                            // break;
                                            //ids[j] = hi.Sections[j].IdSection;
                                        }
                                        Test.CreateHouseImage.TestCreateImage(hi);
                                        housesTemp.Add(hi);
                                    }

                                    //if (!isContinue2)
                                    //    break;
                                    //if (ids.GroupBy(x => x).ToList().Count == hi.Sections.Count)         //Отсев секций без повторений
                                    //{
                                    //    indexSelectedSection[i]++;
                                    //    if (indexSelectedSection[i] >= sectionInfos[i].Sections.Count)
                                    //    {
                                    //        if (!SetIndexesSection(indexSelectedSection, indexSelectedSize, i, sectionInfos))
                                    //            isContinue2 = false;
                                    //    }
                                    //    continue;
                                    //}
                                    //else
                                    //{
                                    //GetHousePercentage(ref hi, spotInfo, insulation);
                                    //housesTemp.Add(hi);
                                    //indexSelectedSection[i]++;
                                    //if (indexSelectedSection[i] >= sectionInfos[i].Sections.Count)
                                    //{
                                    //    if (!SetIndexesSection(indexSelectedSection, indexSelectedSize, i, sectionInfos))
                                    //        isContinue2 = false;
                                    //}
                                    //}
                                }
                                catch
                                {
                                    break;
                                }
                                //}
                            }
                            indexSelectedSize[i]++;
                            if (indexSelectedSize[i] >= masSizes.Length)
                            {
                                if (!SetIndexesSize(indexSelectedSize, i, masSizes))
                                    isContinue1 = false;
                            }
                            break;
                        }
                        else if (countModulesTotal < 32)
                        {
                            indexSelectedSize[i]++;
                            if (indexSelectedSize[i] >= masSizes.Length)
                            {
                                if (!SetIndexesSize(indexSelectedSize, i, masSizes))
                                    isContinue1 = false;
                            }
                            break;
                        }
                    }
                }
                //foreach (var v in variantHouses)
                //{
                //    foreach (var s in v.SectionsBySize)
                //    {
                //        s.Sections = s.Sections.OrderByDescending(x => x.Code).ToList();
                //    }
                //}
                //  if (variantHouses[0].SectionsBySize.Count <= minimalCountSection)
               // totalObject.Add(variantHouses);


                  totalObject.Add(housesTemp);
            }

            //for (int i = 0; i < totalObject[0].Count; i++)
            //{
            //    List<SectionInformation> sections = new List<SectionInformation>();
            //    foreach (var sec1 in totalObject[0][i].SectionsBySize)
            //    {

            //        foreach (var sec11 in sec1.Sections)
            //        {
            //            sections.Add(sec11);
            //        }

            //    }
            //    List<Section> sectionsInHouses = new List<Section>();
            //    for (int j = 0; j < totalObject[1].Count; j++)
            //    {
            //        foreach (var sec2 in totalObject[1][j].SectionsBySize)
            //        {
            //            sectionsInHouses.Add(sec2);
            //        }
            //    }

            //}

             //GetAllSectionPercentage(totalObject, requirment);




            //for (int k = 0; k < totalObject[0].Count; k++)
            //{
            //    for (int l = 0; l < totalObject[1].Count; l++)
            //    {
            //        List<List<FlatInfo>> sections = new List<List<FlatInfo>>();
            //        foreach (var s in totalObject[0][k].SectionsBySize)
            //        {
            //            sections.Add(s.Sections);
            //        }
            //        foreach (var s in totalObject[1][l].SectionsBySize)
            //        {
            //            sections.Add(s.Sections);
            //        }
            //        List<CodeSection> codeSections = new List<CodeSection>();
            //        int counter = 0;
            //        foreach (var ss in sections)
            //        {
            //            List<Code> codes = new List<Code>();
            //            CodeSection codeSection = new CodeSection();
            //            codeSection.CountFloors = ss[0].Floors;
            //            foreach (var s in ss.OrderByDescending(x=>x.CountFlats))
            //            {

            //                if (codes.Any(x => x.CodeStr.Equals(s.Code)))
            //                    codes.First(x => x.CodeStr.Equals(s.Code)).IdSections.Add(s.IdSection);
            //                else
            //                {
            //                    codes.Add(new Code(s.Code, s.IdSection));
            //                }
            //            }
            //            codeSection.Codes = codes;
            //            codeSections.Add(codeSection);
            //            counter++;
            //        }
            //        int[] selectedSect = new int[40];
            //        isContinue2 = true;
            //        //Обход сформированных секций с уникальными кодами на объект
            //        while (isContinue2)
            //        {
            //            //Общее число квартир на объект
            //            int totalCountFlats = 0;
            //            for (int i = 0; i < codeSections.Count; i++)
            //            {
            //                if (codeSections[i].Codes.Count == selectedSect[i])

            //                {
            //                    isContinue2 = false;
            //                    break;
            //                }
            //                string code = codeSections[i].Codes[selectedSect[i]].CodeStr;
            //                for (int j = 0; j < code.Length; j++)
            //                {
            //                    if (code[j].Equals('0')) continue;
            //                    totalCountFlats += Convert.ToInt16(code[j].ToString()) * (codeSections[i].CountFloors - 1);
            //                }
            //            }
            //            //string newCode = "";
            //            //foreach (var req in spotInfo.requirments)
            //            //{
            //            //    newCode += "0";
            //            //}
            //            //int countF = 0;

            //            //for (int i = 0; i < sections.Count; i++)
            //            //{
            //            //    string sub = sections[i][selectedSect[i]].Code;
            //            //    newCode = SummCode(newCode, sub);
            //            //    //countF += Convert.ToInt16(sub);
            //            //}
            //            //}
            //            selectedSect[sections.Count - 1]++;
            //            if (selectedSect[sections.Count - 1] >= codeSections[codeSections.Count - 1].Codes.Count)
            //                if (!IncrementSection(selectedSect, codeSections.Count - 1, codeSections))
            //                {
            //                    break;
            //                }
            //        }


            //    }
            //}

            FormManager.ViewDataProcentage(dg2, spinfos);
            th.Abort();
            lblCountObjects.Text = ob.Count.ToString();
              this.pb.Image = global::AR_AreaZhuk.Properties.Resources.объект;

        }

        public string SummCode(string oldCode, string newCode)
        {
            string code = "";
            for (int i = 0; i < oldCode.Length; i++)
            {
                code += (Convert.ToInt16(oldCode[i].ToString() + Convert.ToInt16(newCode[i].ToString()))).ToString() +
                        ";";
            }
            return code;
        }

        public bool IncrementSection(int[] selectedSect, int index, List<CodeSection> sections)
        {
            if (index == 0)
            {
                isContinue2 = false;
                return false;
            }
            selectedSect[index] = 0;
            selectedSect[index - 1]++;

            if (selectedSect[index - 1] >= sections[index - 1].Codes.Count)
            {
                IncrementSection(selectedSect, index - 1, sections);
            }
            return true;
        }

        private static void GetDBSections(int startIndex, InsolationSpot insulation, FrameWork fw, List<Section> dbSections, int countFloors, bool isLeftCorner, bool isRightCorner)
        {
            //for (int i = 7; i < 15; i++)
            //{
            Section sec = new Section();
            sec.Sections = new List<FlatInfo>();
            sec.Floors = countFloors;
            sec.CountModules = startIndex * 4;
            sec.IsLeftBottomCorner = isLeftCorner;
            sec.IsRightBottomCorner = isRightCorner;
            sec.Sections = fw.GetAllSectionsFromDB(startIndex * 4, isLeftCorner, isRightCorner, sec.Floors);
            dbSections.Add(sec);
            //}
        }

        private void pb_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            FormPreviewImage v = new FormPreviewImage(pb.Image);
            v.Show();
        }

        private void dg2_SortStringChanged(object sender, EventArgs e)
        {
            isEvent = false;
            BindingSource bs = new BindingSource();
            bs.DataSource = dg2.DataSource;
            bs.Sort = dg2.SortString;
            dg2.DataSource = bs;
            lblCountObjects.Text = dg2.RowCount.ToString();
            isEvent = true;
        }

        private void dg2_FilterStringChanged(object sender, EventArgs e)
        {
            isEvent = false;
            BindingSource bs = new BindingSource();
            bs.DataSource = dg2.DataSource;
            bs.Filter = dg2.FilterString;
            dg2.DataSource = bs;
            lblCountObjects.Text = dg2.RowCount.ToString();
            isEvent = true;
        }

        private void chkDominant_CheckedChanged(object sender, EventArgs e)
        {
            txtOffsetDominants.Enabled = chkDominant.Checked;
            IsRemainingDominants = chkDominant.Checked;
        }

        private void txtOffsetDominants_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    DominantOffSet = Convert.ToInt16(txtOffsetDominants.Text);
            //}
            //catch{}

        }

        private void dg2_SelectionChanged(object sender, EventArgs e)
        {
            if (!isEvent)
                return;
            try
            {
                List<string> guids = (from DataGridViewRow row in dg2.SelectedRows select dg2[dg2.Columns.Count - 1, row.Index].Value.ToString()).ToList();
                foreach (var g in guids)
                {

                    GeneralObject go = ob.First(x => x != null && x.SpotInf.GUID.Equals(g));
                    if (go == null) break;
                    string imagePath = @"\\ab4\CAD_Settings\Revit_server\13. Settings\02_RoomManager\00_PNG_ПИК1\";

                    string ExcelDataPath = @"\\ab4\CAD_Settings\Revit_server\13. Settings\02_RoomManager\БД_Параметрические данные квартир ПИК1 -Не трогать.xlsx";

                    BeetlyVisualisation.ImageCombiner imgComb = new BeetlyVisualisation.ImageCombiner(go, ExcelDataPath, imagePath, 72);
                    var im = imgComb.generateGeneralObject();
                    pb.Image = im;
                    break;

                }
            }
            catch { }


        }

        private void dg2_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void txtOffsetDominants_ValueChanged(object sender, EventArgs e)
        {
            DominantOffSet = Convert.ToInt16(txtOffsetDominants.Value);
        }

        private void btnMenuGroup1_Click(object sender, EventArgs e)
        {

        }

        private void btnMenuGroup1_Click_1(object sender, EventArgs e)
        {
            FormManager.Panel_Show(pnlMenuGroup1, btnMenuGroup1, 25, 315);
        }

        private void btnMenuGroup2_Click(object sender, EventArgs e)
        {
            FormManager.Panel_Show(pnlMenuGroup2, btnMenuGroup2, 25, 260);
        }

        private void btnMenuGroup3_Click(object sender, EventArgs e)
        {
            FormManager.Panel_Show(pnlMenuGroup3, btnMenuGroup3, 25, 273);
        }

        private void pb_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Картинка (*.jpg)|*.jpg";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pb.Image.Save(dialog.FileName, ImageFormat.Jpeg);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkEnableDominant_CheckedChanged(object sender, EventArgs e)
        {
            numDomCountFloor.Enabled = chkEnableDominant.Checked;
        }

        private void GetFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Файл задание инсоляции (*.xlsx)|*.xlsx";
            openFileDialog.RestoreDirectory = true;
            PathToFileInsulation = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PathToFileInsulation = openFileDialog.FileName;
            }
            if (PathToFileInsulation == "")
                btnStartScan.Enabled = false;
            else btnStartScan.Enabled = true;
        }        
    }
}