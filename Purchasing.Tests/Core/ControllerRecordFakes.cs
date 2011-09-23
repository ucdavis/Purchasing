using System.Collections.Generic;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing.Fakes;

namespace Purchasing.Tests.Core
{
    public class FakeAutoApprovals : ControllerRecordFakes<AutoApproval>
    {
        protected override AutoApproval CreateValid(int i)
        {
            return CreateValidEntities.AutoApproval(i);
        }
        public FakeAutoApprovals(int count, IRepository<AutoApproval> repository, List<AutoApproval> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeAutoApprovals(int count, IRepository<AutoApproval> repository)
        {
            Records(count, repository);
        }
        public FakeAutoApprovals()
        {

        }
    }
}
