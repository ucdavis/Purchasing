using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    /// <summary>
    /// ModifyModel for the Workgroup class
    /// </summary>
    public class WorkgroupModifyModel
    {
        public Workgroup Workgroup { get; set; }
        public List<ListItem> Organizations { get; set; } 

        public static WorkgroupModifyModel Create(IRepository repository, User user, Workgroup workgroup = null)
        {
            Check.Require(repository != null, "Repository must be supplied");

            var modifyModel = new WorkgroupModifyModel { Workgroup = workgroup ?? new Workgroup() };

            modifyModel.Organizations = workgroup != null ? workgroup.Organizations.Select(x => new ListItem(x.Name, x.Id, true) {Selected = true}).ToList() : new List<ListItem>();

            var userOrgs = user.Organizations.Where(x => !modifyModel.Organizations.Select(y => y.Value).Contains(x.Id));
            modifyModel.Organizations.AddRange(userOrgs.Select(x => new ListItem(x.Name, x.Id, true)));

            return modifyModel;
        }
    }
}