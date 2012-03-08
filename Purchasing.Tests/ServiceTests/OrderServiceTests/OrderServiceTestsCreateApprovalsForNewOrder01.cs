using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core;
using Purchasing.Core.Domain;
using Purchasing.Tests.Core;
using Purchasing.WS;
using Purchasing.Web.Services;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using Purchasing.Tests;
using UCDArch.Core.Utils;
using UCDArch.Testing;

namespace Purchasing.Tests.ServiceTests.OrderServiceTests
{
    public partial class OrderServiceTests
    {

        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestCreateApprovalsForNewOrderRequiresAccountIdOrManager()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var order = CreateValidEntities.Order(1);
                order.Splits = new List<Split>();
                order.Splits.Add(new Split());
                order.Splits[0].Account = "12345";
                thisFar = true;
                #endregion Arrange

                #region Act
                OrderService.CreateApprovalsForNewOrder(order, null, null, null, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("You must either supply the ID of a valid account or provide the userId for an account manager", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreateApprovalsForNewOrder1()
        {
            #region Arrange
            var order = CreateValidEntities.Order(1);
            order.Splits = new List<Split>();
            order.Splits.Add(new Split());
            order.Splits[0].Account = "12345";
            order.Workgroup.SetIdTo(1);
            new FakeWorkgroupAccounts(3, WorkgroupAccountRepository);
            new FakeAccounts(3, AccountRepository);
            #endregion Arrange

            #region Act
            OrderService.CreateApprovalsForNewOrder(order, null, "12345", null, null);
            #endregion Act

            #region Assert
            #endregion Assert		
        }
    }
}
