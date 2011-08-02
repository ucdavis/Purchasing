using AutoMapper;
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
        }
    }
}