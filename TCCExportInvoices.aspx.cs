using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using etlMon.Common.Managers;
using etlMon.Common.Managers.E1;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types;
using etlMon.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace etlMon
{
    public partial class TCCExportInvoices : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string _sortDirection = "ASC";
        DataTable dt = new DataTable();
        private static string _plant = "";

        Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        Color cHeaderColor = ColorTranslator.FromHtml("#FFFCCCC");

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {DateTime.Now.ToLocalTime()}");
            gvHistory.HeaderStyle.BackColor = cHeaderColor;
            gvHistory.AlternatingRowStyle.BackColor = cAltRowColor;
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
                    Plant plant = GetOperatorPlant(operatorName);
                    GetPlants();
                    if (plant != null)
                    {
                        selPlant.Items.FindByText(plant.PlantCode).Selected = true;
                        GetCustomers();
                    }
                    txtFromDate.Text = Convert.ToString(System.DateTime.Today.Date.ToShortDateString());
                    txtToDate.Text = Convert.ToString(System.DateTime.Today.Date.ToShortDateString());
                    fnSelDivs(RadioButtonList1.SelectedValue);
                }
                catch (Exception err)
                {
                    Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
                }
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void btnExport_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnExport_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            var plantCode = _plant;
            var custNum = selCustomer.SelectedValue;
            var startDate = Convert.ToDateTime(txtFromDate.Text);
            var endDate = Convert.ToDateTime(txtToDate.Text);
            var startInvoice = txtFromInvoice.Text;
            var endInvoice = txtToInvoice.Text;
            var invoiceList = txtInvoiceList.Text;

            var bookingNum = txtBookingNum.Text;
            var bolNum = txtBOLNum.Text;
            var loadNum = txtLoadNum.Text;

            //var URL = ConfigurationManager.AppSettings["TCCExportInvoicesWebServiceURL"];
            //var userName = ConfigurationManager.AppSettings["TCCExportedInvoicesUserName"];
            //var password = ConfigurationManager.AppSettings["TCCExportedInvoicesPassword"];
            var email = txtEmail.Text;
            var operatorName = Request.LogonUserIdentity.Name.Replace("CGI\\", "");
            var exportPath = ConfigurationManager.AppSettings["TCCExportedInvoicesFilePathCorpDrop"];

            try
            {
                var sb = new StringBuilder();
                foreach (GridViewRow gvRow in gvHistory.Rows)
                {
                    var document = gvRow.Cells[0].Text;
                    if (gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked)
                    {
                        sb.Append($@"{document},");
                    }
                }

                var includedDocuments = sb.ToString().TrimEnd(',');
                if (includedDocuments.Length > 0)
                {
                    var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
                    cnTCCSUPPORT.Open();
                    TCCExportInvoicesManager.TCCExportInvoices(cnTCCSUPPORT, RadioButtonList1.SelectedValue, plantCode, custNum, startDate, endDate, operatorName, email, startInvoice, endInvoice, invoiceList, includedDocuments);
                    cnTCCSUPPORT.Close();
                    Logger.Info($@"response.StatusDescription == ""OK""");
                    lblJobResults.Text = $@"Invoice PDFs have been exported to: <strong>{exportPath}\{plantCode}</strong>";
                    lblJobResults1.Text = $@"You can paste the bold text above into Windows File Explorer";
                    lblJobResults2.Text = $@"An email containing a link to the PDFs has been sent to:  {email}";
                    divJobResults.Visible = true;
                    gvHistory.Visible = false;
                }
                else
                {
                    lblError.Text = "Please select one or more Invoices.";
                    lblError.Visible = true;
                    divError.Visible = true;
                }
            }
            catch (Exception err)
            {
                Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
            }
            Logger.Info("btnExport_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void GetCustomers()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetCustomers BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            selCustomer.Items.Clear();
            var plant = PlantManager.GetPlant(selPlant.Text);
            var cn = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cn.Open();
            SqlDataAdapter sda = new SqlDataAdapter("[usp_GetCustomers]", cn);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.CommandTimeout = 0;
            SqlParameter pSubscriptionId = sda.SelectCommand.Parameters.Add("@PlantNum", SqlDbType.Int);
            pSubscriptionId.Direction = ParameterDirection.Input;
            pSubscriptionId.Value = plant.NumericValue;
            SqlParameter pRowCount = sda.SelectCommand.Parameters.Add("@RowCount", SqlDbType.Int);
            pRowCount.Direction = ParameterDirection.Output;
            DataSet ds = new DataSet();
            Logger.Info(Logging.DbCmdString(sda));
            sda.Fill(ds, "Customers");
            selCustomer.DataSource = ds;
            selCustomer.DataTextField = "CUST";
            selCustomer.DataValueField = "DOCNUMBER2";
            selCustomer.DataBind();
            cn.Close();
            Logger.Info($"Rows: {selCustomer.Items.Count}");
            Logger.Info("GetCustomers END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
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
            divGridHistory.Visible = false;
            gvHistory.DataSource = null;
            gvHistory.DataBind();
        }

        protected void fnEMailChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }
        
        protected void fnPlantChanged(Object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnPlantChanged BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            GetCustomers();
            divGo.Visible = true; 
            Logger.Info("fnPlantChanged END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void fnRadioButtonList1Changed(Object sender, EventArgs e)
        {
            fnSelDivs(RadioButtonList1.SelectedValue);
        }

        protected void fnSelDivs(string selValue)
        {
            if (selValue == "CustDate")
            {
                divSelCustDate.Visible = true;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = false;
                divGo.Visible = (selPlant.Items.Count > 0 && selCustomer.Items.Count > 0);
            }
            else if (selValue == "InvoiceRange")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = true;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = false;
                divGo.Visible = true; 
            }
            else if (selValue == "InvoiceList")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = true;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = false;
                divGo.Visible = true; 
            }
            else if (selValue == "BookingNum")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = true;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = false;
                divGo.Visible = true; 
            }
            else if (selValue == "BOLNum")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = true;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = false;
                divGo.Visible = true;
            }
            else if (selValue == "LoadNum")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = true;
                divSelPONum.Visible = false;
                divGo.Visible = true; 
            }
            else if (selValue == "PONum")
            {
                divSelCustDate.Visible = false;
                divSelInvoiceRange.Visible = false;
                divSelInvoiceList.Visible = false;
                divSelBookingNum.Visible = false;
                divSelBOLNum.Visible = false;
                divSelLoadNum.Visible = false;
                divSelPONum.Visible = true;
                divGo.Visible = true; 
            }
        }

        protected void fnRadInvoiceNumChanged(Object sender, EventArgs e)
        {
            ClearGrid();
        }

        protected void fnCustomerChanged(Object sender, EventArgs e)
        {
            ClearGrid();
        }

        protected void fnStartDateChanged(Object sender, EventArgs e)
        {
            ClearGrid();
        }

        protected void fnEndDateChanged(Object sender, EventArgs e)
        {
            ClearGrid();
        }

        protected void fnInvoiceStartChanged(Object sender, EventArgs e)
        {
            //ClearGrid();
        }

        protected void fnInvoiceEndChanged(Object sender, EventArgs e)
        {
            //ClearGrid();
        }

        protected void fnInvoiceListChanged(Object sender, EventArgs e)
        {
            //ClearGrid();
        }

        protected void chkInclude_Changed(Object sender, EventArgs e)
        {
            fnInclude_Changed();
        }
        protected void chkDocTypes_Changed(Object sender, EventArgs e)
        {
            fnDocTypes_Changed();
        }

        protected void fnShowDiv(String sDivType)
        {
            switch (sDivType)
            {
                case "None":
                    divSelDates.Visible = false;
                    //divSelPlant.Visible = false;
                    divGo.Visible = false;
                    break;
                case "Dates":
                    divSelDates.Visible = true;
                    break;
                case "Plant":
                    divSelPlant.Visible = true;
                    break;
                case "Go":
                    divGo.Visible = true;
                    break;
            }
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
                if (txtEmail.Text.Length == 0) txtEmail.Text = userprincipalname;
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
                        plantCode = plantCodes[count].Value.Replace("OU=","").Replace(",","");
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

        protected Int16 GetPlants()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetPlants BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            var cn = new SqlConnection();
            cn = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
            cn.Open();
            SqlDataAdapter sda = new SqlDataAdapter("[usp_GetPlants]", cn);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            Logger.Info(Logging.DbCmdString(sda));
            sda.Fill(ds, "Plants");
            selPlant.DataSource = ds;
            selPlant.DataTextField = "Plant";
            selPlant.DataValueField = "ID";
            selPlant.DataBind();
            cn.Close();
            Logger.Info($"Rows: {selPlant.Items.Count}");
            Logger.Info("GetPlants END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return 0;
        }

        protected void btnGetHistory_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnGetHistory_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            string plantNum = selPlant.Text;
            string plantCode = selPlant.SelectedItem.Text;
            _plant = plantCode;
            var plant = PlantManager.GetPlant(plantCode);
            string custNum = selCustomer.SelectedValue;
            var startDate = Convert.ToDateTime(txtFromDate.Text);
            var endDate = Convert.ToDateTime(txtToDate.Text);
            var startInvoice = txtFromInvoice.Text;
            var endInvoice = txtToInvoice.Text;
            var invoiceList = txtInvoiceList.Text;

            var bookingNum = txtBookingNum.Text;
            var bolNum = txtBOLNum.Text;
            var loadNum = txtLoadNum.Text;
            var poNum = txtPONum.Text;

            if (fnValidate(plantCode, custNum, startDate, endDate, txtEmail.Text, startInvoice, endInvoice, invoiceList, bookingNum, bolNum, loadNum, poNum))
            {
                fnGetHistory(plant.PlantNum, custNum, startDate, endDate, startInvoice, endInvoice, invoiceList, bookingNum, bolNum, loadNum, poNum);
                fnInclude_Changed();
                divGridHistory.Visible = true;
            }
            Logger.Info("btnGetHistory_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected Boolean fnValidate(string plantCode, string custNum, DateTime startDate, DateTime endDate, string email, string startInvoice, string endInvoice, string invoiceList, string bookingNum, string bolNum, string loadNum, string poNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnValidate BEGIN: {DateTime.Now.ToLocalTime()}");
            DateTime dFromDate = System.DateTime.Today;
            DateTime dToDate = System.DateTime.Today.AddDays(1);
            lblError.Text = "";
            divError.Visible = false;
            txtFromDate.BackColor = System.Drawing.Color.White;
            txtToDate.BackColor = System.Drawing.Color.White;
            //if (plantCode.Length < 4)
            //{
            //    lblError.Text = "Please select a Plant.";
            //    lblError.Visible = true;
            //    divError.Visible = true;
            //    return false;
            //}
            if (!email.Contains("@cardinalcorp.com") && !email.Contains("@cgi.int"))
            {
                lblError.Text = "Please enter a valid Cardinal email address.";
                lblError.Visible = true;
                divError.Visible = true;
                return false;
            }

            var searchType = RadioButtonList1.SelectedValue;
            if (searchType == "CustDate")
            {
                if (custNum.Length < 8)
                {
                    lblError.Text = "Please select a Customer.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
                if (startDate.ToShortDateString().Length != 0)
                {
                    dFromDate = startDate;
                }
                if (endDate.ToShortDateString().Length != 0)
                {
                    dToDate = endDate;
                }
                if (dFromDate > dToDate)
                {
                    lblError.Text = "Start Date must be before End Date.";
                    lblError.Visible = true;
                    txtFromDate.BackColor = System.Drawing.Color.Pink;
                    divError.Visible = true;
                    return false;
                }
            }
            else if (searchType == "InvoiceRange")
            {
                if (startInvoice.Length > 0)
                {
                    int fromInvoice;
                    bool isNumber = Int32.TryParse(startInvoice, out fromInvoice);
                    if (!isNumber)
                    {
                        lblError.Text = "Please enter a valid starting Invoice #.";
                        lblError.Visible = true;
                        divError.Visible = true;
                        return false;
                    }

                    if (endInvoice.Length > 0)
                    {
                        int toInvoice;
                        isNumber = Int32.TryParse(endInvoice, out toInvoice);
                        if (!isNumber)
                        {
                            lblError.Text = "Please enter a valid ending Invoice #.";
                            lblError.Visible = true;
                            divError.Visible = true;
                            return false;
                        }

                        if (fromInvoice > toInvoice)
                        {
                            lblError.Text = "Ending Invoice # should be greater than starting Invoice #.";
                            lblError.Visible = true;
                            divError.Visible = true;
                            return false;
                        }
                    }
                }
            }
            else if (searchType == "InvoiceList")
            {
                if (txtInvoiceList.Text.Length == 0)
                {
                    lblError.Text = "Please enter a list of Invoice #s separated by commas.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
                txtInvoiceList.Text = txtInvoiceList.Text.TrimEnd(',');
            }
            else if (searchType == "BookingNum")
            {
                if (txtBookingNum.Text.Length == 0)
                {
                    lblError.Text = "Please enter a Booking #.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
            }
            else if (searchType == "BOLNum")
            {
                if (txtBOLNum.Text.Length == 0)
                {
                    lblError.Text = "Please enter a BOL #.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
            }
            else if (searchType == "LoadNum")
            {
                if (txtLoadNum.Text.Length == 0)
                {
                    lblError.Text = "Please enter a Load #.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
            }
            else if (searchType == "PONum")
            {
                if (txtPONum.Text.Length == 0)
                {
                    lblError.Text = "Please enter a PO #.";
                    lblError.Visible = true;
                    divError.Visible = true;
                    return false;
                }
            }

            Logger.Info("fnValidate END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return true;
        }

        protected void fnGetHistory(string plantNum, string custNum, DateTime startDate, DateTime endDate, string startInvoice, string endInvoice, string invoiceList, string bookingNum, string bolNum, string loadNum, string poNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnGetHistory BEGIN: {DateTime.Now.ToLocalTime()}");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            var filterString = "";
            if (RadioButtonList1.SelectedValue == "CustDate")
            {
                filterString = $@"DOCNUMBER2 = '{custNum}' AND DOCDATE_CALC >= '{startDate}' AND DOCDATE_CALC <= '{endDate}' ";
            }
            else if (RadioButtonList1.SelectedValue == "InvoiceRange")
            {
                if (endInvoice == "")
                {
                    filterString = $@"DOCNUMBER1 = '{startInvoice}' ";
                }
                else
                {
                    filterString = $@"DOCNUMBER1 >= '{startInvoice}' AND DOCNUMBER1 <= '{endInvoice}' ";

                }
            }
            else if (RadioButtonList1.SelectedValue == "InvoiceList")
            {
                invoiceList = invoiceList.TrimEnd(' ');
                invoiceList = invoiceList.TrimEnd(',');
                filterString = $@"TRY_CONVERT(BIGINT,DOCNUMBER1) IN ({invoiceList}) ";
            }

            else if (RadioButtonList1.SelectedValue == "BookingNum")
            {
                invoiceList = GetInvoiceListFromE1BookingNum(bookingNum);
                invoiceList = invoiceList.TrimEnd(' ');
                invoiceList = invoiceList.TrimEnd(',');
                if (invoiceList.Length > 0) filterString = $@"TRY_CONVERT(BIGINT,DOCNUMBER1) IN ({invoiceList}) ";
            }
            else if (RadioButtonList1.SelectedValue == "BOLNum")
            {
                filterString = $@"BOL = '{bolNum}' ";
            }
            else if (RadioButtonList1.SelectedValue == "LoadNum")
            {
                filterString = $@"LOADNUM = '{loadNum}' ";
            }
            else if (RadioButtonList1.SelectedValue == "PONum")
            {
                filterString = $@"PO1 LIKE '%{poNum}%' ";
            }
            if (filterString.Length > 0)
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnTCCSUPPORT,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT 
                            DocID,
                            DOCSUBJECT as DocSubject,
                            DOCNUMBER1 as DocNum,
                            DOCNUMBER2 as CustNum,
                            DOCDESC1 as CustName,
                            CONVERT(CHAR(10), DOCDATE_CALC, 101) as DocDate,
                            LOADNUM as LoadNum,
                            BOL,
                            PO1 as PO,
                            1 as Include
                        FROM [TCCSUPPORT].[dbo].[FINAL_INVOICES_IMAGES]
                        WHERE 
                            DOCSUBJECT LIKE '%INVOICE%' AND 
                            {filterString}
                        UNION
                        SELECT 
                            DocID,
                            DOCSUBJECT as DocSubject,
                            DOCNUMBER1 as DocNum,
                            DOCNUMBER2 as CustNum,
                            DOCDESC1 as CustName,
                            CONVERT(CHAR(10), DOCDATE_CALC, 101) as DocDate,
                            LOADNUM as LoadNum,
                            BOL,
                            PO1 as PO,
                            1 as Include
                        FROM [TCCSUPPORT].[dbo].[MANUFACTURING]
                        WHERE 
                            DOCSUBJECT IN ('Proof Invoice', 'Packing Slip') AND 
                            {filterString}
                    ORDER BY DocID
                "
                };
                cmd.Parameters.Add(new SqlParameter("@PlantNum", SqlDbType.NVarChar, 20));
                cmd.Parameters["@PlantNum"].Value = plantNum;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                Logger.Info(Logging.DbCmdString(sda));
                try
                {
                    sda.Fill(dt);
                    gvHistory.DataSource = dt;
                    gvHistory.DataBind();
                    ViewState["dirState"] = dt;
                    ViewState["sortdr"] = "Asc";
                    if (dt.Rows.Count > 0)
                    {
                        txtQueryResults.Text = dt.Rows.Count == 1 ? Convert.ToString(dt.Rows.Count) + " Document found" : Convert.ToString(dt.Rows.Count) + " Documents found";
                        btnExport.Visible = true;
                        // If there are more than one Doc Type then give them check boxes
                        List<string> docTypes = new List<string>();
                        foreach (GridViewRow gvRow in gvHistory.Rows)
                        {
                            var docType = gvRow.Cells[1].Text;
                            if (gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked) if (!docTypes.Contains(docType)) docTypes.Add(docType);
                        }
                        chkDocTypes.Items.Clear();
                        if (docTypes.Count > 1)
                        {
                            var i = 0;
                            foreach (var docType in docTypes)
                            {
                                chkDocTypes.Items.Add(docType);
                                chkDocTypes.Items[i].Selected = true;
                                i++;
                            }
                        }
                        divDocTypes.Visible = docTypes.Count > 1;
                    }
                    else
                    {
                        txtQueryResults.Text = "No Invoices found";
                        btnExport.Visible = false;
                    }
                    divJobResults.Visible = false;
                    gvHistory.Visible = true;
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR: " + e.Message + ": " + e.StackTrace);
                    lblError.Text = "ERROR: " + e.Message;
                    lblError.Visible = true;
                    divError.Visible = true;
                }
            }
            else
            {
                txtQueryResults.Text = "No Invoices found";
            }
            cnTCCSUPPORT.Close();
            Logger.Info($"Rows: {dt.Rows.Count}");
            Logger.Info("fnGetHistory END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        
        protected void fnDocTypes_Changed()
        {
            IDictionary<string, bool> docTypes = new Dictionary<string, bool>();
            for (int i = 0; i < chkDocTypes.Items.Count; i++)
            {
                if (!docTypes.ContainsKey(chkDocTypes.Items[i].Text))
                {
                    docTypes.Add(chkDocTypes.Items[i].Text, chkDocTypes.Items[i].Selected);
                }
            }
            foreach (GridViewRow gvRow in gvHistory.Rows)
            {
                var docType = gvRow.Cells[1].Text;
                if (docTypes.TryGetValue(docType, out bool include))
                {
                    if (gvRow.FindControl("chkInclude") is CheckBox checkbox) checkbox.Checked = include;
                }
            }
            GetCountSelectedDocs();
        }

        protected void fnInclude_Changed()
        {
            GetCountSelectedDocs();
            if (chkDocTypes.Items.Count > 1)
            {
                IDictionary<string, bool> docTypes = new Dictionary<string, bool>();
                foreach (GridViewRow gvRow in gvHistory.Rows)
                {
                    var docType = gvRow.Cells[1].Text;
                    var isChecked = gvRow.FindControl("chkInclude") is CheckBox checkbox && checkbox.Checked;
                    if (!docTypes.ContainsKey(docType))
                    {
                        docTypes.Add(docType, isChecked);
                    }
                    else
                    {
                        if (isChecked) docTypes[docType] = true;
                    }
                }
                chkDocTypes.Items.Clear();
                var i = 0;
                foreach (KeyValuePair<string, bool> docType in docTypes)
                {
                    chkDocTypes.Items.Add(docType.Key);
                    chkDocTypes.Items[i].Selected = docType.Value;
                    i++;
                }
            }
        }

        protected int GetCountSelectedDocs()
        {
            var invoicesSelected = 0;
            foreach (GridViewRow gvRow in gvHistory.Rows)
            {
                if (gvRow.FindControl("chkInclude") is CheckBox checkbox)
                {
                    if (checkbox.Checked) invoicesSelected++;
                }
            }

            txtInvoicesSelected.Text = invoicesSelected == 1 ? Convert.ToString(invoicesSelected) + " Document selected" : Convert.ToString(invoicesSelected) + " Documents selected";
            return invoicesSelected;
        }

        private void ToggleCheckState(bool checkState)
        {
            // Iterate through the Products.Rows property
            foreach (GridViewRow row in gvHistory.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl("chkInclude");
                if (cb != null) cb.Checked = checkState;
            }
            fnInclude_Changed();
        }

        protected void btnCheckAll_Click(object sender, EventArgs e)
        {
            ToggleCheckState(true);
            for (int i = 0; i < chkDocTypes.Items.Count; i++)
            {
                chkDocTypes.Items[i].Selected = true;
            }
        }

        protected void btnUncheckAll_Click(object sender, EventArgs e)
        {
            ToggleCheckState(false);
            for (int i = 0; i < chkDocTypes.Items.Count; i++)
            {
                chkDocTypes.Items[i].Selected = false;
            }
        }

        protected void gvHistory_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)ViewState["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                IDictionary<int, bool> documents = new Dictionary<int, bool>();
                foreach (GridViewRow gvRow in gvHistory.Rows)
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

                gvHistory.DataSource = dtrslt;
                gvHistory.DataBind();

                foreach (GridViewRow gvRow in gvHistory.Rows)
                {
                    var document = Convert.ToInt32(gvRow.Cells[0].Text);
                    if (documents.TryGetValue(document, out bool include))
                    {
                        if (gvRow.FindControl("chkInclude") is CheckBox checkbox) checkbox.Checked = include;
                    }
                }
                GetCountSelectedDocs();

            }
        }

        public static string GetInvoiceListFromE1BookingNum(string bookingNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            string invoiceList = "";
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var cmd = new iDB2Command()
                {
                    Connection = e1Session.DbConnection,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT DISTINCT
	                        F4211.SDDOC
                        FROM
	                        F580150
		                        INNER JOIN F4211
	                        ON F580150.LHLDNM = F4211.SDPSN
                        WHERE
	                        F580150.LHZ58BKNG# = ?
                        UNION
                        SELECT DISTINCT
	                        SDDOC
                        FROM
	                        F580150
		                        INNER JOIN F42119
	                        ON F580150.LHLDNM = F42119.SDPSN
                        WHERE
	                        F580150.LHZ58BKNG# = ?
                        FOR READ ONLY
                    "
                };
                cmd.Parameters.Add(new iDB2Parameter("@BookingNum1", iDB2DbType.iDB2Graphic, 25));
                cmd.Parameters["@BookingNum1"].Value = bookingNum;
                cmd.Parameters.Add(new iDB2Parameter("@BookingNum2", iDB2DbType.iDB2Graphic, 25));
                cmd.Parameters["@BookingNum2"].Value = bookingNum;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        var invoiceNum = CastDbNull.To<decimal>(reader["SDDOC"], 0);
                        invoiceList = $@"{invoiceNum},{invoiceList}";
                        i++;
                    }

                    reader.Close();
                    Logger.Trace($"ROWS: {i}");
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return invoiceList;
        }

        public static string GetInvoiceListFromE1BOLNum(string bolNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            string invoiceList = "";
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var cmd = new iDB2Command()
                {
                    Connection = e1Session.DbConnection,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT DISTINCT
	                        F4211.SDDOC
                        FROM
	                        F580150
		                        INNER JOIN F4211
	                        ON F580150.LHLDNM = F4211.SDPSN
                        WHERE
	                        F580150.LHBOLN = ?
                        FOR READ ONLY
                    "
                };
                cmd.Parameters.Add(new iDB2Parameter("@BOLNum", iDB2DbType.iDB2Decimal, 8));
                cmd.Parameters["@BOLNum"].Value = bolNum;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        var invoiceNum = CastDbNull.To<decimal>(reader["SDDOC"], 0);
                        invoiceList = $@"{invoiceNum},{invoiceList}";
                        i++;
                    }

                    reader.Close();
                    Logger.Trace($"ROWS: {i}");
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return invoiceList;
        }

        public static string GetInvoiceListFromE1LoadNum(string loadNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            string invoiceList = "";
            var e1Session = BusinessSystemSessionManager.GetBusinessSession();
            try
            {
                var cmd = new iDB2Command()
                {
                    Connection = e1Session.DbConnection,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT DISTINCT
	                        F4211.SDDOC
                        FROM
	                        F580150
		                        INNER JOIN F4211
	                        ON F580150.LHLDNM = F4211.SDPSN
                        WHERE
	                        F580150.LHLDNM = ?
                        FOR READ ONLY
                    "
                };
                cmd.Parameters.Add(new iDB2Parameter("@LoadNum", iDB2DbType.iDB2Decimal, 25));
                cmd.Parameters["@LoadNum"].Value = loadNum;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var i = 0;
                    while (reader.Read())
                    {
                        var invoiceNum = CastDbNull.To<decimal>(reader["SDDOC"], 0);
                        invoiceList = $@"{invoiceNum},{invoiceList}";
                        i++;
                    }

                    reader.Close();
                    Logger.Trace($"ROWS: {i}");
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return invoiceList;
        }

    }
}