using Purchasing.Core.Domain;

namespace Purchasing.Core.Helpers
{
    /// <summary>
    /// Copies values accepted by edit forms without replacing database identities
    /// or traversing NHibernate-owned relationships.
    /// </summary>
    public static class EditableValues
    {
        public static void CopyWorkgroup(Workgroup source, Workgroup destination)
        {
            destination.Name = source.Name;
            destination.Administrative = source.Administrative;
            destination.IsFullFeatured = source.IsFullFeatured;
            destination.Disclaimer = source.Disclaimer;
            destination.SyncAccounts = source.SyncAccounts;
            destination.AllowControlledSubstances = source.AllowControlledSubstances;
            destination.ForceAccountApprover = source.ForceAccountApprover;
            destination.DefaultTag = source.DefaultTag;
            destination.IsActive = source.IsActive;
            destination.NotificationEmailList = source.NotificationEmailList;
            destination.RequireApproval = source.RequireApproval;
            destination.DoNotInheritPermissions = source.DoNotInheritPermissions;
            // The create/edit workflows resolve PrimaryOrganization and Organizations.
            // Accounts, addresses, vendors, permissions, approvals and orders have
            // their own workflows and must retain their tracked collections.
        }

        public static void CopyAutoApproval(AutoApproval source, AutoApproval destination)
        {
            destination.TargetUser = source.TargetUser;
            destination.Account = source.Account;
            destination.MaxAmount = source.MaxAmount;
            destination.LessThan = source.LessThan;
            destination.Equal = source.Equal;
            destination.IsActive = source.IsActive;
            destination.Expiration = source.Expiration;
        }

        public static void CopyAddress(WorkgroupAddress source, WorkgroupAddress destination)
        {
            destination.Name = source.Name;
            destination.Building = source.Building;
            destination.BuildingCode = source.BuildingCode;
            destination.Room = source.Room;
            destination.Address = source.Address;
            destination.City = source.City;
            destination.State = source.State;
            destination.Zip = source.Zip;
            destination.Phone = source.Phone;
            destination.IsActive = source.IsActive;
            destination.AeLocationCode = source.AeLocationCode;
        }

        public static void CopyVendor(WorkgroupVendor source, WorkgroupVendor destination)
        {
            destination.VendorId = source.VendorId;
            destination.VendorAddressTypeCode = source.VendorAddressTypeCode;
            destination.AeSupplierNumber = source.AeSupplierNumber;
            destination.AeSupplierSiteCode = source.AeSupplierSiteCode;
            destination.IsValidInAggieEnterprise = source.IsValidInAggieEnterprise;
            destination.Name = source.Name;
            destination.Line1 = source.Line1;
            destination.Line2 = source.Line2;
            destination.Line3 = source.Line3;
            destination.City = source.City;
            destination.State = source.State;
            destination.Zip = source.Zip;
            destination.CountryCode = source.CountryCode;
            destination.IsActive = source.IsActive;
            destination.Phone = source.Phone;
            destination.Fax = source.Fax;
            destination.Email = source.Email;
            destination.Url = source.Url;
        }

        public static void CopyCustomField(CustomField source, CustomField destination)
        {
            destination.Name = source.Name;
            destination.Rank = source.Rank;
            destination.IsActive = source.IsActive;
            destination.IsRequired = source.IsRequired;
        }

        public static void CopyServiceMessage(ServiceMessage source, ServiceMessage destination)
        {
            destination.Message = source.Message;
            destination.BeginDisplayDate = source.BeginDisplayDate;
            destination.EndDisplayDate = source.EndDisplayDate;
            destination.Critical = source.Critical;
            destination.IsActive = source.IsActive;
        }
    }
}
