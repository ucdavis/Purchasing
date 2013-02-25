﻿using System.Configuration;
using Castle.Windsor;
using Purchasing.Core;
using Purchasing.WS;
using Purchasing.Web.Helpers;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Castle.MicroKernel.Registration;
using Purchasing.Web.Services;
using NHibernate;
using System.Security.Principal;
using System.Web;

namespace Purchasing.Web
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.Register(Component.For<IUserIdentity>().ImplementedBy<UserIdentity>().Named("userIdentity"));

            container.Register(Component.For<ISearchService>().ImplementedBy<IndexSearchService>().Named("searchService"));

            //Register the index service and pass along the current App_Data/Indexes path location if HttpContext is available
            //TODO: Maybe make it a singleton so we don't have to keep opening the indexreader
            container.Register(
                Component.For<IIndexService>().ImplementedBy<IndexService>().Named("indexService").OnCreate(
                    service =>
                        {
                            if (HttpContext.Current != null)
                                service.SetIndexRoot(HttpContext.Current.Server.MapPath("~/App_Data/Indexes"));
                        }
                    ));
            
            container.Register(Component.For<IRepositoryFactory>().ImplementedBy<RepositoryFactory>().Named("repositoryFactory"));
            container.Register(Component.For<IQueryRepositoryFactory>().ImplementedBy<QueryRepositoryFactory>().Named("queryRepositoryFactory"));
            container.Register(Component.For<IReportRepositoryFactory>().ImplementedBy<ReportRepositoryFactory>().Named("reportRepositoryFactory"));

            container.Register(Component.For<IEventService>().ImplementedBy<EventService>().Named("eventService"));
            
            container.Register(Component.For<INotificationService>().ImplementedBy<NotificationService>().Named("notificationService"));
            container.Register(Component.For<IServerLink>().ImplementedBy<ServerLink>().Named("serverLink"));
            
            container.Register(Component.For<IOrderService>().ImplementedBy<OrderService>().Named("orderService"));
            container.Register(Component.For<IDirectorySearchService>().ImplementedBy<DirectorySearchService>().Named("directorySearch"));
            container.Register(Component.For<IInterceptor>().ImplementedBy<AuditInterceptor>().Named("audit"));
            container.Register(Component.For<IDbService>().ImplementedBy<DbService>().Named("dbService"));
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<NHibernateQueryExtensionProvider>().Named("queryExtensions"));
            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("dbContext"));
            container.Register(Component.For<IWorkgroupAddressService>().ImplementedBy<WorkgroupAddressService>().Named("workgroupAddressService"));
            container.Register(Component.For<ISecurityService>().ImplementedBy<SecurityService>().Named("securityService"));
            container.Register(Component.For<IWorkgroupService>().ImplementedBy<WorkgroupService>().Named("workgroupService")); //Common methods for Workgroup and wizard controllers
            container.Register(Component.For<IReportService>().ImplementedBy<ReportService>().Named("reportService"));
            
            container.Register(Component.For<IFinancialSystemService>().ImplementedBy<FinancialSystemService>().Named("financialSystemService"));
            container.Register(Component.For<IUservoiceService>().ImplementedBy<UservoiceService>().Named("uservoiceService"));
            container.Register(Component.For<IBugTrackingService>().ImplementedBy<BugTrackingService>().Named("bugTrackingService"));

//#if DEBUG   
//            container.Register(Component.For<INotificationSender>().ImplementedBy<DevNotificationSender>().Named("notificationSender"));
//#else
//            container.Register(
//                Component.For<INotificationSender>().ImplementedBy<EmailNotificationSender>().Named("notificationSender")
//                    .OnCreate(service => service.SetAuthentication(ConfigurationManager.AppSettings["SendGridUserName"],
//                                                                   ConfigurationManager.AppSettings["SendGridPassword"])));
//#endif
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(Component.For(typeof(IRepositoryWithTypedId<,>)).ImplementedBy(typeof(RepositoryWithTypedId<,>)).Named("repositoryWithTypedId"));
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).Named("repositoryType"));
            container.Register(Component.For<IRepository>().ImplementedBy<Repository>().Named("repository"));
        }
    }
}