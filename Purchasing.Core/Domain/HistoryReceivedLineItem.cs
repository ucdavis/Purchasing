using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentNHibernate.Mapping;
using Purchasing.Core.Helpers;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class HistoryReceivedLineItem : DomainObject
    {
        public HistoryReceivedLineItem()
        {
            UpdateDate = DateTime.Now;
            CommentsUpdated = false;
        }
        [Required]
        public virtual LineItem LineItem { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual decimal? OldReceivedQuantity { get; set; }
        public virtual decimal? NewReceivedQuantity { get; set; }
        public virtual bool CommentsUpdated { get; set; }
        [Required]
        public virtual User User { get; set; }
        
    }

    public class HistoryReceivedLineItemMap : ClassMap<HistoryReceivedLineItem>
    {
        public HistoryReceivedLineItemMap()
        {
            Id(x => x.Id);

            Map(x => x.UpdateDate);
            Map(x => x.OldReceivedQuantity);
            Map(x => x.NewReceivedQuantity);
            Map(x => x.CommentsUpdated);

            References(x => x.User).Column("UserId");
            References(x => x.LineItem).Column("LineItemId");

        }
    }
}
