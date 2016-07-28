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
    /// Инсоляция одного пятна (дома)
    /// </summary>
    public class InsolationSpot
    {
        /// <summary>
        /// Ширина секции - в шагах (модулях)
        /// </summary>
        public const int CountStepWithSection = 4;

        internal int curNumbersSectionsInSpot;
        private static StartCellHelper startCellHelper;
        internal InsolationFrameWork insFramework = new InsolationFrameWork();

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
            // Учитывается порядок индексов A<B<C<D. Поэтому писать только минимальное требования. Например, если требуется C, то писать требование D не обязательно.                   
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
        /// </summary>        
        /// <param name="cellFirstSection">Угол первой секции - левый верхний для вертикальной или левый нижний для горизонтальной</param>
        public Section GetInsulationSections (List<FlatInfo> sections, bool isVertical,
            bool isCorner, int numberSection, int numbersSectionsInSpot, SpotInfo spotInfo, Cell cellFirstSection)
        {
            curNumbersSectionsInSpot = numbersSectionsInSpot;
            Section s = CreateSection(sections, isVertical, isCorner, numberSection, isInvert:false);            

            // Определение стартовой точки секции в матрице инсоляции     
            // Начальная точка первой секции   
            if (numberSection == 1)
            {
                startCellHelper = new StartCellHelper(this, s, spotInfo, cellFirstSection);
            }
            else
            {
                startCellHelper.Define(s);
            }

            IInsCheck insCheck = InsCheckFactory.CreateInsCheck(this, s,
                startCellHelper, sections, spotInfo);

            foreach (var sect in sections)
            {
                // Пропуск секций с малым количеством квартир (меньше 5 (вместе с ЛЛУ))
                //if (sect.Flats.Count < 5)
                //    continue;

#if TEST
                //// !!!! Только для тестирования!!!! - добавление всех секций с пометками квартир прошедших/непрошедших инсоляцию
                FlatInfo flats = NewFlats(isVertical, isCorner, numberSection, sect, isInvert: false);
                insCheck.CheckSection(flats, isRightOrTopLLu: true);                
                s.Sections.Add(flats);

                if (!isCorner)
                {
                    flats = NewFlats(isVertical, isCorner, numberSection, sect, isInvert: true);                    
                    insCheck.CheckSection(flats, isRightOrTopLLu: false);                    
                    s.Sections.Add(flats);
                }
#else
                // Добавление прошедших инсоляцию секций
                if (insCheck.CheckSection(sect, isRightOrTopLLu: true))
                {
                    FlatInfo flats = NewFlats(isVertical, isCorner, numberSection, sect, isInvert:false);
                    s.Sections.Add(flats);
                }

                if (!isCorner)
                {
                    // Проверка инсоляции инвертированной секции
                    if (insCheck.CheckSection(sect, isRightOrTopLLu: false))
                    {
                        FlatInfo flats = NewFlats(isVertical, isCorner, numberSection, sect, isInvert: true);
                        s.Sections.Add(flats);
                    }
                }               
#endif
            }
            return s;
        }

        private Section CreateSection (List<FlatInfo> sections, bool isVertical, bool isCorner, int numberSection, bool isInvert)
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
            s.IsInvert = isInvert;
            return s;
        }

        private FlatInfo NewFlats (bool isVertical, bool isCorner, int numberSection, FlatInfo sect, bool isInvert)
        {
            FlatInfo flats = new FlatInfo();
            flats.IdSection = sect.IdSection;
            flats.SpotOwner = Name;
            flats.NumberInSpot = numberSection;
#if TEST
            flats.Flats = sect.Flats.Select(f => (RoomInfo)f.Clone()).ToList();
            // Временно - подмена индекса освещенностим для боковых квартир!!!???
            foreach (var flat in flats.Flats)
            {         
                var sideFlat = SideFlatFake.GetSideFlat(flat.Type);
                if (sideFlat != null)
                {
                    flat.LightingTop = sideFlat.LightingTop;
                    flat.LightingNiz = sideFlat.LightingBot;
                }
            }            
            flats.StartSextionCell = startCellHelper.StartCell.ToString();
#else
            flats.Flats = sect.Flats;
#endif
            flats.IsCorner = isCorner;
            flats.IsVertical = isVertical;
            flats.CountStep = sect.CountStep;
            flats.IsInvert = isInvert;
            flats.Floors = sect.Floors;
            return flats;
        }

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
