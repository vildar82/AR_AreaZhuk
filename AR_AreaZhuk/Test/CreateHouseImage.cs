using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Test
{
    static class CreateHouseImage
    {
        static CreateHouseImage()
        {   
            var imgs = Directory.GetFiles(@"c:\work\!Acad_РГ\АР\ЖУКИ\Инсоляция\Тест\");
            foreach (var item in imgs)
            {
                File.Delete(item);
            }
        }

        static long countFile = 0;        
        public static void TestCreateImage (HouseInfo house)
        {
            countFile++;

            // Лог дома
            LogHouse(house, countFile);

            GeneralObject go = new GeneralObject();
            go.SpotInf = house.SpotInf;
            //double area = GetTotalArea(house);            
            go.Houses.Add(house);
            //go.SpotInf.RealArea = area;
            go.GUID = Guid.NewGuid().ToString();
            // ob.Add(go);

            string spotName = house.Sections.First().SpotOwner.Split('|')[0];
            string steps = string.Join(".", house.Sections.Select(s=>s.CountStep.ToString()));
            string ids = string.Join("_", house.Sections.Select(s => s.IdSection.ToString()));
            string name = $"{ids}_{steps}_{spotName}_{countFile}.png";            

            string imagePath = @"c:\work\!Acad_РГ\АР\ЖУКИ\Инсоляция\Тест\" + name;

            string sourceImgFlats = @"z:\Revit_server\13. Settings\02_RoomManager\00_PNG_ПИК1\";
            string ExcelDataPath = @"c:\work\!Acad_РГ\АР\ЖУКИ\Инсоляция\БД_Параметрические данные квартир ПИК1 -Не трогать.xlsx";

            BeetlyVisualisation.ImageCombiner imgComb = new BeetlyVisualisation.ImageCombiner(go, ExcelDataPath, sourceImgFlats, 72);
            var img = imgComb.generateGeneralObject();
            img.Save(imagePath, ImageFormat.Png);
        }

        private static void LogHouse (HouseInfo house, long countFile)
        {
            StringBuilder logHouse = new StringBuilder();
            logHouse.Append("HouseCount=").Append(countFile.ToString()).AppendLine();
            foreach (var section in house.Sections)
            {                
                logHouse.Append("ID=").Append(section.IdSection.ToString()).Append(", IsInvert=").Append(section.IsInvert).AppendLine();
                foreach (var flat in section.Flats)
                {
                    logHouse.Append("Flat=").Append(flat.Type).Append(", isInsPassed=").Append(flat.IsInsPassed).AppendLine();
                }
            }
            Trace.Write(logHouse);
        }
    }
}
