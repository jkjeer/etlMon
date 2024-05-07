using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace etlMon
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String sOperator = Request.LogonUserIdentity.Name;
            if (Message1.Text != sOperator) // This is not the first load of this page so .
            {
                Message1.Text = sOperator + "&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            }
            if (!IsPostBack) // This is the first load of this page so .
            {
            }
        }

        public Boolean fnIsIPhoneUser() 
        {
            Boolean bVal = false;
            bVal = HttpContext.Current.Request.UserAgent.ToLower().Contains("iphone"); 
            return bVal;
        }
    
        public void pageHeaderLabel(String val)
        {
            pageHeader.Text = val;
        } 

    }
}
