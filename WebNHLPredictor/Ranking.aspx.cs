using SeasonPredict;
using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebNHLPredictor
{
    public partial class Contact : Page
    {
        private enum Source
        {
            Internal, External
        }
        protected DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["DataTable"] == null)
            {
                dt = new DataTable();
                //Initiating data table's column model
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("A", typeof(int));
                dt.Columns.Add("G", typeof(int));
                dt.Columns.Add("P", typeof(int));
                dt.Columns.Add("GP", typeof(int));
            }
            else
            {
                dt = Session["DataTable"] as DataTable;
            }

            if (Default.PlayersMemory != null)
            {
                //Calling the populating method
                PopulateGrid();

                //Binding grid to the data table
                rankingGrid.DataSource = dt;
                rankingGrid.DataBind();
            }
        }

        /// <summary>
        /// Populates and binds the grid to the player memory
        /// </summary>
        private void PopulateGrid()
        {
            dt.Rows.Clear();

            //Adds players to the data table
            foreach (var player in Default.PlayersMemory)
            {
                //Adding new row containing the player's expected season's info if it has sufficient information
                if (player.HasSufficientInfo)
                {
                    dt.Rows.Add(player.FullName, player.ExpectedSeason.Assists, player.ExpectedSeason.Goals, player.ExpectedSeason.Points, player.ExpectedSeason.GamesPlayed);
                }
            }

            Session["DataTable"] = dt;
        }

        /// <summary>
        /// Sorts the ranking gridview according to a specific column
        /// Can be descending or ascending
        /// </summary>
        protected void SortColumn_Event(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            SortDirection direction = SortDirection.Ascending;

            if (Session["SortExpression"] == null || (SortDirection)Session["SortDirection"] == SortDirection.Ascending || !Session["SortExpression"].Equals(e.SortExpression))
            {
                direction = SortDirection.Descending;
            }

            DataRow[] rows;

            //Sorting according to the sorting direction
            if (direction == SortDirection.Descending)
            {
                rows = dt.Select().OrderByDescending(r => r[e.SortExpression]).ToArray();
            }
            else
            {
                rows = dt.Select().OrderBy(r => r[e.SortExpression]).ToArray();
            }
            
            dt = rows.CopyToDataTable();

            //Setting session sorting states to present states so that sorting is reversed each time
            Session["DataTable"] = dt;
            Session["SortDirection"] = direction;
            if (Session["SortExpression"] == null || !Session["SortExpression"].Equals(e.SortExpression))
            {
                Session["SortExpression"] = e.SortExpression;
            }

            //Binding grid to the data table
            rankingGrid.DataSource = dt;
            rankingGrid.DataBind();
        }

        /// <summary>
        /// Computes all players in the NHL and populates the ranking grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ComputeAll_Click(object sender, EventArgs e)
        {
            dt.Rows.Clear();

            if (Default.TeamsCollection != null)
            {
                foreach (Team team in Default.TeamsCollection)
                {
                    foreach(Roster2 person in team.PersonList)
                    {
                        var player = ApiLoader.loadPlayer(person.Id);
                        player.FullName = person.Name;

                        if (player.HasSufficientInfo)
                        {
                            dt.Rows.Add(player.FullName, player.ExpectedSeason.Assists, player.ExpectedSeason.Goals, player.ExpectedSeason.Points, player.ExpectedSeason.GamesPlayed);
                            Default.AddToPlayersMemory(player);
                        }
                    }
                }

                rankingGrid.DataSource = dt;
                rankingGrid.DataBind();

                Session["DataTable"] = dt;
            }
            else
            {
                //Initializing Default.TeamsCollection
                Default.LoadTeamsCollection();
                //Callback to the method with a now initialized TeamsCollection
                ComputeAll_Click(sender, e);
            }
        }

    }
}