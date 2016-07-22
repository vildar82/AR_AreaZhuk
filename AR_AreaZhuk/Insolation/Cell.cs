using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation
{
    struct Cell
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
    }
}
