using System;

namespace Purchasing.Core.Models.AggieEnterprise
{
    public class AeResultStatus
    {

        public Guid? RequestId { get; set; }

        public string ConsumerTrackingId { get; set; }
        
        public string ConsumerReferenceId { get; set; }

        public string? ConsumerNotes { get; set; }

        public string RequestDateTime { get; set; }


        public string LastStatusDateTime { get; set; }

        public string ProcessedDateTime { get; set; }
        public string OracleReq { get; set; }
        public string PoNumber { get; set; }


        public string Status { get; set; }
    }
}
