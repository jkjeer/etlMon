<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SSISOld.aspx.cs" Inherits="etlMon._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="~/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="~/favicon.ico" type="image/ico" />
    <style type="text/css">@import url('style.css');</style>
    <script type="text/javascript" src="etlMon.js" ></script>
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" />     
    <title>CGI ETL Monitor</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="topsection">
            <table width="400px">
                <tr>
                    <td><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" style="width: 37px; height: 37px" /></td>
                    <td><span class="header">CGI ETL Monitor</span></td>
                    <td><a href="testpage.htm"><span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span></a></td>
                </tr>
            </table>
        </div>
        <table>
            <tr>
                <td valign="top">
                    <div id="leftcolumn">
                        <div class="fieldwrapper">
                            <asp:DropDownList ID="selPackage" runat="server" OnSelectedIndexChanged="fnPackageChanged" AutoPostBack="true">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CVPackageID" ControlToValidate="selPackage" runat="server" 
                                Operator="GreaterThan" ValueToCompare="0"
                                CssClass="disabled" ErrorMessage="Please select a Package">
                            </asp:CompareValidator>                        
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <span class="btnleft"><asp:Button id="btnSchedule" Runat="server" Text="Schedule" Width="100" onClick="btnSchedule_Click" /></span>
                    <span class="btnleft"><asp:Button id="btnConfig" Runat="server" Text="Configuration" Width="100" onClick="btnConfiguration_Click" /></span>
                    <span class="btnleft"><asp:Button id="btnLogs" Runat="server" Text="Logs" Width="100" onClick="btnLogs_Click" /></span>
                    <span class="btnleft"><asp:Button id="btnAgentHist" Runat="server" Text="History" Width="100" onClick="btnHistory_Click" /></span>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divSchedule" runat="server" visible="false">
                        <div class="fieldwrapper">
	                        <label for="txtSchedule" class="styled">Job Name:</label>
                            <div class="thefield"><asp:TextBox id="txtSchedule" Width="260" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtDescription" class="styled">Description:</label>
                            <div class="thefield"><asp:TextBox id="txtDescription" Width="260" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtSchedName" class="styled">Schedule Name:</label>
                            <div class="thefield"><asp:TextBox id="txtSchedName" Width="260" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="chkIsJobEnabled" class="styled">Enabled:</label>
                            <asp:CheckBox id="chkIsJobEnabled" runat="server" OnCheckedChanged="fnCheckJobEnabledChanged" AutoPostBack="true" />
                            <asp:Button id="btnUpdateJobEnabled" Runat="server" Text="Update" Width="100" onClick="btnUpdateJobEnabled_Click" Visible="false" />
                            <asp:Label id="lblJobEnabledUpdated" runat="server"/>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtNextRunDate" class="styled">Next Run Date:</label>
                            <div class="thefield"><asp:TextBox id="txtNextRunDate" Width="130" Runat="server" ReadOnly="true" /></div>
                            <asp:Button id="btnRunJob" Runat="server" Text="Run Now" Width="100" onClick="btnRunJob_Click" />
                            <asp:Label id="lblJobResults" runat="server"/>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtLastRunDate" class="styled">Last Run Date:</label>
                            <div class="thefield"><asp:TextBox id="txtLastRunDate" Width="130" Runat="server" ReadOnly="true" /><asp:TextBox id="txtLastRunDuration" Width="120" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtLastRunOutcome" class="styled">Last Run Outcome:</label>
                            <div class="thefield"><asp:TextBox id="txtLastRunOutcome" Width="130" Runat="server" ReadOnly="true"  Wrap="true" /></div>
                        </div>
                        <div class="fieldwrapper">
	                        <label for="txtLastRunOutcomeMsg" class="styled">Last Run Message:</label>
	                        <div class="thefield"><textarea id="txtLastRunOutcomeMsg" cols="260" rows="4" runat="server"></textarea></div>
                        </div>
                    </div>
                    <div id="divConfig" runat="server" visible="false">
                        <div class="fieldwrapper">
	                        <label for="txtConfig" class="styled">Job Name:</label>
                            <div class="thefield"><asp:TextBox id="txtConfig" Width="260" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <div id="divConfigValues" runat="server"></div>
                        <asp:GridView ID="gvConfig" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>                
                    </div>
                    <div id="divLogs" runat="server" visible="false">
                        <div class="fieldwrapper">
	                        <label for="txtLogs" class="styled">Job Name:</label>
                            <div class="thefield"><asp:TextBox id="txtLogs" Width="260" Runat="server" ReadOnly="true" /></div>
	                        <label for="txtNumLogs" class="styled">Number of Logs to Show:</label>
                            <div class="thefield">
                                <asp:TextBox id="txtNumLogs" Width="60" Runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="txtNumLogs" 
                                    Font-Size="9pt" ErrorMessage="Please enter a number."
                                    Runat="server" ID="RFVNumLogs"> </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ControlToValidate="txtNumLogs" 
                                    ValidationExpression="^\d+(?:\.\d{0,2})?$"
                                    Font-Size="9pt" ErrorMessage="Please enter a number."
                                    Runat="server" ID="REVNumLogs"> </asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>                
                    </div>
                    <div id="divHistory" runat="server" visible="false">
                        <div class="fieldwrapper">
	                        <label for="txtHistory" class="styled">Job Name:</label>
                            <div class="thefield"><asp:TextBox id="txtHistory" Width="260" Runat="server" ReadOnly="true" /></div>
                        </div>
                        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>                
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
