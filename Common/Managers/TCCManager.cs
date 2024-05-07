using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types.Archie;
using etlMon.Common.Types.Enterprise;
using etlMon.Common.Types.Response;
using etlMon.Common.Utilities;
using NLog;
// ReSharper disable InconsistentNaming

namespace etlMon.Common.Managers.Archie
{
    public class TCCManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse TCCExportInvoices(SqlConnection cnTCCSUPPORT, string plantCode, string custNum,
            DateTime startDate, DateTime endDate, string user, string email)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response =
                new ProcessResponse($"TCC Export Invoices {ConfigurationManager.AppSettings["PrimaryEnvironment"]}",
                    true);
            response.ConnectionStrings.Add(new ConnectionString("cnTCCSUPPORT", cnTCCSUPPORT.ConnectionString));
            var tccExportedInvoicesFilePathCorpDrop =
                ConfigurationManager.AppSettings["TCCExportedInvoicesFilePathCorpDrop"];
            response.ConnectionStrings.Add(new ConnectionString("TCCExportedInvoicesFilePathCorpDrop",
                tccExportedInvoicesFilePathCorpDrop));
            var exportGUID = Guid.NewGuid();

            var plant = PlantManager.GetPlant(plantCode);
            response.Plant = plant;

            var tccExportPlantPath = $@"{tccExportedInvoicesFilePathCorpDrop}\{plant.PlantCode.ToUpper()}";
            DirectoryInfo tccExportDirPDF = new DirectoryInfo(tccExportPlantPath);
            if (tccExportDirPDF.Exists)
            {
                var filesPDF = tccExportDirPDF.GetFiles("*.pdf");
                foreach (FileInfo file in filesPDF)
                {
                    file.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory(tccExportPlantPath);
            }

            var docs = new TCCUnions();
            try
            {
                var step1GetOldestDeleteDates = new ProcessStep($@"Step 1. Get the Invoice records");
                try
                {
                    using (IDataReader reader = GetInvoices(cnTCCSUPPORT, plant.PlantNum, custNum, startDate, endDate))
                    {
                        while (reader.Read())
                        {
                            var doc = new TCCUnion();
                            doc.Filter = CastDbNull.To(reader["Filter"], string.Empty);
                            doc.fsdDocID = CastDbNull.To(reader["fsdDocID"], 0);
                            doc.Image = CastDbNull.To<byte[]>(reader["Image"], new byte[0]);
                            doc.DocID = CastDbNull.To(reader["DocID"], 0);
                            doc.DOCSUBJECT = CastDbNull.To(reader["DOCSUBJECT"], string.Empty);
                            doc.DOCLOCATION = CastDbNull.To(reader["DOCLOCATION"], string.Empty);
                            doc.DOCNUMBER1 = CastDbNull.To(reader["DOCNUMBER1"], string.Empty);
                            doc.DOCNUMBER2 = CastDbNull.To(reader["DOCNUMBER2"], string.Empty);
                            doc.DOCDESC1 = CastDbNull.To(reader["DOCDESC1"], string.Empty);
                            doc.DOCDATE_CALC = CastDbNull.To(reader["DOCDATE_CALC"], new DateTime(1900, 1, 1));
                            doc.DOCDATE_STR = doc.DOCDATE_CALC.ToString("M/dd/yyyy",
                                CultureInfo.CreateSpecificCulture("en-US"));
                            doc.LOADNUM = CastDbNull.To(reader["LOADNUM"], string.Empty);
                            doc.BOL = CastDbNull.To(reader["BOL"], string.Empty);
                            doc.PO1 = CastDbNull.To(reader["PO1"], string.Empty);
                            doc.DOCDATE = CastDbNull.To(reader["DOCDATE"], string.Empty);
                            docs.Add(doc);
                        }

                        reader.Close();
                    }

                    Logger.Info($@"Rows: {docs.Count}");
                    step1GetOldestDeleteDates.RowsAffected = docs.Count;
                    response.RowsAffected = step1GetOldestDeleteDates.RowsAffected;
                }
                catch (Exception e)
                {
                    step1GetOldestDeleteDates.Errors.Add(new Error(e));
                    Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                    throw new Exception(e.Message);
                }

                step1GetOldestDeleteDates.Done();
                response.ProcessSteps.Add(step1GetOldestDeleteDates);

                if (!step1GetOldestDeleteDates.HasErrors())
                {
                    var step2 = new ProcessStep($@"Step 2. Export PDF files");
                    try
                    {
                        foreach (var doc in docs)
                        {
                            doc.PDFPath =
                                $@"{tccExportPlantPath}\{doc.DOCNUMBER1}_{doc.DOCSUBJECT.Replace(" ", "_")}.pdf";
                            // Delete the PDF file
                            if (File.Exists(doc.PDFPath))
                            {
                                try
                                {
                                    File.Delete(doc.PDFPath);
                                }
                                catch (Exception e)
                                {
                                    step2.Errors.Add(new Error($"{doc.PDFPath} {e.Message} {e.StackTrace}"));
                                    InsertDocDeleteError(cnTCCSUPPORT, doc, $"{doc.PDFPath} {e.Message}", exportGUID);
                                }
                            }

                            FileStream fs = new FileStream(doc.PDFPath, FileMode.CreateNew, FileAccess.Write);
                            fs.Write(doc.Image, 0, doc.Image.Length);
                            fs.Flush();
                            fs.Close();
                            step2.RowsAffected++;

                        }
                    }
                    catch (Exception e)
                    {
                        step2.Errors.Add(new Error(e));
                    }

                    step2.Done();
                    response.ProcessSteps.Add(step2);
                }

                var step3 = new ProcessStep($@"Step 3. Capture errors");
                try
                {
                    step3.RowsAffected = 0;
                    foreach (var step in response.ProcessSteps)
                    {
                        foreach (var err in step.Errors)
                        {
                            InsertProcessError(cnTCCSUPPORT, "TCCExportInvoices", err, exportGUID);
                            step3.RowsAffected += 1;
                        }
                    }
                }
                catch (Exception e)
                {
                    step3.Errors.Add(new Error(e));
                }

                step3.Done();
                response.ProcessSteps.Add(step3);

                response.Close();
                SummarizeTCCExportInvoices(response, docs, exportGUID, custNum, startDate, endDate, user, email);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) +
                            " SECONDS");
            }

            return response;
        }

        public static IDataReader GetInvoices(SqlConnection cnTCCSUPPORT, string plantNum, string custNum,
            DateTime startDate, DateTime endDate)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnTCCSUPPORT,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                    SELECT 
                        Filter,
                        fsdDocID,
                        [Image],
                        DocID,
                        DOCSUBJECT,
                        DOCLOCATION,
                        DOCNUMBER1,
                        DOCNUMBER2,
                        DOCDESC1,
                        DOCDATE_CALC,
                        LOADNUM,
                        BOL,
                        PO1,
                        DOCDATE
                    FROM [TCCSUPPORT].[dbo].[FINAL_INVOICES_IMAGES]
                    WHERE 
                        DOCLOCATION = @PlantNum AND
                        DOCNUMBER2 = @CustNum AND
                        DOCDATE_CALC >= @StartDate AND DOCDATE_CALC <= @EndDate AND
                        DOCSUBJECT LIKE '%INVOICE%'
                    ORDER BY DOCNUMBER1, DOCSUBJECT
                    "
                };
                // Plant Num
                cmd.Parameters.Add(new SqlParameter("@PlantNum", SqlDbType.NVarChar, 20));
                cmd.Parameters["@PlantNum"].Value = plantNum;
                // Cust Num
                cmd.Parameters.Add(new SqlParameter("@CustNum", SqlDbType.NVarChar, 20));
                cmd.Parameters["@CustNum"].Value = custNum;
                // Start Date
                cmd.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
                cmd.Parameters["@StartDate"].Value = startDate;
                // End Date
                cmd.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));
                cmd.Parameters["@EndDate"].Value = endDate;
                Logger.Info(Logging.DbCmdString(cmd));

                // Reader
                return cmd.ExecuteReader(CommandBehavior.SingleResult);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SummarizeTCCExportInvoices(ProcessResponse response, TCCUnions docs, Guid exportGUID, string custNum, DateTime startDate, DateTime endDate, string user, string email)
        {
            var stepSummarize = new ProcessStep("Summary Collections");
            try
            {
                var custName = "";
                if (docs.Any())
                {
                    response.SerializedObjects.Add("ResultLine", docs, "ResultLines");
                    custName = $@"{docs.First().DOCDESC1}";
                }
                var querySummary = new
                {
                    user,
                    plant = $@"{response.Plant.PlantCode} - {response.Plant.PlantNum}",
                    Customer = (string.IsNullOrEmpty(custName)) ? $@"{custNum}" : $@"{custNum} - {custName}",
                    StartDateString = startDate.ToString("M/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US")),
                    EndDateString = endDate.ToString("M/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US")),
                    response.RowsAffected,
                    email
                };
                response.SerializedObjects.Add(new SerializedObject("QueryNode", XmlSerializationUtility.SerializeAnonymousType(querySummary, "Query")));

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
                    exportGUID,
                    response.RowsAffected
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
            }
        }

        public static int InsertDocDeleteError(SqlConnection cnTCCSUPPORT, TCCUnion doc, string errMsg, Guid deleteGUID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            int rowCount;
            try
            {
                // Command
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $"dbo.usp_InsertDocumentDeleteError",
                    CommandTimeout = 30
                };
                cmd.Parameters.AddWithValue("@fsdDocID", doc.fsdDocID);
                cmd.Parameters.AddWithValue("@errMsg", errMsg);
                cmd.Parameters.AddWithValue("@deleteGUID", deleteGUID);
                Logger.Info(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) +
                            " SECONDS");
            }

            return rowCount;
        }

        public static int InsertProcessError(SqlConnection cnTCCSUPPORT, string process, Error err, Guid deleteGUID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            //var response = new ProcessResponse($"Insert TCC Process Errors {ConfigurationManager.AppSettings["PrimaryEnvironment"]}.", true);
            int rowCount;
            try
            {
                // Command
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = $"dbo.usp_InsertProcessError",
                    CommandTimeout = 30
                };
                cmd.Parameters.AddWithValue("@process", process);
                cmd.Parameters.AddWithValue("@errMsg", err.Message);
                cmd.Parameters.AddWithValue("@errSource", err.Source);
                cmd.Parameters.AddWithValue("@errStackTrace", err.StackTrace);
                cmd.Parameters.AddWithValue("@deleteGUID", deleteGUID);
                Logger.Info(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                Logger.Trace("ROWS: " + rowCount);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) +
                            " SECONDS");
            }

            return rowCount;
        }


    }
}