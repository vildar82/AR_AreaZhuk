using System.Collections.Generic;
using NUnit.Framework;

namespace AR_AreaZhuk.Insolation.Tests
{
    [TestFixture()]
    public class LightingStringParserTests
    {
        [Test()]
        public void GetLightingsTest ()
        {
            string lightingstringFlat = "1-2";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { 1, 2 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }
    }
}