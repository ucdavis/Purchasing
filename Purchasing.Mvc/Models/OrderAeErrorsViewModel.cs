using Purchasing.Core.Domain;
using Purchasing.Core.Models.AggieEnterprise;
using System.Collections.Generic;

namespace Purchasing.Mvc.Models
{
    public class OrderAeErrorsViewModel
    {
        public List<AeJobErrorDetailCleaned> Errors { get; set; } = new List<AeJobErrorDetailCleaned>();
        public Order Order { get; set; }

        public bool AllowSetStatusBack { get; set; } = false;
    }
}
