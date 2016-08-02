using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AR_AreaZhuk
{
    public class Insolation
    {
        InsolationFrameWork insFramework = new InsolationFrameWork();
        public string Name { get; set; }
        public bool IsRightNizSection { get; set; }
        public bool IsRightTopSection { get; set; }
        public bool IsLeftNizSection { get; set; }
        public bool IsLeftTopSection { get; set; }
        public int CountFloorsDominant { get; set; }
        public int CountFloorsMain { get; set; }

        public List<bool> DominantPositions = new List<bool>(); 
        public List<int> MinLeftXY { get; set; }
         public List<int> MaxLeftXY { get; set; }
         public List<int> MinRightXY { get; set; }
         public List<int> MaxRightXY { get; set; }

        public string[,] Matrix { get; set; }

        public List<RoomInsulation> RoomInsulations { get; set; }

        public Insolation()
        {
            RoomInsulations = new List<RoomInsulation>();
            RoomInsulations.Add(new RoomInsulation("Однокомнатная или студия",1,new List<string>(){"1=C","1=D"}));
            RoomInsulations.Add(new RoomInsulation("Двухкомнатная", 2, new List<string>() { "1=C","1=D","2=B|C","2=B" }));
            RoomInsulations.Add(new RoomInsulation("Трехкомнатная", 3, new List<string>() { "1=C", "1=D", "2=B" }));
            RoomInsulations.Add(new RoomInsulation("Четырехкомнатная", 4, new List<string>() { "2=C", "2=D", "1=C&2=B" }));
        }



        public Section GetInsulationSections(List<FlatInfo> sections, bool isRightOrTopLLu, bool isVertical, int indexRowStart,
          int indexColumnStart, Insolation insulation, bool isCorner, int numberSection, SpotInfo sp)
        {
            
            Section s = new Section();
            s.Sections = new List<FlatInfo>();
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
                FlatInfo flats = new FlatInfo();
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

                topFlats = insFramework.GetTopFlatsInSection(sect.Flats, true, false);
                bottomFlats = insFramework.GetTopFlatsInSection(sect.Flats, false, false);

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

                    var lightTop = insFramework.GetLightingPosition(topFlat.LightingTop, topFlat, sect.Flats);
                    var lightNiz = insFramework.GetLightingPosition(topFlat.LightingNiz, topFlat, sect.Flats);
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
                    var lightNiz = insFramework.GetLightingPosition(bottomFlat.LightingNiz, bottomFlat, sect.Flats);
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
               // bool isAdd = true;
                //foreach (var sectionGeneral in s.Sections)
                //{
                //    if (!IsEqualSections(sectionGeneral.Flats, flats.Flats))
                //        continue;
                //    isAdd = false;
                //    break;

                ////}
                //if (isAdd)
                //{
                //    //SpotInfo sp1 = new SpotInfo();
                //    //sp1 = sp1.CopySpotInfo(spotInfo);
                //    //for (int l = 0; l < flats.Flats.Count; l++) //Квартиры
                //    //{
                //    //    if (flats.Flats[l].SubZone.Equals("0")) continue;
                //    //    var reqs =
                //    //        sp1.requirments.Where(
                //    //            x => x.SubZone.Equals(flats.Flats[l].SubZone))
                //    //            .Where(
                //    //                x =>
                //    //                    x.MaxArea + 5 >= flats.Flats[l].AreaTotal &
                //    //                    x.MinArea - 5 <= flats.Flats[l].AreaTotal)
                //    //            .ToList();
                //    //    if (reqs.Count == 0) continue;
                //    //    reqs[0].RealCountFlats++;
                //    //}
                //    //string code = "";
                //    //foreach (var r in sp1.requirments)
                //    //{
                //    //    code += r.RealCountFlats.ToString();
                //    //}
                //    //flats.Code = code;
                //    s.Sections.Add(flats);
                //}
            }
            return s;
        }

    }
            
    public class RoomInsulation
    {
        public string NameType { get; set; }
        public int CountRooms { get; set; }
       public List<string> Rules = new List<string>();

        public RoomInsulation(string name, int countRooms, List<string> rules)
        {
            this.NameType = name;
            this.CountRooms = countRooms;
            this.Rules = rules;
        }
    }
}
