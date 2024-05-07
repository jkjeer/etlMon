<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FGLogs.aspx.cs" Inherits="etlMon.FGLogs" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            <asp:DropDownList ID="selLookBack" runat="server" OnSelectedIndexChanged="fnLookBackChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" onClick="btnRefresh_Click" />
        </div>
    </div>
    <div id="divHistory" runat="server" visible="true">
        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>
        <asp:Literal ID="litNoLogs" runat="server"></asp:Literal>                
    </div>
</asp:Content>