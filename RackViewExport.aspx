<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RackViewExport.aspx.cs" Inherits="etlMon.RackViewExport" AsyncTimeout="800" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="Resources/favicon.ico" type="image/ico" />
    <script type="text/javascript" src="etlMon.js" ></script>
    <style type="text/css">@import url('style.css');</style>
    <title>RackViewExport</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="topSection">
            <table>
                <tr>
                    <td><a href="Default.aspx"><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
                    <td>
                        <span class="header"><asp:Label ID="pageHeader" runat="server">CGI Monitor</asp:Label></span><br/>
                        <span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divRunJob" runat="server" class="prompt">
            <br />
            <br />RackViewExport
            <br />
            <br />
            <div id="divRunNow" runat="server" >
                <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="800" /> 
                <asp:UpdatePanel runat="server" id="updPanel" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger controlid="btnRunNow" eventname="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Button id="btnRunNow" Runat="server" Text="Run Now" Width="104" onClick="btnRunNow_Click" style="position:absolute; left:102px;" /><br />
                        <br /><br />
                        <div class="fieldwrapper">
                            <label for="txtSrcServer" class="styled">Source Server:</label>
                            <div class="thefield">
                                <asp:TextBox id="txtSrcServer" Runat="server" />
                            </div>
                        </div>
                        <div class="fieldwrapper">
                            <label for="txtDestServer" class="styled">Dest Server:</label>
                            <div class="thefield">
                                <asp:TextBox id="txtDestServer" Runat="server" />
                            </div>
                        </div>
                        <div class="fieldwrapper">
                            <label for="txtRunDate" class="styled">Date:</label>
                            <div class="thefield">
                                <asp:TextBox id="txtRunDate" Runat="server" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtRunDate">
                                </cc1:CalendarExtender>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdateProgress runat="server" id="PageUpdateProgress">
                    <ProgressTemplate>
                        <br />
                        <br />
                        <br />
                        <%
                            string startTime;
                            startTime = Convert.ToString(DateTime.Now);
                            Response.Write("Job started at " + startTime); %>
                        <br />
                        <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..." />
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <asp:UpdatePanel id="updRunResults" runat="server" UpdateMode="Conditional" >
                    <Triggers>
                        <asp:AsyncPostBackTrigger controlid="btnRunNow" eventname="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <br />
                        <br />
                        <br />
                        <asp:Label id="lblJobResults" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>
