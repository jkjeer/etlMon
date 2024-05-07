using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using etlMon.Common.Managers;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types;
using etlMon.Common.Types.Archie;
using etlMon.Common.Types.Enterprise;
using etlMon.Common.Types.Response;
using etlMon.Common.Utilities;
using NLog;

namespace etlMon
{
    public partial class TCCImportBOLs : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected static string _guid = ""; 
        protected static Guid _realguid;
        private static string _operatorName = "";
        private static string _email = "";
        protected System.Web.UI.HtmlControls.HtmlIframe framePDF;
        protected static BOL _bol = new BOL();
        private static ProcessResponse _response = new ProcessResponse();

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {startTime}");
            String sOperator = Request.LogonUserIdentity.Name;
            if (Message1.Text != sOperator) // This is not the first load of this page so .
            {
                Message1.Text = sOperator + "&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            }

            _guid = Request.QueryString["guid"];
            if (Guid.TryParse(Request.QueryString["guid"], out _realguid))
            {
                Logger.Info($"Guid {_guid} successfully parsed");
                if (!IsPostBack) /* This is the first load of this page. */
                {
                    _response = new ProcessResponse($"BOL Importer {ConfigurationManager.AppSettings["Environment"]}: ", true);
                    try
                    {
                        _operatorName = Request.LogonUserIdentity.Name.Replace("CGI\\", "");
                        Plant plant = GetOperatorPlant(_operatorName);
                        _bol = GetBOLAttachment(_guid);
                        LoadPDFViewer();
                        LoadControls(plant.PlantNum);
                    }
                    catch (Exception err)
                    {
                        Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
                    }
                }
            }
            else
            {
                Logger.Error($"Error: Guid {_guid} not parsed ");
                btnSubmit.Enabled = false;
                lblError.Text = $@"BOL Attachment {_guid} not found";
                divError.Visible = true;
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void LoadControls(string plantNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"LoadControls BEGIN: {DateTime.Now.ToLocalTime()}");
            var step13 = new ProcessStep($@"Step 3. Load Controls");
            try
            {
                txtBOLDate.Text = Convert.ToString(_bol.DOCDATE_CALC.ToShortDateString());
                txtLoadNum.Text = _bol.LOADNUM;
                txtPlantNum.Text = _bol.DOCLOCATION;
                if (txtPlantNum.Text.Length == 0) txtPlantNum.Text = plantNum;
                txtBOLNum.Text = _bol.BOLNum;
                txtCarrier.Text = _bol.CARRIER;
                txtCustName.Text = _bol.DOCDESC1;
                txtCustNum.Text = _bol.DOCNUMBER2;
                txtDocNum.Text = _bol.DOCNUMBER1;
                btnSubmit.Enabled = true;
                divError.Visible = false;
            }
            catch (Exception e)
            {
                step13.Errors.Add(new Error($"Error: {e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }
            finally
            {
                step13.RowsAffected = 1;
                step13.Done();
                _response.ProcessSteps.Add(step13);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        protected void LoadPDFViewer()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"LoadPDFViewer BEGIN: {DateTime.Now.ToLocalTime()}");
            var step13 = new ProcessStep($@"Step 3. Load PDFViewer");
            try
            {
                framePDF = (HtmlIframe)FindControl("frame1");
                framePDF.Attributes["src"] = $@"PDFViewer.aspx?guid={_guid}";
            }
            catch (Exception e)
            {
                step13.Errors.Add(new Error($"Error: {e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }
            finally
            {
                step13.RowsAffected = 1;
                step13.Done();
                _response.ProcessSteps.Add(step13);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        protected void btnRetrieve_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnRetrieve_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            var stepSubmit = new ProcessStep($@"Retrieve BOL");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            try
            {
                var loadNum = txtLoadNum.Text;
                if (loadNum.Length > 0)
                {
                    _bol = GetBOL(cnTCCSUPPORT, loadNum);
                    txtBOLDate.Text = Convert.ToString(_bol.DOCDATE_CALC.ToShortDateString());
                    txtLoadNum.Text = _bol.LOADNUM;
                    txtPlantNum.Text = _bol.DOCLOCATION;
                    txtBOLNum.Text = _bol.BOLNum;
                    txtCarrier.Text = _bol.CARRIER;
                    txtCustName.Text = _bol.DOCDESC1;
                    txtCustNum.Text = _bol.DOCNUMBER2;
                    txtDocNum.Text = _bol.DOCNUMBER1;
                }
                else
                {
                    lblError.Text = $@"Please enter a valid Load #";
                    divError.Visible = true;
                }
            }
            catch (Exception err)
            {
                stepSubmit.Errors.Add(new Error($"Error: {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                cnTCCSUPPORT.Close();
                //cnTCC.Close();
                stepSubmit.RowsAffected = 1;
                stepSubmit.Done();
                _response.ProcessSteps.Add(stepSubmit);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            Logger.Info("btnRetrieve_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected BOL GetBOL(SqlConnection cnTCCSUPPORT, string loadNum)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetBOL BEGIN: {startTime}");
            var doc = new BOL();
            try
            {

                using (IDataReader reader = GetBOLReader(cnTCCSUPPORT, loadNum))
                {
                    while (reader.Read())
                    {
                        doc.Image = CastDbNull.To<byte[]>(reader["Image"], new byte[0]);
                        doc.AttachmentGuid = CastDbNull.To(reader["AttachmentGuid"], Guid.Empty);
                        doc.DocID = CastDbNull.To(reader["DocID"], 0);
                        doc.DOCSUBJECT = CastDbNull.To(reader["DOCSUBJECT"], string.Empty);
                        doc.DOCLOCATION = CastDbNull.To(reader["DOCLOCATION"], string.Empty);
                        doc.DOCNUMBER1 = CastDbNull.To(reader["DOCNUMBER1"], string.Empty);
                        doc.DOCNUMBER2 = CastDbNull.To(reader["DOCNUMBER2"], string.Empty);
                        doc.DOCDESC1 = CastDbNull.To(reader["DOCDESC1"], string.Empty);
                        doc.DOCDATE_CALC = CastDbNull.To(reader["DOCDATE_CALC"], new DateTime(1900, 1, 1));
                        doc.DOCDATE_STR = doc.DOCDATE_CALC.ToString("M/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        doc.LOADNUM = CastDbNull.To(reader["LOADNUM"], string.Empty);
                        doc.CARRIER = CastDbNull.To(reader["CARRIER"], string.Empty);
                        doc.BOLNum = CastDbNull.To(reader["BOL"], string.Empty);
                        doc.DOCDATE = CastDbNull.To(reader["DOCDATE"], string.Empty);
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            Logger.Info("GetBOL END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return doc;
        }

        public static IDataReader GetBOLReader(SqlConnection cnTCCSUPPORT, string loadNum)
        {
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnTCCSUPPORT,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT TOP(1) m.[DocID]
                          ,m.[DateStored]
                          ,m.[DOCSUBJECT]
                          ,m.[DOCLOCATION]
                          ,m.[DOCNUMBER1]
                          ,m.[DOCNUMBER2]
                          ,m.[DOCDESC1]
                          ,m.[DOCDATE_CALC]
                          ,m.[LOADNUM]
                          ,m.[CARRIER]
                          ,m.[BOL]
                          ,m.[DOCDATE]
                          ,d.[Image]
	                      ,a.[AttachmentGuid]
                          ,m.[DOCSTAMP]
                      FROM [TCCSUPPORT].[dbo].[MANUFACTURING] m
                          LEFT OUTER JOIN [TCCSUPPORT].[dbo].[DOCUMENTS] d ON d.fsdDocID = m.fsdDocID
                          LEFT OUTER JOIN [TCCSUPPORT].[dbo].[ImportBOLAttachments] a ON a.LOADNUM = m.LOADNUM
                      WHERE m.[DOCSUBJECT] = 'Signed BOL'
                        AND m.[LOADNUM] = '{loadNum}'"
                };

                cmd.CommandText = cmd.CommandText + " ORDER BY m.[DateStored] DESC";
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleRow);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        protected void btnSubmit_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnSubmit_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            var stepSubmit = new ProcessStep($@"Save BOL");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            var cnTCC = ConnectionManager.GetSqlConnectionFromKey("TCC");
            cnTCC.Open();
            try
            {

                if (ValidateBOL(cnTCCSUPPORT))
                {
                    _bol.DOCDATE_CALC = Convert.ToDateTime(txtBOLDate.Text); //= Convert.ToString(doc.DOCDATE_CALC.ToShortDateString());
                    _bol.LOADNUM = txtLoadNum.Text;
                    _bol.DOCLOCATION = txtPlantNum.Text;
                    _bol.BOLNum = txtBOLNum.Text;
                    _bol.CARRIER = txtCarrier.Text;
                    _bol.DOCDESC1 = txtCustName.Text;
                    _bol.DOCNUMBER2 = txtCustNum.Text;
                    _bol.DOCNUMBER1 = txtDocNum.Text;

                    _bol.BOLCUSTNUM = _bol.DOCNUMBER2;
                    _bol.BOLCUSTNAM = _bol.DOCDESC1;
                    _bol.BOLDATE = Convert.ToString(_bol.DOCDATE_CALC.ToShortDateString());
                    _bol.BOLLOADNO = _bol.LOADNUM;
                    _bol.BOLCARRIER = _bol.CARRIER;
                    _bol.BOLBOLNUM = _bol.BOLNum;
                    _bol.BOLDOCNUM = _bol.DOCNUMBER1;
                    _bol.bolMCU = _bol.DOCLOCATION;

                    _bol.ProcessingStatus = "Load";
                    TCCImportBOLManager.TCCImportBOL(cnTCCSUPPORT, cnTCC, _bol, _operatorName, _email);
                    lblSaved.Text = $@"BOL data saved";
                    divSaved.Visible = true;
                }
            }
            catch (Exception err)
            {
                stepSubmit.Errors.Add(new Error($"Error: {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                cnTCCSUPPORT.Close();
                cnTCC.Close();
                stepSubmit.RowsAffected = 1;
                stepSubmit.Done();
                _response.ProcessSteps.Add(stepSubmit);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            Logger.Info("btnSubmit_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        public bool ValidateBOL(SqlConnection cnTCCSUPPORT)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"ValidateBOL BEGIN: {startTime}");
            var stepValidateBOLs = new ProcessStep($@"Step 35. Validate BOL");
            try
            {
                _bol.ProcessingStatus = "Load";
                _bol.ProcessingReason = "";
                _bol.IsValid = true;
                //if (_bol.LOADNUM.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Reject";
                //    _bol.ProcessingReason = "LoadNum,";
                //    _bol.IsValid = false;
                //}

                ////if (_bol.DOCDATE_CALC.Length == 0)
                ////{
                ////    _bol.ProcessingStatus = "Hold";
                ////    _bol.ProcessingReason = "PlantNum,";
                ////    _bol.IsValid = false;
                ////}
                //if (_bol.DOCLOCATION.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Hold";
                //    _bol.ProcessingReason = "CustNum,";
                //    _bol.IsValid = false;
                //}

                //if (_bol.BOLNum.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Hold";
                //    _bol.ProcessingReason = "CustName,";
                //    _bol.IsValid = false;
                //}

                //if (_bol.CARRIER.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Hold";
                //    _bol.ProcessingReason = "Carrier,";
                //    _bol.IsValid = false;
                //}
                DateTime dateVal;
                if (!DateTime.TryParse(txtBOLDate.Text, out dateVal))
                {
                    _bol.ProcessingStatus = "Hold";
                    _bol.ProcessingReason = "Date,";
                    _bol.IsValid = false;
                    lblError.Text = $@"{txtBOLDate.Text} is not a valid date";
                    divError.Visible = true;
                    Logger.Info($@"{txtBOLDate.Text} is not a valid date");
                }

                //if (_bol.DOCNUMBER2.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Hold";
                //    _bol.ProcessingReason = "BOLNum,";
                //    _bol.IsValid = false;
                //}

                //if (_bol.DOCNUMBER1.Length == 0)
                //{
                //    _bol.ProcessingStatus = "Option";
                //    _bol.ProcessingReason = "DocNum,";
                //    _bol.IsValid = true;
                //}

                if (_bol.ProcessingReason.Length > 0) _bol.ProcessingReason = _bol.ProcessingReason.TrimEnd(',');
                UpdateSQLBOLEmailAttachmentValidation(cnTCCSUPPORT);
            }
            catch (Exception e)
            {
                stepValidateBOLs.Errors.Add(new Error($"Error: {e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }
            finally
            {
                stepValidateBOLs.RowsAffected = 1;
                stepValidateBOLs.Done();
                _response.ProcessSteps.Add(stepValidateBOLs);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return _bol.IsValid;
        }

        public int UpdateSQLBOLEmailAttachmentValidation(SqlConnection cnTCCSUPPORT)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"UpdateSQLBOLEmailAttachmentValidation BEGIN: {startTime}");
            var stepUpdateSQLEmailAttachmentValidation = new ProcessStep($@"Step 4. Update SQL BOL Email Attachment Validation {_bol.AttachmentGuid}");
            int rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_UpdateImportBOLEmailAttachmentValidation",
                    CommandTimeout = 0
                };
                cmd.Parameters.AddWithValue("@ID", _bol.AttachmentId);
                cmd.Parameters.AddWithValue("@AttachmentGuid", _bol.AttachmentGuid);
                cmd.Parameters.AddWithValue("@ProcessingStatus", _bol.ProcessingStatus);
                cmd.Parameters.AddWithValue("@ProcessingReason", _bol.ProcessingReason);
                if (_bol.Image ==  null) _bol.Image = new byte[0];
                //cmd.Parameters.AddWithValue("@Image", _bol.Image);
                cmd.Parameters.AddWithValue("@IsValid", _bol.IsValid);
                _response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                _response.RowsAffected = rowCount;
                //28806, ff781a6f-9663-4ade-a7e2-417efcae7301, Load, , System.Byte[], True

            }
            catch (Exception e)
            {
                stepUpdateSQLEmailAttachmentValidation.Errors.Add(new Error($"{cnTCCSUPPORT.ConnectionString}  {e.Message} : {e.StackTrace}"));
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                //throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                stepUpdateSQLEmailAttachmentValidation.RowsAffected = 1;
                stepUpdateSQLEmailAttachmentValidation.Done();
                _response.ProcessSteps.Add(stepUpdateSQLEmailAttachmentValidation);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return rowCount;
        }

        protected BOL GetBOLAttachment(string guid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetBOLAttachment BEGIN: {startTime}");
            var stepGetBOLAttachment = new ProcessStep($@"Step 1. Get BOL Attachment");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            byte[] image = new byte[0];
            var bol = new BOL();
            try
            {

                using (IDataReader reader = GetBOLAttachment(cnTCCSUPPORT, guid))
                {
                    while (reader.Read())
                    {
                        bol.AttachmentId = CastDbNull.To<Int64>(reader["ID"], 0);
                        bol.AttachmentGuid = CastDbNull.To<Guid>(reader["AttachmentGuid"], new Guid());
                        bol.ProcessingStatus = CastDbNull.To(reader["ProcessingStatus"], string.Empty);
                        bol.ProcessingReason = CastDbNull.To(reader["ProcessingReason"], string.Empty);
                        bol.IsValid = CastDbNull.To(reader["IsValid"], false);
                        bol.Image = CastDbNull.To<byte[]>(reader["Image"], new byte[0]);
                        bol.DocID = CastDbNull.To(reader["DocID"], 0);
                        bol.DOCSUBJECT = CastDbNull.To(reader["DOCSUBJECT"], string.Empty);
                        bol.DOCLOCATION = CastDbNull.To(reader["DOCLOCATION"], string.Empty);
                        bol.DOCNUMBER1 = CastDbNull.To(reader["DOCNUMBER1"], string.Empty);
                        bol.DOCNUMBER2 = CastDbNull.To(reader["DOCNUMBER2"], string.Empty);
                        bol.DOCDESC1 = CastDbNull.To(reader["DOCDESC1"], string.Empty);
                        bol.DOCDATE_CALC = CastDbNull.To(reader["DOCDATE_CALC"], new DateTime(1900, 1, 1));
                        bol.DOCDATE_STR = bol.DOCDATE_CALC.ToString("M/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                        bol.LOADNUM = CastDbNull.To(reader["LOADNUM"], string.Empty);
                        bol.CARRIER = CastDbNull.To(reader["CARRIER"], string.Empty);
                        bol.BOLNum = CastDbNull.To(reader["BOL"], string.Empty);
                        bol.DOCDATE = CastDbNull.To(reader["DOCDATE"], string.Empty);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                cnTCCSUPPORT.Close();
                stepGetBOLAttachment.RowsAffected = 1;
                stepGetBOLAttachment.Done();
                _response.ProcessSteps.Add(stepGetBOLAttachment);
                Logger.Info("GetBOLAttachment END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }
            return bol;
        }

        public static IDataReader GetBOLAttachment(SqlConnection cnTCCSUPPORT, string guid)
        {
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnTCCSUPPORT,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT [ID]
                              ,[AttachmentGuid]
                              ,[ProcessingStatus]
                              ,[ProcessingReason]
                              ,[IsValid]
                              ,[Image]
                              ,[DocID]
                              ,[SatID]
                              ,[IsValid]
                              ,[DOCSUBJECT]
                              ,[DOCLOCATION]
                              ,[DOCNUMBER1]
                              ,[DOCNUMBER2]
                              ,[DOCDESC1]
                              ,[DOCDATE_CALC]
                              ,[LOADNUM]
                              ,[CARRIER]
                              ,[BOL]
                              ,[DOCDATE]
                              ,[DOCSTAMP]
                          FROM [dbo].[ImportBOLAttachments]
                          WHERE [AttachmentGuid] = '{guid}'
                    "
                };
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleRow);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

        protected void fnBOLDateChanged(Object sender, EventArgs e)
        {
        }

        protected void fnLoadNumChanged(Object sender, EventArgs e)
        {
        }

        protected void fnPlantNumChanged(Object sender, EventArgs e)
        {
        }

        protected void fnBOLNumChanged(Object sender, EventArgs e)
        {
        }

        protected void fnCustNameChanged(Object sender, EventArgs e)
        {
        }

        protected void fnCustNumChanged(Object sender, EventArgs e)
        {
        }

        protected void fnCarrierChanged(Object sender, EventArgs e)
        {
        }

        protected void fnDocNumChanged(Object sender, EventArgs e)
        {
        }


        protected Plant GetOperatorPlant(string operatorName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetOperatorPlant BEGIN: {DateTime.Now.ToLocalTime()}");
            var stepGetOperatorPlant = new ProcessStep($@"Step 1. Get Operator Plant");
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

                _email = userprincipalname;
                Logger.Info($"userprincipalname: {userprincipalname}");
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
                stepGetOperatorPlant.Errors.Add(new Error($"Error: {e.Message} : {e.StackTrace} "));
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            finally
            {
                cnDW.Close();
                Logger.Info("GetOperatorPlant END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
                stepGetOperatorPlant.RowsAffected = 1;
                stepGetOperatorPlant.Done();
                _response.ProcessSteps.Add(stepGetOperatorPlant);
            }
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