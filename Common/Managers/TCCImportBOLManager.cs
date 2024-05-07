using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using etlMon.Common.Types.Archie;
using etlMon.Common.Types.Enterprise;
using etlMon.Common.Types.Response;
using etlMon.Common.Utilities;
using NLog;

namespace etlMon.Common.Managers
{
    public class TCCImportBOLManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ProcessResponse TCCImportBOL(SqlConnection cnTCCSUPPORT, SqlConnection cnTCC, BOL bol, string operatorName, string email)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"TCC Import BOL {ConfigurationManager.AppSettings["PrimaryEnvironment"]}", true);
            response.ConnectionStrings.Add(new ConnectionString("cnTCCSUPPORT", cnTCCSUPPORT.ConnectionString));
            try
            {
                UpdateSQLBOLEmailAttachment(bol, cnTCCSUPPORT, operatorName, ref response);
                
                MapBOLToSQL(ref bol, cnTCCSUPPORT, cnTCC, ref response);
                DeleteExistingDocs(ref bol, cnTCC, ref response);
                InsertSQLDocHeader(ref bol, cnTCC, ref response);
                InsertSQLBOL(ref bol, cnTCC, ref response);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            }

            return response;
        }

        public static int UpdateSQLBOLEmailAttachment(BOL bol, SqlConnection cnTCCSUPPORT, string operatorName, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"UpdateSQLBOLEmailAttachment BEGIN: {startTime}");
            var stepInsertSQLEmail = new ProcessStep($@"Step 4. Update SQL BOL Email Attachment {bol.AttachmentGuid}");
            int rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_UpdateImportBOLEmailAttachment",
                    CommandTimeout = 0
                };
                cmd.Parameters.AddWithValue("@ID", bol.AttachmentId);
                cmd.Parameters.AddWithValue("@AttachmentGuid", bol.AttachmentGuid);
                cmd.Parameters.AddWithValue("@BOLCUSTNUM", bol.BOLCUSTNUM);
                cmd.Parameters.AddWithValue("@BOLCUSTNAM", bol.BOLCUSTNAM);
                cmd.Parameters.AddWithValue("@BOLDATE", bol.BOLDATE);
                cmd.Parameters.AddWithValue("@BOLLOADNO", bol.BOLLOADNO);
                cmd.Parameters.AddWithValue("@BOLCARRIER", bol.BOLCARRIER);
                cmd.Parameters.AddWithValue("@BOLBOLNUM", bol.BOLBOLNUM);
                cmd.Parameters.AddWithValue("@BOLDOCNUM", bol.BOLDOCNUM);
                cmd.Parameters.AddWithValue("@ProcessingStatus", bol.ProcessingStatus);
                cmd.Parameters.AddWithValue("@ProcessingReason", bol.ProcessingReason);
                cmd.Parameters.AddWithValue("@CompletedBy", operatorName);
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                response.RowsAffected = rowCount;
            }
            catch (Exception e)
            {
                stepInsertSQLEmail.Errors.Add(new Error($"{cnTCCSUPPORT.ConnectionString}  {e.Message} : {e.StackTrace}"));
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                stepInsertSQLEmail.RowsAffected = 1;
                stepInsertSQLEmail.Done();
                response.ProcessSteps.Add(stepInsertSQLEmail);
            }
            return rowCount;
        }

        public static byte[] GetBOLImage(BOL bol, SqlConnection cnTCCSUPPORT, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"RetrieveSQLBOLEmailAttachment BEGIN: {startTime}");
            byte[] image = new byte[0];
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.Text,
                    CommandTimeout = 0
                };
                cmd.CommandText = $@"
                    SELECT [Image]
                    FROM [dbo].[ImportBOLAttachments]
                    WHERE [AttachmentGuid] = '{bol.AttachmentGuid}'";
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        image = CastDbNull.To(reader["Image"], new byte[0]);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return image;
        }

        public static void MapBOLToSQL(ref BOL bol, SqlConnection cnTCCSUPPORT, SqlConnection cnTCC, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"MapBOLToSQL  BEGIN: {startTime}");
            try
            {
                var catID = GetCatID(cnTCC);
                try
                {
                    // FS_Documents
                    bol.DocID = GetNewDocID(cnTCC);
                    bol.Version = 1;
                    bol.Image = GetBOLImage(bol, cnTCCSUPPORT, ref response);
                    bol.DeleteFlag = null;
                    bol.SatID = 1;
                    bol.DocType = "pdf";
                    bol.DateStored = DateTime.Now;
                    bol.DateDelete = null;
                    bol.OfflinePath = null;
                    bol.OfflineFlag = null;
                    bol.OfflineDate = null;
                    bol.CatID = catID;

                    // BOL
                    bol.FS_InboxOwner = null;
                    bol.FS_InboxSubject = "";
                    bol.FS_ReprintServer = null;
                    bol.FS_ReprintPrinter = null;
                    bol.bolCustNum = bol.BOLCUSTNUM;
                    bol.bolCustName = bol.BOLCUSTNAM;
                    bol.bolDateCalc = DateTime.TryParse(bol.BOLDATE, out var bolDate) ? bolDate : new DateTime(1900, 1, 1);
                    bol.bolLoadNum = bol.BOLLOADNO;
                    bol.bolCarrier = bol.BOLCARRIER;
                    bol.bolBOLNum = bol.BOLBOLNUM;
                    bol.bolDocNum = bol.BOLDOCNUM;
                    bol.bolDate = bol.bolDateCalc.ToString("yyyyMMdd");
                    bol.bolSubject = "Signed BOL";
                    bol.bolDocStamp = bol.bolDateCalc.ToString("MM/dd/yy H:mm:ss tt");
                    bol.bolDocTitle = "R5842530A";

                    bol.ProcessingStatus = "Mapped";
                    UpdateSQLBOLEmailAttachmentMapped(bol, cnTCCSUPPORT, ref response);
                }
                catch (Exception e)
                {
                    Logger.Error("ERROR:  " + e.Message + ": " + e.StackTrace);
                    throw new ApplicationException(e.Message + ": " + e.StackTrace);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR:  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(e.Message + ": " + e.StackTrace);
            }

            finally

            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            //return bols;
        }

        public static int GetCatID(SqlConnection cnTCC)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetCatID BEGIN: {startTime}");
            int categoryID = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"
                        SELECT [CategoryID]
                          FROM [TCC].[webrepadmin].[FS_Categories]
                          WHERE [CategoryName] = @CategoryName"
                };
                cmd.Parameters.Add(new SqlParameter("@CategoryName", SqlDbType.NVarChar, 50));
                cmd.Parameters["@CategoryName"].Value = ConfigurationManager.AppSettings["BOLCategoryName"];
                cmd.Connection = cnTCC;
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        categoryID = CastDbNull.To(reader["CategoryID"], 0);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return categoryID;
        }


        public static int GetNewDocID(SqlConnection cnTCC)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetNewDocID BEGIN: {startTime}");
            int docID;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnTCC,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "webrepadmin.FS_sp_GetNewID",
                    CommandTimeout = 0
                };
                cmd.Parameters.Add("@p_NewID", SqlDbType.Int).Direction = ParameterDirection.Output;
                Logger.Info(Logging.DbCmdString(cmd));
                cmd.ExecuteNonQuery();
                docID = Convert.ToInt32(cmd.Parameters["@p_NewID"].Value);
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return docID;
        }


        public static int UpdateSQLBOLEmailAttachmentMapped(BOL bol, SqlConnection cnTCCSUPPORT, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"UpdateSQLBOLEmailAttachmentImage BEGIN: {startTime}");
            var stepInsertSQLEmail = new ProcessStep($@"Step 4. Update SQL BOL Email Attachment Image {bol.AttachmentGuid}");
            int rowCount = 0;
            try
            {
                var cmd = new SqlCommand
                {
                    Connection = cnTCCSUPPORT,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_UpdateImportBOLEmailAttachmentMapped",
                    CommandTimeout = 0
                };
                cmd.Parameters.AddWithValue("@ID", bol.AttachmentId);
                cmd.Parameters.AddWithValue("@AttachmentGuid", bol.AttachmentGuid);
                cmd.Parameters.AddWithValue("@ProcessingStatus", bol.ProcessingStatus);
                cmd.Parameters.AddWithValue("@ProcessingReason", bol.ProcessingReason);
                cmd.Parameters.AddWithValue("@DocId", bol.DocID);
                cmd.Parameters.AddWithValue("@DOCSUBJECT", bol.bolSubject);
                cmd.Parameters.AddWithValue("@DOCLOCATION", bol.bolMCU ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCNUMBER1", bol.bolDocNum ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCNUMBER2", bol.bolCustNum ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCDESC1", bol.bolCustName ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCDATE_CALC", bol.bolDateCalc);
                cmd.Parameters.AddWithValue("@LOADNUM", bol.BOLLOADNO ?? string.Empty);
                cmd.Parameters.AddWithValue("@CARRIER", bol.bolCarrier ?? string.Empty);
                cmd.Parameters.AddWithValue("@BOL", bol.bolBOLNum ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCDATE", bol.bolDate ?? string.Empty);
                cmd.Parameters.AddWithValue("@DOCSTAMP", bol.bolDocStamp ?? string.Empty);
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                rowCount = cmd.ExecuteNonQuery();
                response.RowsAffected = rowCount;
            }
            catch (Exception e)
            {
                stepInsertSQLEmail.Errors.Add(new Error($"{cnTCCSUPPORT.ConnectionString}  {e.Message} : {e.StackTrace}"));
                Logger.Error("ERROR: " + cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                //throw new ApplicationException(cnTCCSUPPORT.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
                stepInsertSQLEmail.RowsAffected = 1;
                stepInsertSQLEmail.Done();
                response.ProcessSteps.Add(stepInsertSQLEmail);
            }
            return rowCount;
        }

        public static void DeleteExistingDocs(ref BOL bol, SqlConnection cnTCC, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"DeleteExistingDocs BEGIN: {startTime}");
            var step6a = new ProcessStep($@"Step 9. Delete Existing Documents");
            try
            {
                    var docIDs = FindExistingSQLBOL(cnTCC, bol);
                    foreach (var docID in docIDs)
                    {
                        DeleteSQLBOL(cnTCC, docID);
                        DeleteSQLDocHeader(cnTCC, docID);
                        bol.IsExistingDoc = true;
                        step6a.RowsAffected += 1;
                    }
            }
            catch (Exception e)
            {
                step6a.Errors.Add(new Error($"{e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }

            step6a.Done();
            response.ProcessSteps.Add(step6a);

        }

        public static List<int> FindExistingSQLBOL(SqlConnection cnTCC, BOL bol)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"FindExistingSQLBOL [{bol.bolMCU}] [{bol.bolLoadNum}] BEGIN: {startTime}");
            var response =
                new ProcessResponse(
                    $"Find Existing SQL BOL {ConfigurationManager.AppSettings["Environment"]}: document {bol.bolDocNum}",
                    true);
            List<int> docIDs = new List<int>();
            if (string.IsNullOrEmpty(bol.bolMCU)) bol.bolMCU = string.Empty;
            //{
            try
            {
                var cmd = FindExistingSQLBOL();
                //cmd.Parameters["@DOCLOCATION"].Value = bol.bolMCU;
                cmd.Parameters["@LOADNUM"].Value = bol.bolLoadNum;
                cmd.Connection = cnTCC;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    var rowCount = 0;
                    while (reader.Read())
                    {
                        var docID = CastDbNull.To(reader["DocID"], 0);
                        docIDs.Add(docID);
                    }

                    response.RowsAffected = rowCount;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }
            //}

            return docIDs;
        }

        private static SqlCommand FindExistingSQLBOL()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    SELECT [DocID] FROM [TCC].[webrepadmin].[Manufacturing]
                    WHERE [DOCSUBJECT] = 'Signed BOL'
                        AND [LOADNUM] = @LOADNUM
                "
            };
            cmd.Parameters.Add(new SqlParameter("@LOADNUM", SqlDbType.NVarChar));
            //cmd.Parameters.Add(new SqlParameter("@DOCLOCATION", SqlDbType.Int));
            //AND[DOCLOCATION] = @DOCLOCATION
            return cmd;
        }

        public static int DeleteSQLBOL(SqlConnection cnTCC, int docID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            var response =
                new ProcessResponse($"Delete SQL BOL {ConfigurationManager.AppSettings["Environment"]}: DocID {docID}",
                    true);
            int rowCount = 0;
            try
            {
                var cmd = DeleteSQLBOL();
                cmd.Parameters["@DocID"].Value = docID;
                cmd.Connection = cnTCC;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return rowCount;
        }

        private static SqlCommand DeleteSQLBOL()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    DELETE FROM [TCC].[webrepadmin].[Manufacturing]
                    WHERE [DocID] = @DocID
                "
            };
            cmd.Parameters.Add(new SqlParameter("@DocID", SqlDbType.Int));
            return cmd;
        }

        private static SqlCommand DeleteSQLDocHeader()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    DELETE FROM [TCC].[webrepadmin].[FS_Documents]
                    WHERE [DocID] = @DocID
                "
            };
            cmd.Parameters.Add(new SqlParameter("@DocID", SqlDbType.Int));
            return cmd;
        }

        public static int DeleteSQLDocHeader(SqlConnection cnTCC, int docID)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            var response =
                new ProcessResponse(
                    $"Delete SQL Doc Header {ConfigurationManager.AppSettings["Environment"]}: DocID {docID}", true);
            int rowCount = 0;
            try
            {
                var cmd = DeleteSQLDocHeader();
                cmd.Parameters["@DocID"].Value = docID;
                cmd.Connection = cnTCC;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return rowCount;
        }

        public static void InsertSQLDocHeader(ref BOL bol, SqlConnection cnTCC, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"InsertSQLDocHeader BEGIN: {startTime}");
            var step6 = new ProcessStep($@"Step 10. Insert SQL Document Header");
            try
            {
                InsertSQLDocHeader(cnTCC, bol);
                bol.IsInsertedDocHeader = true;
                step6.RowsAffected += 1;
            }
            catch (Exception e)
            {
                step6.Errors.Add(new Error($"{e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }

            step6.Done();
            response.ProcessSteps.Add(step6);

        }

        public static int InsertSQLDocHeader(SqlConnection cnTCC, BOL bol)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            var response =
                new ProcessResponse(
                    $"Insert SQL Doc Header {ConfigurationManager.AppSettings["Environment"]}: document {bol.bolDocNum}",
                    true);
            int rowCount = 0;
            try
            {
                var cmd = InsertSQLDocHeader();
                cmd.Parameters["@DocID"].Value = bol.DocID;
                cmd.Parameters["@Version"].Value = bol.Version;
                cmd.Parameters["@Image"].Value = bol.Image;
                cmd.Parameters["@DeleteFlag"].Value = bol.DeleteFlag ?? (object)DBNull.Value;
                cmd.Parameters["@SatID"].Value = bol.SatID.ToString();
                cmd.Parameters["@DocType"].Value = bol.DocType;
                cmd.Parameters["@DateStored"].Value = bol.DateStored;
                cmd.Parameters["@DateDelete"].Value = bol.DateDelete ?? (object)DBNull.Value;
                cmd.Parameters["@OfflinePath"].Value = bol.OfflinePath ?? (object)DBNull.Value;
                cmd.Parameters["@OfflineFlag"].Value = bol.OfflineFlag ?? (object)DBNull.Value;
                cmd.Parameters["@OfflineDate"].Value = bol.OfflineDate ?? (object)DBNull.Value;
                cmd.Parameters["@CatID"].Value = bol.CatID;
                cmd.Connection = cnTCC;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return rowCount;
        }

        private static SqlCommand InsertSQLDocHeader()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO [TCC].[webrepadmin].[FS_Documents]
                    (
                        [DocID]
                        ,[Version]
                        ,[Image]
                        ,[DeleteFlag]
                        ,[SatID]
                        ,[DocType]
                        ,[DateStored]
                        ,[DateDelete]
                        ,[OfflinePath]
                        ,[OfflineFlag]
                        ,[OfflineDate]
                        ,[CatID]
                    )
                    VALUES 
                    (
                        @DocID
                        ,@Version
                        ,@Image
                        ,@DeleteFlag
                        ,@SatID
                        ,@DocType
                        ,@DateStored
                        ,@DateDelete
                        ,@OfflinePath
                        ,@OfflineFlag
                        ,@OfflineDate
                        ,@CatID
                    )
                    "
            };
            cmd.Parameters.Add(new SqlParameter("@DocID", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@Version", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@Image", SqlDbType.Image));
            cmd.Parameters.Add(new SqlParameter("@DeleteFlag", SqlDbType.SmallInt));
            cmd.Parameters.Add(new SqlParameter("@SatID", SqlDbType.Char));
            cmd.Parameters.Add(new SqlParameter("@DocType", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DateStored", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@DateDelete", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@OfflinePath", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@OfflineFlag", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@OfflineDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@CatID", SqlDbType.Int));
            return cmd;
        }

        public static void InsertSQLBOL(ref BOL bol, SqlConnection cnTCC, ref ProcessResponse response)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"InsertSQLBOL BEGIN: {startTime}");
            var step7 = new ProcessStep($@"Step 11. Insert SQL BOL");
            try
            {
                InsertSQLBOL(cnTCC, bol);
                bol.IsInsertedBOL = true;
                step7.RowsAffected += 1;
            }
            catch (Exception e)
            {
                step7.Errors.Add(new Error($"{e.Message} : {e.StackTrace} "));
                Logger.Error($"Error: {e.Message} : {e.StackTrace} ");
            }

            step7.Done();
            response.ProcessSteps.Add(step7);

        }

        public static int InsertSQLBOL(SqlConnection cnTCC, BOL bol)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {startTime}");
            var response =
                new ProcessResponse(
                    $"Insert SQL BOL {ConfigurationManager.AppSettings["Environment"]}: document {bol.bolDocNum}",
                    true);
            int rowCount = 0;
            try
            {
                var cmd = InsertSQLBOL();
                cmd.Parameters["@DocID"].Value = bol.DocID;
                cmd.Parameters["@SatID"].Value = bol.SatID;
                cmd.Parameters["@DateStored"].Value = bol.DateStored;
                cmd.Parameters["@DateDelete"].Value = bol.DateDelete ?? (object)DBNull.Value;
                cmd.Parameters["@DocType"].Value = bol.DocType;
                cmd.Parameters["@OfflineDate"].Value = bol.OfflineDate ?? (object)DBNull.Value;
                cmd.Parameters["@DeleteFlag"].Value = bol.DeleteFlag ?? 0;
                cmd.Parameters["@FS_InboxOwner"].Value = bol.FS_InboxOwner ?? (object)DBNull.Value;
                cmd.Parameters["@FS_InboxSubject"].Value = bol.FS_InboxSubject ?? (object)DBNull.Value;
                cmd.Parameters["@FS_ReprintServer"].Value = bol.FS_ReprintServer ?? (object)DBNull.Value;
                cmd.Parameters["@FS_ReprintPrinter"].Value = bol.FS_ReprintPrinter ?? (object)DBNull.Value;
                cmd.Parameters["@DOCSUBJECT"].Value = bol.bolSubject;
                cmd.Parameters["@DOCLOCATION"].Value = bol.bolMCU;
                cmd.Parameters["@DOCDESC1"].Value = bol.bolCustName;
                cmd.Parameters["@DOCNUMBER1"].Value = bol.bolDocNum ?? (object)DBNull.Value;
                cmd.Parameters["@DOCNUMBER2"].Value = bol.bolCustNum;
                cmd.Parameters["@DOCDATE_CALC"].Value = bol.bolDateCalc;
                cmd.Parameters["@LOADNUM"].Value = bol.bolLoadNum;
                cmd.Parameters["@CARRIER"].Value = bol.bolCarrier;
                cmd.Parameters["@BOL"].Value = bol.bolBOLNum;
                cmd.Parameters["@DOCDATE"].Value = bol.bolDate;
                cmd.Parameters["@DOCTITLE"].Value = bol.bolDocTitle;
                cmd.Parameters["@DOCSTAMP"].Value = bol.bolDocStamp;
                cmd.Connection = cnTCC;
                response.Init(cmd);
                Logger.Info(Logging.DbCmdString(cmd));
                var rows = cmd.ExecuteNonQuery();
                response.RowsAffected = rows;
            }
            catch (Exception e)
            {
                Logger.Error("ERROR: " + cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
                throw new ApplicationException(cnTCC.ConnectionString + "  " + e.Message + ": " + e.StackTrace);
            }
            finally
            {
                Logger.Info($@"END: {Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds)} SECONDS");
            }

            return rowCount;
        }

        private static SqlCommand InsertSQLBOL()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText = @"
                    INSERT INTO [TCC].[webrepadmin].[Manufacturing]
                    (
                        [DocID]
                        ,[SatID]
                        ,[DateStored]
                        ,[DateDelete]
                        ,[DocType]
                        ,[OfflineDate]
                        ,[DeleteFlag]
                        ,[FS_InboxOwner]
                        ,[FS_InboxSubject]
                        ,[FS_ReprintServer]
                        ,[FS_ReprintPrinter]
                        ,[DOCSUBJECT]
                        ,[DOCLOCATION]
                        ,[DOCDESC1]
                        ,[DOCNUMBER1]
                        ,[DOCNUMBER2]
                        ,[DOCDATE_CALC]
                        ,[LOADNUM]
                        ,[CARRIER]
                        ,[BOL]
                        ,[DOCDATE]
                        ,[DOCTITLE]
                        ,[DOCSTAMP]
                    )
                    VALUES
                    (
                        @DocID
                        ,@SatID
                        ,@DateStored
                        ,@DateDelete
                        ,@DocType
                        ,@OfflineDate
                        ,@DeleteFlag
                        ,@FS_InboxOwner
                        ,@FS_InboxSubject
                        ,@FS_ReprintServer
                        ,@FS_ReprintPrinter
                        ,@DOCSUBJECT
                        ,@DOCLOCATION
                        ,@DOCDESC1
                        ,@DOCNUMBER1
                        ,@DOCNUMBER2
                        ,@DOCDATE_CALC
                        ,@LOADNUM
                        ,@CARRIER
                        ,@BOL
                        ,@DOCDATE
                        ,@DOCTITLE
                        ,@DOCSTAMP
                    )                    
                "
            };
            cmd.Parameters.Add(new SqlParameter("@DocID", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@SatID", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@DateStored", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@DateDelete", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@DocType", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@OfflineDate", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@DeleteFlag", SqlDbType.SmallInt));
            cmd.Parameters.Add(new SqlParameter("@FS_InboxOwner", SqlDbType.Int));
            cmd.Parameters.Add(new SqlParameter("@FS_InboxSubject", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@FS_ReprintServer", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@FS_ReprintPrinter", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCSUBJECT", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCLOCATION", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCDESC1", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCNUMBER1", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCNUMBER2", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCDATE_CALC", SqlDbType.DateTime));
            cmd.Parameters.Add(new SqlParameter("@LOADNUM", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@CARRIER", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@BOL", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCDATE", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCTITLE", SqlDbType.NVarChar));
            cmd.Parameters.Add(new SqlParameter("@DOCSTAMP", SqlDbType.NVarChar));
            return cmd;
        }


    }
}