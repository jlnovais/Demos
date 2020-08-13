using JN.ApiDemo.Identity;
using JN.ApiDemo.Identity.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JN.ApiDemo.AdminAPI.ServicesInstallers
{
    public class IdentityInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity(configuration, new IdentityConfig()
            {
                MaxFailedAccessAttempts = 5,
                ConStringConfigName = "SqlConnectionIdentity",
                DefaultLockoutTimeMinutes = 2
            });
        }
    }
}
