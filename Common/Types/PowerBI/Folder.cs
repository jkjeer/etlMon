using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace etlMon.Common.Types.PowerBI
{
    public class Folders : List<Folder>
    {
    }

    public class Folder
    {
        public string FolderName { get; set; }
        public string FolderValue { get; set; }
    }
}