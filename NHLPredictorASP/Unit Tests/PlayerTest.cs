#region Header

// Author: Tommy Andrews
// File: PlayerTest.cs
// Project: NHLPredictorASP
// Created: 09/29/2019

#endregion

using System.Collections.Generic;
using NHLPredictorASP.Classes.Entities;
using NUnit.Framework;

namespace NHLPredictorASP.Unit_Tests
{
    public class PlayerTest
    {
        private List<Season> career;
        private Player referencePlayer;

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

        [Test]
        public void TestHasSufficientInfo()
        {
            var insufficientQuantity = new List<Season>
            {
                new Season(0, 0, 50),
                new Season(0, 0, 50)
            };
            var testPlayer = new Player(insufficientQuantity);
            Assert.IsFalse(testPlayer.HasSufficientInfo);

            var sufficientSeason = insufficientQuantity;
            sufficientSeason.Add(new Season(0, 0, 50));
            testPlayer = new Player(sufficientSeason);
            Assert.IsTrue(testPlayer.HasSufficientInfo);

            var insufficientGameAverage = new List<Season>
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
            var playerTrue = referencePlayer;
            Assert.IsTrue(playerTrue.Equals(referencePlayer));

            var falseCareer = new List<Season>
            {
                new Season(13, 45, 52),
                new Season(73, 45, 51),
                new Season(13, 16, 52)
            };
            var playerFalse = new Player(falseCareer);
            Assert.IsFalse(playerFalse.Equals(referencePlayer));
        }
    }
}