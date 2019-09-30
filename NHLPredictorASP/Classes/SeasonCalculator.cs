using System;
using System.Collections.Generic;
using System.Linq;

namespace NHLPredictorASP.Classes
{
    public static class SeasonCalculator
    {

        //Adjustment calculated with the CalibrateCalculation method.
        public static double Adjustment { get; private set; } = 1.0152455599855;

        /// <summary>
        /// Calculates the adjustment needed for the ExpectedSeason's stats.
        /// Predicts the last nbSeasons seasons of all players in the league and compares
        /// them with the actual last nbSeasons seasons.
        /// <param name="nbSeason">Integer representing the number of seasons to calculate for the calibration</param>
        /// </summary>
        public  static void CalibrateCalculation(int nbSeasons)
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

                    var initPlayer = ApiLoader.LoadPlayer(year--, person.Id);
                    CalculateExpectedSeason(initPlayer);
                    var completeSeasonList = initPlayer.SeasonList;

                    for (var i = 0; i < nbSeasons; i++)
                    {
                        var index = completeSeasonList.FindIndex(s => s.SeasonYears == $"{year}{year + 1}");
                        if (index < 0)
                        {
                            continue;
                        }

                        var player = ApiLoader.LoadPlayer(year, person.Id);
                        CalculateExpectedSeason(player);
                        if (!player.HasSufficientInfo)
                        {
                            break;
                        }

                        var actualSeason = completeSeasonList[index];

                        year--;

                        //Incrementing points totals
                        totalPoints += actualSeason.Points;
                        totalExpectedPoints += player.ExpectedSeason.Points;
                        completeSeasonList.RemoveAt(index);
                    }
                }
            }
            //Adjustment is meant to bring the expectd points of all players to the actual points scored
            Adjustment = totalPoints / totalExpectedPoints;
        }

        /// <summary>
        /// Calculates an estimation of the player's next season by computing a weighted average with the most important season being the most recent
        /// </summary>
        public static void CalculateExpectedSeason(Player player)
        {
            var growthRate = 1.0;//Growth rate used to adjust the prediction according to the player's production's improvement
            var previousValid = false;
            var weightsList = new List<double>();

            player.SeasonList.Reverse();

            for (var i = 0; i < player.SeasonList.Count; i++)
            {
                if (player.SeasonList.Count > 5)//If there are enough seasons to eliminate the ones below games played average
                {
                    if (player.SeasonList[i].GamesPlayed >= 50)//If above respectable number of games played
                    {
                        AddWeight(player, weightsList, i);
                        if (previousValid)
                        {
                            weightsList[weightsList.Count - 1] *= (double)player.SeasonList[i].Points / player.SeasonList[i - 1].Points;
                        }
                        previousValid = true;
                    }
                    else//Eliminate season with below average games played
                    {
                        previousValid = false;
                        player.Remove(i);
                        i--;//Stay at the same index since the next one is moved back
                    }
                }
                else
                {
                    AddWeight(player, weightsList, i);
                }
            }
            //Total of all absolute weights used to calculate relative weight of each season in next step
            var total = weightsList.Sum();

            for (var i = 0; i < weightsList.Count; i++)
            {
                ApplySeasonWeight(player, weightsList, i, total);
            }

            if (player.ExpectedSeason.GamesPlayed > 82)
            {
                player.ExpectedSeason.GamesPlayed = 82;
            }

            //Determines if the player has sufficient info
            player.HasSufficientInfo = player.SeasonList.Count > 2 && player.ExpectedSeason.GamesPlayed >= 50;

            Adjust(player, growthRate <= 1.1f ? (growthRate >= 0.9f ? growthRate : 0.9f) : 1.1f);

            player.ExpectedSeason.CalculatePoints();
        }

        /// <summary>
        /// Adjusts the player's stats according to the Adjustment ratio and the player's growth rate
        /// </summary>
        /// <param name="growth">Growth rate used to adjust the prediction according to the player's production's improvement</param>
        private static void Adjust(Player player, double growth = 1.0)
        {
            player.ExpectedSeason.Assists = (int)Math.Round(player.ExpectedSeason.Assists * Adjustment * growth);
            player.ExpectedSeason.Goals = (int)Math.Round(player.ExpectedSeason.Goals * Adjustment * growth);
        }

        /// <summary>
        /// Adds the wanted season stats to the totals used later for averages
        /// </summary>
        /// <param name="weightList">Current list of weights each season has on the overall calculation</param>
        /// <param name="i">Current season index</param>
        /// <param name="total">Sum of all season weights</param>
        private static void ApplySeasonWeight(Player player, List<double> weightList, int i, double total)
        {
            weightList[i] /= total;//Making this season's weight into percentage
            player.ExpectedSeason.Assists += (int)Math.Round((player.SeasonList[i].Assists * weightList[i]));
            player.ExpectedSeason.Goals += (int)Math.Round((player.SeasonList[i].Goals * weightList[i]));
            player.ExpectedSeason.GamesPlayed += (int)Math.Round((player.SeasonList[i].GamesPlayed * weightList[i]));
        }

        /// <summary>
        /// Adds weight the the weightsList at index i. Uses arbritrary multipliers to make most recent seasons more important 
        /// </summary>
        /// <param name="weightsList">Current list of weights each season has on the overall calculation</param>
        /// <param name="i">Index indicating which season's weight is added</param>
        private static void AddWeight(Player player, List<double> weightsList, int i)
        {
            if (i == 0)
            {
                weightsList.Add((double)(player.SeasonList.Count - i) * 0.4f);
            }
            else if (i == 1)
            {
                weightsList.Add((double)(player.SeasonList.Count - i) / (player.SeasonList.Count) * 1.1f);

            }
            else
            {
                weightsList.Add((double)(player.SeasonList.Count - i) / (player.SeasonList.Count * (i + 1)));
            }
        }
    }
}