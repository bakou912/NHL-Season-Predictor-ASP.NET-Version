#region Header

// Author: Tommy Andrews
// File: Player.cs
// Project: NHLPredictorASP
// Created: 09/30/2019

#endregion

using System.Collections.Generic;
using NHLPredictorASP.Classes.Deserialization;

namespace NHLPredictorASP.Classes.Entities
{
    #region Player class used to represent an NHL player

    public class Player : Person
    {
        public Player(List<Season> seasonsToDuplicate)
        {
            SeasonList = new List<Season>();
            ExpectedSeason = new Season();
            FullName = "";
            HasSufficientInfo = false;

            foreach (var s in seasonsToDuplicate)
            {
                Add(s);
            }
        }

        /// <summary>
        ///     Copy constructor
        /// </summary>
        /// <param name="p">Player to copy</param>
        /// <param name="name">The wanted name for this Player</param>
        /// <param name="id">The wanted id for this Player</param>
        /// <param name="teamAbv"></param>
        public Player(Player p, string name, string id, string teamAbv) : this(p.SeasonList)
        {
            Id = id;
            FullName = name;
            TeamAbv = teamAbv;
        }

        //The list of seasons in this Player's career
        public List<Season> SeasonList { get; }

        //The expected season for this Player's next year
        public Season ExpectedSeason { get; }

        //Represents the sufficiency of this Player's stats in his career
        public bool HasSufficientInfo { get; set; }

        /// <summary>
        ///     Adds season s to the SeasonList
        /// </summary>
        /// <param name="s">Season to add</param>
        public void Add(Season s)
        {
            SeasonList.Add(s);
        }

        /// <summary>
        ///     Removes the season at index i in SeasonList
        /// </summary>
        /// <param name="i">Index indicating which season to remove</param>
        public void Remove(int i)
        {
            SeasonList.RemoveAt(i);
        }

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

        public override bool Equals(object o)
        {
            if (!(o is Player))
            {
                return false;
            }

            var p = (Player) o;

            return ExpectedSeason.Equals(p.ExpectedSeason) && FullName.Equals(p.FullName) && Id == p.Id &&
                   GetHashCode() == p.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int hashingBase = (int) 2166136261;
                const int hashingMultiplier = 16777619;

                var hash = hashingBase;
                hash = (hash * hashingMultiplier) ^ (!(ExpectedSeason is null) ? ExpectedSeason.GetHashCode() : 0);
                hash = (hash * hashingMultiplier) ^ (!(FullName is null) ? FullName.GetHashCode() : 0);
                hash = (hash * hashingMultiplier) ^ (!(Id is null) ? Id.GetHashCode() : 0);
                return hash;
            }
        }
    }

    #endregion Player class used to represent an NHL player
}