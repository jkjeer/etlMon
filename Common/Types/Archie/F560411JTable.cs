using System;
using System.Data;
using System.ServiceModel;
using etlMon.Common.Utilities;
using NLog;

namespace etlMon.Common.Types.Archie
{
    [System.ComponentModel.DesignerCategory(@"Code")]

    public class F560411JTable : DataTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public DateTime MaxDate
        //{
        //    get { return (from a in this.AsEnumerable() select a.Field<DateTime>("UpdateDate")).Max(); }
        //    set { }
        //}

        public F560411JTable()
        {
            ExtendedProperties.Add("Target", "[dbo].[F560411J]");
            Columns.Add("ABAN8", typeof(decimal));
            Columns.Add("ABALPH", typeof(string));
            Columns.Add("RPDOC", typeof(decimal));
            Columns.Add("RPAN8", typeof(decimal));
            Columns.Add("RPDIVJ", typeof(decimal));
            Columns.Add("RPVINV", typeof(string));
            Columns.Add("RPMCU", typeof(string));
            Columns.Add("RPICU", typeof(decimal));
            Columns.Add("RPDGJ", typeof(decimal));
            Columns.Add("RPAG", typeof(decimal));
            Columns.Add("RPOBJ", typeof(string));
            Columns.Add("RPSBLT", typeof(string));
            Columns.Add("RPPO", typeof(string));
            Columns.Add("RPPDCT", typeof(string));
            Columns.Add("RNDOC", typeof(decimal));
            Columns.Add("RNPYID", typeof(decimal));
            Columns.Add("RMPYID", typeof(decimal));
            Columns.Add("RMDOCM", typeof(decimal));
            Columns.Add("RMDMTJ", typeof(decimal));
            Columns.Add("ABAC08", typeof(string));
            Columns.Add("VOLUME", typeof(string));
            Columns.Add("RPUPMJ", typeof(decimal));
        }

        public void Add(IDataReader reader)
        {
            try
            {
                using (reader)
                {
                    while (reader.Read())
                    {
                        var dr = NewRow();
                        dr["ABAN8"] = CastDbNull.To<decimal>(reader["ABAN8"], 0);
                        dr["ABALPH"] = CastDbNull.To(reader["ABALPH"], String.Empty).Trim();
                        dr["RPDOC"] = CastDbNull.To<decimal>(reader["RPDOC"], 0);
                        dr["RPAN8"] = CastDbNull.To<decimal>(reader["RPAN8"], 0);
                        dr["RPDIVJ"] = CastDbNull.To<decimal>(reader["RPDIVJ"], 0);
                        dr["RPVINV"] = CastDbNull.To(reader["RPVINV"], String.Empty).Trim();
                        dr["RPMCU"] = CastDbNull.To(reader["RPMCU"], String.Empty).Trim();
                        dr["RPICU"] = CastDbNull.To<decimal>(reader["RPICU"], 0);
                        dr["RPDGJ"] = CastDbNull.To<decimal>(reader["RPDGJ"], 0);
                        dr["RPAG"] = CastDbNull.To<decimal>(reader["RPAG"], 0);
                        dr["RPOBJ"] = CastDbNull.To(reader["RPOBJ"], String.Empty).Trim();
                        dr["RPSBLT"] = CastDbNull.To(reader["RPSBLT"], String.Empty).Trim();
                        dr["RPPO"] = CastDbNull.To(reader["RPPO"], String.Empty).Trim();
                        dr["RPPDCT"] = CastDbNull.To(reader["RPPDCT"], String.Empty).Trim();
                        dr["RNDOC"] = CastDbNull.To<decimal>(reader["RNDOC"], 0);
                        dr["RNPYID"] = CastDbNull.To<decimal>(reader["RNPYID"], 0);
                        dr["RMPYID"] = CastDbNull.To<decimal>(reader["RMPYID"], 0);
                        dr["RMDOCM"] = CastDbNull.To<decimal>(reader["RMDOCM"], 0);
                        dr["RMDMTJ"] = CastDbNull.To<decimal>(reader["RMDMTJ"], 0);
                        dr["ABAC08"] = CastDbNull.To(reader["ABAC08"], String.Empty).Trim();
                        dr["RPUPMJ"] = CastDbNull.To<decimal>(reader["RPUPMJ"], 0);
                        Rows.Add(dr);
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new FaultException(e.Message);
            }
        }
    }
}


