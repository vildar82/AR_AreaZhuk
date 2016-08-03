using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AR_Zhuk_DataModel;

namespace BeetlyVisualisation
{
    class ImgSection
    {
        string ImgPath { get; set; }
        public  int CoordX { get; set; }
        public int CoordY { get; set; }

        /// <summary>
        /// Длина секции в модулях
        /// </summary>
        public int Lenght { get; set; }

        /// <summary>
        /// Высота секции в модулях
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Коррекция положения секции по высоте
        /// </summary>
        public int CoordYCorrection { get; set; }

        /// <summary>
        /// Угол вращения секции
        /// </summary>
        public int Angle { get;  set; }
        

        public List<ImgFlat> ImgFlats;

        /// <summary>
        /// Изображение секции
        /// </summary>
        public Bitmap BmpSection { get; set; }

        /// <summary>
        /// Информация о секции
        /// </summary>
        public FlatInfo SectionInfo { get; set; }

        public ImgSection(string dir, string name, int coordX, int coordY, List<ImgFlat> imgFlats)
        {
            this.ImgPath = dir + "\\" + name + ".png";
            this.CoordX = coordX;
            this.CoordY = coordY;

            this.ImgFlats = imgFlats;
        }

        public ImgSection()
        {
            ImgFlats = new List<ImgFlat>();
            Lenght = 0;
            Height = 4;
            this.CoordYCorrection = -1;
            this.Angle = 0;            
        }
    }
}
