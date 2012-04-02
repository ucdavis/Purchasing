using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class SubAccount : DomainObjectWithTypedId<Guid>
    {
        [Required]
        [StringLength(10)]
        public virtual string AccountNumber { get; set; }
        [Required]
        [StringLength(5)]
        public virtual string SubAccountNumber { get; set; }
        [StringLength(40)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }

    public class SubAccountMap : ClassMap<SubAccount>
    {
        public SubAccountMap()
        {
            ReadOnly();

            Table("vSubAccounts");

            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.AccountNumber);
            Map(x => x.SubAccountNumber);
            Map(x => x.Name);
            Map(x => x.IsActive);
        }
    }
}
