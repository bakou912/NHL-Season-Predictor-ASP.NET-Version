using System;
using System.Collections.Generic;
using System.Linq;

namespace NHLPredictorASP.Classes
{
    #region Player class
    public class Player : Person
    {
        //The list of seasons in this Player's career
        public List<Season> SeasonList { get; }
        //The expected season for this Player's next year
        public Season ExpectedSeason { get; }
        //Represents the sufficiency of this Player's stats in his career
        public bool HasSufficientInfo { get; private set; }

        //Adjustment calculated with the CalibrateCalculation method.
        public static double Adjustment { get; private set; } = 1.0558325638658;//Before growth rate:1.00806340590399f;

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
                Add(Season.duplicate(s));
            }
            CalculateExpectedSeason();
        }

        /// <summary>
        /// Calculates the adjustment needed for the ExpectedSeason's stats.
        /// Predicts the last 5 seasons of all players in the league and compares
        /// them with the actual last 5 seasons.
        /// <param name="nbSeason">Integer representing the number of seasons to calculate for the calibration</param>
        /// </summary>
        //TODO: optimize method, time complexity is too high
        public static void CalibrateCalculation(int nbSeasons)
        {
            //All actual points scored by any player in the last {nbSeasons} seasons
            var totalPoints = 0.0;
            //All expected points calculated for any player in the last {nbSeasons} seasons
            var totalExpectedPoints = 0.0;

            //For all teams, all players are calculated and compared to the actual production
            foreach (var team in new TeamList())
            {
                foreach (var person in team.PersonList)
                {
                    //Setting the starting year to the last completed season
                    var year = DateTime.Now.Year;
                    for (var i = 0; i < nbSeasons; i++)
                    {
                        var player = ApiLoader.LoadPlayer(year - 1, person.Id);
                        var actualSeason = ApiLoader.GetSeason(year, person.Id);

                        if (!player.HasSufficientInfo || actualSeason == null)
                        {
                            break;
                        }

                        year--;

                        //Incrementing points totals
                        totalPoints += actualSeason.Points;
                        totalExpectedPoints += player.ExpectedSeason.Points;
                    }
                }
            }
            //Adjustment is meant to bring the expectd points of all players to the actual points scored
            Adjustment = totalPoints / totalExpectedPoints;
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
        /// <param name="p">Player to duplicate</param>
        /// <returns>The duplication of the player passed as a parameter</returns>
        public static Player Duplicate(Player p) => new Player(p, p.FullName, p.Id);

        /// <summary>
        /// Calculates an estimation of the player's next season by computing a weighted average with the most important season being the most recent
        /// </summary>
        public void CalculateExpectedSeason()
        {
            var total = 0.0;
            var growthRate = 1.0;//Growth rate used to adjust the prediction according to the player's production's improvement
            var previousValid = false;
            var weightsList = new List<double>();
            var i = 0;
            var averageGames = SeasonList.Count == 0 ? 0 : (int)SeasonList.Average(s => s.GamesPlayed);

            for (i = 0; i < SeasonList.Count; i++)
            {
                if (SeasonList.Count > 5)//If there are enough seasons to eliminate the ones below games played average
                {
                    if (SeasonList[i].GamesPlayed >= averageGames)//If above games played average
                    {
                        AddWeight(weightsList, i);
                        if (previousValid)
                        {
                            growthRate *= (double)SeasonList[i].Points / SeasonList[i - 1].Points;
                        }
                        previousValid = true;
                    }
                    else//Eliminate season with below average games played
                    {
                        previousValid = false;
                        Remove(i);
                        i--;//Stay at the same index since the next one is moved back
                    }
                }
                else
                {
                    AddWeight(weightsList, i);
                }
            }
            //Total of all absolute weights used to calculate relative weight of each season in next step
            total = weightsList.Sum();

            for (i = 0; i < weightsList.Count; i++)
            {
                ApplySeasonWeight(weightsList, i, total);
            }

            if (ExpectedSeason.GamesPlayed > 82)
            {
                ExpectedSeason.GamesPlayed = 82;
            }

            //Determines if the player has sufficient info
            HasSufficientInfo = SeasonList.Count > 2 && ExpectedSeason.GamesPlayed >= 50;

            Adjust(growthRate);

            ExpectedSeason.calculatePoints();
        }

        /// <summary>
        /// Adjusts the player's stats according to the Adjustment ratio and the player's growth rate
        /// </summary>
        /// <param name="growth">Growth rate used to adjust the prediction according to the player's production's improvement</param>
        private void Adjust(double growth = 1.0)
        {
            ExpectedSeason.Assists = (int)Math.Round(ExpectedSeason.Assists * Adjustment * growth);
            ExpectedSeason.Goals = (int)Math.Round(ExpectedSeason.Goals * Adjustment * growth);
        }

        /// <summary>
        /// Adds the wanted season stats to the totals used later for averages
        /// </summary>
        /// <param name="weightList">Current list of weights each season has on the overall calculation</param>
        /// <param name="i">Current season index</param>
        /// <param name="total">Sum of all season weights</param>
        private void ApplySeasonWeight(List<double> weightList, int i, double total)
        {
            weightList[i] /= total;//Making this season's weight into percentage
            ExpectedSeason.Assists += (int)Math.Round((SeasonList[i].Assists * weightList[i]));
            ExpectedSeason.Goals += (int)Math.Round((SeasonList[i].Goals * weightList[i]));
            ExpectedSeason.GamesPlayed += (int)Math.Round((SeasonList[i].GamesPlayed * weightList[i]));
        }

        /// <summary>
        /// Adds weight the the weightsList at index i. Uses arbritrary multipliers to make most recent seasons more important 
        /// </summary>
        /// <param name="weightsList">Current list of weights each season has on the overall calculation</param>
        /// <param name="i">Index indicating which season's weight is added</param>
        private void AddWeight(List<double> weightsList, int i)
        {
            if (i == 0)
            {
                weightsList.Add((double)(SeasonList.Count - i) * 0.4f);
            }
            else if (i == 1)
            {
                weightsList.Add((double)(SeasonList.Count - i) / (SeasonList.Count) * 1.1f);

            }
            else
            {
                weightsList.Add((double)(SeasonList.Count - i) / (SeasonList.Count * (i + 1)));
            }
        }


        /// <returns>String representation of the Player</returns>
        public override string ToString()
        {
            if (HasSufficientInfo)
            {
                return FullName
                       + "\nAssists: " + ExpectedSeason.Assists
                       + "\nGoals: " + ExpectedSeason.Goals
                       + "\nPoints: " + ExpectedSeason.Points
                       + "\nGames played: " + ExpectedSeason.GamesPlayed;

            }
            return "Insufficient number of seasons or games played.";
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