using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Castle.Windsor;
using Purchasing.Tests.Core;
using Purchasing.Mvc;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Mvc.Controllers;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

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
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();

            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            Controller = new TestControllerBuilder().CreateController<AutoApprovalController>(AutoApprovalRepository, UserRepository);
            //Controller = new TestControllerBuilder().CreateController<AutoApprovalController>(AutoApprovalRepository, ExampleService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        protected override void RegisterRoutes()
        {
           RouteConfig.RegisterRoutes(RouteTable.Routes);
            
        }

        public AutoApprovalControllerTests()
        {
            WorkgoupAccountRepository = FakeRepository<WorkgroupAccount>();
            Controller.Repository.Expect(a => a.OfType<WorkgroupAccount>()).Return(WorkgoupAccountRepository).Repeat.Any();

            WorkgroupPermissionRepository = FakeRepository<WorkgroupPermission>();
            Controller.Repository.Expect(a => a.OfType<WorkgroupPermission>()).Return(WorkgroupPermissionRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<AutoApproval>()).Return(AutoApprovalRepository).Repeat.Any();

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
                autoApprovals[i].User.SetIdTo("Me");
            }

            autoApprovals[0].User.SetIdTo("NotMe");
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
                innerPermissions[i].User.SetIdTo("Someone" + i);
                innerPermissions[i].User.IsActive = true;
                innerPermissions[i].Role.SetIdTo(Role.Codes.Requester);
            }
            innerPermissions[0].User.IsActive = false;
            innerPermissions[1].Role.SetIdTo(Role.Codes.Purchaser);

            var workgroupPermissions = new List<WorkgroupPermission>();
            for(int i = 0; i < 5; i++)
            {
                workgroupPermissions.Add(CreateValidEntities.WorkgroupPermission(i + 1));
                workgroupPermissions[i].Role.SetIdTo(Role.Codes.Approver);
                workgroupPermissions[i].User.SetIdTo("Me");
                workgroupPermissions[i].Workgroup.Permissions = innerPermissions;
            }
            workgroupPermissions[0].User.SetIdTo("NotMe");
            workgroupPermissions[1].Role.SetIdTo(Role.Codes.Purchaser);
            workgroupPermissions[2].Workgroup.Permissions = new List<WorkgroupPermission>();
            workgroupPermissions[2].Workgroup.Permissions.Add(CreateValidEntities.WorkgroupPermission(55));
            workgroupPermissions[2].Workgroup.Permissions[0].User.SetIdTo("Someone" + 55);
            workgroupPermissions[2].Workgroup.Permissions[0].User.IsActive = true;
            workgroupPermissions[2].Workgroup.Permissions[0].Role.SetIdTo(Role.Codes.Requester);
            new FakeWorkgroupPermissions(0, WorkgroupPermissionRepository, workgroupPermissions);

            var workgroupAccounts = new List<WorkgroupAccount>();
            for(int i = 0; i < 10; i++)
            {
                workgroupAccounts.Add(CreateValidEntities.WorkgroupAccount(i + 1));
                workgroupAccounts[i].Approver.SetIdTo("Me");
                workgroupAccounts[i].Account.SetIdTo("AcctId" + i);
                workgroupAccounts[i].Account.Name = "AccountName" + i;
                workgroupAccounts[i].Account.IsActive = true;
            }
            workgroupAccounts[0].Approver.SetIdTo("NotMe");
            workgroupAccounts[1].Account = workgroupAccounts[2].Account;
            workgroupAccounts[4].Account.IsActive = false;
            new FakeWorkgroupAccounts(0, WorkgoupAccountRepository, workgroupAccounts);

            var users = new List<User>();
            users.Add(CreateValidEntities.User(5));
            users[0].SetIdTo("Me");
            new FakeUsers(0, UserRepository, users, true);

        }

        public void SetupData3()
        {
            var autoApprovals = new List<AutoApproval>();
            for(int i = 0; i < 3; i++)
            {
                autoApprovals.Add(CreateValidEntities.AutoApproval(i + 1));
                autoApprovals[i].User = CreateValidEntities.User(1);
                autoApprovals[i].User.SetIdTo("Me");
            }
            autoApprovals[0].User.SetIdTo("NotMe");
            autoApprovals[0].IsActive = false;
            new FakeAutoApprovals(0, AutoApprovalRepository, autoApprovals);
        }
        #endregion Helpers
    }
}
