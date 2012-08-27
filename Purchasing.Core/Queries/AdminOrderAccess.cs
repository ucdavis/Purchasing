//using FluentNHibernate.Mapping;
//using UCDArch.Core.DomainModel;

//namespace Purchasing.Core.Queries
//{
//    public class AdminOrderAccess : DomainObject
//    {
//        public virtual int AdminWorkgroupId { get; set; }
//        public virtual int DescendantWorkgroupId { get; set; }

//        public virtual string OrgId { get; set; }
//        public virtual string RollupParentId { get; set; }
        
//        public virtual int OrderId { get; set; }
//        public virtual string AccessUserId { get; set; }
//        public virtual bool IsAway { get; set; }
//        public virtual string RoleId { get; set; }

//        public virtual string OrderStatusCode { get; set; }
//        public virtual bool IsComplete { get; set; }
//        public virtual bool IsPending { get; set; }
//    }

//    public class AdminOrderAccessMap : ClassMap<AdminOrderAccess>
//    {
//        public AdminOrderAccessMap()
//        {
//            Id(x => x.Id);

//            Table("vAdminOrderAccess");
//            ReadOnly();

//            Map(x => x.AdminWorkgroupId);
//            Map(x => x.DescendantWorkgroupId);
            
//            Map(x => x.OrgId);
//            Map(x => x.RollupParentId);

//            Map(x => x.OrderId);
//            Map(x => x.AccessUserId);
//            Map(x => x.IsAway);
//            Map(x => x.RoleId);

//            Map(x => x.OrderStatusCode);
//            Map(x => x.IsComplete);
//            Map(x => x.IsPending);
//        }
//    }

//}
