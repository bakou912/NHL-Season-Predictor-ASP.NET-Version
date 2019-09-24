using NUnit.Framework;
using NHLPredictorASP.Classes;
using System.Collections.Generic;

namespace NHLPredictorASP.Unit_Tests
{
    public class Player_test
    {
        Player referencePlayer;
        List<Season> career;

        [OneTimeSetUp]
        public void Before()
        {
            career = new List<Season>();
            career.Add(new Season(13, 45, 52));
            career.Add(new Season(73, 45, 52));
            career.Add(new Season(13, 16, 52));
            referencePlayer = new Player(career);
        }

        /// <summary>
        /// HasSufficientInfo should be false when:
        /// SeasonList.count < 3, GameAverage < 50
        /// HasSufficientInfo should be true otherwise:
        /// </summary>
        [Test]
        public void TestHasSufficientInfo()
        {
            List<Season> insufficientQuantity = new List<Season>();
            insufficientQuantity.Add(new Season(0, 0, 50));
            insufficientQuantity.Add(new Season(0, 0, 50));
            Player testPlayer = new Player(insufficientQuantity);
            Assert.IsFalse(testPlayer.HasSufficientInfo);

            List<Season> sufficientSeason = insufficientQuantity;
            sufficientSeason.Add(new Season(0, 0, 50));
            testPlayer = new Player(sufficientSeason);
            Assert.IsTrue(testPlayer.HasSufficientInfo);

            List<Season> insufficientGameAverage = new List<Season>();
            insufficientGameAverage.Add(new Season(0, 0, 50));
            insufficientGameAverage.Add(new Season(0, 0, 30));
            insufficientGameAverage.Add(new Season(0, 0, 30));
            testPlayer = new Player(insufficientGameAverage);
            Assert.IsFalse(testPlayer.HasSufficientInfo);
        }

        //Equals method should compare two players correctly
        //If Equals works properly, then GetHashCode as well
        [Test]
        public void TestEquals()
        {
            Player playerTrue = referencePlayer.Duplicate();
            Assert.IsTrue(playerTrue.Equals(referencePlayer));

            List<Season> falseCareer = new List<Season>();
            falseCareer.Add(new Season(13, 45, 52));
            falseCareer.Add(new Season(73, 45, 51));
            falseCareer.Add(new Season(13, 16, 52));
            Player playerFalse = new Player(falseCareer);
            Assert.IsFalse(playerFalse.Equals(referencePlayer));
        }

        //Duplicate method should consider copiedPlayer equal to testPlayer but they should not be the same object
        [Test]
        public void TestDuplicate()
        {
            Player toCopy = new Player(career);
            Player copiedPlayer = toCopy.Duplicate();
            Assert.AreEqual(copiedPlayer, toCopy);
            Assert.AreNotSame(copiedPlayer, toCopy);
        }
    }
}