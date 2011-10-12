using System.Collections.Generic;
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

        public virtual int Quantity { get; set; }
        public virtual string CatalogNumber { get; set; }
        public virtual string Description { get; set; }
        public virtual string Unit { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual string Url { get; set; }
        public virtual string Notes { get; set; }
        public virtual Order Order { get; set; }

        public virtual Commodity Commodity { get; set; }

        public virtual IList<Split> Splits { get; set; }

        public virtual void AddSplit(Split split)
        {
            split.LineItem = this;
            split.Order = Order;

            Splits.Add(split);
        }
    }

    public class LineItemMap : ClassMap<LineItem>
    {
        public LineItemMap()
        {
            Id(x => x.Id);

            Map(x => x.Quantity);
            Map(x => x.CatalogNumber);
            Map(x => x.Description);
            Map(x => x.Unit);
            Map(x => x.UnitPrice);
            Map(x => x.Url);
            Map(x => x.Notes);

            References(x => x.Order);
            References(x => x.Commodity);

            HasMany(x => x.Splits).Inverse().ExtraLazyLoad().Cascade.AllDeleteOrphan();
        }
    }
}