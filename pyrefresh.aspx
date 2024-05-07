<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="pyrefresh.aspx.cs" Inherits="etlMon.pyrefresh" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divNavButtons" runat="server" >
        <br />
        <asp:Button id="btnStopSQLAgent" Runat="server" Text="Stop SQL Agent" CssClass="btnNavWide" onClick="btnStopSQLAgent_Click" />
        <asp:Button id="btnStartSQLAgent" Runat="server" Text="Start SQL Agent" CssClass="btnNavWide" onClick="btnStartSQLAgent_Click" />
        <br />
        <br />
        <asp:Button id="btnTruncIntfaceTables" Runat="server" Text="Trunc Intface Tables" CssClass="btnNavWide" onClick="btnTruncIntfaceTables_Click" />
        <br />
        <br />
        <asp:Button id="btnStopSQDR" Runat="server" Text="Stop SQDR" CssClass="btnNavWide" onClick="btnStopSQDR_Click" />
        <asp:Button id="btnStartSQDR" Runat="server" Text="Start SQDR" CssClass="btnNavWide" onClick="btnStartSQDR_Click" />
    </div>
    <br />
    <br />
    <br />
    <div id="divSendCNCEmail" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to send the email now?</span> <br />
        <br />
        <asp:Button id="btnSendNow" Runat="server" Text="Send Now" onClick="btnSendNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divStopSQLAgent" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to stop the SQL Agent now?</span> <br />
        <br />
        <asp:Button id="btnStopNow" Runat="server" Text="Stop Now" onClick="btnStopNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divStartSQLAgent" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to start the SQL Agent now?</span> <br />
        <br />
        <asp:Button id="btnStartNow" Runat="server" Text="Start Now" onClick="btnStartNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divTruncIntfaceTables" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to truncate the interface tables now?</span> <br />
        <br />
        <asp:Button id="btnTruncateNow" Runat="server" Text="Truncate Now" onClick="btnTruncateNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <asp:Label id="lblJobResults" runat="server" CssClass="message" />
</asp:Content>