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
    public partial class PDFViewer : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected static string _guid = ""; 
        protected static Guid _realguid;

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {startTime}");
            _guid = Request.QueryString["guid"];
            if (Guid.TryParse(Request.QueryString["guid"], out _realguid))
            {
                Logger.Info($"Guid {_guid} successfully parsed");
                var doc = GetBOLAttachment(_guid);
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
            }
            else
            {
                Logger.Error($"Error: Guid {_guid} not parsed ");
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected TCCUnion GetBOLAttachment(string guid)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetBOLAttachment BEGIN: {startTime}");
            var cnTCCSUPPORT = ConnectionManager.GetSqlConnectionFromKey("TCCSUPPORT");
            cnTCCSUPPORT.Open();
            var doc = new TCCUnion();
            try
            {

                using (IDataReader reader = GetBOLAttachment(cnTCCSUPPORT, guid))
                {
                    while (reader.Read())
                    {
                        doc.Image = CastDbNull.To<byte[]>(reader["Image"], new byte[0]);
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
                        doc.BOL = CastDbNull.To(reader["BOL"], string.Empty);
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
            Logger.Info("GetBOLAttachment END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return doc;
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
                        SELECT [AttachmentGuid]
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
                          WHERE [AttachmentGuid] = '{guid}'"
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

    }
}