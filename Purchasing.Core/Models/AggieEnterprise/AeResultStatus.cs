using AggieEnterpriseApi;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchasing.Core.Models.AggieEnterprise
{
    public class AeResultStatus
    {

        public Guid? RequestId { get; set; }

        public string ConsumerTrackingId { get; set; }

        public string ConsumerReferenceId { get; set; }

        public string? ConsumerNotes { get; set; }

        public DateTimeOffset RequestDateTime { get; set; }


        public DateTimeOffset LastStatusDateTime { get; set; }

        public DateTimeOffset? ProcessedDateTime { get; set; }


        public string Status { get; set; }
    }
}
