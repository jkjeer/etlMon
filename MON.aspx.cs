using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Data.Common;

namespace etlMon
{
    public partial class MON : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        String sTitleHeader = "E1 Integration Transfers";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (chkAutoRefresh.Checked)
            {
                //Response.AppendHeader("Refresh", 30 + "; URL=MON.aspx");
                //Response.AppendHeader("Refresh", 30 + "; URL=ErrLogs.aspx");
                Response.AppendHeader("Refresh", 30 + "; URL=E1.aspx");
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
                if (e.Row.Cells[6].Text == "2")
                    {
                    e.Row.ForeColor = System.Drawing.Color.Red;
                }
                else if (e.Row.Cells[6].Text == "3")
                {
                    e.Row.ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected void fnLoadLookBack()
        {
            selLookBack.Items.Add("2 minute");
            selLookBack.Items.Add("5 minutes");
            selLookBack.Items.Add("10 minutes");
            selLookBack.Items.Add("30 minutes");
            selLookBack.Items.Add("60 minutes");
            selLookBack.Items.Add("6 hours");
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

        protected void fnGetLogs()
        {
            String sLookBack = selLookBack.SelectedItem.Text;
            int iLookBackMinutes = 0;
            /* Initialize controls. */
            switch (sLookBack)
            {
                case "2 minute":
                    iLookBackMinutes = 2;
                    break;
                case "5 minutes":
                    iLookBackMinutes = 5;
                    break;
                case "10 minutes":
                    iLookBackMinutes = 10;
                    break;
                case "30 minutes":
                    iLookBackMinutes = 30;
                    break;
                case "60 minutes":
                    iLookBackMinutes = 60;
                    break;
                case "6 hours":
                    iLookBackMinutes = 3600;
                    break;
            }
            //WebMsgBox.Show(sPackageName); 
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataReader drHistory = null;
            SqlCommand cmdPackage = new SqlCommand("usp_GetDownloadsUploadsMon", cnStaging);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            SqlParameter pLookBack = cmdPackage.Parameters.Add("@LookBack", SqlDbType.Int);
            pLookBack.Direction = ParameterDirection.Input;
            pLookBack.Value = iLookBackMinutes;
            cnStaging.Open();
            drHistory = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drHistory.HasRows)
            {
                litNoErr.Text = "";
            }
            else
            {
                litNoErr.Text = "No errors logged";
            }
            gvHistory.DataSource = drHistory;
            gvHistory.DataBind();
            drHistory.Close();
            cnStaging.Close();
        }

    }

}
