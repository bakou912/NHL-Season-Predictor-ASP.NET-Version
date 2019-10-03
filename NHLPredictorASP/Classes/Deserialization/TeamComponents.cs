#region Header

// Author: Tommy Andrews
// File: TeamComponents.cs
// Project: NHLPredictorASP
// Created: 09/30/2019

#endregion

using System.Collections.Generic;
using System.Linq;
using NHLPredictorASP.Classes.Utility;

namespace NHLPredictorASP.Classes.Deserialization
{
    #region Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API

    public class StatsRoster
    {
        public Person Person { get; set; }
        public Position Position { get; set; }
        public string Name => Person.FullName;
        public string Id => Person.Id;
        public string Code => Position.Code;

        public override string ToString()
        {
            return Person.FullName;
        }
    }

    public class RosterList
    {
        public List<StatsRoster> Roster { get; set; }
    }

    public class Team
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public RosterList Roster { get; set; }

        public List<StatsRoster> PersonList
        {
            get => Roster.Roster;
            set => Roster.Roster = value;
        }

        public override string ToString()
        {
            return $"{Name} ({Abbreviation})";
        }
    }

    #endregion Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API

    #region TeamList class used to contain all teams fetched from the API and use as a wrapper

    public class TeamList : List<Team>
    {
        public TeamList()
        {
            foreach (var t in ApiLoader.LoadTeams().OrderBy(t => t.Name).ToList())
            {
                Add(t);
            }
        }
    }

    #endregion TeamList class used to contain all teams fetched from the API and use as a wrapper
}