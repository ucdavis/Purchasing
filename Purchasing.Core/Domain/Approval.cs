using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class Approval : DomainObject
    {
        public Approval()
        {
            Completed = false;
        }

        public virtual bool Completed { get; set; }
        
        public virtual User User { get; set; }
        public virtual User SecondaryUser { get; set; }

        [Required]
        public virtual OrderStatusCode StatusCode { get; set; }
        [Required]
        public virtual Order Order { get; set; }

        [Required]
        public virtual Split Split { get; set; }
    }

    public class ApprovalMap : ClassMap<Approval>
    {
        public ApprovalMap()
        {
            Id(x => x.Id);

            Map(x => x.Completed);

            References(x => x.User);
            References(x => x.SecondaryUser).Column("SecondaryUserId");
            References(x => x.StatusCode);
            References(x => x.Order);
            References(x => x.Split);
        }
    }
}
