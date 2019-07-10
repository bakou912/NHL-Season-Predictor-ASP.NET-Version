<%@ Page Title="Ranking" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Ranking.aspx.cs" Inherits="WebNHLPredictor.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <h3>Ranking Page to be built containing all previously estimated players</h3>

    <asp:GridView ID="rankingGrid" runat="server" AllowSorting="True" OnSorting="SortColumn_Event"/>
</asp:Content>
