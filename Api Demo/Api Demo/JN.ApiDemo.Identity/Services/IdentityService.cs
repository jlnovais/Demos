using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JN.ApiDemo.Identity.Data;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Dto;
using JN.ApiDemo.Identity.Helpers;
using JN.ApiDemo.Identity.Parameters;
using JN.ApiDemo.Utils.Paging;
using JN.ApiDemo.Utils.Sorting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityResult = JN.ApiDemo.Identity.Dto.IdentityResult;


namespace JN.ApiDemo.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IdentityDataContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ISortingPropertyMappingService _sortingPropertyMappingService;

        public IdentityService(UserManager<ApplicationUser> userManager, 
            IdentityDataContext context, RoleManager<ApplicationRole> roleManager, ISortingPropertyMappingService sortingPropertyMappingService)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _sortingPropertyMappingService = sortingPropertyMappingService;
        }

        public async Task<IdentityResult<UserWithRoles>> GetUserByKey(string key)
        {
            var res = new IdentityResult<UserWithRoles>();

            var apiKeyResult = await _context.ApiKeys.SingleOrDefaultAsync(apiKey => apiKey.Key == key && apiKey.Active);

            if (apiKeyResult == null)
            {
                res.Errors = new[] { "Invalid key" };
                res.ErrorType = ErrorType.InvalidParameters;
                return res;
            }

            var user = await _userManager.FindByNameAsync(apiKeyResult.User.UserName);

            if (user == null)
            {
                res.Errors = new[] { "User with this username not found" };
                res.ErrorType = ErrorType.NotFound;
                return res;
            }

            var details = new UserWithRoles
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };

            res.Object = details;
            res.Success = true;

            return res;
        }

 
        /// <summary>
        /// Validate user by username and password
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="lockoutOnFailure">to disable the lockout feature, use false</param>
        /// <returns></returns>
        public async Task<IdentityResult<UserWithRoles>> GetUserByUsername(string username, string password, bool lockoutOnFailure)
        {
            var res = new IdentityResult<UserWithRoles>();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                res.Errors = new[] {"User with this username not found"};
                res.ErrorType = ErrorType.NotFound;
                return res;
            }

            
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                res.Errors = new[] { "Invalid password" };

                if (_userManager.SupportsUserLockout && lockoutOnFailure)
                {
                    // If lockout is requested, increment access failed count which might lock out the user
                    await _userManager.AccessFailedAsync(user);

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        res.Errors = new[] { "Invalid password", "Account is locked out" };
                    }
                }
                
                res.ErrorType = ErrorType.InvalidParameters;
                return res;
            }

            // account is still in lockout period
            if (_userManager.SupportsUserLockout && user.LockoutEnabled && user.LockoutEnd.HasValue)
            {
                if (user.LockoutEnd.Value.DateTime > DateTime.UtcNow)
                {
                    res.Errors = new[] { "Account is still locked out" };
                    res.ErrorType = ErrorType.InvalidParameters;
                    return res;
                }
            }

            await _userManager.ResetLockout(user);

            var details = new UserWithRoles
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };

            res.Object = details;
            res.Success = true;

            return res;

        }

        //public async Task<IdentityResult<IEnumerable<UserWithRoles>>> GetUsersFromRole(UsersParameters parameters)
        //{
        //    const int defaultRoleId = -1;

        //    var res = new IdentityResult<IEnumerable<UserWithRoles>>();

        //    int roleId;

        //    if (string.IsNullOrWhiteSpace(parameters.Role))
        //        roleId = defaultRoleId;
        //    else
        //    {
        //        var role = await _roleManager.FindByNameAsync(parameters.Role);
        //        if (role != null)
        //        {
        //            roleId = role.Id;
        //        }

        //        else
        //        {
        //            res.Success = false;
        //            res.Errors = new[] {"Invalid role"};
        //            res.ErrorType = ErrorType.NotFound;
        //            return res;
        //        }
        //    }

        //    // based on https://stackoverflow.com/questions/51004516/net-core-2-1-identity-get-all-users-with-their-associated-roles

        //    var usersList =
        //        (await _context.Users
        //                    //limit users to search to users that have the role selected
        //                .Where(user =>
        //                    roleId == defaultRoleId ||
        //                    _context.UserRoles.Where(r=> r.RoleId == roleId).Select(w=>w.UserId).Contains(user.Id)
        //                    )
        //                .SelectMany(
        //                    // -- below emulates a left outer join, as it returns DefaultIfEmpty in the collectionSelector
        //                    user => _context.UserRoles.Where(userRoleMapEntry => user.Id == userRoleMapEntry.UserId)
        //                        .DefaultIfEmpty(),
        //                    (user, roleMapEntry) => new {User = user, RoleMapEntry = roleMapEntry})
        //                .SelectMany(
        //                    // perform the same operation to convert role IDs from the role map entry to roles
        //                    x => _context.Roles.Where(role => role.Id == x.RoleMapEntry.RoleId).DefaultIfEmpty(),
        //                    (x, role) => new {User = x.User, RoleName = role.Name})
        //                .ToListAsync() // runs the queries and sends us back into EF Core LINQ world
        //        )
        //        .Aggregate(
        //            new Dictionary<ApplicationUser, List<string>>(), // seed (accumulator)
        //            (accumulator, itemToProcess) =>
        //            {
        //                // safely ensure the user entry is configured
        //                accumulator.TryAdd(itemToProcess.User, new List<string>());
        //                if (itemToProcess.RoleName != null)
        //                {
        //                    accumulator[itemToProcess.User].Add(itemToProcess.RoleName);
        //                }

        //                return accumulator;
        //            },
        //            x => x.Select(item => new UserWithRoles {User = item.Key, Roles = item.Value})
        //        );

        //    res.Success = true;
        //    res.Object = usersList;

        //    return res;
        //}

        public async Task<IdentityResult<PagedList<UserWithRoles>>> GetUsersFromRole(UsersParameters parameters)
        {
            const int defaultRoleId = -1;
            
            var res = new IdentityResult<PagedList<UserWithRoles>>();

            int roleId;

            if (string.IsNullOrWhiteSpace(parameters.Role))
                roleId = defaultRoleId;
            else
            {
                var role = await _roleManager.FindByNameAsync(parameters.Role);
                if (role != null)
                {
                    roleId = role.Id;
                }

                else
                {
                    res.Success = false;
                    res.Errors = new[] { "Invalid role" };
                    res.ErrorType = ErrorType.NotFound;
                    return res;
                }
            }

            // based on https://stackoverflow.com/questions/51004516/net-core-2-1-identity-get-all-users-with-their-associated-roles

            var totalUsers = 0;
            var usersList = _context.Users
                        //limit users to search to users that have the role selected
                        .Where(user =>
                            roleId == defaultRoleId ||
                            _context.UserRoles.Where(r => r.RoleId == roleId).Select(w => w.UserId).Contains(user.Id)
                        );

            var propertyMappingDictionary =
                _sortingPropertyMappingService.GetPropertyMappingByTypeNames(parameters.GetSortingMappingSourceTypeName(),parameters.GetSortingMappingDestinationTypeName());

            usersList = usersList.ApplySort(parameters.OrderBy, parameters.Direction, propertyMappingDictionary);

            totalUsers = usersList.Count();

            var usersWithRoles = (await
                    usersList
                        .Skip((parameters.Page - 1) * parameters.PageSize).Take(parameters.PageSize)
                        .SelectMany(
                            // -- below emulates a left outer join, as it returns DefaultIfEmpty in the collectionSelector
                            user => _context.UserRoles.Where(userRoleMapEntry => user.Id == userRoleMapEntry.UserId)
                                .DefaultIfEmpty(),
                            (user, roleMapEntry) => new {User = user, RoleMapEntry = roleMapEntry})
                        .SelectMany(
                            // perform the same operation to convert role IDs from the role map entry to roles
                            x => _context.Roles.Where(role => role.Id == x.RoleMapEntry.RoleId).DefaultIfEmpty(),
                            (x, role) => new {User = x.User, RoleName = role.Name})
                        .ToListAsync()
                )
                .Aggregate(
                    new Dictionary<ApplicationUser, List<string>>(), // seed (accumulator)
                    (accumulator, itemToProcess) =>
                    {
                        // safely ensure the user entry is configured
                        accumulator.TryAdd(itemToProcess.User, new List<string>());
                        if (itemToProcess.RoleName != null)
                        {
                            accumulator[itemToProcess.User].Add(itemToProcess.RoleName);
                        }

                        return accumulator;
                    },
                    x => x.Select(item => new UserWithRoles {User = item.Key, Roles = item.Value})
                );

            res.Success = true;
            res.Object = new PagedList<UserWithRoles>(usersWithRoles, totalUsers, parameters.Page, parameters.PageSize); 

            return res;
        }

        public async Task<IdentityResult<UserWithRoles>> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return new IdentityResult<UserWithRoles>("User with this username not found", errorType: ErrorType.NotFound);
            }

            var details = new UserWithRoles
            {
                User = user,
                Roles = await _userManager.GetRolesAsync(user)
            };

            var res = new IdentityResult<UserWithRoles>
            {
                Object = details, 
                Success = true
            };

            return res;

        }


        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName))
            {
                return new IdentityResult("Invalid user", errorType: ErrorType.InvalidParameters);
            }


            if (!string.IsNullOrWhiteSpace(password))
            {
                if (_userManager.PasswordValidators != null)
                {
                    foreach (var validator in _userManager.PasswordValidators)
                    {
                        var resValidation = await validator.ValidateAsync(_userManager, user, password);

                        if(!resValidation.Succeeded)
                            return new IdentityResult
                            {
                                ErrorType = ErrorType.InvalidParameters,
                                Errors = resValidation.Errors.Select(x=>x.Description + " " + x.Code)
                            };
                    }
                }

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
            }

            var res = await _userManager.UpdateAsync(user);
            
            if (!res.Succeeded)
            {
                return new IdentityResult
                {
                    Errors = res.Errors.Select(x => x.Description),
                    ErrorType = ErrorType.InvalidParameters
                };
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            if (roles != null)
            {
                foreach (var role in rolesList)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }

                foreach (var role in roles)
                {
                    if (Enum.TryParse(role, true, out IdentityConstants.UserRoles newRole))
                    {
                        await _userManager.AddToRoleAsync(user, newRole.ToString());
                    }
                }
            }

            return new IdentityResult() { Success = true };

        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.UserName))
            {
                return new IdentityResult("Empty username", errorType: ErrorType.InvalidParameters);
            }

            var existingUser = await _userManager.FindByNameAsync(user.UserName); // FindByEmailAsync(email);


            if (existingUser != null)
            {
                return new IdentityResult("User with this username already exists", errorType: ErrorType.InvalidParameters);
            }

            user.DateCreated = DateTime.Now;
            
            var createdUserResult = await _userManager.CreateAsync(user, password);

            if (!createdUserResult.Succeeded)
            {
                return new IdentityResult
                {
                    Errors = createdUserResult.Errors.Select(x => x.Description),
                    ErrorType = ErrorType.InvalidParameters
                };
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (Enum.TryParse(role, true, out IdentityConstants.UserRoles newRole))
                    {
                        await _userManager.AddToRoleAsync(user, newRole.ToString());
                    }
                }
            }

            return new IdentityResult() {Success = true};
        }


        public async Task<IdentityResult> DeleteUserAsync(string userName)
        {
            var existingUser = await _userManager.FindByNameAsync(userName); // FindByEmailAsync(email);

            if (existingUser == null)
            {
                return new IdentityResult("User with this username not found", errorType: ErrorType.NotFound);
            }

            var resDelete = await _userManager.DeleteAsync(existingUser);

            if (!resDelete.Succeeded)
            {
                return new IdentityResult
                {
                    Errors = resDelete.Errors.Select(x => x.Description),
                    ErrorType = ErrorType.Other
                };
            }

            return new IdentityResult() { Success = true };
        }

        public async Task<IdentityResult<ApiKey>> RegisterUserApiKeyAsync(ApiKey apiKey, string userId)
        {
            if (apiKey==null)
            {
                return new IdentityResult<ApiKey>("Invalid key details", errorType: ErrorType.InvalidParameters);
            }

            var existKey = await _context.ApiKeys.AnyAsync(item => item.Key.Equals(apiKey.Key));

            if(existKey)
            {
                return new IdentityResult<ApiKey>("Key already used", errorType: ErrorType.InvalidParameters);
            }

            var existingUser = await _userManager.FindByNameAsync(userId); // FindByEmailAsync(email);

            if (existingUser == null)
            {
                return new IdentityResult<ApiKey>("User with this username does not exist", errorType: ErrorType.NotFound);
            }

            apiKey.UserId = existingUser.Id;
            apiKey.CreationDate = DateTime.Now;

            var res = await _context.ApiKeys.AddAsync(apiKey);

            await _context.SaveChangesAsync();

            return new IdentityResult<ApiKey> {Success = true, Object = res.Entity};
        }

        public async Task<IdentityResult> DeleteUserApiKeyAsync(string apiKey, string userId)
        {
            var existingUser = await _userManager.FindByNameAsync(userId); // FindByEmailAsync(email);

            if (existingUser == null)
            {
                return new IdentityResult("User with this username does not exist", errorType: ErrorType.NotFound);
            }

            var keyToRemove = await _context.ApiKeys.FirstOrDefaultAsync(item => item.Key.Equals(apiKey) && item.UserId == existingUser.Id);
            
            if (keyToRemove == null)
            {
                return new IdentityResult("Key does not exist", errorType: ErrorType.NotFound);
            }

            var resKey =  _context.ApiKeys.Remove(keyToRemove);
            await _context.SaveChangesAsync();
            
            return new IdentityResult
            {
                Success = true
            };
        }


        public async Task<IdentityResult<ApiKey>> GetUserApiKeyAsync(string apiKey, string userId)
        {
            var existingUser = await _userManager.FindByNameAsync(userId); // FindByEmailAsync(email);

            if (existingUser == null)
            {
                return new IdentityResult<ApiKey>("User with this username does not exist", errorType: ErrorType.NotFound);
            }

            //var resKey = await (_context.ApiKeys.Where(item => item.Key.Equals(apiKey) && item.UserId == existingUser.Id)).FirstAsync();

            var resKey = await _context.ApiKeys.FirstOrDefaultAsync(item => item.Key.Equals(apiKey) && item.UserId == existingUser.Id);

            if (resKey == null)
            {
                return new IdentityResult<ApiKey>("Key does not exist", errorType: ErrorType.NotFound);
            }

            return new IdentityResult<ApiKey>
            {
                Success = true,
                Object = resKey
            };
        }

        public async Task<IdentityResult<PagedList<ApiKey>>> GetUserApiKeysAsync(UserKeysParameters userKeysParameters)
        {
            var existingUser = await _userManager.FindByNameAsync(userKeysParameters.UserId); // FindByEmailAsync(email);

            if (existingUser == null)
            {
                return new IdentityResult<PagedList<ApiKey>>("User with this username does not exist", errorType: ErrorType.NotFound);
            }

            var resKeys = _context.ApiKeys
                .Where(item => item.UserId == existingUser.Id);

            var propertyMappingDictionary =
                _sortingPropertyMappingService.GetPropertyMappingByTypeNames(userKeysParameters.GetSortingMappingSourceTypeName(), userKeysParameters.GetSortingMappingDestinationTypeName());

            resKeys = resKeys.ApplySort(userKeysParameters.OrderBy, userKeysParameters.Direction, propertyMappingDictionary);


            return new IdentityResult<PagedList<ApiKey>>
            {
                Success = true,
                Object = await PagedList<ApiKey>.Create(resKeys, userKeysParameters.Page, userKeysParameters.PageSize)
            };
        }

        public async Task<IdentityResult> UpdateApiKeyAsync(ApiKey key)
        {
            // nothing to do besides savechanges because entity being tracked has already been updated

            await _context.SaveChangesAsync();
            
            return new IdentityResult {Success = true};
        }


    }
}
