using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Web.Helpers;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests
{
    [TestClass]
    public class WorkgroupServiceTests
    {
        public IWorkgroupService WorkgroupService;
        public IRepositoryWithTypedId<Vendor, string> VendorRepository;
        public IRepositoryWithTypedId<VendorAddress, Guid> VendorAddressRepository;
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<EmailPreferences, string> EmailPreferencesRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<Workgroup> WorkgroupRepository;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository;
        public IDirectorySearchService SearchService;
        public IRepositoryFactory RepositoryFactory;

        #region Init
        public WorkgroupServiceTests()
        {
            AutomapperConfig.Configure();
            VendorRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Vendor, string>>();
            VendorAddressRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<VendorAddress, Guid>>();
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            EmailPreferencesRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<EmailPreferences, string>>();
            WorkgroupPermissionRepository = MockRepository.GenerateStub<IRepository<WorkgroupPermission>>();
            WorkgroupRepository = MockRepository.GenerateStub<IRepository<Workgroup>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            SearchService = MockRepository.GenerateStub<IDirectorySearchService>();
            RepositoryFactory = MockRepository.GenerateStub<IRepositoryFactory>();

            WorkgroupService = new WorkgroupService(VendorRepository,
                VendorAddressRepository,
                UserRepository,
                EmailPreferencesRepository,
                WorkgroupPermissionRepository,
                WorkgroupRepository,
                OrganizationRepository,
                SearchService,
                RepositoryFactory);
        }
        #endregion Init

        #region TransferValues Tests
        
        [TestMethod]
        public void TestTransferValuesWhenKfsVendorHasNullState()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].SetIdTo("1");
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].State = null;
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();
            Assert.AreNotEqual(destination.State, "XX");
            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("XX", destination.State);
            #endregion Assert		
        }

        [TestMethod]
        public void TestTransferValuesWhenKfsVendorHasNullZip()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].SetIdTo("1");
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = null;
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();
            Assert.AreNotEqual(destination.Zip, "XXXXX");
            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("XXXXX", destination.Zip);
            #endregion Assert
        }

        [TestMethod]
        public void TestTransferValuesWhenKfsVendor()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Name = "VendorName1";
            vendors[0].SetIdTo("1");
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = "98765";
            vendorAddress[0].Name = "DoesNotMatter";
            vendorAddress[0].Line1 = "VaLine1";
            vendorAddress[0].Line2 = "VaLine2";
            vendorAddress[0].Line3 = "VaLine3";
            vendorAddress[0].City = "VaCity1";
            vendorAddress[0].State = "AB";
            vendorAddress[0].CountryCode = "UK";
            vendorAddress[0].PhoneNumber = "333-444-55555";
            vendorAddress[0].FaxNumber = "222-444-55555";
            
            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = "1";
            source.VendorAddressTypeCode = "2";

            var destination = new WorkgroupVendor();

            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("VendorName1", destination.Name);
            Assert.AreEqual("VaLine1", destination.Line1);
            Assert.AreEqual("VaLine2", destination.Line2);
            Assert.AreEqual("VaLine3", destination.Line3);
            Assert.AreEqual("VaCity1", destination.City);
            Assert.AreEqual("AB", destination.State);
            Assert.AreEqual("98765", destination.Zip);
            Assert.AreEqual("UK", destination.CountryCode);
            Assert.AreEqual("333-444-55555", destination.Phone);
            Assert.AreEqual("222-444-55555", destination.Fax);
            #endregion Assert
        }

        [TestMethod]
        public void TestTransferValuesWhenNotKfsVendor()
        {
            #region Arrange
            var vendors = new List<Vendor>();
            vendors.Add(CreateValidEntities.Vendor(1));
            vendors[0].Name = "VendorName1";
            vendors[0].SetIdTo("1");
            new FakeVendors(0, VendorRepository, vendors, true);
            var vendorAddress = new List<VendorAddress>();
            vendorAddress.Add(CreateValidEntities.VendorAddress(2));
            vendorAddress[0].Vendor = VendorRepository.Queryable.Single(a => a.Id == "1");
            vendorAddress[0].TypeCode = "2";
            vendorAddress[0].Zip = "98765";
            vendorAddress[0].Name = "DoesNotMatter";
            vendorAddress[0].Line1 = "VaLine1";
            vendorAddress[0].Line2 = "VaLine2";
            vendorAddress[0].Line3 = "VaLine3";
            vendorAddress[0].City = "VaCity1";
            vendorAddress[0].State = "AB";
            vendorAddress[0].CountryCode = "UK";
            vendorAddress[0].PhoneNumber = "333-444-55555";
            vendorAddress[0].FaxNumber = "222-444-55555";

            new FakeVendorAddresses(0, VendorAddressRepository, vendorAddress, false);

            var source = CreateValidEntities.WorkgroupVendor(1);
            source.VendorId = null;
            source.VendorAddressTypeCode = "2";
            source.Phone = "111";
            source.Fax = "222";

            var destination = new WorkgroupVendor();

            #endregion Arrange

            #region Act
            WorkgroupService.TransferValues(source, ref destination);
            #endregion Act

            #region Assert
            Assert.AreEqual("Name1", destination.Name);
            Assert.AreEqual("Line11", destination.Line1);
            Assert.AreEqual(null, destination.Line2);
            Assert.AreEqual(null, destination.Line3);
            Assert.AreEqual("City1", destination.City);
            Assert.AreEqual("CA", destination.State);
            Assert.AreEqual("95616", destination.Zip);
            Assert.AreEqual("US", destination.CountryCode);
            Assert.AreEqual("111", destination.Phone);
            Assert.AreEqual("222", destination.Fax);
            #endregion Assert
        }
        #endregion TransferValues Tests

        #region TryToAddPeople Tests

        [TestMethod]
        public void TestDescription()
        {
            #region Arrange
            Assert.Inconclusive("Write these tests");
            #endregion Arrange

            #region Act
            #endregion Act

            #region Assert
            #endregion Assert		
        } 
        #endregion TryToAddPeople Tests
    }
}
