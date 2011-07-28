using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Organization : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string TypeCode { get; set; }
        public virtual string TypeName { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual Organization Parent { get; set; }
    }

    public class OrganizationMap : ClassMap<Organization>
    {
        public OrganizationMap()
        {
            ReadOnly();

            Table("vOrganizations");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.TypeCode);
            Map(x => x.TypeName);
            Map(x => x.IsActive);
            
            References(x => x.Parent);
        }
    }
}