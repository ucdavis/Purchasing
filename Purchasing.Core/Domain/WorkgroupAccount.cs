using System.ComponentModel;
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
        [Display(Name = "Account Manager")]
        public virtual User AccountManager { get; set; }
        public virtual User Purchaser { get; set; }

        [StringLength(64)] // 64 to hold account id and name (3-CRU9033: Account Name)
        public virtual string Name { get;set;}
        [DisplayName("CCOA")]
        [StringLength(128)]
        public virtual string FinancialSegmentString { get; set; }

        //NotMapped
        public virtual string GetName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    return Name;
                }
                return Account?.Name ?? "Not Set";
            }
        }

    public class WorkgroupAccountMap : ClassMap<WorkgroupAccount>
    {
        public WorkgroupAccountMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
            Map(x => x.FinancialSegmentString);

            References(x => x.Workgroup);
            References(x => x.Account);

            References(x => x.Approver).Column("ApproverUserId");
            References(x => x.AccountManager).Column("AccountManagerUserId");
            References(x => x.Purchaser).Column("PurchaserUserId");
        }
    }
}