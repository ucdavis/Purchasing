using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Split : DomainObject
    {
        public Split()
        {
            Approvals = new List<Approval>();
        }

        public virtual decimal Amount { get; set; }

        [Required]
        public virtual Order Order { get; set; }
        public virtual LineItem LineItem { get; set; }
        
        [StringLength(10)]
        public virtual string Account { get; set; }
        [StringLength(5)]
        public virtual string SubAccount { get; set; }
        [StringLength(10)]
        public virtual string Project { get; set; }

        public virtual Account DbAccount { get; set; }

        public virtual IList<Approval> Approvals { get; set; }
        
        public virtual string AccountDisplay
        {
            get {
                return DbAccount == null ? Account : DbAccount.NameAndId;
            }
        }

        public virtual void AssociateApproval(Approval approval)
        {
            approval.Split = this;
            Order.AddApproval(approval);
        }

        public virtual void RemoveApproval(Approval approval)
        {
            Approvals.Remove(approval);
            Order.Approvals.Remove(approval);
        }
    }

    public class SplitMap : ClassMap<Split>
    {
        public SplitMap()
        {
            Id(x => x.Id);

            Map(x => x.Amount);
            Map(x => x.Account);
            Map(x => x.SubAccount);
            Map(x => x.Project);
            
            References(x => x.Order);
            References(x => x.LineItem);

            References(x => x.DbAccount).Column("Account").Nullable().ReadOnly();
            
            HasMany(x => x.Approvals).ReadOnly();
        }
    }
}