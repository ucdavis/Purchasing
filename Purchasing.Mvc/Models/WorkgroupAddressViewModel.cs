using System.Collections.Generic;
using System.Linq;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace Purchasing.Web.Models
{
    public class WorkgroupAddressViewModel
    {
        public Workgroup Workgroup { get; set; }
        public WorkgroupAddress WorkgroupAddress { get; set; }
        public List<State> States { get; set; }
        public State State { get; set; }

        public static WorkgroupAddressViewModel Create(Workgroup workgroup, IRepositoryWithTypedId<State, string> stateRepository, bool loadStates = false)
        {
            Check.Require(workgroup != null);
            var viewModel = new WorkgroupAddressViewModel { Workgroup = workgroup };
            if(loadStates)
            {
                viewModel.States = stateRepository.GetAll().ToList();
            }
            return viewModel;
        }
    }
}