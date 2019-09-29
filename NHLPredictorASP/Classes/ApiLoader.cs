using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace NHLPredictorASP.Classes
{
    /// <summary>
    /// Class needed for deserialization of the team list
    /// </summary>
    public class TeamArrayWrapper
    {
        public List<Team> Teams { get; set; }
        //Default and only constructor
    }
    public static class ApiLoader
    {
        /// <summary>The base UR for the NHL's apiL</summary>
        private static readonly string BaseUrl = "https://statsapi.web.nhl.com/api/v1";

        /// <summary>The rest client</summary>
        static readonly RestClient RestClient = new RestClient(BaseUrl);

        static readonly RestRequest RestRequest = new RestRequest(Method.GET);

        private static void SetRestRequest(string resource)
        {
            RestRequest.Resource = resource;
        }

        /// <summary>Fetching and deserializing all active teams</summary>
        /// <returns>The complete list of active teams (with their roster)</returns>
        public static List<Team> LoadTeams()
        {
            var teamList = new List<Team>();

            SetRestRequest("teams/?expand=team.roster");

            var response = RestClient.Execute(RestRequest);

            var teams = JsonConvert.DeserializeObject<TeamArrayWrapper>(response.Content)?.Teams;

            //Stopping the process if the response form the RestClient was null
            if (teams == null)
            {
                return null;
            }

            foreach (var team in teams)
            {
                //Skipping inactive teams
                if (!team.Active)
                {
                    continue;
                }

                //Deserializing the team's roster (collection of Roster2 objects)
                team.PersonList = new List<StatsRoster>(team.PersonList.OrderBy(r => r.Person.FullName));

                //Removing all goaltenders from the roster
                while (team.PersonList.Any(p => p.Code.Equals("G")))
                {
                    team.PersonList.Remove(team.PersonList.First(p => p.Code.Equals("G")));
                }

                teamList.Add(team);
            }

            return teamList;
        }

        /// <summary>
        /// Fetching player object by deserializing StatsList of all seasons in career
        /// </summary>
        /// <param name="year">year to estimate</param>
        /// <param name="id">player's ID</param>
        /// <returns>Player built from NHL's stats api</returns>
        public static Player LoadPlayer(int year, string id)
        {
            var seasonYears = $"{year - 1}{year}";
            var seasonList = new List<Season>();

            SetRestRequest("people/" + id + "/stats?stats=yearByYear");

            var response = RestClient.Execute(RestRequest);

            var statsList = JsonConvert.DeserializeObject<StatsList>(response.Content);

            string lastYear = "";
            foreach (var split in statsList.Stats[0].Splits)
            {
                if (split.League.Id != 133)
                {
                    continue;
                }

                var newSeason = new Season(split);
                if (newSeason.SeasonYears.CompareTo(seasonYears) > 0)
                {
                    break;
                }

                if (lastYear == newSeason.SeasonYears)
                {
                    MergeSeasons(newSeason, seasonList[seasonList.Count - 1]);
                    seasonList.RemoveAt(seasonList.Count - 1);
                }

                seasonList.Add(newSeason);
                lastYear = newSeason.SeasonYears;
            }

            return new Player(seasonList);
        }

        public static void MergeSeasons(Season initial, Season toMerge)
        {
            initial.Assists += toMerge.Assists;
            initial.Goals += toMerge.Goals;
            initial.GamesPlayed += toMerge.GamesPlayed;
        }
    }
}