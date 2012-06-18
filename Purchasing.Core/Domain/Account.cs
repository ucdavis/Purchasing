using System.Collections.Generic;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Account : DomainObjectWithTypedId<string>
    {
        public Account()
        {
            SubAccounts = new List<SubAccount>();
        }

        public virtual string Name { get; set; }
        public virtual string AccountManager { get; set; }
        public virtual string AccountManagerId { get; set; }

        public virtual string PrincipalInvestigator { get; set; }
        public virtual string PrincipalInvestigatorId { get; set; }
        public virtual string OrganizationId { get; set; }
        public virtual bool IsActive { get; set; }

        public virtual IList<SubAccount> SubAccounts { get; set; }

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
            Map(x => x.AccountManagerId);
            Map(x => x.PrincipalInvestigator);
            Map(x => x.PrincipalInvestigatorId);
            Map(x => x.OrganizationId);
            Map(x => x.IsActive);

            HasMany(x => x.SubAccounts).Table("vSubAccounts").KeyColumn("AccountNumber").ExtraLazyLoad();
        }
    }
}