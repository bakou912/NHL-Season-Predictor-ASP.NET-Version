using System.Collections.Generic;

namespace NHLPredictorASP.Classes
{
    #region Static SelectionComponents class used to encapsulate runtime team list, person list and player memory
    public static class SelectionComponents
    {
        /// <summary>
        /// Static constructor for SelectonComponents
        /// Initializes all components needed for the Selection and Ranking pages
        /// </summary>
        static SelectionComponents()
        {
            TeamList = new TeamList();
            PersonList = new List<StatsRoster>();
            PlayersMemory = new List<Player>();
        }

        public static TeamList TeamList { get; set; }

        public static List<StatsRoster> PersonList { get; set; }

        public static List<Player> PlayersMemory { get; set; }

        public static int TeamIndex = 0;
        public static int PlayerIndex = 0;
    }
    #endregion
}