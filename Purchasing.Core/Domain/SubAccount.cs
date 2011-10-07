using System;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class SubAccount : DomainObjectWithTypedId<Guid>
    {
        public string AccountNumber { get; set; }
        public string SubAccountNumber { get; set; }
        public string Name { get; set; }
    }

    public class SubAccountMap : ClassMap<SubAccount>
    {
        public SubAccountMap()
        {
            ReadOnly();

            Table("SubAccounts");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.AccountNumber);
            Map(x => x.SubAccountNumber);
            Map(x => x.Name);
        }
    }
}
