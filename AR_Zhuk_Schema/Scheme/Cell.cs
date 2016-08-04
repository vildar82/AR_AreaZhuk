using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_Zhuk_Schema.Scheme
{
    /// <summary>
    /// Ячейка схемы инсоляции
    /// </summary>
    public struct Cell : IEquatable<Cell>
    {
        /// <summary>
        /// Направление - вверх
        /// </summary>
        public static readonly Cell Up = new Cell(-1, 0);
        /// <summary>
        /// Направление вниз
        /// </summary>
        public static readonly Cell Down = new Cell(1, 0);
        /// <summary>
        /// Направление влево
        /// </summary>
        public static readonly Cell Left = new Cell(0, -1);
        /// <summary>
        /// Направление вправо
        /// </summary>
        public static readonly Cell Right = new Cell(0, 1);        

        /// <summary>
        /// Строка
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// Столбец
        /// </summary>
        public int Col { get; set; }
        public Cell Negative {
            get {
                return new Cell(Row * -1, Col * -1);
            }
        }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;            
        }

        public Cell Offset (Cell offset)
        {
            var result = new Cell(Row + offset.Row, Col + offset.Col);
            return result;
        }

        public Cell OffsetNegative (Cell offset)
        {
            var result = new Cell(Row - offset.Row, Col - offset.Col);
            return result;
        }

        public static Cell operator * (Cell cell, int factor)
        {
            Cell res = new Cell(cell.Row * factor, cell.Col * factor);            
            return res;
        }

        public override string ToString ()
        {
            return "s[r" + Row + ",c" + Col + "]";
        }

        public bool Equals (Cell other)
        {
            var res = Row == other.Row && Col == other.Col;
            return res;
        }

        public override int GetHashCode ()
        {
            return Row.GetHashCode() ^ Col.GetHashCode();                
        }

        /// <summary>
        /// Направление полученное поворотом направо от текущего
        /// </summary>        
        public Cell ToRight ()
        {
            Cell toRight = new Cell(0, 0);
            if (Row != 0)
            {
                toRight.Col = Row > 0 ? -1 : 1;
            }
            else
            {
                toRight.Row = Col > 0 ? 1 : -1;
            }
            return toRight;
        }

        /// <summary>
        /// Направление полученное поворотом налево от текущего
        /// </summary>        
        public Cell ToLeft ()
        {
            Cell toLeft = new Cell(0,0);
            if (Row != 0)
            {
                toLeft.Col = Row > 0 ? 1 : -1;                
            }
            else
            {
                toLeft.Row = Col > 0 ? -1 : 1;
            }
            return toLeft;
        }
    }
}
