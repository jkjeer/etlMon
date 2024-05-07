using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace etlMon
{
    public partial class Switchboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page.
            {
                String sOperator = Request.LogonUserIdentity.Name;
                Message1.Text = sOperator + "   " + Convert.ToString(DateTime.Now); 
            }
        }
        
        protected void btnSSIS_Click(Object obj, EventArgs e)
        {
            Response.Redirect("SSIS.aspx");
        }

        protected void btnETL_Click(Object obj, EventArgs e)
        {
            Response.Redirect("ETL.aspx");
        }

        protected void btnMSTR_Click(Object obj, EventArgs e)
        {
            Response.Redirect("MSTR.aspx");
        }
        
    }
}
