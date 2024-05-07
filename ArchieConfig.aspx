<%@ Page Language="C#" Title="Archie Configuration" AutoEventWireup="true" CodeBehind="ArchieConfig.aspx.cs" Inherits="etlMon.ArchieConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Mimic Internet Explorer 8 -->
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8" />    
    <link id="Link1" runat="server" rel="shortcut icon" href="Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="Resources/favicon.ico" type="image/ico" />
    <script type="text/javascript" src="etlMon.js" ></script>
    <style type="text/css">@import url('style.css');</style>
<%--    <![if !IE]>
        <link media="only screen and (max-device-width: 480px)" href="iPhone.css" type="text/css" rel="stylesheet" />
    <![endif]>--%>
    <meta name="viewport" content="width=320, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>Archie Configuration</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="topSection">
            <table>
                <tr>
                    <td><a href="Default.aspx"><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
                    <td>
                        <span class="header"><asp:Label ID="pageHeader" runat="server">CGI Data Archiving Configuration</asp:Label></span><br/>
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
            <asp:AsyncPostBackTrigger controlid="btnGetArchiveConfigs"/>
        </Triggers>
        <ContentTemplate>

<%--            <div id="divEmail" class="fieldwrapper" Runat="server">
                <div class="fieldwrapper">
                    <label for="txtEmail" class="styled">Email:</label>
                    <div class="thefieldNarrow">
                        <asp:TextBox id="txtEmail" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnEMailChanged" />
                    </div>
                </div>
            </div>--%>
            <div id="divSelTask" class="fieldwrapper" Runat="server">
                <asp:RadioButtonList ID="radListTask" runat="server" AutoPostBack="true" RepeatDirection="Vertical" OnSelectedIndexChanged="fnRadListTaskChanged">
                    <asp:ListItem ID="liArchive" runat="server" Value="ArchiveData" Selected="True">Archive Data to Corp</asp:ListItem>
                    <asp:ListItem Value="TailData" enabled="false">Tail Data - Not Archived</asp:ListItem>
                    <asp:ListItem Value="DeleteLogs">Delete Log Files</asp:ListItem>
                </asp:RadioButtonList>
            </div>
