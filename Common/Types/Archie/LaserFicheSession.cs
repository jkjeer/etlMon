using System;
using System.Runtime.Serialization;
using System.Web;

namespace etlMon.Common.Types.Archie
{
    [DataContract(Namespace = "")]
    public class LaserFicheSession
    {

        public LaserFicheSession(string taskName, string userName)
        {
            Hostname = HttpContext.Current.Server.MachineName;
            SessionGuid = Guid.NewGuid();
            PackageName = "LaserFiche";
            Version = "3.1";
            TaskName = taskName;
            UserName = userName;
            InsertRows = 0;
            UpdateRows = 0;
            DeleteRows = 0;
            SelectRows = 0;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Guid SessionGuid { get; set; }

        public string Hostname { get; set; }

        public string PackageName { get; set; }

        public string Version { get; set; }

        public string TaskName { get; set; }

        public string UserName { get; set; }

        public string Comments { get; set; }

        public int InsertRows { get; set; }

        public int UpdateRows { get; set; }

        public int DeleteRows { get; set; }

        public int SelectRows { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}