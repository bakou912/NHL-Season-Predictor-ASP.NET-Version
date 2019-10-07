#region Header

// Author: Tommy Andrews
// File: ApiLoader.cs
// Project: NHLPredictorASP
// Created: 09/30/2019

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using NHLPredictorASP.Classes.Deserialization;
using NHLPredictorASP.Classes.Entities;
using RestSharp;
using RestSharp.Extensions;

namespace NHLPredictorASP.Classes.Utility
{
    /// <summary>
    ///     Class needed for deserialization of the team list
    /// </summary>
    public class TeamArrayWrapper
    {
        public List<Team> Teams { get; set; }
        //Default and only constructor
    }

    public static class ApiLoader
    {
        /// <summary>The base UR for the NHL's apiL</summary>
        private const string BaseUrl = "https://statsapi.web.nhl.com/api/v1";

        /// <summary>The rest client</summary>
        private static readonly RestClient RestClient = new RestClient(BaseUrl);

        private static readonly RestRequest RestRequest = new RestRequest(Method.GET);

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

            var response = RestClient.Execute(RestRequest).ToAsyncResponse<TeamArrayWrapper>();

            var teams = new RestSharp.Deserializers.JsonDeserializer().Deserialize<TeamArrayWrapper>(response)?.Teams;


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
                team.PersonList.ForEach(p => p.Person.TeamAbv = team.Abbreviation);

                //Removing all goaltenders from the roster
                team.PersonList.RemoveAll(p => p.Code.Equals("G"));

                teamList.Add(team);
            }

            return teamList;
        }

        /// <summary>
        ///     Fetching player object by deserializing StatsList of all seasons in career
        /// </summary>
        /// <param name="year">year to estimate</param>
        /// <param name="id">player's ID</param>
        /// <returns>Player built from NHL's stats api</returns>
        public static Player LoadPlayer(int year, string id)
        {
            var seasonYears = $"{year - 1}{year}";
            var seasonList = new List<Season>();

            SetRestRequest("people/" + id + "/stats?stats=yearByYear");

            var response = RestClient.Execute(RestRequest).ToAsyncResponse<StatsList>();

            var statsList = new RestSharp.Deserializers.JsonDeserializer().Deserialize<StatsList>(response);

            var lastYear = "";
            foreach (var split in statsList.Stats[0].Splits)
            {
                if (split.League.Id != 133)
                {
                    continue;
                }

                var newSeason = new Season(split);
                if (string.Compare(newSeason.SeasonYears, seasonYears, StringComparison.Ordinal) > 0)
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