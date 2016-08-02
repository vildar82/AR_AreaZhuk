using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_DataModel
{
    public class Section
    {
        public int IdSection { get; set; }
        public int CountStep { get; set; }
        public bool IsInvert { get; set; }
        public bool IsVertical { get; set; }
        public bool IsCorner { get; set; }
        public bool IsStartSectionInHouse { get; set; }             
        public bool IsEndSectionInHouse { get; set; }
        /// <summary>
        /// Направление движения:
        /// 1 - вниз или вправо
        /// -1 - вверх или влево
        /// </summary>
        public int Direction { get; set; }
        public string SpotOwner { get; set; }
        public int NumberInSpot { get; set; }
        public int CountModules { get; set; }
        public int Floors { get; set; }
        public string Code { get; set; }
        public SectionType SectionType { get; set; }    
        /// <summary>
        /// Стартовый торец секции
        /// </summary>
        public Joint JointStart { get; set; }
        /// <summary>
        /// Конечный торец секции
        /// </summary>
        public Joint JointEnd { get; set; }
        public double AxisArea { get; set; }
        public double TotalArea { get; set; }
        /// <summary>
        /// Инсоляция сверху - справа-налево
        /// Для боковой секции - от хвостового угла до 1 ячейки над ллу.
        /// </summary>
        public List<Module> InsTop { get; set; }
        /// <summary>
        /// Инсоляция снизу - слева-направо        
        /// Для угловой секции - от 1 углового углового шага (загиб) к хвосту, включая боковые ячейки
        /// </summary>
        public List<Module> InsBot { get; set; }
        /// <summary>
        /// Инсоляция с торца секции - если нет то null
        /// </summary>
        public List<Module> InsSide { get; set; }

        public List<FlatInfo> Sections = new List<FlatInfo>();
        public int TotalIndex = 0;
        public double RealIndex = 0;

        public Section Copy ()
        {
            var copySection = (Section)MemberwiseClone();
            return copySection;
            //Section newSection = new Section();
            //newSection.Sections = Sections;
            //newSection.IsCorner = IsCorner;
            //newSection.IsVertical = IsVertical;
            //newSection.NumberInSpot = NumberInSpot;
            //newSection.SpotOwner = SpotOwner;
            //newSection.CountStep = CountStep;
            //newSection.CountModules = CountModules;
            //newSection.Floors = Floors;
            //newSection.IsInvert = IsInvert;
            //newSection.AxisArea = AxisArea;
            //newSection.Code = Code;
            //newSection.Direction = Direction;
            //newSection.IdSection = IdSection;
            //newSection.InsBot = InsBot;
            //newSection.InsSide = InsSide;
            //newSection.InsTop = InsTop;
            //newSection.IsEndSectionInHouse = IsEndSectionInHouse;
            //newSection.IsStartSectionInHouse = IsStartSectionInHouse;
            //newSection.JointEnd = JointEnd;
            //newSection.JointStart = JointStart;
            //newSection.RealIndex = RealIndex;
            //newSection.SectionType = SectionType;
            //newSection.TotalArea = TotalArea;
            //newSection.TotalIndex = TotalIndex;

            //return newSection;
        }
    }
}
