using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupAccountsModel
    {
        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupPermission> WorkGroupPermissions { get; set; }

        public static WorkgroupAccountsModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null);
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAccountsModel { Workgroup = workgroup };
            viewModel.WorkGroupPermissions = repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup).ToList();

            return viewModel;
        }
    }
}