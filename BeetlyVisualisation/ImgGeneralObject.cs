using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetlyVisualisation
{
    class ImgGeneralObject
    {
        /// <summary>
        /// Список изображений домов
        /// </summary>
        public List<ImgHouse> ImgHouses;

        /// <summary>
        /// Длина объекта в модулях
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота объекта в модулях
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Размер модуля в пикселях
        /// </summary>
        public int moduleWidth { get; set; }

        // разрыв между изображениями домов в модулях
        int gap = 0;


        public ImgGeneralObject(int ModuleWidth)
        {
            this.ImgHouses = new List<ImgHouse>();

            this.moduleWidth = ModuleWidth;

            this.Width = 0;
            this.Height = 0;

            this.gap = 2;
        }


        private void CalculateDimentions()
        {
            foreach (ImgHouse house in this.ImgHouses)
            {
                // Принимаем за ширину объекта наиболее широкий дом
                if (house.Width > this.Width)
                {
                    this.Width = house.Width;
                }



                // Высота объекта - сумма высот домов
                this.Height += house.Height + gap;
            }

            // Удаление последнего зазора
            this.Height -= gap;
        }

        private void CalculateCoords()
        {
            int Y = 0;

            foreach (ImgHouse house in this.ImgHouses)
            {
                house.CoordY = Y;
                Y = house.Height + gap;
            }
        }

        public Bitmap Generate()
        {

            CalculateDimentions();
            CalculateCoords();

            int width = this.Width * this.moduleWidth;
            int height = this.Height * this.moduleWidth;

            var bitmap = new Bitmap(width, height);

            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.Clear(Color.White);
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                foreach (ImgHouse imgHouse in this.ImgHouses)
                {
                    int imgHouseWidth = 0;
                    int imgHouseHeight = 0;

                    imgHouseWidth = imgHouse.Width * moduleWidth;
                    imgHouseHeight = imgHouse.Height * moduleWidth;
                    imgHouse.CoordX *= moduleWidth;
                    imgHouse.CoordY *= moduleWidth;

                    Image frame = imgHouse.BmpImageHouse ?? new Bitmap(imgHouseWidth, imgHouseHeight);

                        canvas.DrawImage(frame,
                 new Rectangle(imgHouse.CoordX,
                               imgHouse.CoordY,
                               imgHouseWidth,
                               imgHouseHeight),
                 new Rectangle(0,
                               0,
                               //imgFlat.CoordY * ModuleWidth,
                               //-ModuleWidth,
                               frame.Width,
                               frame.Height),
                 GraphicsUnit.Pixel);





                }

                canvas.Save();
            }

            return bitmap;
        }
    }
}
