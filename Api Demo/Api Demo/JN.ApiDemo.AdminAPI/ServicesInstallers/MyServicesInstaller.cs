using System;
using System.Collections.Generic;
using JN.ApiDemo.Contracts.V1.Admin.Responses;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Services;
using JN.ApiDemo.Utils.Sorting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JN.ApiDemo.AdminAPI.ServicesInstallers
{
    public class MyServicesInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIdentityService, IdentityService>();

            //add properties mapping between Contract (dto) classes and Domain (entities) classes to allow sorting in the repository
            // we can only sort by contract object properties that map to Domain classes properties that could have different names,
            // and map to one ore more properties or need reverse order
            services.AddMappingService(() =>
            {
                var userMapping =
                    new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                        {"Username", new PropertyMappingValue(new List<string>() {"Username"})},
                        {"Email", new PropertyMappingValue(new List<string>() {"Email"})},
                        {"PhoneNumber", new PropertyMappingValue(new List<string>() {"PhoneNumber"})},
                        {"NotificationEmail", new PropertyMappingValue(new List<string>() {"NotificationEmail"})},
                        {"FirstName", new PropertyMappingValue(new List<string>() {"FirstName"})},
                        {"LastName", new PropertyMappingValue(new List<string>() {"LastName"})},
                        {"Active", new PropertyMappingValue(new List<string>() {"Active"})},
                        {"Description", new PropertyMappingValue(new List<string>() {"Description"})},
                        {"DateCreated", new PropertyMappingValue(new List<string>() {"DateCreated"})}
                    };

                var userMappingShort =
                    new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                    {
                        {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                        {"Username", new PropertyMappingValue(new List<string>() {"Username"})},
                        {"Email", new PropertyMappingValue(new List<string>() {"Email"})},
                        {"Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})},
                        {"Active", new PropertyMappingValue(new List<string>() {"Active"})},
                        {"Description", new PropertyMappingValue(new List<string>() {"Description"})}
                    };

                var apiKeyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
                {
                    {"Key", new PropertyMappingValue(new List<string>() {"Key"})},
                    {"CreationDate", new PropertyMappingValue(new List<string>() {"CreationDate"})},
                    {"Active", new PropertyMappingValue(new List<string>() {"Active"})}
                };

                return new IPropertyMapping[]
                {
                    new PropertyMapping<UserDetailsResponse, ApplicationUser>(userMapping),
                    new PropertyMapping<UserDetailsShortResponse, ApplicationUser>(userMappingShort),
                    new PropertyMapping<UserApiKeyResponse, ApiKey>(apiKeyMapping)
                };
            });

        }
    }
}
