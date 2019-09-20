using System;
using System.Collections.Generic;

namespace NHLPredictorASP.Classes
{
    #region Player class used to represent an NHL player
    public class Player : Person
    {
        //The list of seasons in this Player's career
        public List<Season> SeasonList { get; }
        //The expected season for this Player's next year
        public Season ExpectedSeason { get; }
        //Represents the sufficiency of this Player's stats in his career
        public bool HasSufficientInfo { get; set; }

        /// <summary>
        /// Adds season s to the SeasonList
        /// </summary>
        /// <param name="s">Season to add</param>
        public void Add(Season s) => SeasonList.Add(s);

        /// <summary>
        /// Removes the season at index i in SeasonList
        /// </summary>
        /// <param name="i">Index indicating which season to remove</param>
        public void Remove(int i) => SeasonList.RemoveAt(i);

        public Player(List<Season> seasonsToDuplicate)
        {
            SeasonList = new List<Season>();
            ExpectedSeason = new Season();
            FullName = "";
            HasSufficientInfo = false;

            foreach (var s in seasonsToDuplicate)
            {
                Add(Season.Duplicate(s));
            }
            try
            {
                SeasonCalculator.CalculateExpectedSeason(this);
            }
            catch (Exception)
            {
                HasSufficientInfo = false;
            }
            
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="p">Player to copy</param>
        /// <param name="name">The wanted name for this Player</param>
        /// <param name="id">The wanted id for this Player</param>
        public Player(Player p, string name, string id) : this(p.SeasonList)
        {
            Id = id;
            FullName = name;
        }

        /// <summary>
        /// Complete player duplicator: calls the copy constructor
        /// </summary>
        /// <param name="p">Player to Duplicate</param>
        /// <returns>The duplication of the player passed as a parameter</returns>
        public static Player Duplicate(Player p) => new Player(p, p.FullName, p.Id);

        /// <returns>String representation of the Player</returns>
        public override string ToString()
        {
            return HasSufficientInfo
                ? FullName
                       + "\nAssists: " + ExpectedSeason.Assists
                       + "\nGoals: " + ExpectedSeason.Goals
                       + "\nPoints: " + ExpectedSeason.Points
                       + "\nGames played: " + ExpectedSeason.GamesPlayed
                : "Insufficient number of seasons or games played.";
        }
    }
    #endregion

    #region Objects needed for deserialization of the JSON persons/stats coming from the NHL's API
    public class Stat2
    {
        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Games { get; set; }
    }
    public class Split
    {
        public Stat2 Stat { get; set; }
        //public string Season { get => season; set => season = value; }
    }
    public class Stat
    {
        public List<Split> Splits { get; set; }
    }
    public class StatsList
    {
        public List<Stat> Stats { get; set; }
        private int Assists => Stats[0].Splits[0].Stat.Assists;
        private int Goals => Stats[0].Splits[0].Stat.Goals;
        private int Games => Stats[0].Splits[0].Stat.Games;
        public Season Season => new Season(Assists, Goals, Games);
    }

    public class Position
    {
        public string Code { get; set; }
    }
    public class Person
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }

        public Person()
        {
            Active = true;
        }
    }

    public class PersonList
    {
        public List<Person> People { get; set; }
    }
}
#endregion