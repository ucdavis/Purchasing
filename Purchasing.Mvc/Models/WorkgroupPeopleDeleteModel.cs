using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Models
{
    public class WorkgroupPeopleDeleteModel
    {
        public WorkgroupPermission WorkgroupPermission { get; set; }
        public List<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public int AccountApproverCount { get; set; }
        public int AccountAccountManagerCount { get; set; }
        public int AccountPurchaserCount { get; set; }
        public static WorkgroupPeopleDeleteModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, WorkgroupPermission workgroupPermission)
        {
            Check.Require(workgroupPermissionRepository != null);
            Check.Require(workgroupPermission != null);
            var viewModel = new WorkgroupPeopleDeleteModel{WorkgroupPermission = workgroupPermission};
            viewModel.WorkgroupPermissions = workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroupPermission.Workgroup && a.User == workgroupPermission.User && !a.Role.IsAdmin && !a.IsAdmin).ToList();

            viewModel.AccountApproverCount = 0;
            viewModel.AccountAccountManagerCount = 0;
            viewModel.AccountPurchaserCount = 0;
            foreach (var wgPermission in viewModel.WorkgroupPermissions)
            {
                switch (wgPermission.Role.Id)
                {
                    case Role.Codes.Approver:
                        viewModel.AccountApproverCount = wgPermission.Workgroup.Accounts.Count(a => a.Approver == wgPermission.User);
                        break;
                    case Role.Codes.AccountManager:
                        viewModel.AccountAccountManagerCount = wgPermission.Workgroup.Accounts.Count(a => a.AccountManager == wgPermission.User);
                        break;
                    case Role.Codes.Purchaser:
                        viewModel.AccountPurchaserCount = wgPermission.Workgroup.Accounts.Count(a => a.Purchaser == wgPermission.User);
                        break;
                    default:
                        continue;
                }
            }
            return viewModel;
        }
    }
}