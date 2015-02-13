using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using Purchasing.Core.Helpers;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderComment : DomainObject 
    {
        public OrderComment()
        {
            DateCreated = DateTime.UtcNow.ToPacificTime();
        }

        [Required]
        public virtual string Text { get; set; }
        public virtual DateTime DateCreated { get; set; }

        [Required]
        public virtual User User { get; set; }
        [Required]
        public virtual Order Order { get; set; }
    }

    public class OrderCommentMap : ClassMap<OrderComment>
    {
        public OrderCommentMap()
        {
            Id(x => x.Id);

            Map(x => x.Text).Length(int.MaxValue);
            Map(x => x.DateCreated);

            References(x => x.User);
            References(x => x.Order);
        }
    }
}
