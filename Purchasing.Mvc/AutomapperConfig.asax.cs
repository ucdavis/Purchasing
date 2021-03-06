﻿using AutoMapper;
using Purchasing.Core.Domain;
using Purchasing.Core.Queries;
using Purchasing.Mvc.Controllers;

namespace Purchasing.Mvc
{
    public class AutomapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ViewModelProfile>());
        }
    }


    public class ViewModelProfile : Profile
    {
        protected override void Configure()
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