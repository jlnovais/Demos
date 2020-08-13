using System.Net;
using System.Text.Json;
using JN.ApiDemo.Identity.Dto;
using JN.ApiDemo.Utils.Paging;
using Microsoft.AspNetCore.Mvc;

namespace JN.ApiDemo.AdminAPI.Helpers
{
    public static class ControllersHelper
    {


        public static void SetPaginationInfoHeader<T>(this ControllerBase controller, PagedList<T> list)
        {
            object paginationInfo = (list == null)
                ? new
                {
                    totalItems = 0,
                    pageSize = 0,
                    currentPage = 0,
                    totalPages = 0,
                    HasNext = false,
                    HasPrevious = false
                }
                : new
                {
                    totalItems = list.TotalItems,
                    pageSize = list.PageSize,
                    currentPage = list.CurrentPage,
                    totalPages = list.TotalPages,
                    list.HasNext,
                    list.HasPrevious
                };

            controller.Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationInfo));
        }

        public static ActionResult GetValidationProblem(this ControllerBase controller, IdentityResult result)
        {
            foreach (var resultError in result.Errors)
            {
                controller.ModelState.TryAddModelError("ObjectErrors", resultError);
            }

            //controller.HttpContext.Response.ContentType = "application/problem+json";

            return controller.ValidationProblem(controller.ModelState);
        }

        public static ActionResult GetProblem(this ControllerBase controller, IdentityResult result)
        {

            HttpStatusCode httpStatus;
            switch (result.ErrorType)
            {
                case ErrorType.NotFound:
                    httpStatus = HttpStatusCode.NotFound;
                    break;
                case ErrorType.InvalidParameters:
                    httpStatus = HttpStatusCode.BadRequest;
                    break;
                default:
                    httpStatus = HttpStatusCode.InternalServerError;
                    break;
            }

            //controller.HttpContext.Response.ContentType = "application/problem+json";

            return controller.Problem(
                statusCode: (int) httpStatus,
                detail: result.Errors == null ? "" : string.Join("; ", result.Errors),
                instance: controller.HttpContext.Request.Path,
                title: "Error processing request"
                //type: ""
            );

        }

        public static ActionResult GetGenericProblem(this ControllerBase controller, IdentityResult result)
        {
            if (result.ErrorType == ErrorType.InvalidParameters)
                return controller.GetValidationProblem(result);

            return controller.GetProblem(result);

        }

        public static ActionResult GetGenericProblem(this ControllerBase controller, HttpStatusCode httpStatus, string details)
        {
            //controller.HttpContext.Response.ContentType = "application/problem+json";

            return controller.Problem(
                statusCode: (int)httpStatus,
                detail: details,
                instance: controller.HttpContext.Request.Path,
                title: "Error processing request"
                //type: ""
            );

        }

    }


}