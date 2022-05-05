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
using AutoMapper;
using UCDArch.Core;
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
            OrganazationRepository = new Moq.Mock<IRepositoryWithTypedId<Organization, string>>().Object;
            SecurityService = new Moq.Mock<ISecurityService>().Object;

            Controller = new CustomFieldController(CustomFieldRepository,
                OrganazationRepository,
                SecurityService,
                SmartServiceLocator<IMapper>.GetService());
            //Controller = new CustomFieldController(CustomFieldRepository, ExampleService);
        }

        protected override void RegisterAdditionalServices(IWindsorContainer container)
        {
            container.Install(new AutoMapperInstaller());
            base.RegisterAdditionalServices(container);
        }

        public CustomFieldControllerTests()
        {
            //    ExampleRepository = FakeRepository<Example>();
            //    Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<Example>()).Returns(ExampleRepository);

            Moq.Mock.Get(Controller.Repository).Setup(a => a.OfType<CustomField>()).Returns(CustomFieldRepository);
        }
        #endregion Init






    }
}
