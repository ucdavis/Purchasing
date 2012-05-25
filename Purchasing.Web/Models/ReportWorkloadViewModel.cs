using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Core.Reports;

namespace Purchasing.Web.Models
{
    public class ReportWorkloadViewModel
    {
        public IEnumerable<Workgroup> Workgroups;

        public static ReportWorkloadViewModel Create(IRepositoryFactory repositoryFactory, IQueryRepositoryFactory queryRepositoryFactory, string userName)
        {
            // load the person's orgs
            var person = repositoryFactory.UserRepository.Queryable.Where(x => x.Id == userName).Fetch(x => x.Organizations).Single();
            var porgs = person.Organizations.Select(x => x.Id).ToList();

            // get the administrative rollup on orgs
            var wgIds = queryRepositoryFactory.AdminWorkgroupRepository.Queryable.Where(a => porgs.Contains(a.RollupParentId)).Select(a => a.WorkgroupId).ToList();

            // get the workgroups
            var workgroups = repositoryFactory.WorkgroupRepository.Queryable.Where(a => wgIds.Contains(a.Id));
            
            var viewModel = new ReportWorkloadViewModel()
                                {
                                    Workgroups = workgroups.ToList()
                                };

            return viewModel;
        }

        public IEnumerable<ReportWorkload> ReportWorkloads { get; set; }
        public IEnumerable<ReportWorkloadHeaders> OrgHeaders { get; set; }

        public void GenerateDisplayTable(IReportRepositoryFactory reportRepositoryFactory, int workgroupId)
        {
            ReportWorkloads = reportRepositoryFactory.ReportWorkloadRepository.Queryable.Where(a => a.ReportWorkgroupId == workgroupId).ToList();

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