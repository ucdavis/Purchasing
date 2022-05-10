using Castle.Windsor;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;
using Moq;

namespace UCDArch.Testing
{
    public abstract class ControllerTestBase<CT> where CT : SuperController
    {
        protected CT Controller { get; set; }

        protected ControllerTestBase()
        {
            InitServiceLocator();

            SetupController();

            Controller.Repository = new Mock<IRepository>().Object;
        }

        protected virtual void InitServiceLocator()
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
            return new Mock<IRepository<T>>().Object;
        }
    }
}