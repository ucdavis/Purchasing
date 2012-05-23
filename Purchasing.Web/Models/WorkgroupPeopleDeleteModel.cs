using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupPeopleDeleteModel
    {
        public WorkgroupPermission WorkgroupPermission { get; set; }
        public List<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public static WorkgroupPeopleDeleteModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, WorkgroupPermission workgroupPermission)
        {
            Check.Require(workgroupPermissionRepository != null);
            Check.Require(workgroupPermission != null);
            var viewModel = new WorkgroupPeopleDeleteModel{WorkgroupPermission = workgroupPermission};
            viewModel.WorkgroupPermissions = workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroupPermission.Workgroup && a.User == workgroupPermission.User && !a.Role.IsAdmin).ToList();

            return viewModel;
        }
    }
}