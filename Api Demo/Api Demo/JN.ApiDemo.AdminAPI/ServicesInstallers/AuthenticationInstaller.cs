using System.Text;
using JN.ApiDemo.AdminAPI.AuthorizationHandlers;
using JN.ApiDemo.AdminAPI.Services;
using JN.Authentication.Interfaces;
using JN.Authentication.Scheme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JN.ApiDemo.AdminAPI.ServicesInstallers
{
    public class AuthenticationInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            // Basic authentication 
            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    options.Realm = "adminApi";
                    options.LogInformation = true; //optional, default is false;
                    options.HttpPostMethodOnly = false;
                    options.HeaderEncoding = Encoding.UTF8; //optional, default is UTF8;
                    options.ChallengeResponse = BasicValidationService.ChallengeResponse;
                });

            // validation service
            services.AddTransient<IBasicValidationService, BasicValidationService>();

            // Add custom authorization handlers
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(true)));
                //options.AddPolicy("IsNotAdminPolicy", policy => policy.Requirements.Add(new CustomRequirement(false)));
            });

            services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();
            /*Authorization using custom policies - end*/

        }
    }
}