using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using etlMon.ScriptLauncherServiceProxy;

namespace etlMon
{
    public partial class IGCustomerLogos : System.Web.UI.Page
    {

        String sOperator = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            sOperator = Request.LogonUserIdentity.Name;
        }

        protected void btnMoveCustLogos_Click(Object obj, EventArgs e)
        {
            fnShowDiv("MoveCustLogos");
        }

        protected void btnRunMoveCustLogos_Click(Object obj, EventArgs e)
        {
            String sDirectory = "s:\\scripts\\CGIIS";
            String sScriptName = "Invoke-CGIIS-Job.ps1";
            String sArguments = "-jobNames ('IG_CUSTOMER_LOGOS') -sql_days_start -1 -sql_days_end 1";
            fnLaunchScript(sDirectory, sScriptName, sArguments);
        }

        protected void fnLaunchScript(String sDirectory, String sScriptName, String sArguments)
        {
            ScriptLauncherService slProxy = new ScriptLauncherService();
            slProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            lblJobResults.Text = "";
            etlMon.ScriptLauncherServiceProxy.ResultRecord result = slProxy.ExecutePowershellScript(sDirectory, sScriptName, sArguments);
            //if (result.StandardError.Contains("Status code = 0"))
            //{
                result.StandardOutput = "Job started successfully.";
                result.StandardError = "";
            //}
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
            divMoveCustLogos.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            switch (sDivType)
            {
                case "MoveCustLogos":
                    divNavButtons.Visible = true;
                    divMoveCustLogos.Visible = true;
                    break;
                case "None":
                    divNavButtons.Visible = true;
                    break;
            }
        }

    }
}
