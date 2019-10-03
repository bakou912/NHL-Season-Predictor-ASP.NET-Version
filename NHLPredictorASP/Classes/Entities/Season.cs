#region Header

// Author: Tommy Andrews
// File: Season.cs
// Project: NHLPredictorASP
// Created: 09/30/2019

#endregion

using NHLPredictorASP.Classes.Deserialization;

namespace NHLPredictorASP.Classes.Entities
{
    #region Season Class representing the offensive production of a Player for a given year

    public class Season
    {
        public Season()
        {
            Assists = 0;
            Goals = 0;
            Points = 0;
            GamesPlayed = 0;
        }

        public Season(int assists, int goals, int gamesPlayed, string seasonYears = "")
        {
            Assists = assists;
            Goals = goals;
            GamesPlayed = gamesPlayed;
            SeasonYears = seasonYears;
            CalculatePoints();
        }

        public Season(Split split) : this(split.Stat.Assists, split.Stat.Goals, split.Stat.Games, split.Season)
        {
        }

        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Points { get; set; }
        public int GamesPlayed { get; set; }
        public string SeasonYears { get; }

        public void CalculatePoints()
        {
            Points = Assists + Goals;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Season))
            {
                return false;
            }

            var s = (Season) obj;

            return s.Points == Points && s.Goals == Goals && s.GamesPlayed == GamesPlayed &&
                   GetHashCode() == s.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int hashingBase = (int) 2166136261;
                const int hashingMultiplier = 16777619;

                var hash = hashingBase;
                hash = (hash * hashingMultiplier) ^ (Assists != 0 ? Assists.GetHashCode() : 0);
                hash = (hash * hashingMultiplier) ^ (Goals != 0 ? Goals.GetHashCode() : 0);
                hash = (hash * hashingMultiplier) ^ (GamesPlayed != 0 ? GamesPlayed.GetHashCode() : 0);
                return hash;
            }
        }
    }

    #endregion Season Class representing the offensive production of a Player for a given year
}