using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using UCDArch.Testing.Extensions;
using Moq;

namespace Purchasing.Tests.ControllerTests.WorkgroupControllerTests
{
    public partial class WorkgroupControllerTests
    {
        #region Addresses Tests

        [TestMethod]
        public void TestAddressesRedirectsWhenWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.Addresses(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddressesReturnView()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            for (int i = 0; i < 3; i++)
            {
                workgroups.Add(CreateValidEntities.Workgroup(i+1));
                workgroups[i].Addresses = new List<WorkgroupAddress>();
                for (int j = 0; j < 3; j++)
                {
                    var address = CreateValidEntities.WorkgroupAddress((i*3) + (j + 1));
                    if(j == 1)
                    {
                        address.IsActive = false;
                    }
                    workgroups[i].AddAddress(address);

                }
            }
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);
            #endregion Arrange

            #region Act
            var result = Controller.Addresses(3)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressListModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Name3", result.Workgroup.Name);
            Assert.AreEqual(2, result.WorkgroupAddresses.Count());
            Assert.AreEqual("Name7", result.WorkgroupAddresses.ElementAt(0).Name);
            Assert.AreEqual("Name9", result.WorkgroupAddresses.ElementAt(1).Name);
            #endregion Assert		
        }
        #endregion Addresses Tests

        #region AddAddress Get Tests

        [TestMethod]
        public void TestAddAddressGetRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddAddress(4)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddAddressGetReturnsView()
        {
            #region Arrange
            new FakeStates(3, StateRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            var result = Controller.AddAddress(2)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.WorkgroupAddress.Id);
            Assert.AreEqual(2, result.Workgroup.Id);
            Assert.AreEqual(2,result.WorkgroupAddress.Workgroup.Id);
            Assert.AreEqual(3, result.States.Count());
            #endregion Assert		
        }
        #endregion AddAddress Get Tests

        #region AddAddress Post Tests
        [TestMethod]
        public void TestAddAddressPostRedirectsIfWorkgroupNotFound()
        {
            #region Arrange
            new FakeWorkgroups(3, WorkgroupRepository);
            #endregion Arrange

            #region Act
            Controller.AddAddress(4, new WorkgroupAddress())
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual("Workgroup could not be found.", Controller.ErrorMessage);
            Mock.Get(WorkgroupAddressService).Verify(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>()), Times.Never());
            #endregion Assert
        }


        [TestMethod]
        public void TestAddAddressPostReturnsViewWhenInvalid()
        {
            #region Arrange
            new FakeStates(3, StateRepository);
            new FakeWorkgroups(3, WorkgroupRepository);
            var address = CreateValidEntities.WorkgroupAddress(4);
            address.Name = string.Empty;
            #endregion Arrange

            #region Act
            var result = Controller.AddAddress(2, address)
                .AssertViewRendered()
                .WithViewData<WorkgroupAddressViewModel>();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.WorkgroupAddress.Id);
            Assert.AreEqual(2, result.Workgroup.Id);
            Assert.AreEqual(2, result.WorkgroupAddress.Workgroup.Id);
            Assert.AreEqual(3, result.States.Count());
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(It.IsAny<Workgroup>()), Times.Never());
            Assert.AreEqual("Address not valid", Controller.ErrorMessage);
            Mock.Get(WorkgroupAddressService).Verify(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>()), Times.Never());
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddAddressPostWhenNoMatchRedirects1()
        {
            #region Arrange
            SetupDataForAddress();
            var address = CreateValidEntities.WorkgroupAddress(10);
            var compareArgs = new Dictionary<int, object[]>();
            var compareArgsIndex = 0;
            Mock.Get(WorkgroupAddressService).Setup(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>())).Returns(0)
                .Callback((object[] x) => compareArgs[compareArgsIndex++] = x);
            Workgroup args = default;
            Mock.Get( WorkgroupRepository).Setup(a => a.EnsurePersistent(It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.AddAddress(2, address)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("Address created", Controller.Message);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(It.IsAny<Workgroup>()));
 
            Assert.IsNotNull(args);
            Assert.AreEqual(2, args.Id);
            Assert.AreEqual(4, args.Addresses.Count());
            Assert.AreEqual("Name10", args.Addresses[3].Name);

            Mock.Get(WorkgroupAddressService).Verify(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>()), Times.Exactly(3));
            Assert.IsNotNull(compareArgs);
            Assert.AreEqual(3, compareArgs.Count());
            Assert.AreEqual("Name4", ((WorkgroupAddress)compareArgs[0][1]).Name);
            Assert.AreEqual("Name5", ((WorkgroupAddress)compareArgs[1][1]).Name);
            Assert.AreEqual("Name6", ((WorkgroupAddress)compareArgs[2][1]).Name);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual("Name10", ((WorkgroupAddress)compareArgs[i][0]).Name);
            }
            #endregion Assert		
        }


        [TestMethod]
        public void TestAddAddressPostWhenInactiveMatchFound()
        {
            #region Arrange
            SetupDataForAddress();
            var address = CreateValidEntities.WorkgroupAddress(10);
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[0]))
                .Returns(0)
                .Throws(new Exception("Called  for address 0 too many times"));
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[1]))
                .Returns(5)
                .Throws(new Exception("Called  for address 1 too many times"));
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[2]))
                .Returns(0)
                .Throws(new Exception("Called  for address 2 too many times"));
            Workgroup args = default;
            Mock.Get(WorkgroupRepository).Setup(a => a.EnsurePersistent(It.IsAny<Workgroup>()))
                .Callback<Workgroup>(x => args = x);
            #endregion Arrange

            #region Act
            var result = Controller.AddAddress(2, address)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("Address created.", Controller.Message);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(It.IsAny<Workgroup>()));

            Assert.IsNotNull(args);
            Assert.AreEqual(2, args.Id);
            Assert.AreEqual(3, args.Addresses.Count());
            Assert.AreEqual(true, args.Addresses[1].IsActive);
            Assert.AreEqual("Name5", args.Addresses[1].Name);

            Mock.Get(WorkgroupAddressService).Verify(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>()), Times.Exactly(2));
            #endregion Assert		
        }

        [TestMethod]
        public void TestAddAddressPostWhenActiveMatchFound()
        {
            #region Arrange
            SetupDataForAddress();
            var address = CreateValidEntities.WorkgroupAddress(10);
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[0]))
                .Returns(0)
                .Throws(new Exception("Called  for address 0 too many times"));
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[1]))
                .Returns(0)
                .Throws(new Exception("Called  for address 1 too many times"));
            Mock.Get(WorkgroupAddressService)
                .SetupSequence(a => a.CompareAddress(address, WorkgroupRepository.GetNullableById(2).Addresses[2]))
                .Returns(6)
                .Throws(new Exception("Called  for address 2 too many times"));
            #endregion Arrange

            #region Act
            var result = Controller.AddAddress(2, address)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.RouteValues["id"]);
            Assert.AreEqual("This Address already exists.", Controller.Message);
            Mock.Get(WorkgroupRepository).Verify(a => a.EnsurePersistent(It.IsAny<Workgroup>()), Times.Never());

            Mock.Get(WorkgroupAddressService).Verify(a => a.CompareAddress(It.IsAny<WorkgroupAddress>(), It.IsAny<WorkgroupAddress>()), Times.Exactly(3));
            #endregion Assert
        }

        #endregion AddAddress Post Tests
    }
}
