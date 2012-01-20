using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupDetailModel
    {
        public Workgroup Workgroup { get; set; }
        public List<UserRoles> UserRoles { get; set; }

        public static WorkgroupDetailModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, Workgroup workgroup)
        {
            Check.Require(workgroupPermissionRepository != null);
            Check.Require(workgroup != null);

            var viewModel = new WorkgroupDetailModel
            {
                Workgroup = workgroup,
                UserRoles = new List<UserRoles>()
            };

            var workgroupPermissions = workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive && a.Role.Level >= 1 && a.Role.Level <= 4);
            foreach (var workgroupPermission in workgroupPermissions)
            {
                if (workgroupPermissions.Where(a => a.User.Id == workgroupPermission.User.Id).Any())
                {
                    if (viewModel.UserRoles.Where(a => a.User.Id == workgroupPermission.User.Id).Any())
                    {
                        viewModel.UserRoles.Where(a => a.User.Id == workgroupPermission.User.Id).Single().Roles.Add(workgroupPermission.Role);
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