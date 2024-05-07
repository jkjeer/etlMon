using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using etlMon.ScriptLauncherServiceProxy;

namespace etlMon
{
    public partial class DataStageJobs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnRELOAD_FIN_AD_HOC_Click(Object obj, EventArgs e)
        {
            fnShowDiv("RELOAD_FIN_AD_HOC");
        }

        protected void btnRELOAD_HRP_AD_HOC_Click(Object obj, EventArgs e)
        {
            fnShowDiv("RELOAD_HRP_AD_HOC");
        }

        protected void btnMAIN_TimeTracking_Click(Object obj, EventArgs e)
        {
            fnShowDiv("MAIN_TimeTracking");
        }

         protected void btnRunRELOAD_FIN_AD_HOC_Click(Object obj, EventArgs e)
        {
            String sOperator = Request.LogonUserIdentity.Name;
            //String sDirectory = "s:\\scripts";
            //String sScriptName = "Get-DataStageJob.ps1";
            String sOption = "RELOAD_FIN_AD_HOC";
            String sArguments = "-option " + sOption + " -operator " + sOperator;
            //fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

         protected void btnRunRELOAD_HRP_AD_HOC_Click(Object obj, EventArgs e)
        {
            String sOperator = Request.LogonUserIdentity.Name;
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-DataStageJob.ps1";
            String sOption = "RELOAD_HRP_AD_HOC";
            String sArguments = "-option " + sOption + " -operator " + sOperator;
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

         protected void btnRunMAIN_TimeTracking_Click(Object obj, EventArgs e)
        {
            String sOperator = Request.LogonUserIdentity.Name;
            String sDirectory = "s:\\scripts";
            String sScriptName = "Get-DataStageJob.ps1";
            String sOption = "MAIN_TimeTracking";
            String sArguments = "-option " + sOption + " -operator " + sOperator;
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void fnLaunchScript(String sDirectory, String sScriptName, String sArguments)
        {
            ScriptLauncherService slProxy = new ScriptLauncherService();
            slProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            lblJobResults.Text = "";
            etlMon.ScriptLauncherServiceProxy.ResultRecord result = slProxy.ExecutePowershellScript(sDirectory, sScriptName, sArguments);
            if (result.StandardError.Contains("Status code = 0"))
            {
                result.StandardOutput = "Job started successfully.";
                result.StandardError = "";
            }
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
            divRELOAD_FIN_AD_HOC.Visible = false;
            divRELOAD_HRP_AD_HOC.Visible = false;
            divMAIN_TimeTracking.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            switch (sDivType)
            {
                case "RELOAD_FIN_AD_HOC":
                    divNavButtons.Visible = true;
                    divRELOAD_FIN_AD_HOC.Visible = true;
                    break;
                case "RELOAD_HRP_AD_HOC":
                    divNavButtons.Visible = true;
                    divRELOAD_HRP_AD_HOC.Visible = true;
                    break;
                case "MAIN_TimeTracking":
                    divNavButtons.Visible = true;
                    divMAIN_TimeTracking.Visible = true;
                    break;
                case "None":
                    divNavButtons.Visible = true;
                    break;
            }
        }

    }
}
