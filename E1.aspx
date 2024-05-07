<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="E1.aspx.cs" Inherits="etlMon.E1" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            <label for="selLookBack" class="styled">Lookback: </label>
            <asp:DropDownList ID="selLookBack" runat="server" OnSelectedIndexChanged="fnLookBackChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" onClick="btnRefresh_Click" />
        </div>
        <label for="chkAutoRefresh" class="styled">Auto-refresh: </label>
        <asp:CheckBox ID="chkAutoRefresh" runat="server" Checked="true" AutoPostBack="true" />
    </div>
    <div id="divHistory" runat="server" visible="true" >
        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" class="gvlogs"  OnRowDataBound="gvHistory_OnRowDataBound"></asp:GridView>
        <asp:Literal ID="litNoErr" runat="server"></asp:Literal>                
    </div>
</asp:Content>