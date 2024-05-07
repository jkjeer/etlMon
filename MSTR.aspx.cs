using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace etlMon
{
    public partial class MSTR : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                fnShowDiv("divStatus");
                fnPageLoad();
            }
        }

        protected void fnPageLoad()
        {
            txtBatchStatus.Text = fnGetRunTime();
            txtSQL.Value = "SELECT *" + "\r" +
                "FROM [PSBI_WH_PROD].[dbo].[ETL_LAST_RUN_DATE]" + "\r" +
                "WHERE [JOB_DESC] = 'MAIN_NIGHTLY_BATCH'" + "\r" +
                "AND DATEDIFF(d,[LAST_RUN_DATE],GETDATE()) = 0";
            fnGetLogs();
        }

        protected String fnGetRunTime()
        {
            String sRunTime = "";
            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            String sQry = "SELECT [LAST_RUN_DATE] FROM [PSBI_WH_PROD].[dbo].[ETL_LAST_RUN_DATE] WHERE [JOB_DESC] = 'MAIN_NIGHTLY_BATCH' AND DATEDIFF(d,[LAST_RUN_DATE],GETDATE()) = 0";
            SqlCommand cmdPackage = new SqlCommand(sQry, cnDW);
            cmdPackage.CommandType = CommandType.Text;
            SqlDataReader drItems = null;
            cnDW.Open();
            drItems = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drItems.Read())
            {
                divUpdate.Visible = false;
                //txtSQL.Visible = false;
                //txtBatchStatus.BackColor =  
                sRunTime = Convert.ToString(drItems["LAST_RUN_DATE"]);
            }
            else
            {
                divUpdate.Visible = true;
                //txtSQL.Visible = true;
                sRunTime = "";
            }
            drItems.Close();
            cnDW.Close();
            return sRunTime;
        }

        protected void fnGetLogs()
        {
            /* Initialize controls. */
            //WebMsgBox.Show(sPackageName); 
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection cnSSIS = new SqlConnection(csSSIS);
            SqlDataReader drHistory = null;
            String sQry = "SELECT JobDesc, Status, RunTime FROM [SSIS].[dbo].[MicroStrategyLog] WHERE CONVERT(CHAR(10), [RunTime], 23) = CONVERT(CHAR(10), GETDATE(), 23)";
            SqlCommand cmdPackage = new SqlCommand(sQry, cnSSIS);
            cmdPackage.CommandType = CommandType.Text;
            cnSSIS.Open();
            drHistory = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            gvLog.DataSource = drHistory;
            gvLog.DataBind();
            drHistory.Close();
            cnSSIS.Close();
        }

        protected void btnUpdate_Click(Object obj, EventArgs e)
        {
            fnShowDiv("divUpdateNow");
        }

        protected void btnUpdateNow_Click(Object obj, EventArgs e)
        {
            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            String sQry = "UPDATE [PSBI_WH_PROD].[dbo].[ETL_LAST_RUN_DATE] SET [LAST_RUN_DATE] = GETDATE() WHERE [JOB_DESC] = 'MAIN_NIGHTLY_BATCH' AND [JOB_ID] = 10";
            SqlCommand cmdPackage = new SqlCommand(sQry, cnDW);
            cmdPackage.CommandType = CommandType.Text;
            cnDW.Open();
            cmdPackage.ExecuteNonQuery();
            cnDW.Close();
            txtBatchStatus.Text = fnGetRunTime();
            fnShowDiv("divStatus");
            fnPageLoad();
        }

        protected void btnRefresh_Click(Object obj, EventArgs e)
        {
            fnShowDiv("divStatus");
            fnPageLoad();
        }
        
        protected void fnShowDiv(String sDivType)
        {
            //divNavButtons.Visible = false;
            divStatus.Visible = false;
            divLog.Visible = false;
            divUpdateNow.Visible = false;
            switch (sDivType)
            {
                case "divUpdateNow":
                    divUpdateNow.Visible = true;
                    break;
                case "divStatus":
                    //divNavButtons.Visible = true;
                    divStatus.Visible = true;
                    divLog.Visible = true;
                    break;
                case "None":
                    //divNavButtons.Visible = true;
                    break;
            }
        }

    }


}
