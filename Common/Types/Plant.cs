using System.Collections.Generic;
using System.Runtime.Serialization;
using etlMon.Common.Types.Enterprise;

namespace etlMon.Common.Types
{
    public class Plants : List<Plant>
    {
    }

    public class Plant
    {
        public string PlantNum { get; set; }
        public string PlantCode { get; set; }
        public int NumericValue { get; set; }
        public string Name { get; set; }

        public string ShortCompanyNum { get; set; }
        public string CompanyNum { get; set; }

        public decimal AddressNum { get; set; }
        public string LegacyAddressNum { get; set; }

        public Address Address { get; set; }

        public string TimeZone { get; set; }
        public int TimeOffset { get; set; }

        public string OrderCalendarName { get; set; }
    }
}