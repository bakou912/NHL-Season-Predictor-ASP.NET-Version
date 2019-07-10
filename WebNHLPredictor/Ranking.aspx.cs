using System;
using System.Data;
using System.Web.UI;

namespace WebNHLPredictor
{
    public partial class Contact : Page
    {
        public static DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack && Default.PlayersMemory != null)
            {
                //Initializing data table
                dt = new DataTable();

                //Initiating data table's column model
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Assists", typeof(int));
                dt.Columns.Add("Goals", typeof(int));
                dt.Columns.Add("Points", typeof(int));
                dt.Columns.Add("Games Played", typeof(int));

                //Calling the populating method
                PopulateGrid();
            }
        }

        /// <summary>
        /// Populates and binds the grid to the player memory
        /// </summary>
        private void PopulateGrid()
        {
            //Adds players to the data table
            foreach (var player in Default.PlayersMemory)
            {
                DataRow dr = dt.NewRow();
                //Adding new row containing the player's expected season's info
                dt.Rows.Add(player.FullName, player.ExpectedSeason.Assists, player.ExpectedSeason.Goals, player.ExpectedSeason.Points, player.ExpectedSeason.GamesPlayed);
            }

            //Binding grid to the data table
            rankingGrid.DataSource = dt;
            rankingGrid.DataBind();
        }

        protected void SortColumn_Event(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
        {
            //TODO code sorting by column method
        }
    }
}