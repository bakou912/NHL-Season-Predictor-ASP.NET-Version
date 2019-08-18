using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHLPredictorASP.Classes;

namespace NHLPredictorASP
{
    public partial class Selection : Page
    {
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
                //TeamList = Session["TeamList"] == null ? new TeamList() : Session["TeamList"] as TeamList;
                //PlayersMemory = Session["PlayersMemory"] == null ? new List<Player>() : Session["PlayersMemory"] as List<Player>;

                teamsSelect.SelectedIndex = SelectionComponents.TeamIndex;
                teamsSelect.DataSource = SelectionComponents.TeamList;
                teamsSelect.DataBind();

                SelectionComponents.PersonList = SelectionComponents.TeamList[SelectionComponents.TeamIndex].PersonList;

                playersSelect.SelectedIndex = SelectionComponents.PlayerIndex;
                playersSelect.DataSource = SelectionComponents.PersonList;
                playersSelect.DataBind();

                ChangeImage(teamImg, TeamUrl, SelectionComponents.TeamList[SelectionComponents.TeamIndex].Id);
                ChangeImage(playerImg, PlayerUrl, SelectionComponents.PersonList[SelectionComponents.PlayerIndex].Id);
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
                SelectionComponents.PlayerIndex = playersSelect.SelectedIndex;
                computeButton.Enabled = false;
                var person = SelectionComponents.PersonList[playersSelect.SelectedIndex] as Roster2;

                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!SelectionComponents.PlayersMemory.Any(p => p.Id.Equals(SelectionComponents.PersonList[playersSelect.SelectedIndex].Id)))
                {
                    //Fetching the player through the player loader
                    var player = new Player(ApiLoader.LoadPlayer(DateTime.Now.Year, person.Id), person.Name, person.Id);

                    //Adding player to the already calculated players
                    SelectionComponents.PlayersMemory.Add(Player.Duplicate(player));
                    //Session["PlayersMemory"] = SelectionComponents.PlayersMemory;
 
                    //Printing the player's expected season to the result textbox
                    result.Text = player.ToString();
                }
                else
                {
                    result.Text = SelectionComponents.PlayersMemory.First(p => p.Id.Equals(person.Id)).ToString();
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
            SelectionComponents.PlayerIndex = 0;
            SelectionComponents.TeamIndex = teamsSelect.SelectedIndex;
            SelectionComponents.PersonList = SelectionComponents.TeamList[teamsSelect.SelectedIndex].PersonList;
            playersSelect.DataSource = SelectionComponents.PersonList;
            playersSelect.DataBind();

            ChangeImage(teamImg, TeamUrl, SelectionComponents.TeamList[teamsSelect.SelectedIndex].Id);
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
                SelectionComponents.TeamIndex = teamsSelect.SelectedIndex;
                SelectionComponents.PlayerIndex = playersSelect.SelectedIndex;
            }

            if (!computeButton.Enabled)
            {
                computeButton.Enabled = true;
            }

            ChangeImage(playerImg, PlayerUrl, SelectionComponents.PersonList[playersSelect.SelectedIndex].Id);
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
            SelectionComponents.TeamList = new TeamList();
        }
        public static void ResetPlayersMemory()
        {
            SelectionComponents.PlayersMemory = new List<Player>();
        }

        protected void Calibrate(object sender, EventArgs e)
        {
            Player.CalibrateCalculation(5);
            Response.Write("Adjustment: " + Player.Adjustment);
        }
    }
}