using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_AreaZhuk.Insolation;
using AR_AreaZhuk.Scheme;
using AR_AreaZhuk.Scheme.Cutting;
using AR_Zhuk_DataModel;
using NUnit.Framework;

namespace AR_AreaZhukTests.Scheme.Cutting
{
    class TestCutting
    {
        [Test]
        public void TestGetProjectSpot ()
        {
            string insolationFile = @"c:\Задание по инсоляции ПИК1.xlsx";
            List<HouseOptions> options = new List<HouseOptions>() {
                 new HouseOptions("P1", 15, 25, new List<bool> { true, false, false, false, false }),
                 new HouseOptions("P2", 15, 25, new List<bool> { true, false, false, false, false })
            };

            ProjectSpot projectSpot = new ProjectSpot(options);            
            projectSpot.ReadScheme(insolationFile);            
            List<HouseSpot> houseSpots = projectSpot.HouseSpots;

            List<List<HouseInfo>> totalObject = new List<List<HouseInfo>>();
            foreach (var item in houseSpots)
            {
                ICutting cutting = CuttingFactory.Create(item);
                List<Section> houses = cutting.Cut();

                HouseInfo hi = new HouseInfo();
                hi.SectionsBySize = houses;
            }           
            Assert.AreEqual(houseSpots.Count, 2);
        }
    }
}
