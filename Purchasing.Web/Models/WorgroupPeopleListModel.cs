using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Web.Controllers;
using Purchasing.Web.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorgroupPeopleListModel
    {
        public Workgroup Workgroup { get; set; }
        //public IEnumerable<WorkgroupPermission> WorkgroupPermissions { get; set; }
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<UserRoles> UserRoles { get; set; } 

        /// <summary>
        /// Create ViewModel
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="roleRepository"></param>
        /// <param name="workgroup"></param>
        /// <param name="rolefilter">Role Id</param>
        /// <returns></returns>
        public static WorgroupPeopleListModel Create(IRepository repository, IRepositoryWithTypedId<Role, string> roleRepository ,Workgroup workgroup, string rolefilter)
        {
            Check.Require(repository != null);
            Check.Require(roleRepository != null);
            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleListModel()
                                {
                                    Workgroup = workgroup,
                                    UserRoles = new List<UserRoles>()
                                };
            var allWorkgroupPermissions =
                repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Workgroup == workgroup && a.User.IsActive && a.Role.Level >= 1 && a.Role.Level <= 4);


            var workgroupPermissions = !string.IsNullOrWhiteSpace(rolefilter) ? allWorkgroupPermissions.Where(a => a.Role.Id == rolefilter) : allWorkgroupPermissions;
            foreach(var workgroupPermission in allWorkgroupPermissions)
            {
                if(workgroupPermissions.Where(a => a.User.Id == workgroupPermission.User.Id).Any())
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

            viewModel.Roles = roleRepository.Queryable.Where(a => a.Level >= 1 && a.Level <= 4).ToList();
            return viewModel;
        }

    }
}