<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CGIISRunJob.aspx.cs" Inherits="etlMon.CGIISRunJob" %>

<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            <asp:DropDownList ID="selJob" runat="server" OnSelectedIndexChanged="fnJobChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
        </div>
    </div>
    <br />
    <div id="divNavButtons" runat="server" visible="false" class="prompt">
        <asp:CheckBox ID="chkIntraday" runat="server" Checked="true" Text="Intraday" />
        <br />
        Start Days: <asp:DropDownList ID="selStartDays" runat="server"></asp:DropDownList>
        <br />
        End Days: <asp:DropDownList ID="selEndDays" runat="server"></asp:DropDownList>
        <br />
        <br />
        <span class="prompt">Are you sure you want to run <br />
            <asp:Literal ID="litJobName" runat="server" /> <br />
            now?</span>
        <br />
        <br />
    </div>
    <div id="divUpdates" runat="server" visible="false" class="prompt">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel runat="server" ID="updPanel">
            <ContentTemplate>
                <asp:Button ID="btnRunNow" runat="server" Text="Run Now" Width="104" OnClick="btnRunNow_Click" Style="position: absolute; left: 102px;" CssClass="btnScriptLauncher" Enabled="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdateProgress runat="server" ID="PageUpdateProgress">
            <ProgressTemplate>
                <br />
                <br />
                Job has been started.
                <br />
                <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..." />
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="updRunResults" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <br />
                <asp:Label runat="server" id="lblGuid" />
                <asp:Label ID="lblJobResults" runat="server" CssClass="message" Text=" " />
                <br />
                <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="true" CssClass="gvlogsSmall"></asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <br />
        <br />
    </div>
</asp:Content>
