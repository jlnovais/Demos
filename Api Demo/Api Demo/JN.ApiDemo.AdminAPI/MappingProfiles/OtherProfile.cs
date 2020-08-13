using AutoMapper;
using JN.ApiDemo.Identity.Parameters;
using JN.ApiDemo.Utils.Parameters;

namespace JN.ApiDemo.AdminAPI.MappingProfiles
{
    public class OtherProfile : Profile
    {
        public OtherProfile()
        {
            CreateMap<PaginationAndSortingParameters, UserKeysParameters>();
        }
        
    }
}