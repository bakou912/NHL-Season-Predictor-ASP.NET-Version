using NUnit.Framework;
using NHLPredictorASP.Classes;
using System.Collections.Generic;

namespace NHLPredictorASP.Unit_Tests
{
    public class PlayerTest
    {
        Player referencePlayer;
        List<Season> career;

        [OneTimeSetUp]
        public void Before()
        {
            career = new List<Season>
            {
                new Season(13, 45, 52),
                new Season(73, 45, 52),
                new Season(13, 16, 52)
            };
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
            List<Season> insufficientQuantity = new List<Season>
            {
                new Season(0, 0, 50),
                new Season(0, 0, 50)
            };
            Player testPlayer = new Player(insufficientQuantity);
            Assert.IsFalse(testPlayer.HasSufficientInfo);

            List<Season> sufficientSeason = insufficientQuantity;
            sufficientSeason.Add(new Season(0, 0, 50));
            testPlayer = new Player(sufficientSeason);
            Assert.IsTrue(testPlayer.HasSufficientInfo);

            List<Season> insufficientGameAverage = new List<Season>
            {
                new Season(0, 0, 50),
                new Season(0, 0, 30),
                new Season(0, 0, 30)
            };
            testPlayer = new Player(insufficientGameAverage);
            Assert.IsFalse(testPlayer.HasSufficientInfo);
        }

        //Equals method should compare two players correctly
        //If Equals works properly, then GetHashCode as well
        [Test]
        public void TestEquals()
        {
            Player playerTrue = referencePlayer;
            Assert.IsTrue(playerTrue.Equals(referencePlayer));

            List<Season> falseCareer = new List<Season>
            {
                new Season(13, 45, 52),
                new Season(73, 45, 51),
                new Season(13, 16, 52)
            };
            Player playerFalse = new Player(falseCareer);
            Assert.IsFalse(playerFalse.Equals(referencePlayer));
        }
    }
}