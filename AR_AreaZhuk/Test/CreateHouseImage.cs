using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;

namespace AR_AreaZhuk.Test
{
    static class CreateHouseImage
    {
        public static void TestCreateImage (HouseInfo house)
        {
            GeneralObject go = new GeneralObject();
            go.SpotInf = house.SpotInf;
            //double area = GetTotalArea(house);            
            go.Houses.Add(house);
            //go.SpotInf.RealArea = area;
            go.GUID = Guid.NewGuid().ToString();
            // ob.Add(go);

            string spotName = house.Sections.First().SpotOwner.Split('|')[0];
            string steps = string.Join(".", house.Sections.Select(s=>s.CountStep.ToString()));
            string name = $"{spotName}_{steps}_{go.GUID}.png";

            string imagePath = @"c:\work\!Acad_РГ\АР\ЖУКИ\Инсоляция\Тест\" + name;

            string sourceImgFlats = @"z:\Revit_server\13. Settings\02_RoomManager\00_PNG_ПИК1\";
            string ExcelDataPath = @"c:\work\!Acad_РГ\АР\ЖУКИ\Инсоляция\БД_Параметрические данные квартир ПИК1 -Не трогать.xlsx";

            BeetlyVisualisation.ImageCombiner imgComb = new BeetlyVisualisation.ImageCombiner(go, ExcelDataPath, sourceImgFlats, 72);
            var img = imgComb.generateGeneralObject();
            img.Save(imagePath, ImageFormat.Png);
        }
    }
}
