<%@ Page Title="TCC Viewer" Language="C#" AutoEventWireup="true" CodeBehind="TCC.aspx.cs" Inherits="etlMon.TCCBOLs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<%--    <!-- Mimic Internet Explorer 8 -->
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE8" />    --%>
    <link id="Link1" runat="server" rel="shortcut icon" href="Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="Resources/favicon.ico" type="image/ico" />
    <script type="text/javascript" src="etlMon.js" ></script>
    <style type="text/css">@import url('style.css');</style>
<%--    <![if !IE]>
        <link media="only screen and (max-device-width: 480px)" href="iPhone.css" type="text/css" rel="stylesheet" />
    <![endif]>--%>
<%--    <meta name="viewport" content="width=320, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />--%>
    <title>CGI Monitor</title>
</head>
<body onload="hideURLbar();" >
<form id="form1" runat="server">
<div id="topSection">
    <table>
        <tr>
            <td><a href="Default.aspx"><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
            <td>
                <span class="header"><asp:Label ID="pageHeader" runat="server">CGI BOL Viewer</asp:Label></span><br/>
                <span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span>
            </td>
            <td width="600px">
                <div id="divError" runat="server" Visible="false" width="600px">
                    <asp:Label id="lblError" runat="server" ForeColor="Red" Visible="true"/>
                </div>
            </td>
        </tr>
    </table>
</div>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
    <span class="message">
        <asp:Label id="User" runat="server"></asp:Label>
    </span>
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="800"/>
    
    <div id="tablediv">
        <table id="t01">
            <tr>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtBOLDate" class="styled">Date:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtBOLDate" runat="server" Font-Size="10" Width="80px" Enabled="False" />
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtBOLDate">
                            </cc1:CalendarExtender>
                        </div>
                    </div>
                </td>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtLoadNum" class="styled">Load #:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtLoadNum" runat="server" Font-Size="10" Width="80px" />
                        </div>
                    </div>
                </td>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtPlantNum" class="styled">Bill of Lading #:</label>
                        <div class="thefield">
                            <asp:TextBox id="txtPlantNum" runat="server" Font-Size="10" Width="20px" Enabled="True" /> - <asp:TextBox id="txtBOLNum" Runat="server" Font-Size="10" Width="80px" Enabled="True" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtCustName" class="styled">Name:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtCustName" runat="server" Font-Size="10" Width="280px" Enabled="False" />
                        </div>
                    </div>
                </td>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtCustNum" class="styled">Location #:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtCustNum" runat="server" Font-Size="10" Width="80px" Enabled="False" />
                        </div>
                    </div>
                </td>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtCarrier" class="styled">Carrier #:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtCarrier" runat="server" Font-Size="10" Width="80px" Enabled="False" />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtDocNum" class="styled">Document #:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtDocNum" runat="server" Font-Size="10" Width="80px" Enabled="True" />
                        </div>
                    </div>
                </td>
                <td width="30%">
                    <div class="fieldwrapper">
                        <label for="txtGuid" class="styled">Attachment Guid:</label>
                        <div class="thefieldNarrow">
                            <asp:TextBox id="txtGuid" runat="server" Font-Size="10" Width="280px"  />
                        </div>
                    </div>
                </td>
                <td>
                    <div id="divGo" runat="server">
                        <asp:Button id="btnSubmit" runat="server" Text="Get" CssClass="btnNavWide" onClick="btnSubmit_Click" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <br/>
    <iframe id="frame1" runat="server" width="100%" height="700" scrolling="auto"></iframe>
</form>
</body>
</html>
