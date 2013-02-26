using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class LineItem : DomainObject
    {
        public LineItem()
        {
            Splits = new List<Split>();
        }

        public virtual decimal Quantity { get; set; }
        public virtual decimal? QuantityReceived { get; set; }
        public virtual decimal? QuantityPaid { get; set; }
        public virtual string ReceivedNotes { get; set; }
        public virtual string PaidNotes { get; set; }
        public virtual bool Received { get; set; } // calculated in db
        public virtual bool Paid { get; set; } // calculated in db

        [StringLength(50)]
        public virtual string CatalogNumber { get; set; } //Note: truncated to 15 for dafis
        [Required] //As per meeting 2011/12/06 JCS
        public virtual string Description { get; set; }
        [StringLength(25)]
        public virtual string Unit { get; set; }
        public virtual decimal UnitPrice { get; set; }
        [StringLength(2000)]
        public virtual string Url { get; set; }
        public virtual string Notes { get; set; }
        [Required]
        public virtual Order Order { get; set; }

        public virtual Commodity Commodity { get; set; }

        public virtual IList<Split> Splits { get; set; }

        public virtual void AddSplit(Split split)
        {
            split.LineItem = this;
            split.Order = Order;

            Splits.Add(split);
        }

        public virtual decimal Total()
        {
            return Quantity*UnitPrice;
        }

        public virtual decimal TotalWithTax()
        {
            return Total()*(1 + (Order.EstimatedTax/100.0m));
        }
    }

    public class LineItemMap : ClassMap<LineItem>
    {
        public LineItemMap()
        {
            Id(x => x.Id);

            Map(x => x.Quantity);
            Map(x => x.QuantityReceived);
            Map(x => x.QuantityPaid);
            Map(x => x.ReceivedNotes).Length(int.MaxValue);
            Map(x => x.PaidNotes).Length(int.MaxValue);
            Map(x => x.Received).ReadOnly();
            Map(x => x.Paid).ReadOnly();
            Map(x => x.CatalogNumber);
            Map(x => x.Description).Length(int.MaxValue);
            Map(x => x.Unit);
            Map(x => x.UnitPrice);
            Map(x => x.Url);
            Map(x => x.Notes).Length(int.MaxValue);

            References(x => x.Order);
            References(x => x.Commodity);

            HasMany(x => x.Splits).Inverse().ExtraLazyLoad().Cascade.AllDeleteOrphan();
        }
    }
}