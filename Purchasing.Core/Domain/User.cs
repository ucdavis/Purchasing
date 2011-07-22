using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class User : DomainObjectWithTypedId<string>
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Email);
        }
    }
}