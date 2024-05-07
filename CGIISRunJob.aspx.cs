using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Data;
using System.Data.SqlClient;
using Cardinal.Components.WcfClient;
using etlMon.ServiceReference1;

namespace etlMon
{
    public partial class CGIISRunJob : System.Web.UI.Page
    {
        String sLocation = @"\\CGD1BIETL1\S$\Scripts\CGIIS\";
        String sChooseJob = "Choose job:";

        protected void Page_Load(object sender, EventArgs e)
        {
            //ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            //scriptManager.RegisterPostBackControl(this.btnRunNow);

            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                Int16 iJobID = 0;
                fnGetJobs(iJobID);
                fnLoadStartDays();
                fnLoadEndDays();
            }
        }

        protected void fnGetJobs(int selectedJobID)
        {
            String sFileName = "";
            String sFileExt = "";
            Int16 iFileID = 0;
            ListItemCollection lJobsList = new ListItemCollection();
            lJobsList.Add(new ListItem(sChooseJob, iFileID.ToString()));
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sLocation);
            foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
            {                
                //LOAD JOB NAMES 
                sFileName = f.Name;
                sFileExt = f.Extension.ToUpper();
                if (sFileExt.Equals(".XML") || sFileExt.Equals(".CMD"))
                {
                    if ( !sFileName.Contains("CGIIS-Config"))
                    {
                        iFileID += 1;
                        lJobsList.Add(new ListItem(sFileName, iFileID.ToString()));
                    }
                }
            }
            selJob.Items.AddRange(lJobsList.Cast<ListItem>().ToArray());
            selJob.SelectedIndex = selectedJobID;
        }

        protected void fnJobChanged(Object sender, EventArgs e)
        {
            if (Convert.ToInt16(selJob.SelectedValue) == 0)
            {
                fnShowDiv("None");
            }
            else
            {
                String sJobFileName = selJob.SelectedItem.Text;
                String sJobName = System.IO.Path.GetFileNameWithoutExtension(sJobFileName);
                litJobName.Text = sJobName;
                selJob.BackColor = System.Drawing.Color.White;
                fnShowDiv("divNavButtons");
            }
        }

        protected void btnRunNow_Click(Object obj, EventArgs e)
        {
            String sJobFileName = selJob.SelectedItem.Text;
            String sJobName = System.IO.Path.GetFileNameWithoutExtension(sJobFileName);
            String[] aJobName = { sJobName };
            fnLaunchJob(aJobName);
        }

        protected void fnLaunchJob(String[] aJobName)
        {
            Boolean sIntraday = chkIntraday.Checked;
            int sStartDays = Convert.ToInt16(selStartDays.SelectedItem.Text);
            int sEndDays = Convert.ToInt16(selEndDays.SelectedItem.Text);
            String sInvGUID = Guid.NewGuid().ToString();
            String sOperator = Request.LogonUserIdentity.Name;

            lblGuid.Text = sInvGUID;
            //updRunResults.Update();

            var binding = new MaxNetTcpBinding();
            binding.OpenTimeout = new TimeSpan(0, 10, 0);
            var uri = new System.Uri("net.tcp://cgd1bietl1/CGIIS/CGIIS.EtlService.svc");
            var endpoint = new EndpointAddress(uri, EndpointIdentity.CreateSpnIdentity(""));
            var channelFactory = new MaxChannelFactory<IEtlService>(binding, endpoint);
            var client = channelFactory.CreateChannel();

            String sResult = client.RunJob(aJobName, sIntraday, sStartDays, sEndDays, sInvGUID);

            lblJobResults.Text = fnGetJobLog(sInvGUID);
            updRunResults.Update();
            //fnShowDiv("None");
        }

        protected String fnGetJobLog(String sInvGUID)
        {
            String sReturnVal = "";

            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            SqlDataReader drHistory = null;
            SqlCommand cmdLog = new SqlCommand("[uspGetEventLogByClientGUID]", cnDW);
            cmdLog.CommandType = CommandType.StoredProcedure;
            SqlParameter pJobGUID = cmdLog.Parameters.Add("@SearchGuid", SqlDbType.VarChar);
            pJobGUID.Direction = ParameterDirection.Input;
            pJobGUID.Value = sInvGUID;
            cnDW.Open();
            drHistory = cmdLog.ExecuteReader(CommandBehavior.CloseConnection);
            gvLog.DataSource = drHistory;
            gvLog.DataBind();
            drHistory.Close();
            cnDW.Close();
            return sReturnVal;
        }

        protected void fnShowDiv(String sDivType)
        {
            divNavButtons.Visible = false;
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            switch (sDivType)
            {
                case "divNavButtons":
                    divNavButtons.Visible = true;
                    divUpdates.Visible = true;
                    break;
                case "None":
                    divNavButtons.Visible = false;
                    break;
            }
        }

        protected void fnLoadStartDays()
        {
            selStartDays.Items.Clear();
            selStartDays.Items.Add(Convert.ToString(-1));
            selStartDays.Items.Add(Convert.ToString(-2));
            selStartDays.Items.Add(Convert.ToString(-3));
            selStartDays.Items.Add(Convert.ToString(-5));
            selStartDays.Items.Add(Convert.ToString(-7));
            selStartDays.Items.Add(Convert.ToString(-10));
            selStartDays.Items.Add(Convert.ToString(-14));
            selStartDays.Items.Add(Convert.ToString(-30));
            selStartDays.Items.Add(Convert.ToString(-45));
            selStartDays.Items.Add(Convert.ToString(-60));
            selStartDays.Items.Add(Convert.ToString(-90));
            selStartDays.Items.Add(Convert.ToString(-180));
            selStartDays.Items.Add(Convert.ToString(-365));
            selStartDays.Items.Add(Convert.ToString(-1000));
            selStartDays.Items.Add(Convert.ToString(-10000));
            selStartDays.SelectedIndex = 0;
        }

        protected void fnLoadEndDays()
        {
            selEndDays.Items.Clear();
            selEndDays.Items.Add(Convert.ToString(1));
            selEndDays.Items.Add(Convert.ToString(2));
            selEndDays.Items.Add(Convert.ToString(3));
            selEndDays.Items.Add(Convert.ToString(5));
            selEndDays.Items.Add(Convert.ToString(7));
            selEndDays.Items.Add(Convert.ToString(15));
            selEndDays.Items.Add(Convert.ToString(30));
            selEndDays.SelectedIndex = 0;
        }
    }
}
