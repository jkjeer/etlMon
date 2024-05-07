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
    public partial class ArchieConfig : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string _sortDirection = "ASC";
        DataTable dt = new DataTable();

        SqlConnection cnArchRepo = ConnectionManager.GetSqlConnectionFromKey("ArchieRepositoryServer");

        Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        Color cHeaderColor = ColorTranslator.FromHtml("#FFFCCCC");

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {DateTime.Now.ToLocalTime()}");

            liArchive.Text = $"Archive Data to Corp - {cnArchRepo.DataSource}";

            gvArcConfigs.HeaderStyle.BackColor = cHeaderColor;
            gvArcConfigs.AlternatingRowStyle.BackColor = cAltRowColor;

            gvTailConfigs.HeaderStyle.BackColor = cHeaderColor;
            gvTailConfigs.AlternatingRowStyle.BackColor = cAltRowColor;

            gvDeleteLogs.HeaderStyle.BackColor = cHeaderColor;
            gvDeleteLogs.AlternatingRowStyle.BackColor = cAltRowColor;

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
                    //Plant plant = GetOperatorPlant(operatorName);
                    //GetGroups();
                    fnSelDivs(radListTask.SelectedValue);
                }
                catch (Exception err)
                {
                    Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
                }
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

        }

        protected Int16 GetGroups()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetGroups BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            var cnArchCfg = ConnectionManager.GetSqlConnectionFromKey("ArchieConfigServer");
            cnArchCfg.Open();
            SqlDataAdapter sda = new SqlDataAdapter("[usp_GetGroups]", cnArchCfg);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            Logger.Info(Logging.DbCmdString(sda));
            sda.Fill(ds, "Groups");
            //selGroup.DataSource = ds;
            //selGroup.DataTextField = "GroupName";
            //selGroup.DataValueField = "GroupID";
            //selGroup.DataBind();
            //cnArchCfg.Close();
            //Logger.Info($"Rows: {selGroup.Items.Count}");
            Logger.Info("GetGroups END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return 0;
        }

        protected void SetSortDirection(string sortDirection)
        {
            if (sortDirection == "ASC")
            {
                _sortDirection = "DESC";
            }
            else
            {
                _sortDirection = "ASC";
            }
        }

        protected void ClearGrid()
        {
            divGridArchive.Visible = false;
            gvConfigs.DataSource = null;
            gvConfigs.DataBind();
        }

        protected void fnRadListTaskChanged(Object sender, EventArgs e)
        {
            fnSelDivs(radListTask.SelectedValue);
        }

        protected void fnEMailChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcServerChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcDatabaseChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcSchemaChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcTableChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcKeyColumnChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcDateColumnChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnArcRetentionDaysChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailServerChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailDatabaseChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailSchemaChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailTableChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailKeyColumnChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailDateColumnChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnTailRetentionDaysChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnDelServerChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnDelNLogRetentionDaysChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnDelIISLogRetentionDaysChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnDelScriptLogRetentionDaysChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }


        protected void btnGetArchiveConfigs_Click(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void btnCheckAll_Click(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void btnUncheckAll_Click(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void btnExport_Click(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void chkDocTypes_Changed(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }
        
        protected void gvConfigs_Sorting(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void gvArcConfigs_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                IDictionary<int, bool> documents = new Dictionary<int, bool>();
                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    documents.Add(document, gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked);
                }

                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                SetSortDirection(_sortDirection);

                gvDeleteLogs.DataSource = dtrslt;
                gvDeleteLogs.DataBind();

                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    if (documents.TryGetValue(document, out bool include))
                    {
                        if (gvRow.FindControl("chkInclude") is CheckBox checkbox) checkbox.Checked = include;
                    }
                }
                //GetCountSelectedDocs();

            }
        }

        protected void gvTailConfigs_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                IDictionary<int, bool> documents = new Dictionary<int, bool>();
                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    documents.Add(document, gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked);
                }

                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                SetSortDirection(_sortDirection);

                gvDeleteLogs.DataSource = dtrslt;
                gvDeleteLogs.DataBind();

                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    if (documents.TryGetValue(document, out bool include))
                    {
                        if (gvRow.FindControl("chkInclude") is CheckBox checkbox) checkbox.Checked = include;
                    }
                }
                //GetCountSelectedDocs();

            }
        }

        protected void gvDeleteLogs_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                IDictionary<int, bool> documents = new Dictionary<int, bool>();
                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    documents.Add(document, gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked);
                }

                if (Convert.ToString(ViewState["sortdr"]) == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                SetSortDirection(_sortDirection);

                gvDeleteLogs.DataSource = dtrslt;
                gvDeleteLogs.DataBind();

                foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    if (documents.TryGetValue(document, out bool include))
                    {
                        if (gvRow.FindControl("chkInclude") is CheckBox checkbox) checkbox.Checked = include;
                    }
                }
                //GetCountSelectedDocs();

            }
        }

        protected void chkInclude_Changed(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnGroupChanged(Object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnGroupChanged BEGIN: {DateTime.Now.ToLocalTime()}");
            //ClearGrid();
            //GetCustomers();
            //divGo.Visible = true; //(selCustomer.Items.Count > 0);
            Logger.Info("fnGroupChanged END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void fnSelDivs(string selValue)
        {
            if (selValue == "ArchiveData")
            {
                divArchiveData.Visible = true;
                divTailData.Visible = false;
                divDeleteLogs.Visible = false;
                //divGo.Visible = (selGroup.Items.Count > 0 && selGroup.Items.Count > 0);
            }
            else if (selValue == "TailData")
            {
                divArchiveData.Visible = false;
                divTailData.Visible = true;
                divDeleteLogs.Visible = false;
                //divGo.Visible = true; //(txtFromInvoice.Text.Length > 0); // && txtToInvoice.Text.Length> 0);
            }
            else if (selValue == "DeleteLogs")
            {
                divArchiveData.Visible = false;
                divTailData.Visible = false;
                divDeleteLogs.Visible = true;
                //divGo.Visible = true; //(txtInvoiceList.Text.Length > 0);
                fnGetDeleteLogConfigs();
            }
        }

        protected void fnGetDeleteLogConfigs()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnGetDeleteLogConfigs BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            //if (fnValidate(plantCode, custNum, startDate, endDate, txtEmail.Text, startInvoice, endInvoice, invoiceList))
            //{
            SqlConnection cnArchCfg = ConnectionManager.GetSqlConnectionFromKey("ArchieConfigServer");
            cnArchCfg.Open();

            var cmd = new SqlCommand()
            {
                Connection = cnArchCfg,
                CommandTimeout = 0,
                CommandType = CommandType.Text,
                CommandText = $@"
                    SELECT [ID]
                          ,[GroupName]
                          ,[PlantCode]
                          ,[ServerName]
                          ,[IsActive]
                          ,[IsEnabled]
                          ,[IsRunning]
                          ,[LogRetentionDays]
                          ,[DeleteNLog]
                          ,[DeleteIISLog]
                          ,[DeleteScriptLog]
                          ,[CreatedDate]
                          ,[CreatedBy]
                          ,[LastEditBy]
                          ,[BITimeStamp]
                      FROM [Archiver].[dbo].[cfgServersLogDelete]
                    ORDER BY [ServerName]
                "
            };

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            Logger.Info(Logging.DbCmdString(sda));
            try
            {
                sda.Fill(dt);
                gvDeleteLogs.DataSource = dt;
                gvDeleteLogs.DataBind();
                ViewState["dirState"] = dt;
                ViewState["sortdr"] = "Asc";
                //if (dt.Rows.Count > 0)
                //{
                //    txtQueryResults.Text = dt.Rows.Count == 1 ? Convert.ToString(dt.Rows.Count) + " Document found" : Convert.ToString(dt.Rows.Count) + " Documents found";
                //    btnExport.Visible = true;
                //    // If there are more than one Doc Type then give them check boxes
                //    List<string> docTypes = new List<string>();
                //    foreach (GridViewRow gvRow in gvDeleteLogs.Rows)
                //    {
                //        var docType = gvRow.Cells[1].Text;
                //        if (gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked) if (!docTypes.Contains(docType)) docTypes.Add(docType);
                //    }
                //    chkDocTypes.Items.Clear();
                //    if (docTypes.Count > 1)
                //    {
                //        var i = 0;
                //        foreach (var docType in docTypes)
                //        {
                //            chkDocTypes.Items.Add(docType);
                //            chkDocTypes.Items[i].Selected = true;
                //            i++;
                //        }
                //    }
                //    divDocTypes.Visible = docTypes.Count > 1;
                //}
                //else
                //{
                //    txtQueryResults.Text = "No Invoices found";
                //    btnExport.Visible = false;
                //}
                //divJobResults.Visible = false;
                //gvDeleteLogs.Visible = true;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                lblError.Text = "ERROR: " + e.Message;
                lblError.Visible = true;
                divError.Visible = true;
            }

            cnArchCfg.Close();
            Logger.Info($"Rows: {dt.Rows.Count}");
            //fnInclude_Changed();
            gvDeleteLogs.Visible = true;
            //}
            Logger.Info("fnGetDeleteLogConfigs END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected Plant GetOperatorPlant(string operatorName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetOperatorPlant BEGIN: {DateTime.Now.ToLocalTime()}");
            Plant plant = new Plant();
            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            cnDW.Open();
            try
            {
                var displayname = "";
                var description = "";
                var mail = "";
                var userprincipalname = "";
                var distinguishedname = "";
                var memberof = "";

                using (IDataReader reader = GetOperatorPlant(cnDW, operatorName))
                {
                    while (reader.Read())
                    {
                        displayname = CastDbNull.To(reader["displayname"], string.Empty);
                        description = CastDbNull.To(reader["description"], string.Empty);
                        mail = CastDbNull.To(reader["mail"], string.Empty);
                        userprincipalname = CastDbNull.To(reader["userprincipalname"], string.Empty);
                        distinguishedname = CastDbNull.To(reader["distinguishedname"], string.Empty);
                        memberof = CastDbNull.To(reader["memberof"], string.Empty);
                    }
                    reader.Close();
                }

                Logger.Info($"userprincipalname: {userprincipalname}");
                //if (txtEmail.Text.Length == 0) txtEmail.Text = userprincipalname;
                var plantCode = displayname.Substring(displayname.Length - 4, 4);
                plant = PlantManager.GetPlantNull(plantCode);
                Logger.Info($"plant: {plant}");
                if (plant == null)
                {
                    Logger.Info($"distinguishedname OU=[A-Z][A-Z][A-Z][A-Z]: {distinguishedname}");
                    string pattern = @"OU=[A-Z][A-Z][A-Z][A-Z],";
                    Regex rg = new Regex(pattern);
                    var plantCodes = rg.Matches(distinguishedname);
                    for (int count = 0; count < plantCodes.Count; count++)
                    {
                        plantCode = plantCodes[count].Value.Replace("OU=", "").Replace(",", "");
                        plant = PlantManager.GetPlantNull(plantCode);
                        if (plant != null) count = plantCodes.Count;
                        Logger.Info($"plant: {plant}");
                    }
                    if (plant == null)
                    {
                        Logger.Info($"distinguishedname OU=Corporate: {distinguishedname}");
                        pattern = @"OU=Corporate,";
                        rg = new Regex(pattern);
                        plantCodes = rg.Matches(distinguishedname);
                        for (int count = 0; count < plantCodes.Count; count++)
                        {
                            plantCode = plantCodes[count].Value.Replace("OU=", "").Replace(",", "");
                            plant = PlantManager.GetPlantNull(plantCode);
                            if (plant != null) count = plantCodes.Count;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            cnDW.Close();
            Logger.Info("GetOperatorPlant END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return plant;
        }

        public static IDataReader GetOperatorPlant(SqlConnection cnDW, string operatorName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnDW,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT [samaccountname]
                              ,[displayname]
                              ,[description]
                              ,[userprincipalname]
                              ,[mail]
                              ,[distinguishedname]
                              ,[memberof]
                          FROM [PSBI_WH_PROD].[PROD].[ACTIVE_DIRECTORY]
                          WHERE [samaccountname] = @OperatorName"
                };
                // Plant Num
                cmd.Parameters.Add(new SqlParameter("@OperatorName", SqlDbType.NVarChar, 100));
                cmd.Parameters["@OperatorName"].Value = operatorName;
                // Reader
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleRow);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }
    }
}