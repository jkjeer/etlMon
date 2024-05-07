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
            Page.Header.Title = "CGI Monitor"; 
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
            }
        }

        protected void btnArchie_Click(Object obj, EventArgs e)
        {
            Response.Redirect("ArchieConfig.aspx", false);
        }

        protected void btnMON_Click(Object obj, EventArgs e)
        {
            Response.Redirect("MON.aspx", false);
        }

        protected void btnE1_Click(Object obj, EventArgs e)
        {
            Response.Redirect("E1.aspx", false);
        }

        protected void btnETL_Click(Object obj, EventArgs e)
        {
            Response.Redirect("ETL.aspx", false);
        }

        protected void btnMSTR_Click(Object obj, EventArgs e)
        {
            Response.Redirect("MSTR.aspx", false);
        }
        
        protected void btnTools_Click(Object obj, EventArgs e)
        {
            Response.Redirect("Tools.aspx", false);
        }

        protected void btnServices_Click(Object obj, EventArgs e)
        {
            Response.Redirect("Services.aspx", false);
        }

        protected void btnNetwork_Click(Object obj, EventArgs e)
        {
            Response.Redirect("Network.aspx", false);
        }
        
        protected void btnForkLift_Click(Object obj, EventArgs e)
        {
            Response.Redirect("http://cgd2bi2/ForkLift/default.aspx");
        }

        protected void btnLFEmails_Click(Object obj, EventArgs e)
        {
            Response.Redirect("LFEmails.aspx");
        }

        protected void btnLaserFiche_Click(Object obj, EventArgs e)
        {
            Response.Redirect("laserfiche.aspx");
        }

        protected void btnTCCInvoices_Click(Object obj, EventArgs e)
        {
            Response.Redirect("TCCExportInvoices.aspx");
        }

        

        protected void btnTCCViewer_Click(Object obj, EventArgs e)
        {
            Response.Redirect("TCCViewer.aspx");
        }

        protected void btnTCCBOLs_Click(Object obj, EventArgs e)
        {
            Response.Redirect("TCCImportBOLs.aspx?guid=6EDDF3D0-88CF-4898-87EE-DC63781400D3");
        }

        protected void btnPYRefresh_Click(Object obj, EventArgs e)
        {
            Response.Redirect("pyrefresh.aspx");
        }

        protected void btnErrLogs_Click(Object obj, EventArgs e)
        {
            Response.Redirect("ErrLogs.aspx");
        }

        protected void btnLFLogs_Click(Object obj, EventArgs e)
        {
            Response.Redirect("LFLogs.aspx");
        }

        protected void btnFGLogs_Click(Object obj, EventArgs e)
        {
            Response.Redirect("FGLogs.aspx");
        }

        protected void btnIntradayLogs_Click(Object obj, EventArgs e)
        {
            Response.Redirect("IntradayLogs.aspx");
        }

        protected void btnRackViewExport_Click(Object obj, EventArgs e)
        {
            Response.Redirect("RackViewExport.aspx");
        }

        protected void btnESS_Click(Object obj, EventArgs e)
        {
            Response.Redirect("ESS.aspx");
        }

        protected void btnIGCustomerLogos_Click(Object obj, EventArgs e)
        {
            Response.Redirect("IGCustomerLogos.aspx");
        }

        protected void btnE1WOStatus_Click(Object obj, EventArgs e)
        {
            Response.Redirect("E1WOStatus.aspx");
        }
    }
}
