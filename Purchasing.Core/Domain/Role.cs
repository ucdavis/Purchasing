using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Role : DomainObjectWithTypedId<string>
    {
        protected Role()
        {
            Users = new List<User>();
        }

        public Role(string id) : this()
        {
            Id = id;
        }

        public virtual string Name { get; set; }

        public virtual IList<User> Users { get; set; }
    }

    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);

            HasManyToMany(x => x.Users).Table("Permissions").ParentKeyColumn("RoleID").ChildKeyColumn("UserID");
        }
    }
}