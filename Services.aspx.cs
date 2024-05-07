using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using etlMon.ScriptLauncherServiceProxy;

namespace etlMon
{
    public partial class Services : System.Web.UI.Page
    {
        protected const string sKey = "Services";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                fnLoadServers();
            }
        }

        protected void fnLoadServers()
        {
            selServer.Items.Clear();
            selServer.Items.Add(Convert.ToString("PY"));
            selServer.Items.Add(Convert.ToString("PRODUCTION"));
            selServer.SelectedIndex = 0;
            lblServer1.Text = "PY";
            lblServer2.Text = "PY";
            lblServer3.Text = "PY";
            lblServer4.Text = "PY";
        }

        protected void btnStopSQLAgent_Click(object obj, EventArgs e)
        {
            fnShowDiv("StopSQLAgent");
        }

        protected void btnStartSQLAgent_Click(object obj, EventArgs e)
        {
            fnShowDiv("StartSQLAgent");
        }

        protected void btnStopSQDR_Click(object obj, EventArgs e)
        {
            fnShowDiv("StopSQDR");
        }

        protected void btnStartSQDR_Click(object obj, EventArgs e)
        {
            fnShowDiv("StartSQDR");
        }

//param( [string] $serverName = '',
//        [String] $serviceName = '',
//        [string] $serviceState = '',
//        [string] $serviceStartMode = '',
//        [int] $sendReport = 1 )

        protected void btnStopSQLAgentNow_Click(object obj, EventArgs e)
        {
            var sServer = Convert.ToString(selServer.Text);
            var sDirectory = "s:\\scripts";
            var sScriptName = "Get-ServiceState.ps1";
            var sArguments = sServer + "_SQLSERVERAGENT_STOP_DISABLED";
            fnUpdateSubscriptions(sServer, "1");
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnStartSQLAgentNow_Click(object obj, EventArgs e)
        {
            var sServer = Convert.ToString(selServer.Text);
            var sDirectory = "s:\\scripts";
            var sScriptName = "Get-ServiceState.ps1";
            var sArguments = sServer + "_SQLSERVERAGENT_START_AUTOMATIC";
            fnUpdateSubscriptions(sServer, "0");
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnStopSQDRNow_Click(object obj, EventArgs e)
        {
            var sServer = Convert.ToString(selServer.Text);
            var sDirectory = "s:\\scripts";
            var sScriptName = "Get-ServiceState.ps1";
            var sArguments = sServer + "_SQDRSVC_STOP_DISABLED";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void btnStartSQDRNow_Click(object obj, EventArgs e)
        {
            var sServer = Convert.ToString(selServer.Text);
            var sDirectory = "s:\\scripts";
            var sScriptName = "Get-ServiceState.ps1";
            var sArguments = sServer + "_SQDRSVC_START_MANUAL";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }
        
        protected void fnLaunchScript(string sDirectory, string sScriptName, string sArguments)
        {
            var slProxy = new ScriptLauncherService();
            slProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            lblJobResults.Text = "";
            var result = slProxy.ExecutePowershellScript(sDirectory, sScriptName, sArguments);
            var sResult = Convert.ToString("Standard In: " + result.StandardInput + "<br />" +
                                              "Standard Out: " + result.StandardOutput + "<br />" +
                                              "Standard Error: " + result.StandardError);
            fnShowDiv("None");
            lblJobResults.Text = sResult;
            lblJobResults.Visible = true;
        }

        protected void fnUpdateSubscriptions(string sServer, string isDisabled)
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = $@"
                    UPDATE xx 
                    SET xx.[IS_DISABLED] = {isDisabled}
                    FROM [INTEGRATION].[IC].[SUBSCRIPTION_TYPE] xx
                "
            };
            cmd.Connection = GetSqlConnectionFromKey("INTEGRATION-" + sServer);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

        protected void selServer_IndexChanged(object sender, EventArgs e)
        {
            lblServer1.Text = Convert.ToString(selServer.Text);
            lblServer2.Text = Convert.ToString(selServer.Text);
            lblServer3.Text = Convert.ToString(selServer.Text);
            lblServer4.Text = Convert.ToString(selServer.Text);
            if (lblServer1.Text == "PY")
            {
                lblServer1.ForeColor = System.Drawing.Color.Black;
                lblServer2.ForeColor = System.Drawing.Color.Black;
                lblServer3.ForeColor = System.Drawing.Color.Black;
                lblServer4.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                lblServer1.ForeColor = System.Drawing.Color.Red;
                lblServer2.ForeColor = System.Drawing.Color.Red;
                lblServer3.ForeColor = System.Drawing.Color.Red;
                lblServer4.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void fnShowDiv(string sDivType)
        {
            divNavButtons.Visible = false;
            divStopSQLAgent.Visible = false;
            divStartSQLAgent.Visible = false;
            divStopSQDRService.Visible = false;
            divStartSQDRService.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            switch (sDivType)
            {
                case "StopSQLAgent":
                    divNavButtons.Visible = true;
                    divStopSQLAgent.Visible = true;
                    break;
                case "StartSQLAgent":
                    divNavButtons.Visible = true;
                    divStartSQLAgent.Visible = true;
                    break;
                case "StopSQDR":
                    divNavButtons.Visible = true;
                    divStopSQDRService.Visible = true;
                    break;
                case "StartSQDR":
                    divNavButtons.Visible = true;
                    divStartSQDRService.Visible = true;
                    break;
                case "None":
                    divNavButtons.Visible = true;
                    break;
            }
        }

        public static SqlConnection GetSqlConnectionFromKey(string keyValue)
        {
            var cn = new SqlConnection();
            var builder = new SqlConnectionStringBuilder();
            try
            {
                builder.ConnectionString = ConfigurationManager.ConnectionStrings[keyValue].ConnectionString;
                //if (!builder.MultiSubnetFailover) IsPingable(builder.DataSource);
                cn.ConnectionString = builder.ConnectionString;
            }
            catch (Exception e)
            {
                var eMsg = e.Message;
                var errorMsg = "Connection String Key not found for " + keyValue;
                throw new Exception(errorMsg);
            }
            return cn;
        }

    }
}