using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using etlMon.Common.Managers;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types;
using etlMon.Common.Utilities;
using NLog;

namespace etlMon
{
    public partial class ArchieMain : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {DateTime.Now.ToLocalTime()}");

            if (!IsPostBack) /* This is the first load of this page. */
            {
                try
                {
                    string sOperator = Request.LogonUserIdentity.Name;
                    if (Message1.Text.Length == 0) // This is the first load of this page so .
                    {
                        Message1.Text = sOperator + "&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                    }
                    var operatorName = Request.LogonUserIdentity.Name.Replace("CGI\\", "");
                }
                catch (Exception err)
                {
                    Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
                }
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void btnArchiveData_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnArchiveData_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            Response.Redirect("ArchieConfig.aspx", false);
            Logger.Info("btnArchiveData_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

    }
}