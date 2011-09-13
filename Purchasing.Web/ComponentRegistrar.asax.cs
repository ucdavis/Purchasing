﻿using Castle.Windsor;
using Purchasing.Core;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Castle.MicroKernel.Registration;
using Purchasing.Web.Services;
using NHibernate;

namespace Purchasing.Web
{
    internal static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);

            container.Register(Component.For<IRepositoryFactory>().ImplementedBy<RepositoryFactory>().Named("repositoryFactory"));
            container.Register(Component.For<IEventService>().ImplementedBy<EventService>().Named("eventService"));
            container.Register(Component.For<IOrderService>().ImplementedBy<OrderService>().Named("orderService"));
            container.Register(Component.For<IDirectorySearchService>().ImplementedBy<DirectorySearchService>().Named("directorySearch"));
            container.Register(Component.For<IInterceptor>().ImplementedBy<AuditInterceptor>().Named("audit"));
            container.Register(Component.For<IDbService>().ImplementedBy<DbService>().Named("dbService"));
            container.Register(Component.For<IQueryExtensionProvider>().ImplementedBy<NHibernateQueryExtensionProvider>().Named("queryExtensions"));
            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("dbContext"));
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(Component.For(typeof(IRepositoryWithTypedId<,>)).ImplementedBy(typeof(RepositoryWithTypedId<,>)).Named("repositoryWithTypedId"));
            container.Register(Component.For(typeof(IRepository<>)).ImplementedBy(typeof(Repository<>)).Named("repositoryType"));
            container.Register(Component.For<IRepository>().ImplementedBy<Repository>().Named("repository"));
        }
    }
}