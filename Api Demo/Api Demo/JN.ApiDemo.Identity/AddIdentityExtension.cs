using System;
using JN.ApiDemo.Identity.Config;
using JN.ApiDemo.Identity.Data;
using JN.ApiDemo.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JN.ApiDemo.Identity
{


    public static class AddIdentityExtension
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration,
            IdentityConfig config)
        {
            if (config == null)
                throw new ArgumentException("Invalid configuration for Identity");

            services.AddDbContext<IdentityDataContext>(options =>

                options.UseSqlServer(
                    configuration.GetConnectionString(config.ConStringConfigName)
                )
            );


            // default implementation: IdentityUser and IdentityRole are default implementations
            //services.AddIdentityCore<IdentityUser>()
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<DataContext>();

            //custom implementation:

            services.AddIdentityCore<ApplicationUser>() // <-- ApplicationUser is custom implementation
                .AddRoles<ApplicationRole>() // <-- ApplicationRole is custom implementation
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddEntityFrameworkStores<IdentityDataContext>();



            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(config.DefaultLockoutTimeMinutes);
                options.Lockout.MaxFailedAccessAttempts = config.MaxFailedAccessAttempts;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            return services;
        }
    }

 
}
