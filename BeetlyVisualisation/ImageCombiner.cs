#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using AR_AreaZhuk;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using AR_Zhuk_DataModel;




namespace BeetlyVisualisation
{
    public class ImageCombiner
    {
        /// <summary>
        /// Данные по домам
        /// </summary>
        GeneralObject genObj;


        /// <summary>
        /// Список изображений секций
        /// </summary>
        List<ImgSection> imgSections;


        /// <summary>
        /// Путь к папке c исходными изображениями
        /// </summary>
        string ImagePath;


        /// <summary>
        /// Ширина одного модуля в пикселях
        /// 1 модуль - 3600 мм
        /// </summary>
        int ModuleWidth { get; }

        /// <summary>
        /// Построение вариантов по файлу XML
        /// </summary>
        /// <param name="XMLPath">Путь к файлу XML с данными по домам</param>
        /// <param name="excelDataPath">Путь к файлу XLSX с данными по квартирам</param>
        /// <param name="imageSourcePath">Путь к папке с изображениями квартир</param>
        /// <param name="imageOutPutPath">Путь к папке для генерации изображений домов</param>
        /// <param name="moduleWidth">Ширина одного модуля в пикселях</param>
        public ImageCombiner (string XMLPath, string excelDataPath, string imageSourcePath, int moduleWidth)
        {

            Serializer ser = new Serializer();
            this.genObj = Utils.getHouseInfo(XMLPath);
            Utils.AddInfoForVisualisation(this.genObj, excelDataPath);
            imgSections = new List<ImgSection>();

            this.ImagePath = imageSourcePath.TrimEnd('\\') + "\\";

            ModuleWidth = moduleWidth;
        }


        /// <summary>
        /// Построение вариантов по списку информации по объекту
        /// </summary>        
        /// <param name="excelDataPath">Путь к файлу XLSX с данными по квартирам</param>
        /// <param name="imageSourcePath">Путь к папке с изображениями квартир</param>        
        /// <param name="moduleWidth">Ширина одного модуля в пикселях</param>
        /// <param name="GeneralObject"></param>
        public ImageCombiner (GeneralObject GenObj, string excelDataPath, string imageSourcePath, int moduleWidth)
        {
            this.genObj = GenObj;
            Utils.AddInfoForVisualisation(this.genObj, excelDataPath);
            imgSections = new List<ImgSection>();

            this.ImagePath = imageSourcePath.TrimEnd('\\') + "\\";

            ModuleWidth = moduleWidth;
        }


        public void CombineImages (string imageOutPutPath)
        {
            int HouseIndex = 1;

            List<ImgHouse> imgHouses = new List<ImgHouse>();

            foreach (HouseInfo hi in this.genObj.Houses)
            {
                ImgHouse imgH = generateOneHouse(hi);

                string imgHName = "Вариант" + HouseIndex + ".png";
                string imgHDir = imageOutPutPath;

                SaveImage(imgH.BmpImageHouse, imgHDir, imgHName);
                GC.Collect();

                HouseIndex++;
            }
        }

        /// <summary>
        /// Генерация изображения в указанную папку
        /// </summary>
        /// <param name="imageOutPutPath">Путь к папке, в которой будет сохранено изображение</param>
        public void generateGeneralObject (string imageOutPutPath)
        {
            Bitmap bmpImageGO = _generateGeneralObject();

            string imgHName = "Площадка" + ".png";
            string imgHDir = imageOutPutPath;

            SaveImage(bmpImageGO, imgHDir, imgHName);
        }

        /// <summary>
        /// Генерация изображения
        /// </summary>
        /// <returns>Bitmap</returns>
        public Bitmap generateGeneralObject ()
        {
            Bitmap bmpImageGO = _generateGeneralObject();
            return bmpImageGO;
        }


        private Bitmap _generateGeneralObject ()
        {
            ImgGeneralObject imgGO = new ImgGeneralObject(ModuleWidth);


            foreach (HouseInfo hi in this.genObj.Houses)
            {
                ImgHouse imgH = generateOneHouse(hi);
                imgGO.ImgHouses.Add(imgH);
            }



            Bitmap bmpImageGO = imgGO.Generate();

            return bmpImageGO;
        }

