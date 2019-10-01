<%@ Page Title="NHL Season Predictor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Selection.aspx.cs" Inherits="NHLPredictorASP.Selection" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="content">
                <div class="row">
                    <div class="col-md-3">
                        <h3>Choose Team</h3>
                        <asp:DropDownList ID="teamsSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="TeamsSelect_OnServerChange" />
                    </div>
                    <div class="col-md-2">
                        <br />
                        <asp:Image ID="teamImg" runat="server" Height="75px" Width="75px" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <h3>Choose Player</h3>
                        <asp:DropDownList ID="playersSelect" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EnableComputeButton" />
                        <br />
                        <br />
                        <div>
                            <asp:Button Text="Compute" ID="computeButton" runat="server" OnClick="ComputePlayer"></asp:Button>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <br />
                        <asp:Image ID="playerImg" runat="server" Height="75px" Width="75px" />
                    </div>
                </div>
                <h3>Result</h3>
                <asp:TextBox TextMode="multiline" Rows="5" Columns="80" ID="result" runat="server" ReadOnly="true" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="Button1" runat="server" Text="Calibrate" OnClick="Calibrate" />
</asp:Content>