using System;
using System.IO;
using Microsoft.AspNetCore.Builder;

namespace JN.ApiDemo.AdminAPI.Swagger
{
    public static class SwaggerConfigExtension
    {
        public static IApplicationBuilder ConfigSwagger(this IApplicationBuilder app, SwaggerOptionsConfig config,
            Func<Stream> customPageLoader = null)
        {

            //https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio
            // Enable middleware to serve generated Swagger as a JSON endpoint.

            app.UseSwagger(option=> { option.RouteTemplate = config.JsonRoute; });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.InjectStylesheet("/css/customcss.css");

                if (customPageLoader != null)
                    c.IndexStream = customPageLoader;

                c.SwaggerEndpoint(config.UiEndpoint, config.Description);

                //To serve the Swagger UI at the app's root set the RoutePrefix property to an empty string
                c.RoutePrefix = string.Empty;

                c.EnableFilter();
                c.DisplayRequestDuration();

            });

            return app;
        }

 
    }
}
