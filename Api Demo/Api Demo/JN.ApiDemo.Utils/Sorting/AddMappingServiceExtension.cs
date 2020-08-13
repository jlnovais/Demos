using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace JN.ApiDemo.Utils.Sorting
{
    public static class AddMappingServiceExtension
    {
        public static IServiceCollection AddMappingService(this IServiceCollection services,
            Func<IEnumerable<IPropertyMapping>> mapOptions)
        {
            services.AddTransient<ISortingPropertyMappingService, SortingPropertyMappingService>(opt => new SortingPropertyMappingService(mapOptions()));

            return services;
        }
    }
}