        /// <summary>
        /// Построение изображений отдельных домов
        /// </summary>
        private ImgHouse generateOneHouse (HouseInfo hi)
        {
            int SectionIndex = 1;



            ImgHouse imgHouse = new ImgHouse();

            List<Bitmap> bmpSections = new List<Bitmap>();

            foreach (FlatInfo fi in hi.Sections)
            {




                ImgSection imgSection = new ImgSection();

                if (fi.IsVertical)
                {
                    imgSection.Angle += 90;
                }

                if (fi.IsInvert)
                {
                    imgSection.Angle += 180;
                }


                // Корректировка точки вставки секций
                if (fi.IsCorner)
                {
                    imgSection.CoordYCorrection = 0;
                    imgHouse.IsCorner = true;
                }


                // Определение положения дома - вертикальный/горизонтальный
                if (!fi.IsVertical)
                {
                    imgHouse.IsVertical = false;
                }


                int SelectedDownPrev = 0;
                int offset = 0;
                int minX = 0;

                int X = 0;

                bool prev3NL2 = false;

                int FlatIndex = 0;



                foreach (RoomInfo ri in fi.Flats)
                {

                    // Корректировка неверно выданных данных
                    if (ri.Type.Contains("3NL2"))
                    {
                        ri.SelectedIndexTop = 4;
                        ri.SelectedIndexBottom = 0;
                        imgSection.Height = 5;
                    }

                    // Корректировка неверно выданных данных
                    if (ri.Type == "PIK1U_BS_A_10-17_A_2")
                    {
                        ri.SelectedIndexTop = 3;
                    }

                    // Корректировка положения квартиры 2KL2 и 2NM1


                    if (ri.Type.Contains("2KL2") || ri.Type.Contains("2NM1"))
                    {
                        if (prev3NL2)
                        {
                            ri.ImageNameSuffix = "_U";

                            if (ri.Type.Contains("2NM1"))
                                ri.SelectedIndexBottom = 1;
                            if (ri.Type.Contains("2KL2"))
                                ri.SelectedIndexBottom = 2;
                            ri.HorisontalModules = 2;
                            prev3NL2 = false;
                        }
                        else
                        {
                            ri.ImageNameSuffix = "";
                            ri.SelectedIndexBottom = 3;
                            ri.HorisontalModules = 3;
                        }

                    }









                    if (ri.Type.Contains("3NL2"))
                    {
                        // Флаг предыдущей секции 3NL2
                        prev3NL2 = true;

                        // Если после ЛЛУ идёт квартира типа 3NL2, то дом L-ориентирован
                        if (FlatIndex == 1)
                        {
                            imgHouse.LOrientation = true;
                        }



                        // Если встретилась повёрнутая секция, то дом не является горизонтально ориентированным
                        // TODO: неясно, для чего было условие, разобраться

                        //if (fi.IsVertical)
                        //{

                        //}
                    }
                    else
                    {
                        prev3NL2 = false;
                    }
                    FlatIndex++;







                    ImgFlat imgFlat = new ImgFlat(ImagePath, ri.Type + ri.ImageNameSuffix, 0);
                    imgFlat.IsInsPassed = ri.IsInsPassed;
                    imgFlat.ShortName = ri.ShortType;
                    imgFlat.Width = ri.HorisontalModules + 2;
                    imgFlat.Heigth = 6;


                    // Определение точки вставки квартиры
                    if (ri.SelectedIndexTop > 0 && ri.SelectedIndexBottom > 0 && SelectedDownPrev > 0)
                    {
                        offset = SelectedDownPrev;
                        SelectedDownPrev = 0;
                    }
                    else
                    {
                        offset = SelectedDownPrev - ri.SelectedIndexTop;
                        if (ri.SelectedIndexBottom > 0)
                        {
                            SelectedDownPrev = ri.HorisontalModules + ri.NextOffsetX;
                        }
                        else
                        {
                            SelectedDownPrev = ri.SelectedIndexBottom;
                        }
                    }



                    X = X + offset + ri.CurrentOffsetX;

                    if (X < minX)
                        minX = X;

                    imgFlat.CoordX = X;

                    imgSection.Lenght += ri.SelectedIndexBottom;



                    imgSection.ImgFlats.Add(imgFlat);
                }




                foreach (ImgFlat imgFlat in imgSection.ImgFlats)
                {
                    imgFlat.CoordX -= minX;
                }


                imgSection.SectionInfo = fi;
                //string imgName = "Секция" + SectionIndex + ".png";
                //string imgDir = this.ImageOutputPath + "Вариант" + HouseIndex + "\\";

                //imgSection.BmpSectionPath = imgDir + imgName;

                imgSection.BmpSection = generateOneSection(imgSection);

                // Сохранение отдельных секций для отладки
                // SaveImage(imgSection.BmpSection, imgDir, imgName);

                imgHouse.ImgSections.Add(imgSection);
                SectionIndex++;

            }


            Bitmap imageOneHouse = null;

            try
            {
                imageOneHouse = imgHouse.generateOneHouse(imgHouse, ModuleWidth);
            }
            catch
            {
                return null;
            }

            // Удаление отдельных секций из объекта
            imgHouse.ImgSections.Clear();
            GC.Collect();

            imgHouse.BmpImageHouse = imageOneHouse;



            return imgHouse;
        }

