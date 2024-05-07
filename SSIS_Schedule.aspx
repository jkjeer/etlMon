<%@ Page Title="" Language="C#" MasterPageFile="~/SSIS.Master" AutoEventWireup="true" CodeBehind="SSIS_Schedule.aspx.cs" Inherits="etlMon.SSIS_Schedule" %>
<%@ MasterType VirtualPath="SSIS.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="divSchedule" runat="server" >
        <div class="fieldwrapper">
            <label for="txtSchedule" class="styled">Job Name:</label>
            <div class="thefield"><asp:TextBox id="txtSchedule" Width="220" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtDescription" class="styled">Description:</label>
            <div class="thefield"><asp:TextBox id="txtDescription" Width="220" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtSchedName" class="styled">Schedule Name:</label>
            <div class="thefield"><asp:TextBox id="txtSchedName" Width="220" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="chkIsJobEnabled" class="styled">Enabled:</label>
            <asp:CheckBox id="chkIsJobEnabled" runat="server" OnCheckedChanged="fnCheckJobEnabledChanged" AutoPostBack="true" />
            <asp:Button id="btnUpdateJobEnabled" Runat="server" Text="Update" Width="100" onClick="btnUpdateJobEnabled_Click" Visible="false" />
            <asp:Label id="lblJobEnabledUpdated" runat="server"/>
        </div>
        <div class="fieldwrapper">
            <label for="txtNextRunDate" class="styled">Next Run Date:</label>
            <div class="thefield"><asp:TextBox id="txtNextRunDate" Width="120" Runat="server" ReadOnly="true" /></div>
            <asp:Button id="btnRunJob" Runat="server" Text="Run Now" Width="90" onClick="btnRunJob_Click" />
            <asp:Label id="lblJobResults" runat="server"/>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunDate" class="styled">Last Run Date:</label>
            <div class="thefield">
                <asp:TextBox id="txtLastRunDate" Width="120" Runat="server" ReadOnly="true" />
                <asp:TextBox id="txtLastRunDuration" Width="84" Runat="server" ReadOnly="true" />
            </div>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunOutcome" class="styled">Last Run Outcome:</label>
            <div class="thefield"><asp:TextBox id="txtLastRunOutcome" Width="120" Runat="server" ReadOnly="true"  Wrap="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunOutcomeMsg" class="styled">Last Run Message:</label>
            <div class="thefield"><textarea id="txtLastRunOutcomeMsg" cols="200" rows="4" runat="server"></textarea></div>
        </div>
    </div>
</asp:Content>
