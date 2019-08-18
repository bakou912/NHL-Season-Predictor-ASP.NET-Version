<%@ Page Title="Ranking" Language="C#" MasterPageFile="~/Site.Master" Async="true" AutoEventWireup="true" CodeBehind="Ranking.aspx.cs" Inherits="NHLPredictorASP.Ranking" EnableEventValidation="false" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="Stylesheet" href="Content/rankingStyle.css" type="text/css" />

    <div id="content">
        <div class="row">
            <h2><%: Title %></h2> <asp:Button ID="exportButton" CssClass="export" runat="server" OnClick="Export_Click" Text="Export to Word"/>
            <br />
        </div>
        <div class="row">
            <asp:GridView ID="rankingGrid" CssClass="rankingGrid" runat="server" AllowSorting="True" OnSorting="SortColumn_Event" CellPadding="5"/>
            <br/>
            <asp:Button ID="computeAllButton" runat="server" OnClick="ComputeAll_Click" Text="Compute All Players" />
        </div>
    </div>
</asp:Content>
