using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using etlMon.SSISExecutionServiceProxy;
using etlMon.ScriptLauncherServiceProxy;

namespace etlMon
{
    public partial class RackViewExport : System.Web.UI.Page
    {
        protected const string sKey = "RackViewExport";

        protected void Page_Load(object sender, EventArgs e)
        {
            String sOperator = Request.LogonUserIdentity.Name;
            Message1.Text = sOperator + "&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(DateTime.Now);
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                txtSrcServer.Text = fnGetServer("SrcServer");
                txtDestServer.Text = fnGetServer("DestServer");
                txtRunDate.Text = DateTime.Now.ToString("d");
            }
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
        }

        protected String fnGetServer(String sServerName)
        {
            String sSrcServer = "";
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            String sQry = "SELECT [ConfiguredValue] FROM [Configurations] where [ConfigurationFilter] = '" + sKey + "' and [QualifiedName] = 'User::" + sServerName + "'";
            SqlCommand cmdPackage = new SqlCommand(sQry, cnSSIS);
            cmdPackage.CommandType = CommandType.Text;
            SqlDataReader drItems = null;
            cnSSIS.Open();
            drItems = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drItems.HasRows)
            {
                while (drItems.HasRows)
                {
                    sSrcServer = Convert.ToString(drItems["ConfiguredValue"]);
                }
            }
            else
            {
                sSrcServer = "";
            }
            drItems.Close();
            cnSSIS.Close();
            return sSrcServer;
        }

        protected void btnRunNow_Click(Object obj, EventArgs e)
        {
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            updRunResults.Update();
            String sOperator = Request.LogonUserIdentity.Name;
            String sSrcServer = Convert.ToString(txtSrcServer.Text);
            String sDestServer = Convert.ToString(txtDestServer.Text);
            String sRunDate = Convert.ToString(txtRunDate.Text);
            String sDirectory = "s:\\scripts";
            String sScriptName = "DEV_RACK_VIEW_EXPORT.ps1";
            String sArguments = "-srcServer " + sSrcServer + " -destServer " + sDestServer + " -runDate " + sRunDate;
            ScriptLauncherService slProxy = new ScriptLauncherService();
            slProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            slProxy.Timeout = 800000;
            lblJobResults.Text = "";
            updRunResults.Update();
            fnSetServer("SrcServer", sSrcServer);
            fnSetServer("DestServer", sDestServer);
            etlMon.ScriptLauncherServiceProxy.ResultRecord result = slProxy.ExecutePowershellScript(sDirectory, sScriptName, sArguments);
            String sResult = Convert.ToString("Standard In: " + result.StandardInput + "<br />" +
                                              "Standard Out: " + result.StandardOutput + "<br />" +
                                              "Standard Error: " + result.StandardError);
            if (result.StandardError.Length == 0)
            {
                sResult = "Success";
                lblJobResults.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                sResult = "Failed";
                lblJobResults.ForeColor = System.Drawing.Color.Red;
            }
            lblJobResults.Text = sResult;
            updRunResults.Update();
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
            lblJobResults.Text = sResult;
            lblJobResults.Visible = true;
        }

        protected void fnSetServer(String sServerName, String sValue)
        {
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            String sQry = "update [Configurations] set [ConfiguredValue] = '" + sValue + "' where [ConfigurationFilter] = '" + sKey + "' and [QualifiedName] = 'User::" + sServerName + "'";
            SqlCommand cmdPackage = new SqlCommand(sQry, cnSSIS);
            cmdPackage.CommandType = CommandType.Text;
            cnSSIS.Open();
            cmdPackage.ExecuteNonQuery();
            cnSSIS.Close();
        }
    }

}
