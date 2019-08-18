<%@ Page Title="NHL Season Predictor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Selection.aspx.cs" Inherits="NHLPredictorASP.Selection" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
                <div id="content">
                    <div class="row">
                        <div class="col-md-3">
                            <h3>Choose Team</h3>
                                <asp:DropDownList id="teamsSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TeamsSelect_OnServerChange" />
                        </div>
                        <div class="col-md-2">
                            <br/>
                            <asp:Image ID="teamImg" runat="server" Height="75px" Width="75px" />  
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-3">
                            <h3>Choose Player</h3>
                            <asp:DropDownList id="playersSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EnableComputeButton" />
                            <br/><br/>
                            <div>
                                <asp:button Text="Compute" id="computeButton" runat="server" onclick="ComputePlayer"></asp:button>
                            </div> 
                        </div>
                        <div class="col-md-2">
                            <br/>
                            <asp:Image ID="playerImg" runat="server" Height="75px" Width="75px"/>
                        </div>
                    </div>
                    <h3>Result</h3>
                    <asp:TextBox TextMode="multiline" rows="5" Columns="80" id ="result" runat="server" ReadOnly="true" />
                </div>
            </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="Button1" runat="server" Text="Calibrate" onclick="Calibrate"/>
</asp:Content>
