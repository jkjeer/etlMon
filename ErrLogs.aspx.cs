using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace etlMon
{
    public partial class ErrLogs : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        String sTitleHeader = "E1 Integration Errors";

        protected void Page_Load(object sender, EventArgs e)
        {
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
                int iSecondsWait = 4;
                if (fnGetLogs() > 0)
                {
                    iSecondsWait = 30;
                }
                if (chkAutoRefresh.Checked)
                {
                    //Response.AppendHeader("Refresh", 30 + "; URL=ErrLogs.aspx");
                    Response.AppendHeader("Refresh", iSecondsWait + "; URL=MON.aspx");
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
                //selNumLogs.SelectedIndex = 9;
        }

        protected void btnRefresh_Click(Object obj, EventArgs e)
        {
            fnGetLogs();
        }
        
        protected void fnLookBackChanged(Object sender, EventArgs e)
        {
            fnGetLogs();
        }

        protected int fnGetLogs()
        {
            int iRetval = 0;
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
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataReader drHistory = null;
            SqlCommand cmdPackage = new SqlCommand("usp_GetErrLog", cnStaging);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            SqlParameter pLookBack = cmdPackage.Parameters.Add("@LookBack", SqlDbType.Int);
            pLookBack.Direction = ParameterDirection.Input;
            pLookBack.Value = iLookBackMinutes;
            cnStaging.Open();
            drHistory = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drHistory.HasRows)
            {
                litNoErr.Text = "";
                iRetval = 1;
            }
            else
            {
                litNoErr.Text = "No errors logged";
                iRetval = 0;
            }
            gvHistory.DataSource = drHistory;
            gvHistory.DataBind();
            drHistory.Close();
            cnStaging.Close();
            return iRetval;
        }

        protected void gvHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.ForeColor = System.Drawing.Color.Red;
            }
        }

    }

}