        private Bitmap generateOneSection (ImgSection section)
        {
            int width = section.Lenght * ModuleWidth + ModuleWidth / 9;
            int height = section.Height * ModuleWidth;
            int yCorrection = section.CoordYCorrection * ModuleWidth;
            var bitmap = new Bitmap(width, height + 50);

            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.Clear(Color.White);
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;



                foreach (ImgFlat imgFlat in section.ImgFlats)
                {

                    imgFlat.Heigth *= ModuleWidth;
                    imgFlat.Width *= ModuleWidth;

                    // Подгонка угловой секции по высоте
                    if (section.SectionInfo.IsCorner)
                    {
                        imgFlat.Heigth = imgFlat.Heigth - imgFlat.Heigth / 35;
                        yCorrection = imgFlat.Heigth / 40;

                        if (imgFlat.ImgPath.Contains("3NL2"))
                        {
                            imgFlat.Width = imgFlat.Width - ModuleWidth / 6;
                            yCorrection = yCorrection - 12;
                            imgFlat.Heigth = imgFlat.Width + ModuleWidth / 3;
                        }
                    }



                    imgFlat.CoordX = --imgFlat.CoordX * ModuleWidth + ModuleWidth / 18;

                    Image frame;
                    try
                    {

                        frame = Image.FromFile(imgFlat.ImgPath);
                    }
                    catch (Exception ex)

                    {
                        //#if DEBUG
                        //                        //throw new Exception("Не найдено изображение: \n" + imgFlat.ImgPath);


                        System.Windows.Forms.MessageBox.Show("Не найдено изображение: \n" + imgFlat.ImgPath);
                        //#endif

                        string emptyImage = System.IO.Path.GetDirectoryName(imgFlat.ImgPath) + "\\PIK1_empty.png";

                        if (System.IO.File.Exists(emptyImage))
                        {
                            frame = Image.FromFile(emptyImage);
                        }
                        else
                        {
                            frame = new Bitmap(width, height);
                        }
                    }

                    canvas.DrawImage(frame,
                                     new Rectangle(imgFlat.CoordX,
                                                   yCorrection,
                                                   //imgFlat.CoordY * ModuleWidth * 2,
                                                   //-ModuleWidth - ModuleWidth/4,
                                                   imgFlat.Width,
                                                   imgFlat.Heigth + 50),
                                     new Rectangle(0,
                                                   0,
                                                   //imgFlat.CoordY * ModuleWidth,
                                                   //-ModuleWidth,
                                                   frame.Width,
                                                   frame.Height),
                                     GraphicsUnit.Pixel);

                    

                    //canvas.Save();                        
                }

                var format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                foreach (ImgFlat imgFlat in section.ImgFlats.Where(f=>!f.ShortName.StartsWith("BS")))
                {                    
                    using (Font arialFont = new Font("Arial", 20))
                    {
                        canvas.DrawString(section.SectionInfo.IdSection.ToString(), arialFont, Brushes.Black, width * 0.5f, height * 0.5f, format);
                    }                    
                    if (!imgFlat.IsInsPassed)
                    {
                        using (Font arialFont = new Font("Arial", 16))
                        {                            
                            canvas.DrawString(imgFlat.ShortName, arialFont, Brushes.Red, imgFlat.CoordX + imgFlat.Width * 0.5f, yCorrection + imgFlat.Heigth * 0.6f + 50, format);
                        }
                    }
                }

                // canvas.RotateTransform(7);
                canvas.Save();
            }
            try
            {
                switch (section.Angle)
                {
                    case 90:
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;

                    case 180:
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 270:
                        bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    default:

                        break;
                }

            }
            catch (Exception ex)

            {
                return null;
            }

            return bitmap;

        }

        /// <summary>
        /// Сохранение изображения
        /// </summary>
        /// <param name="bitmap">Изображение</param>
        /// <param name="i">Индекс</param>
        private void SaveImage (Bitmap bitmap, string DirImage, string ImageName)
        {

            try
            {


                if (!System.IO.Directory.Exists(DirImage))
                {
                    System.IO.Directory.CreateDirectory(DirImage);
                }

                string imgPath = DirImage + ImageName;

                bitmap.Save(imgPath,
                System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {

            }
        }
    }    
}
