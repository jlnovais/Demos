using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JN.ApiDemo.AdminAPI.ServicesInstallers;
using JN.ApiDemo.Contracts.V1.Admin.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JN.ApiDemo.AdminAPI.Swagger
{


    public class ProblemDetailsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var operationResponse in operation.Responses)
            {
                if (operationResponse.Key.StartsWith("2"))
                {
                    // for 2xx codes, remove problems media type from returned type
                    operationResponse.Value.Content =
                        operationResponse.Value.Content.Where(pair =>
                                !(pair.Key == "application/problem+json" || pair.Key == "application/problem+xml"))
                            .ToDictionary(r => r.Key, r => r.Value);
                }

                if (operationResponse.Key.StartsWith("4") || operationResponse.Key.StartsWith("5"))
                {
                    // for 4xx and 5xx codes, keep only problems media type in returned type
                    operationResponse.Value.Content =
                        operationResponse.Value.Content.Where(pair =>
                                (pair.Key == "application/problem+json" || pair.Key == "application/problem+xml"))
                            .ToDictionary(r => r.Key, r => r.Value);

                }
            }
        }
    }


    public class CustomMediaTypeFilter : IOperationFilter
    {

        private void AddType<T>(string mediaTypeName, string description, OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.SchemaRepository.Schemas.TryGetValue(nameof(T), out var schema)) 
                return;

            schema = context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
            schema.Description = description;

            var value = new OpenApiMediaType()
            {
                Schema = schema
            };

            if (operation.Responses[StatusCodes.Status200OK.ToString()].Content.ContainsKey(mediaTypeName))
            {
                var u = operation.Responses[StatusCodes.Status200OK.ToString()].Content[mediaTypeName];
                u.Schema = schema;

            }
            else
                operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(mediaTypeName, value);
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "GetUsers")
            {
                AddType<IEnumerable<UserDetailsShortResponse>>(AdminApiConstants.MediaTypeShortUserDetails, "List of user details (short version)", operation, context);
            }

            if (operation.OperationId == "GetUser")
            {
                AddType<UserDetailsShortResponse>(AdminApiConstants.MediaTypeShortUserDetails, "User details (short version)", operation, context);
            }
        }
    }


    public class SwaggerInstaller : IServiceInstaller
    {

        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", GetInfo());

                c.OperationFilter<CustomMediaTypeFilter>();
                c.OperationFilter<ProblemDetailsFilter>();

                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "**Basic Authorization** - specify username and password.",
                });



                //load all xml comment files
                var executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

                if (!string.IsNullOrWhiteSpace(executingAssemblyName))
                {
                    var assemblyNamespace = executingAssemblyName.Split('.').First();

                    var pathToXmlDocumentsToLoad = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(x => x.FullName.StartsWith(assemblyNamespace))
                        .Select(x => x.GetName().Name + ".xml")
                        .ToList();

                    pathToXmlDocumentsToLoad.ForEach(x =>
                    {
                        var fullPath = Path.Combine(AppContext.BaseDirectory, x);
                        if (File.Exists(fullPath))
                            c.IncludeXmlComments(fullPath, true);
                    });
                }



                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basic"
                            }
                        },
                        Array.Empty<string>()
                    }
                });


                // Adds fluent validation rules to swagger
                c.AddFluentValidationRules();

            });
        }



        private static OpenApiInfo GetInfo()
        {
            return new OpenApiInfo
            {
                Version = "v1",
                Title = "Demo Admin API",
                Description = "An API to manage Users and Keys. Implemented using [ASP.NET Core Web API](https://dotnet.microsoft.com/apps/aspnet/apis).",
                //TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "José Novais",
                    Email = "contacto@josenovais.com",
                    Url = new Uri("https://www.josenovais.com"),
                },
                //License = new OpenApiLicense
                //{
                //    Name = "Use under LICX",
                //    Url = new Uri("https://example.com/license"),
                //}

            };
        }


    }

 
}