<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LFEmails.aspx.cs" Inherits="etlMon.LFEmails" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            &nbsp;&nbsp;
        </div>
    </div>
    <div id="divCurStatus" runat="server" visible="true">
        <asp:Repeater id="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <HeaderTemplate>
	            <table>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                <td class="checkbox"><%# DataBinder.Eval(Container.DataItem, "EmailName")%>:</td>
                <td>
                    <asp:CheckBox ID="chkEmail" runat="server" ></asp:CheckBox>
                    <asp:TextBox ID="txtID" Width="10" Runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "ID")%>' ></asp:TextBox>
                </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
	            </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div id="divSave" class="dividersection">
        <asp:Label id="msgDataSaved" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button id="btnSave" Runat="server" Text="Save" CssClass="btnStyle" onClick="btnSave_Click" />
    </div>
</asp:Content>
