using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class RelatedWorkgroups : DomainObject
    {
        public virtual int  WorkgroupId { get; set; }
        public virtual string WorkgroupName { get; set; }
        public virtual string PrimaryOrganizationId { get; set; }
        public virtual int AdminWorkgroupId { get; set; }
        public virtual string AdminOrgId { get; set; }
    }

    public class RelatedWorkgroupsMap : ClassMap<RelatedWorkgroups>
    {
        public RelatedWorkgroupsMap()
        {
            Table("vRelatedWorkgroups");

            ReadOnly();

            Id(x => x.Id);

            Map(x => x.WorkgroupId);
            Map(x => x.WorkgroupName).Column("Name");
            Map(x => x.PrimaryOrganizationId);
            Map(x => x.AdminWorkgroupId);
            Map(x => x.AdminOrgId);
        }
    }
}
