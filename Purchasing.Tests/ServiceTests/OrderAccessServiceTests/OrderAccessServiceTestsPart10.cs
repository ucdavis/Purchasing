//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Purchasing.Core.Domain;
//using Purchasing.Tests.Core;
//using Rhino.Mocks;
//using UCDArch.Core.Utils;
//using UCDArch.Testing;

//namespace Purchasing.Tests.ServiceTests.OrderAccessServiceTests
//{
//    public partial class OrderAccessServiceTests
//    {
//        #region GetAdministrativeListofOrders Tests

//        [TestMethod]
//        public void TestGetAdministrativeListOfOrderReturnsEmptyList1()
//        {
//            #region Arrange
//            UserIdentity.Expect(a => a.Current).Return("noAdmin").Repeat.Any();
//            var user = new User("noAdmin");
//            user.WorkgroupPermissions = new List<WorkgroupPermission>();
//            user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
//            user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
//            user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
//            foreach (var workgroupPermission in user.WorkgroupPermissions)
//            {
//                workgroupPermission.Workgroup = CreateValidEntities.Workgroup(null);
//                workgroupPermission.Workgroup.Administrative = false;
//            }
//            SetupUsers1(user);
//            #endregion Arrange

//            #region Act
//            var results = OrderService.GetAdministrativeListofOrders();
//            #endregion Act

//            #region Assert
//            Assert.IsNotNull(results);
//            Assert.AreEqual(0, results.Count);
//            #endregion Assert		
//        }

//        [TestMethod]
//        [ExpectedException(typeof (PreconditionException))]
//        public void TestGetAdministrativeListofOrdersWithInfiniteRegression()
//        {
//            var thisFar = false;
//            try
//            {
//                #region Arrange
//                var organizations = new List<Organization>();
//                for (int i = 0; i < 3; i++)
//                {
//                    organizations.Add(CreateValidEntities.Organization(i+1));
//                    organizations[i].SetIdTo((i + 1).ToString());
//                }
//                organizations[0].Parent = organizations[2];
//                organizations[1].Parent = organizations[0];
//                organizations[2].Parent = organizations[1];
//                new FakeOrganizations(0, OrganizationRepository, organizations, true);

//                UserIdentity.Expect(a => a.Current).Return("Admin").Repeat.Any();
//                var user = new User("Admin");
//                user.WorkgroupPermissions = new List<WorkgroupPermission>();
//                user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(1));
//                user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(2));
//                user.WorkgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(3));
//                foreach(var workgroupPermission in user.WorkgroupPermissions)
//                {
//                    workgroupPermission.Workgroup = CreateValidEntities.Workgroup(null);
//                    workgroupPermission.Workgroup.Administrative = false;
//                }
//                user.WorkgroupPermissions[1].Workgroup.Administrative = true;
//                user.WorkgroupPermissions[1].Workgroup.Organizations.Add(OrganizationRepository.GetNullableById("2"));
//                SetupUsers1(user);

//                thisFar = true;
//                #endregion Arrange

//                #region Act
//                OrderService.GetAdministrativeListofOrders();
//                #endregion Act
//            }
//            catch (Exception ex)
//            {
//                Assert.IsTrue(thisFar);
//                Assert.IsNotNull(ex);
//                Assert.AreEqual("Possible infinite regression for Name3", ex.Message);
//                throw;
//            }
//        }

//        #endregion GetAdministrativeListofOrders Tests
//    }
//}
