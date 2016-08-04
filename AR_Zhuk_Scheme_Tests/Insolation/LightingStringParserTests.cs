using NUnit.Framework;
using AR_AreaZhuk.Insolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_Schema.Insolation;

namespace AR_AreaZhuk.Insolation.Tests
{
    [TestFixture()]
    public class LightingStringParserTests
    {
        Side flatEndSideExpected;
        Side flatSideActual;
        bool isTop;

        List<int> expectedLightings;
        List<int> expectedSideLightings;

        [SetUp]
        public void Setup()
        {            
        }

        private void Expect (List<int> actualLightings, List<int> actualSideLightings)
        {
            CollectionAssert.AreEqual(expectedLightings, actualLightings);
            CollectionAssert.AreEqual(expectedSideLightings, actualSideLightings);
            Assert.AreEqual(flatEndSideExpected, flatSideActual);
        }

        [Test]
        public void GetLightingsSimple1Test ()
        {
            string lightingstringFlat = "1";
            flatSideActual = Side.None;
            
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);

            List<int> actualLightings = new List<int> { 1 };

            Expect(actualLightings, new List<int> ());
        }

        [Test]
        public void GetLightingsSimple2Test ()
        {
            string lightingstringFlat = "1-3";
            flatSideActual = Side.None;
            List<int> actualLightings = new List<int> { 1, 2, 3 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, new List<int>());            
        }

        [Test]
        public void GetLightingsHard1Test ()
        {
            string lightingstringFlat = "1|2-3";
            flatSideActual = Side.None;
            List<int> actualLightings = new List<int> { -1, -2, 3 };
            
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, new List<int>());
        }

        [Test]
        public void GetLightingsHard2Test ()
        {
            string lightingstringFlat = "1-2|3";
            flatSideActual = Side.None;
            List<int> actualLightings = new List<int> { 1, -2, -3 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, new List<int>());
        }

        [Test]
        public void GetLightingsHard3Test ()
        {
            string lightingstringFlat = "1-2|3-4";
            flatSideActual = Side.None;
            List<int> actualLightings = new List<int> { 1, -2, -3, 4 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, new List<int>());
        }

        [Test]
        public void GetLightingsSide1Test ()
        {
            string lightingstringFlat = "B,1|2";            
            flatSideActual = Side.Right;
            isTop = true;
            List<int> actualLightings = new List<int> { -1, -2 };
            List<int> actualSideLightings = new List<int> { 1 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }        

        [Test]
        public void GetLightingsSide2Test ()
        {
            string lightingstringFlat = "2|3,B";            
            flatSideActual = Side.Left;
            isTop = true;
            List<int> actualLightings = new List<int> { -2, -3 };
            List<int> actualSideLightings = new List<int> { 1 };
            
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }

        [Test]
        public void GetLightingsSide3Test ()
        {
            string lightingstringFlat = "B,1|2";            
            flatSideActual = Side.Left;
            isTop = false;
            List<int> actualLightings = new List<int> { -1, -2 };
            List<int> actualSideLightings = new List<int> { 1 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }

        [Test]
        public void GetLightingsSide4Test ()
        {
            string lightingstringFlat = "B|1";            
            flatSideActual = Side.Right;
            isTop = true;
            List<int> actualLightings = new List<int> { -1 };
            List<int> actualSideLightings = new List<int> { -1 };
            
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);

            Expect(actualLightings, actualSideLightings);
        }

        [Test]
        public void GetLightingsSide5Test ()
        {
            string lightingstringFlat = "1|B";            
            flatSideActual = Side.Right;
            isTop = false;
            List<int> actualLightings = new List<int> { -1 };
            List<int> actualSideLightings = new List<int> { -1 };
                        
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }

        [Test]
        public void GetLightingsSideHard1Test ()
        {
            string lightingstringFlat = "1-2|B1,B2";            
            flatSideActual = Side.Right;
            isTop = false;
            List<int> actualLightings = new List<int> { 1, -2 };
            List<int> actualSideLightings = new List<int> { -1, 2 };

            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }

        [Test]
        public void GetLightingsSideHard2Test ()
        {
            string lightingstringFlat = "1|2,3|B";            
            flatSideActual = Side.Right;
            isTop = false;
            List<int> actualLightings = new List<int> { -1, -2, -3 };
            List<int> actualSideLightings = new List<int> { -1 };
            
            expectedLightings = LightingStringParser.GetLightings(lightingstringFlat, out expectedSideLightings, isTop, out flatEndSideExpected);            

            Expect(actualLightings, actualSideLightings);
        }
    }
}