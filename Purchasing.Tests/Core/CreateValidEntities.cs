using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purchasing.Core.Domain;
using UCDArch.Testing;

namespace Purchasing.Tests.Core
{
    public static class CreateValidEntities
    {
        #region Helper Extension

        private static string Extra(this int? counter)
        {
            var extraString = "";
            if (counter != null)
            {
                extraString = counter.ToString();
            }
            return extraString;
        }

        #endregion Helper Extension

        // Once database design settles, uncomment and complete.

        public static Account Account(int? counter, bool loadAll = true)
        {
            var rtValue = new Account();
            rtValue.SetIdTo(counter.Extra());
            if (loadAll)
            {
                rtValue.Name = "Name" + counter.Extra();
                rtValue.AccountManager = "AccountManager" + counter.Extra();
                rtValue.PrincipalInvestigator = "PrincipalInvestigator" + counter.Extra();
                rtValue.IsActive = true;
            }
            return rtValue;
        }

        public static Audit Audit(int? counter, bool loadAll = true)
        {
            var rtValue = new Audit();
            rtValue.ObjectName = "ObjectName" + counter.Extra();
            rtValue.SetActionCode(AuditActionType.Create);
            rtValue.Username = "Username" + counter.Extra();
            rtValue.AuditDate = DateTime.Now.Date;
            if(loadAll)
            {
                rtValue.ObjectId = "ObjectId" + counter.Extra();
            }

            return rtValue;
        }

        public static Commodity Commodity(int? counter, bool loadAll = true)
        {
            var rtValue = new Commodity();
            rtValue.SetIdTo(counter.Extra());
            if (loadAll)
            {
                rtValue.Name = "Name" + counter.Extra();
                rtValue.GroupCode = "GroupCode" + counter.Extra();
                rtValue.SubGroupCode = "SubGroupCode" + counter.Extra();
            }

            return rtValue;
        }

        public static CommodityGroup CommodityGroup(int? counter, bool loadAll = true)
        {
            var rtValue = new CommodityGroup();
            rtValue.SetIdTo(counter.HasValue ? SpecificGuid.GetGuid(counter.Value) : Guid.NewGuid());
            if (loadAll)
            {
                rtValue.GroupCode = "GroupCode" + counter.Extra();
                rtValue.Name = "Name" + counter.Extra();
                rtValue.SubGroupCode = "SubGroupCode" + counter.Extra();
                rtValue.SubGroupName = "SubGroupName" + counter.Extra();
            }
            return rtValue;
        }

        public static EmailPreferences EmailPreferences(int? counter)
        {
            var rtValue = new EmailPreferences(counter.HasValue ? counter.Value.ToString() : "notSet");

            return rtValue;
        }

        public static AutoApproval AutoApproval(int i)
        {
            var rtResult = new AutoApproval();
            rtResult.IsActive = true;
            rtResult.LessThan = true;
            rtResult.MaxAmount = (decimal) 10.77;
            rtResult.TargetUser = CreateValidEntities.User(98);

            rtResult.User = CreateValidEntities.User(55);

            return rtResult;
        }

        public static User User(int i)
        {
            var rtResult = new User(i.ToString());
            rtResult.SetIdTo(i.ToString());
            rtResult.IsActive = true;
            rtResult.LastName = "LastName" + i;
            rtResult.FirstName = "FirstName" + i;
            rtResult.Email = string.Format("Email{0}@testy.com", i);
            return rtResult;
        }

        public static WorkgroupPermission WorkgroupPermission(int? counter)
        {
            var rtValue = new WorkgroupPermission();
            rtValue.Workgroup = new Workgroup();
            rtValue.Role = new Role();
            rtValue.User = new User();

            return rtValue;
        }

        public static WorkgroupAccount WorkgroupAccount(int? counter)
        {
            var rtValue = new WorkgroupAccount();
            rtValue.Account = new Account();
            rtValue.AccountManager = new User();
            rtValue.Approver = new User();
            rtValue.Purchaser = new User();
            rtValue.Workgroup = new Workgroup();

            return rtValue;
        }

        public static Workgroup Workgroup(int? counter)
        {
            var rtValue = new Workgroup();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.PrimaryOrganization = CreateValidEntities.Organization(counter);

            return rtValue;
        }

