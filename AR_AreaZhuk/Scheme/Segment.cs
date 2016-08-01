using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Scheme
{
    /// <summary>
    /// Линейный сегмент дома
    /// Direction - направление сегмента - от старта к концу
    /// Определение сторон - взгляд от стартового торца в направлении сегмента - определяет левую и правую сторону сегмента.
    /// </summary>
    public class Segment
    {
        private ISchemeParser parser;
        public readonly int StartLevel;
        public readonly int EndLevel;

        public readonly Cell CellStartLeft;
        public readonly Cell CellStartRight;
        public readonly Cell CellEndLeft;
        public readonly Cell CellEndRight;
        public readonly Cell Direction;
        public readonly List<Module> ModulesLeft;
        public readonly List<Module> ModulesRight;
        public readonly SegmentEnd StartType;
        public readonly SegmentEnd EndType;
        public readonly bool IsVertical;

        public HouseSpot HouseSpot { get; private set; }

        /// <summary>
        /// Номер сегмента в доме
        /// </summary>
        public int Number { get; private set; }
        public int CountSteps { get; private set; }

        public Segment(Cell cellStartLeft, Cell cellStartRight, Cell direction, ISchemeParser parser, HouseSpot houseSpot)
        {
            HouseSpot = houseSpot;
            Number = houseSpot.Segments.Count + 1;

            CellStartLeft = cellStartLeft;
            CellStartRight = cellStartRight;
            Direction = direction;
            this.parser = parser;            
            
            ModulesLeft = parser.GetSteps(cellStartLeft, direction, out CellEndLeft);
            ModulesRight = parser.GetSteps(cellStartRight, direction, out CellEndRight);

            // Кол шагов в секции
            CountSteps = ModulesLeft.Count > ModulesRight.Count ? ModulesLeft.Count : ModulesRight.Count;

            IsVertical = defineVertical();

            StartLevel = GetMaxLevel(cellStartLeft, cellStartRight, direction);
            EndLevel = GetMaxLevel(CellEndLeft, CellEndRight, direction.Negative);

            // Определение вида торцов сегмента
            StartType = defineEndType(CellStartLeft, CellStartRight, direction.Negative, true);
            EndType = defineEndType(CellEndLeft, CellEndRight, direction, false);
        }

        /// <summary>
        /// Проверка попадает ли шаг в мертвую зону сегмента (угол)
        /// </summary>
        /// <param name="step">Шаг в сегменте</param>        
        public bool StepInDeadZone (int step)
        {
            // Если стартовый торец секции - угловой и шаг попадает в угол
            if ((StartType == SegmentEnd.CornerLeft || StartType == SegmentEnd.CornerRight) &&
                (step < HouseSpot.CornerSectionMinStep-1 && step != HouseSpot.WidthOrdinary +1 ))
            {
                return true;
            }
            if (EndType == SegmentEnd.CornerLeft || EndType == SegmentEnd.CornerRight)
            {
                int counToEnd = CountSteps - step;
                if (counToEnd< HouseSpot.CornerSectionMinStep && counToEnd != HouseSpot.WidthOrdinary+2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Определение вертикальности сегмента
        /// </summary>
        /// <returns></returns>
        private bool defineVertical ()
        {
            bool isVertical = Direction.Row != 0 ? true : false;
            return isVertical;
        }

        /// <summary>
        /// Определение торцов сегмента - начало или конец дома, нормальный или угловой торец
        /// </summary>
        /// <param name="directionOut">Направление от торца во вне сегмента</param>
        private SegmentEnd defineEndType (Cell cellEndLeft, Cell cellEndRight, Cell directionOut, bool isStartEnd)
        {
            SegmentEnd endType;
            // Если ячейки на одном уровне, то это не угловой торец
            if (isOnSomeStep(cellEndLeft, cellEndRight))
            {
                if (parser.IsInsCell(cellEndLeft.Offset(directionOut)))
                {
                    endType = SegmentEnd.Normal;
                }
                else
                {
                    endType = SegmentEnd.End;
                }
            }
            else
            {
                // угловой торец
                endType = GetCornerEnd(cellEndLeft, cellEndRight, isStartEnd);
            }
            return endType;           
        }      

        /// <summary>
        /// Определение - на одном ли шаге сегмента находятся ячейки
        /// </summary>        
        private bool isOnSomeStep (Cell cell1, Cell cell2)
        {
            bool res = IsVertical ? cell1.Row == cell2.Row : cell1.Col == cell2.Col;            
            return res;
        }

        /// <summary>
        /// Определение типа углового торца
        /// </summary>
        /// <param name="cellEndLeft">Крайняя левая точка сегмента (по направлению)</param>
        /// <param name="cellEndRight">Крайняя правая точка сегмента</param>
        /// <param name="isStartEnd">Это стартовый торец</param>
        /// <returns></returns>
        private SegmentEnd GetCornerEnd (Cell cellEndLeft, Cell cellEndRight, bool isStartEnd)
        {
            SegmentEnd resEndCornerType;
            int dir = IsVertical ? Direction.Row : Direction.Col;
            int levelLeft = GetCellLevel(CellEndLeft) * dir;
            int levelRight = GetCellLevel(cellEndRight) * dir;
            if (isStartEnd)
            {
                resEndCornerType = levelLeft > levelRight ? SegmentEnd.CornerLeft : SegmentEnd.CornerRight;                
            }
            else
            {
                resEndCornerType = levelLeft > levelRight ? SegmentEnd.CornerRight : SegmentEnd.CornerLeft;
            }
            return resEndCornerType;
        }

        /// <summary>
        /// Максимальный уровень по основному направлению
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns></returns>
        private int GetMaxLevel (Cell cell1, Cell cell2, Cell directionInner)
        {
            int level = 0;
            int dir = IsVertical ? directionInner.Row : directionInner.Col;
            int l1 = GetCellLevel(cell1) * dir;
            int l2 = GetCellLevel(cell2) * dir;            
            level = Math.Abs(l1 < l2 ? l1 : l2);            
            return level;
        }

        private int GetCellLevel (Cell cell)
        {
            var res = IsVertical ? cell.Row : cell.Col;
            return res;
        }
    }
}
