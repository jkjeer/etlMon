using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace etlMon.Common.Types.PowerBI
{
    public class Files : List<File>
    {
    }

    public class File
    {
        public string FileName { get; set; }
        public string FileValue { get; set; }
    }
}