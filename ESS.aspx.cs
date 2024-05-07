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
    public partial class ESS : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        String sTitleHeader = "ESS Data Imports";

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.AppendHeader("Refresh", 30 + "; URL=ESS.aspx");
            Label mpLabel = (Label)Master.FindControl("pageHeader");
            if (mpLabel != null)
            {
                mpLabel.Text = sTitleHeader;
            }
            if (!IsPostBack) // This is the first load of this page so 
            {
                fnGetLogs();
            }
        }

        protected void btnRefresh_Click(Object obj, EventArgs e)
        {
            fnGetLogs();
        }

        protected void fnGetLogs()
        {
            //WebMsgBox.Show(sPackageName); 
            String sMaxDateChanged = "";
            String sDays = "";
            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            SqlDataReader drHistory = null;
            SqlCommand cmdPackage = new SqlCommand("usp_GetESSImportMon", cnDW);
            cmdPackage.CommandType = CommandType.StoredProcedure;
            cnDW.Open();
            drHistory = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection);
            if (drHistory.Read())
            {
                sMaxDateChanged = Convert.ToString(drHistory["MaxDateChanged"]);
                sDays = Convert.ToString(drHistory["days"]);
                txtMaxDateChanged.Text = sMaxDateChanged;
                txtDays.Text = sDays;
            }
            else
            {
                txtMaxDateChanged.Text = "";
                txtDays.Text = "";
            }
            drHistory.Close();
            cnDW.Close();
        }

    }

}
