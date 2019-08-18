using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHLPredictorASP.Classes
{
    public static class SelectionComponents
    {
        private static TeamList _teamList = new TeamList();
        private static List<Roster2> _personList = new List<Roster2>();
        private static List<Player> _playersMemory = new List<Player>();


        public static TeamList TeamList
        {
            get => _teamList;
            set => _teamList = value;
        }

        public static List<Roster2> PersonList
        {
            get => _personList;
            set => _personList = value;
        }

        public static List<Player> PlayersMemory
        {
            get => _playersMemory;
            set => _playersMemory = value;
        }

        public static int TeamIndex = 0;
        public static int PlayerIndex = 0;
    }
}