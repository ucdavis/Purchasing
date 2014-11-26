using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Models
{
    public class WorkgroupAddressListModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupAddress> WorkgroupAddresses { get; set; }

        public static WorkgroupAddressListModel Create(Workgroup workgroup)
        {
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAddressListModel { Workgroup = workgroup };
            viewModel.WorkgroupAddresses = workgroup.Addresses.Where(a => a.IsActive);
            return viewModel;
        }
    }
}