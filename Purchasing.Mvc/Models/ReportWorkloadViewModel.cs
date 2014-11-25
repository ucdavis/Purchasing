using System.Collections.Generic;
using System.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Reports;
using Purchasing.Mvc.Services;

namespace Purchasing.Mvc.Models
{
    public class ReportWorkloadViewModel
    {
        public IEnumerable<Workgroup> Workgroups;
        public Workgroup Workgroup { get; set; }

        public static ReportWorkloadViewModel Create(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, IWorkgroupService workgroupService, string userName, Workgroup workgroup)
        {
            var workgroups = workgroupService.LoadAdminWorkgroups();
            
            var viewModel = new ReportWorkloadViewModel()
                                {
                                    Workgroups = workgroups,
                                    Workgroup = workgroup
                                };

            return viewModel;
        }

        public IEnumerable<ReportWorkload> ReportWorkloads { get; set; }
        public IEnumerable<ReportWorkloadHeaders> OrgHeaders { get; set; }

        public void GenerateDisplayTable(IReportRepositoryFactory reportRepositoryFactory, int workgroupId)
        {
            ReportWorkloads = reportRepositoryFactory.ReportWorkloadRepository.Queryable.Where(a => a.ReportingWorkgroupId == workgroupId).ToList();

            var headers = ReportWorkloads.Select(a => a.WorkgroupOrg).OrderBy(a => a).Distinct().Select(org => new ReportWorkloadHeaders()
                                                                                                                   {
                                                                                                                       OrgId = org, Workgroups = ReportWorkloads.Where(a => a.WorkgroupOrg == org).Select(a => a.WorkgroupName).Distinct().OrderBy(a => a).ToList()
                                                                                                                   }).ToList();

            OrgHeaders = headers;
        }

    }

    public class ReportWorkloadHeaders
    {
        public string OrgId { get; set; }
        public List<string> Workgroups { get; set; }
    }
}