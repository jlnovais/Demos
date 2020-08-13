using AutoMapper;
using JN.ApiDemo.Contracts.V1.Admin.Requests;
using JN.ApiDemo.Identity.Domain;

namespace JN.ApiDemo.AdminAPI.MappingProfiles
{
    public class ContractToDomainProfile : Profile
    {
        public ContractToDomainProfile()
        {
            CreateMap<UserRequest, ApplicationUser>();

            CreateMap<UserApiKeyRequest, ApiKey>();

            CreateMap<UserApiKeyUpdateRequest, ApiKey>();

        }

        
    }
}
