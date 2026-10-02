using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Helpers;
using Purchasing.Core.Queries;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class EditableValuesTests
    {
        [TestMethod]
        public void WorkgroupEditPreservesIdentityAndTrackedRelationships()
        {
            var organization = new Organization { Id = "original" };
            var destination = new Workgroup { Id = 42, PrimaryOrganization = organization };
            destination.Organizations.Add(organization);
            destination.AddAccount(new WorkgroupAccount { Id = 1 });
            destination.AddAddress(new WorkgroupAddress { Id = 2 });
            destination.AddVendor(new WorkgroupVendor { Id = 3 });
            destination.AddPermission(new WorkgroupPermission { Id = 4 });
            destination.ConditionalApprovals.Add(new ConditionalApproval { Id = 5 });
            destination.Orders.Add(new Order { Id = 6 });
            var accounts = destination.Accounts;
            var addresses = destination.Addresses;
            var vendors = destination.Vendors;
            var permissions = destination.Permissions;
            var approvals = destination.ConditionalApprovals;
            var orders = destination.Orders;
            var organizations = destination.Organizations;
            var source = new Workgroup
            {
                Id = 999, Name = "Updated", Administrative = true, IsFullFeatured = true,
                Disclaimer = "Terms", SyncAccounts = true, AllowControlledSubstances = true,
                ForceAccountApprover = true, DefaultTag = "TAG", IsActive = false,
                NotificationEmailList = "test@example.org", RequireApproval = true,
                DoNotInheritPermissions = true, PrimaryOrganization = new Organization { Id = "other" }
            };

            EditableValues.CopyWorkgroup(source, destination);

            Assert.AreEqual(42, destination.Id);
            Assert.AreEqual("Updated", destination.Name);
            Assert.IsTrue(destination.Administrative);
            Assert.IsTrue(destination.IsFullFeatured);
            Assert.AreEqual("Terms", destination.Disclaimer);
            Assert.IsTrue(destination.SyncAccounts);
            Assert.IsTrue(destination.AllowControlledSubstances);
            Assert.IsTrue(destination.ForceAccountApprover);
            Assert.AreEqual("TAG", destination.DefaultTag);
            Assert.IsFalse(destination.IsActive);
            Assert.AreEqual("test@example.org", destination.NotificationEmailList);
            Assert.IsTrue(destination.RequireApproval);
            Assert.IsTrue(destination.DoNotInheritPermissions);
            Assert.AreSame(organization, destination.PrimaryOrganization);
            Assert.AreSame(organizations, destination.Organizations);
            Assert.AreSame(accounts, destination.Accounts);
            Assert.AreSame(addresses, destination.Addresses);
            Assert.AreSame(vendors, destination.Vendors);
            Assert.AreSame(permissions, destination.Permissions);
            Assert.AreSame(approvals, destination.ConditionalApprovals);
            Assert.AreSame(orders, destination.Orders);
            Assert.AreEqual(1, destination.Accounts[0].Id);
            Assert.AreEqual(2, destination.Addresses[0].Id);
            Assert.AreEqual(3, destination.Vendors[0].Id);
            Assert.AreEqual(4, destination.Permissions[0].Id);
            Assert.AreEqual(5, destination.ConditionalApprovals[0].Id);
            Assert.AreEqual(6, destination.Orders[0].Id);
            Assert.AreSame(organization, destination.Organizations[0]);
        }

        [TestMethod]
        public void NewWorkgroupDoesNotCloneExistingChildrenOrIdentity()
        {
            var source = new Workgroup { Id = 99, Name = "New group" };
            source.AddAccount(new WorkgroupAccount { Id = 10 });
            source.AddPermission(new WorkgroupPermission { Id = 11 });
            source.AddAddress(new WorkgroupAddress { Id = 12 });
            source.AddVendor(new WorkgroupVendor { Id = 13 });
            source.Orders.Add(new Order { Id = 14 });
            source.Organizations.Add(new Organization { Id = "other" });
            var destination = new Workgroup();

            EditableValues.CopyWorkgroup(source, destination);

            Assert.AreEqual(0, destination.Id);
            Assert.AreEqual("New group", destination.Name);
            Assert.AreEqual(0, destination.Accounts.Count);
            Assert.AreEqual(0, destination.Permissions.Count);
            Assert.AreEqual(0, destination.Addresses.Count);
            Assert.AreEqual(0, destination.Vendors.Count);
            Assert.AreEqual(0, destination.Orders.Count);
            Assert.AreEqual(0, destination.Organizations.Count);
        }

        [TestMethod]
        public void AutoApprovalEditKeepsOwnerAndUsesSelectedEntityWithoutCloningIt()
        {
            var owner = new User { Id = "owner" };
            var target = new User { Id = "target", FirstName = "Selected" };
            var destination = new AutoApproval { Id = 42, User = owner, Account = new Account() };
            var expiration = new DateTime(2027, 1, 15);
            var source = new AutoApproval
            {
                Id = 99, User = new User(), TargetUser = target, Account = null,
                MaxAmount = 125.50m, LessThan = true, Equal = true, IsActive = true, Expiration = expiration
            };

            EditableValues.CopyAutoApproval(source, destination);

            Assert.AreEqual(42, destination.Id);
            Assert.AreSame(owner, destination.User);
            Assert.AreSame(target, destination.TargetUser);
            Assert.AreEqual("Selected", target.FirstName);
            Assert.IsNull(destination.Account);
            Assert.AreEqual(125.50m, destination.MaxAmount);
            Assert.IsTrue(destination.LessThan);
            Assert.IsTrue(destination.Equal);
            Assert.IsTrue(destination.IsActive);
            Assert.AreEqual(expiration, destination.Expiration);

            var account = new Account { Id = "selected-account" };
            source.TargetUser = null;
            source.Account = account;
            source.Expiration = null;
            EditableValues.CopyAutoApproval(source, destination);
            Assert.IsNull(destination.TargetUser);
            Assert.AreSame(account, destination.Account);
            Assert.IsNull(destination.Expiration);
            Assert.AreSame(owner, destination.User);
        }

        [TestMethod]
        public void AddressReplacementHasFreshIdentityAndCopiesCampusLocation()
        {
            var workgroup = new Workgroup { Id = 42 };
            var building = new Building { Id = "building" };
            var source = new WorkgroupAddress
            {
                Id = 99, Workgroup = new Workgroup(), Name = "Delivery", Building = "Building",
                BuildingCode = building, Room = "12", Address = "1 College Way", City = "Davis",
                State = "CA", Zip = "95616", Phone = "555-0100", IsActive = true, AeLocationCode = "LOC"
            };
            var destination = new WorkgroupAddress { Workgroup = workgroup };

            EditableValues.CopyAddress(source, destination);

            Assert.AreEqual(0, destination.Id);
            Assert.AreSame(workgroup, destination.Workgroup);
            Assert.AreSame(building, destination.BuildingCode);
            Assert.AreEqual("Delivery", destination.Name);
            Assert.AreEqual("Building", destination.Building);
            Assert.AreEqual("12", destination.Room);
            Assert.AreEqual("1 College Way", destination.Address);
            Assert.AreEqual("Davis", destination.City);
            Assert.AreEqual("CA", destination.State);
            Assert.AreEqual("95616", destination.Zip);
            Assert.AreEqual("555-0100", destination.Phone);
            Assert.IsTrue(destination.IsActive);
            Assert.AreEqual("LOC", destination.AeLocationCode);
            Assert.AreEqual(99, source.Id);
            Assert.IsTrue(source.IsActive);
        }

        [TestMethod]
        public void VendorEditKeepsIdentityAndWorkgroupAndCanClearOptionalFields()
        {
            var workgroup = new Workgroup { Id = 42 };
            var destination = new WorkgroupVendor { Id = 7, Workgroup = workgroup, Line2 = "Old", Email = "old@example.org" };
            var source = new WorkgroupVendor
            {
                Id = 99, Workgroup = new Workgroup(), VendorId = "KFS", VendorAddressTypeCode = "TYPE",
                AeSupplierNumber = "SUPPLIER", AeSupplierSiteCode = "SITE", IsValidInAggieEnterprise = false,
                Name = "Vendor", Line1 = "Street", Line2 = null, Line3 = "Suite", City = "Davis",
                State = "CA", Zip = "95616", CountryCode = "US", IsActive = false,
                Phone = "phone", Fax = "fax", Email = null, Url = "https://example.org"
            };

            EditableValues.CopyVendor(source, destination);

            Assert.AreEqual(7, destination.Id);
            Assert.AreSame(workgroup, destination.Workgroup);
            Assert.AreEqual("KFS", destination.VendorId);
            Assert.AreEqual("TYPE", destination.VendorAddressTypeCode);
            Assert.AreEqual("SUPPLIER", destination.AeSupplierNumber);
            Assert.AreEqual("SITE", destination.AeSupplierSiteCode);
            Assert.IsFalse(destination.IsValidInAggieEnterprise);
            Assert.AreEqual("Vendor", destination.Name);
            Assert.AreEqual("Street", destination.Line1);
            Assert.IsNull(destination.Line2);
            Assert.AreEqual("Suite", destination.Line3);
            Assert.AreEqual("Davis", destination.City);
            Assert.AreEqual("CA", destination.State);
            Assert.AreEqual("95616", destination.Zip);
            Assert.AreEqual("US", destination.CountryCode);
            Assert.IsFalse(destination.IsActive);
            Assert.AreEqual("phone", destination.Phone);
            Assert.AreEqual("fax", destination.Fax);
            Assert.IsNull(destination.Email);
            Assert.AreEqual("https://example.org", destination.Url);
        }

        [TestMethod]
        public void CustomFieldEditCannotMoveTheFieldToAnotherOrganization()
        {
            var organization = new Organization { Id = "original" };
            var destination = new CustomField { Id = 42, Organization = organization };
            var source = new CustomField
            {
                Id = 99, Organization = new Organization(), Name = "Question", Rank = 3,
                IsActive = false, IsRequired = true
            };

            EditableValues.CopyCustomField(source, destination);

            Assert.AreEqual(42, destination.Id);
            Assert.AreSame(organization, destination.Organization);
            Assert.AreEqual("Question", destination.Name);
            Assert.AreEqual(3, destination.Rank);
            Assert.IsFalse(destination.IsActive);
            Assert.IsTrue(destination.IsRequired);
        }

        [TestMethod]
        public void ServiceMessageEditPreservesIdentityAndClearsEndDate()
        {
            var start = new DateTime(2027, 1, 1);
            var destination = new ServiceMessage { Id = 42, EndDisplayDate = start.AddDays(1) };
            var source = new ServiceMessage
            {
                Id = 99, Message = "Maintenance", BeginDisplayDate = start,
                EndDisplayDate = null, Critical = true, IsActive = false
            };

            EditableValues.CopyServiceMessage(source, destination);

            Assert.AreEqual(42, destination.Id);
            Assert.AreEqual("Maintenance", destination.Message);
            Assert.AreEqual(start, destination.BeginDisplayDate);
            Assert.IsNull(destination.EndDisplayDate);
            Assert.IsTrue(destination.Critical);
            Assert.IsFalse(destination.IsActive);
        }

        [TestMethod]
        public void SearchResultUsesOrderIdentityAndShippingFields()
        {
            var created = new DateTime(2026, 9, 1);
            var source = new OrderHistory
            {
                Id = 999, OrderId = 42, DateCreated = created, ShipTo = "Recipient", ShipToEmail = "ship@example.org",
                Justification = "Justification", BusinessPurpose = "Purpose", CreatedBy = "Creator",
                RequestNumber = "REQ", PoNumber = "PO", Tag = "TAG", ReferenceNumber = "REF",
                Approver = "Approver", AccountManager = "Manager", Purchaser = "Purchaser"
            };

            var result = SearchResults.OrderResult.FromHistory(source);

            Assert.AreEqual(42, result.Id);
            Assert.AreEqual(created, result.DateCreated);
            Assert.AreEqual("Recipient", result.DeliverTo);
            Assert.AreEqual("ship@example.org", result.DeliverToEmail);
            Assert.AreEqual("Justification", result.Justification);
            Assert.AreEqual("Purpose", result.BusinessPurpose);
            Assert.AreEqual("Creator", result.CreatedBy);
            Assert.AreEqual("REQ", result.RequestNumber);
            Assert.AreEqual("PO", result.PoNumber);
            Assert.AreEqual("TAG", result.Tag);
            Assert.AreEqual("REF", result.ReferenceNumber);
            Assert.AreEqual("Approver", result.Approver);
            Assert.AreEqual("Manager", result.AccountManager);
            Assert.AreEqual("Purchaser", result.Purchaser);
            Assert.IsNull(SearchResults.OrderResult.FromHistory(null));
        }
    }
}
