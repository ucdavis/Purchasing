using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace Purchasing.Core.Domain
{
    public class WorkgroupAccount : DomainObject
    {
        [Required]
        public virtual Workgroup Workgroup { get; set; }
        [Required]
        public virtual Account Account { get; set; }

        public virtual User Approver { get; set; }
        public virtual User AccountManager { get; set; }
        public virtual User Purchaser { get; set; }
    }

    public class WorkgroupAccountMap : ClassMap<WorkgroupAccount>
    {
        public WorkgroupAccountMap()
        {
            Id(x => x.Id);

            References(x => x.Workgroup);
            References(x => x.Account);

            References(x => x.Approver).Column("ApproverUserId");
            References(x => x.AccountManager).Column("AccountManagerUserId");
            References(x => x.Purchaser).Column("PurchaserUserId");
        }
    }
}