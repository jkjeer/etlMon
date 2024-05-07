<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Timer.aspx.cs" Inherits="etlMon.Timer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <style type="text/css">@import url('style.css');</style>
    <title></title>
    <script type="text/javascript" src="CountDown.js"></script>
</head>
<body onload="init()">
    <form id="form1" runat="server">
        Time remaining: <input id="txt" readonly="readonly" type="text" name="disp" />
        <div>
            <asp:Button id="btnSubmit" Runat="server" Text="Submit" CssClass="btnNav" onClick="btnSubmit_Click" />    
        </div>
    </form>
</body>
</html>
