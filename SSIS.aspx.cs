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
using System.Drawing;

namespace etlMon
{
    public partial class SSIS : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");

        protected void Page_Load(object sender, EventArgs e)
        {
            gvHistory.HeaderStyle.BackColor = cHeaderColor;
            gvHistory.AlternatingRowStyle.BackColor = cAltRowColor;
            gvLogs.HeaderStyle.BackColor = cHeaderColor;
            gvLogs.AlternatingRowStyle.BackColor = cAltRowColor;
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                Int16 iPackageID = 0;
                fnGetPackages(iPackageID);
                fnLoadNumLogs();
            }
        }

        protected void fnGetPackages(int selectedPackageID)
        {
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", cnSSIS);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            selPackage.SelectedIndex = selectedPackageID;
            cnSSIS.Close();
        }

        protected void fnLoadNumLogs()
        {
            for (Int16 iIndex = 1; iIndex <= 10; iIndex++)
            {
                selNumLogs.Items.Add(Convert.ToString(iIndex));
	        }
            selNumLogs.Items.Add(Convert.ToString(25));
            selNumLogs.Items.Add(Convert.ToString(50));
            selNumLogs.Items.Add(Convert.ToString(100));
            selNumLogs.Items.Add(Convert.ToString(1000));
            selNumLogs.SelectedIndex = 9;
        }

        protected void fnPackageChanged(Object sender, EventArgs e)
        {
            if (Convert.ToInt16(selPackage.SelectedValue) == 0)
            {
                fnShowDiv("None");
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.White;
                btnSchedule_Click(sender, e);
            }
        }
                                
        protected void fnNumLogsChanged(Object sender, EventArgs e)
        {
            btnLogs_Click(sender, e);
        }

        protected void fnShowDiv(String sDivType)
        {
            divNavButtons.Visible = false;
            divConfig.Visible = false;
            divLogs.Visible = false;
            divSchedule.Visible = false;
            divHistory.Visible = false;
            divRunJob.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            lblJobEnabledUpdated.Text = "";
            switch (sDivType)
            {
                case "Schedule":
                    divNavButtons.Visible = true;
                    divSchedule.Visible = true;
                    break;
                case "Logs":
                    divNavButtons.Visible = true;
                    divLogs.Visible = true;
                    break;
                case "Config":
                    divNavButtons.Visible = true;
                    divConfig.Visible = true;
                    break;
                case "History":
                    divNavButtons.Visible = true;
                    divHistory.Visible = true;
                    break;
                case "RunJob":
                    divNavButtons.Visible = true;
                    divRunJob.Visible = true;
                    break;
            }
        }

        protected void fnGetPackage(int selectedPackageID)
        {
            String connectionString = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(connectionString);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", cnSSIS);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            selPackage.SelectedIndex = selectedPackageID;
            cnSSIS.Close();
        }

        protected void btnSchedule_Click(Object obj, EventArgs e)
        {
            if (selPackage.SelectedIndex > 0 )
            {
                fnShowDiv("Schedule");
                fnGetSchedule();
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.Pink;
            }
        }
        
        protected void btnConfiguration_Click(Object obj, EventArgs e)
        {
            if (selPackage.SelectedIndex > 0)
            {
                fnShowDiv("Config");
                fnGetConfig();
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.Pink;
            }
        }

        protected void btnHistory_Click(Object obj, EventArgs e)
        {
            if (selPackage.SelectedIndex > 0)
            {
                fnShowDiv("History");
                fnGetJobHistory();
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.Pink;
            }
        }

        protected void btnLogs_Click(Object obj, EventArgs e)
        {
            Int16 iNumLogs = 1;
            if (selPackage.SelectedIndex > 0)
            {
                fnShowDiv("Logs");
                iNumLogs = Convert.ToInt16(selNumLogs.Text);
                fnGetLogs(iNumLogs);
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.Pink;
            }
        }

        protected void btnUpdateJobEnabled_Click(Object obj, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }

        protected void fnCheckJobEnabledChanged(Object sender, EventArgs e)
        {
            fnUpdateCheckJobEnabled();
        }

        protected void btnRunNow_Click(Object obj, EventArgs e)
        {
            String sPackageName = selPackage.SelectedItem.Text;
            String sOperator = Request.LogonUserIdentity.Name;
            String sEmail = sOperator.Substring(4) + "@cardinalcorp.com";
            fnSetTempEmail(sPackageName, sEmail);
            SSISExecutionService esProxy = new SSISExecutionService();
            esProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            lblJobResults.Text = "";
            updRunResults.Update();
            //esProxy.executePackageCompleted += new executePackageCompletedEventHandler(OnExecutePackageCompleted);
            //esProxy.executePackageAsync(sPackageName);
            //String sResult = "Running"; //Convert.ToString(result.DtsExecResult);

            SSISExecutionServiceProxy.ResultRecord result = esProxy.executePackage(sPackageName);
            String sResult = Convert.ToString(result.DtsExecResult);
            lblJobResults.Text = sResult;
            if (sResult == "Success")
            {
                lblJobResults.ForeColor = System.Drawing.Color.Green; 
            }
            else
            {
                lblJobResults.ForeColor=System.Drawing.Color.Red; 
            }           
            updRunResults.Update();
        }

        protected void OnExecutePackageCompleted(object Source, executePackageCompletedEventArgs e)
        {
            lblJobResults.Text = "Complete";
            updRunResults.Update();
        }

        protected void btnRunJob_Click(Object obj, EventArgs e)
        {
            if (selPackage.SelectedIndex > 0)
            {
                fnShowDiv("RunJob");
                fnGetJobHistory();
            }
            else
            {
                selPackage.BackColor = System.Drawing.Color.Pink;
            }
        }

        protected void fnSetTempEmail(String sPackageName, String sEmail)
        {
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlCommand cmdPackage = new SqlCommand("[usp_SetConfigurationRecord]", cnSSIS);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            SqlParameter pConfigurationFilter = cmdPackage.Parameters.Add("@ConfigurationFilter", SqlDbType.VarChar);
            pConfigurationFilter.Direction = ParameterDirection.Input;
            pConfigurationFilter.Value = sPackageName;
            SqlParameter pPackagePath = cmdPackage.Parameters.Add("@PackagePath", SqlDbType.VarChar);
            pPackagePath.Direction = ParameterDirection.Input;
            pPackagePath.Value = "User::sEmailCompleteSendToTemp";
            SqlParameter pConfiguredValue = cmdPackage.Parameters.Add("@ConfiguredValue", SqlDbType.VarChar);
            pConfiguredValue.Direction = ParameterDirection.Input;
            pConfiguredValue.Value = sEmail;
            SqlParameter pConfiguredValueType = cmdPackage.Parameters.Add("@ConfiguredValueType", SqlDbType.VarChar);
            pConfiguredValueType.Direction = ParameterDirection.Input;
            pConfiguredValueType.Value = "String";
            cnSSIS.Open();
            cmdPackage.ExecuteNonQuery();
            cnSSIS.Close();
        }
    
        protected void fnGetConfig()
        {
            String sPackageName = selPackage.SelectedItem.Text;
             //txtConfig.Text = sPackageName;
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
            String sConfig = "" + Environment.NewLine;
            while (drConfig.Read())
            {
                String sType = Convert.ToString(drConfig["ConfiguredValueType"]);
                switch (sType)
                {
                    case "Int16":
                    case "Int32":
                        sConfig = sConfig +
                        "        <div class=\"fieldwrapper\">" + Environment.NewLine +
                        "            <label for=\"txt" + drConfig["PackagePath"] + "\" class=\"styled\">" + drConfig["Variable"] + ": " + "</label>" + Environment.NewLine +
                        "            <div class=\"thefieldNarrow\">" + Environment.NewLine +
                        "                <input name=\"txt" + drConfig["PackagePath"] + "\" id=\"txt" + drConfig["PackagePath"] + "\" type=\"text\" readonly=\"readonly\" value=\"" + drConfig["Value"] + "\" />" + Environment.NewLine +
                        "            </div>" + Environment.NewLine +
                        "        </div>" + Environment.NewLine;
                        break;
                    case "Boolean":
                        String sChecked = "";
                        if ( Convert.ToBoolean(drConfig["Value"]))
                        {
                            sChecked = "checked";
                        }
                        sConfig = sConfig +
                        "        <div class=\"fieldwrapper\">" + Environment.NewLine +
                        "            <label for=\"chk" + drConfig["PackagePath"] + "\" class=\"styled\">" + drConfig["Variable"] + ": " + "</label>" + Environment.NewLine +
                        "            <div class=\"thefield\">" + Environment.NewLine +
                        "                <input name=\"chk" + drConfig["PackagePath"] + "\" id=\"chk" + drConfig["PackagePath"] + "\" type=\"checkbox\" readonly=\"readonly\" " + sChecked + " />" + Environment.NewLine +
                        "            </div>" + Environment.NewLine +
                        "        </div>" + Environment.NewLine;
                        break;
                    default:
                        sConfig = sConfig +
                        "        <div class=\"fieldwrapper\">" + Environment.NewLine +
                        "            <label for=\"txt" + drConfig["PackagePath"] + "\" class=\"styled\">" + drConfig["Variable"] + ": " + "</label>" + Environment.NewLine +
                        "            <div class=\"thefieldWide\">" + Environment.NewLine +
                        "                <input name=\"txt" + drConfig["PackagePath"] + "\" id=\"txt" + drConfig["PackagePath"] + "\" type=\"text\" readonly=\"readonly\" value=\"" + drConfig["Value"] + "\" />" + Environment.NewLine +
                        "            </div>" + Environment.NewLine +
                        "        </div>" + Environment.NewLine;
                        break;
                }
            }
            divConfigFields.InnerHtml = sConfig;
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
            //txtLogs.Text = sPackageName;
            //WebMsgBox.Show(sPackageName); 
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drLogs = null;
            String sProc = "";
            if ( Master.fnIsIPhoneUser() )
            {
                sProc = "usp_GetLastLogs";
            }
            else
            {
                sProc = "usp_GetLastLogsVerbose";
            }
            SqlCommand cmdLogs = new SqlCommand(sProc, cnSSIS);
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

        protected void fnClearSchedule()
        {
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
        }

        protected void fnHideSchedule()
        {
            /* Initialize controls. */
            txtDescription.Visible = true;
            txtSchedName.Visible = false;
            chkIsJobEnabled.Visible = false;
            lblJobEnabledUpdated.Visible = false;
            txtNextRunDate.Visible = false;
            txtLastRunDate.Visible = false;
            txtLastRunOutcome.Visible = false;
            txtLastRunOutcomeMsg.Visible = false;
            txtLastRunDuration.Visible = false;
        }

        protected void fnShowSchedule()
        {
            /* Initialize controls. */
            txtDescription.Visible = true;
            txtSchedName.Visible = true;
            chkIsJobEnabled.Visible = true;
            lblJobEnabledUpdated.Visible = true;
            txtNextRunDate.Visible = true;
            txtLastRunDate.Visible = true;
            txtLastRunOutcome.Visible = true;
            txtLastRunOutcomeMsg.Visible = true;
            txtLastRunDuration.Visible = true;
        }
   
        protected void fnGetSchedule()
        {
            String sPackageName = selPackage.SelectedItem.Text;
            //txtSchedule.Text = sPackageName;
            /* Initialize controls. */
            fnClearSchedule();
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
                fnShowSchedule();
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
                fnHideSchedule();
                txtDescription.Text = "No schedule data found.";
            }
            drPackage.Close();
            cnSSIS.Close();
        }

        protected String fnUpdateCheckJobEnabled()
        {
            String sReturnVal = "Data not saved.";
            String sPackageName = selPackage.SelectedItem.Text;
            //txtSchedule.Text = sPackageName;
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
                sReturnVal = "Job schedule is enabled.";
                lblJobEnabledUpdated.CssClass = "enabled";
            }
            else
            {
                sReturnVal = "Job schedule is disabled.";
                lblJobEnabledUpdated.CssClass = "disabled";
            }
            return sReturnVal;
        }

        protected void UpdateButton_Click(Object obj, EventArgs e)
        {
            System.Threading.Thread.Sleep(5000);
        }


    }
}
