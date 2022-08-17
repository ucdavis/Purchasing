using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Purchasing.Mvc.Models
{
    /// <summary>
    /// ModifyModel for the Workgroup class
    /// </summary>
    public class WorkgroupModifyModel
    {
        public Workgroup Workgroup { get; set; }
        public List<SelectListItem> Organizations { get; set; } 
 

        public static WorkgroupModifyModel Create(User user, IQueryRepositoryFactory queryRepositoryFactory, Workgroup workgroup = null)
        {
            var modifyModel = new WorkgroupModifyModel { Workgroup = workgroup ?? new Workgroup() };

            modifyModel.Organizations = workgroup != null ? workgroup.Organizations.Select(x => new SelectListItem(x.Name + " (" + x.Id + ")", x.Id) {Selected = true}).ToList() : new List<SelectListItem>();

            var userOrgs = user.Organizations.Where(x => !modifyModel.Organizations.Select(y => y.Value).Contains(x.Id));
            modifyModel.Organizations.AddRange(userOrgs.Select(x => new SelectListItem(x.Name + " (" + x.Id + ")", x.Id)));

            var ids = modifyModel.Organizations.Select(y => y.Value).ToList();

            var decendantOrgs = queryRepositoryFactory.OrganizationDescendantRepository.Queryable.Where(a => ids.Contains(a.RollupParentId)).ToList();
            modifyModel.Organizations.AddRange(decendantOrgs.Select(x => new SelectListItem(x.Name + " (" + x.OrgId + ")", x.OrgId)));

            modifyModel.Organizations = modifyModel.Organizations.Distinct().OrderBy(a => a.Text).ToList();

            return modifyModel;
        }
    }
}