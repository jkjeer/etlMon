using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using etlMon.ScriptLauncherServiceProxy;

namespace etlMon
{
    public partial class pyrefresh : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
 
        protected void btnSendCNCEmail_Click(Object obj, EventArgs e)
        {
           fnShowDiv("SendCNCEmail");
        }

        protected void btnStopSQLAgent_Click(Object obj, EventArgs e)
        {
            fnShowDiv("StopSQLAgent");
        }

        protected void btnStartSQLAgent_Click(Object obj, EventArgs e)
        {
            fnShowDiv("StartSQLAgent");
        }

        protected void btnTruncIntfaceTables_Click(Object obj, EventArgs e)
        {
            fnShowDiv("TruncIntfaceTables");
        }

        protected void btnSendNow_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "SEND_CNC_MAIL";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnStopNow_Click(Object obj, EventArgs e)
        {                                  
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "STOP_SQL_AGENT";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }
          
        protected void btnStartNow_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "START_SQL_AGENT";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnTruncateNow_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "TRUNCATE_TABLES";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }
        
        protected void btnStopSQDR_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "STOP_SQDR";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnStartSQDR_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-PYRefresh.ps1";
            String sArguments = "START_SQDR";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }
        
        protected void fnLaunchScript(String sDirectory, String sScriptName, String sArguments)
        {
            ScriptLauncherService slProxy = new ScriptLauncherService();
            slProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            lblJobResults.Text = "";
            etlMon.ScriptLauncherServiceProxy.ResultRecord result = slProxy.ExecutePowershellScript(sDirectory, sScriptName, sArguments);
            String sResult = Convert.ToString("Standard In: " + result.StandardInput + "<br />" +
                                              "Standard Out: " + result.StandardOutput + "<br />" +
                                              "Standard Error: " + result.StandardError);
            fnShowDiv("None");
            lblJobResults.Text = sResult;
            lblJobResults.Visible = true;
        }
           
        protected void fnShowDiv(String sDivType)
        {
            divNavButtons.Visible = false;
            divSendCNCEmail.Visible = false;
            divStopSQLAgent.Visible = false;
            divStartSQLAgent.Visible = false;
            divTruncIntfaceTables.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            switch (sDivType)
            {
                case "SendCNCEmail":
                    divNavButtons.Visible = true;
                    divSendCNCEmail.Visible = true;
                    break;
                case "StopSQLAgent":
                    divNavButtons.Visible = true;
                    divStopSQLAgent.Visible = true;
                    break;
                case "StartSQLAgent":
                    divNavButtons.Visible = true;
                    divStartSQLAgent.Visible = true;
                    break;
                case "TruncIntfaceTables":
                    divNavButtons.Visible = true;
                    divTruncIntfaceTables.Visible = true;
                    break;
                case "None":
                    divNavButtons.Visible = true;
                    break;
            }
        }

    }
}
