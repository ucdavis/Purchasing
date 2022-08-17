using Castle.Windsor;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using UCDArch.Core;

namespace UCDArch.Testing
{
    public abstract class ControllerTestBase<CT> where CT : SuperController
    {
        protected CT Controller { get; set; }

        protected ControllerTestBase()
        {
            InitServiceLocator();

            SetupController();

            Controller.Repository = Mock.Of<IRepository>();
            Controller.ControllerContext = new ControllerContext()
            {
                HttpContext = SmartServiceLocator<HttpContext>.GetService(),
                RouteData = new RouteData()
            };
            Controller.TempData = new TempDataDictionary(Controller.HttpContext, Mock.Of<ITempDataProvider>());

        }

        private void InitServiceLocator()
        {
            var container = ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }

        /// <summary>
        /// Instead of overriding InitServiceLocator, you can jump in here to register additional services for testing
        /// </summary>
        protected virtual void RegisterAdditionalServices(IWindsorContainer container)
        {

        }

        /// <summary>
        /// Setup your controller using something like Controller = new YourController(params);
        /// </summary>
        protected abstract void SetupController();

        protected IRepository<T> FakeRepository<T>() where T : ValidatableObject
        {
            return Mock.Of<IRepository<T>>();
        }
    }
}