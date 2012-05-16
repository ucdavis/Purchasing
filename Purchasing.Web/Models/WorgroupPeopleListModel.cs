using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorgroupPeopleListModel
    {
        public Workgroup Workgroup { get; set; }
        //public IEnumerable<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public Role CurrentRole { get; set; }
        //public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<UserRoles> UserRoles { get; set; }

        /// <summary>
        /// Create ViewModel
        /// </summary>
        /// <param name="workgroupPermissionRepository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="workgroup"></param>
        /// <param name="rolefilter">Role Id</param>
        /// <returns></returns>
        public static WorgroupPeopleListModel Create(IRepository<WorkgroupPermission> workgroupPermissionRepository, IRepositoryWithTypedId<Role, string> roleRepository ,Workgroup workgroup, string rolefilter)
        {
            Check.Require(workgroupPermissionRepository != null);
            Check.Require(roleRepository != null);
            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleListModel()
                                {
                                    Workgroup = workgroup,
                                    UserRoles = new List<UserRoles>()
                                };

            var allWorkgroupPermissions =
                workgroupPermissionRepository.Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive && !a.Role.IsAdmin);


            var workgroupPermissions = !string.IsNullOrWhiteSpace(rolefilter) ? allWorkgroupPermissions.Where(a => a.Role.Id == rolefilter) : allWorkgroupPermissions;
            foreach(var workgroupPermission in allWorkgroupPermissions)
            {
                if(workgroupPermissions.Any(a => a.User.Id == workgroupPermission.User.Id))
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

            viewModel.Roles = roleRepository.Queryable.Where(a => !a.IsAdmin).ToList();
            
            return viewModel;
        }

    }

    public class UserRoles
    {
        public User User { get; set; }
        public IList<Role> Roles { get; set; }
        public int FirstWorkgroupPermissionId { get; set; }

        public UserRoles(WorkgroupPermission workgroupPermission)
        {
            User = workgroupPermission.User;
            FirstWorkgroupPermissionId = workgroupPermission.Id;
            Roles = new List<Role>();
            Roles.Add(workgroupPermission.Role);
        }

        public string RolesList
        {
            get
            {
                return string.Join(", ", Roles.OrderBy(x => x.Level).Select(a => a.Name).ToArray());
            }
        }
    }
}