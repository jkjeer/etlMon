<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ESS.aspx.cs" Inherits="etlMon.ESS" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" onClick="btnRefresh_Click" />
        </div>
    </div>
    <div id="divCurStatus" runat="server" visible="true">
        <div class="fieldwrapper">
            <label for="txtMaxDateChanged" class="styled">MaxDateChanged:</label>
            <div class="thefieldWide"><asp:TextBox id="txtMaxDateChanged" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtDays" class="styled">Days:</label>
            <div class="thefieldNarrow"><asp:TextBox id="txtDays" Runat="server" ReadOnly="true" /></div>
        </div>
    </div>
</asp:Content>
