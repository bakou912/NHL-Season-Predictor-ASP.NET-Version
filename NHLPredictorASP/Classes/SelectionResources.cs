using NHLPredictorASP.Deserialization;
using NHLPredictorASP.Entities;
using System.Collections.Generic;

namespace NHLPredictorASP.Classes
{
    #region Static SelectionResources class used to encapsulate runtime team list, person list and player memory

    public static class SelectionResources
    {

        public static TeamList TeamList { get; set; } = new TeamList();

        public static List<StatsRoster> PersonList { get; set; } = new List<StatsRoster>();

        public static List<Player> PlayersMemory { get; set; } = new List<Player>();
        public static int TeamIndex { get; set; }

        public static int PlayerIndex { get; set; }
    }

    #endregion Static SelectionComponents class used to encapsulate runtime team list, person list and player memory
}