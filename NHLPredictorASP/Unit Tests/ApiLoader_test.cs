using NUnit.Framework;
using NHLPredictorASP.Classes;

namespace NHLPredictorASP.Unit_Tests
{
    [TestFixture]
    public class ApiLoader_Test
    {

        const int NB_TEAMS = 31, NB_SEASONS = 12;
        string ovechkinId;

        [OneTimeSetUp]
        public void Before()
        {
            ovechkinId = "8471214";
        }

        //LoadTeams method should load the right number of teams
        [Test]
        public void TestTeamNumber()
        {
            Assert.AreEqual(ApiLoader.LoadTeams().Count, NB_TEAMS);
        }

        //LoadPlayer method should load the right player with the passed ID
        [Test]
        public void TestLoadPlayer()
        {
            Assert.AreEqual(NB_SEASONS, ApiLoader.LoadPlayer(2019, ovechkinId).SeasonList.Count);
        }
    }
}