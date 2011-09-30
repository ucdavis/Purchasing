using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupDetailsViewModel
    {
        public Workgroup Workgroup { get; set; }
        public virtual int OrganizationCount { get; set; }
        public virtual int AccountCount { get; set; }
        public virtual int VendorCount { get; set; }
        public virtual int AddressCount { get; set; }
        public virtual int UserCount { get; set; }
        public virtual int ApproverCount { get; set; }
        public virtual int AccountManagerCount { get; set; }
        public virtual int PurchaserCount { get; set; }

        public static WorkgroupDetailsViewModel Create(IRepository repository, Workgroup workgroup)
        {
            Check.Require(repository != null, "Repository is required.");

            var workgroupPermsByGroup = (from wp in repository.OfType<WorkgroupPermission>().Queryable
                                         where wp.Workgroup.Id == workgroup.Id
                                         group wp.Role by wp.Role.Id
                                         into role
                                         select new { count = role.Count(), name = role.Key }).ToList();

            var viewModel = new WorkgroupDetailsViewModel()
                                {
                                    Workgroup = workgroup,
                                    OrganizationCount = workgroup.Organizations.Count(),
                                    AccountCount = workgroup.Accounts.Count(),
                                    VendorCount = workgroup.Vendors.Count(),
                                    AddressCount = workgroup.Addresses.Count(),
                                    UserCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Requester).Select(x => x.count).
                                        SingleOrDefault(),
                                    ApproverCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Approver).Select(x => x.count)
                                        .SingleOrDefault(),
                                    AccountManagerCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.AccountManager).Select(
                                            x => x.count).SingleOrDefault(),
                                    PurchaserCount =
                                        workgroupPermsByGroup.Where(x => x.name == Role.Codes.Purchaser).Select(x => x.count)
                                        .SingleOrDefault()
                                };

            return viewModel;
        }
    }
}