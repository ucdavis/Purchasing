using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.MappingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.Web.Controllers;
using Purchasing.Web.Models;
using Rhino.Mocks;
using UCDArch.Testing;


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
                .AssertActionRedirect()
                .ToAction<WorkgroupController>(a => a.Index());
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

        #region AddAddress Tests
         
        #endregion AddAddress Tests
    }
}
