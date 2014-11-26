using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;

namespace Purchasing.Mvc.Models
{
    public class ContactViewModel
    {
        public User User { get; set; }
        public IEnumerable<string> Workgroups { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Contacts { get; set; }
        public IEnumerable<Order> Orders { get; set; }

        public static ContactViewModel Create(IRepositoryFactory repository, IQueryRepositoryFactory queryRepository, User user)
        {
            var viewModel = new ContactViewModel() {User = user};

            if (user != null)
            {
                // check for the workgroups the user is in
                viewModel.Workgroups = user.WorkgroupPermissions.Select(a => a.Workgroup.Name).Distinct().ToList();

                // contacts for the workgroups
                var wkids = user.WorkgroupPermissions.Select(a => a.Workgroup.Id).ToList();
                viewModel.Contacts = queryRepository.WorkgroupAdminRepository.Queryable.Where(a => wkids.Contains(a.WorkgroupId)).Select(a => new KeyValuePair<string, string>(string.Format("{0} {1}", a.FirstName, a.LastName), a.Email)).ToList();

                if (!viewModel.Workgroups.Any())
                {
                    viewModel.Orders = repository.ApprovalRepository.Queryable.Where(a => a.User == user && !a.Completed).Select(a => a.Order).ToList();
                }
            }

            return viewModel;
        }

    }
}