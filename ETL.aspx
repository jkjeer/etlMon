<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ETL.aspx.cs" Inherits="etlMon.ETL" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="divSelSubscription" class="fieldwrapper" Runat="server">
        <div class="thefieldWide">
            <asp:DropDownList ID="selSubscription" runat="server" OnSelectedIndexChanged="fnSubscriptionChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
        </div>
    </div>
    <div id="divSelPlant" class="fieldwrapper" Runat="server" visible="false" >
        <div class="fieldwrapper">
            <label for="selPlant" class="styled">Plant:</label>
            <div class="thefieldNarrow">
                <asp:DropDownList ID="selPlant" runat="server" >
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <div id="divSelDates" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtFromDate" class="styled">Date:</label>
            <div class="thefieldNarrow">
                <asp:TextBox id="txtFromDate" Runat="server" />
                <label> to </label>
                <asp:TextBox id="txtToDate" Runat="server" />
                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFromDate">
                </cc1:CalendarExtender>
                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtToDate">
                </cc1:CalendarExtender>
            </div>
        </div>
        <div id="divDatesError" class="fieldwrapper" runat="server" Visible="false" >
            <asp:Label id="lblDatesError" runat="server" ForeColor="Red" Visible="false" />
        </div>
    </div>    
    <div id="divSelWorkOrder" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtWorkOrder" class="styled">Work Order:</label>
            <div class="thefield">
                <asp:TextBox id="txtWorkOrder" Runat="server" />
            </div>
        </div>
    </div>    
    <div id="divSelSerialNumber" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtSerialNumber" class="styled">Serial Number:</label>
            <div class="thefield">
                <asp:TextBox id="txtSerialNumber" Runat="server" />
            </div>
        </div>
    </div>    
    <div id="divSelSchedule" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtSchedule" class="styled">Schedule</label>
            <div class="thefield">
                <asp:TextBox id="txtSchedule" Runat="server" />
            </div>
        </div>
    </div>    
    <div id="divSelItem" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtItem" class="styled">Item</label>
            <div class="thefield">
                <asp:TextBox id="txtItem" Runat="server" />
            </div>
        </div>
    </div>    
    <div id="divSelRackID" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtRackID" class="styled">Rack ID</label>
            <div class="thefield">
                <asp:TextBox id="txtRackID" Runat="server" />
            </div>
        </div>
    </div>    
    <div id="divGo" runat="server" visible="false">
        <asp:Button id="btnGetHistory" Runat="server" Text="Get History" CssClass="btnNav" onClick="btnGetHistory_Click" />
    </div>
    <div id="divGridHistory" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtDataSource" class="styled">Source:</label>
            <div class="thefieldWide">
                <asp:TextBox id="txtDataSource" runat="server" CssClass="message" ReadOnly="true" />
            </div>
        </div>
        <asp:Label id="lblJobResults" runat="server" CssClass="message" />
        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" CssClass="gvlogs" ></asp:GridView>     
    </div>           
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
</asp:Content>
