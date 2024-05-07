<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LaserFiche.aspx.cs" Inherits="etlMon.LaserFiche" Async="true" AsyncTimeout="800" %>
<%@ MasterType VirtualPath="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div id="divRunJob" runat="server" class="prompt">
            <div id="divRunNow" runat="server" >
                <br />
                <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="800" /> 
                <asp:UpdatePanel runat="server" id="updPanel" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Button id="Button1" Runat="server" Text="Run Now" Width="104" onClick="btnRunNow_Click" style="position:absolute; left:102px;" /><br />
                        <br /><br />
                        Days back: <asp:DropDownList ID="selDaysBack" runat="server"></asp:DropDownList>
                        <br /><br />
<%--                        Minutes lockout: <asp:DropDownList ID="selLockout" runat="server"></asp:DropDownList>
                        <br /><br />--%>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="LFEmails.aspx" >Update Email Recipients</asp:HyperLink>
                        <br /><br />
                        <asp:Label id="lblJobStarted" runat="server"> </asp:Label>
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
                        <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <br />
                        <asp:Label id="lblJobEndTime" runat="server"/>
                        <br />
                        <br />
                        <asp:Label id="lblJobResults" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
</asp:Content>

