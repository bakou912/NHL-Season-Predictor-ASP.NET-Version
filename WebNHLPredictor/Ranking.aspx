<%@ Page Title="Ranking" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ranking.aspx.cs" Inherits="WebNHLPredictor.Contact" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="Stylesheet" href="Content/rankingStyle.css" type="text/css" />
    <h2><%: Title %></h2>

    <asp:GridView ID="rankingGrid" CssClass="rankingGrid" runat="server" AllowSorting="True" OnSorting="SortColumn_Event" CellPadding="5"/>
</asp:Content>
