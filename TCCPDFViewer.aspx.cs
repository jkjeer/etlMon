using System;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using etlMon.Common.Managers;
using etlMon.Common.Types.Archie;
using etlMon.Common.Utilities;
using NLog;

namespace etlMon
{
    public partial class TCCPDFViewer : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected static string _docID = "";
        protected static string _loadNum = "";
        protected static string _guid = "";
        protected static string _filePath = "";
        protected static Guid _realguid;

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {startTime}");
            _docID = Request.QueryString["docID"];
            _loadNum = Request.QueryString["loadNum"];
            _guid = Request.QueryString["guid"];
            _filePath = Request.QueryString["filePath"];
            var doc = GetBOL(_docID, _loadNum, _guid );
            if (doc.Image != null)
            {
                var bytes = doc.Image;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
                Logger.Info($"PDF successfully rendered");
            }
            else 
            {
                Response.ContentType = "Application/pdf";
                Response.WriteFile(_filePath);
                Response.End();
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected BOL GetBOL(string docID, string loadNum, string guid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetBOL BEGIN: {startTime}");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            var doc = new BOL();
            try
            {

                using (IDataReader reader = GetBOL(cnTCCSUPPORT, docID, loadNum, guid))
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
            cnTCCSUPPORT.Close();
            Logger.Info("GetBOL END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return doc;
        }

        public static IDataReader GetBOL(SqlConnection cnTCCSUPPORT, string docID, string loadNum, string guid)
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
                      WHERE m.[DOCSUBJECT] = 'Signed BOL'"
                };
                if (!string.IsNullOrEmpty(docID))
                {
                    cmd.CommandText = cmd.CommandText + $@"AND m.[fsdDocID] = '{docID}'";
                }
                else if (!string.IsNullOrEmpty(loadNum))
                {
                    cmd.CommandText = cmd.CommandText + $@"AND m.[LOADNUM] = '{loadNum}'";
                }
                else if (!string.IsNullOrEmpty(guid))
                {
                    cmd.CommandText = cmd.CommandText + $@"AND a.[AttachmentGuid] = '{guid}'";
                }

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

    }
}