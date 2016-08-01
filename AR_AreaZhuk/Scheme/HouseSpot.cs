using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.Scheme.SpatialIndex;

namespace AR_AreaZhuk.Scheme
{
    /// <summary>
    /// Пятно одного дома
    /// Состоит из прямых участков (сегментов)
    /// </summary>
    public class HouseSpot
    {
        /// <summary>
        /// Ширина обычной секции в шагах
        /// </summary>
        public const int WIDTHORDINARY = 4;

        private RTree<Segment> tree = new RTree<Segment>();
        private readonly Cell cellStart;
        private readonly ISchemeParser parser;

        public string SpotName { get; private set; }
        public List<Segment> Segments { get; private set; } = new List<Segment>();

        public HouseSpot (string spotName, Cell cellStart, ISchemeParser parser)
        {
            SpotName = spotName;
            this.cellStart = cellStart;
            this.parser = parser;
            DefineSpot();
        }

        public void DefineSpot ()
        {
            // Определение начального сегмента дома
            DefineStartSegment();
            DefineOtherSegments();
        }

        protected void AddSegment (Segment segment)
        {
            Segments.Add(segment);
            // добавление прямоугольника сегмента в дерево, для проверки попадания любой ячейки в этот дом
            Rectangle r = GetRectangle(segment);
            tree.Add(r, segment);
        }

        /// <summary>
        /// Проверка входит ли ячейка в этот дом
        /// </summary>      
        public bool HasCell (Cell cell)
        {
            bool res = false;
            // 1 ячейка отступа от границы дома - т.к. она не может использоваться другим домом
            Rectangle r = new Rectangle(cell.Col - 1, cell.Row - 1, cell.Col + 1, cell.Row + 1, 0, 0);
            var segments = tree.Intersects(r);
            if (segments != null && segments.Count > 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// определение стартового сегмента
        /// </summary>
        private void DefineStartSegment ()
        {
            // Наименьшая длина дома от стартовой точки - определяет начало стартового сегмента.
            // Влево от стартовой точки 
            Cell lastCellLeft;
            var modulesLeft = parser.GetSteps(cellStart, Cell.Left, out lastCellLeft);
            if (modulesLeft.Count == WIDTHORDINARY)
            {
                // Сегмент вертикальный сверху-вниз
                var startSegment = new Segment(lastCellLeft, cellStart, Cell.Down, parser);
                AddSegment(startSegment);
                return;
            }

            // Вниз от стартовой точки 
            Cell lastCellDown;
            var modulesDown = parser.GetSteps(cellStart, Cell.Down, out lastCellDown);
            if (modulesDown.Count == WIDTHORDINARY)
            {
                // Сегмент горизонтальный слева-направо
                var startSegment = new Segment(cellStart, lastCellDown, Cell.Right, parser);
                AddSegment(startSegment);
                return;
            }

            // от нижней точки влево
            Cell lastCell;
            var modules = parser.GetSteps(lastCellDown, Cell.Left, out lastCell);
            if (modules.Count == WIDTHORDINARY)
            {
                // Сегмент вертикальный снизу-вверх
                var startSegment = new Segment(lastCellDown, lastCell, Cell.Up, parser);
                AddSegment(startSegment);
                return;
            }

            // Последний заворот, от последней точки вверх
            Cell lastCellLast;
            modules = parser.GetSteps(lastCell, Cell.Up, out lastCellLast);
            if (modules.Count == WIDTHORDINARY)
            {
                // Сегмент горизонтальный слева-направо
                var startSegment = new Segment(lastCell, lastCellLast, Cell.Left, parser);
                AddSegment(startSegment);
                return;
            }

            // Значит это башня    
            if (modulesLeft.Count < modulesDown.Count)
            {
                var segment = new Segment(lastCellLeft, cellStart, Cell.Down, parser);
            }
            else
            {
                var segment = new Segment(cellStart, lastCellDown, Cell.Left, parser);
            }

            // Сюда не должен никогда попасть
            throw new InvalidOperationException("Не определено начало дома");
        }

        /// <summary>
        /// Определение остальных сегментов дома
        /// </summary>
        private void DefineOtherSegments ()
        {
            // Последний сегмент в доме
            var lastSegment = Segments.Last();

            Segment newSegment = null;
            if (lastSegment.EndType == SegmentEnd.Normal)
            {
                newSegment =new Segment(lastSegment.CellEndLeft.Offset(lastSegment.Direction),
                    lastSegment.CellEndRight.Offset(lastSegment.Direction), lastSegment.Direction, parser);
                               
            }            
            else if (lastSegment.EndType != SegmentEnd.End)
            {
                Cell newSegmentDir = lastSegment.EndType == SegmentEnd.CornerLeft ? 
                    lastSegment.Direction.ToLeft() : lastSegment.Direction.ToRight();                
                newSegment = new Segment(lastSegment.CellEndLeft.Offset(lastSegment.Direction).Offset(newSegmentDir),
                    lastSegment.CellEndRight.Offset(newSegmentDir), newSegmentDir, parser);
            }            
            else
            {
                // Конец дома
                return;
            }
            AddSegment(newSegment);
            DefineOtherSegments();
        }

        private Rectangle GetRectangle (Segment segment)
        {
            Cell startRightMin;
            Cell endLeftMax;

            // Стартовый 
            if (segment.StartType == SegmentEnd.Normal || segment.StartType == SegmentEnd.End)
            {
                startRightMin = segment.CellStartRight;
            }
            else
            {
                // угловой торец у сегмента
                if (segment.IsVertical)
                {
                    startRightMin = segment.CellStartRight;
                    startRightMin.Row = segment.StartLevel;
                }
                else
                {
                    startRightMin = segment.CellStartRight;
                    startRightMin.Col = segment.StartLevel;
                }
            }

            // Конечный торец
            if (segment.EndType == SegmentEnd.Normal || segment.EndType == SegmentEnd.End)
            {
                endLeftMax = segment.CellEndLeft;
            }
            else
            {
                // угловой торец у сегмента
                if (segment.IsVertical)
                {
                    endLeftMax = segment.CellEndLeft;
                    endLeftMax.Row = segment.EndLevel;
                }
                else
                {
                    endLeftMax = segment.CellEndLeft;
                    endLeftMax.Col = segment.EndLevel;
                }
            }

            Rectangle r = new Rectangle(startRightMin.Col, startRightMin.Row, endLeftMax.Col, endLeftMax.Row, 0, 0);
            return r;
        }
    }
}
