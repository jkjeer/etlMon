<%@ Page Title="Power BI Report Manager" Language="C#" AutoEventWireup="true" CodeBehind="PowerBI.aspx.cs" Inherits="etlMon.PowerBI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Mimic Internet Explorer 8 -->
    <link id="Link1" runat="server" rel="shortcut icon" href="Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="Resources/favicon.ico" type="image/ico" />
    <script type="text/javascript" src="etlMon.js"></script>
    <style type="text/css">
        @import url('stylePowerBI.css');
    </style>
    <title>Power BI Report Manager</title>

</head>
<body onload="hideURLbar();">
    <form id="form1" runat="server">
        <div id="topSection">
            <table>
                <tr>
                    <td><a href="Default.aspx">
                        <img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
                    <td>
                        <span class="header">
                            <asp:Label ID="pageHeader" runat="server">Power BI Report Manager</asp:Label></span><br />
                        <span class="message">
                            <asp:Label ID="Message1" runat="server"></asp:Label></span>
                    </td>
                </tr>
            </table>
        </div>

        <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
        <span class="message">
            <asp:Label ID="User" runat="server"></asp:Label>
        </span>
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="800" />
        <asp:UpdatePanel runat="server" ID="updPanel" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="selWorkspaceNameSource" />
            </Triggers>
            <ContentTemplate>

                <div id="divEmail" class="fieldwrapper" runat="server">
                    <div class="fieldwrapper">
                        <label for="txtEmail" class="styled">Email:</label>
                        <div class="thefieldWide">
                            <asp:TextBox ID="txtEmail" runat="server" Font-Size="10" Width="180px" AutoPostBack="true" OnTextChanged="fnEMailChanged" />
                        </div>
                    </div>
                </div>

                <div id="divSelAction" class="fieldwrapper" runat="server">
                    <div class="fieldwrapper">
                        <div class="thefieldWide">
                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true" RepeatDirection="Vertical" OnSelectedIndexChanged="fnSelActionChanged">
<%--                                <asp:ListItem Value="PublishReport" Selected="True">Publish Report</asp:ListItem>--%>
                                <asp:ListItem Value="PublishFile" Selected="True">Publish File</asp:ListItem>
                                <asp:ListItem Value="UpdateParameter">Update Report Parameter</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <br />

                <div id="divSelPublish" runat="server">
                    <div id="divSelPublishReport" runat="server">
                        <div id="divSelWorkspaceNameSource" class="fieldwrapperWide" runat="server">
                            <label for="selWorkspaceNameSource" class="styledWide">Workspace Source:</label>
                            <div class="thefieldWide">
                                <asp:DropDownList ID="selWorkspaceNameSource" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnWorkspaceNameSourceChanged" />
                            </div>
                        </div>
                        <div id="divSelReportNameSource" class="fieldwrapperWide" runat="server">
                            <label for="selReportNameSource" class="styledWide">Report Name Source:</label>
                            <div class="thefieldWide">
                                <asp:DropDownList ID="selReportNameSource" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnReportNameSourceChanged" />
                            </div>
                        </div>
                    </div>

                    <div id="divSelPublishFile" runat="server">
                        <div id="divSelFolder" class="fieldwrapperWide" runat="server">
                            <label for="selFolderName" class="styledWide">Folder Name:</label>
                            <div class="thefieldWide">
                                <asp:DropDownList ID="selFolderName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnFolderNameChanged" />
                            </div>
                        </div>
                        <div id="divSelFile" class="fieldwrapperWide" runat="server">
                            <label for="selFileName" class="styledWide">File Name:</label>
                            <div class="thefieldWide">
                                <asp:DropDownList ID="selFileName" runat="server" AutoPostBack="true" OnSelectedIndexChanged="fnFileNameChanged" />
                            </div>
                        </div>
                    </div>

                    <br />
                        <asp:Label runat="server" Font-Bold="True" Font-Names="helvetica, arial, sans-serif" Font-Size="Small">Publish to:</asp:Label>
                    <br />
                    <br />

                    <div id="divSelWorkspaceGroupTarget" class="fieldwrapperWide" runat="server">
                        <label for="cblWorkspaceGroup" class="styledWide">Workspace Group:</label>
                        <div class="thefieldWide">
                            <asp:CheckBoxList ID="cblWorkspaceGroup" class="chkChoice" 
                                CellPadding="20"
                                CellSpacing="0"
                                RepeatColumns="8"
                                RepeatDirection="Horizontal"
                                RepeatLayout="Table"
                                Font-Names="helvetica, arial, sans-serif" 
                                Font-Size="X-Small"
                                TextAlign="Right"
                                runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="fnWorkspaceGroupTargetChanged">
                                <asp:ListItem>All CG</asp:ListItem>
                                <asp:ListItem>All CT</asp:ListItem>
                                <asp:ListItem>All IG</asp:ListItem>
                                <asp:ListItem>All LG</asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>

                    <div id="divSelWorkspaceNameTarget" class="fieldwrapperWide" runat="server">
                       <label for="selWorkspaceNameTarget" class="styledWide">Workspace Target:</label>
                       <div class="thefieldWide">
                            <asp:CheckBoxList ID="cblWorkspaces" class="chkChoice"
                                CellPadding="0"
                                CellSpacing="5"
                                RepeatColumns="7"
                                RepeatDirection="Vertical"
                                RepeatLayout="Table"
                                Font-Names="helvetica, arial, sans-serif" 
                                Font-Size="X-Small"
                                TextAlign="Right"
                                runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="fnWorkspaceNameTargetChanged">
                            </asp:CheckBoxList>
                        </div>
                    </div>

                    <br />

                    <div id="divSelReportNameTarget" class="fieldwrapperWide" runat="server">
                        <label for="txtReportNameTarget" class="styledWide">Report Name Target:</label>
                        <div class="thefieldWide">
                            <asp:TextBox ID="txtReportNameTarget" runat="server" Font-Size="10" Width="180px" />
                        </div>
                    </div>
                    <div id="divSelParameterName" class="fieldwrapperWide" runat="server">
                        <label for="txtParameterName" class="styledWide">Parameter Name:</label>
                        <div class="thefieldWide">
                            <asp:TextBox ID="txtParameterName" runat="server" Font-Size="10" Width="180px" Text="ConnectionStringServer" />
                        </div>
                    </div>
                    <div id="divSelNewValue" class="fieldwrapperWide" runat="server">
                        <label for="txtNewValue" class="styledWide">New Value:</label>
                        <div class="thefieldWide">
                            <asp:TextBox ID="txtNewValue" runat="server" Font-Size="10" Width="180px" />
                            <asp:Label ID="lblConnString" runat="server" Text="Connection String Value will be derived from PlantCode plus 'SQLSRV03'" ForeColor="Green" Font-Size="10" Visible="false" />
                        </div>
                    </div>

