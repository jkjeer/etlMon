<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Services.aspx.cs" Inherits="etlMon.Services" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divNavButtons" runat="server" >
        <br />
        Server: <asp:DropDownList ID="selServer" runat="server" autopostback="True" OnSelectedIndexChanged ="selServer_IndexChanged"></asp:DropDownList>
        <br />
        <br />
        <br />
        <asp:Button id="btnStopSQLAgent" Runat="server" Text="Stop SQL Agent" CssClass="btnNavWide" onClick="btnStopSQLAgent_Click" />
        <asp:Button id="btnStartSQLAgent" Runat="server" Text="Start SQL Agent" CssClass="btnNavWide" onClick="btnStartSQLAgent_Click" />
        <br />
        <br />
        <asp:Button id="btnStopSQDR" Runat="server" Text="Stop SQDR" CssClass="btnNavWide" onClick="btnStopSQDR_Click" />
        <asp:Button id="btnStartSQDR" Runat="server" Text="Start SQDR" CssClass="btnNavWide" onClick="btnStartSQDR_Click" />
    </div>
    <br />
    <br />
    <br />
    <div id="divStopSQLAgent" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to stop the <asp:Label ID="lblServer1" runat="server" Text=""></asp:Label> SQL Agent Service now?</span> <br />
        <br />
        <asp:Button id="btnStopSQLAgentNow" Runat="server" Text="Stop Now" onClick="btnStopSQLAgentNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divStartSQLAgent" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to start the <asp:Label ID="lblServer2" runat="server" Text=""></asp:Label> SQL Agent Service now?</span> <br />
        <br />
        <asp:Button id="btnStartSQLAgentNow" Runat="server" Text="Start Now" onClick="btnStartSQLAgentNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divStopSQDRService" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to stop the <asp:Label ID="lblServer3" runat="server" Text=""></asp:Label> SQDR Service now?</span> <br />
        <br />
        <asp:Button id="btnStopSQDRNow" Runat="server" Text="Stop Now" onClick="btnStopSQDRNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divStartSQDRService" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to start the <asp:Label ID="lblServer4" runat="server" Text=""></asp:Label> SQDR Service now?</span> <br />
        <br />
        <asp:Button id="btnStartSQDRNow" Runat="server" Text="Start Now" onClick="btnStartSQDRNow_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <asp:Label id="lblJobResults" runat="server" CssClass="message" />
</asp:Content>