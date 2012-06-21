using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Web.Services;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Web.Models
{
    public class ReportPermissionsViewModel
    {
        public IEnumerable<Workgroup> Workgroups { get; set; }
        public IEnumerable<WorkgroupPermission> Permissions { get; set; }
        public IEnumerable<User> Users { get; set; }
        
        public static ReportPermissionsViewModel Create(IRepositoryFactory repositoryFactory, IWorkgroupService workgroupService)
        {
            var viewModel = new ReportPermissionsViewModel()
                                {
                                    Workgroups = workgroupService.LoadAdminWorkgroups()
                                };

            viewModel.Permissions = viewModel.Workgroups.SelectMany(a => a.Permissions);
            viewModel.Users = viewModel.Permissions.Select(a => a.User).Distinct();

            return viewModel;
        }

    }
}