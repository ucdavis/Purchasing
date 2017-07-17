using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Models
{
    public class ReportPermissionsViewModel
    {
        public IEnumerable<Workgroup> Workgroups { get; set; }
        public IEnumerable<WorkgroupPermission> Permissions { get; set; }
        public IEnumerable<User> Users { get; set; }

        public bool HideInherited { get; set; }


        public static ReportPermissionsViewModel Create(IRepositoryFactory repositoryFactory, IWorkgroupService workgroupService, bool hideInherited = false)
        {
            var viewModel = new ReportPermissionsViewModel()
                                {
                                    Workgroups = workgroupService.LoadAdminWorkgroups()
                                };

            viewModel.Permissions = viewModel.Workgroups.SelectMany(a => a.Permissions);

            if (hideInherited)
            {
                viewModel.Users = viewModel.Permissions.Where(w => !w.IsAdmin).Select(a => a.User).Distinct();
                viewModel.Permissions = viewModel.Permissions.Where(a => !a.IsAdmin);
            }
            else
            {
                viewModel.Users = viewModel.Permissions.Select(a => a.User).Distinct();
            }

            viewModel.HideInherited = hideInherited;

            return viewModel;
        }

    }
}