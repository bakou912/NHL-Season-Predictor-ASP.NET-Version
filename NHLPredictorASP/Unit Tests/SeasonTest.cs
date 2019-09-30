using NUnit.Framework;
using NHLPredictorASP.Classes;

namespace NHLPredictorASP.Unit_Tests
{
    [TestFixture]
    public class SeasonTest
    {
        readonly Season testSeason = new Season(15, 15, 15);
        [OneTimeSetUp]
        public void Before()
        {
           
        }

        //CalculatePoints should add the points correctly 
        //(15 assists + 15 goals = 30 points)
        [Test]
        public void TestCalculatePoints()
        {
            Assert.AreEqual(testSeason.Points, 30);
        }

        //Equals method should compare two seasons correctly
        //If Equals works properly, then GetHashCode as well
        [Test]
        public void TestEquals()
        {
            Season seasonFalse = new Season(1,2,3);
            Season seasonTrue = new Season(15, 15, 15);

            Assert.IsTrue(seasonTrue.Equals(testSeason));
            Assert.IsTrue(testSeason.Equals(seasonTrue));
            Assert.IsFalse(testSeason.Equals(seasonFalse));
            Assert.IsFalse(seasonFalse.Equals(testSeason));
        }
    }
}