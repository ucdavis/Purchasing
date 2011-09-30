using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    /// <summary>
    /// ViewModel for the Workgroup class
    /// </summary>
    public class WorkgroupViewModel
    {
        public Workgroup Workgroup { get; set; }
 
        public static WorkgroupViewModel Create(IRepository repository)
        {
            Check.Require(repository != null, "Repository must be supplied");
			
            var viewModel = new WorkgroupViewModel {Workgroup = new Workgroup()};
 
            return viewModel;
        }
    }
}