using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Account : DomainObjectWithTypedId<string>
    {
        public virtual string Name { get; set; }
        public virtual string AccountManager { get; set; }
        public virtual string PrincipalInvestigator { get; set; }
        //TODO: Organization
        public virtual bool IsActive { get; set; }

        public virtual string NameAndId { get { return string.Format("{0} ({1})", Name, Id); } }
    }

    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            ReadOnly();

            Table("vAccounts");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.AccountManager);
            Map(x => x.PrincipalInvestigator);
            Map(x => x.IsActive);
        }
    }
}