﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NHLPredictorASP.Classes
{
    #region Objects needed for deserialization of the JSON teams/rosters coming from the NHL's API
    public class Roster2
    {
        public Person Person { get; set; }
        public Position Position { get; set; }
        public string Name => Person.FullName;
        public string Id => Person.Id;
        public string Code => Position.Code;

        public override string ToString() => Person.FullName;
    }

    public class RosterList
    {
        public List<Roster2> Roster { get; set; }
    }
    public class Team
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public RosterList Roster { get; set; }

        public List<Roster2> PersonList
        {
            get => Roster.Roster;
            set => Roster.Roster = value;
        }

        public override string ToString() => $"{Name} ({Abbreviation})";
    }

    #endregion

    #region TeamCollection class used to contain all teams fetched from the API and display them in the GUI
    public class TeamCollection : List<Team>
    {
        public TeamCollection()
        {
            foreach (var t in ApiLoader.LoadTeams().OrderBy(t => t.Name).ToList())
            {
                Add(t);
            }
        }

    }
    #endregion
}
