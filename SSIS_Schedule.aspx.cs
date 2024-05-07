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

namespace etlMon
{
    public partial class SSIS_Schedule : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            fnGetSchedule();
        }
        
        protected void fnGetSchedule()
        {
            String sPackageName = Convert.ToString(Session["sPackageName"]); // Master.selPackageText();
            txtSchedule.Text = sPackageName;
            /* Initialize controls. */
            txtDescription.Text = "";
            txtSchedName.Text = "";
            chkIsJobEnabled.Checked = false;
            lblJobEnabledUpdated.Text = "";
            txtNextRunDate.Text = "";
            txtLastRunDate.Text = "";
            txtLastRunOutcome.Text = "";
            txtLastRunOutcomeMsg.Value = "";
            txtLastRunDuration.Text = "";
            //WebMsgBox.Show(sPackageName); 
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drPackage = null;
            SqlCommand cmdPackage = new SqlCommand("usp_GetJobInfo", cnSSIS);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobName = cmdPackage.Parameters.Add("@JobName", SqlDbType.VarChar);
            pJobName.Direction = ParameterDirection.Input;
            pJobName.Value = sPackageName;
            cnSSIS.Open();
            drPackage = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drPackage.Read())
            {
                String sJobID = Convert.ToString(drPackage["job_id"]);
                txtDescription.Text = Convert.ToString(drPackage["description"]);
                txtSchedName.Text = Convert.ToString(drPackage["sched_name"]);
                chkIsJobEnabled.Checked = Convert.ToBoolean(drPackage["enabled"]);
                lblJobEnabledUpdated.Text = fnIsJobEnabled(Convert.ToBoolean(drPackage["enabled"]));
                txtNextRunDate.Text = Convert.ToString(drPackage["next_run_date"]);
                txtLastRunDate.Text = Convert.ToString(drPackage["last_run_date"]);
                Int16 iOutcome = Convert.ToInt16(drPackage["last_run_outcome"]);
                String sOutcome = "";
                switch (iOutcome)
                {
                    case 0:
                        sOutcome = "Failed";
                        txtLastRunOutcome.CssClass = "disabled";
                        break;
                    case 1:
                        sOutcome = "Succeeded";
                        txtLastRunOutcome.CssClass = "enabled";
                        break;
                    case 3:
                        sOutcome = "Canceled";
                        txtLastRunOutcome.CssClass = "disabled";
                        break;
                    case 5:
                        sOutcome = "Unknown";
                        txtLastRunOutcome.CssClass = "disabled";
                        break;
                }
                txtLastRunOutcome.Text = sOutcome;
                txtLastRunOutcomeMsg.Value = Convert.ToString(drPackage["last_outcome_message"]);
                txtLastRunDuration.Text = Convert.ToString(drPackage["last_run_duration"]) + " seconds";
            }
            else
            {
                txtDescription.Text = "No schedule data found.";
            }
            drPackage.Close();
            cnSSIS.Close();
        }
        
        protected String fnIsJobEnabled(Boolean bIsEnabled)
        {
            String sReturnVal = "";
            if (bIsEnabled == true)
            {
                sReturnVal = "Job is enabled.";
                lblJobEnabledUpdated.CssClass = "enabled";
            }
            else
            {
                sReturnVal = "Job is disabled.";
                lblJobEnabledUpdated.CssClass = "disabled";
            }
            return sReturnVal;
        }

        protected String fnUpdateCheckJobEnabled()
        {
            String sReturnVal = "Data not saved.";
            String sPackageName = Master.selPackageText();
            txtSchedule.Text = sPackageName;
            Int16 iJobEnabled = Convert.ToInt16(chkIsJobEnabled.Checked == true);
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlCommand cmdPackage = new SqlCommand("[usp_UpdateJobEnabled]", cnSSIS);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobName = cmdPackage.Parameters.Add("@JobName", SqlDbType.VarChar);
            pJobName.Direction = ParameterDirection.Input;
            pJobName.Value = sPackageName;
            SqlParameter pEnabled = cmdPackage.Parameters.Add("@Enabled", SqlDbType.TinyInt);
            pEnabled.Direction = ParameterDirection.Input;
            pEnabled.Value = iJobEnabled;
            SqlParameter pRowCount = cmdPackage.Parameters.Add("@RowCount", SqlDbType.Int);
            pRowCount.Direction = ParameterDirection.Output;
            cnSSIS.Open();
            cmdPackage.ExecuteNonQuery();
            cnSSIS.Close();
            Int16 iRowCount = Convert.ToInt16(pRowCount.Value);
            if (iRowCount > 0)
            {
                sReturnVal = fnIsJobEnabled(Convert.ToBoolean(iJobEnabled > 0));
            }
            else
            {
                sReturnVal = "Data not saved.";
                lblJobEnabledUpdated.CssClass = "disabled";
            }
            lblJobEnabledUpdated.Text = sReturnVal;
            return sReturnVal;
        }

        protected void fnCheckJobEnabledChanged(Object sender, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }

        protected void btnUpdateJobEnabled_Click(Object obj, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }

        protected void btnRunJob_Click(Object obj, EventArgs e)
        {
            String sPackageName = Master.selPackageText();
            SSISExecutionService proxy = new SSISExecutionService();
            proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            SSISExecutionServiceProxy.ResultRecord result = proxy.executePackage(sPackageName);
            String sResult = Convert.ToString(result.DtsExecResult);
            lblJobResults.Text = sResult;
            if (sResult == "Success")
            {
                lblJobResults.CssClass = "enabled";
            }
            else
            {
                lblJobResults.CssClass = "disabled";
            }
        }
         
    }
}
