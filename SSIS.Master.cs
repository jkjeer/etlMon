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

namespace etlMon
{
    public partial class SSIS : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Int16 iPackageID = 0;
            Master.pageHeaderLabel("CGI SSIS Monitor");
            if (!IsPostBack) // This is the first load of this page so load the packages dropdown.
            {
                iPackageID = Convert.ToInt16(Session["iPackageID"]);
                fnGetPackages(iPackageID);
                selPackage.SelectedIndex = iPackageID;
            }
            else
            {
                iPackageID = Convert.ToInt16(Session["iPackageID"]);
            }
        }

        public String selPackageText()
        {
            String x = "";
            if (selPackage.SelectedItem != null)
            {
                x = selPackage.SelectedItem.Text; 
            }
            return x;
        } 

        protected void fnGetPackages(int selectedPackageID)
        {
            String csSSIS = Properties.Settings.Default.csSSIS;
            SqlConnection conn = new SqlConnection(csSSIS);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", conn);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            //selPackage.SelectedIndex = selectedPackageID;
        }

        protected void fnPackageChanged(Object sender, EventArgs e)
        {
            Int16 iPackageID = 0;
            iPackageID = Convert.ToInt16(selPackage.SelectedValue);
            if (iPackageID == 0)
            {
                Response.Redirect("SSIS.aspx");
            }
            else
            {
                Session["iPackageID"] = iPackageID;
                Session["sPackageName"] = Convert.ToString(selPackage.SelectedItem);
                btnSchedule_Click(sender,e);
            }
        }

        protected void fnGetPackage(int selectedPackageID)
        {
            String connectionString = Properties.Settings.Default.csSSIS;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlDataAdapter cmd = new SqlDataAdapter("uspGetActivePackages", conn);
            DataSet ds = new DataSet();
            cmd.Fill(ds, "Packages");
            selPackage.DataSource = ds;
            selPackage.DataTextField = "PackageName";
            selPackage.DataValueField = "ID";
            selPackage.DataBind();
            selPackage.SelectedIndex = selectedPackageID;
        }
        
        protected void btnConfiguration_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                //fnShowDiv("Config");
                //fnGetConfig();
            }
            else
            {
                //Message1.Text = "Data has NOT been saved.";
            }
    }

        protected void btnHistory_Click(Object obj, EventArgs e)
        {
            if (Page.IsValid)
            {
                //fnShowDiv("History");
                //fnGetJobHistory();
            }
            else
            {
               // Message1.Text = "";
            }
        }

        protected void btnLogs_Click(Object obj, EventArgs e)
        {
            Int16 iNumLogs = 1;
            if (Page.IsValid)
            {
                //fnShowDiv("Logs");
                //if (Convert.ToString(txtNumLogs.Text).Length > 0)
                //{
                //    iNumLogs = Convert.ToInt16(txtNumLogs.Text);
                //}
                //fnGetLogs(iNumLogs);
            }
            else
            {
                //Message1.Text = "";
            }
        }
        
        protected void btnSchedule_Click(Object obj, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                Response.Redirect("SSIS_Schedule.aspx");
                //fnShowDiv("Schedule");
                //fnGetSchedule();
            }
            else
            {
                //Message1.Text = "";
            }
        }

    }
}
