using System.Threading.Tasks;
using JN.ApiDemo.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace JN.ApiDemo.AdminAPI.AuthorizationHandlers
{

    // A custom authorization requirement 
    internal class CustomRequirement : IAuthorizationRequirement
    {
        public bool IsAdmin { get; private set; }

        public CustomRequirement(bool isAdmin)
        {
            IsAdmin = isAdmin;
        }
    }

    internal class CustomAuthorizationHandler : AuthorizationHandler<CustomRequirement>
    {
        public CustomAuthorizationHandler(IConfiguration config)
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            if (context.User.IsInRole(IdentityConstants.UserRoles.Admin.ToString()))
                // Mark the requirement as satisfied
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}