<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="switchboard.aspx.cs" Inherits="etlMon.Switchboard" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link id="Link1" runat="server" rel="shortcut icon" href="~Resources/favicon.ico" type="image/x-icon" />
    <link id="Link2" runat="server" rel="icon" href="~Resources/favicon.ico" type="image/ico" />
    <style type="text/css">@import url('style.css');</style>
    <script type="text/javascript" src="etlMon.js" ></script>
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0;" />     
    <title>CGI Monitor</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="topsection">
            <table width="400px">
                <tr>
                    <td><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" /></td>
                    <td>
                        <span class="header">CGI ETL Monitor</span><br />
                        <a href="testpage.htm"><span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span></a>
                    </td>
                </tr>
            </table>
        </div>
        <div id="navsection">
            <table class="layout">
                <tr>
                    <td>
                        <span class="btnleft"><asp:Button id="btnSSIS" Runat="server" Text="SSIS" Width="100" onClick="btnSSIS_Click" /></span>
                        <span class="btnleft"><asp:Button id="btnETL" Runat="server" Text="ETL" Width="100" onClick="btnETL_Click" Enabled="false" /></span>
                        <span class="btnleft"><asp:Button id="btnMSTR" Runat="server" Text="MSTR" Width="100" onClick="btnMSTR_Click" Enabled="false" /></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="btnleft"><asp:Button id="Button1" Runat="server" Text="Network" Width="100" onClick="btnSSIS_Click" Enabled="false" /></span>
                        <span class="btnleft"><asp:Button id="Button2" Runat="server" Text="SQL" Width="100" onClick="btnSSIS_Click" Enabled="false" /></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="footersection">
        </div>
    </form>
</body>
</html>
