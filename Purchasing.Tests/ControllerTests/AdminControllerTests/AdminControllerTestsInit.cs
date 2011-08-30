using System;
using System.Linq;
using System.Web.Mvc;
using Purchasing.Core.Domain;
using Purchasing.Web;
using Purchasing.Web.Controllers;
using Purchasing.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;

namespace Purchasing.Tests.ControllerTests.AdminControllerTests
{
    [TestClass]
    public partial class AdminControllerTests : ControllerTestBase<AdminController>
    {
        protected readonly Type ControllerClass = typeof(AdminController);
        public IRepositoryWithTypedId<User, string> UserRepository;
        public IRepositoryWithTypedId<Role, string> RoleRepository;
        public IRepositoryWithTypedId<Organization, string> OrganizationRepository; 
        //public IRepository<Admin> AdminRepository;
        //public IExampleService ExampleService;
        //public IRepository<Example> ExampleRepository;

        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            UserRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<User, string>>();
            RoleRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Role, string>>();
            OrganizationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();

            //ExampleService = MockRepository.GenerateStub<IExampleService>();  
            //Controller = new TestControllerBuilder().CreateController<AdminController>(AdminRepository);
            Controller = new TestControllerBuilder().CreateController<AdminController>(UserRepository, RoleRepository, OrganizationRepository);
        }

        protected override void RegisterRoutes()
        {
            new RouteConfigurator().RegisterRoutes(); //Try this one if below doesn't work
            //RouteRegistrar.RegisterRoutes(RouteTable.Routes);
        }

        public AdminControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            //Controller.Repository.Expect(a => a.OfType<Admin>()).Return(AdminRepository).Repeat.Any();	
            
        }
        #endregion Init
    }
}
