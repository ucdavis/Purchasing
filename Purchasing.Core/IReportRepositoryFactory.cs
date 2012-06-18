using Purchasing.Core.Reports;
using UCDArch.Core.PersistanceSupport;

namespace Purchasing.Core
{
    public interface IReportRepositoryFactory
    {
        IRepository<ReportWorkload> ReportWorkloadRepository { get; set; }
    }

    public class ReportRepositoryFactory : IReportRepositoryFactory
    {
        public IRepository<ReportWorkload> ReportWorkloadRepository { get; set; }
    }
}
