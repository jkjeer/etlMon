<%@ Page Title="TCC Viewer" Language="C#" AutoEventWireup="true" CodeBehind="TCCViewer.aspx.cs" Inherits="etlMon.TCCViewer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="Resources/favicon.ico" type="image/ico" />
    <script type="text/javascript" src="etlMon.js" ></script>
    <style type="text/css">@import url('style.css');</style>
    <title>TCC Viewer</title>
</head>
<body onload="hideURLbar();" >
<form id="form1" runat="server">
    <div id="topSection">
        <table>
            <tr>
                <td><a href="Default.aspx"><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
                <td>
                    <span class="header"><asp:Label ID="pageHeader" runat="server">CGI Document Viewer</asp:Label></span><br/>
                    <span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span>
                </td>
            </tr>
        </table>
    </div>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
    <span class="message">
        <asp:Label id="User" runat="server"></asp:Label>
    </span>
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="800"/>
    <asp:UpdatePanel runat="server" id="updPanel" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="selPlant"/>
        </Triggers>
        <ContentTemplate>

            <div id="divEmail" class="fieldwrapper" Runat="server">
                <div class="fieldwrapper">
                    <label for="txtEmail" class="styled">Email:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtEmail" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnEMailChanged" />
                    </div>
                </div>
            </div>
            <div id="divSelPlant" class="fieldwrapper" Runat="server">
                <div class="fieldwrapper">
                    <label for="selPlant" class="styled">Plant:</label>
                    <div class="thefieldNarrow">
                        <asp:DropDownList ID="selPlant" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnPlantChanged"></asp:DropDownList>
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true" RepeatDirection="Vertical" OnSelectedIndexChanged="fnRadioButtonList1Changed">
                            <asp:ListItem Value="CustDate" Selected="True">Search by Customer & Date</asp:ListItem>
                            <asp:ListItem Value="InvoiceRange">Search by Invoice # Range</asp:ListItem>
                            <asp:ListItem Value="InvoiceList">Search by Invoice # Comma-separated List: Invoice1,Invoice2,Invoice3</asp:ListItem>
                            <asp:ListItem Value="BookingNum">Search by Booking #</asp:ListItem>
                            <asp:ListItem Value="BOLNum">Search by BOL #</asp:ListItem>
                            <asp:ListItem Value="LoadNum">Search by Load #</asp:ListItem>
                            <asp:ListItem Value="PONum">Search by PO #</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
            <br/>
            <div id="divSelCustDate" Runat="server">
                <div id="divSelCustomer" class="fieldwrapper" Runat="server">
                    <label for="selCustomer" class="styled">Customer:</label>
                    <div class="thefield">
                        <asp:DropDownList ID="selCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnCustomerChanged" />
                    </div>
                </div>
                <div id="divSelDates" class="fieldwrapper" runat="server" visible="true">
                    <label for="txtFromDate" class="styled">Date:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtFromDate" Runat="server" Font-Size="10" Width="80px" AutoPostBack="true" OnTextChanged="fnStartDateChanged" />
                        <label> to </label>
                        <asp:TextBox id="txtToDate" Runat="server" Font-Size="10" Width="80px" AutoPostBack="true" OnTextChanged="fnEndDateChanged" />
                        <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFromDate">
                        </cc1:CalendarExtender>
                        <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtToDate">
                        </cc1:CalendarExtender>
                    </div>
                </div>
            </div>
            <div id="divSelBookingNum" runat="server" visible="false">
                <div class="fieldwrapper">
                </div>
                <div class="fieldwrapper">
                    <label for="txtBookingNum" class="styled">Booking #:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtBookingNum" Runat="server" Font-Size="10" Width="180px" />
                    </div>
                </div>
            </div>
            <div id="divSelBOLNum" runat="server" visible="false">
                <div class="fieldwrapper">
                </div>
                <div class="fieldwrapper">
                    <label for="txtBOLNum" class="styled">BOL #:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtBOLNum" Runat="server" Font-Size="10" Width="80px" />
                    </div>
                </div>
            </div>
            <div id="divSelLoadNum" runat="server" visible="false">
                <div class="fieldwrapper">
                </div>
                <div class="fieldwrapper">
                    <label for="txtLoadNum" class="styled">Load #:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtLoadNum" Runat="server" Font-Size="10" Width="80px" />
                    </div>
                </div>
            </div>
            <div id="divSelPONum" runat="server" visible="false">
                <div class="fieldwrapper">
                </div>
                <div class="fieldwrapper">
                    <label for="txtPONum" class="styled">PO #:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtPONum" Runat="server" Font-Size="10" Width="180px" />
                    </div>
                </div>
            </div>
            <div id="divSelInvoiceRange" runat="server" visible="false">
                <div class="fieldwrapper">
                </div>
                <div class="fieldwrapper">
                    <label for="txtFromInvoice" class="styled">Invoice #:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtFromInvoice" Runat="server" Font-Size="10" Width="80px" />
                        <label> to </label>
                        <asp:TextBox id="txtToInvoice" Runat="server" Font-Size="10" Width="80px" />
                    </div>
                </div>
            </div>
            <div id="divSelInvoiceList" runat="server" visible="false">
                <div class="fieldwrapper">
                    <label for="txtInvoiceList" class="styled">Invoice # List:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtInvoiceList" Runat="server" Font-Size="10" TextMode="MultiLine" Rows="4" Columns="75" />
                    </div>
                </div>
            </div>
            <div id="divError" class="fieldwrapper" runat="server" Visible="false">
                <asp:Label id="lblError" runat="server" ForeColor="Red" Visible="false"/>
            </div>
            <br/>
            <div id="divGo" runat="server" visible="false">
                <asp:Button id="btnGetHistory" Runat="server" Text="Get Documents" CssClass="btnNavWide" onClick="btnGetHistory_Click" />
            </div>
            <br/>
            <asp:UpdateProgress runat="server" id="PageUpdateProgress">
                <ProgressTemplate>
                    <br/>
                    <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..."/>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div id="divGridHistory" runat="server" visible="false">
                <div class="fieldwrapper">
                    <label for="txtQueryResults" class="styled">Results:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtQueryResults" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                    </div>
                </div>
