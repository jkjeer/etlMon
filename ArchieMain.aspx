<%@ Page Language="C#" Title="Archie Main" AutoEventWireup="true" CodeBehind="ArchieMain.aspx.cs" Inherits="etlMon.ArchieMain" %>
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
    <title>Archie Main</title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="topSection">
            <table>
                <tr>
                    <td><a href="Default.aspx"><img alt="Cardinal Glass Logo" src="Resources/cardinalLogo_regular.jpg" class="cgiLogo" title="Home" /></a></td>
                    <td>
                        <span class="header"><asp:Label ID="pageHeader" runat="server">CGI Archiver</asp:Label></span><br/>
                        <span class="message"><asp:Label id="Message1" runat="server"></asp:Label></span>
                    </td>
                </tr>
            </table>
        </div>
        <div class="fieldwrapper">
            <div class="thefield">
                <asp:Button id="btnArchiveData" runat="server" Text="Archive Data" onClick="btnArchiveData_Click" />
            </div>
        </div>
    </form>
</body>
</html>
