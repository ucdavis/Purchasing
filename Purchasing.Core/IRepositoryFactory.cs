using System;
using Purchasing.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using Purchasing.Core.Repositories;

namespace Purchasing.Core
{
    public interface IRepositoryFactory
    {
        IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        IRepository<Approval> ApprovalRepository { get; set; }
        IRepositoryWithTypedId<Attachment, Guid> AttachmentRepository { get; set; }
        IRepository<AutoApproval> AutoApprovalRepository { get; set; }
        IRepositoryWithTypedId<ColumnPreferences, string> ColumnPreferencesRepository { get; set; }
        IRepositoryWithTypedId<Commodity, string> CommodityRepository { get; set; }
        IRepository<ConditionalApproval> ConditionalApprovalRepository { get; set; }
        ISearchRepository SearchRepository { get; set; }
        IRepository<ControlledSubstanceInformation> ControlledSubstanceInformationRepository { get; set; }
        IRepository<CustomFieldAnswer> CustomFieldAnswerRepository { get; set; }
        IRepository<CustomField> CustomFieldRepository { get; set; }
        IRepository<LineItem> LineItemRepository { get; set; }
        IRepository<Order> OrderRepository { get; set; }
        IRepository<OrderComment> OrderCommentRepository { get; set; }
        IRepositoryWithTypedId<OrderStatusCode,string> OrderStatusCodeRepository { get; set; }
        IRepository<OrderTracking> OrderTrackingRepository { get; set; }
        IRepositoryWithTypedId<OrderType, string> OrderTypeRepository { get; set; }
        IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        IRepositoryWithTypedId<ShippingType, string> ShippingTypeRepository { get; set; }
        IRepository<Split> SplitRepository { get; set; }
        IRepositoryWithTypedId<UnitOfMeasure, string> UnitOfMeasureRepository { get; set; }
        IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        IRepository<Workgroup> WorkgroupRepository { get; set; }
        IRepository<WorkgroupPermission> WorkgroupPermissionRepository { get; set; }
        IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        IRepository<WorkgroupAddress> WorkgroupAddressRepository { get; set; }
        IRepository<WorkgroupVendor> WorkgroupVendorRepository { get; set; }
        IRepository<Building> BuildingRepository { get; set; }
        IRepository<ServiceMessage> ServiceMessageRepository { get; set; } 
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepositoryWithTypedId<Account, string> AccountRepository { get; set; }
        public IRepository<Approval> ApprovalRepository { get; set; }
        public IRepositoryWithTypedId<Attachment,Guid> AttachmentRepository { get; set; }
        public IRepository<AutoApproval> AutoApprovalRepository { get; set; }
        public IRepositoryWithTypedId<ColumnPreferences, string> ColumnPreferencesRepository { get; set; }
        public IRepositoryWithTypedId<Commodity, string> CommodityRepository { get; set; }
        public IRepository<ConditionalApproval> ConditionalApprovalRepository { get; set; }
        public ISearchRepository SearchRepository { get; set; }
        public IRepository<ControlledSubstanceInformation> ControlledSubstanceInformationRepository { get; set; }
        public IRepository<CustomFieldAnswer> CustomFieldAnswerRepository { get; set; }
        public IRepository<CustomField> CustomFieldRepository { get; set; }
        public IRepository<LineItem> LineItemRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }
        public IRepository<OrderComment> OrderCommentRepository { get; set; }
        public IRepositoryWithTypedId<OrderStatusCode,string> OrderStatusCodeRepository { get; set; }
        public IRepository<OrderTracking> OrderTrackingRepository { get; set; }
        public IRepositoryWithTypedId<OrderType, string> OrderTypeRepository { get; set; }
        public IRepositoryWithTypedId<Role, string> RoleRepository { get; set; }
        public IRepositoryWithTypedId<ShippingType, string> ShippingTypeRepository { get; set; }
        public IRepository<Split> SplitRepository { get; set; }
        public IRepositoryWithTypedId<UnitOfMeasure, string> UnitOfMeasureRepository { get; set; }
        public IRepositoryWithTypedId<User, string> UserRepository { get; set; }
        public IRepository<WorkgroupAccount> WorkgroupAccountRepository { get; set; }
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository { get; set; }
        public IRepository<Workgroup> WorkgroupRepository { get; set; }
        public IRepository<WorkgroupAddress> WorkgroupAddressRepository { get; set; }
        public IRepository<WorkgroupVendor> WorkgroupVendorRepository { get; set; }
        public IRepository<Building> BuildingRepository { get; set; }
        public IRepository<ServiceMessage> ServiceMessageRepository { get; set; } 
    }
}