<%--                    <div id="divSelDatabase" class="fieldwrapperWide" runat="server">
                        <label for="txtDatabase" class="styledWide">New Value:</label>
                        <div class="thefieldWide">
                            <asp:TextBox ID="txtDatabase" runat="server" Font-Size="10" Width="180px" />
                            <asp:Label ID="lblDatabase" runat="server" Text="Database name is stored in CGD1BISQL1.[DW_MART].[PBI_ADMIN].[PLANT_REPORTING_DB_NAME]" ForeColor="Green" Font-Size="10" Visible="true" />
                        </div>
                    </div>--%>
                </div>

                <div id="divSelUpdateParameter" runat="server" visible="false">
                </div>

                <div id="divError" class="fieldwrapper" runat="server" visible="false">
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false" />
                </div>

                <br />

                <div id="divPublish" runat="server" visible="false">
                    <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btnNavWide" OnClick="btnPublish_Click" />
                </div>
                <div id="divUpdateParam" runat="server" visible="false">
                    <asp:Button ID="btnUpdateParam" runat="server" Text="Update" CssClass="btnNavWide" OnClick="btnUpdateParam_Click" />
                </div>

                <br />

                <asp:UpdateProgress runat="server" ID="PageUpdateProgress">
                    <ProgressTemplate>
                        <br />
                        <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..." />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <div id="divGridHistory" runat="server" visible="false">
                    <div class="fieldwrapper">
                        <label for="txtQueryResults" class="styled">Results:</label>
                        <div class="thefield">
                            <asp:TextBox ID="txtQueryResults" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                        </div>
                    </div>
                    <div class="fieldwrapper">
                        <label for="txtQueryResults" class="styled">Selected:</label>
                        <div class="thefield">
                            <asp:TextBox ID="txtInvoicesSelected" runat="server" ReadOnly="true" Font-Size="10" Width="160px" />
                        </div>
                    </div>
                    <div id="divJobResults" runat="server" visible="false">
                        <br />
                        <asp:Label ID="lblJobResults" runat="server" CssClass="header" />
                        <br />
                        <br />
                        <asp:Label ID="lblJobResults1" runat="server" CssClass="header" />
                        <br />
                        <br />
                        <asp:Label ID="lblJobResults2" runat="server" CssClass="header" />
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
        <%--    <asp:UpdateProgress runat="server" id="PageUpdateProgress">
        <ProgressTemplate>
            <br/>
            <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..."/>
        </ProgressTemplate>
    </asp:UpdateProgress>--%>
        <asp:UpdatePanel ID="updRunResults" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="selWorkspaceNameSource" />
            </Triggers>
            <ContentTemplate>
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
