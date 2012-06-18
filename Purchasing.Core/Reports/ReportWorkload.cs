using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Reports
{
    public class ReportWorkload : DomainObject
    {
        public virtual int ReportingWorkgroupId { get; set; }
        public virtual string ReportWorkgroup { get; set; }
        public virtual string UserId { get; set; }
        public virtual string UserFullName { get; set; }
        public virtual string WorkgroupName { get; set; }
        public virtual string WorkgroupOrg { get; set; }
        public virtual int Approved { get; set; }
        public virtual int Edited { get; set; }
    }

    public class ReportWorkloadMap : ClassMap<ReportWorkload>
    {
        public ReportWorkloadMap()
        {
            Table("vReportWorkload");

            ReadOnly();

            Id(x => x.Id);

            Map(x => x.ReportingWorkgroupId);
            Map(x => x.ReportWorkgroup);
            Map(x => x.UserId);
            Map(x => x.UserFullName);
            Map(x => x.WorkgroupName);
            Map(x => x.WorkgroupOrg);
            Map(x => x.Approved);
            Map(x => x.Edited);
        }
    }
}
