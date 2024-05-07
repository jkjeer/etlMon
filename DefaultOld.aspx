<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DefaultOld.aspx.cs" Inherits="etlMon.WebForm2" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table class="layout">
        <tr>
            <td>
                <span class="btnleft"><asp:Button id="btnSSIS" Runat="server" Text="SSIS" Width="100" onClick="btnSSIS_Click" /></span>
                <span class="btnleft"><asp:Button id="btnETL" Runat="server" Text="ETL" Width="100" onClick="btnETL_Click" /></span>
                <span class="btnleft"><asp:Button id="btnMSTR" Runat="server" Text="MSTR" Width="100" onClick="btnMSTR_Click" /></span>
            </td>
        </tr>
    </table>
</asp:Content>
