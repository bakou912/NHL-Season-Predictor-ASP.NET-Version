#region Header

// Author: Tommy Andrews
// File: SeasonTest.cs
// Project: NHLPredictorASP
// Created: 09/29/2019

#endregion

using NHLPredictorASP.Classes.Entities;
using NUnit.Framework;

namespace NHLPredictorASP.Unit_Tests
{
    [TestFixture]
    public class SeasonTest
    {
        private readonly Season _testSeason = new Season(15, 15, 15);

        [OneTimeSetUp]
        public void Before()
        {
        }

        //CalculatePoints should add the points correctly
        //(15 assists + 15 goals = 30 points)
        [Test]
        public void TestCalculatePoints()
        {
            Assert.AreEqual(_testSeason.Points, 30);
        }

        //Equals method should compare two seasons correctly
        //If Equals works properly, then GetHashCode as well
        [Test]
        public void TestEquals()
        {
            var seasonFalse = new Season(1, 2, 3);
            var seasonTrue = new Season(15, 15, 15);

            Assert.IsTrue(seasonTrue.Equals(_testSeason));
            Assert.IsTrue(_testSeason.Equals(seasonTrue));
            Assert.IsFalse(_testSeason.Equals(seasonFalse));
            Assert.IsFalse(seasonFalse.Equals(_testSeason));
        }
    }
}