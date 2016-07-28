using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    public struct Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void Offset (Cell offset)
        {
            Row += offset.Row;
            Col += offset.Col;
        }

        public void OffsetNegative(Cell offset)
        {
            Row -= offset.Row;
            Col -= offset.Col;
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
    }
}
