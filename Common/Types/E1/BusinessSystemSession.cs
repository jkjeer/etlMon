using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
//using System.ServiceModel;
using etlMon.Common;
using IBM.Data.DB2.iSeries;

namespace etlMon.Common.Types.E1
{
    public class BusinessSystemSession
    {
        public iDB2Connection DbConnection;
        public Dictionary<string, iDB2Command> DbCommands;
        public Dictionary<string, object> Managers;
        public PlantMap PlantMap;
        public bool IsOpen;

        public DateTime GetCurrentTimestamp()
        {
            var dt = new DateTime();
            try
            {
                var cmd = GetCurrentTimestampStmt();
                cmd.Connection = this.DbConnection;
                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    while (reader.Read())
                    {
                        try
                        {
                            dt = Utilities.CastDbNull.To<DateTime>(reader["TS"], new DateTime(1900, 1, 1));
                        }
                        catch (Exception e)
                        {
                            throw new ApplicationException(e.ToString());
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                throw new ApplicationException(err.ToString());
            }

            return dt;
        }

        public iDB2Command GetCurrentTimestampStmt()
        {
            var cmd = new iDB2Command();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
            SELECT
              CURRENT TIMESTAMP AS TS
            FROM
              SYSIBM.SYSDUMMY1";
            return cmd;
        }


        public void Close()
        {
            try
            {
                DbConnection.Close();
                IsOpen = false;
            }
            catch (ApplicationException e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
