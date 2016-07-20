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
using OfficeOpenXml;


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
        private void MainForm_Load(object sender, EventArgs e)
        {
            btnMenuGroup1.Image = Properties.Resources.up;
            btnMenuGroup2.Image = Properties.Resources.down;
            btnMenuGroup3.Image = Properties.Resources.down;
            pnlMenuGroup2.Height = 25;
            pnlMenuGroup3.Height = 25;
           // Exporter.ExportFlatsToSQL();
            // Exporter.ExportSectionsToSQL(56, "Угловая лево", 18, true, false);
            //  Requirment requirment = new Requirment();
            // this.pb.Image = global::AR_AreaZhuk.Properties.Resources.объект;
            FrameWork fw = new FrameWork();
            //  var roomInfo = fw.GetRoomData("");
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



        public Section GetInsulationSections(List<SectionInformation> sections, bool isRightOrTopLLu, bool isVertical, int indexRowStart,
            int indexColumnStart, Insolation insulation, bool isCorner, int numberSection, SpotInfo sp)
        {
            // List<FlatInfo> listSections = new List<FlatInfo>();
            Section s = new Section();
            s.Sections = new List<SectionInformation>();
            s.IsCorner = isCorner;
            s.IsVertical = isVertical;
            s.NumberInSpot = numberSection;
            s.SpotOwner = insulation.Name;
            s.CountStep = sections[0].CountStep;
            s.CountModules = sections[0].CountStep * 4;
            s.Floors = sections[0].Floors;
            foreach (var sect in sections)
            {

                if (sect.Flats.Count == 0)
                    continue;

                //bool isBreak = false;
                //for (int i = 0; i < 7; i++)
                //{
                //    if (!section.Flats[i].SubZone.Contains('1')) continue;
                //    isBreak = true;
                //    break;
                //}
                //if (isBreak)
                //    continue;

                //  Section s = new Section();

                //s.IsCorner = isCorner;
                //s.IsVertical = isVertical;
                // s.Sections = new List<FlatInfo>();
                SectionInformation flats = new SectionInformation();
                flats.IdSection = sect.IdSection;
                flats.IsInvert = !isRightOrTopLLu;
                flats.SpotOwner = insulation.Name;
                flats.NumberInSpot = numberSection;
                flats.Flats = sect.Flats;
                flats.IsCorner = isCorner;
                flats.IsVertical = isVertical;
                // flats.CountStep = section.CountStep;

                flats.Floors = sect.Floors;
                int direction = 1;
                int indexColumnLLUTop = 0;
                int indexColumnLLUBottom = 0;
                if (isRightOrTopLLu)
                {
                    if (insulation.IsLeftNizSection)
                    {
                        if (isVertical)
                            indexColumnLLUTop = 3;
                        else
                        {
                            indexColumnLLUBottom = 3;
                            indexColumnLLUTop = 0;
                        }
                        direction = -1;
                    }
                    else if (insulation.IsRightNizSection)
                    {
                        if (isVertical)
                        {
                            indexColumnLLUTop = 0;
                            indexColumnLLUBottom = -3;
                            direction = -1;
                        }
                        else
                        {
                            indexColumnLLUBottom = 0;
                            indexColumnLLUTop = -3;
                            direction = -1;
                            if (!isCorner)
                            {
                                indexColumnLLUBottom = 3;
                                indexColumnLLUTop = 0;
                                direction = -1;
                            }
                        }

                    }

                }
                else
                {
                    if (insulation.IsRightNizSection)
                    {
                        direction = -1;
                        indexColumnLLUBottom = 0;
                        indexColumnLLUTop = -3;
                    }
                    else
                    {
                        indexRowStart = insulation.MinLeftXY[1];
                        indexColumnLLUBottom = 3;
                    }

                }
                List<RoomInfo> topFlats = new List<RoomInfo>();
                List<RoomInfo> bottomFlats = new List<RoomInfo>();

                topFlats = GetTopFlatsInSection(sect.Flats, true, false);
                bottomFlats = GetTopFlatsInSection(sect.Flats, false, false);

                int indexRow = indexRowStart;
                int indexColumn = indexColumnStart;
                bool isValid = false;
                for (int i = 0; i < topFlats.Count; i++)
                {
                    if (isCorner & topFlats.Count - 1 == i & !insulation.IsRightNizSection)
                    {
                        indexRow--;
                        indexRow--;
                    }
                    var topFlat = topFlats[i];
                    var rul = insulation.RoomInsulations.Where(x => x.CountRooms.Equals(Convert.ToInt16(topFlat.SubZone))).ToList();
                    if (rul.Count == 0)
                    {
                        if (isVertical)
                            indexRow += topFlat.SelectedIndexTop * direction;
                        else indexColumn += topFlat.SelectedIndexTop * direction;
                        continue;
                    }
                    isValid = false;

                    var lightTop = GetLightingPosition(topFlat.LightingTop, topFlat, sect.Flats);
                    var lightNiz = GetLightingPosition(topFlat.LightingNiz, topFlat, sect.Flats);
                    if (lightTop == null | lightNiz == null)
                    {
                        break;
                    }
                    string ins = "";

                    foreach (var r in rul[0].Rules)
                    {
                        string[] masRule = r.Split('=', '|');
                        int countValidCell = 0;
                        if (insulation.IsRightNizSection & isCorner & i == 0)
                        {
                            indexColumn = insulation.MaxRightXY[0];
                            bool isOr = false;
                            foreach (var ln in lightNiz)
                            {
                                if (ln.Equals(0)) break;
                                //if (isOr)
                                //    continue;
                                isOr = ln < 0;
                                int v = insulation.MaxRightXY[1] - 5 + topFlat.SelectedIndexBottom - Math.Abs(ln) + 1;
                                ins = insulation.Matrix[indexColumn, v];

                                if (string.IsNullOrWhiteSpace(ins)) continue;
                                if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;

                            }
                            isOr = false;
                            foreach (var ln in lightTop)
                            {
                                if (ln.Equals(0)) break;
                                //if (isOr)
                                //    continue;
                                isOr = ln < 0;
                                int v = insulation.MaxRightXY[1] - 5 + Math.Abs(ln);
                                ins = insulation.Matrix[insulation.MaxRightXY[0] - 3, v];
                                if (string.IsNullOrWhiteSpace(ins)) continue;

                                else if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;
                            }
                            indexRow = insulation.MaxRightXY[1];
                            indexColumn = insulation.MaxRightXY[0] - 3;

                        }
                        else if ((indexRow == indexRowStart & isVertical) | (indexColumn == indexColumnStart & !isVertical) | topFlat.SelectedIndexBottom == 0)  //первая справа квартира
                        {
                            bool isOr = false;
                            foreach (var ln in lightNiz)
                            {
                                if (ln.Equals(0)) break;
                                //if (isOr)
                                //    continue;
                                isOr = ln < 0;
                                if (isVertical)
                                    ins = insulation.Matrix[indexColumn + indexColumnLLUBottom, indexRow + Math.Abs(ln) * direction];
                                else
                                    ins = insulation.Matrix[indexColumn + Math.Abs(ln) * (-direction) + direction * topFlat.SelectedIndexBottom + direction, indexRow + indexColumnLLUBottom];
                                if (string.IsNullOrWhiteSpace(ins)) continue;
                                if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;

                            }
                            isOr = false;
                            foreach (var ln in lightTop)
                            {

                                if (ln.Equals(0)) break;
                                //if (isOr)
                                //    continue;
                                isOr = ln < 0;
                                if (isVertical)
                                    ins = insulation.Matrix[indexColumn + indexColumnLLUTop, indexRow + Math.Abs(ln) * direction];
                                else ins = insulation.Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUTop];
                                if (string.IsNullOrWhiteSpace(ins)) continue;

                                else if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;
                            }
                        }
                        else if (indexRow != 0 & topFlat.SelectedIndexBottom > 0)               //первая слева квартира
                        {
                            bool isOr = false;
                            foreach (var ln in lightNiz)
                            {
                                if (ln.Equals(0)) break;
                                //if (isOr)
                                //    continue;
                                isOr = ln < 0;
                                if (isCorner)
                                {
                                    if (insulation.IsLeftNizSection)
                                        ins = insulation.Matrix[insulation.MinLeftXY[0], indexRow - Math.Abs(ln) * direction];
                                    else if (insulation.IsRightNizSection)
                                        // ins = insulation.Matrix[indexColumn - topFlat.SelectedIndexTop - 1 + Math.Abs(ln), indexRow];
                                        ins = insulation.Matrix[indexColumn - topFlat.SelectedIndexTop + Math.Abs(ln), indexRow];
                                }
                                else if (isVertical)
                                    ins = insulation.Matrix[indexColumn + indexColumnLLUBottom, indexRow - Math.Abs(ln) * direction];
                                else ins = insulation.Matrix[indexColumn - Math.Abs(ln) * direction, indexRow +

indexColumnLLUBottom];
                                if (string.IsNullOrWhiteSpace(ins)) continue;

                                if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;
                            }
                            isOr = false;
                            foreach (var ln in lightTop)
                            {
                                if (ln.Equals(0)) break;
                                //isOr = ln < 0;
                                if (isCorner & insulation.IsLeftNizSection)
                                {
                                    ins = insulation.Matrix[insulation.MinLeftXY[0] + 3, indexRow - Math.Abs(ln) * direction];
                                }
                                else if (isCorner & insulation.IsRightNizSection)
                                {
                                    ins = insulation.Matrix[indexColumn - Math.Abs(ln), indexRow - 3];
                                }

                                else if (isVertical)
                                    ins = insulation.Matrix[indexColumn + indexColumnLLUTop, indexRow + Math.Abs(ln) * direction];
                                else ins = insulation.Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUTop];

                                ///////cxdzscs
                                if (string.IsNullOrWhiteSpace(ins)) continue;
                                if (!masRule[1].Equals(ins.Split('|')[1]))
                                    continue;
                                countValidCell++;
                            }
                        }
                        if (Convert.ToInt16(masRule[0]) > countValidCell)
                            continue;
                        isValid = true;
                        if (isVertical)
                            indexRow += topFlat.SelectedIndexTop * direction;
                        else if (i == 0 & isCorner & insulation.IsRightNizSection)
                        { }
                        else indexColumn += topFlat.SelectedIndexTop * direction;

                        //else if (i >=2  & isCorner & insulation.IsRightNizSection)
                        //    indexColumn += topFlat.SelectedIndexTop * indexLLU;
                        // counteR++;
                        break;
                    }
                    if (!isValid) break;
                }
                if (!isValid) continue;
                //  indexRow = 0;
                bool isFirstEnter = true;
                bool isPovorot = false;
                if (insulation.IsRightNizSection & isCorner)
                {
                    direction = 1;
                }
                foreach (var bottomFlat in bottomFlats)
                {
                    var rul =
                        insulation.RoomInsulations.Where(x => x.CountRooms.Equals(Convert.ToInt16(bottomFlat.SubZone)))
                            .ToList();
                    if (rul.Count == 0)
                    {
                        if (isVertical)
                            indexRow += bottomFlat.SelectedIndexBottom * direction;
                        else indexColumn += bottomFlat.SelectedIndexBottom * direction;

                        continue;
                    }
                    if (isCorner & isFirstEnter & !insulation.IsRightNizSection)
                    {
                        indexRow++;
                        indexRow++;
                        isFirstEnter = false;

                    }
                    isValid = false;
                    //  string[] lightNizStr = bottomFlat.LightingNiz.Split(';');
                    var lightNiz = GetLightingPosition(bottomFlat.LightingNiz, bottomFlat, sect.Flats);
                    if (lightNiz == null)
                        break;
                    int tempIndex = indexRow;
                    string ins = "";
                    foreach (var r in rul[0].Rules)
                    {
                        indexRow = tempIndex;
                        string[] masRule = r.Split('=', '|');
                        int countValidCell = 0;
                        bool isOr = false;
                        foreach (var ln in lightNiz)
                        {
                            if (ln.Equals(0)) break;
                            //if (isOr)
                            //    continue;
                            isOr = ln < 0;
                            if (isCorner)
                            {
                                if (isPovorot)
                                {
                                    ins = insulation.Matrix[indexColumn - Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
                                }

                                else if ((insulation.IsLeftNizSection) && (indexRow - Math.Abs(ln) * direction) - insulation.MaxLeftXY[1] >= 1)
                                {
                                    // indexRow = 13;
                                    indexColumn = insulation.MaxLeftXY[0];
                                    if (Math.Abs(ln) >= 3)
                                        indexColumn = insulation.MaxLeftXY[0] - 3;
                                    ins = insulation.Matrix[indexColumn - Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
                                    indexRow = insulation.MaxLeftXY[1];

                                }
                                else if ((insulation.IsRightNizSection) && (indexColumn + Math.Abs(ln) - insulation.MaxRightXY[0] >= 1))
                                {
                                    indexColumn = insulation.MaxRightXY[0];
                                    if (Math.Abs(ln) >= 3)
                                        indexRow = insulation.MaxRightXY[1] + 3;
                                    ins = insulation.Matrix[indexColumn, indexRow + indexColumnLLUBottom - Math.Abs(ln)];
                                    indexRow = insulation.MaxRightXY[1] - 3;
                                }
                                else if (!insulation.IsRightNizSection)
                                {
                                    ins = insulation.Matrix[insulation.MaxLeftXY[0], indexRow - Math.Abs(ln) * direction];
                                }
                                else if (insulation.IsRightNizSection)
                                {
                                    ins = insulation.Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
                                }
                                else
                                {
                                    ins = insulation.Matrix[insulation.MaxRightXY[0], indexRow + Math.Abs(ln)];
                                }

                            }
                            else if (isVertical)
                                ins = insulation.Matrix[indexColumn + indexColumnLLUBottom, indexRow - Math.Abs(ln) * direction - topFlats[topFlats.Count - 1].SelectedIndexBottom * direction];
                            else ins = insulation.Matrix[indexColumn - Math.Abs(ln) * direction - topFlats[topFlats.Count - 1].SelectedIndexBottom * direction, indexRow + indexColumnLLUBottom];
                            if (string.IsNullOrWhiteSpace(ins)) continue;
                            if (!masRule[1].Equals(ins.Split('|')[1]))
                                continue;
                            countValidCell++;
                        }
                        if (Convert.ToInt16(masRule[0]) > countValidCell)
                            continue;
                        isValid = true;
                        if (isCorner & insulation.IsRightNizSection)
                        {
                            indexColumn += bottomFlat.SelectedIndexBottom * direction;
                        }
                        else if (isCorner & indexRow - bottomFlat.SelectedIndexBottom * direction < insulation.MaxLeftXY[1] & !isPovorot)
                        {
                            indexRow -= bottomFlat.SelectedIndexBottom * direction;

                        }
                        else if (isCorner & indexRow - bottomFlat.SelectedIndexBottom * direction >= insulation.MaxLeftXY[1] & !isPovorot)
                        {
                            indexRow = insulation.MaxLeftXY[1] - 3;
                            indexColumn = bottomFlat.SelectedIndexBottom - 3;
                            isPovorot = true;
                            //  indexColumn -= (indexRow - bottomFlat.SelectedIndexBottom * indexLLU - 13) * indexLLU;
                        }
                        else if (isPovorot)
                        {
                            indexColumn -= bottomFlat.SelectedIndexBottom * direction;
                        }
                        else if (isVertical)
                            indexRow -= bottomFlat.SelectedIndexBottom * direction;
                        else indexColumn -= bottomFlat.SelectedIndexBottom * direction;
                        break;
                    }
                    if (!isValid) break;
                }
                if (!isValid) continue;
                bool isAdd = true;
                foreach (var sectionGeneral in s.Sections)
                {
                    if (!IsEqualSections(sectionGeneral.Flats, flats.Flats))
                        continue;
                    isAdd = false;
                    break;

                }
                if (isAdd)
                {
                    SpotInfo sp1 = new SpotInfo();
                    sp1 = sp1.CopySpotInfo(spotInfo);
                    for (int l = 0; l < flats.Flats.Count; l++) //Квартиры
                    {
                        if (flats.Flats[l].SubZone.Equals("0")) continue;
                        var reqs =
                            sp1.requirments.Where(
                                x => x.SubZone.Equals(flats.Flats[l].SubZone))
                                .Where(
                                    x =>
                                        x.MaxArea + 5 >= flats.Flats[l].AreaTotal &
                                        x.MinArea - 5 <= flats.Flats[l].AreaTotal)
                                .ToList();
                        if (reqs.Count == 0) continue;
                        reqs[0].RealCountFlats++;
                    }
                    string code = "";
                    foreach (var r in sp1.requirments)
                    {
                        code += r.RealCountFlats.ToString();
                    }
                    flats.Code = code;
                    s.Sections.Add(flats);
                }
            }
            return s;
        }

         List<RoomInfo> GetTopFlatsInSection(List<RoomInfo> section, bool isTop, bool isRight)
        {
            List<RoomInfo> topFlats = new List<RoomInfo>();
            if (isTop)
            {
                if (!isRight)
                {
                    for (int i = section.Count - 3; i < section.Count; i++)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                }
                else
                {
                    for (int i = 3; i >= 0; i--)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }

                    for (int i = section.Count - 1; i > section.Count - 4; i--)
                    {
                        if (section[i].SelectedIndexTop == 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
            }
            else
            {
                if (!isRight)
                {
                    for (int i = 0; i < section.Count; i++)
                    {
                        if (section[i].SelectedIndexTop != 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
                else
                {


                    for (int i = section.Count - 1; i >= 0; i--)
                    {
                        if (section[i].SelectedIndexTop != 0) continue;
                        topFlats.Add(section[i]);
                    }
                }
            }
            return topFlats;
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



        private int[] GetLightingPosition(string lightStr, RoomInfo room, List<RoomInfo> allRooms)
        {
            int[] light = new int[5];
            string[] masStr = lightStr.Split(';');
            var l = lightStr.Length;
            if (masStr.Length > 1)
            {

                string[] ss = masStr[1].Split('*');
                if (allRooms.IndexOf(room) - 1 < 0)
                    return null;
                var preRoom = allRooms[allRooms.IndexOf(room) - 1];
                if (preRoom.LinkagePOSLE.Contains(ss[0].Trim().Substring(0, 1)) &
                    (room.LinkageDO.Contains(ss[0].Trim().Substring(1, 1))))
                {
                    masStr[0] = ss[1];
                }
            }
            if (masStr[0].Contains('|'))
            {
                if (masStr[0].Contains('-'))
                {
                    string[] ss = masStr[0].Split('-');
                    if (ss[0].Contains('|'))
                    {

                        light[0] = -Convert.ToInt16(ss[0].Split('|')[0]);
                        light[1] = -Convert.ToInt16(ss[0].Split('|')[1]);
                        light[2] = Convert.ToInt16(ss[1]);
                    }
                    else
                    {
                        light[1] = -Convert.ToInt16(ss[1].Split('|')[0]);
                        light[2] = -Convert.ToInt16(ss[1].Split('|')[1]);
                        light[0] = Convert.ToInt16(ss[0]);
                    }
                }
                else
                {
                    light[0] = -Convert.ToInt16(masStr[0].Split('|')[0]);
                    light[1] = -Convert.ToInt16(masStr[0].Split('|')[1]);
                }
            }
            else if (masStr[0].Contains('-'))
            {
                int counter = 0;
                string[] ms = masStr[0].Split('|');
                if (ms[0].Contains('-'))
                {
                    for (int i = Convert.ToInt16(ms[0].Split('-')[0]); i <= Convert.ToInt16(ms[0].Split('-')[1]); i++)
                    {
                        light[counter] = i;
                        counter++;
                    }
                }
                else
                {
                    for (int i = Convert.ToInt16(ms[1].Split('-')[0]); i <= Convert.ToInt16(ms[1].Split('-')[1]); i++)
                    {
                        light[counter] = i;
                        counter++;
                    }
                }
            }
            else if (masStr[0].Contains(','))
            {
                string[] mass = masStr[0].Split(',');
                int counter = 0;
                for (int i = 0; i < mass.Length; i++)
                {
                    light[counter] = Convert.ToInt16(mass[i]);
                    counter++;
                }
            }
            else light[0] = Convert.ToInt16(masStr[0]);
            return light;
        }


        //public bool IncrementSection(List<HouseInfo> listSections, int i)
        //{
        //    if (i == -1)
        //    {
        //        i = 0;
        //    }
        //    var sizeSection = listSections[i].SectionsBySize[listSections[i].LastSizeSelected];
        //    if (i == 0)
        //    {
        //        return false;
        //        //Для поточности
        //        //for (int j = 1; j < listSections.Count; j++)
        //        //{
        //        //    listSections[j].LastSizeSelected = 0;
        //        //}
        //    }
        //    // sizeSection.LastSectionSelected++;
        //    sizeSection.LastSectionSelected++;
        //    //sizeSection.LastSectionSelected +5;
        //    if (sizeSection.LastSectionSelected >= (sizeSection.Sections.Count))    //последняя секция по размеру
        //    {
        //        if (i == 0)
        //        {
        //            sizeSection.LastSectionSelected = 0;
        //            listSections[i].LastSizeSelected++;
        //            if (listSections[i].LastSizeSelected.Equals(listSections[i].SectionsBySize.Count))
        //                return false;

        //        }
        //        sizeSection.LastSectionSelected = 0;
        //        listSections[i].LastSizeSelected++;
        //        if (listSections[i].LastSizeSelected.Equals(listSections[i].SectionsBySize.Count))
        //        {
        //            listSections[i].LastSizeSelected = 0;
        //            if (!IncrementSection(listSections, i - 1))
        //                return false;
        //            //var preSection = listSections[i-1].Sections[listSections[i-1].LastSizeSelected];
        //            //preSection.LastSectionSelected++;

        //        }
        //    }
        //    return true;
        //}

        //public bool IsExist(HouseInfo house)
        //{
        //    bool isEqual = false;
        //    if (houses.Count == 0)
        //        return false;
        //    for (int i = 0; i < houses.Count; i++)
        //    {
        //        isEqual = true;
        //        for (int j = 0; j < houses[0][i].SectionsBySize.Count; j++)
        //        {
        //            for (int k = 0; k < houses[0][i].SectionsBySize[j].Sections.Count; k++)
        //            {
        //                for (int l = 0; l < houses[0][i].SectionsBySize[j].Sections[k].Flats.Count; l++)
        //                {
        //                    if (houses[0][i].SectionsBySize[j].Sections[k].Flats[l].Type.Equals(house.SectionsBySize[j].Sections[k].Flats[l].Type)) continue;
        //                    isEqual = false;
        //                    break;
        //                }

        //            }
        //            if (!isEqual)
        //                break;
        //        }
        //        if (isEqual)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}



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

                // 
                // dg2[dg2.RowCount - 1, 0].Value = infoPercent;
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


        //public void IncrementSection(List<HouseInfo> listSections, int indexSection, int[] indexSelectedSize, int[] indexSelectedSection, bool isSizeSection)
        //{
        //    if (indexSection == 0)
        //    {
        //        isContinue = false;
        //        return;
        //    }
        //    var section = listSections[indexSection];
        //    var sectionSize = section.SectionsBySize[indexSelectedSize[indexSection]];
        //    if (isSizeSection)
        //    {
        //        indexSelectedSize[indexSection]++;

        //        var section1 = listSections[indexSection - 1];
        //        var sectionSize1 = section.SectionsBySize[indexSelectedSize[indexSection - 1]];
        //        if (indexSelectedSize[indexSection] >= section.SectionsBySize.Count)
        //        {
        //            IncrementSection(listSections, indexSection - 1, indexSelectedSize, indexSelectedSection,
        //                isSizeSection);
        //        }
        //    }
        //    else
        //    {
        //        indexSelectedSection[indexSection]++;
        //        if (indexSelectedSection[indexSection] >= sectionSize.Sections.Count)
        //        {
        //            indexSelectedSection[indexSection - 1]++;
        //            indexSelectedSection[indexSection] = 0;
        //            // indexSelectedSize[indexSection]=0;
        //            var section1 = listSections[indexSection - 1];
        //            var sectionSize1 = section1.SectionsBySize[indexSelectedSize[indexSection - 1]];
        //            if (indexSelectedSection[indexSection - 1] >= sectionSize1.Sections.Count)
        //            {
        //                IncrementSection(listSections, indexSection - 1, indexSelectedSize, indexSelectedSection,
        //              true);
        //            }
        //            //IncrementSection(listSections, indexSection - 1, indexSelectedSize, indexSelectedSection,
        //            //  isSizeSection);
        //        }
        //    }
        //}

        //public HouseInfo SelectSectionForHouse(int[] indexSelectedSection, int[] indexSelectedSize, List<HouseInfo> listSections, HouseInfo house, int remainingModules)
        //{
        //    bool isExit = false;
        //    int indexSection = house.SectionsBySize[0].Sections.Count;
        //    var section = listSections[indexSection];
        //    var sectionSize = section.SectionsBySize[indexSelectedSize[indexSection]];
        //    if (sectionSize.Sections.Count == 0)
        //    {
        //        IncrementSection(listSections, indexSection, indexSelectedSize, indexSelectedSection,
        //                   true);
        //        isExit = true;
        //    }

        //    if (!isExit)
        //    {
        //        if (remainingModules - sectionSize.CountModules != 0)
        //        {
        //            house.SectionsBySize[0].Sections.Add(sectionSize.Sections[indexSelectedSection[indexSection]]);
        //            if (remainingModules - sectionSize.CountModules > 32)
        //            {

        //                remainingModules = remainingModules - sectionSize.CountModules;
        //                SelectSectionForHouse(indexSelectedSection, indexSelectedSize, listSections, house,
        //                    remainingModules);
        //            }
        //            else if (remainingModules - sectionSize.CountModules < 32)
        //            {
        //                indexSelectedSize[indexSection]++;
        //                if (indexSelectedSize[indexSection] >= section.SectionsBySize.Count)
        //                {
        //                    IncrementSection(listSections, indexSection - 1, indexSelectedSize, indexSelectedSection,
        //                        true);
        //                }
        //                isExit = true;
        //            }
        //        }
        //        else
        //        {
        //            house.SectionsBySize[0].Sections.Add(sectionSize.Sections[indexSelectedSection[indexSection]]);
        //            IncrementSection(listSections, indexSection, indexSelectedSize, indexSelectedSection, false);
        //        }
        //    }


        //    // house.SectionsBySize[0].Sections.Add(sectionSize.Sections[indexSelectedSection[indexSection]]);
        //    if (isExit)
        //        house.SectionsBySize[0].Sections.Clear();
        //    return house;


        //}

        //public void SelectSectionForHouse(int[] indexSelected, List<HouseInfo> listSections,HouseInfo house)
        //{

        //}

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
        //private void GetHousesBySection(List<HouseInfo> listSections, int countStart, int indexSpot)//List<HouseInfo> listSections
        //{
        //    SpotInfo sp = new SpotInfo();
        //    int[] indexSelectedSize = new int[15];
        //    int[] indexSelectedSection = new int[15];
        //    bool isGo = true;
        //    houses.Add(new List<HouseInfo>());
        //    isContinue = true;
        //    while (isContinue)
        //    {
        //        int countModulesTotal = Convert.ToInt16(sp.SpotArea / 12.96);
        //        for (int i = 0; i < listSections.Count; i++)
        //        {
        //            countModulesTotal = countModulesTotal -
        //                                listSections[i].SectionsBySize[indexSelectedSize[i]].CountModules;
        //            if (countModulesTotal == 0)
        //            {
        //                bool isValid = true;
        //                for (int j = 0; j < listSections.Count; j++)
        //                {
        //                    if (listSections[j].SectionsBySize[indexSelectedSize[j]].Sections.Count > 0)
        //                        continue;
        //                    isValid = false;
        //                    break;
        //                }
        //                if (isValid)
        //                {
        //                    isContinue2 = true;
        //                    while (isContinue2)
        //                    {
        //                        //try
        //                        //{
        //                        //    HouseInfo house = new HouseInfo();
        //                        //    house.SectionsBySize = new List<Section>();
        //                        //    house.SectionsBySize.Add(new Section());
        //                        //    for (int j = 0; j <= i; j++)
        //                        //    {
        //                        //        house.SectionsBySize[0].Sections.Add(
        //                        //            listSections[j].SectionsBySize[indexSelectedSize[j]].Sections[
        //                        //                indexSelectedSection[j]]);
        //                        //    }
        //                        //    GetHousePercentage(ref house);
        //                        //    houses[indexSpot].Add(house);
        //                        //    indexSelectedSection[i]++;
        //                        //    if (indexSelectedSection[i] >=
        //                        //        listSections[i].SectionsBySize[indexSelectedSize[i]].Sections.Count)
        //                        //    {
        //                        //        SetIndexesSection(indexSelectedSection, indexSelectedSize, i, listSections);
        //                        //    }
        //                        //}
        //                        //catch
        //                        //{
        //                        //    break;
        //                        //}

        //                    }
        //                }
        //                //выполняется код
        //                indexSelectedSize[i]++;
        //                if (indexSelectedSize[i] >= listSections[i].SectionsBySize.Count)
        //                {
        //                    //SetIndexesSize(indexSelectedSize, i, listSections);
        //                }
        //                break;
        //            }
        //            else if (countModulesTotal < 32)
        //            {
        //                indexSelectedSize[i]++;
        //                if (indexSelectedSize[i] >= listSections[i].SectionsBySize.Count)
        //                {
        //                    //SetIndexesSize(indexSelectedSize, i, listSections);
        //                }
        //                break;
        //            }
        //        }


        //    }
        //  var opa =  GetCountHouses(houses, dg, false).ToString();
        //while (true)
        //{
        //    HouseInfo house = new HouseInfo();
        //    house.SectionsBySize = new List<Section>();
        //    house.SectionsBySize.Add(new Section());
        //    IsExit = false;
        //    house = SelectSectionForHouse(indexSelectedSection, indexSelectedSize, listSections, house, 108);//house,Convert.ToInt16(sp.SpotArea/12.96));
        //    if (!isContinue) break;
        //    if (house.SectionsBySize[0].Sections.Count == 0)
        //        continue;
        //    if (listSections[0].SectionsBySize.Count <= indexSelectedSize[0])
        //        break;
        //    houses.Add(house);
        //}



        //List<HouseInfo> listSections = (List<HouseInfo>)listSections1;
        //bool isContinue = true;
        //while (isContinue)
        //{
        //    int countTotalModules = 108;//Convert.ToInt16(sp.SpotArea / 12.96);
        //    HouseInfo house = new HouseInfo();
        //    house.SectionsBySize = new List<Section>();
        //    house.SectionsBySize.Add(new Section());
        //    house.SectionsBySize[0].Sections.Add(listSections[0].SectionsBySize[0].Sections[countStart]);
        //    for (int i = 0; i < listSections.Count; i++)
        //    {
        //        var sizeSection = listSections[i].SectionsBySize[listSections[i].LastSizeSelected];
        //        while (listSections[i].SectionsBySize[listSections[i].LastSizeSelected].Sections.Count == 0)
        //        {
        //            listSections[i].LastSizeSelected++;
        //            if (listSections[i].LastSizeSelected >= (listSections[i].SectionsBySize.Count))
        //            {
        //                listSections[i].LastSizeSelected = 0;
        //                sizeSection = listSections[i].SectionsBySize[listSections[i].LastSizeSelected];
        //                sizeSection.LastSectionSelected = 0;
        //                if (!IncrementSection(listSections, i - 1))
        //                {
        //                    isContinue = false;
        //                    break;
        //                }

        //                //MessageBox.Show("Нет подходящей секции для инсоляции");
        //                //isContinue = false;
        //                //break;
        //            }
        //            else
        //            {
        //                sizeSection = listSections[i].SectionsBySize[listSections[i].LastSizeSelected];
        //            }
        //        }
        //        if (!isContinue)
        //        {
        //            break;
        //        }
        //        countTotalModules -= sizeSection.CountModules;
        //        if (sizeSection.Sections.Count == 0)
        //            continue;
        //        house.SectionsBySize[0].Sections.Add(sizeSection.Sections[sizeSection.LastSectionSelected]);

        //        if (countTotalModules == 0)
        //        {
        //            if (!IncrementSection(listSections, i))
        //            {
        //                isContinue = false;
        //                break;
        //            }
        //            if (!IsExist(houses, house))
        //                houses.Add(house);
        //            break;
        //        }
        //        else if (countTotalModules < 32)
        //        {
        //            listSections[i].LastSizeSelected++;
        //            if (listSections[i].LastSizeSelected == listSections[i].SectionsBySize.Count)
        //            {
        //                listSections[i].LastSizeSelected = 0;
        //            }
        //            //if (!IncrementSection(listSections, i))
        //            //{
        //            //    isContinue = false;
        //            //    break;
        //            //}
        //        }
        //        else if (i == listSections.Count - 1)
        //        {
        //            if (!IncrementSection(listSections, i))
        //            {
        //                isContinue = false;
        //                break;
        //            }
        //        }
        //        if (!isContinue)
        //        {
        //            break;
        //        }
        //        if (listSections[0].LastSizeSelected.Equals(listSections[0].SectionsBySize.Count))
        //        {
        //            var sizeSection11 = listSections[0].SectionsBySize[listSections[0].LastSizeSelected];
        //            if (sizeSection11.LastSectionSelected.Equals(sizeSection11.Sections.Count))
        //            {
        //                isContinue = false;
        //            }
        //        }
        //        //if (countTotalModules < 0)
        //        //{


        //        //    sec.LastSectionSelected++;
        //        //    if (sec.LastSectionSelected == sec.SectionsInSection.Count)
        //        //    {
        //        //        sec.LastSectionSelected = 0;
        //        //        listSections[i].LastSizeSelected++;
        //        //        //if (listSections[i].LastSizeSelected == listSections[i].Sections.Count)
        //        //        //{
        //        //        //    listSections[i-1].la
        //        //        //}
        //        //    }
        //        //    break;
        //        //}
        //        //if (i == listSections.Count - 1 & countTotalModules > 0)
        //        //{
        //        //    break;
        //        //}
        //        //house.Add(sec.SectionsInSection[sec.LastSectionSelected]);
        //        //sec.LastSectionSelected++;
        //        ////sec.SectionsInSection[sec.LastSectionSelected];
        //    }
        //    //if (houses.Count == 50000)
        //    //    break;
        //}
        // }


        public void GetHousePercentage(ref HouseInfo houseInfo, SpotInfo sp1, Insolation insulation)
        {
            sp1 = sp1.CopySpotInfo(spotInfo);
            for (int k = 0; k < houseInfo.Sections.Count; k++) //Квартиры
            {
                SectionInformation section = houseInfo.Sections[k];
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

        //private void dg2_FilterStringChanged(object sender, EventArgs e)
        //{
        //    BindingSource bs = new BindingSource();
        //    bs.DataSource = dg2.DataSource;
        //    bs.Filter = dg2.FilterString;
        //    dg2.DataSource = bs;
        //}

        //private void dg2_SortStringChanged(object sender, EventArgs e)
        //{
        //    BindingSource bs = new BindingSource();
        //    bs.DataSource = dg2.DataSource;
        //    bs.Sort = dg2.SortString;
        //    dg2.DataSource = bs;
        //}

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


            }



            List<List<HouseInfo>> totalObject = new List<List<HouseInfo>>();
            List<List<HouseInfo>> houses = new List<List<HouseInfo>>();
            List<HouseInfo> listSections = new List<HouseInfo>();
            List<List<SectionInformation>> sectionsInHouse = new List<List<SectionInformation>>();
            bool isContinue1 = true;
            bool isContinue2 = true;
            int[] masSizes = new int[] { 28, 32, 36, 40, 44, 48, 52, 56 };
            int counterr = 0;
      
            foreach (var insulation in insulations)
            {

                List<HouseInfo> variantHouses = new List<HouseInfo>();
                MainForm.isContinue = true;
                List<HouseInfo> housesTemp = new List<HouseInfo>();
                SpotInfo sp = new SpotInfo();
                int[] indexSelectedSize = new int[15];
                int[] indexSelectedSection = new int[15];
                bool isGo = true;

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
                            variantHouse.Sections = new List<SectionInformation>();
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
                                    s.Sections = new List<SectionInformation>();
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
                                    List<SectionInformation> sections = new List<SectionInformation>();
                                    sections = sectionGood.Sections;
                                    if (sections.Count > 0)
                                    {
                                        var listSections1 = insulation.GetInsulationSections(sections, isRightOrTopLLu, isVertical, indexRowStart,   ////////////////////////////////////////Инсоляция
                                            indexColumnStart, insulation, isCorner, m + 1, spotInfo);
                                        s1 = listSections1;
                                        if (!isCorner)
                                        {
                                            var listSections2 = insulation.GetInsulationSections(sections, false, isVertical, indexRowStart,         ////////////////////////////////////////Инсоляция
                                                indexColumnStart, insulation, isCorner, m + 1, spotInfo);
                                            foreach (var l in listSections2.Sections)
                                                s1.Sections.Add(l);

                                        }
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
                                    variantHouses.Add(s);

                                }
                                if (!isEmptySection)
                                {
                                    hhh.SectionsBySize = secInHouse;
                                    variantHouses.Add(hhh);
                                }
                                //  string   path = @"E:\444.xlsx";
                                //using (var xlPackage = new ExcelPackage(new FileInfo(path.ToString())))
                                //{
                                //    int countSect = 0;
                                //    foreach (var house in sectionInfos)
                                //    {
                                //        countSect++;
                                //        int countRow = 2;
                                //        foreach (var section in house.Sections)
                                //        {
                                //            SpotInfo sp1 = new SpotInfo();
                                //            sp1 = sp1.CopySpotInfo(spotInfo);

                                //            for (int l = 0; l < section.Flats.Count; l++) //Квартиры
                                //            {
                                //                if (section.Flats[l].SubZone.Equals("0")) continue;
                                //                var reqs =
                                //                    sp1.requirments.Where(
                                //                        x => x.SubZone.Equals(section.Flats[l].SubZone))
                                //                        .Where(
                                //                            x =>
                                //                                x.MaxArea + 5 >= section.Flats[l].AreaTotal &
                                //                                x.MinArea - 5 <= section.Flats[l].AreaTotal)
                                //                        .ToList();
                                //                if (reqs.Count == 0) continue;
                                //                reqs[0].RealCountFlats++;
                                //            }
                                //            int countColumn = 1;
                                //            xlPackage.Workbook.Worksheets[countSect].Cells[countRow, countColumn].Value
                                //                = section.IdSection;
                                //            foreach (var req in sp1.requirments)
                                //            {
                                //                countColumn++;
                                //                xlPackage.Workbook.Worksheets[countSect].Cells[countRow, countColumn].Value
                                //                = req.RealCountFlats;
                                //            }
                                //            countRow++;
                                //        }
                                //    }
                                //    xlPackage.Save();
                                //}




                                indexSelectedSection = new int[15];
                                while (isContinue2)
                                {
                                    //if (sectionInfos.Count > 4 & counterr == 2)
                                    //    break;
                                    HouseInfo hi = new HouseInfo();
                                    hi.Sections = new List<SectionInformation>();
                                    try
                                    {
                                        List<SectionInformation> secs = new List<SectionInformation>();
                                        int[] ids = new int[i + 1];
                                        for (int j = 0; j <= i; j++)
                                        {
                                            if (sectionInfos[j].Sections.Count == 0 || indexSelectedSection[j] >= sectionInfos[j].Sections.Count)
                                            {
                                                isContinue2 = false;
                                                break;
                                            }
                                          //  if ()
                                                hi.Sections.Add(sectionInfos[j].Sections[indexSelectedSection[j]]);
                                          //  else continue;
                                           // if (i != hi.Sections.Count)
                                               // break;
                                            ids[j] = hi.Sections[j].IdSection;
                                        }
                                      
                                        if (!isContinue2)
                                            break;
                                        if (ids.GroupBy(x => x).ToList().Count == hi.Sections.Count)         //Отсев секций без повторений
                                        {
                                            indexSelectedSection[i]++;
                                            if (indexSelectedSection[i] >= sectionInfos[i].Sections.Count)
                                            {
                                                if (!SetIndexesSection(indexSelectedSection, indexSelectedSize, i, sectionInfos))
                                                    isContinue2 = false;
                                            }
                                            continue;
                                        }
                                        GetHousePercentage(ref hi, spotInfo, insulation);
                                        housesTemp.Add(hi);
                                        indexSelectedSection[i]++;
                                        if (indexSelectedSection[i] >= sectionInfos[i].Sections.Count)
                                        {
                                            if (!SetIndexesSection(indexSelectedSection, indexSelectedSize, i, sectionInfos))
                                                isContinue2 = false;
                                        }
                                       
                                    }
                                    catch
                                    {
                                        break;
                                    }

                                }
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
            GetAllSectionPercentage(totalObject, requirment);
            FormManager.ViewDataProcentage(dg2, spinfos);
            th.Abort();
            lblCountObjects.Text = ob.Count.ToString();
            //  this.pb.Image = global::AR_AreaZhuk.Properties.Resources.объект;

        }

        private static void GetDBSections(int startIndex, Insolation insulation, FrameWork fw, List<Section> dbSections, int countFloors, bool isLeftCorner, bool isRightCorner)
        {
            //for (int i = 7; i < 15; i++)
            //{
            Section sec = new Section();
            sec.Sections = new List<SectionInformation>();
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
