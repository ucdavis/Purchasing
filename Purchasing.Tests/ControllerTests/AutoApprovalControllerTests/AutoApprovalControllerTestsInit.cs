using System;
using System.Collections.Generic;
using System.Linq;
using Purchasing.Tests.Core;
using Purchasing.Web;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Purchasing.Web.Controllers.Dev;
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

        protected override void RegisterRoutes()
        {
           new RouteConfigurator().RegisterRoutes();
            
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
            autoApprovals[2].Expiration = DateTime.Now.Date;
            autoApprovals[3].Expiration = DateTime.Now.Date.AddDays(1);
            autoApprovals[4].Expiration = DateTime.Now.Date.AddDays(-1);

            new FakeAutoApprovals(0, AutoApprovalRepository, autoApprovals);
        } 
        #endregion Helpers
    }
}