        public static Role Role(int? counter)
        {
            var rtValue = new Role(counter.Extra());
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;
        }

        public static Organization Organization(int? counter)
        {
            var rtValue = new Organization();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.TypeCode = "D";
            rtValue.TypeName = "TypeName" + counter.Extra();

            return rtValue;
        }


        public static WorkgroupAddress WorkgroupAddress(int? counter)
        {
            var rtValue = new WorkgroupAddress();
            rtValue.Workgroup = new Workgroup();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Building = "Building" + counter.Extra();
            rtValue.Room = "Room" + counter.Extra();
            rtValue.Address = "Address" + counter.Extra();
            rtValue.City = "City" + counter.Extra();
            rtValue.State = "CA";
            rtValue.Zip = "95616";
            rtValue.Phone = "(530) 555-5555";

            return rtValue;
        }

        public static State State(int? counter)
        {
            var locCounter = "99";
            if(counter.HasValue)
            {
                locCounter = counter.Extra();
            }
            var rtValue = new State();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.SetIdTo(locCounter);

            return rtValue;
        }

        public static WorkgroupVendor WorkgroupVendor(int? counter)
        {
            var rtValue = new WorkgroupVendor();
            rtValue.Workgroup = new Workgroup();
            rtValue.VendorId = "VendorId" + counter.Extra();
            rtValue.VendorAddressTypeCode = "tc" + counter.Extra();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Line1 = "Line1" + counter.Extra();
            rtValue.City = "City" + counter.Extra();
            rtValue.State = "CA";
            rtValue.Zip = "95616";
            rtValue.CountryCode = "US";

            return rtValue;
        }

        public static Vendor Vendor(int? counter)
        {
            var rtValue = new Vendor();
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;

        }

        public static VendorAddress VendorAddress(int? counter)
        {
            var rtValue = new VendorAddress();
            rtValue.TypeCode = "tc" + counter.Extra();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Line1 = "Line1" + counter.Extra();
            rtValue.Line2 = "Line2" + counter.Extra();
            rtValue.Line3 = "Line3" + counter.Extra();
            rtValue.City = "City" + counter.Extra();
            rtValue.State = "XX";
            rtValue.Zip = "12345";
            rtValue.CountryCode = "AA";

            return rtValue;
        }

        public static Order Order(int? counter)
        {
            var rtValue = new Order();
            
            rtValue.DeliverTo = "DeliverTo" + counter.Extra();

            return rtValue;
        }

        public static OrderStatusCode OrderStatusCode(int? counter)
        {
            var rtValue = new OrderStatusCode();
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;
        }

        public static Approval Approval(int? counter)
        {
            var rtValue = new Approval();
            rtValue.User = new User();
            rtValue.Order = new Order();
            rtValue.StatusCode = new OrderStatusCode();

            return rtValue;
        }

        public static OrderTracking OrderTracking(int? counter)
        {
            var rtValue = new OrderTracking();
            rtValue.DateCreated = DateTime.Now;
            rtValue.Description = "Description" + counter.Extra();
            rtValue.Order = new Order();
            rtValue.StatusCode = new OrderStatusCode();

            return rtValue;
        }

        public static ConditionalApproval ConditionalApproval(int? counter)
        {
            var rtValue = new ConditionalApproval();
            rtValue.Question = "Question" + counter.Extra();
            rtValue.PrimaryApprover = new User();
            rtValue.Workgroup = new Workgroup();

            return rtValue;
        }

        public static SubAccount SubAccount(int? counter)
        {
            var rtValue = new SubAccount();
            rtValue.AccountNumber = "AccountNumber" + counter.Extra();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.SubAccountNumber = "SubAccountNumber" + counter.Extra();

            return rtValue;
        }

        public static Attachment Attachment(int? counter)
        {
            var rtValue = new Attachment();
            rtValue.FileName = "FileName" + counter.Extra();
            rtValue.ContentType = "ContentType" + counter.Extra();
            rtValue.Contents = new byte[]{1,2,3,4,5};
            rtValue.User = new User();
            //rtValue.Order = 
            
            return rtValue;
        }
    }
}
