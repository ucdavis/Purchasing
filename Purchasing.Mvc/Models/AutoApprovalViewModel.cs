using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Models
{
    /// <summary>
    /// ViewModel for the AutoApproval class
    /// </summary>
    public class AutoApprovalViewModel
    {
        public AutoApproval AutoApproval { get; set; }
        public IList<Account> Accounts { get; set; }
        public IList<User> Users { get; set; } 
 
        public static AutoApprovalViewModel Create(IRepository repository, string userName)
        {
            Check.Require(repository != null, "Repository must be supplied");
            Check.Require(!string.IsNullOrWhiteSpace(userName), "user name");

            var viewModel = new AutoApprovalViewModel
                                {
                                    AutoApproval = new AutoApproval { IsActive = true, Expiration = DateTime.UtcNow.ToPacificTime().AddYears(1) }
                                };

            var workgroups = repository.OfType<WorkgroupPermission>().Queryable.Where(a => a.Role != null && a.Role.Id == Role.Codes.Approver && a.User != null && a.User.Id == userName && (!a.IsAdmin || (a.IsAdmin && a.IsFullFeatured))).Select(b => b.Workgroup).Distinct().ToList();
            //viewModel.Accounts = repository.OfType<WorkgroupAccount>().Queryable.Where(a => a.Approver != null && a.Approver.Id == userName && a.Account.IsActive).Select(b => b.Account).Distinct().ToList(); //workgroups.SelectMany(a => a.Accounts).Select(b => b.Account).Distinct().ToList();
            viewModel.Users = workgroups.SelectMany(a => a.Permissions).Where(b => b.Role != null && b.Role.Id == Role.Codes.Requester).Select(c => c.User).Where(c => c.IsActive).Distinct().ToList();

            return viewModel;
        }
    }
}