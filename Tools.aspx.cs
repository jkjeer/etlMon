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

using System.Drawing;
using System.Collections;
using etlMon.Util;

namespace etlMon
{
    public partial class Tools : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                DateTime dtNow = DateTime.Now;
                String sCalendar = Convert.ToString(dtNow.ToString("MM/dd/yyyy"));
                txtJulianDate.Text = fnConvertToJulian(dtNow);
                txtCalendarDate.Text = sCalendar;
            }
        }

        protected void fnShowDiv(String sDivType)
        {
            divNavButtons.Visible = false;
            divJulian.Visible = false;
            divPing.Visible = false;
            switch (sDivType)
            {
                case "Julian":
                    divNavButtons.Visible = true;
                    divJulian.Visible = true;
                    break;
                case "Ping":
                    divNavButtons.Visible = true;
                    divPing.Visible = true;
                    break;
            }
        }

        protected void btnPing_Click(Object obj, EventArgs e)
        {
            fnShowDiv("Ping");
        }

        protected void btnJulian_Click(Object obj, EventArgs e)
        {
            fnShowDiv("Julian");
        }

        protected void btnTimer_Click(Object obj, EventArgs e)
        {
            Response.Redirect("Timer.aspx");
        }

        protected void btnJulianToCal_Click(Object obj, EventArgs e)
        {
            lblJulianError.Visible = false;
            lblJulianError.Text = "Please enter a valid Julian date between 1 and 199365.";
            txtJulianDate.BackColor = System.Drawing.Color.Pink;
            String sJul = Convert.ToString(txtJulianDate.Text);
            Int32 dtJul;
            if (Int32.TryParse(sJul, out dtJul))
            {
                if (dtJul > 1)
                {
                    if (dtJul < 199365)
                    {
                        DateTime dtCal = Convert.ToDateTime(fnConvertFromJulian(dtJul));
                        String sCal = dtCal.ToString("d");
                        txtCalendarDate.Text = sCal;
                        txtCalendarDate.BackColor = System.Drawing.Color.White;
                        txtJulianDate.BackColor = System.Drawing.Color.White;
                        lblJulianError.Text = "";
                    }
                }
            }
            lblJulianError.Visible = true;
        }

        protected void btnCalToJulian_Click(Object obj, EventArgs e)
        {
            lblJulianError.Visible = false;
            lblJulianError.Text = "Please enter a valid Calendar date between 1/1/1900 and 12/31/2099.";
            txtCalendarDate.BackColor = System.Drawing.Color.Pink;
            String sCal = Convert.ToString(txtCalendarDate.Text);
            DateTime dtCal;
            if (DateTime.TryParse(sCal, out dtCal))
            {
                if (dtCal > Convert.ToDateTime("1/1/1900"))
                {
                    if (dtCal < Convert.ToDateTime("12/31/2099"))
                    {
                        txtJulianDate.Text = Convert.ToString(fnConvertToJulian(dtCal));
                        txtJulianDate.BackColor = System.Drawing.Color.White;
                        txtCalendarDate.BackColor = System.Drawing.Color.White;
                        lblJulianError.Text = "";
                    }
                }
            }
            lblJulianError.Visible = true;
        }

        protected String fnConvertToJulian(DateTime dtCalendarDate)
        {
            String sSQL = "select dbo.fnConvertToJulian('" + Convert.ToString(dtCalendarDate) + "') AS 'Date'";
            String sDate = "";
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataReader drJulian = null;
            SqlCommand cmdJulian = new SqlCommand(sSQL, cnStaging);
            cmdJulian.CommandType = CommandType.Text;
            cnStaging.Open();
            drJulian = cmdJulian.ExecuteReader(CommandBehavior.CloseConnection);
            while (drJulian.HasRows)
            {
                sDate = Convert.ToString(drJulian["Date"]);
            }
            return sDate;
        }

        protected String fnConvertFromJulian(Int32 dtJulianDate)
        {
            String sSQL = "select dbo.fnConvertFromJulian(" + Convert.ToString(dtJulianDate) + ", 0) AS 'Date'";
            String sDate = "";
            String csStaging = Properties.Settings.Default.csStaging;
            SqlConnection cnStaging = new SqlConnection(csStaging);
            SqlDataReader drJulian = null;
            SqlCommand cmdJulian = new SqlCommand(sSQL, cnStaging);
            cmdJulian.CommandType = CommandType.Text;
            cnStaging.Open();
            drJulian = cmdJulian.ExecuteReader(CommandBehavior.CloseConnection);
            while (drJulian.HasRows)
            {
                sDate = Convert.ToString(drJulian["Date"]);
            }
            return sDate;
        }

        protected void btnPingServer_Click(Object obj, EventArgs e)
        {
            Ping p = new Ping();
            txtPingOutput2.Value = "";
            for (int i = 0; i < 3; i++)
            {
                txtPingOutput2.Value = txtPingOutput2.Value + p.PingHost(txtPingServer.Text) + Environment.NewLine;
            }
        }

    }
}
