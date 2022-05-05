using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Purchasing.Core.Helpers;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Controllers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UCDArch.Core;

namespace Purchasing.Tests.ControllerTests.AutoApprovalControllerTests
{
    [TestClass]
    public partial class AutoApprovalControllerTests : ControllerTestBase<AutoApprovalController>
    {
        public IRepositoryWithTypedId<User, string> UserRepository;
        private readonly Type _controllerClass = typeof(AutoApprovalController);
        public IRepository<AutoApproval> AutoApprovalRepository;
        public IRepository<WorkgroupPermission> WorkgroupPermissionRepository;
        public IRepository<WorkgroupAccount> WorkgoupAccountRepository; 
        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            AutoApprovalRepository = FakeRepository<AutoApproval>();
            UserRepository = new Moq.Mock<IRepositoryWithTypedId<User, string>>().Object;

            //ExampleService = new Moq.Mock<IExampleService>().Object;  
            Controller = new AutoApprovalController(AutoApprovalRepository, UserRepository, SmartServiceLocator<IMapper>.GetService());
            //Controller = new AutoApprovalController(AutoApprovalRepository, ExampleService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            base.RegisterAdditionalServices(container);
        }

        public AutoApprovalControllerTests()
        {
            WorkgoupAccountRepository = FakeRepository<WorkgroupAccount>();
            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<WorkgroupAccount>()).Returns(WorkgoupAccountRepository);

            WorkgroupPermissionRepository = FakeRepository<WorkgroupPermission>();
            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<WorkgroupPermission>()).Returns(WorkgroupPermissionRepository);

            //Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<AutoApproval>()).Returns(AutoApprovalRepository);

        }
        #endregion Init

        #region Helpers
        public void SetupData1()
        {
            var autoApprovals = new List<AutoApproval>();
            for(int i = 0; i < 6; i++)
            {
                autoApprovals.Add(CreateValidEntities.AutoApproval(i + 1));
                if(i % 2 == 0)
                {
                    autoApprovals[i].TargetUser = null;
                    autoApprovals[i].Account = CreateValidEntities.Account(i);
                }
                autoApprovals[i].User.Id = "Me";
            }

            autoApprovals[0].User.Id = "NotMe";
            autoApprovals[1].IsActive = false;
            autoApprovals[2].Expiration = DateTime.UtcNow.ToPacificTime().Date;
            autoApprovals[3].Expiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(1);
            autoApprovals[4].Expiration = DateTime.UtcNow.ToPacificTime().Date.AddDays(-1);

            new FakeAutoApprovals(0, AutoApprovalRepository, autoApprovals);
        }
        public void SetupData2()
        {
            var innerPermissions = new List<WorkgroupPermission>();
            for(int i = 0; i < 5; i++)
            {
                innerPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 100));
                innerPermissions[i].User.Id = "Someone" + i;
                innerPermissions[i].User.IsActive = true;
                innerPermissions[i].Role.Id = Role.Codes.Requester;
            }
            innerPermissions[0].User.IsActive = false;
            innerPermissions[1].Role.Id = Role.Codes.Purchaser;

            var workgroupPermissions = new List<WorkgroupPermission>();
            for(int i = 0; i < 5; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                workgroupPermissions[i].Role.Id = Role.Codes.Approver;
                workgroupPermissions[i].User.Id = "Me";
                workgroupPermissions[i].Workgroup.Permissions = innerPermissions;
            }
            workgroupPermissions[0].User.Id = "NotMe";
            workgroupPermissions[1].Role.Id = Role.Codes.Purchaser;
            workgroupPermissions[2].Workgroup.Permissions = new List<WorkgroupPermission>();
            workgroupPermissions[2].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(55));
            workgroupPermissions[2].Workgroup.Permissions[0].User.Id = "Someone" + 55;
            workgroupPermissions[2].Workgroup.Permissions[0].User.IsActive = true;
            workgroupPermissions[2].Workgroup.Permissions[0].Role.Id = Role.Codes.Requester;
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var workgroupAccounts = new List<WorkgroupAccount>();
            for(int i = 0; i < 10; i++)
            {
                workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupAccounts[i].Approver.Id = "Me";
                workgroupAccounts[i].Account.Id = "AcctId" + i;
                workgroupAccounts[i].Account.Name = "AccountName" + i;
                workgroupAccounts[i].Account.IsActive = true;
            }
            workgroupAccounts[0].Approver.Id = "NotMe";
            workgroupAccounts[1].Account = workgroupAccounts[2].Account;
            workgroupAccounts[4].Account.IsActive = false;
            new FakeWorkgroupAccounts(0, WorkgoupAccountRepository, workgroupAccounts);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(5));
            users[0].Id = "Me";
            new FakeUsers(0, UserRepository, users, true);

        }

        public void SetupData3()
        {
            var autoApprovals = new List<AutoApproval>();
            for(int i = 0; i < 3; i++)
            {
                autoApprovals.Add(CreateValidEntities.AutoApproval(i + 1));
                autoApprovals[i].User = CreateValidEntities.User(1);
                autoApprovals[i].User.Id = "Me";
            }
            autoApprovals[0].User.Id = "NotMe";
            autoApprovals[0].IsActive = false;
            new FakeAutoApprovals(0, AutoApprovalRepository, autoApprovals);
        }
        #endregion Helpers
    }
}
