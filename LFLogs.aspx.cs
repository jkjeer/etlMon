using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using etlMon.Common.Managers;
using NLog;

namespace etlMon
{
    public partial class LFLogs : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        String sTitleHeader = "LaserFiche Transfers";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (chkAutoRefresh.Checked)
            {
                Response.AppendHeader("Refresh", 30 + "; URL=LFLogs.aspx");
            }
            Label mpLabel = (Label)Master.FindControl("pageHeader");
            if (mpLabel != null)
            {
                mpLabel.Text = sTitleHeader;
            }
            gvHistory.HeaderStyle.BackColor = cHeaderColor;
            gvHistory.AlternatingRowStyle.BackColor = cAltRowColor;
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                fnLoadLookBack();
                fnGetLogs();
            }
        }

        protected void gvHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[0].Text == "Script Finalize")
                {
                    e.Row.BackColor = System.Drawing.Color.LightGray;
                }
            }
        }

        protected void fnLoadLookBack()
        {
            selLookBack.Items.Add("30 minutes");
            selLookBack.Items.Add("1 hour");
            selLookBack.Items.Add("2 hours");
            selLookBack.Items.Add("4 hours");
            selLookBack.Items.Add("8 hours");
            selLookBack.Items.Add("12 hours");
            selLookBack.Items.Add("24 hours");
            selLookBack.Items.Add("48 hours");
            selLookBack.Items.Add("96 hours");
            selLookBack.SelectedIndex = 4;
        }

        protected void btnRefresh_Click(Object obj, EventArgs e)
        {
            fnGetLogs();
        }
        
        protected void fnLookBackChanged(Object sender, EventArgs e)
        {
            fnGetLogs();
        }

        protected void fnGetLogs()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetLaserFicheData BEGIN: {DateTime.Now.ToLocalTime()}");
            SqlConnection cnCorpInt = new SqlConnection();
            try
            {

                String sLookBack = selLookBack.SelectedItem.Text;
                int iLookBackMinutes = 0;
                /* Initialize controls. */
                switch (sLookBack)
                {
                    case "30 minutes":
                        iLookBackMinutes = 30;
                        break;
                    case "1 hour":
                        iLookBackMinutes = 60;
                        break;
                    case "2 hours":
                        iLookBackMinutes = 120;
                        break;
                    case "4 hours":
                        iLookBackMinutes = 240;
                        break;
                    case "8 hours":
                        iLookBackMinutes = 480;
                        break;
                    case "12 hours":
                        iLookBackMinutes = 960;
                        break;
                    case "24 hours":
                        iLookBackMinutes = 1920;
                        break;
                    case "48 hours":
                        iLookBackMinutes = 3840;
                        break;
                    case "96 hours":
                        iLookBackMinutes = 7680;
                        break;
                }

                //WebMsgBox.Show(sPackageName); 
                cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                cnCorpInt.Open();
                SqlDataReader drHistory = null;
                SqlCommand cmdPackage = new SqlCommand("usp_GetLFLog", cnCorpInt);
                cmdPackage.CommandType = CommandType.StoredProcedure;
                SqlParameter pLookBack = cmdPackage.Parameters.Add("@LookBack", SqlDbType.Int);
                pLookBack.Direction = ParameterDirection.Input;
                pLookBack.Value = iLookBackMinutes;
                drHistory = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
                if (drHistory.HasRows)
                {
                    litNoLogs.Text = "";
                }
                else
                {
                    litNoLogs.Text = "No LF requests logged";
                }

                gvHistory.DataSource = drHistory;
                gvHistory.DataBind();
                drHistory.Close();
                cnCorpInt.Close();
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnCorpInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
        }
    }
}
