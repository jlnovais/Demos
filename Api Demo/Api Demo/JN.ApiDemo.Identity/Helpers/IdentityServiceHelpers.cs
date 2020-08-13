using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Dto;
using Microsoft.AspNetCore.Identity;

namespace JN.ApiDemo.Identity.Helpers
{
    internal static class IdentityServiceHelpers
    {
        /// <summary>
        /// Used to reset a user's lockout count.
        /// Based on https://github.com/dotnet/aspnetcore/blob/v2.2.2/src/Identity/Core/src/SignInManager.cs
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="user">The user</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="Microsoft.AspNetCore.Identity.IdentityResult"/> of the operation.</returns>
        internal static Task ResetLockout(this UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            if (userManager.SupportsUserLockout)
            {
                return userManager.ResetAccessFailedCountAsync(user);
            }

            return Task.CompletedTask;
        }

        internal static UserWithRoles GetDetailsObject(ApplicationUser info, IEnumerable<ApplicationRole> roles)
        {
            var userWithRoles = new UserWithRoles
            {
                User = info,
                Roles = roles.Select(role => role.Name)
            };

            return userWithRoles;
        }

    }
}
