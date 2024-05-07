<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="IGCustomerLogos.aspx.cs" Inherits="etlMon.IGCustomerLogos" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divNavButtons" runat="server" >
        <br />
        <asp:Button id="btnMoveCustLogos" Runat="server" Text="Move Cust Logos" CssClass="btnNavWide" onClick="btnMoveCustLogos_Click" />
    </div>
    <br />
    <br />
    <br />
    <div id="divMoveCustLogos" runat="server" visible="false" class="prompt">
        <span class="prompt">Are you sure you want to move the Customer Logos now?</span> <br />
        <br />
        <asp:Button id="btnRunMoveCustLogos" Runat="server" Text="Start Now" onClick="btnRunMoveCustLogos_Click" CssClass="btnScriptLauncher" />
        <br />
        <br />
        <br />
    </div>
    <asp:Label id="lblJobResults" runat="server" CssClass="message" />
</asp:Content>