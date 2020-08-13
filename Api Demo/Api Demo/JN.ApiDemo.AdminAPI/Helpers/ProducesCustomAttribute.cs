using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JN.ApiDemo.AdminAPI.Helpers
{
    /// <summary>
    /// Media type application/problem+json lost in combination with ProducesAttribute.
    /// In this way, a custom 'Produces' is needed to avoid that problem.
    ///
    /// also:
    /// 
    /// In Start class / ConfigureServices method / services.AddControllers
    /// 
    /// ReturnHttpNotAcceptable should be kept to false (Default value) for this to work but this does not show the HTTP 406 Not Acceptable code when
    /// the Accept header in the request contains an unknown media type
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ProducesCustomAttribute : ProducesAttribute
    {
        public ProducesCustomAttribute(Type type) : base(type)
        {
        }

        public ProducesCustomAttribute(string contentType, params string[] additionalContentTypes) : base(contentType, additionalContentTypes)
        {
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
  
            if (context.Result is ObjectResult result && result.Value is ProblemDetails)
            {
                result.ContentTypes.Clear();

                if (context.HttpContext.Request.Headers.ContainsKey("Accept"))
                {
                    var contentTypesHeader = context.HttpContext.Request.Headers["accept"];

                    if (contentTypesHeader.Any(y => y.EndsWith("xml")))
                        result.ContentTypes.Add("application/problem+xml");
                    else
                    {
                        result.ContentTypes.Add("application/problem+json");
                    }
                }
                else
                {
                    result.ContentTypes.Add("application/problem+json");
                }

                return;
            }

            base.OnResultExecuting(context);
        }
    }
}