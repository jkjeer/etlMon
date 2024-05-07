using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace etlMon
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                //String sOperator = Request.LogonUserIdentity.Name;
                //Message1.Text = sOperator + "   " + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                //int iPackageID = 0;
                //fnGetPackages(iPackageID);
            }
        }

        protected void btnSSIS_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                Response.Redirect("SSIS.aspx");
//fnShowDiv("Config");
                //fnGetConfig();
            }
            else
            {
                //Message1.Text = "Data has NOT been saved.";
            }
        }

        protected void btnETL_Click(Object obj, EventArgs e)
        {
        }

        protected void btnMSTR_Click(Object obj, EventArgs e)
        {
        }
    }
}
