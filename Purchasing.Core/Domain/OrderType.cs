using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Domain
{
    public class OrderType : DomainObjectWithTypedId<string>
    {
        public OrderType()
        {
    
        }

        public OrderType(string id)
        {
            Id = id;
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual bool PurchaserAssignable { get; set; }

        /// <summary>
        /// Temporary property to determine doc type if the orderType is KFS
        /// </summary>
        public virtual string DocType { get; set; }

        public static class Types
        {
            public const string DepartmentalPurchaseOrder = "DPO";
            public const string DepartmentalRepairOrder = "DRO";
            public const string OrderRequest = "OR";
            public const string PurchasingCard = "PC";
            public const string PurchaseRequest = "PR";
            public const string UcdBuyOrder = "UCB";
            public const string KfsDocument = "KFS";
            public const string CampusPurchaseOrder = "PO";
        }
    }

    public class OrderTypeMap : ClassMap<OrderType>
    {
        public OrderTypeMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Name);
            Map(x => x.PurchaserAssignable);
        }
    }
}