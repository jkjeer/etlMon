using System;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using etlMon.Common.Managers;
using etlMon.Common.Managers.E1;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types.Archie;
using etlMon.Common.Types.E1;
using etlMon.Common.Types.Enterprise;
using etlMon.Common.Types.Response;
using etlMon.Common.Utilities;
using IBM.Data.DB2.iSeries;
using NLog;

namespace etlMon
{
    public partial class LaserFiche : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected const string sKey = "LaserFiche";

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"Page_Load BEGIN: {startTime}");
            String sOperator = Request.LogonUserIdentity.Name;
            Label mpLabel = (Label)Master.FindControl("pageHeader");
            if (mpLabel != null)
            {
                mpLabel.Text = sKey;
            }
            Label mpMsg = (Label)Master.FindControl("Message1");
            if (mpMsg != null)
            {
                mpMsg.Text = sOperator; // + "&nbsp;&nbsp;&nbsp;&nbsp;" +Convert.ToString(DateTime.Now); 
            }
            if (!IsPostBack) // This is the first load of this page so insert a new checklist record.
            {
                fnLoadDaysBack();
                //fnLoadLockout();
            }
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
        }

        protected void fnLoadDaysBack()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnLoadDaysBack BEGIN: {startTime}");
            selDaysBack.Items.Clear();
            //selDaysBack.Items.Add(Convert.ToString(1));
            //selDaysBack.Items.Add(Convert.ToString(2));
            //selDaysBack.Items.Add(Convert.ToString(3));
            selDaysBack.Items.Add(Convert.ToString(5));
            selDaysBack.Items.Add(Convert.ToString(7));
            selDaysBack.Items.Add(Convert.ToString(15));
            selDaysBack.Items.Add(Convert.ToString(30));
            selDaysBack.Items.Add(Convert.ToString(60));
            selDaysBack.Items.Add(Convert.ToString(90));
            selDaysBack.Items.Add(Convert.ToString(180));
            selDaysBack.Items.Add(Convert.ToString(365));
            selDaysBack.Items.Add(Convert.ToString(730));
            selDaysBack.Items.Add(Convert.ToString(1095));
            selDaysBack.Items.Add(Convert.ToString(1460));
            selDaysBack.SelectedIndex = 6;
            Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
        }

        //protected void fnLoadLockout()
        //{
        //    var startTime = DateTime.Now.ToLocalTime();
        //    Logger.Info($"fnLoadLockout BEGIN: {startTime}");
        //    selLockout.Items.Clear();
        //    selLockout.Items.Add(Convert.ToString(1));
        //    selLockout.Items.Add(Convert.ToString(2));
        //    selLockout.Items.Add(Convert.ToString(3));
        //    selLockout.Items.Add(Convert.ToString(4));
        //    selLockout.Items.Add(Convert.ToString(5));
        //    selLockout.Items.Add(Convert.ToString(6));
        //    selLockout.Items.Add(Convert.ToString(7));
        //    selLockout.Items.Add(Convert.ToString(8));
        //    selLockout.Items.Add(Convert.ToString(9));
        //    selLockout.Items.Add(Convert.ToString(10));
        //    selLockout.Items.FindByText(ConfigurationManager.AppSettings["LaserFicheLockoutPeriod"]).Selected = true;
        //    Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
        //}

        protected void btnRunNow_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnRunNow_Click BEGIN: {startTime}");
            lblJobResults.Text = "";
            lblJobResults.ForeColor = System.Drawing.Color.Black;
            updRunResults.Update();
            try
            {
                // Check to see if there is an execution in progress.
                Boolean bIsInProgress = fnIsInProgress(Convert.ToInt32(ConfigurationManager.AppSettings["LaserFicheLockoutPeriod"]));
                if (bIsInProgress == false)
                {
                    String sOperator = Request.LogonUserIdentity.Name;
                    String sEmail = sOperator.Substring(4) + "@cardinalcorp.com";
                    lblJobStarted.Text = "Job started at " + Convert.ToString(DateTime.Now);
                    lblJobEndTime.Text = "";
                    lblJobResults.Text = "";
                    updPanel.Update();
                    updRunResults.Update();
                    String sDaysBack = Convert.ToString(selDaysBack.Text);
                    var sResult = GetLaserFicheData(sDaysBack, sOperator);
                    if (sResult == "Success")
                    {
                        lblJobResults.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblJobResults.ForeColor = System.Drawing.Color.Red;
                        Logger.Error($"ERROR");
                    }
                    var endTime = Convert.ToString(DateTime.Now);
                    lblJobEndTime.Text = "Job completed at " + endTime;
                    lblJobEndTime.Visible = true;
                    lblJobResults.Text = sResult;
                    lblJobResults.Visible = true;
                    updRunResults.Update();
                }
                else
                {
                    lblJobStarted.Text = "";
                    lblJobEndTime.Text = "";
                    updPanel.Update();
                    updRunResults.Update();
                }
            }
            catch (Exception exception)
            {
                Logger.Error("ERROR: " + exception.Message + ": " + exception.StackTrace);
            }
            Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
        }

        protected string GetLaserFicheData(string sDaysBack, string sOperator)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetLaserFicheData BEGIN: {DateTime.Now.ToLocalTime()}");
            string sResult = "ERROR";
            SqlConnection cnCorpInt = new SqlConnection();
            SqlConnection cnLaserFiche = new SqlConnection();
            try
            {

                var response = new ProcessResponse($"Import LaserFiche data from E1 {ConfigurationManager.AppSettings["Environment"]}: ", true);
                LaserFicheSession laserFicheSession = new LaserFicheSession("Initialize", sOperator);
                var stepSubmit = new ProcessStep($@"Import LaserFiche data from E1");
                try
                {
                    cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
                    cnCorpInt.Open();
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR: " + cnCorpInt.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                    throw new Exception(e.Message);
                }
                laserFicheSession.Comments = $@"Requested by: {sOperator} Days back: {sDaysBack}";
                InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);
                try
                {
                    cnLaserFiche = ConnectionManager.GetSqlConnectionFromKey("LaserFiche");
                    cnLaserFiche.Open();
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR: " + cnLaserFiche.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                    throw new Exception(e.Message);
                }
                response.ConnectionStrings.Add(new ConnectionString("cnCorpInt", cnCorpInt.ConnectionString));
                response.ConnectionStrings.Add(new ConnectionString("cnLaserFiche", cnLaserFiche.ConnectionString));
                try
                {
                    var stepFetchE1Data = new ProcessStep($@"Fetch E1 Data");
                    decimal julianDate = GetJulianDate(sDaysBack);
                    var ds = FetchE1Data(julianDate, ref laserFicheSession, ref response);
                    var table = ds.Tables["F560411J"];
                    Logger.Info($"Rows: {table.Rows.Count}");
                    stepFetchE1Data.RowsAffected = table.Rows.Count;
                    stepFetchE1Data.Done();
                    response.ProcessSteps.Add(stepFetchE1Data);
                    laserFicheSession.TaskName = $@"Fetch data from PRODDTA.F560411J";
                    laserFicheSession.SelectRows = table.Rows.Count;
                    laserFicheSession.EndDate = DateTime.Now;
                    InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);
                    if (table.Rows.Count > 0)
                    {
                        TruncateTable(cnLaserFiche, ref laserFicheSession, ref response);
                        laserFicheSession.SelectRows = 0;
                        laserFicheSession.EndDate = DateTime.Now;
                        InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);

                        laserFicheSession.TaskName = $@"BulkCopy data into [LaserFicheDB].[dbo].[F560411J]";
                        laserFicheSession.Comments = "";
                        laserFicheSession.StartDate = DateTime.Now;
                        var step3 = new ProcessStep(laserFicheSession.TaskName);
                        var bulkResponse = new SqlBulkCopyResponse();
                        try
                        {
                            bulkResponse = BulkCopyManager.BulkTable("[dbo].[F560411J]", table, cnLaserFiche);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("ERROR: " + cnLaserFiche.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                            sResult = "Failed";
                            step3.Errors.Add(new Error(e));
                        }
                        finally
                        {
                            step3.RowsAffected = (int)bulkResponse.RowsCopied;
                            //step3.SqlBulkCopyResponses.Add(bulkResponse);
                            step3.Done();
                            response.RowsAffected = step3.RowsAffected;
                            response.AddStep(step3);
                            laserFicheSession.SelectRows = table.Rows.Count;
                            laserFicheSession.EndDate = DateTime.Now;
                            InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);
                            laserFicheSession.SelectRows = 0;
                        }

                        CheckDataFrom4000(cnLaserFiche, ref laserFicheSession, ref response);
                        InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);

                        InvoiceDataFrom4000(cnLaserFiche, ref laserFicheSession, ref response);
                        InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);

                        sResult = "Success";
                    }
                    response.Close();
                    SummarizeLaserFicheImport(response, laserFicheSession, sDaysBack);
                    NotifyLaserFicheImport(response, cnCorpInt);

                }
                catch (Exception err)
                {
                    sResult = "Failed";
                    stepSubmit.Errors.Add(new Error($"Error: {err.Message} : {err.StackTrace} "));
                    Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
                }
                finally
                {
                    cnLaserFiche.Close();
                    stepSubmit.Done();
                    response.ProcessSteps.Add(stepSubmit);
                    laserFicheSession.TaskName = $@"Finalize";
                    laserFicheSession.Comments = "";
                    laserFicheSession.SelectRows = 0;
                    laserFicheSession.EndDate = DateTime.Now;
                    InsertSQLProcessLog(laserFicheSession, cnCorpInt, ref response);
                    cnCorpInt.Close();
                    Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }
            Logger.Info("GetLaserFicheData END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return sResult;
        }

        private static void SummarizeLaserFicheImport(ProcessResponse response, LaserFicheSession laserFicheSession, string sDaysBack)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"SummarizeLaserFicheImport BEGIN: {startTime}");
            var stepSummarize = new ProcessStep("Summary Collections");
            try
            {
                var headerSummary = new
                {
                    response.TransferStatus,
                    response.TransferDate,
                    HasErrors = response.HasErrors(),
                    response.TransferMessage,
                    response.StartDate,
                    StartDateString = response.StartDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    response.EndDate,
                    EndDateString = response.EndDate.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                    ElapsedSeconds = (decimal)(response.EndDate - response.StartDate).TotalSeconds,
                    laserFicheSession.SessionGuid,
                    laserFicheSession.UserName,
                    laserFicheSession.Hostname,
                    sDaysBack
                };
                response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSummarize.Errors.Add(new Error(e));
            }
            finally
            {
                stepSummarize.Done();
                response.ProcessSteps.Add(stepSummarize);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        private static void NotifyLaserFicheImport(ProcessResponse response, SqlConnection cnCorpInt)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"NotifyLaserFicheImport BEGIN: {startTime}");
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                //var toConfigAddresses = ConfigurationManager.AppSettings["MailLaserFicheToAddress"];
                var toConfigAddresses = GetEmails(cnCorpInt, ref response);
                var serializedXml = response.Serialize();
                //Logger.Info($"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "etlMon.Common.Xslt.LaserFiche.xslt", Encoding.UTF8);
                Logger.Info($"Report XML: {xslOutput}");
                var subject = $@"LaserFiche Importer [{ConfigurationManager.AppSettings["PrimaryEnvironment"]}]";
                if (response.HasErrors())
                {
                    subject = $"Error - {subject}";
                }
                else if (response.Messages.Any())
                {
                    subject = $"Warning - {subject}";
                }
                Mail.SendHtml(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
                Logger.Info($"Email sent.");
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        protected static string GetEmails(SqlConnection cnCorpInt, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetLaserFicheEmailRecipients BEGIN: {startTime}");
            var step13 = new ProcessStep($@"Get Email Addresses");
            var emails = "";
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnCorpInt,
                    CommandTimeout = 0,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $@"[dbo].[usp_GetLaserFicheEmailRecipients]"
                };
                cmd.Parameters.Add("@ConfiguredValue", SqlDbType.VarChar, 1000);
                cmd.Parameters["@ConfiguredValue"].Direction = ParameterDirection.Output;
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                emails = Convert.ToString(cmd.Parameters["@ConfiguredValue"].Value);
                step13.RowsAffected = rows;
            }
            catch (Exception err)
            {
                step13.Errors.Add(new Error($"Error: {cnCorpInt.ConnectionString} {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                step13.Done();
                response.ProcessSteps.Add(step13);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            return emails;
        }

        protected static void InsertSQLProcessLog(LaserFicheSession laserFicheSession, SqlConnection cnCorpInt, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"InsertSQLProcessLog BEGIN: {startTime}");
            var step13 = new ProcessStep($@"Insert SQL Process Log");
            try
            {
                var cmd = InsertSQLProcessLog();
                cmd.Parameters["@SessionGUID"].Value = laserFicheSession.SessionGuid;
                cmd.Parameters["@Hostname"].Value = laserFicheSession.Hostname;
                cmd.Parameters["@PackageName"].Value = laserFicheSession.PackageName;
                cmd.Parameters["@Version"].Value = laserFicheSession.Version;
                cmd.Parameters["@TaskName"].Value = laserFicheSession.TaskName;
                cmd.Parameters["@UserName"].Value = laserFicheSession.UserName;
                cmd.Parameters["@Comments"].Value = laserFicheSession.Comments;
                cmd.Parameters["@InsertRows"].Value = laserFicheSession.InsertRows;
                cmd.Parameters["@UpdateRows"].Value = laserFicheSession.UpdateRows;
                cmd.Parameters["@DeleteRows"].Value = laserFicheSession.DeleteRows;
                cmd.Parameters["@SelectRows"].Value = laserFicheSession.SelectRows;
                cmd.Parameters["@StartDate"].Value = laserFicheSession.StartDate;
                cmd.Parameters["@EndDate"].Value = laserFicheSession.EndDate;
                cmd.Connection = cnCorpInt;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                step13.RowsAffected = rows;
            }
            catch (Exception e)
            {
                step13.Errors.Add(new Error($"Error: {cnCorpInt.ConnectionString} {e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }
            finally
            {
                step13.Done();
                //response.ProcessSteps.Add(step13);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        private static SqlCommand InsertSQLProcessLog()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO [LaserFicheLogging] 
                    (
                        [event_type],
                        [event_code],
                        [execution_instance_guid],
                        [machine_name],
                        [package_name],
                        [version_number],
                        [task_name],
                        [user_name],
                        [comments],
                        [insert_rows],
                        [update_rows],
                        [delete_rows],
                        [select_rows],
                        [start_date],
                        [end_date],
                        [create_date]
                    )
                    VALUES 
                    (
                        'Event',
                        0,
                        @SessionGUID,
                        @Hostname,
                        @PackageName,
                        @Version,
                        @TaskName,
                        @UserName,
                        @Comments,
                        @InsertRows,
                        @UpdateRows,
                        @DeleteRows,
                        @SelectRows,
                        @StartDate,
                        @EndDate,
                        GetDate()
                    )        
                "
            };
            cmd.Parameters.Add(new SqlParameter("@SessionGUID", SqlDbType.UniqueIdentifier));
            cmd.Parameters.Add(new SqlParameter("@Hostname", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@PackageName", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@Version", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@TaskName", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@UserName", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar));
            cmd.Parameters.Add(new SqlParameter("@InsertRows", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@UpdateRows", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@DeleteRows", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@SelectRows", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
            return cmd;
        }


        protected void CheckDataFrom4000(SqlConnection cnLaserFiche, ref LaserFicheSession laserFicheSession, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"CheckDataFrom4000 BEGIN: {startTime}");
            laserFicheSession.TaskName = $@"EXEC [LaserFicheDB].[dbo].[usp_CheckDataFrom4000]";
            laserFicheSession.StartDate = startTime;
            var stepCheckDataFrom4000 = new ProcessStep(laserFicheSession.TaskName);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnLaserFiche,
                    CommandTimeout = 0,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $@"[dbo].[usp_CheckDataFrom4000]"
                };
                Logger.Info(Logging.DbCmdString(cmd));
                laserFicheSession.Comments = cmd.CommandText;
                laserFicheSession.InsertRows = cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                stepCheckDataFrom4000.Errors.Add(new Error($"Error: {cnLaserFiche.ConnectionString} {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                stepCheckDataFrom4000.RowsAffected = laserFicheSession.InsertRows;
                stepCheckDataFrom4000.Done();
                response.ProcessSteps.Add(stepCheckDataFrom4000);
                laserFicheSession.EndDate = DateTime.Now;
            }
        }

        protected void InvoiceDataFrom4000(SqlConnection cnLaserFiche, ref LaserFicheSession laserFicheSession, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"InvoiceDataFrom4000 BEGIN: {startTime}");
            laserFicheSession.TaskName = $@"EXEC [LaserFicheDB].[dbo].[usp_InvoiceDataFrom4000]";
            laserFicheSession.StartDate = startTime;
            var stepInvoiceDataFrom4000 = new ProcessStep(laserFicheSession.TaskName);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnLaserFiche,
                    CommandTimeout = 0,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $@"[dbo].[usp_InvoiceDataFrom4000]"
                };
                Logger.Info(Logging.DbCmdString(cmd));
                laserFicheSession.Comments = cmd.CommandText;
                laserFicheSession.InsertRows = cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                stepInvoiceDataFrom4000.Errors.Add(new Error($"Error: {cnLaserFiche.ConnectionString} {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                stepInvoiceDataFrom4000.RowsAffected = laserFicheSession.InsertRows;
                stepInvoiceDataFrom4000.Done();
                response.ProcessSteps.Add(stepInvoiceDataFrom4000);
                laserFicheSession.EndDate = DateTime.Now;
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        private static DataSet FetchE1Data(decimal julianDate, ref LaserFicheSession laserFicheSession, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetE1Load BEGIN: {startTime}");
            laserFicheSession.StartDate = startTime;
            var e1Session = new BusinessSystemSession();
            var ds = new DataSet();
            try
            {
                e1Session = BusinessSystemSessionManager.GetBusinessSession();
                response.ConnectionStrings.Add(new ConnectionString("cnE1", e1Session.DbConnection.ConnectionString));
                var cmd = new iDB2Command
                {
                    Connection = e1Session.DbConnection,
                    CommandType = CommandType.Text,
                    CommandTimeout = 0
                };
                cmd.CommandText = $@"
                    SELECT Q01.ABAN8 ,
						        Q01.ABALPH ,
						        Q01.ABAC08 ,
						        Q02.RPDOC ,
						        Q02.RPAN8 ,
						        Q02.RPDIVJ ,
						        Q02.RPVINV ,
						        Q02.RPMCU ,
						        Q02.RPICU ,
						        Q02.RPDGJ ,
						        Q02.RPAG ,
						        Q02.RPOBJ ,
						        Q02.RPSBLT ,
						        Q02.RPPO ,
						        Q02.RPPDCT ,
						        Q02.RPUPMJ ,
						        Q03.RNDOC ,
						        Q03.RNPYID ,
						        Q04.RMPYID ,
						        Q04.RMDOCM ,
						        Q04.RMDMTJ
						 FROM   F0101 AS Q01
						        LEFT OUTER JOIN F0411 AS Q02
						            ON ( Q01.ABAN8 = Q02.RPAN8 )
						        LEFT OUTER JOIN F0414 AS Q03
						            ON ( Q02.RPDOC = Q03.RNDOC
						                 AND Q02.RPDCT = Q03.RNDCT
						                 AND Q02.RPKCO = Q03.RNKCO
						                 AND Q02.RPSFX = Q03.RNSFX
						                 AND Q02.RPSFXE = Q03.RNSFXE
						               )
						        LEFT OUTER JOIN F0413 AS Q04
						            ON ( Q03.RNPYID = Q04.RMPYID )   
 						WHERE Q02.RPUPMJ > {julianDate} FOR READ ONLY       
                ";
                laserFicheSession.Comments = cmd.CommandText;
                Logger.Info(Logging.DbCmdString(cmd));
                var da = new iDB2DataAdapter {SelectCommand = cmd};
                da.Fill(ds, "F560411J");
                return ds;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e1Session.DbConnection.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                e1Session.Close();
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }

        protected void TruncateTable(SqlConnection cnLaserFiche, ref LaserFicheSession laserFicheSession, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"TruncateTable BEGIN: {startTime}");
            laserFicheSession.TaskName = $@"TRUNCATE TABLE [LaserFicheDB].[dbo].[F560411J]";
            laserFicheSession.StartDate = startTime;
            laserFicheSession.Comments = "";
            var stepTruncateTable = new ProcessStep(laserFicheSession.TaskName);
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnLaserFiche,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"TRUNCATE TABLE [LaserFicheDB].[dbo].[F560411J]"
                };
                Logger.Info(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                stepTruncateTable.Errors.Add(new Error($"Error: {cnLaserFiche} {err.Message} : {err.StackTrace} "));
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                stepTruncateTable.RowsAffected = 0;
                stepTruncateTable.Done();
                response.ProcessSteps.Add(stepTruncateTable);
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
        }
        
        private static decimal GetJulianDate(string sDaysBack)
        {
            decimal julianDate;
            int daysLookBack = Convert.ToInt16(sDaysBack);
            var lookbackDate = DateTime.Today.AddDays(-daysLookBack);
            julianDate = Common.Utilities.Common.CastToE1Date(lookbackDate);
            return julianDate;
        }

        protected Boolean fnIsInProgress(Int32 iMinutesLookback)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnIsInProgress BEGIN: {startTime}");
            Boolean bIsInProgress = false;
            var cnCorpInt = ConnectionManager.GetSqlConnectionFromKey("CORP.INTEGRATION");
            cnCorpInt.Open();
            try
            {
                SqlCommand cmdPackage = new SqlCommand("[uspGetIsLaserFicheInProgress]", cnCorpInt);
                cmdPackage.CommandType = CommandType.StoredProcedure;
                SqlParameter pMinutesLookback = cmdPackage.Parameters.Add("@iMinutesLookback", SqlDbType.Int);
                pMinutesLookback.Direction = ParameterDirection.Input;
                pMinutesLookback.Value = iMinutesLookback;
                Logger.Info(Logging.DbCmdString(cmdPackage));
                using (SqlDataReader drItems = cmdPackage.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (drItems.Read())
                    {
                        //Int32 iFinalEventID = Convert.ToInt32(drItems["final_event_id"]);
                        //if (iFinalEventID == 0)
                        //{
                            bIsInProgress = true;
                            lblJobResults.Text = "Job already in progress;<br/><br/> " +
                                                 Convert.ToString(drItems["comments"]);
                        //}
                        //else
                        //{
                        //    bIsInProgress = false;
                        //}
                    }

                    drItems.Close();
                }

            }
            catch (Exception err)
            {
                Logger.Error($"Error: {err.Message} : {err.StackTrace} ");
            }
            finally
            {
                cnCorpInt.Close();
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            Logger.Info($"fnIsInProgress RESULT: {bIsInProgress.ToString()}");
            return bIsInProgress;
        }
    }
}
