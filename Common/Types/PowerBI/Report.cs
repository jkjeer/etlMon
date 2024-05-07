using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace etlMon.Common.Types.PowerBI
{
    public class Reports : List<Report>
    {
    }

    public class Report
    {
        public string ReportName { get; set; }
        public string ReportValue { get; set; }
    }
}