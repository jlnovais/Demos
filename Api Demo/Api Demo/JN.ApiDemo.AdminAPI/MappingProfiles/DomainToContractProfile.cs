using AutoMapper;
using JN.ApiDemo.Contracts.V1.Admin.Requests;
using JN.ApiDemo.Contracts.V1.Admin.Responses;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Dto;

namespace JN.ApiDemo.AdminAPI.MappingProfiles
{
    public class DomainToContractProfile : Profile
    {
        public DomainToContractProfile()
        {
            CreateMap<UserWithRoles, UserDetailsResponse>()
                .IncludeMembers(x => x.User)
                .ForMember(dest => dest.Roles, opt =>
                    opt.MapFrom(src => string.Join(';', src.Roles)));


            CreateMap<UserWithRoles, UserDetailsShortResponse>()
                .IncludeMembers(x => x.User);

            CreateMap<ApplicationUser, UserDetailsResponse>();

            CreateMap<ApplicationUser, UserDetailsShortResponse>()
                .ForMember(dest => dest.Name,
                    opts =>
                        opts.MapFrom(src => (src.FirstName + " " + src.LastName).Trim())
                );

            CreateMap<ApplicationUser, UserRequestUpdate>();

            CreateMap<ApiKey, UserApiKeyResponse>()
                .ForMember(dest => dest.UserId, 
                    opts => opts.MapFrom(src => src.User.UserName));

            CreateMap<ApiKey, UserApiKeyUpdateRequest>();
        }
    }
}