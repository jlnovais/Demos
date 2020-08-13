using JN.ApiDemo.AdminAPI.Swagger;
using Microsoft.Extensions.Configuration;

namespace JN.ApiDemo.AdminAPI.Helpers
{
    public static class SettingsHelper
    {

        public static SwaggerOptionsConfig GetSwaggerConfig(this IConfiguration configuration, string sectionName)
        {
            SwaggerOptionsConfig config = new SwaggerOptionsConfig();

            configuration.Bind(sectionName, config);

            return config;
        }
    }
}
