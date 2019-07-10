<%@ Page Title="NHL Season Predictor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebNHLPredictor.Default" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="SeasonPredict" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                <div id="content">
                    <div class="row">
                        <div class="col-md-4">
                            <h3>Choose Team</h3>
                                <asp:DropDownList id="teamsSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TeamsSelect_OnServerChange" />
                        </div>
                        <div class="col-md-4">
                            <img class="img" id="teamImg">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-4">
                            <h3>Choose Player</h3>
                            <asp:DropDownList id="playersSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EnableComputeButton" />
                            <br/><br/>
                            <div>
                                <asp:button Text="Compute" id="computeButton" runat="server" AutoPostBack="True" onclick="ComputePlayer"></asp:button>
                            </div> 
                        </div>
                        <div class="col-md-4">
                            <img id="playerImg">
                        </div>
                    </div>
                    <h3>Result</h3>
                    <asp:TextBox TextMode="multiline" rows="5" Columns="50" id ="result" runat="server" ReadOnly="true" />
                </div>
            </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
