using System.Collections.Generic;
using System.Threading.Tasks;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Dto;
using JN.ApiDemo.Identity.Parameters;
using JN.ApiDemo.Utils.Paging;

namespace JN.ApiDemo.Identity.Services
{
    public interface IIdentityService
    {
        Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);
        Task<IdentityResult<ApiKey>> RegisterUserApiKeyAsync(ApiKey apiKey, string userId);
        Task<IdentityResult<UserWithRoles>> GetUserByUsername(string username, string password, bool lockoutOnFailure);
        Task<IdentityResult<UserWithRoles>> GetUserByKey(string key);
        Task<IdentityResult> UpdateApiKeyAsync(ApiKey apiKey);
        Task<IdentityResult<UserWithRoles>> GetUserByUsername(string username);
        Task<IdentityResult<PagedList<UserWithRoles>>> GetUsersFromRole(UsersParameters parameters);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);
        Task<IdentityResult> DeleteUserAsync(string userName);
        Task<IdentityResult<ApiKey>> GetUserApiKeyAsync(string apiKey, string userId);
        Task<IdentityResult> DeleteUserApiKeyAsync(string apiKey, string userId);
        Task<IdentityResult<PagedList<ApiKey>>> GetUserApiKeysAsync(UserKeysParameters userId);
    }
}