using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_DataModel;
using AR_Zhuk_InsSchema.Scheme;
using AR_Zhuk_InsSchema.Scheme.Cutting;
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
            var sp = TestProjectSpot.GetSpotInformation();

            ProjectSpot projectSpot = new ProjectSpot(options, sp);            
            projectSpot.ReadScheme(insolationFile);            
            List<HouseSpot> houseSpots = projectSpot.HouseSpots;

            List<List<HouseInfo>> totalObject = new List<List<HouseInfo>>();
            foreach (var item in houseSpots)
            {
                ICutting cutting = CuttingFactory.Create(item, sp);
                var houses = cutting.Cut();
                totalObject.Add(houses);
            }           
            Assert.AreEqual(houseSpots.Count, 2);
        }
    }
}
