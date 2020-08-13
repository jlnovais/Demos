using System.Threading.Tasks;
using JN.ApiDemo.Identity;
using JN.ApiDemo.Identity.Config;
using JN.ApiDemo.Identity.Services;
using JN.ApiDemo.Utils.Sorting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JN.ApiDemo.UpdateDB
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }


        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<WorkerService>();

                    services.AddIdentity(hostContext.Configuration, new IdentityConfig()
                    {
                        MaxFailedAccessAttempts = 3,
                        ConStringConfigName = "SqlConnectionIdentity",
                        DefaultLockoutTimeMinutes = 2
                    });

                    services.AddScoped<IIdentityService, IdentityService>();
                    services.AddScoped<ISortingPropertyMappingService, SortingPropertyMappingService>();
                });
        }

    }
}
