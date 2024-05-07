<%@ Page Language="C#"  Title="CGI Monitor" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="etlMon.WebForm2"%>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <asp:Button id="btnArchie" Runat="server" Text="Archie" onClick="btnArchie_Click" CssClass="btnNav" />
    <asp:Button id="btnIGCustomerLogos" Runat="server" Text="IG Cust Logos" onClick="btnIGCustomerLogos_Click" CssClass="btnNav" />
    <asp:Button id="btnE1WOStatus" Runat="server" Text="E1WOStatus" onClick="btnE1WOStatus_Click" CssClass="btnNav" />
    <br />
    <br />
    <asp:Button id="btnMON" Runat="server" Text="MON" onClick="btnMON_Click" CssClass="btnNav" />
    <asp:Button id="btnE1" Runat="server" Text="E1" onClick="btnE1_Click" CssClass="btnNav" />
    <asp:Button id="btnErrLogs" Runat="server" Text="Err Logs" onClick="btnErrLogs_Click" CssClass="btnNav" />
    <br />
    <br />
    <asp:Button id="btnTools" Runat="server" Text="Tools" onClick="btnTools_Click" CssClass="btnNav" />
    <asp:Button id="btnServices" Runat="server" Text="Services" onClick="btnServices_Click" CssClass="btnNav" />
    <asp:Button id="btnNetwork" Runat="server" Text="Network" onClick="btnNetwork_Click" CssClass="btnNav" />
    <br />
    <br />
    <asp:Button id="btnLaserFiche" Runat="server" Text="LaserFiche" onClick="btnLaserFiche_Click" CssClass="btnNav" />
    <asp:Button id="btnLFLogs" Runat="server" Text="LF Logs" onClick="btnLFLogs_Click" CssClass="btnNav" />
    <asp:Button id="btnLFEmails" Runat="server" Text="LFEmails" onClick="btnLFEmails_Click" CssClass="btnNav" />
    <br />
    <br />
    <asp:Button id="btnTCCViewer" Runat="server" Text="TCC Viewer" onClick="btnTCCViewer_Click" CssClass="btnNav" />
    <asp:Button id="btnTCCInvoices" Runat="server" Text="TCC Invoices" onClick="btnTCCInvoices_Click" CssClass="btnNav" />
    <asp:Button id="btnTCCBOLs" Runat="server" Text="Import BOLs" onClick="btnTCCBOLs_Click" CssClass="btnNav" />
    <br />
    <br />
    <asp:Button id="btnFGLogs" Runat="server" Text="FG Logs" onClick="btnFGLogs_Click" CssClass="btnNav" />
    <asp:Button id="btnIntradayLogs" Runat="server" Text="Intraday Logs" onClick="btnIntradayLogs_Click" CssClass="btnNav" />
    <asp:Button id="btnPYRefresh" Runat="server" Text="PY Refresh" onClick="btnPYRefresh_Click" CssClass="btnNav" />
</asp:Content>
