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

    public class FakeWorkgroupPermissions : ControllerRecordFakes<WorkgroupPermission>
    {
        protected override WorkgroupPermission CreateValid(int i)
        {
            return CreateValidEntities.WorkgroupPermission(i);
        }
        public FakeWorkgroupPermissions(int count, IRepository<WorkgroupPermission> repository, List<WorkgroupPermission> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroupPermissions(int count, IRepository<WorkgroupPermission> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroupPermissions()
        {

        }
    }

    public class FakeWorkgroupAccounts : ControllerRecordFakes<WorkgroupAccount>
    {
        protected override WorkgroupAccount CreateValid(int i)
        {
            return CreateValidEntities.WorkgroupAccount(i);
        }
        public FakeWorkgroupAccounts(int count, IRepository<WorkgroupAccount> repository, List<WorkgroupAccount> specificRecords)
        {
            Records(count, repository, specificRecords);
        }

        public FakeWorkgroupAccounts(int count, IRepository<WorkgroupAccount> repository)
        {
            Records(count, repository);
        }
        public FakeWorkgroupAccounts()
        {

        }
    }

    public class FakeUsers : AbstractControllerRecordFakesStrings<User>
    {
        protected override User CreateValid(int i)
        {
            return CreateValidEntities.User(i);
        }
        public FakeUsers(int count, IRepositoryWithTypedId<User, string> repository, List<User> specificRecords, bool bypassSetIdTo)
        {
            Records(count, repository, specificRecords, bypassSetIdTo);
        }

        public FakeUsers(int count, IRepositoryWithTypedId<User, string> repository)
        {
            Records(count, repository, false);
        }
        public FakeUsers()
        {

        }
    }
}
