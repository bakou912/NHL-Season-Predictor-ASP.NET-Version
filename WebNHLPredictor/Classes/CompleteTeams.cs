using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web.Management;

namespace SeasonPredict
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
        public ObservableCollection<Roster2> Roster { get; set; }
    }
    public class Team
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        public RosterList Roster { get; set; }

        public ObservableCollection<Roster2> PersonList
        {
            get => Roster.Roster;
            set => Roster.Roster = value;
        }

        public override string ToString() => $"{Name} ({Abbreviation})";
    }
    //Using an ObservableCollection instead of a List for the data binding required in the GUI
    /*public class PersonTable : DataTable
    {
        private ObservableCollection<Roster2> Collection;

        public PersonTable(ObservableCollection<Roster2> personList)
        {
            Collection = personList;

            //Columns.Add("Name", typeof(string));
            Columns.Add("Player", typeof(Roster2));

            foreach (Roster2 person in Collection)
            {
                Rows.Add(person);
                AcceptChanges();
            }
        }
    }*/

    public class TeamList
    {
        public List<Team> Teams { get; set; }
    }
    #endregion

    #region TeamCollection class used to contain all teams fetched from the API and display them in the GUI
    public class TeamCollection : ObservableCollection<Team>
    {
        //Default and only constructor
        public TeamCollection()
        {
            teamsInit();
        }

        private void teamsInit()
        {
            var temp = new ApiLoader().loadTeams().OrderBy(t => t.Name);
            foreach (var t in temp)
            {
                Add(t);
            }
        }
    }
    #endregion
}
