using NHLPredictorASP.Deserialization;


namespace NHLPredictorASP.Entities
{
    #region Season Class representing the offensive production of a Player for a given year

    public class Season
    {
        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Points { get; set; }
        public int GamesPlayed { get; set; }
        public string SeasonYears { get; private set; }

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
        {}

        public void CalculatePoints()
        {
            Points = Assists + Goals;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Season))
            {
                return false;
            }

            Season s = obj as Season;

            return s.Points == Points && s.Goals == Goals && s.GamesPlayed == GamesPlayed && GetHashCode() == s.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (Assists != 0 ? Assists.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (Goals != 0 ? Goals.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (GamesPlayed != 0 ? GamesPlayed.GetHashCode() : 0);
                return hash;
            }
        }
    }
    #endregion Season Class representing the offensive production of a Player for a given year
}