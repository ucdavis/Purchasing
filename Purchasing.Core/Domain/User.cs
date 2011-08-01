using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class User : DomainObjectWithTypedId<string>
    {
        protected User()
        {
            Organizations = new List<Organization>();
            Roles = new List<Role>();
        }
        public User(string id) : this()
        {
            Id = id;
        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IList<Organization> Organizations { get; set; }
        public virtual IList<Role> Roles { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
            Map(x => x.IsActive);

            HasManyToMany(x => x.Organizations).Table("UsersXOrganizations").ParentKeyColumn("UserID").ChildKeyColumn("OrganizationID");
            HasManyToMany(x => x.Roles).Table("Permissions").ChildKeyColumn("RoleID").ParentKeyColumn("UserID");
        }
    }
}