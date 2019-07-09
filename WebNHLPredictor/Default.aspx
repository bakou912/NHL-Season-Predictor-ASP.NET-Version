<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebNHLPredictor._Default" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="SeasonPredict" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div id="content">
        <div class="row">
            <div class="col-md-4">
                <h3>Choose Team</h3>
                    <asp:DropDownList id="teamsSelect" runat="server" AutoPostBack="True" AppendDataBoundItems="True" OnSelectedIndexChanged="teamsSelect_OnServerChange">
                    </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <img class="img" id="teamImg">
            </div>
            
        </div>
        <div class="row">
            <div class="col-md-4">
                <h3>Choose Player</h3>
                <asp:DropDownList id="playersSelect" runat="server" ></asp:DropDownList>
                <br/><br/>
                <div>
                    <button id="compute" runat="server" >Compute</button>
                </div> 
            </div>
            
            <div class="col-md-4">
                <img id="playerImg">
            </div>
        </div>

        <h3>Result</h3>
        <textarea rows="5" cols="50" id ="result" readonly></textarea>
    </div>
</asp:Content>
