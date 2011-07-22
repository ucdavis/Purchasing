using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Department : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual OrganizationType OrganizationType { get; set; }
        public virtual Department Parent { get; set; }
    }

    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            ReadOnly();

            Table("vDepartments");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);

            References(x => x.OrganizationType);
            References(x => x.Parent);
        }
    }
}