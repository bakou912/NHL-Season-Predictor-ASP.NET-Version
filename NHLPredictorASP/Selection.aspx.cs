#region Header

// Author: Tommy Andrews
// File: Selection.aspx.cs
// Project: NHLPredictorASP
// Created: 06/07/2019

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHLPredictorASP.Classes;
using NHLPredictorASP.Classes.Deserialization;
using NHLPredictorASP.Classes.Entities;
using NHLPredictorASP.Classes.Utility;

namespace NHLPredictorASP
{
    public partial class Selection : Page
    {
        private static readonly string[] PlayerUrl =
            {"https://nhl.bamcontent.com/images/headshots/current/168x168/", ".jpg"};

        private static readonly string[] TeamUrl =
        {
            "https://www-league.nhlstatic.com/builds/site-core/01c1bfe15805d69e3ac31daa090865845c189b1d_1458063644/images/team/logo/current/",
            "_dark.svg"
        };

        #region Page and events related methods

        /// <summary>
        ///     Loads the page and initializes the following components:
        ///     The API loader used to fetch info from the NHL's API
        ///     The team collection (all the teams loaded from the NHL's API)
        ///     The player memory, used to store already calculated players (prevents extra calls to the API)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) //Prevents from resetting the components at every postback
            {
                teamsSelect.SelectedIndex = SelectionResources.TeamIndex;
                teamsSelect.DataSource = SelectionResources.TeamList;
                teamsSelect.DataBind();

                SelectionResources.PersonList = SelectionResources.TeamList[SelectionResources.TeamIndex].PersonList;

                playersSelect.SelectedIndex = SelectionResources.PlayerIndex;
                playersSelect.DataSource = SelectionResources.PersonList;
                playersSelect.DataBind();

                ChangeImage(teamImg, TeamUrl, SelectionResources.TeamList[SelectionResources.TeamIndex].Id);
                ChangeImage(playerImg, PlayerUrl, SelectionResources.PersonList[SelectionResources.PlayerIndex].Id);
            }
        }

        #endregion Page and events related methods

        /// <summary>
        ///     Calls the needed methods to calculate and print the player's expected season in the result textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ComputePlayer(object sender, EventArgs e)
        {
            if (playersSelect.SelectedItem == null)
            {
                return;
            }

            SelectionResources.PlayerIndex = playersSelect.SelectedIndex;
            computeButton.Enabled = false;
            var person = SelectionResources.PersonList[playersSelect.SelectedIndex];

            //Check if the player hasn't already been loaded (avoiding to call the api again)
            if (!SelectionResources.PlayersMemory.Any(p =>
                p.Id.Equals(SelectionResources.PersonList[playersSelect.SelectedIndex].Id)))
            {
                //Fetching the player through the player loader
                var player = new Player(ApiLoader.LoadPlayer(DateTime.Now.Year, person.Id), person);
                SeasonCalculator.CalculateExpectedSeason(player);

                //Adding player to the already calculated players if he has sufficient info
                if (player.HasSufficientInfo)
                {
                    SelectionResources.PlayersMemory.Add(player);
                }

                //Printing the player's expected season to the result textbox
                result.Text = player.ToString();
            }
            else
            {
                result.Text = SelectionResources.PlayersMemory.First(p => p.Id.Equals(person.Id)).ToString();
            }
        }

        /// <summary>
        ///     teamsListbox SelectionChanged event handling method
        ///     Changes the playersListBox content to the newly selected team in teamsListBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TeamsSelect_OnServerChange(object sender, EventArgs e)
        {
            SelectionResources.PlayerIndex = 0;
            SelectionResources.TeamIndex = teamsSelect.SelectedIndex;
            SelectionResources.PersonList = SelectionResources.TeamList[teamsSelect.SelectedIndex].PersonList;
            playersSelect.DataSource = SelectionResources.PersonList;
            playersSelect.DataBind();

            ChangeImage(teamImg, TeamUrl, SelectionResources.TeamList[teamsSelect.SelectedIndex].Id);
            EnableComputeButton(sender, e);
        }

        /// <summary>
        ///     playersListbox SelectionChanged event handling method
        ///     When selection is changed and the button is disabled, it gets enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EnableComputeButton(object sender, EventArgs e)
        {
            if (sender == playersSelect)
            {
                SelectionResources.TeamIndex = teamsSelect.SelectedIndex;
                SelectionResources.PlayerIndex = playersSelect.SelectedIndex;
            }

            if (!computeButton.Enabled)
            {
                computeButton.Enabled = true;
            }

            ChangeImage(playerImg, PlayerUrl, SelectionResources.PersonList[playersSelect.SelectedIndex].Id);
        }

        private void ChangeImage(Image img, string[] url, string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                img.ImageUrl = url[0] + id + url[1];
            }
        }

        protected void Calibrate(object sender, EventArgs e)
        {
            SeasonCalculator.CalibrateCalculation(5);
            Response.Write("Adjustment: " + SeasonCalculator.Adjustment);
        }
    }
}