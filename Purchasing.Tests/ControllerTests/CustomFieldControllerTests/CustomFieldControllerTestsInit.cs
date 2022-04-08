using System;
using System.Linq;
using Castle.Windsor;
using Purchasing.Mvc;
using Purchasing.Mvc.Controllers;
using Purchasing.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Purchasing.Mvc.Helpers;
using Purchasing.Mvc.Services;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using UCDArch.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
//using Purchasing.Controllers.Filters;
//using Purchasing.Services;


namespace Purchasing.Tests.ControllerTests.CustomFieldControllerTests
{
    [TestClass]
    public partial class CustomFieldControllerTests : ControllerTestBase<CustomFieldController>
    {
        protected readonly Type ControllerClass = typeof(CustomFieldController);
        protected IRepository<CustomField> CustomFieldRepository;
        protected IRepositoryWithTypedId<Organization, string> OrganazationRepository;
        protected ISecurityService SecurityService;


        #region Init
        /// <summary>
        /// Setups the controller.
        /// </summary>
        protected override void SetupController()
        {
            CustomFieldRepository = FakeRepository<CustomField>();
            OrganazationRepository = MockRepository.GenerateStub<IRepositoryWithTypedId<Organization, string>>();
            SecurityService = MockRepository.GenerateStub<ISecurityService>();

            Controller = new TestControllerBuilder().CreateController<CustomFieldController>(CustomFieldRepository,
                OrganazationRepository,
                SecurityService);
            //Controller = new TestControllerBuilder().CreateController<CustomFieldController>(CustomFieldRepository, ExampleService);
        }

        protected override void RegisterRoutes()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            AutomapperConfig.Configure();
            base.RegisterAdditionalServices(container);
        }

        public CustomFieldControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Controller.Repository.Expect(a => a.OfType<Example>()).Return(ExampleRepository).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<CustomField>()).Return(CustomFieldRepository).Repeat.Any();	
        }
        #endregion Init






    }
}
