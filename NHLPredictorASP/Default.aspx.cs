using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHLPredictorASP.Classes;

namespace NHLPredictorASP
{
    public partial class Default : Page
    {
        public static TeamList TeamList { get; private set; }

        public static List<Roster2> PersonList { get; private set; }

        public static List<Player> PlayersMemory { get; private set; }

        private static readonly string[] PlayerUrl = { "https://nhl.bamcontent.com/images/headshots/current/168x168/", ".jpg" };
        private static readonly string[] TeamUrl = { "https://www-league.nhlstatic.com/builds/site-core/01c1bfe15805d69e3ac31daa090865845c189b1d_1458063644/images/team/logo/current/", "_dark.svg" };

        #region Page and events related methods
        /// <summary>
        /// Loads the page and initializes the following components:
        /// The API loader used to fetch info from the NHL's API
        /// The team collection (all the teams loaded from the NHL's API)
        /// The player memory, used to store already calculated players (prevents extra calls to the API)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)//Prevents from resetting the components at every postback
            {
                TeamList = Session["TeamList"] == null ? new TeamList() : Session["TeamList"] as TeamList;
                PlayersMemory = Session["PlayersMemory"] == null ? new List<Player>() : Session["PlayersMemory"] as List<Player>;

                int teamIndex = (int?) Session["SelectedTeamIndex"] ?? 0;
                int playerIndex = (int?) Session["SelectedPlayerIndex"] ?? 0;

                teamsSelect.SelectedIndex = teamIndex;
                teamsSelect.DataSource = TeamList;
                teamsSelect.DataBind();

                PersonList = TeamList[teamIndex].PersonList;

                playersSelect.SelectedIndex = playerIndex;
                playersSelect.DataSource = PersonList;
                playersSelect.DataBind();

                ChangeImage(teamImg, TeamUrl, TeamList[teamIndex].Id);
                ChangeImage(playerImg, PlayerUrl, PersonList[playerIndex].Id);
            }
        }
        #endregion
        /// <summary>
        /// Calls the needed methods to calculate and print the player's expected season in the result textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ComputePlayer(object sender, EventArgs e)
        {
            if (playersSelect.SelectedItem != null)
            {
                Session["SelectedPlayerIndex"] = playersSelect.SelectedIndex;
                computeButton.Enabled = false;
                var person = PersonList[playersSelect.SelectedIndex] as Roster2;

                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!PlayersMemory.Any(p => p.Id.Equals(PersonList[playersSelect.SelectedIndex].Id)))
                {
                    //Fetching the player through the player loader
                    var player = new Player(ApiLoader.LoadPlayer(DateTime.Now.Year, person.Id), person.Name, person.Id);

                    //Adding player to the already calculated players
                    PlayersMemory.Add(Player.duplicate(player));
                    Session["PlayersMemory"] = PlayersMemory;
 
                    //Printing the player's expected season to the result textbox
                    result.Text = player.ToString();
                }
                else
                {
                    result.Text = PlayersMemory.First(p => p.Id.Equals(person.Id)).ToString();
                }
            }
        }

        /// <summary>
        /// teamsListbox SelectionChanged event handling method
        /// Changes the playersListBox content to the newly selected team in teamsListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TeamsSelect_OnServerChange(object sender, EventArgs e)
        {
            Session["SelectedPlayerIndex"] = 0;
            Session["SelectedTeamIndex"] = teamsSelect.SelectedIndex;
            PersonList = TeamList[teamsSelect.SelectedIndex].PersonList;
            playersSelect.DataSource = PersonList;
            playersSelect.DataBind();

            ChangeImage(teamImg, TeamUrl, TeamList[teamsSelect.SelectedIndex].Id);
            EnableComputeButton(sender, e);
        }

        /// <summary>
        /// playersListbox SelectionChanged event handling method
        /// When selection is changed and the button is disabled, it gets enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EnableComputeButton(object sender, EventArgs e)
        {
            if (sender == playersSelect)
            {
                Session["SelectedTeamIndex"] = teamsSelect.SelectedIndex;
                Session["SelectedPlayerIndex"] = playersSelect.SelectedIndex;
            }

            if (!computeButton.Enabled)
            {
                computeButton.Enabled = true;
            }

            ChangeImage(playerImg, PlayerUrl, PersonList[playersSelect.SelectedIndex].Id);
        }

        private void ChangeImage(Image img, string[] url, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                img.ImageUrl = url[0] + id + url[1];
            }
        }
        
        /// <summary>
        /// Initializes the teams collection through TeamCollection's contsructor
        /// </summary>
        public static void LoadTeamList()
        {
            TeamList = new TeamList();
        }
        public static void ResetPlayersMemory()
        {
            PlayersMemory = new List<Player>();
        }

        protected void Calibrate(object sender, EventArgs e)
        {
            Player.CalibrateCalculation();
            Response.Write("Adjustment: " + Player.Adjustment);
        }
    }
}