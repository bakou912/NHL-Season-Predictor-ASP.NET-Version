﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHLPredictorASP.Classes;
using WebNHLPredictor;
using DataTable = System.Data.DataTable;
using Page = System.Web.UI.Page;
using Spire.Doc;

namespace NHLPredictorASP
{
    public partial class Ranking : Page
    {
        private DataTable _dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["DataTable"] == null)
            {
                InitDataTable();
            }
            else
            {
                _dt = Session["DataTable"] as DataTable;
            }

            exportButton.Visible = false;

            if (Default.PlayersMemory != null && Default.PlayersMemory.Count > 0)
            {
                //Calling the populating method
                PopulateGrid();
            }
        }

        /// <summary>
        /// Initializes the DataTable _dt with the right columns
        /// </summary>
        private void InitDataTable()
        {
            _dt = new DataTable("Ranking");
            //Initiating data table's column model
            _dt.Columns.Add("Name", typeof(string));
            _dt.Columns.Add("A", typeof(int));
            _dt.Columns.Add("G", typeof(int));
            _dt.Columns.Add("P", typeof(int));
            _dt.Columns.Add("GP", typeof(int));
        }

        /// <summary>
        /// Populates and binds the grid to the player memory
        /// </summary>
        private void PopulateGrid()
        {
            exportButton.Visible = true;

            if (Default.PlayersMemory.Count <= _dt.Rows.Count)
            {
                return;
            }

            _dt.Rows.Clear();

            //Adds players to the data table
            foreach (var player in Default.PlayersMemory)
            {
                //Adding new row containing the player's expected season's info if it has sufficient information
                if (player.HasSufficientInfo)
                {
                    _dt.Rows.Add(player.FullName,
                                player.ExpectedSeason.Assists,
                                player.ExpectedSeason.Goals,
                                player.ExpectedSeason.Points,
                                player.ExpectedSeason.GamesPlayed);
                }
            }

            BindGrid();
        }

        /// <summary>
        /// Sorts the ranking GridView according to a specific column
        /// Can be descending or ascending
        /// </summary>
        protected void SortColumn_Event(object sender, GridViewSortEventArgs e)
        {
            var direction = SortDirection.Ascending;

            if (Session["SortExpression"] == null || (SortDirection)Session["SortDirection"] == SortDirection.Ascending || !Session["SortExpression"].Equals(e.SortExpression))
            {
                direction = SortDirection.Descending;
            }

            //Sorting according to the sorting direction
            var rows = direction == SortDirection.Descending
                ? _dt.Select().OrderByDescending(r => r[e.SortExpression]).ToArray()
                : _dt.Select().OrderBy(r => r[e.SortExpression]).ToArray();

            _dt = rows.CopyToDataTable();

            //Setting session sorting states to present states so that sorting is reversed each time
            Session["SortDirection"] = direction;
            if (Session["SortExpression"] == null || !Session["SortExpression"].Equals(e.SortExpression))
            {
                Session["SortExpression"] = e.SortExpression;
            }
            BindGrid();
        }

        /// <summary>
        /// Computes all players in the NHL and populates the ranking grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ComputeAll_Click(object sender, EventArgs e)
        {
            _dt.Rows.Clear();

            if (Default.TeamsCollection != null)
            {
                foreach (var team in Default.TeamsCollection)
                {
                    foreach(var person in team.PersonList)
                    {
                        var player = ApiLoader.LoadPlayer(DateTime.Now.Year,person.Id);
                        player.FullName = person.Name;

                        if (!player.HasSufficientInfo)
                        {
                            continue;
                        }
                        _dt.Rows.Add(player.FullName, player.ExpectedSeason.Assists, player.ExpectedSeason.Goals, player.ExpectedSeason.Points, player.ExpectedSeason.GamesPlayed); }
                }
                BindGrid();
            }
            else
            {
                //Initializing Default.TeamsCollection
                Default.LoadTeamsCollection();
                //Callback to the method with a now initialized TeamsCollection
                ComputeAll_Click(sender, e);
            }

            //Making the computeAll button invisible
            computeAllButton.Visible = false;

            //Making the export button visible
            exportButton.Visible = true;
        }

        /// <summary>
        /// Binds the DataTable to the ranking GridView
        /// </summary>
        protected void BindGrid()
        {
            _dt.AcceptChanges();
            Session["DataTable"] = _dt;
            rankingGrid.DataSource = _dt;
            rankingGrid.DataBind();
        }

        /// <summary>
        /// Exports the ranking grid to a Word file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Export_Click(object sender, EventArgs e)
        {
            if (rankingGrid.Rows.Count <= 0)
            {
                return;
            }

            Response.ClearContent();
            Response.AppendHeader("content-disposition", "attachment; filename=ranking.doc");
            Response.ContentType = "application/word";

            var stringWriter = new StringWriter();
            var htw = new HtmlTextWriter(stringWriter);

            rankingGrid.HeaderRow.Style.Clear();
            rankingGrid.HeaderRow.Style.Add("background-color", "#999999");
            rankingGrid.RenderControl(htw);

            Response.Write(stringWriter.ToString());
            Response.End();
        }

        //Prevents the RenderControl method from verifying the rendering for the ranking GridView
        public override void VerifyRenderingInServerForm(Control control) {}
        
    }
}