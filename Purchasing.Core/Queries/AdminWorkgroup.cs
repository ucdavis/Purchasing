using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class AdminWorkgroup : DomainObject
    {
        public virtual int WorkgroupId { get; set; }
        public virtual string Name { get; set; }
        public virtual string PrimaryOrganizationId { get; set; }
        public virtual string RollupParentId { get; set; }
    }

    public class AdminWorkgroupMap : ClassMap<AdminWorkgroup>
    {
        public AdminWorkgroupMap()
        {
            Id(x => x.Id);

            Table("vAdminWorkgroups");
            ReadOnly();

            Map(x => x.WorkgroupId);
            Map(x => x.Name);
            Map(x => x.PrimaryOrganizationId);
            Map(x => x.RollupParentId);
        }
    }
}
