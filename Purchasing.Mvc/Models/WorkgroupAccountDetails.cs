using Purchasing.Core.Domain;
using Purchasing.Core.Models.AggieEnterprise;

namespace Purchasing.Mvc.Models
{
    public class WorkgroupAccountDetails
    {
        public WorkgroupAccount WorkgroupAccount { get; set; }
        public AccountValidationModel AccountValidationModel { get; set; }
    }
}
