using AR_Zhuk_DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AR_AreaZhuk.Insolation
{
    /// <summary>
    /// Инсоляция одного пятна
    /// </summary>
    public class InsolationSpot
    {
        InsolationFrameWork insFramework = new InsolationFrameWork();
        public string Name { get; set; }
        /// <summary>
        /// Угловая. Правый угол. Низ.
        /// </summary>
        public bool IsRightNizSection { get; set; }
        /// <summary>
        /// Угловая. Правый угол. Верх.
        /// </summary>
        public bool IsRightTopSection { get; set; }
        /// <summary>
        /// Угловая. Левый угол. Низ.
        /// </summary>
        public bool IsLeftNizSection { get; set; }
        /// <summary>
        /// Угловая. Левый угол. Верх.
        /// </summary>
        public bool IsLeftTopSection { get; set; }
        public int CountFloorsDominant { get; set; }
        public int CountFloorsMain { get; set; }

        public List<bool> DominantPositions = new List<bool>();
        public List<int> MinLeftXY { get; set; }
        public List<int> MaxLeftXY { get; set; }
        public List<int> MinRightXY { get; set; }
        public List<int> MaxRightXY { get; set; }
        public string[,] Matrix { get; set; }
        public List<RoomInsulation> RoomInsulations { get; private set; }

        public InsolationSpot ()
        {
            // Список выражений требований (должно удовлетворяться любое из них):
            // Синтаксис выражения одного требования:
            // [Кол.помещений][Индеск инсоляции]+[Кол.помещений][Индеск инсоляции]+...
            // [Кол.помещений] - кол инсолируемых комнат. Пусто или любое число. Длина - один символ. Если пусто, то это 1 помещение.
            // [Индеск инсоляции] - один символ A, B, C, D - латинская.
            // + не обязательно. Например: C+2B - требуется 3 помещения с 1C и 2B.     
            // Учитывается порядок индексов A<B<C<D. Поэтому нужно писать только минимальное требования. Например, если требуется C, то писать требование D не обязательно.                   
            RoomInsulations = new List<RoomInsulation>()
            {
                new RoomInsulation ("Однокомнатная или студия", 1, new List<string>() { "C" }),
                new RoomInsulation ("Двухкомнатная", 2, new List<string>() { "C", "2B" }),
                new RoomInsulation ("Трехкомнатная", 3, new List<string>() { "C", "2B" }),
                new RoomInsulation ("Четырехкомнатная", 4, new List<string>() { "2C", "C+2B" })
            };
        }
        /// <summary>
        /// Проверка инсоляции секции (всех вариантов секции)
        /// 
        /// </summary>        
        public Section GetInsulationSections (List<FlatInfo> sections, bool isRightOrTopLLu, bool isVertical,
            int indexRowStart, int indexColumnStart, bool isCorner, int numberSection, SpotInfo sp)
        {
            Section s = new Section();
            s.Sections = new List<FlatInfo>();
            s.IsCorner = isCorner;
            s.IsVertical = isVertical;
            s.NumberInSpot = numberSection;
            s.SpotOwner = Name;
            s.CountStep = sections[0].CountStep;
            s.CountModules = sections[0].CountStep * 4;
            s.Floors = sections[0].Floors;

            IInsCheck insCheck = InsCheckFactory.CreateInsCheck(this, s, isCorner, isVertical, indexRowStart, indexColumnStart);

            foreach (var sect in sections)
            {
                if (sect.Flats.Count == 0)
                    continue;
                FlatInfo flats = new FlatInfo();
                flats.IdSection = sect.IdSection;
                flats.IsInvert = !isRightOrTopLLu;
                flats.SpotOwner = Name;
                flats.NumberInSpot = numberSection;
                flats.Flats = sect.Flats;
                flats.IsCorner = isCorner;
                flats.IsVertical = isVertical;
                // flats.CountStep = section.CountStep;
                flats.Floors = sect.Floors;

                if (insCheck.CheckSection(sect, isRightOrTopLLu: true))
                {
                    s.Sections.Add(flats);
                }
                if (insCheck.CheckSection(sect, isRightOrTopLLu: false))
                {
                    s.Sections.Add(flats);
                }
                continue;
            }
            return s;
        }

        ///// <summary>
        ///// Проверка инсоляции секции (всех вариантов секции)
        ///// 
        ///// </summary>        
        //public Section GetInsulationSections (List<FlatInfo> sections, bool isRightOrTopLLu, bool isVertical,
        //    int indexRowStart, int indexColumnStart, bool isCorner, int numberSection, SpotInfo sp)
        //{
        //    Section s = new Section();
        //    s.Sections = new List<FlatInfo>();
        //    s.IsCorner = isCorner;
        //    s.IsVertical = isVertical;
        //    s.NumberInSpot = numberSection;
        //    s.SpotOwner = Name;
        //    s.CountStep = sections[0].CountStep;
        //    s.CountModules = sections[0].CountStep * 4;
        //    s.Floors = sections[0].Floors;

        //    IInsCheck insCheck = InsCheckFactory.CreateInsCheck(this, s, isCorner, isVertical, indexRowStart, indexColumnStart);            

        //    foreach (var sect in sections)
        //    {
        //        if (sect.Flats.Count == 0)
        //            continue;
        //        FlatInfo flats = new FlatInfo();
        //        flats.IdSection = sect.IdSection;
        //        flats.IsInvert = !isRightOrTopLLu;
        //        flats.SpotOwner = Name;
        //        flats.NumberInSpot = numberSection;
        //        flats.Flats = sect.Flats;
        //        flats.IsCorner = isCorner;
        //        flats.IsVertical = isVertical;
        //        // flats.CountStep = section.CountStep;
        //        flats.Floors = sect.Floors;



        //        if (insCheck.CheckSection(sect, isRightOrTopLLu: true))
        //        {
        //            s.Sections.Add(flats);
        //        }
        //        if (insCheck.CheckSection(sect, isRightOrTopLLu: false))
        //        {
        //            s.Sections.Add(flats);
        //        }
        //        continue;

        //        //s.Sections.Add(flats);
        //        //return s;




        //        int direction = 1;
        //        int indexColumnLLUTop = 0;
        //        int indexColumnLLUBottom = 0;

        //        var topFlats = insFramework.GetTopFlatsInSection(sect.Flats, true, false);
        //        var bottomFlats = insFramework.GetTopFlatsInSection(sect.Flats, false, false);


        //        if (isRightOrTopLLu)
        //        {
        //            if (IsLeftNizSection)
        //            {
        //                if (isVertical)
        //                    indexColumnLLUTop = 3;
        //                else
        //                {
        //                    indexColumnLLUBottom = 3;
        //                    indexColumnLLUTop = 0;
        //                }
        //                direction = -1;
        //            }
        //            else if (IsRightNizSection)
        //            {
        //                if (isVertical)
        //                {
        //                    indexColumnLLUTop = 0;
        //                    indexColumnLLUBottom = -3;
        //                    direction = -1;
        //                }
        //                else
        //                {
        //                    indexColumnLLUBottom = 0;
        //                    indexColumnLLUTop = -3;
        //                    direction = -1;
        //                    if (!isCorner)
        //                    {
        //                        indexColumnLLUBottom = 3;
        //                        indexColumnLLUTop = 0;
        //                        direction = -1;
        //                    }
        //                }
        //            }
        //        }
        //        else // isRightOrTopLLu == false
        //        {
        //            if (IsRightNizSection)
        //            {
        //                direction = -1;
        //                indexColumnLLUBottom = 0;
        //                indexColumnLLUTop = -3;
        //            }
        //            else
        //            {
        //                indexRowStart = MinLeftXY[1];
        //                indexColumnLLUBottom = 3;
        //            }
        //        }

        //        int indexRow = indexRowStart;
        //        int indexColumn = indexColumnStart;

        //        bool isValid = false;

        //        for (int i = 0; i < topFlats.Count; i++)
        //        {
        //            if (isCorner & topFlats.Count - 1 == i & !IsRightNizSection)
        //            {
        //                indexRow--;
        //                indexRow--;
        //            }
        //            var topFlat = topFlats[i];
        //            var rul = RoomInsulations.Where(x => x.CountRooms.Equals(Convert.ToInt16(topFlat.SubZone))).ToList();
        //            if (rul.Count == 0)
        //            {
        //                if (isVertical)
        //                    indexRow += topFlat.SelectedIndexTop * direction;
        //                else indexColumn += topFlat.SelectedIndexTop * direction;
        //                continue;
        //            }
        //            isValid = false;

        //            var lightTop = insFramework.GetLightingPosition(topFlat.LightingTop, topFlat, sect.Flats);
        //            var lightNiz = insFramework.GetLightingPosition(topFlat.LightingNiz, topFlat, sect.Flats);
        //            if (lightTop == null | lightNiz == null)
        //            {
        //                break;
        //            }
        //            string ins = "";

        //            foreach (var r in rul[0].Rules)
        //            {
        //                string[] masRule = r.Split('=', '|');
        //                int countValidCell = 0;
        //                if (IsRightNizSection & isCorner & i == 0)
        //                {
        //                    indexColumn = MaxRightXY[0];
        //                    bool isOr = false;
        //                    foreach (var ln in lightNiz)
        //                    {
        //                        if (ln.Equals(0)) break;
        //                        //if (isOr)
        //                        //    continue;
        //                        isOr = ln < 0;
        //                        int v = MaxRightXY[1] - 5 + topFlat.SelectedIndexBottom - Math.Abs(ln) + 1;
        //                        ins = Matrix[indexColumn, v];

        //                        if (string.IsNullOrWhiteSpace(ins)) continue;
        //                        if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;
        //                    }
        //                    isOr = false;
        //                    foreach (var ln in lightTop)
        //                    {
        //                        if (ln.Equals(0)) break;
        //                        //if (isOr)
        //                        //    continue;
        //                        isOr = ln < 0;
        //                        int v = MaxRightXY[1] - 5 + Math.Abs(ln);
        //                        ins = Matrix[MaxRightXY[0] - 3, v];
        //                        if (string.IsNullOrWhiteSpace(ins)) continue;

        //                        else if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;
        //                    }
        //                    indexRow = MaxRightXY[1];
        //                    indexColumn = MaxRightXY[0] - 3;

        //                }
        //                else if ((indexRow == indexRowStart & isVertical) | (indexColumn == indexColumnStart & !isVertical) 
        //                    | topFlat.SelectedIndexBottom == 0)  //первая справа квартира
        //                {
        //                    bool isOr = false;
        //                    foreach (var ln in lightNiz)
        //                    {
        //                        if (ln.Equals(0)) break;
        //                        //if (isOr)
        //                        //    continue;
        //                        isOr = ln < 0;
        //                        if (isVertical)
        //                            ins = Matrix[indexColumn + indexColumnLLUBottom, indexRow + Math.Abs(ln) * direction];
        //                        else
        //                            ins = Matrix[indexColumn + Math.Abs(ln) * (-direction) + direction * topFlat.SelectedIndexBottom + direction, indexRow + indexColumnLLUBottom];
        //                        if (string.IsNullOrWhiteSpace(ins)) continue;
        //                        if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;

        //                    }
        //                    isOr = false;
        //                    foreach (var ln in lightTop)
        //                    {

        //                        if (ln.Equals(0)) break;
        //                        //if (isOr)
        //                        //    continue;
        //                        isOr = ln < 0;
        //                        if (isVertical)
        //                            ins = Matrix[indexColumn + indexColumnLLUTop, indexRow + Math.Abs(ln) * direction];
        //                        else ins = Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUTop];
        //                        if (string.IsNullOrWhiteSpace(ins)) continue;

        //                        else if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;
        //                    }
        //                }
        //                else if (indexRow != 0 & topFlat.SelectedIndexBottom > 0)               //первая слева квартира
        //                {
        //                    bool isOr = false;
        //                    foreach (var ln in lightNiz)
        //                    {
        //                        if (ln.Equals(0)) break;
        //                        //if (isOr)
        //                        //    continue;
        //                        isOr = ln < 0;
        //                        if (isCorner)
        //                        {
        //                            if (IsLeftNizSection)
        //                                ins = Matrix[MinLeftXY[0], indexRow - Math.Abs(ln) * direction];
        //                            else if (IsRightNizSection)
        //                                // ins = insulation.Matrix[indexColumn - topFlat.SelectedIndexTop - 1 + Math.Abs(ln), indexRow];
        //                                ins = Matrix[indexColumn - topFlat.SelectedIndexTop + Math.Abs(ln), indexRow];
        //                        }
        //                        else if (isVertical)
        //                            ins = Matrix[indexColumn + indexColumnLLUBottom, indexRow - Math.Abs(ln) * direction];
        //                        else ins = Matrix[indexColumn - Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
        //                        if (string.IsNullOrWhiteSpace(ins)) continue;

        //                        if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;
        //                    }
        //                    isOr = false;
        //                    foreach (var ln in lightTop)
        //                    {
        //                        if (ln.Equals(0)) break;
        //                        //isOr = ln < 0;
        //                        if (isCorner & IsLeftNizSection)
        //                        {
        //                            ins = Matrix[MinLeftXY[0] + 3, indexRow - Math.Abs(ln) * direction];
        //                        }
        //                        else if (isCorner & IsRightNizSection)
        //                        {
        //                            ins = Matrix[indexColumn - Math.Abs(ln), indexRow - 3];
        //                        }

        //                        else if (isVertical)
        //                            ins = Matrix[indexColumn + indexColumnLLUTop, indexRow + Math.Abs(ln) * direction];
        //                        else ins = Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUTop];

        //                        ///////cxdzscs
        //                        if (string.IsNullOrWhiteSpace(ins)) continue;
        //                        if (!masRule[1].Equals(ins.Split('|')[1]))
        //                            continue;
        //                        countValidCell++;
        //                    }
        //                }
        //                if (Convert.ToInt16(masRule[0]) > countValidCell)
        //                    continue;
        //                isValid = true;
        //                if (isVertical)
        //                    indexRow += topFlat.SelectedIndexTop * direction;
        //                else if (i == 0 & isCorner & IsRightNizSection)
        //                { }
        //                else indexColumn += topFlat.SelectedIndexTop * direction;

        //                //else if (i >=2  & isCorner & insulation.IsRightNizSection)
        //                //    indexColumn += topFlat.SelectedIndexTop * indexLLU;
        //                // counteR++;
        //                break;
        //            }
        //            if (!isValid) break;

        //        } // for (int i = 0; i < topFlats.Count; i++)

        //        if (!isValid) continue;
        //        //  indexRow = 0;
        //        bool isFirstEnter = true;
        //        bool isPovorot = false;
        //        if (IsRightNizSection & isCorner)
        //        {
        //            direction = 1;
        //        }

        //        foreach (var bottomFlat in bottomFlats)
        //        {
        //            var rul = RoomInsulations.Where(x => x.CountRooms.Equals(Convert.ToInt16(bottomFlat.SubZone))).ToList();
        //            if (rul.Count == 0)
        //            {
        //                if (isVertical)
        //                    indexRow += bottomFlat.SelectedIndexBottom * direction;
        //                else indexColumn += bottomFlat.SelectedIndexBottom * direction;

        //                continue;
        //            }
        //            if (isCorner & isFirstEnter & !IsRightNizSection)
        //            {
        //                indexRow++;
        //                indexRow++;
        //                isFirstEnter = false;

        //            }
        //            isValid = false;
        //            //  string[] lightNizStr = bottomFlat.LightingNiz.Split(';');
        //            var lightNiz = insFramework.GetLightingPosition(bottomFlat.LightingNiz, bottomFlat, sect.Flats);
        //            if (lightNiz == null)
        //                break;
        //            int tempIndex = indexRow;
        //            string ins = "";
        //            foreach (var r in rul[0].Rules)
        //            {
        //                indexRow = tempIndex;
        //                string[] masRule = r.Split('=', '|');
        //                int countValidCell = 0;
        //                bool isOr = false;
        //                foreach (var ln in lightNiz)
        //                {
        //                    if (ln.Equals(0)) break;
        //                    //if (isOr)
        //                    //    continue;
        //                    isOr = ln < 0;
        //                    if (isCorner)
        //                    {
        //                        if (isPovorot)
        //                        {
        //                            ins = Matrix[indexColumn - Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
        //                        }

        //                        else if ((IsLeftNizSection) && (indexRow - Math.Abs(ln) * direction) - MaxLeftXY[1] >= 1)
        //                        {
        //                            // indexRow = 13;
        //                            indexColumn = MaxLeftXY[0];
        //                            if (Math.Abs(ln) >= 3)
        //                                indexColumn = MaxLeftXY[0] - 3;
        //                            ins = Matrix[indexColumn - Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
        //                            indexRow = MaxLeftXY[1];

        //                        }
        //                        else if ((IsRightNizSection) && (indexColumn + Math.Abs(ln) - MaxRightXY[0] >= 1))
        //                        {
        //                            indexColumn = MaxRightXY[0];
        //                            if (Math.Abs(ln) >= 3)
        //                                indexRow = MaxRightXY[1] + 3;
        //                            ins = Matrix[indexColumn, indexRow + indexColumnLLUBottom - Math.Abs(ln)];
        //                            indexRow = MaxRightXY[1] - 3;
        //                        }
        //                        else if (!IsRightNizSection)
        //                        {
        //                            ins = Matrix[MaxLeftXY[0], indexRow - Math.Abs(ln) * direction];
        //                        }
        //                        else if (IsRightNizSection)
        //                        {
        //                            ins = Matrix[indexColumn + Math.Abs(ln) * direction, indexRow + indexColumnLLUBottom];
        //                        }
        //                        else
        //                        {
        //                            ins = Matrix[MaxRightXY[0], indexRow + Math.Abs(ln)];
        //                        }

        //                    }
        //                    else if (isVertical)
        //                        ins = Matrix[indexColumn + indexColumnLLUBottom, indexRow - Math.Abs(ln) * direction - topFlats[topFlats.Count - 1].SelectedIndexBottom * direction];
        //                    else ins = Matrix[indexColumn - Math.Abs(ln) * direction - topFlats[topFlats.Count - 1].SelectedIndexBottom * direction, indexRow + indexColumnLLUBottom];
        //                    if (string.IsNullOrWhiteSpace(ins)) continue;
        //                    if (!masRule[1].Equals(ins.Split('|')[1]))
        //                        continue;
        //                    countValidCell++;
        //                }
        //                if (Convert.ToInt16(masRule[0]) > countValidCell)
        //                    continue;
        //                isValid = true;
        //                if (isCorner & IsRightNizSection)
        //                {
        //                    indexColumn += bottomFlat.SelectedIndexBottom * direction;
        //                }
        //                else if (isCorner & indexRow - bottomFlat.SelectedIndexBottom * direction < MaxLeftXY[1] & !isPovorot)
        //                {
        //                    indexRow -= bottomFlat.SelectedIndexBottom * direction;

        //                }
        //                else if (isCorner & indexRow - bottomFlat.SelectedIndexBottom * direction >= MaxLeftXY[1] & !isPovorot)
        //                {
        //                    indexRow = MaxLeftXY[1] - 3;
        //                    indexColumn = bottomFlat.SelectedIndexBottom - 3;
        //                    isPovorot = true;
        //                    //  indexColumn -= (indexRow - bottomFlat.SelectedIndexBottom * indexLLU - 13) * indexLLU;
        //                }
        //                else if (isPovorot)
        //                {
        //                    indexColumn -= bottomFlat.SelectedIndexBottom * direction;
        //                }
        //                else if (isVertical)
        //                    indexRow -= bottomFlat.SelectedIndexBottom * direction;
        //                else indexColumn -= bottomFlat.SelectedIndexBottom * direction;
        //                break;
        //            }
        //            if (!isValid) break;

        //        } //foreach (var bottomFlat in bottomFlats)

        //        if (!isValid) continue;
        //        // bool isAdd = true;
        //        //foreach (var sectionGeneral in s.Sections)
        //        //{
        //        //    if (!IsEqualSections(sectionGeneral.Flats, flats.Flats))
        //        //        continue;
        //        //    isAdd = false;
        //        //    break;

        //        ////}
        //        //if (isAdd)
        //        //{
        //        //    //SpotInfo sp1 = new SpotInfo();
        //        //    //sp1 = sp1.CopySpotInfo(spotInfo);
        //        //    //for (int l = 0; l < flats.Flats.Count; l++) //Квартиры
        //        //    //{
        //        //    //    if (flats.Flats[l].SubZone.Equals("0")) continue;
        //        //    //    var reqs =
        //        //    //        sp1.requirments.Where(
        //        //    //            x => x.SubZone.Equals(flats.Flats[l].SubZone))
        //        //    //            .Where(
        //        //    //                x =>
        //        //    //                    x.MaxArea + 5 >= flats.Flats[l].AreaTotal &
        //        //    //                    x.MinArea - 5 <= flats.Flats[l].AreaTotal)
        //        //    //            .ToList();
        //        //    //    if (reqs.Count == 0) continue;
        //        //    //    reqs[0].RealCountFlats++;
        //        //    //}
        //        //    //string code = "";
        //        //    //foreach (var r in sp1.requirments)
        //        //    //{
        //        //    //    code += r.RealCountFlats.ToString();
        //        //    //}
        //        //    //flats.Code = code;
        //        //    s.Sections.Add(flats);
        //        //}
        //    }
        //    return s;
        //}

        public RoomInsulation FindRule (RoomInfo flat)
        {
            var rule = RoomInsulations.Where(x => x.CountRooms == Convert.ToInt32(flat.SubZone)).FirstOrDefault();
            return rule;
        }
    }

    /// <summary>
    /// Правила инсоляции для квартиры (общие правила по типам квартир - 1,2,3,4 комнатной)
    /// </summary>
    public class RoomInsulation
    {
        /// <summary>
        /// Допустимые индексы инсоляции
        /// </summary>
        public static List<string> AllowedIndexes { get; } = new List<string> { "A", "B", "C", "D" };

        /// <summary>
        /// Название типа квартиры
        /// </summary>
        public string NameType { get; private set; }
        /// <summary>
        /// Количество комнат 1,2,3,4
        /// </summary>
        public int CountRooms { get; private set; }   
        /// <summary>
        /// правила инсоляции (нужно чтобы удовлетворялось одно из них)
        /// </summary>
        public List<InsRule> Rules { get; private set; }

        public RoomInsulation (string name, int countRooms, List<string> rulesExpressions)
        {
            this.NameType = name;
            this.CountRooms = countRooms;
            Rules = ParseRules(rulesExpressions);
        }

        private List<InsRule> ParseRules (List<string> rulesExpressions)
        {
            List<InsRule> rules = new List<InsRule>();
            foreach (var ruleExpr in rulesExpressions)
            {
                InsRule rule = new InsRule(ruleExpr);
                rules.Add(rule);
            }
            return rules;
        }        
    }

    /// <summary>
    /// Инсоляционное правило для квартиры - состоит из одного или нескольких требований (перечисленных через + в выражении требования)
    /// </summary>
    public class InsRule
    {
        /// <summary>
        /// Требование инсоляции (B - одно требование; B+2C - два требования, 1B и 2C инсолируемых помещения(окна) в квартире)
        /// </summary>
        public List<InsRequired> Requirements { get; private set; } = new List<InsRequired>();

        /// <summary>
        /// Требования инсоляции (C, 2D, C+2B)
        /// </summary>        
        public InsRule (string ruleExpr)
        {
            var indexes = ruleExpr.Split('+');
            foreach (var item in indexes)
            {
                var requireAdd = new InsRequired(item.Trim());
                Requirements.Add(requireAdd);
            }
            Requirements = Requirements.OrderByDescending(o => o.InsIndex).ToList();
        }       
    }

    /// <summary>
    /// Инсоляционное требование - один индекс и кол инсолируемых комнат(окон)
    /// </summary>
    public struct InsRequired
    {
        /// <summary>
        /// Требуемое кол инсолиуемых окон
        /// </summary>
        public double CountLighting { get; set; }
        /// <summary>
        /// Требуемый индекс инсоляции (A, B, C, D)
        /// </summary>
        public string InsIndex { get; private set; }    

        public InsRequired (string item)
        {            
            string insIndex;
            CountLighting = 0;
            InsIndex = string.Empty;
            CountLighting = GetCountLighting(item, out insIndex);
            InsIndex = insIndex;    
            
            if (!RoomInsulation.AllowedIndexes.Contains(InsIndex))
            {
                throw new Exception("Недопустимый индекс инсоляции в правилах - "+ InsIndex + ".\n " + 
                    "Допустимые индексы инсоляции " + string.Join(", ", RoomInsulation.AllowedIndexes));
            }
        }

        private int GetCountLighting (string item, out string insIndex)
        {
            var resCountLighting = 1;
            insIndex = item;
            // первый символ это требуемое число инсолируемых окон для данного индекса инсоляции, или пусто если 1.
            var firstChar = item.First(); 
            if (char.IsDigit(firstChar))
            {
                resCountLighting = (int)char.GetNumericValue(firstChar);
                insIndex = item.Substring(1);
            }
            return resCountLighting;
        }

        /// <summary>
        /// Проверка - проходит расчетный индекс инсоляции
        /// </summary>
        /// <param name="insIndexProject">Расчетный индекс инсоляции (по Excel)</param>
        /// <returns>Да, если расчетный индекс инсоляции выше или равен требуемому</returns>
        public bool IsPassed (string insIndexProject)
        {
            // Если проектный индекс больше требуемого, то проходит            
            var res = insIndexProject.CompareTo(InsIndex) >= 0;
            return res;
        }
    }
}
