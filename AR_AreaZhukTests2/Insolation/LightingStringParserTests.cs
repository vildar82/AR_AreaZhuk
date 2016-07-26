using NUnit.Framework;
using AR_AreaZhuk.Insolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AR_AreaZhuk.Insolation.Tests
{
    [TestFixture()]
    public class LightingStringParserTests
    {
        [Test()]
        public void GetLightingsSimple1Test ()
        {
            string lightingstringFlat = "1";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { 1 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }

        [Test()]
        public void GetLightingsSimple2Test ()
        {
            string lightingstringFlat = "1-3";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { 1, 2,3 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }

        [Test()]
        public void GetLightingsHard1Test ()
        {
            string lightingstringFlat = "1|2-3";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { -1, -2, 3 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }

        [Test()]
        public void GetLightingsHard2Test ()
        {
            string lightingstringFlat = "1-2|3";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { 1, -2, -3 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }

        [Test()]
        public void GetLightingsHard3Test ()
        {
            string lightingstringFlat = "1-2|3-4";

            List<int> sideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out sideLightings);

            List<int> actualLightings = new List<int> { 1, -2, -3, 4 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
        }

        [Test()]
        public void GetLightingsSide1Test ()
        {
            string lightingstringFlat = "B,1|2";

            List<int> expectedSideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings);

            List<int> actualLightings = new List<int> { -1, -2 };
            List<int> actualSideLightings = new List<int> { 1 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
            CollectionAssert.AreEqual(expectedSideLightings, actualSideLightings);
        }

        [Test()]
        public void GetLightingsSide2Test ()
        {
            string lightingstringFlat = "2|3,B";

            List<int> expectedSideLightings;
            var expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings);

            List<int> actualLightings = new List<int> { -2, -3 };
            List<int> actualSideLightings = new List<int> { 1 };

            CollectionAssert.AreEqual(expectedLightings, actualLightings);
            CollectionAssert.AreEqual(expectedSideLightings, actualSideLightings);
        }
    }
}