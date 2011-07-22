using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrganizationType : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual bool IsActive { get; set; }
    }

    public class OrganizationTypeMap : ClassMap<OrganizationType>
    {
        public OrganizationTypeMap()
        {
            ReadOnly();

            Table("vOrganizationTypes");

            Map(x => x.Name);
            Map(x => x.IsActive);
        }
    }
}