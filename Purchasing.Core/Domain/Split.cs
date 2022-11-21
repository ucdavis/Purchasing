using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
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

        public virtual SubAccount DbSubAccount { get; set; }

        public virtual IList<Approval> Approvals { get; set; }

        [StringLength(64)]
        public virtual string Name { get; set; } //Have the name here?
        [DisplayName("CoA")]
        [StringLength(128)]
        public virtual string FinancialSegmentString { get; set; }

        public virtual string AccountDisplay
        {
            get {
                if (!string.IsNullOrWhiteSpace(FinancialSegmentString))
                {
                    //return $"{FinancialSegmentString} ({Name})";
                    return $"{Name} ({FinancialSegmentString})";
                }
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

        public virtual string FullAccountDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FinancialSegmentString))
                {
                    return AccountDisplay;
                }
                if (Account == null) return string.Empty;

                var result = new StringBuilder();
                result.Append(Account);

                if(!string.IsNullOrEmpty(SubAccount))
                {
                    result.Append(string.Format(" [{0}]", SubAccount));
                }

                return result.ToString();
            }
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
            Map(x => x.Name);
            Map(x => x.FinancialSegmentString);
            
            References(x => x.Order);
            References(x => x.LineItem);

            References(x => x.DbAccount).Column("Account").Nullable().ReadOnly();
            //References(x => x.DbSubAccount).Nullable().ReadOnly().Formula(
              //  "select * from vSubAccounts where accountnumber=; and subaccountnumber=;");
            
            HasMany(x => x.Approvals).ReadOnly();
        }
    }
}