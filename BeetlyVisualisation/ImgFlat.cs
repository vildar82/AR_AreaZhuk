using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetlyVisualisation
{
    class ImgFlat
    {
       public string ImgPath { get; set; }
       public int CoordX { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
        public int CoordY { get; set; }
        public bool IsInsPassed { get; set; }
        public string ShortName { get; set; }

        public ImgFlat(string dir, string name, int coordX)
        {
            this.ImgPath = dir + name + ".png";
            this.CoordX = coordX;
            this.CoordY = -1;
        }
    }
}
