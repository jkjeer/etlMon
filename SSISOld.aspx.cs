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
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                String sOperator = Request.LogonUserIdentity.Name;
                Message1.Text = sOperator + "   " + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                int iPackageID = 0;
                fnGetPackages(iPackageID);
            }
        }

        protected void fnGetPackages(int selectedPackageID)
        {
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection conn = new SqlConnection(csSSIS);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", conn);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            selPackage.SelectedIndex = selectedPackageID;
        }

        protected void fnPackageChanged(Object sender, EventArgs e)
        {
            if (Convert.ToInt16(selPackage.SelectedValue) == 0)
            {
                fnShowDiv("None");
            }
            else
            {
                Page.Validate();
                btnSchedule_Click(sender,e);
            }
        }

        protected void fnCheckJobEnabledChanged(Object sender, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }        

        protected void fnGetPackage(int selectedPackageID)
        {
            String connectionString = Properties.Settings.Default.csSSIS;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", conn);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            selPackage.SelectedIndex = selectedPackageID;
        }
        
        protected void btnConfiguration_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                fnShowDiv("Config");
                fnGetConfig();
            }
            else
            {
                Message1.Text = "Data has NOT been saved.";
            }
    }

        protected void btnHistory_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                fnShowDiv("History");
                fnGetJobHistory();
            }
            else
            {
                Message1.Text = "";
            }
        }

        protected void btnLogs_Click(Object obj, EventArgs e)
        {
            Int16 iNumLogs = 1;
            if (Page.IsValid)
            {
                fnShowDiv("Logs");
                if (Convert.ToString(txtNumLogs.Text).Length > 0)
                {
                    iNumLogs = Convert.ToInt16(txtNumLogs.Text);
                }
                fnGetLogs(iNumLogs);
            }
            else
            {
                Message1.Text = "";
            }
        }
        
        protected void btnSchedule_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                fnShowDiv("Schedule");
                fnGetSchedule();
            }
            else
            {
                Message1.Text = "";
            }
        }

        protected void fnShowDiv(String sDivType)
        {
            divConfig.Visible = false;
            divLogs.Visible = false;
            divSchedule.Visible = false;
            divHistory.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            lblJobEnabledUpdated.Text = "";
            switch (sDivType)
            {
                case "Schedule":
                    divSchedule.Visible = true;
                    break;
                case "Logs":
                    divLogs.Visible = true;
                    break;
                case "Config":
                    divConfig.Visible = true;
                    break;
                case "History":
                    divHistory.Visible = true;
                    break;
            }
        }

        protected void btnUpdateJobEnabled_Click(Object obj, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }

        protected void btnRunJob_Click(Object obj, EventArgs e)
        {
            String sPackageName = selPackage.SelectedItem.Text;
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

        protected void fnGetConfig()
        {
            String sPackageName = selPackage.SelectedItem.Text;
            txtConfig.Text = sPackageName;
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drConfig = null;
            SqlCommand cmdConfig = new SqlCommand("usp_GetConfig", cnSSIS);
            cmdConfig.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobName = cmdConfig.Parameters.Add("@JobName", SqlDbType.VarChar);
            pJobName.Direction = ParameterDirection.Input;
            pJobName.Value = sPackageName;
            cnSSIS.Open();
            drConfig = cmdConfig.ExecuteReader(CommandBehavior.CloseConnection);
            gvConfig.DataSource = drConfig;
            gvConfig.DataBind();

            //String sConfig = "";
            //while (drConfig.Read())
            //{
            //    sConfig = sConfig + drConfig["PackagePath"] + " - " + drConfig["ConfiguredValue"] + " - " + drConfig["ConfiguredValueType"] + "<br/>";
            //}
            //divConfigValues.InnerHtml = sConfig;
            drConfig.Close();
            cnSSIS.Close();
        }

        protected void fnGetJobHistory()
        {
            String sPackageName = selPackage.SelectedItem.Text;
            txtHistory.Text = sPackageName;
            //WebMsgBox.Show(sPackageName); 
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drHistory = null;
            SqlCommand cmdHistory = new SqlCommand("usp_GetJobHistory", cnSSIS);
            cmdHistory.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobName = cmdHistory.Parameters.Add("@JobName", SqlDbType.VarChar);
            pJobName.Direction = ParameterDirection.Input;
            pJobName.Value = sPackageName;
            cnSSIS.Open();
            drHistory = cmdHistory.ExecuteReader(CommandBehavior.CloseConnection);
            //if (drLogs.Read())
            //{
            gvHistory.DataSource = drHistory;
            gvHistory.DataBind();
            //}
            drHistory.Close();
            cnSSIS.Close();
        }

        protected void fnGetLogs(Int16 iNumLogs)
        {
            String sPackageName = selPackage.SelectedItem.Text;
            txtLogs.Text = sPackageName;
            //WebMsgBox.Show(sPackageName); 
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drLogs = null;
            SqlCommand cmdLogs = new SqlCommand("usp_GetLastLogs", cnSSIS);
            cmdLogs.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobName = cmdLogs.Parameters.Add("@JobName", SqlDbType.VarChar);
            pJobName.Direction = ParameterDirection.Input;
            pJobName.Value = sPackageName;
            SqlParameter pNumLogs = cmdLogs.Parameters.Add("@NumLogs", SqlDbType.Int);
            pNumLogs.Direction = ParameterDirection.Input;
            pNumLogs.Value = iNumLogs;
            cnSSIS.Open();
            drLogs = cmdLogs.ExecuteReader(CommandBehavior.CloseConnection);
            //if (drLogs.Read())
            //{
                gvLogs.DataSource = drLogs;
                gvLogs.DataBind();
            //}
            drLogs.Close();
            cnSSIS.Close();
        }

        protected void fnGetSchedule()
        {
            String sPackageName = selPackage.SelectedItem.Text;
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

        protected String fnUpdateCheckJobEnabled()
        {
            String sReturnVal = "Data not saved.";
            String sPackageName = selPackage.SelectedItem.Text;
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

    }
}
