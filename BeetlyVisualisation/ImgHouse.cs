using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BeetlyVisualisation
{
    class ImgHouse : IDisposable
    {


        /// <summary>
        /// Список изображений секций
        /// </summary>
        public List<ImgSection> ImgSections;

        /// <summary>
        /// Флаг ориентации угловой секции.
        /// Для |____-ориентированной угловой секции - true
        /// Для ____| - ориентированной угловой секции - false
        /// </summary>
        public bool LOrientation { get; set; }

        /// <summary>
        /// Флаг ориентации линейного вертикального дома
        /// </summary>
        public bool IsVertical { get; set; }

        /// <summary>
        /// Флаг ориентации углового дома
        /// </summary>
        public bool IsCorner { get; set; }


        /// <summary>
        /// Длина дома в модулях
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота дома в модулях
        /// </summary>
        public int Height { get; set; }


        /// <summary>
        /// Изображение дома
        /// </summary>
        public Bitmap BmpImageHouse { get; set; }

        public int CoordX { get; set; }
        public int CoordY { get; set; }



        public ImgHouse()
        {
            this.ImgSections = new List<ImgSection>();
            // По умолчанию выставляется как ____| - ориентированная угловая секция
            this.LOrientation = false;

            // По умолчанию выставляется как вертикально ориентированная секция
            this.IsVertical = true;

            // По умолчанию выставляется как линейная секция
            this.IsCorner = false;

            this.Width = 0;
            this.Height = 0;

            this.CoordX = 0;
            this.CoordY = 0;
        }


        /// <summary>
        /// Вычисление длины и ширины дома
        /// </summary>
        private void CalculateHouseImageDimentions(int ModuleWidth)
        {
            // Вычисление размеров изображения дома
            // Назначение координат для размещения секций

            foreach (ImgSection section in ImgSections)
            {
                // Определение размеров изображения дома
                // Угловой дом
                if (IsCorner)
                {
                    // TODO переделать определение типов секций через enum
                    // Вертикальная секция
                    if (section.SectionInfo.IsVertical)
                    {
                        Height += section.Lenght;
                    }
                    else
                    {
                        // Угловая секция
                        if (section.SectionInfo.IsCorner)
                        {
                            Height += section.Height;
                            Width = section.Lenght;
                        }
                        // Горизонтальная секция
                        else
                        {
                            Width += section.Lenght;
                        }

                    }

                }
                else
                {
                    // Дом линейный вертикальный
                    if (IsVertical)
                    {
                        Height += section.Lenght;
                        Width = section.Height;
                    }
                    // Дом линейный горизонтальный
                    else
                    {
                        Height = section.Height;
                        Width += section.Lenght;
                    }
                }
            }
        }

        /// <summary>
        /// Вычисление координат секций
        /// </summary>
        private void CalculateImageSectionsCoords(int ModuleWidth)
        {
            int X = 0;
            int Y = 0;

            foreach (ImgSection section in ImgSections)
            {



                // Определение положения секций дома
                // Угловой дом
                if (IsCorner)
                {
                    // TODO переделать определение типов секций через enum
                    // Вертикальная секция
                    if (section.SectionInfo.IsVertical)
                    {
                        if (LOrientation)
                        {
                            section.CoordX = 0;
                        }
                        else
                        {
                            section.CoordX = Width - section.Height;
                        }

                        section.CoordY = Y;

                        Y += section.Lenght;
                    }
                    else
                    {
                        // Угловая секция
                        if (section.SectionInfo.IsCorner)
                        {
                            if (LOrientation)
                            {
                                section.CoordX = 0;
                                section.CoordY += Y;
                                Y = section.CoordY;
                                X = section.Lenght;
                            }
                            else
                            {
                                section.CoordX = Width - section.Height;
                                X = section.CoordX;
                                section.CoordY += Y;
                                Y = section.CoordY;

                            }
                        }
                        // Горизонтальная секция
                        else
                        {
                            if (LOrientation)
                            {
                                section.CoordX = X;
                                X += section.Lenght;
                                section.CoordY = Y - section.CoordYCorrection;
                            }
                            else
                            {
                                section.CoordX = X - section.Lenght;
                                X = section.CoordX;
                                section.CoordY = Y - section.CoordYCorrection;
                            }
                        }
                    }

                }
                else
                {
                    // Дом линейный вертикальный
                    if (IsVertical)
                    {
                        section.CoordX = 0;
                        section.CoordY = Y + section.Lenght;
                        Y = section.CoordY;
                    }
                    // Дом линейный горизонтальный
                    else
                    {
                        Height = section.Height;
                        Width += section.Lenght;
                    }
                }

            }

        }


        public Bitmap generateOneHouse(ImgHouse House, int ModuleWidth)
        {
            CalculateHouseImageDimentions(ModuleWidth);
            CalculateImageSectionsCoords(ModuleWidth);


            int width = House.Width * ModuleWidth;
            int height = House.Height * ModuleWidth;

            var bitmap = new Bitmap(width, height);

            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.Clear(Color.White);
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;



                foreach (ImgSection imgSection in House.ImgSections)
                {

                    int imgSectionWidth = 0;
                    int imgSectionHeight = 0;

                    if (imgSection.SectionInfo.IsVertical)
                    {
                        // TODO: в классе изображения секции привести значения длины секции и высоты секции в соответствии с изображением png
                        imgSectionWidth = imgSection.Height;
                        imgSectionHeight = imgSection.Lenght;
                    }
                    else
                    {
                        imgSectionWidth = imgSection.Lenght;
                        imgSectionHeight = imgSection.Height;
                    }



                    imgSectionWidth *= ModuleWidth;
                    imgSectionHeight *= ModuleWidth;
                    imgSection.CoordX *= ModuleWidth;
                    imgSection.CoordY *= ModuleWidth;



                    Image frame;
                    try
                    {

                        frame = imgSection.BmpSection;
                    }
                    catch (Exception ex)
                    {
                        frame = new Bitmap(imgSectionWidth, imgSectionHeight);                        
                    }


                    //TODO: Здесь возникала ошибка с отсутствующим изображением PIK1_BS_L_10-17_A
                    try
                    {
                        canvas.DrawImage(frame,
                 new Rectangle(imgSection.CoordX,
                               imgSection.CoordY,
                               //imgFlat.CoordY * ModuleWidth * 2,
                               //-ModuleWidth - ModuleWidth/4,
                               imgSectionWidth,
                               imgSectionHeight),
                 new Rectangle(0,
                               0,
                               //imgFlat.CoordY * ModuleWidth,
                               //-ModuleWidth,
                               frame.Width,
                               frame.Height),
                 GraphicsUnit.Pixel);
                    }
                    catch (Exception)
                    {

                        
                    }




                    //canvas.Save();                        
                }

                // canvas.RotateTransform(7);
                canvas.Save();
            }


            return bitmap;

        }




        public void Dispose()
        {

        }
    }
}
