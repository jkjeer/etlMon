<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataStageJobs.aspx.cs" Inherits="etlMon.DataStageJobs" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divNavButtons" runat="server" >
        <br />
        <asp:Button id="btnRELOAD_FIN_AD_HOC" Runat="server" Text="Reload-Fin-AdHoc" CssClass="btnNavWide" onClick="btnRELOAD_FIN_AD_HOC_Click" Enabled="false" />
        <asp:Button id="btnRELOAD_HRP_AD_HOC" Runat="server" Text="Reload-HRP-AdHoc" CssClass="btnNavWide" onClick="btnRELOAD_HRP_AD_HOC_Click" />
        <br />
        <br />
        <asp:Button id="btnMAIN_TimeTracking" Runat="server" Text="Main-TimeTracking" CssClass="btnNavWide" onClick="btnMAIN_TimeTracking_Click" />
    </div>
    <br />
    <br />
    <br />
    <div id="divRELOAD_FIN_AD_HOC" runat="server" visible="false" class="prompt" >
        <span class="prompt">Are you sure you want to run RELOAD_FIN_AD_HOC now?</span> <br />
        <br />
        <asp:Button id="btnSendNow" Runat="server" Text="Start Now" onClick="btnRunRELOAD_FIN_AD_HOC_Click" CssClass="btnScriptLauncher" Enabled="false" />
        <br />
        <br />
        <br />
    </div>
    <div id="divRELOAD_HRP_AD_HOC" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to run RELOAD_HRP_AD_HOC now?</span> <br />
        <br />
        <asp:Button id="btnStopNow" Runat="server" Text="Start Now" onClick="btnRunRELOAD_HRP_AD_HOC_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <div id="divMAIN_TimeTracking" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to run MAIN_TimeTracking now?</span> <br />
        <br />
        <asp:Button id="btnStartNow" Runat="server" Text="Start Now" onClick="btnRunMAIN_TimeTracking_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <asp:Label id="lblJobResults" runat="server" CssClass="message" />
</asp:Content>