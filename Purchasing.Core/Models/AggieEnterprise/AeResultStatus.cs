using AggieEnterpriseApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchasing.Core.Models.AggieEnterprise
{
    public class AeResultStatus
    {
        public IScmPurchaseRequisitionRequestStatus_ScmPurchaseRequisitionRequestStatus_RequestStatus AeStatus { get; set; }
        public string Status { get; set; }
    }
}
