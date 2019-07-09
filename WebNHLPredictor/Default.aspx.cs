using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SeasonPredict;

namespace WebNHLPredictor
{
    public partial class _Default : Page
    {
        public TeamCollection TeamsCollection;

        public ObservableCollection<Roster2> PersonCollection;

        public List<Player> PlayersMemory;

        public static ApiLoader Loader;

        protected void Page_Load(object sender, EventArgs e)
        {
            Loader = new ApiLoader();
            TeamsCollection = new TeamCollection();
            PlayersMemory = new List<Player>();

            teamsSelect.DataSource = TeamsCollection;
            teamsSelect.DataBind();

            PersonCollection =TeamsCollection[0].PersonList;

            playersSelect.DataSource = PersonCollection;
            playersSelect.DataBind();

            compute.Disabled = true;
        }
/*
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SendRequest_Click(object sender, EventArgs e)
        {
            if (!playersSelect.SelectedItem.Equals(null))
            {
                //Check if the player hasn't already been loaded (avoiding to call the api again)
                if (!PlayersMemory.Any(p => p.Id.Equals((playersSelect.SelectedItem as Roster2).Id)))
                {
                    //When api is called, all GUI components are disabled to prevent user selection change problems
                    setComponentsAvailability(false);

                    expectedSeasonBox.Text = "Calculating...";

                    var player = new Player(loader.loadPlayer((playersSelect.SelectedItem as Roster2).Id), (playersSelect.SelectedItem as Roster2).Name, (playersSelect.SelectedItem as Roster2).Id);

                    PlayersMemory.Add(Player.duplicate(player));

                    expectedSeasonBox.Text = player.ToString();

                    //GUI components are enabled for the user
                    setComponentsAvailability(true);
                }
                else
                {
                    expectedSeasonBox.Text = PlayersMemory.First(p => p.Id.Equals((playersSelect.SelectedItem as Roster2).Id)).ToString();
                }
            }
        }

        /// <summary>
        ///     Makes GUI elements enabled or not according to availability parameter
        /// </summary>
        /// <param name="availability">Boolean value assigned to the IsEnabled property of graphical interface elements</param>
        private void setComponentsAvailability(bool availability)
        {
            teamsSelect.Disabled = availability;
            playersSelect.Disabled = availability;
            compute.Disabled = availability;
        }
        */
        /// <summary>
        ///     teamsListbox SelectionChanged event handling method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void teamsSelect_OnServerChange(object sender, EventArgs e)
        {
            PersonCollection = TeamsCollection[teamsSelect.SelectedIndex].PersonList;

            playersSelect.DataSource = PersonCollection;
            playersSelect.DataBind();
            compute.Disabled = true; //Since playersListbox isn't focused, the calculation button is disabled
        }
        
        /// <summary>
        ///     playersListbox SelectionChanged event handling method
        ///     When selection is changed and the button is disabled, it gets enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersListbox_OnSelectionChanged(object sender, EventArgs e)
        {
            if (compute.Disabled)
            {
                compute.Disabled = false;
            }
        }
    }
}