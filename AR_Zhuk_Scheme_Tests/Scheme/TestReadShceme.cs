using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_Schema;
using AR_Zhuk_Schema.Scheme;
using NUnit.Framework;

namespace AR_Zhuk_Scheme_Tests.Scheme
{
    [TestFixture]
    class TestReadScheme
    {
        [Test]
        public void TestRead ()
        {
            string insolationFile = @"c:\work\test\АР\ЖУКИ\Задание по инсоляции ПИК1.xlsx";
            List<HouseOptions> options = new List<HouseOptions>() {
                 new HouseOptions("P1", 15, 25, new List<bool> { true, false, false, false, false }),
                 new HouseOptions("P2", 15, 25, new List<bool> { true, false, false, false, false })
            };

            SpotInfo sp = TestProjectScheme.GetSpotInformation();
            ProjectScheme projectSpot = new ProjectScheme(options, sp);

            // Чтение файла схемы объекта
            projectSpot.ReadScheme(insolationFile);
            // Получение домов (пятен)
            List<HouseSpot> houseSpots = projectSpot.HouseSpots;            

            Assert.AreEqual(houseSpots.Count, 2);
        }
    }
}
