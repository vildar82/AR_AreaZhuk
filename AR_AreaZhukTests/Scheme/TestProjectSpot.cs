using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.Scheme;
using NUnit.Framework;

namespace AR_AreaZhukTests.Scheme
{
    [TestFixture]
    class TestProjectSpot
    {
        [Test]
        public void TestGetProjectSpot()
        {
            string insolationFile = @"c:\Задание по инсоляции ПИК1.xlsx";
            List<HouseOptions> options = new List<HouseOptions>() {
                 new HouseOptions("P1", 15, 25, new List<bool> { true, false, false, false, false })
            };


            ProjectSpot projectSpot = new ProjectSpot(options);

            // Чтение файла схемы объекта
            projectSpot.ReadScheme(insolationFile);
            // Получение домов (пятен)
            List<HouseSpot> houseSpots = projectSpot.HouseSpots;

            Assert.AreEqual(houseSpots.Count, 2);
        }
    }
}