<%--            <div id="divSelGroup" class="fieldwrapper" Runat="server">
                <div class="fieldwrapper">
                    <label for="selGroup" class="styled">Group:</label>
                    <div class="thefieldNarrow">
                        <asp:DropDownList ID="selGroup" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnGroupChanged"></asp:DropDownList>
                    </div>
                </div>
            </div>--%>
            <br/>
            <div id="divArchiveData" Runat="server">
                <div id="divServer" class="fieldwrapper" Runat="server">
                    <label for="txtArcServer" class="styled">Server:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcServer" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcServerChanged" />
                    </div>
                </div>
                <div id="divArcDatabase" class="fieldwrapper" Runat="server">
                    <label for="txtArcDatabase" class="styled">Database:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcDatabase" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcDatabaseChanged" />
                    </div>
                </div>
                <div id="divArcSchema" class="fieldwrapper" Runat="server">
                    <label for="txtArcSchema" class="styled">Schema:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcSchema" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcSchemaChanged" />
                    </div>
                </div>
                <div id="divArcTable" class="fieldwrapper" Runat="server">
                    <label for="txtArcTable" class="styled">Table:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcTable" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcTableChanged" />
                    </div>
                </div>
                <div id="divArcKeyColumn" class="fieldwrapper" Runat="server">
                    <label for="txtArcKeyColumn" class="styled">Key Column:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcKeyColumn" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcKeyColumnChanged" />
                    </div>
                </div>
                <div id="divArcDateColumn" class="fieldwrapper" Runat="server">
                    <label for="txtArcDateColumn" class="styled">Date Column:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcDateColumn" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnArcDateColumnChanged" />
                    </div>
                </div>
                <div id="divArcRetentionDays" class="fieldwrapper" Runat="server">
                    <label for="txtArcRetentionDays" class="styled">Retention Days:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtArcRetentionDays" Runat="server" Font-Size="10" Width="80px" AutoPostBack="true" OnTextChanged="fnArcRetentionDaysChanged" />
                    </div>
                </div>

                <asp:GridView ID="gvArcConfigs" runat="server" CssClass="gvlogs" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gvTailConfigs_Sorting">
                    <Columns>
                        <asp:BoundField DataField="ServerName" HeaderText="Server" ReadOnly="True" SortExpression="ServerName" />
                        <asp:BoundField DataField="DeleteNLog" HeaderText="NLog Files" ReadOnly="False" SortExpression="NLog" />
                        <asp:BoundField DataField="NLogRetentionDays" HeaderText="NLog Retention Days" ReadOnly="False" SortExpression="NLogRetentionDays" />
                        <asp:BoundField DataField="DeleteIISLog" HeaderText="IIS Log Files" ReadOnly="False" SortExpression="DeleteIISLog" />
                        <asp:BoundField DataField="IISLogRetentionDays" HeaderText="IIS Log Retention Days" ReadOnly="False" SortExpression="IISLogRetentionDays" />
                        <asp:BoundField DataField="DeleteScriptLog" HeaderText="Script Log Files" ReadOnly="False" SortExpression="DeleteScriptLog" />
                        <asp:BoundField DataField="ScriptLogRetentionDays" HeaderText="Script Log Retention Days" ReadOnly="False" SortExpression="ScriptLogRetentionDays" />
                        <asp:TemplateField HeaderText="Include" SortExpression="Include" >  
                            <ItemTemplate>  
                                <asp:CheckBox id="chkInclude" runat="server" AutoPostBack="true" Checked="True" OnCheckedChanged="chkInclude_Changed" />  
                            </ItemTemplate>  
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
            <div id="divTailData" Runat="server">
                <div id="divTailServer" class="fieldwrapper" Runat="server">
                    <label for="txtTailServer" class="styled">Server:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailServer" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailServerChanged" />
                    </div>
                </div>
                <div id="divTailDatabase" class="fieldwrapper" Runat="server">
                    <label for="txtTailDatabase" class="styled">Database:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailDatabase" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailDatabaseChanged" />
                    </div>
                </div>
                <div id="divTailSchema" class="fieldwrapper" Runat="server">
                    <label for="txtTailSchema" class="styled">Schema:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailSchema" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailSchemaChanged" />
                    </div>
                </div>
                <div id="divTailTable" class="fieldwrapper" Runat="server">
                    <label for="txtTailTable" class="styled">Table:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailTable" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailTableChanged" />
                    </div>
                </div>
                <div id="divTailKeyColumn" class="fieldwrapper" Runat="server">
                    <label for="txtTailKeyColumn" class="styled">Key Column:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailKeyColumn" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailKeyColumnChanged" />
                    </div>
                </div>
                <div id="divTailDateColumn" class="fieldwrapper" Runat="server">
                    <label for="txtTailDateColumn" class="styled">Date Column:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailDateColumn" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnTailDateColumnChanged" />
                    </div>
                </div>
                <div id="divTailRetentionDays" class="fieldwrapper" Runat="server">
                    <label for="txtTailRetentionDays" class="styled">Retention Days:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtTailRetentionDays" Runat="server" Font-Size="10" Width="80px" AutoPostBack="true" OnTextChanged="fnTailRetentionDaysChanged" />
                    </div>
                </div>

                <asp:GridView ID="gvTailConfigs" runat="server" CssClass="gvlogs" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gvTailConfigs_Sorting">
                    <Columns>
                        <asp:BoundField DataField="ServerName" HeaderText="Server" ReadOnly="True" SortExpression="ServerName" />
                        <asp:BoundField DataField="DeleteNLog" HeaderText="NLog Files" ReadOnly="False" SortExpression="NLog" />
                        <asp:BoundField DataField="NLogRetentionDays" HeaderText="NLog Retention Days" ReadOnly="False" SortExpression="NLogRetentionDays" />
                        <asp:BoundField DataField="DeleteIISLog" HeaderText="IIS Log Files" ReadOnly="False" SortExpression="DeleteIISLog" />
                        <asp:BoundField DataField="IISLogRetentionDays" HeaderText="IIS Log Retention Days" ReadOnly="False" SortExpression="IISLogRetentionDays" />
                        <asp:BoundField DataField="DeleteScriptLog" HeaderText="Script Log Files" ReadOnly="False" SortExpression="DeleteScriptLog" />
                        <asp:BoundField DataField="ScriptLogRetentionDays" HeaderText="Script Log Retention Days" ReadOnly="False" SortExpression="ScriptLogRetentionDays" />
                        <asp:TemplateField HeaderText="Include" SortExpression="Include" >  
                            <ItemTemplate>  
                                <asp:CheckBox id="chkInclude" runat="server" AutoPostBack="true" Checked="True" OnCheckedChanged="chkInclude_Changed" />  
                            </ItemTemplate>  
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
            <div id="divDeleteLogs" Runat="server">
                <div id="divDelServer" class="fieldwrapper" Runat="server">
                    <label for="txtDelServer" class="styled">Server:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtDelServer" Runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnDelServerChanged" />
                    </div>
                </div>
                <div id="divDelNLog" class="fieldwrapper" Runat="server">
                    <asp:CheckBox ID="chkNLog" runat="server" Font-Size="10" />NLog files
                    <label for="txtDeleteLogRetentionDays" class="styled">Log Retention Days:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtDeleteLogRetentionDays" Runat="server" Font-Size="10" Width="80px" AutoPostBack="true" OnTextChanged="fnDelNLogRetentionDaysChanged" />
                    </div>
                </div>
                    <br/>

                <div id="divDelIISLog" class="fieldwrapper" Runat="server">
                    <asp:CheckBox ID="chkIISLog" runat="server" Font-Size="10" />IIS Log files
                </div>
                    <br/>
                <div id="divDelScriptLog" class="fieldwrapper" Runat="server">
                    <asp:CheckBox ID="chkScriptLog" runat="server" Font-Size="10" />Script Log files
                </div>

                <asp:GridView ID="gvDeleteLogs" runat="server" CssClass="gvlogs" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gvDeleteLogs_Sorting">
                    <Columns>
                        <asp:BoundField DataField="ServerName" HeaderText="Server" ReadOnly="True" SortExpression="ServerName" />
                        <asp:BoundField DataField="LogRetentionDays" HeaderText="NLog Retention Days" ReadOnly="False" SortExpression="NLogRetentionDays" />
                        <asp:BoundField DataField="DeleteNLog" HeaderText="NLog Files" ReadOnly="False" SortExpression="NLog" />
                        <asp:BoundField DataField="DeleteIISLog" HeaderText="IIS Log Files" ReadOnly="False" SortExpression="DeleteIISLog" />
                        <asp:BoundField DataField="DeleteScriptLog" HeaderText="Script Log Files" ReadOnly="False" SortExpression="DeleteScriptLog" />
                        <asp:TemplateField HeaderText="Include" SortExpression="Include" >  
                            <ItemTemplate>  
                                <asp:CheckBox id="chkInclude" runat="server" AutoPostBack="true" Checked="True" OnCheckedChanged="chkInclude_Changed" />  
                            </ItemTemplate>  
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
            <div id="divError" class="fieldwrapper" runat="server" Visible="false">
                <asp:Label id="lblError" runat="server" ForeColor="Red" Visible="false"/>
            </div>
            <br/>
            <div id="divGo" runat="server" visible="false">
                <asp:Button id="btnGetArchiveConfigs" Runat="server" Text="Get Configs" CssClass="btnNavWide" onClick="btnGetArchiveConfigs_Click" />
            </div>
            <br/>
            <asp:UpdateProgress runat="server" id="PageUpdateProgress">
                <ProgressTemplate>
                    <br/>
                    <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..."/>
                </ProgressTemplate>
            </asp:UpdateProgress>
            <div id="divGridArchive" runat="server" visible="false">
                <div class="fieldwrapper">
                    <label for="txtQueryResults" class="styled">Results:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtQueryResults" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                    </div>
                </div>
                <div class="fieldwrapper">
                    <label for="txtQueryResults" class="styled">Selected:</label>
                    <div class="thefield">
                        <asp:TextBox id="txtInvoicesSelected" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                        <asp:Button id="btnCheckAll" runat="server" Text="Select All" onClick="btnCheckAll_Click" />
                        <asp:Button id="btnUncheckAll" runat="server" Text="Unselect All" onClick="btnUncheckAll_Click" />
                        <asp:Button id="btnExport" Runat="server" Text="Export Selected Invoices" onClick="btnExport_Click" />
                    </div>
                </div>
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
                <asp:GridView ID="gvConfigs" runat="server" CssClass="gvlogs" AutoGenerateColumns="False" AllowSorting="true" OnSorting="gvConfigs_Sorting">
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
                        <asp:TemplateField HeaderText="Include" SortExpression="Include" >  
                            <ItemTemplate>  
                                <asp:CheckBox id="chkInclude" runat="server" AutoPostBack="true" Checked="True" OnCheckedChanged="chkInclude_Changed" />  
                            </ItemTemplate>  
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdatePanel id="updRunResults" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger controlid="btnGetArchiveConfigs"/>
        </Triggers>
        <ContentTemplate>
            <br/>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
