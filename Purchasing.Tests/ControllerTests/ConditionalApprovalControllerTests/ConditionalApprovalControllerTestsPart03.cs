using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Tests.Core;
using Purchasing.Mvc.Controllers;
using Purchasing.Mvc.Services;
using UCDArch.Core.Utils;
using UCDArch.Testing.Fakes;
using UCDArch.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Purchasing.Tests.ControllerTests.ConditionalApprovalControllerTests
{
    public partial class ConditionalApprovalControllerTests
    {
        #region Create Get Tests

        [TestMethod]
        [ExpectedException(typeof (PreconditionException))]
        public void TestCreateWithNoParameteres1()
        {
            var thisFar = false;
            try
            {
                #region Arrange

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(null, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Missing Parameters", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreateWithNoParameteres2()
        {
            var thisFar = false;
            try
            {
                #region Arrange

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(null, string.Empty);
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Missing Parameters", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void TestCreateWithNoParameteres3()
        {
            var thisFar = false;
            try
            {
                #region Arrange

                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(null, " ");
                #endregion Act
            }
            catch(Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Missing Parameters", ex.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCreateWhenWorkgroupNotFound()
        {
            var thisFar = false;
            try
            {
                #region Arrange
                var workgroups = new List<Workgroup>();
                workgroups.Add(CreateValidEntities.Workgroup(1));
                workgroups.Add(CreateValidEntities.Workgroup(2));
                workgroups[0].Administrative = true;
                new FakeWorkgroups(0, WorkgroupRepository, workgroups);
                thisFar = true;
                #endregion Arrange

                #region Act
                Controller.Create(3, null);
                #endregion Act
            }
            catch (Exception ex)
            {
                Assert.IsTrue(thisFar);
                Assert.IsNotNull(ex);
                Assert.AreEqual("Sequence contains no matching element", ex.Message);
                throw;
            }
        }


        [TestMethod]
        public void TestCreateWhenWorkgroupIsAdminsitrativeRedirects()
        {
            #region Arrange
            var workgroups = new List<Workgroup>();
            workgroups.Add(CreateValidEntities.Workgroup(1));
            workgroups.Add(CreateValidEntities.Workgroup(2));
            workgroups[0].Administrative = true;
            new FakeWorkgroups(0, WorkgroupRepository, workgroups);

            #endregion Arrange

            #region Act
            var results = Controller.Create(1, null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.RouteValues["id"]);
            Assert.AreEqual("Conditional Approval may not be added to an administrative workgroup.", Controller.ErrorMessage);
            #endregion Assert		
        }



        [TestMethod]
        public void TestCreateGetRedirectsWhenWorkgroupIfNoWorkgroups()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "4");
            SetupDateForIndex1();
            new FakeAdminWorkgroups(1, QueryRepositoryFactory.AdminWorkgroupRepository);
            const string approvalType = "Workgroup";
            #endregion Arrange

            #region Act
            Controller.Create(1, null)
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Format("You cannot create a conditional approval for type {0} because you are not associated with any {0}s.", approvalType), Controller.ErrorMessage);
            #endregion Assert		
        }

        [TestMethod]
        public void TestCreateGetRedirectsWhenOrganizationIfNoOrganizations()
        {
            #region Arrange
            Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "4");
            SetupDateForIndex1();
            const string approvalType = "Organization";
            #endregion Arrange

            #region Act
            Controller.Create(null, "test")
                .AssertActionRedirect();
            #endregion Act

            #region Assert
            Assert.AreEqual(string.Format("You cannot create a conditional approval for type {0} because you are not associated with any {0}s.", approvalType), Controller.ErrorMessage);
            #endregion Assert
        }



        //[TestMethod]
        //public void TestCreateGetReturnsView2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    const string approvalType = "Organization";
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(approvalType)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(approvalType, result.ApprovalType);
        //    Assert.AreEqual(4, result.Organizations.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Workgroups);
        //    Assert.IsNull(result.PrimaryApprover);
        //    Assert.IsNull(result.Question);
        //    Assert.IsNull(result.SecondaryApprover);
        //    Assert.IsNull(result.Workgroup);
        //    #endregion Assert
        //}
        //#endregion Create Get Tests

        //#region Create Post Tests

        //[TestMethod]
        //public void TestCreatePostReturnsViewWhenModelInvalid1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    Controller.ModelState.AddModelError("Fake", "Fake Error");
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "2";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("Fake Error");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Workgroup", result.ApprovalType);
        //    Assert.AreEqual(3, result.Workgroups.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Organizations);
        //    Assert.AreEqual("2", result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.IsNull(result.SecondaryApprover);
        //    Assert.IsNull(result.Workgroup);
        //    DirectorySearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestCreatePostReturnsViewWhenModelInvalid2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    Controller.ModelState.AddModelError("Fake", "Fake Error");
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "2";
        //    conditionalApprovalModifyModel.SecondaryApprover = "1";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("Fake Error");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Workgroup", result.ApprovalType);
        //    Assert.AreEqual(3, result.Workgroups.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Organizations);
        //    Assert.AreEqual("2", result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.AreEqual("1", result.SecondaryApprover);
        //    Assert.IsNull(result.Workgroup);
        //    DirectorySearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreatePostReturnsViewWhenModelInvalid3()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    Controller.ModelState.AddModelError("Fake", "Fake Error");
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Organization";
        //    conditionalApprovalModifyModel.PrimaryApprover = "2";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("Fake Error");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Organization", result.ApprovalType);
        //    Assert.AreEqual(4, result.Organizations.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Workgroups);
        //    Assert.AreEqual("2", result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.IsNull(result.SecondaryApprover);
        //    Assert.IsNull(result.Workgroup);
        //    DirectorySearchService.AssertWasNotCalled(a => a.FindUser(Arg<string>.Is.Anything));
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    #endregion Assert
        //}

        //[TestMethod]
        //[ExpectedException(typeof (PreconditionException))]
        //public void TestCreatePostThrowsException()
        //{
        //    var thisFar = false;
        //    try
        //    {
        //        #region Arrange
        //        Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //        SetupDateForIndex1();
        //        var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //        conditionalApprovalModifyModel.ApprovalType = "Organization";
        //        conditionalApprovalModifyModel.PrimaryApprover = "2";
        //        conditionalApprovalModifyModel.Question = "Que?";
        //        thisFar = true;
        //        #endregion Arrange

        //        #region Act
        //        Controller.Create(conditionalApprovalModifyModel);
        //        #endregion Act
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsTrue(thisFar);
        //        Assert.IsNotNull(ex);
        //        Assert.AreEqual("Must have a Workgroup or an Organization", ex.Message);
        //        throw;
        //    }
        //}


        //[TestMethod]
        //public void TestCreatePostCallsFindUser1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "99";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    conditionalApprovalModifyModel.Workgroup = WorkgroupRepository.GetNullableById(1);

        //    DirectorySearchService.Expect(a => a.FindUser("99")).Return(null);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("No user could be found with the kerberos or email address entered");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Workgroup", result.ApprovalType);
        //    Assert.AreEqual(3, result.Workgroups.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Organizations);
        //    Assert.AreEqual("99", result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.AreEqual(null, result.SecondaryApprover);
        //    Assert.AreEqual("WName1", result.Workgroup.Name);
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));

        //    DirectorySearchService.AssertWasCalled(a => a.FindUser("99"));
        //    #endregion Assert		
        //}

        //[TestMethod]
        //public void TestCreatePostCallsFindUser2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "99";
        //    conditionalApprovalModifyModel.SecondaryApprover = "88";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    conditionalApprovalModifyModel.Workgroup = WorkgroupRepository.GetNullableById(1);

        //    DirectorySearchService.Expect(a => a.FindUser("99")).Return(null);
        //    DirectorySearchService.Expect(a => a.FindUser("88")).Return(null);
        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("No user could be found with the kerberos or email address entered", "No user could be found with the kerberos or email address entered");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Workgroup", result.ApprovalType);
        //    Assert.AreEqual(3, result.Workgroups.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Organizations);
        //    Assert.AreEqual("99", result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.AreEqual("88", result.SecondaryApprover);
        //    Assert.AreEqual("WName1", result.Workgroup.Name);
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));

        //    DirectorySearchService.AssertWasCalled(a => a.FindUser("99"));
        //    DirectorySearchService.AssertWasCalled(a => a.FindUser("88"));
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreatePostCallsFindUser3()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = null;
        //    conditionalApprovalModifyModel.SecondaryApprover = " ";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    conditionalApprovalModifyModel.Workgroup = WorkgroupRepository.GetNullableById(1);

        //    DirectorySearchService.Expect(a => a.FindUser(null)).Return(null);

        //    #endregion Arrange

        //    #region Act
        //    var result = Controller.Create(conditionalApprovalModifyModel)
        //        .AssertViewRendered()
        //        .WithViewData<ConditionalApprovalModifyModel>();
        //    #endregion Act

        //    #region Assert
        //    Controller.ModelState.AssertErrorsAre("No user could be found with the kerberos or email address entered");
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Workgroup", result.ApprovalType);
        //    Assert.AreEqual(3, result.Workgroups.Count);
        //    Assert.IsNull(result.Organization);
        //    Assert.IsNull(result.Organizations);
        //    Assert.AreEqual(null, result.PrimaryApprover);
        //    Assert.AreEqual("Que?", result.Question);
        //    Assert.AreEqual(" ", result.SecondaryApprover);
        //    Assert.AreEqual("WName1", result.Workgroup.Name);
        //    ConditionalApprovalRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    UserRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything));

        //    DirectorySearchService.AssertWasCalled(a => a.FindUser(null));
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreatePostCreateNewUser1()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "99";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    conditionalApprovalModifyModel.Workgroup = WorkgroupRepository.GetNullableById(1);
        //    var directoryUser = new DirectoryUser();
        //    directoryUser.EmailAddress = "99@test.com";
        //    directoryUser.LastName = "LastName99";
        //    directoryUser.FirstName = "FirstName99";
        //    directoryUser.LoginId = "99";
        //    DirectorySearchService.Expect(a => a.FindUser("99")).Return(directoryUser);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Create(conditionalApprovalModifyModel)
        //        .AssertActionRedirect()
        //        .ToAction<ConditionalApprovalController>(a => a.Index());
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("Conditional approval added successfully", Controller.Message);
        //    UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything, Arg<bool>.Is.Anything));
        //    var userArgs = (User) UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything, Arg<bool>.Is.Anything))[0][0]; 
        //    Assert.IsNotNull(userArgs);
        //    Assert.IsTrue(userArgs.IsActive);
        //    Assert.AreEqual("99", userArgs.Id);
        //    Assert.AreEqual("LastName99", userArgs.LastName);
        //    Assert.AreEqual("FirstName99", userArgs.FirstName);
        //    Assert.AreEqual("99@test.com", userArgs.Email);

        //    ConditionalApprovalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    var args = (ConditionalApproval) ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything))[0][0]; 
        //    Assert.IsNotNull(args);
        //    Assert.AreEqual("WName1", args.Workgroup.Name);
        //    Assert.AreEqual(null, args.Organization);
        //    Assert.AreEqual("99", args.PrimaryApprover.Id);
        //    Assert.AreEqual(null, args.SecondaryApprover);
        //    Assert.AreEqual("Que?", args.Question);
        //    #endregion Assert
        //}

        //[TestMethod]
        //public void TestCreatePostCreateNewUser2()
        //{
        //    #region Arrange
        //    Controller.ControllerContext.HttpContext = new MockHttpContext(0, new[] { "" }, "2");
        //    SetupDateForIndex1();
        //    var conditionalApprovalModifyModel = new ConditionalApprovalModifyModel();
        //    conditionalApprovalModifyModel.ApprovalType = "Workgroup";
        //    conditionalApprovalModifyModel.PrimaryApprover = "99";
        //    conditionalApprovalModifyModel.SecondaryApprover = "88";
        //    conditionalApprovalModifyModel.Question = "Que?";
        //    conditionalApprovalModifyModel.Workgroup = WorkgroupRepository.GetNullableById(1);
        //    var directoryUser1 = new DirectoryUser();
        //    directoryUser1.EmailAddress = "99@test.com";
        //    directoryUser1.LastName = "LastName99";
        //    directoryUser1.FirstName = "FirstName99";
        //    directoryUser1.LoginId = "99";
        //    var directoryUser2 = new DirectoryUser();
        //    directoryUser2.EmailAddress = "88@test.com";
        //    directoryUser2.LastName = "LastName88";
        //    directoryUser2.FirstName = "FirstName88";
        //    directoryUser2.LoginId = "88";
        //    DirectorySearchService.Expect(a => a.FindUser("99")).Return(directoryUser1);
        //    DirectorySearchService.Expect(a => a.FindUser("88")).Return(directoryUser2);
        //    #endregion Arrange

        //    #region Act
        //    Controller.Create(conditionalApprovalModifyModel)
        //        .AssertActionRedirect()
        //        .ToAction<ConditionalApprovalController>(a => a.Index());
        //    #endregion Act

        //    #region Assert
        //    Assert.AreEqual("Conditional approval added successfully", Controller.Message);
        //    UserRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<User>.Is.Anything, Arg<bool>.Is.Anything), x => x.Repeat.Times(2));
        //    var userArgs1 = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything, Arg<bool>.Is.Anything))[0][0];
        //    Assert.IsNotNull(userArgs1);
        //    Assert.IsTrue(userArgs1.IsActive);
        //    Assert.AreEqual("99", userArgs1.Id);
        //    Assert.AreEqual("LastName99", userArgs1.LastName);
        //    Assert.AreEqual("FirstName99", userArgs1.FirstName);
        //    Assert.AreEqual("99@test.com", userArgs1.Email);

        //    var userArgs2 = (User)UserRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<User>.Is.Anything, Arg<bool>.Is.Anything))[1][0];
        //    Assert.IsNotNull(userArgs2);
        //    Assert.IsTrue(userArgs2.IsActive);
        //    Assert.AreEqual("88", userArgs2.Id);
        //    Assert.AreEqual("LastName88", userArgs2.LastName);
        //    Assert.AreEqual("FirstName88", userArgs2.FirstName);
        //    Assert.AreEqual("88@test.com", userArgs2.Email);

        //    ConditionalApprovalRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything));
        //    var args1 = (ConditionalApproval)ConditionalApprovalRepository.GetArgumentsForCallsMadeOn(a => a.EnsurePersistent(Arg<ConditionalApproval>.Is.Anything))[0][0];
        //    Assert.IsNotNull(args1);
        //    Assert.AreEqual("WName1", args1.Workgroup.Name);
        //    Assert.AreEqual(null, args1.Organization);
        //    Assert.AreEqual("99", args1.PrimaryApprover.Id);
        //    Assert.AreEqual("88", args1.SecondaryApprover.Id);
        //    Assert.AreEqual("Que?", args1.Question);

        //    #endregion Assert
        //}
        #endregion Create Post Tests
    }
}
