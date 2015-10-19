using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Models
{
    public class WorkgroupDetailModel
    {
        public Workgroup Workgroup { get; set; }
        public List<UserRoles> UserRoles { get; set; }

        public static WorkgroupDetailModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, Workgroup workgroup)
        {
            Check.Require(workgroupPermissionRepository != null, "workgroupPermissionRepository null");
            Check.Require(workgroup != null, "workgroup null");

            var viewModel = new WorkgroupDetailModel
            {
                Workgroup = workgroup,
                UserRoles = new List<UserRoles>()
            };

            var workgroupPermissions = workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive && !a.Role.IsAdmin);
            foreach (var workgroupPermission in workgroupPermissions)
            {
                if (workgroupPermissions.Any(a => a.User.Id == workgroupPermission.User.Id))
                {
                    if (viewModel.UserRoles.Any(a => a.User.Id == workgroupPermission.User.Id))
                    {
                        viewModel.UserRoles.Single(a => a.User.Id == workgroupPermission.User.Id).Roles.Add(workgroupPermission.Role);
                    }
                    else
                    {
                        viewModel.UserRoles.Add(new UserRoles(workgroupPermission));
                    }
                }
            }
            return viewModel;
        }
    }
}