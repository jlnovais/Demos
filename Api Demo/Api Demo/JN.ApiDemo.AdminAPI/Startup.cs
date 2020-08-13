using System;
using AutoMapper;
using JN.ApiDemo.AdminAPI.ConfigExtensions;
using JN.ApiDemo.AdminAPI.Helpers;
using JN.ApiDemo.AdminAPI.ServicesInstallers;
using JN.ApiDemo.AdminAPI.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JN.ApiDemo.AdminAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallServicesInAssembly(Configuration);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //if (env.IsDevelopment())
            //    app.UseDeveloperExceptionPage();
            //else
                app.UseCustomExceptionHandler(loggerFactory, !env.IsDevelopment());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.ConfigSwagger(Configuration.GetSwaggerConfig("SwaggerOptions"),
                ()
                    => GetType().Assembly
                        .GetManifestResourceStream("JN.ApiDemo.AdminAPI.EmbeddedAssets.index.html"));

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
