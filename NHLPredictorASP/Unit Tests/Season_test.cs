using NUnit.Framework;
using NHLPredictorASP.Classes;

namespace NHLPredictorASP.Unit_Tests
{
    [TestFixture]
    public class Season_test
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

        //Duplicate method should consider copiedSeason equal to testSeason
        [Test]
        public void TestDuplicate()
        {
            Season toCopy = new Season(15,15,15);
            Season copiedSeason = toCopy.Duplicate();
            Assert.AreEqual(copiedSeason, testSeason);
        }

        //Duplicate should create a deep copy of a season compared
        [Test]
        public void TestDuplicateDeepness()
        {
            Season deepCopy = testSeason.Duplicate();

            Assert.AreNotSame(deepCopy, testSeason);
        }
    }
}