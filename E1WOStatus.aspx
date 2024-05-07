<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="E1WOStatus.aspx.cs" Inherits="etlMon.E1WOStatus" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="lblE1WOStatusTitle" runat="server" Text="E1 Work Order Status" CssClass="header"></asp:Label>
    <br />
        <asp:Label ID="Label1" runat="server" Text="Label" class="message"></asp:Label>
    <br />
    <div id="divWorkOrder" runat="server" visible="true">
        <div class="fieldwrapper">
            <label for="txtWorkOrder" class="styled">Work Order:</label>
            <div class="thefieldNarrow"><asp:TextBox id="txtWorkOrder" Runat="server" /></div>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtWorkOrder"
                ErrorMessage="Please Enter Only Numbers" ValidationExpression="^\d+$" ></asp:RegularExpressionValidator>
        </div>
    </div>
    <div id="divRetrieve" runat="server" visible="true">
            <asp:Button id="btnRetrieve" Runat="server" Text="Retrieve" Width="104" onClick="btnRetrieve_Click" style="position:absolute; left:102px;" /><br />
    </div>
    <div id="divCurStatus" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtWorkOrderStatus" class="styled">Status:</label>
            <div class="thefieldNarrow"><asp:TextBox id="txtWorkOrderStatus" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtWorkOrderDate" class="styled">Date:</label>
            <div class="thefieldWide"><asp:TextBox id="txtWorkOrderDate" Runat="server" ReadOnly="true" /></div>
        </div>
    </div>
    <div id="divWONotFound" runat="server" visible="false">
        <br />
        <div class="fieldwrapper">
            <asp:Label id="lblWONotFound" runat="server" ForeColor="Red" Text="Work Order not found." />
        </div>
    </div>
    <div id="divReset" runat="server" visible="false">
            <asp:Button id="btnReset" Runat="server" Text="Reset" Width="104" onClick="btnReset_Click" style="position:absolute; left:102px;" /><br />
    </div>
    <div id="divNoPermissions" runat="server" visible="false">
        <br />
        <div class="fieldwrapper">
            <asp:Label id="lblNoPermissions" runat="server" ForeColor="Red" Text="You don't have permission to perform this operation." />
        </div>
    </div>
</asp:Content>