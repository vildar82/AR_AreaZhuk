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
        [OneTimeSetUp]
        public void init ()
        {

        }

        [SetUp]
        public void setup()
        {
            
        }

        [Test]
        public void TestGetProjectSpot()
        {
            string insolationFile = @"c:\Задание по инсоляции ПИК1.xlsx";
            ProjectSpot projectSpot = new ProjectSpot();

            // Чтение файла схемы объекта
            projectSpot.ReadScheme(insolationFile);
            // Получение домов (пятен)
            List<HouseSpot> houseSpots = projectSpot.HouseSpots;

            Assert.AreEqual(houseSpots.Count, 2);
        }
    }
}
