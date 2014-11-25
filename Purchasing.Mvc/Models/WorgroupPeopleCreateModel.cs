using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Purchasing.Core.Domain;
using Purchasing.Mvc.Utility;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorgroupPeopleCreateModel
    {
        public Workgroup Workgroup { get; set; }
        [Required]
        public Role Role { get; set; }
        public List<IdAndName> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<KeyValuePair<string,string>> ErrorDetails { get; set; }

        public static WorgroupPeopleCreateModel Create(IRepositoryWithTypedId<Role, string> roleRepository, Workgroup workgroup)
        {
            Check.Require(roleRepository != null);

            Check.Require(workgroup != null);

            var viewModel = new WorgroupPeopleCreateModel
                                {
                                    Workgroup = workgroup
                                };
            viewModel.ErrorDetails = new List<KeyValuePair<string, string>>();
            viewModel.Roles = roleRepository.Queryable.Where(a => !a.IsAdmin).ToList();
            return viewModel;
        }

    }
}