<%--                <div class="fieldwrapper">
                    <label for="txtQueryResults" class="styled">Selected:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtInvoicesSelected" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                        <asp:Button id="btnCheckAll" runat="server" Text="Select All" onClick="btnCheckAll_Click" />
                        <asp:Button id="btnUncheckAll" runat="server" Text="Unselect All" onClick="btnUncheckAll_Click" />
                        <asp:Button id="btnExport" Runat="server" Text="Export Selected Invoices" onClick="btnExport_Click" />
                    </div>
                </div>--%>
                <div id="divDocTypes" class="fieldwrapper" runat="server" visible="false">
                    <label for="chkDocTypes" class="styled">Doc Types:</label>
                    <div class="thefield">
                        <asp:CheckBoxList ID="chkDocTypes" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="chkDocTypes_Changed" />
                    </div>
                </div>
                <div id="divJobResults" runat="server" visible="false">
                    <br/>
                    <asp:Label id="lblJobResults" runat="server" CssClass="header"/>
                    <br/>
                    <br/>
                    <asp:Label id="lblJobResults1" runat="server" CssClass="header"/>
                    <br/>
                    <br/>
                    <asp:Label id="lblJobResults2" runat="server" CssClass="header"/>
                </div>
                <asp:GridView ID="gvHistory" runat="server" CssClass="gvlogs" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gvHistory_Sorting" OnSelectedIndexChanged = "gvHistory_OnSelectedIndexChanged">
                    <Columns>
                        <asp:BoundField DataField="DocID" HeaderText="DocID" ReadOnly="True" SortExpression="DocID" />
                        <asp:BoundField DataField="DocSubject" HeaderText="Doc Type" ReadOnly="True" SortExpression="DocSubject" />
                        <asp:BoundField DataField="DocNum" HeaderText="Invoice" ReadOnly="True" SortExpression="DocNum" />
                        <asp:BoundField DataField="CustNum" HeaderText="Cust" ReadOnly="True" SortExpression="CustNum" />
                        <asp:BoundField DataField="CustName" HeaderText="Name" ReadOnly="True" SortExpression="CustName" />
                        <asp:BoundField DataField="DocDate" HeaderText="Date" ReadOnly="True" SortExpression="DocDate" />
                        <asp:BoundField DataField="LoadNum" HeaderText="Load" ReadOnly="True" SortExpression="LoadNum" />
                        <asp:BoundField DataField="BOL" HeaderText="BOL" ReadOnly="True" SortExpression="BOL" />
                        <asp:BoundField DataField="PO" HeaderText="PO" ReadOnly="True" SortExpression="PO" />
                        <asp:TemplateField HeaderText="View" SortExpression="View" >  
                            <ItemTemplate>  
<%--                                <asp:CheckBox id="chkInclude" runat="server" AutoPostBack="true" Checked="True" OnCheckedChanged="chkInclude_Changed" />  --%>
                                <asp:Button id="btnView" runat="server" AutoPostBack="true" Enabled="True" Text="View" CommandArgument='<%# Eval("DocID") %>' OnCommand="btnView_Click" />  
                            </ItemTemplate>  
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Label id="lblDocID" runat="server" CssClass="header"/>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
<%--    <asp:UpdateProgress runat="server" id="PageUpdateProgress">
        <ProgressTemplate>
            <br/>
            <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..."/>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
    <asp:UpdatePanel id="updRunResults" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="selPlant"/>
        </Triggers>
        <ContentTemplate>
            <br/>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br/>
    <iframe id="frame1" runat="server" width="100%" height="700" scrolling="auto" src="\\\\Cgd1tccprod\\tccprod\\Archive\\DolDocs_1\\0\\8B\\88\\25921589-1-1.pdf"></iframe>
</form>
</body>
</html>
