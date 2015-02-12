using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Mvc.Models
{
    public class AutoApprovalListModel
    {
        public List<AutoApproval> AutoApprovals { get; set; }
        public bool ShowAll { get; set; }

        public static AutoApprovalListModel Create(IRepository<AutoApproval> autoApprovalRepository, string userName, bool showAll)
        {
            Check.Require(autoApprovalRepository != null);
            Check.Require(!string.IsNullOrWhiteSpace(userName));

            var viewModel = new AutoApprovalListModel {ShowAll = showAll};
            var temp = autoApprovalRepository.Queryable.Where(a => a.User != null && a.User.Id == userName);
            if (!showAll)
            {
                temp = temp.Where(a => a.IsActive && (a.Expiration == null || a.Expiration >= DateTime.UtcNow.ToPacificTime().Date));
            }

            viewModel.AutoApprovals = temp.ToList();

            return viewModel;
        }

    }
}