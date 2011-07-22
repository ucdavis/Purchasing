using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Role : DomainObjectWithTypedId<string>
    {
        protected Role() { }

        public Role(string id)
        {
            Id = id;
        }

        public virtual string Name { get; set; }
    }

    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
        }
    }
}