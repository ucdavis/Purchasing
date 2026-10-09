using System;

namespace UCDArch.Core.PersistanceSupport
{
    public class TransactionScope : IDisposable
    {
        private readonly IDbContext _dbContext;
        private bool _completed;
        
        public TransactionScope()
        {
            _dbContext = SmartServiceLocator<IDbContext>.GetService();

            _dbContext.BeginTransaction();
        }

        public void RollBackTransaction()
        {
            _completed = true;
            _dbContext.RollbackTransaction();
        }

        public void CommitTransaction()
        {
            _dbContext.CommitTransaction();
            _completed = true;
        }

        public bool HasOpenTransaction
        {
            get { return _dbContext.IsActive; }
        }

        public void Dispose()
        {
            if (!_completed && _dbContext.IsActive) // Roll back incomplete transactions.
            {
                _dbContext.RollbackTransaction();
            }
        }
    }
}
