using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator;
using UCDArch.Core.CommonValidator;
//using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;

namespace UCDArch.Testing
{
    public class ServiceLocatorInitializer
    {
        public static IWindsorContainer Init()
        {
            IWindsorContainer container = new WindsorContainer();

            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            Mock.Get(httpContextAccessor).Setup(a => a.HttpContext).Returns(new DefaultHttpContext());
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            Mock.Get(tempDataProvider).Setup(a => a.LoadTempData(It.IsAny<HttpContext>())).Returns(new Dictionary<string, object>());
            var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            var urlHelperFactory = new UrlHelperFactory();

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("DbContext"));
            container.Register(Component.For<IHttpContextAccessor>().Instance(httpContextAccessor).Named("httpContextAccessor"));
            container.Register(Component.For<ITempDataDictionaryFactory>().Instance(tempDataDictionaryFactory).Named("tempDataDictionaryFactory"));
            container.Register(Component.For<IUrlHelperFactory>().Instance(urlHelperFactory).Named("urlHelperFactory"));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }

        public static IWindsorContainer InitWithFakeDBContext()
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));

            var dbContext = Mock.Of<IDbContext>();

            container.Register(Component.For<IDbContext>().Instance(dbContext).Named("DbContext"));
            
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }
    }
}