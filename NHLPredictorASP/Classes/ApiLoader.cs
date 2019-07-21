using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace NHLPredictorASP.Classes
{
    /// <summary>
    /// Class needed for deserialization
    /// </summary>
    public class TeamArrayWrapper
    {
        public Team[] Teams { get; set; }
        //Default and only constructor
    }
    public static class ApiLoader
    {
        /// <summary>The base UR for the NHL's apiL</summary>
        private static readonly string BaseUrl = "https://statsapi.web.nhl.com/api/v1";

        /// <summary>The rest client</summary>
        private static readonly RestClient RestClient = new RestClient(BaseUrl);

        /// <summary>Fetching and deserializing all active teams</summary>
        /// <returns>The complete list of active teams (with their roster)</returns>
        public static List<Team> LoadTeams()
        {
            var teamList = new List<Team>();

            var request = new RestRequest()
            {
                Method = Method.GET,
                Resource = "teams/?expand=team.roster"
             };

            var response = RestClient.Execute(request);

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
                team.PersonList = new List<Roster2>(team.PersonList.OrderBy(r => r.Person.FullName));

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
        /// Fetching and deserializing player object corresponding to player ID
        /// </summary>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The player </returns>
        public static Player LoadPlayer(int year, string id)
        {

            var nullSeasonCount = 0;
            var seasonList = new List<Season>();

            while (nullSeasonCount <= 4)
            {
                var newSeason = GetSeason(year, id);

                if (newSeason != null)
                {
                    /*If the season is valid - not null - the null season counter is reset, because
                     the season reading process should only be stopped when there are more than 4 consecutive inactive seasons*/
                    nullSeasonCount = 0;

                    //Adding the season to the season list
                    seasonList.Add(newSeason);
                }
                else
                {
                    //When the response is invalid, the null counter is incremented
                    nullSeasonCount++;
                }

                //Decrementing the season year for the next iteration
                year--;
            }
            return new Player(seasonList);
        }

        /// <summary>
        /// Fetches the season of a player for a specified year 
        /// </summary>
        /// <param name="year">Season year</param>
        /// <param name="id">Player ID in the NHL's database</param>
        /// <returns>The season for the year for a player with the specified ID</returns>
        public static Season GetSeason(int year, string id)
        {
            //Base URL for the wanted player
            var baseResource = "people/" + id + "/stats?stats=statsSingleSeason&season=" + $"{year - 1}{year}";

            //Initializing the Rest request
            var restRequest = new RestRequest()
            {
                Method = Method.GET,
                //Setting the base URL for the request
                Resource = baseResource
            };

            //Storing rest request's execution response
            var response = RestClient.Execute(restRequest);

            try
            {
                return JsonConvert.DeserializeObject<StatsList>(response.Content).Season;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}