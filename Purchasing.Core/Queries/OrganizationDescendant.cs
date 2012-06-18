using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OrganizationDescendant : DomainObject
    {
        public virtual string OrgId { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImmediateParentId { get; set; }
        public virtual string RollupParentId { get; set; }
    }

    public class OrganizationDescendantMap : ClassMap<OrganizationDescendant>
    {
        public OrganizationDescendantMap()
        {
            Id(x => x.Id);

            Table("vOrganizationDescendants");
            ReadOnly();

            Map(x => x.OrgId);
            Map(x => x.Name);
            Map(x => x.ImmediateParentId);
            Map(x => x.RollupParentId);
        }
    }
}
