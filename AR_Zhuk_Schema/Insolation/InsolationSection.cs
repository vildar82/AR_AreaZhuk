using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_Zhuk_Schema.Insolation
{
    class InsolationSection : IInsolation
    {
        public static readonly List<RoomInsolation> RoomInsolations = new List<RoomInsolation>()
            {
                new RoomInsolation ("Однокомнатная или студия", 1, new List<string>() { "C" }),
                new RoomInsolation ("Двухкомнатная", 2, new List<string>() { "C", "2B" }),
                new RoomInsolation("Трехкомнатная", 3, new List<string>() { "C", "2B" }),
                new RoomInsolation ("Четырехкомнатная", 4, new List<string>() { "2C", "C+2B" })
            };

        private readonly SpotInfo spOrig;

        public InsolationSection(SpotInfo sp)
        {
            this.spOrig = sp;
        }

        /// <summary>
        /// Проверка инсоляции секции
        /// </summary>
        /// <param name="section">Проверяемая секция</param>
        /// <returns>Секции прошедшие инсоляцию</returns>
        public List<FlatInfo> GetInsolationSections (Section section)
        {
            IInsCheck insCheck = InsCheckFactory.CreateInsCheck(this, section);

            List<FlatInfo> resFlats = new List<FlatInfo>();

            foreach (var sectFlats in section.Sections)
            {
                sectFlats.Code =GetFlatCode(sectFlats);

#if TEST
                //// !!!! Только для тестирования!!!! - добавление всех секций с пометками квартир прошедших/непрошедших инсоляцию
                FlatInfo flats = NewFlats(section, sectFlats, isInvert: false);
                insCheck.CheckSection(flats, isRightOrTopLLu: true);                
                resFlats.Add(flats);

                if (!section.IsCorner)
                {
                    flats = NewFlats(section, sectFlats, isInvert: true);
                    insCheck.CheckSection(flats, isRightOrTopLLu: false);                    
                    resFlats.Add(flats);
                }
#else
                // Проверка однотипной секции
                FlatInfo flats = NewFlats(section, sectFlats, isInvert: false);
                if (!IsIdenticalSection(flats, resFlats))
                {
                    // Добавление прошедших инсоляцию секций
                    if (insCheck.CheckSection(flats, isRightOrTopLLu: true))
                    {                        
                        resFlats.Add(flats);
                    }
                }


                if (!section.IsCorner)
                {
                    // Проверка однотипной секции
                    flats = NewFlats(section, sectFlats, isInvert: true);
                    if (!IsIdenticalSection(flats, resFlats))
                    {
                        // Проверка инсоляции инвертированной секции                        
                        if (insCheck.CheckSection(flats, isRightOrTopLLu: false))
                        {                            
                            resFlats.Add(flats);
                        }
                    }
                }               
#endif
            }
            return resFlats;
        }

        private bool IsIdenticalSection (FlatInfo curSection, List<FlatInfo> resulsSections)
        {
            var res = resulsSections.Any(s => (s.IsInvert == curSection.IsInvert) &&
                                IsEqualSections(s.Flats, curSection.Flats));
            return res;
        }

        public bool IsEqualSections (List<RoomInfo> section1, List<RoomInfo> section2)
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

        private string GetFlatCode(FlatInfo flats)
        {
            var sp = spOrig.CopySpotInfo(spOrig);
            for (int l = 0; l < flats.Flats.Count; l++) //Квартиры
            {
                if (flats.Flats[l].SubZone.Equals("0")) continue;
                var reqs =
                    sp.requirments.Where(
                        x => x.CodeZone.Equals(flats.Flats[l].SubZone))
                        .Where(
                            x =>
                                x.MaxArea + 5 >= flats.Flats[l].AreaTotal &
                                x.MinArea - 5 <= flats.Flats[l].AreaTotal)
                        .ToList();
                if (reqs.Count == 0) continue;
                reqs[0].RealCountFlats++;
            }
            string code = "";
            foreach (var r in sp.requirments)
            {
                code += r.RealCountFlats.ToString();
            }
            return code;            
        }

        public RoomInsolation FindRule (RoomInfo flat)
        {
            var rule = RoomInsolations.Where(x => x.CountRooms == Convert.ToInt32(flat.SubZone)).FirstOrDefault();
            return rule;
        }

        public List<RoomInfo> GetSideFlatsInSection (List<RoomInfo> sectionFlats, bool isTop)
        {
            List<RoomInfo> topFlats = new List<RoomInfo>();

            if (isTop)
            {
                int indexFirstBottomFlat = 0;
                for (int i = 0; i < sectionFlats.Count; i++)
                {
                    indexFirstBottomFlat = i;
                    if (sectionFlats[i].SelectedIndexTop == 0)
                        break;
                }

                for (int i = indexFirstBottomFlat; i < sectionFlats.Count; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop == 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }

                for (int i = 0; i < indexFirstBottomFlat; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop == 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }
            }
            else
            {
                for (int i = 0; i < sectionFlats.Count; i++)
                {
                    if (sectionFlats[i].SelectedIndexTop != 0) continue;
                    topFlats.Add(sectionFlats[i]);
                }
            }

            return topFlats;
        }

        private Section CreateSection (Section section,bool isInvert)
        {
            Section resSection = section.Copy();
            resSection.Sections = new List<FlatInfo>();            
            resSection.IsInvert = isInvert;
            return resSection;
        }

        private FlatInfo NewFlats (Section section, FlatInfo flat, bool isInvert)
        {
            FlatInfo resFlats = new FlatInfo();
            resFlats.IdSection = flat.IdSection;
            resFlats.SpotOwner = section.SpotOwner;
            resFlats.NumberInSpot = section.NumberInSpot;
            resFlats.IsCorner = section.IsCorner;
            resFlats.IsVertical = section.IsVertical;
            resFlats.CountStep = flat.CountStep;
            resFlats.IsInvert = isInvert;
            resFlats.Floors = flat.Floors;
            resFlats.Code = flat.Code;
#if TEST
            resFlats.Flats = flat.Flats.Select(f => (RoomInfo)f.Clone()).ToList();
            // Временно - подмена индекса освещенностим для боковых квартир!!!???
            foreach (var itemFlat in resFlats.Flats)
            {
                var sideFlat = SideFlatFake.GetSideFlat(itemFlat.Type);
                if (sideFlat != null)
                {
                    itemFlat.LightingTop = sideFlat.LightingTop;
                    itemFlat.LightingNiz = sideFlat.LightingBot;
                }
            }            
#else
            resFlats.Flats = flat.Flats;
#endif            
            return resFlats;
        }
    }

    /// <summary>
    /// Правила инсоляции для квартиры (общие правила по типам квартир - 1,2,3,4 комнатной)
    /// </summary>
    public class RoomInsolation
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

        public RoomInsolation (string name, int countRooms, List<string> rulesExpressions)
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

            if (!RoomInsolation.AllowedIndexes.Contains(InsIndex))
            {
                throw new Exception("Недопустимый индекс инсоляции в правилах - " + InsIndex + ".\n " +
                    "Допустимые индексы инсоляции " + string.Join(", ", RoomInsolation.AllowedIndexes));
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
