using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Account : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string Manager { get; set; }
        public virtual string PrimaryInvestigator { get; set; }
        //TODO: Department
        public virtual bool IsActive { get; set; }
    }

    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            ReadOnly();

            Table("vAccounts");

            Map(x => x.Name);
            Map(x => x.Manager);
            Map(x => x.PrimaryInvestigator);
            Map(x => x.IsActive);
        }
    }
}