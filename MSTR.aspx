<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MSTR.aspx.cs" Inherits="etlMon.MSTR" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <asp:Label ID="lblMSTRTitle" runat="server" Text="MicroStrategy Scheduler" CssClass="header"></asp:Label>
    <br />
    <br />
    <div id="divStatus" runat="server" visible="true">
        <div class="fieldwrapper">
            <label for="txtBatchStatus" class="styled">Last Run Time:</label>
            <div class="thefieldWide"><asp:TextBox id="txtBatchStatus" Runat="server" ReadOnly="true" /></div>
        </div>
    </div>
    <div id="divUpdate" runat="server" visible="true">
            <asp:Button id="btnUpdate" Runat="server" Text="Run Now" Width="104" onClick="btnUpdate_Click" style="position:absolute; left:102px;" /><br />
            <br />
            <div class="thefieldWide"><textarea id="txtSQL" cols="40" rows="4" Runat="server" ></textarea></div>
    </div>
    <div id="divLog" runat="server" visible="true">
        <br />
        <div class="fieldwrapper">
            <label class="styled">Today's Log:</label>
            <asp:Button id="btnRefresh" Runat="server" Text="Refresh" Width="104" onClick="btnRefresh_Click" style="position:absolute; left:102px;" /><br />
        </div>
        <br />
        <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>
    </div>
    <div id="divUpdateNow" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to update ETL_LAST_RUN_DATE now?</span> <br />
        <br />
        <asp:Button id="btnUpdateNow" Runat="server" Text="Update Now" onClick="btnUpdateNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
</asp:Content>
