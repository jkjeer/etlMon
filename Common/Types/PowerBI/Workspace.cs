using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace etlMon.Common.Types.PowerBI
{
    public class Workspaces : List<Workspace>
    {
    }

    public class Workspace
    {
        public string WorkspaceName { get; set; }
        public string WorkspaceValue { get; set; }
    }
}