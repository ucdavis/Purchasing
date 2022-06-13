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
using System.Security.Claims;
using System.Security.Principal;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace UCDArch.Testing
{
    public static class ServiceLocatorInitializer
    {
        public static IWindsorContainer Init()
        {
            // var items = new Dictionary<object, object>();
            // var mockItems = Mock.Of<IDictionary<object, object>>();
            // Mock.Get(mockItems).SetupSet(m => m[It.IsAny<object>()] = It.IsAny<object>())
            //     .Callback((object key, object value) => { items[key] = value; });
            // Mock.Get(mockItems).Setup(m => m[It.IsAny<object>()])
            //     .Returns((object key) => key switch {
            //         NHibernateSessionManager.SESSION_KEY => NHibernateSessionManager.Instance.GetSession(),
            //         _ => items[key]
            //     } );

            var httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).SetupGet(m => m.Features).Returns(new FeatureCollection());
            Mock.Get(httpContext).SetupGet(m => m.Items).Returns(new Dictionary<object, object>());


            var httpContextAccessor = Mock.Of<IHttpContextAccessor>();
            Mock.Get(httpContextAccessor).Setup(a => a.HttpContext).Returns(httpContext);
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            Mock.Get(tempDataProvider).Setup(a => a.LoadTempData(It.IsAny<HttpContext>())).Returns(new Dictionary<string, object>());
            var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
            var urlHelperFactory = new UrlHelperFactory();

            var container = new WindsorContainer();
            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("DbContext"));
            container.Register(Component.For<IHttpContextAccessor>().Instance(httpContextAccessor).Named("httpContextAccessor"));
            container.Register(Component.For<ITempDataDictionaryFactory>().Instance(tempDataDictionaryFactory).Named("tempDataDictionaryFactory"));
            container.Register(Component.For<IUrlHelperFactory>().Instance(urlHelperFactory).Named("urlHelperFactory"));
            container.Register(Component.For<HttpContext>().Instance(httpContext).Named("httpContext"));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }

        public static void Setup(this HttpContext httpContext, string[] userRoles, string userName = "UserName", string fileContentType = "application/pdf")
        {
            var identity = Mock.Of<IIdentity>();
            Mock.Get(identity).SetupGet(m => m.AuthenticationType).Returns("MockAuthentication");
            Mock.Get(identity).SetupGet(m => m.IsAuthenticated).Returns(true);
            Mock.Get(identity).SetupGet(m => m.Name).Returns(userName);

            var claimsPrincipal = Mock.Of<ClaimsPrincipal>();
            Mock.Get(claimsPrincipal).SetupGet(m => m.Identity).Returns(identity);
            Mock.Get(claimsPrincipal).Setup(m => m.IsInRole(It.IsAny<string>()))
                .Callback((string role) => userRoles.Contains(role));

            var httpRequest = Mock.Of<HttpRequest>();
            Mock.Get(httpRequest).Setup(a => a.HttpContext).Returns(httpContext);
            Mock.Get(httpRequest).Setup(a => a.Path).Returns(new PathString("/"));
            Mock.Get(httpRequest).Setup(a => a.PathBase).Returns(new PathString("/"));
            Mock.Get(httpRequest).Setup(a => a.Scheme).Returns("http");
            Mock.Get(httpRequest).Setup(a => a.RouteValues).Returns(new RouteValueDictionary());
            

            var serviceProvider = Mock.Of<IServiceProvider>();
            Mock.Get(serviceProvider).Setup(x => x.GetService(It.IsAny<Type>())).Returns<Type>(a => ServiceLocator.Current.GetService(a));

            var mockHttpContext = Mock.Get(httpContext);
            mockHttpContext.SetupGet(m => m.User).Returns(claimsPrincipal);
            mockHttpContext.SetupGet(m => m.Request).Returns(httpRequest);
            mockHttpContext.SetupGet(m => m.RequestServices).Returns(serviceProvider);
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