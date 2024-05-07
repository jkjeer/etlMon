<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tools.aspx.cs" Inherits="etlMon.Tools" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divNavButtons" runat="server" >
        <br />
        <asp:Button id="btnJulian" Runat="server" Text="Julian Date" CssClass="btnNav" onClick="btnJulian_Click" />
        <asp:Button id="btnPing" Runat="server" Text="Ping" CssClass="btnNav" onClick="btnPing_Click" />
        <asp:Button id="btnTimer" Runat="server" Text="Timer" CssClass="btnNav" onClick="btnTimer_Click" />
        <br />
        <br />
        <asp:Button id="btn4" Runat="server" Text="" CssClass="btnNav" Enabled="False" />
        <asp:Button id="btn5" Runat="server" Text="" CssClass="btnNav" Enabled="False" />
        <asp:Button id="Button1" Runat="server" Text="" CssClass="btnNav" Enabled="False" />
    </div>
    <br />
    <br />
    <div id="divJulian" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtJulianDate" class="styled">Julian:</label>
            <div class="thefieldNarrow">
                <asp:TextBox id="txtJulianDate" Runat="server" />
                <asp:Button id="btnJulianToCal" Runat="server" Text="Convert to Calendar" CssClass="btnNavWide" onClick="btnJulianToCal_Click" />
            </div>
        </div>
        <br />
        <div class="fieldwrapper">
            <label for="txtCalendarDate" class="styled">Calendar:</label>
            <div class="thefieldNarrow">
                <asp:TextBox id="txtCalendarDate" Runat="server" />
                <asp:Button id="btnCalToJulian" Runat="server" Text="Convert to Julian" CssClass="btnNavWide" onClick="btnCalToJulian_Click" />
            </div>
        </div>
        <br />
        <div class="fieldwrapper">
            <asp:Label id="lblJulianError" runat="server" ForeColor="Red"/>
        </div>
    </div>
    <div id="divPing" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtPingServer" class="styled">Server:</label>
            <div class="thefield">
                <asp:TextBox id="txtPingServer" Runat="server" />
                <asp:Button id="btnPingServer" Runat="server" Text="Ping Server" CssClass="btnNav" onClick="btnPingServer_Click" />
            </div>
        </div>
        <br />
        <div class="fieldwrapper">
            <label for="txtPingOutput" class="styled">Output:</label>
            <div class="thefield"><textarea id="txtPingOutput2" runat="server" cols="200" rows="4"></textarea></div>
        </div>
        <br />        <div class="fieldwrapper">
            <asp:Label id="lblPingError" runat="server" ForeColor="Red"/>
        </div>
    </div>
</asp:Content>
