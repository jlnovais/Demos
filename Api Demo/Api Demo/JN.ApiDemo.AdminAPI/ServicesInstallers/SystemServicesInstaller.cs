using FluentValidation.AspNetCore;
using JN.ApiDemo.AdminAPI.ConfigExtensions;
using JN.ApiDemo.Contracts.V1.Admin.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace JN.ApiDemo.AdminAPI.ServicesInstallers
{

    public class SystemServicesInstaller : IServiceInstaller
    {

        

        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {


            services


                .AddControllers(setupAction =>
                {

                    // ProducesResponseType for all controllers

                    //setupAction.Filters.Add(
                    //    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    
                    //setupAction.Filters.Add(
                    //    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    
                    setupAction.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                    setupAction.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));

                    setupAction.Filters.Add(
                        new ProducesDefaultResponseTypeAttribute() );

                    // for now keep ReturnHttpNotAcceptable off because Media type application/problem+json is lost in combination with ProducesAttribute
                    //setupAction.ReturnHttpNotAcceptable = true;

                })
                .AddXmlDataContractSerializerFormatters()

                //used for JsonPatch documents
                .AddNewtonsoftJson(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })


                .AddJsonOptions(opts => { opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true; })
                .CustomConfigureApiBehaviorOptions()
//                .AddFluentValidation(x=>x.RegisterValidatorsFromAssembly(typeof(UserRegistrationRequestValidator).Assembly ))
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<UserRequest>()   )
                ;
        }
    }
}