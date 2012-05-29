using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
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

        public static AutoApproval AutoApproval(int? i)
        {
            var rtResult = new AutoApproval();
            rtResult.IsActive = true;
            rtResult.LessThan = true;
            rtResult.MaxAmount = (decimal) 10.77;
            rtResult.TargetUser = CreateValidEntities.User(98);

            rtResult.User = CreateValidEntities.User(55);

            return rtResult;
        }

        public static User User(int? i)
        {
            var rtResult = new User(i.HasValue? i.Value.ToString():"99");
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

        public static WorkgroupAccount WorkgroupAccount(int? counter, bool noNews = false)
        {
            var rtValue = new WorkgroupAccount();
            if(!noNews)
            { 
                rtValue.AccountManager = new User();
                rtValue.Approver = new User();
                rtValue.Purchaser = new User();
            }
            rtValue.Account = new Account();
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
            rtValue.Justification = "Justification" + counter.Extra();
            rtValue.OrderType = new OrderType();
            //rtValue.Vendor = new WorkgroupVendor();
            rtValue.Address = new WorkgroupAddress();
            rtValue.Workgroup = new Workgroup();
            rtValue.Organization = new Organization();
            rtValue.StatusCode = new OrderStatusCode();

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
            rtValue.AccountNumber = "Acc" + counter.Extra();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.SubAccountNumber = "Sub" + counter.Extra();

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

        public static ColumnPreferences ColumnPreferences(int? counter)
        {
            var rtValue = new ColumnPreferences(counter.Extra());
            return rtValue;
        }

        public static ControlledSubstanceInformation ControlledSubstanceInformation(int? counter)
        {
            var rtValue = new ControlledSubstanceInformation();
            //rtValue.AuthorizationNum = "Auth" + counter.Extra();
            rtValue.ClassSchedule = "Class" + counter.Extra();
            rtValue.Use = "Use" + counter.Extra();
            rtValue.StorageSite = "StorageSite" + counter.Extra();
            rtValue.Custodian = "Custodian" + counter.Extra();
            rtValue.EndUser = "EndUser" + counter.Extra();
            rtValue.Order = new Order();

            return rtValue;
        }

        public static EmailQueue EmailQueue(int? counter)
        {
            var rtValue = new EmailQueue();
            rtValue.Text = "Text" + counter.Extra();
            rtValue.Order = new Order();
            rtValue.NotificationType = Purchasing.Core.Domain.EmailPreferences.NotificationTypes.PerEvent;

            return rtValue;
        }

        public static KfsDocument KfsDocument(int? counter)
        {
            var rtValue = new KfsDocument();
            rtValue.DocNumber = "DocNumber" + counter.Extra();
            rtValue.Order = new Order();

            return rtValue;
        }

        public static LineItem LineItem(int? counter)
        {
            var rtValue = new LineItem();
            rtValue.Quantity = counter.HasValue ? counter.Value : 9;
            rtValue.Description = "Description" + counter.Extra();
            rtValue.Unit = "Unit" + counter.Extra(); //Not required, just used for tests
            rtValue.Order = new Order();

            return rtValue;
        }

        public static Split Split(int? counter)
        {
            var rtValue = new Split();
            rtValue.Order = new Order();
            rtValue.Project = "Project" + counter.Extra(); //Not Required, just for tests

            return rtValue;
        }

        public static Notification Notification(int? counter)
        {
            var rtValue = new Notification();
            rtValue.User = new User();
            rtValue.Status = "Status" + counter.Extra();

            return rtValue;
        }

        public static ShippingType ShippingType(int? counter)
        {
            var rtValue = new ShippingType(counter.HasValue ? counter.Value.ToString() : "99");
            rtValue.Name = "Name" + counter.Extra();
            return rtValue;
        }

        public static OrderComment OrderComment(int? counter)
        {
            var rtValue = new OrderComment();
            rtValue.Order = new Order();
            rtValue.User = new User();
            rtValue.Text = "Text" + counter.Extra();

            return rtValue;
        }

        public static OrderType OrderType(int? counter)
        {
            var rtValue = new OrderType(counter.HasValue ? counter.Value.ToString() : "99");
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;
        }

        public static UnitOfMeasure UnitOfMeasure(int? counter)
        {
            var rtValue = new UnitOfMeasure();
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;
        }

        public static CustomFieldAnswer CustomFieldAnswer(int? counter)
        {
            var rtValue = new CustomFieldAnswer();
            rtValue.Answer = "Answer" + counter.Extra();
            rtValue.CustomField = new CustomField();
            rtValue.Order = new Order();

            return rtValue;
        }

        public static CustomField CustomField(int? counter)
        {
            var rtValue = new CustomField();
            rtValue.Name = "Name" + counter.Extra();
            rtValue.Organization = new Organization();

            return rtValue;
        }

        public static OrganizationDescendant OrganizationDescendant(int? counter)
        {
            var rtValue = new OrganizationDescendant();           
            rtValue.OrgId = "OrgId" + counter.Extra();

            return rtValue;
        }

        public static AdminWorkgroup AdminWorkgroup(int? counter)
        {
            var rtValue = new AdminWorkgroup();
            rtValue.Name = "Name" + counter.Extra();

            return rtValue;
        }

        public static Building Building(int? counter)
        {
            var rtValue = new Building();
            var locCounter = "99";
            if(counter.HasValue)
            {
                locCounter = counter.Extra();
            }
            rtValue.SetIdTo(locCounter);
            rtValue.BuildingCode = "BuildingCode" + counter.Extra();
            rtValue.BuildingName = "BuildingName" + counter.Extra();
            rtValue.CampusCode = "CampusCode" + counter.Extra();
            rtValue.CampusName = "CampusName" + counter.Extra();
            rtValue.CampusShortName = "CampusShortName" + counter.Extra();
            rtValue.CampusTypeCode = "CampusTypeCode" + counter.Extra();
            rtValue.IsActive = true;
            rtValue.LastUpdateDate = DateTime.Now;

            return rtValue;
        }

        public static HistoryReceivedLineItem HistoryReceivedLineItem(int? counter)
        {
            var rtValue = new HistoryReceivedLineItem();
            rtValue.NewReceivedQuantity = counter.HasValue ? counter.Value : 0;
            rtValue.LineItem = new LineItem();
            rtValue.User = new User();

            return rtValue;
        }
    }
}
