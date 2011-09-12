using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Core
{
    public interface IRepositoryFactory
    {
        IRepository<AutoApproval> AutoApprovalRepository { get; set; }
        IRepository<ConditionalApproval> ConditionalApprovalRepository { get; set; }
        IRepository<Order> OrderRepository { get; set; }
        IRepository<Workgroup> WorkgroupRepository { get; set; }
        IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        IRepositoryWithTypedId<User, string> UserRepository { get; set; }
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        #region IRepositoryFactory Members

        public IRepository<AutoApproval> AutoApprovalRepository { get; set; }
        public IRepository<ConditionalApproval> ConditionalApprovalRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepository<Workgroup> WorkgroupRepository { get; set; }
        public IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }

        #endregion
    }
}