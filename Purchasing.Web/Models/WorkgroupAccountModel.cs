using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupAccountModel
    {
        public WorkgroupAccount WorkgroupAccount { get; set; }

        public Workgroup Workgroup { get; set; }
        public IEnumerable<WorkgroupPermission> WorkGroupPermissions { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<User> Approvers { get; set; }
        public IEnumerable<User> AccountManagers { get; set; }
        public IEnumerable<User> Purchasers { get; set; }

        public static WorkgroupAccountModel Create(IRepository repository, Workgroup workgroup, WorkgroupAccount workgroupAccount = null)
        {
            Check.Require(repository != null);
            Check.Require(workgroup != null);

            var viewModel = new WorkgroupAccountModel { Workgroup = workgroup, WorkgroupAccount = workgroupAccount ?? new WorkgroupAccount()};

            viewModel.WorkGroupPermissions = repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup && !a.IsAdmin).ToList();  //If we want to allow parent full featured users in list, we need to add a distinct and && (!a.IsAdmin || (a.IsAdmin && a.IsFullFeatured)
            viewModel.Accounts = workgroup.Organizations.SelectMany(x => x.Accounts).Distinct().ToList();

            viewModel.Approvers = viewModel.WorkGroupPermissions.Where(x => x.Role.Id == Role.Codes.Approver).Select(x => x.User).Distinct().ToList();
            viewModel.AccountManagers = viewModel.WorkGroupPermissions.Where(x => x.Role.Id == Role.Codes.AccountManager).Select(x => x.User).Distinct().ToList();
            viewModel.Purchasers = viewModel.WorkGroupPermissions.Where(x => x.Role.Id == Role.Codes.Purchaser).Select(x => x.User).Distinct().ToList();

            return viewModel;
        }
    }
}