using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using etlMon.Common.Managers;

namespace etlMon
{
    public partial class LFEmails : System.Web.UI.Page
    {
        System.Drawing.Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        System.Drawing.Color cHeaderColor = System.Drawing.ColorTranslator.FromHtml("#FFFCCCC");
        String sTitleHeader = "Laser Fiche Emails";

        protected void Page_Load(object sender, EventArgs e)
        {
            Label mpLabel = (Label)Master.FindControl("pageHeader");
            if (mpLabel != null)
            {
                mpLabel.Text = sTitleHeader;
            }
            if (!IsPostBack) // This is the first load of this page so 
            {
                fnGetEmails();
            }
        }

        protected void fnGetEmails()
        {
            String sOperator = Request.LogonUserIdentity.Name;
            //String sConfiguredValue = "";
            //Int16 iIsEnabled = 0;
            //String csSSIS = Properties.Settings.Default.csSSIS;
            //SqlConnection cnSSIS = new SqlConnection(csSSIS);
            var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
            SqlDataAdapter daEmails = new SqlDataAdapter();
            SqlCommand cmdEmails = new SqlCommand("usp_GetLaserFicheEmailRecipient", cnCorpInt);
            cmdEmails.CommandType = CommandType.StoredProcedure;
            SqlParameter pUser = cmdEmails.Parameters.Add("@User", SqlDbType.VarChar);
            pUser.Direction = ParameterDirection.Input;
            pUser.Value = sOperator;
            daEmails.SelectCommand = cmdEmails;
            cnCorpInt.Open();

            DataSet dsEmails = new DataSet();
            daEmails.Fill(dsEmails, "Emails");
            // Bind all the active checklist atributes to a repeater.
            Repeater1.DataSource = dsEmails;
            Repeater1.DataBind();
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = e.Item.DataItem as DataRowView;
                CheckBox chkEmail = e.Item.FindControl("chkEmail") as CheckBox;
                chkEmail.Attributes.Add("internalID", row["ID"].ToString());
                chkEmail.Checked = Convert.ToBoolean(row["IsEnabled"]);
            }
        }

        protected void btnSave_Click(Object obj, EventArgs e)
        {
            fnSaveEmails();
        }

        protected int fnSaveEmails()
        {
            int iRetVal = 0;
            // Get the data from the form.
            foreach (RepeaterItem oItem in Repeater1.Items)
            {
                iRetVal += fnSaveEmail(oItem);
            }
            if (iRetVal > 0) // Make sure the checklist was saved.
            {
                msgDataSaved.Text = "Data has been saved.";
                Response.Redirect("LaserFiche.aspx");
            }
            else
            {
                msgDataSaved.Text = "Data has NOT been saved.";
            }
            return iRetVal;
        }

        protected int fnSaveEmail( RepeaterItem oItem)
        {
            int iRetVal = 0;
            Boolean bIsEnabled = false;
            CheckBox chkEmail = oItem.FindControl("chkEmail") as CheckBox;
            if (chkEmail.Checked) { bIsEnabled = true; }
            TextBox txtID = oItem.FindControl("txtID") as TextBox;
            int nEmailID = Convert.ToInt32(txtID.Text);

            // create a connection to a SQL database
            //String csSSIS = Properties.Settings.Default.csSSIS;
            //SqlConnection cnSSIS = new SqlConnection(csSSIS);
            var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
            SqlCommand cmd = new SqlCommand("usp_UpdateGetLaserFicheEmailRecipient", cnCorpInt);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pID = cmd.Parameters.Add("@ID", SqlDbType.Int);
            pID.Direction = ParameterDirection.Input;
            pID.Value = nEmailID;
            SqlParameter pIsEnabled = cmd.Parameters.Add("@IsEnabled", SqlDbType.Bit);
            pIsEnabled.Direction = ParameterDirection.Input;
            pIsEnabled.Value = bIsEnabled;
            //SqlParameter pRowCount = cmd.Parameters.Add("@RowCount", SqlDbType.Int);
            //pRowCount.Direction = ParameterDirection.Output;
            cnCorpInt.Open();
            cmd.ExecuteNonQuery();
            cnCorpInt.Close();
            iRetVal = 1; // (int)pRowCount.Value;
            return iRetVal;
        }

    }

}
