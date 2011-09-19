﻿using AutoMapper;
using Purchasing.Core.Domain;

namespace Purchasing.Web
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
            CreateMap<Workgroup, Workgroup>();
            CreateMap<AutoApproval, AutoApproval>()
                .ForMember(x => x.User, x=> x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}