<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SSIS.aspx.cs" Inherits="etlMon.SSIS" Async="true" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="fieldwrapper">
        <div class="thefieldWide">
            <asp:DropDownList ID="selPackage" runat="server" OnSelectedIndexChanged="fnPackageChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
        </div>
    </div>
    <div id="divNavButtons" runat="server" visible="false" >
        <asp:Button id="btnSchedule" Runat="server" Text="Schedule" CssClass="btnNav" onClick="btnSchedule_Click" />
        <asp:Button id="btnConfig" Runat="server" Text="Config" CssClass="btnNav" onClick="btnConfiguration_Click" />
        <asp:Button id="btnLogs" Runat="server" Text="Logs" CssClass="btnNav" onClick="btnLogs_Click" /> 
        <br />
        <asp:Button id="btnAgentHist" Runat="server" Text="History" CssClass="btnNav" onClick="btnHistory_Click" />
        <asp:Button id="btnRunJob" Runat="server" Text="Run Job" CssClass="btnNav" onClick="btnRunJob_Click" />
    </div>
    <div id="divSchedule" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtDescription" class="styled">Description:</label>
            <div class="thefieldWide"><asp:TextBox id="txtDescription" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtSchedName" class="styled">Sched Name:</label>
            <div class="thefieldWide"><asp:TextBox id="txtSchedName" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="chkIsJobEnabled" class="styled">Enabled:</label>
            <asp:CheckBox id="chkIsJobEnabled" runat="server" OnCheckedChanged="fnCheckJobEnabledChanged" AutoPostBack="true" />
            <asp:Button id="btnUpdateJobEnabled" Runat="server" Text="Update" Width="66" onClick="btnUpdateJobEnabled_Click" Visible="false" />
            <asp:Label id="lblJobEnabledUpdated" runat="server"/>
        </div>
        <div class="fieldwrapper">
            <label for="txtNextRunDate" class="styled">Next Run:</label>
            <div class="thefield"><asp:TextBox id="txtNextRunDate" Runat="server" ReadOnly="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunDate" class="styled">Last Run:</label>
            <div class="thefield">
                <asp:TextBox id="txtLastRunDate" Runat="server" ReadOnly="true" />
                <asp:TextBox id="txtLastRunDuration" Runat="server" ReadOnly="true" />
            </div>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunOutcome" class="styled">Outcome:</label>
            <div class="thefield"><asp:TextBox id="txtLastRunOutcome" Runat="server" ReadOnly="true"  Wrap="true" /></div>
        </div>
        <div class="fieldwrapper">
            <label for="txtLastRunOutcomeMsg" class="styled">Message:</label>
            <div class="thefield"><textarea id="txtLastRunOutcomeMsg" runat="server" cols="200" rows="4"></textarea></div>
        </div>
    </div>
    <div id="divConfig" runat="server" visible="false">
        <div id="divConfigFields" runat="server">        
        </div>         
    </div>
    <div id="divLogs" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="selNumLogs" class="styled">Num Logs:</label>
            <asp:DropDownList ID="selNumLogs" runat="server" OnSelectedIndexChanged="fnNumLogsChanged" AutoPostBack="true" EnableViewState="true">
            </asp:DropDownList>
        </div>
        <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>                
    </div>
    <div id="divHistory" runat="server" visible="false">
        <div class="fieldwrapper">
            <label for="txtHistory" class="styled">Job Name:</label>
            <div class="thefield"><asp:TextBox id="txtHistory" Width="200" Runat="server" ReadOnly="true" /></div>
        </div>
        <asp:GridView ID="gvHistory" runat="server" AutoGenerateColumns="true" CssClass="gvlogs"></asp:GridView>                
    </div>
    <div id="divRunJob" runat="server" visible="false" class="prompt">
        <br />
        <br />
        <br />
        <br />
        <div id="divRunNow" runat="server" >
            <span class="prompt">Are you sure you want to run the job now?</span> <br />
            <br />
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:UpdatePanel runat="server" id="updPanel">
                <ContentTemplate>
                    <asp:Button id="Button1" Runat="server" Text="Run Now" Width="104" onClick="btnRunNow_Click" style="position:absolute; left:102px;" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" id="PageUpdateProgress">
                <ProgressTemplate>
                    <br />
                    <br />
                    <br />
                    Job has been started. <br />
                    <img src="Resources/Blue_gears.gif" alt="Running..." title="Running..." />
                </ProgressTemplate>
            </asp:UpdateProgress>
            <asp:UpdatePanel id="updRunResults" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                    <br />
                    <br />
                    <br />
                    <asp:Label id="lblJobResults" runat="server"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
