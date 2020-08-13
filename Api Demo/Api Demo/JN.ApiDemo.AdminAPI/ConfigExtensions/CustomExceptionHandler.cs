using System;
using System.Text.Json;
using JN.Authentication.HelperClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

//using Newtonsoft.Json;

namespace JN.ApiDemo.AdminAPI.ConfigExtensions
{
    public static class CustomExceptionHandler
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory,
            bool isProduction)
        {
            app.UseExceptionHandler(appBuilder =>
            {

                appBuilder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        var exception = exceptionHandlerFeature.Error;

                        var problemDetails = new ProblemDetails
                        {
                            Instance = context.Request.HttpContext.Request.Path,
                        };

                        switch (exception)
                        {
                            // BadHttpRequestException can be thrown by webserver if the request is invalid, request body too large, etc
                            case BadHttpRequestException badHttpRequestException:
                                problemDetails.Title = "The request is invalid";
                                problemDetails.Status = StatusCodes.Status400BadRequest;
                                problemDetails.Detail = badHttpRequestException.Message;
                                break;
                            case CustomAuthException authException:
                                problemDetails.Title = "Authentication error. See details.";
                                problemDetails.Status = authException.ErrorCode;
                                problemDetails.Detail = authException.Message;
                                break;
                            default:
                            {
                                var logger = loggerFactory.CreateLogger("CustomExceptionHandler");
                                logger.LogError($"Error: {exceptionHandlerFeature.Error}");

                                problemDetails.Title = "Unexpected error. See details.";
                                problemDetails.Status = StatusCodes.Status500InternalServerError;
                                problemDetails.Detail = !isProduction ? exception.Message + Environment.NewLine + exception.StackTrace : exception.Message;
                                break;
                            }
                        }

                        context.Response.StatusCode = problemDetails.Status.Value;
                        context.Response.ContentType = "application/problem+json";

                        var json = JsonSerializer.Serialize(problemDetails);
                        await context.Response.WriteAsync(json);
                    }
                });
            });
        }


        public static IMvcBuilder CustomConfigureApiBehaviorOptions(this IMvcBuilder builder)
        {
            return builder.ConfigureApiBehaviorOptions(options =>
            {

                //options.SuppressConsumesConstraintForFormFileParameters = true;
                //options.SuppressInferBindingSourcesForParameters = true;
                //options.SuppressModelStateInvalidFilter = true;
                //options.SuppressMapClientErrors = true;
                //options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = "https://httpstatuses.com/404";


                // executed when ModelState is invalid
                //delegate used to convert invalid ModelStateDictionary (errors) into an IActionResult in controllers with [ApiController] Attribute
                //[ApiController] Attribute will validate modelstate and produce bad request automatically. This configuration allows to control that validation


                options.InvalidModelStateResponseFactory = context =>
                {
                    // create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                    var validationProblemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                        context.HttpContext,
                        context.ModelState);

                    // add additional info not added by default
                    validationProblemDetails.Detail = "See the errors field for details.";
                    validationProblemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    var actionExecutingContext =
                        context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    //
                    // if the context couldn't be cast to an ActionExecutingContext
                    // because it's a ControllerContext, we're dealing with an issue 
                    // that happened after the initial input was correctly parsed.  
                    // This happens, for example, when manually validating an object inside
                    // of a controller action.  That means that by then all keys
                    // WERE correctly found and parsed.  In that case, we're
                    // thus also dealing with a validation error.
                    if (context.ModelState.ErrorCount > 0 &&
                        (context is ControllerContext ||
                         actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        //problemDetails.Type = "https://link.to.details.com/validationproblem";
                        validationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        validationProblemDetails.Title = "One or more validation errors occurred.";

                        return new UnprocessableEntityObjectResult(validationProblemDetails)
                        {
                            ContentTypes = { "application/problem+json", "application/problem+xml" }
                        };
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    validationProblemDetails.Status = StatusCodes.Status400BadRequest;
                    validationProblemDetails.Title = "One or more errors on input occurred.";
                    return new BadRequestObjectResult(validationProblemDetails)
                    {
                        ContentTypes = {"application/problem+json", "application/problem+xml" }
                    };
                };
            });
        }
    }
}
