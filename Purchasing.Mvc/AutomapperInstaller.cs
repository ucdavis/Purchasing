using System.Linq;
using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Purchasing.Mvc
{
    public class AutoMapperInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register all mapper profiles
            container.Register(
                Classes.FromAssemblyInThisApplication(GetType().Assembly)
                .BasedOn<Profile>().WithServiceBase());
                
            // Register IConfigurationProvider with all registered profiles
            container.Register(Component.For<IConfigurationProvider>().UsingFactoryMethod(kernel =>
            {
                return new MapperConfiguration(configuration =>
                {
                    kernel.ResolveAll<Profile>().ToList().ForEach(configuration.AddProfile);
                });
            }).LifestyleSingleton());
            
            // Register IMapper with registered IConfigurationProvider
            container.Register(
                Component.For<IMapper>().UsingFactoryMethod(kernel =>
                    new Mapper(kernel.Resolve<IConfigurationProvider>(), kernel.Resolve)));
        }
    }


    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            //Create maps
            CreateMap<User, User>();
            CreateMap<Organization, Organization>();
            CreateMap<Role, Role>();
            CreateMap<Workgroup, Workgroup>()
                .ForMember(a => a.Vendors, a => a.Ignore())
                .ForMember(a => a.PrimaryOrganization, a => a.Ignore());
            CreateMap<AutoApproval, AutoApproval>()
                .ForMember(x => x.User, x=> x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<WorkgroupAddress, WorkgroupAddress>()
                .ForMember(a=> a.Id, a=> a.Ignore())
                .ForMember(a=> a.Workgroup, a=> a.Ignore());

            CreateMap<WorkgroupVendor, WorkgroupVendor>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Workgroup, x => x.Ignore());

            CreateMap<WorkgroupAccount, WorkgroupAccount>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Workgroup, x => x.Ignore());

            CreateMap<CustomField, CustomField>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Organization, x => x.Ignore());

            CreateMap<ServiceMessage, ServiceMessage>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<OrderHistory, SearchResults.OrderResult>()
                .ForMember(x => x.Id, x => x.MapFrom(o => o.OrderId))
                .ForMember(x => x.DeliverTo, x => x.MapFrom(o => o.ShipTo))
                .ForMember(x => x.DeliverToEmail, x => x.MapFrom(o => o.ShipToEmail)
                );
        }
    }
}