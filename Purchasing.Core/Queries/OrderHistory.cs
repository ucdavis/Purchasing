﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;

namespace Purchasing.Core.Queries
{
    public class OrderHistory : DomainObject
    {
        // ids
        public virtual int OrderId { get; set; }
        public virtual int WorkgroupId { get; set; }
        public virtual string StatusId { get; set; }
        public virtual string OrderTypeId { get; set; }

        public virtual string RequestNumber { get; set; }
        public virtual string WorkgroupName { get; set; }
        public virtual string Vendor { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string CreatorId { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string Status { get; set; }
        public virtual bool IsComplete { get; set; }
        public virtual decimal TotalAmount { get; set; }
        public virtual string LineItems { get; set; }
        public virtual string AccountSummary { get; set; }
        public virtual bool HasAccountSplit { get; set; }
        public virtual string ShipTo { get; set; }
        public virtual string AllowBackorder { get; set; }
        public virtual string Restricted { get; set; }
        public virtual DateTime DateNeeded { get; set; }
        public virtual string ShippingType { get; set; }
        public virtual string ReferenceNumber { get; set; }
        public virtual DateTime LastActionDate { get; set; }
        public virtual string LastActionUser { get; set; }
        public virtual string Received { get; set; }
        public virtual string OrderType { get; set; }
        public virtual decimal ShippingAmount { get; set; }
    }

    public class OrderHistoryMap : ClassMap<OrderHistory>
    {
        public OrderHistoryMap()
        {
            Table("vOrderHistory");

            ReadOnly();

            Id(x => x.Id);

            Map(x => x.OrderId);
            Map(x => x.WorkgroupId);
            Map(x => x.StatusId);
            Map(x => x.OrderTypeId);

            Map(x => x.RequestNumber);
            Map(x => x.WorkgroupName);
            Map(x => x.Vendor);
            Map(x => x.CreatedBy);
            Map(x => x.CreatorId);
            Map(x => x.DateCreated);
            Map(x => x.Status);
            Map(x => x.IsComplete);
            Map(x => x.TotalAmount);
            Map(x => x.LineItems);
            Map(x => x.AccountSummary).Column("accountsubaccountsummary");
            Map(x => x.HasAccountSplit);
            Map(x => x.ShipTo);
            Map(x => x.AllowBackorder);
            Map(x => x.Restricted);
            Map(x => x.DateNeeded);
            Map(x => x.ShippingType);
            Map(x => x.ReferenceNumber);
            Map(x => x.LastActionDate);
            Map(x => x.LastActionUser);
            Map(x => x.Received);
            Map(x => x.OrderType);
            Map(x => x.ShippingAmount);
        }
    }
}
