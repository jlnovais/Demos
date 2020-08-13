using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using JN.ApiDemo.AdminAPI.Helpers;
using JN.ApiDemo.AdminAPI.Swagger;
using JN.ApiDemo.Contracts.V1.Admin.Requests;
using JN.ApiDemo.Contracts.V1.Admin.Responses;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Dto;
using JN.ApiDemo.Identity.Parameters;
using JN.ApiDemo.Identity.Services;
using JN.ApiDemo.Utils.Sorting;
using JN.Authentication.Scheme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JN.ApiDemo.AdminAPI.Controllers.V1
{


    [Route("api/v1/Users")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme, Policy = "IsAdminPolicy")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ISortingPropertyMappingService _mappingService;

        public UsersController(IIdentityService identityService, IMapper mapper, ISortingPropertyMappingService mappingService)
        {
            _identityService = identityService;
            _mapper = mapper;
            _mappingService = mappingService;
        }

        /// <summary>
        /// List methods available for this resource.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,DELETE,PUT,PATCH");
            return Ok();
        }


        /// <summary>
        /// Get list of users. A custom accept header can be used for returning a short version of user details.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Returns list of 'UserDetailsResponse' or 'UserDetailsShortResponse' objects (according to the accept header used on request) </returns>
        /// <remarks>
        /// Sample Response:
        /// 
        ///     [
        ///         {
        ///             "id": 1,
        ///             "username": "user",
        ///             "email": "email@email.email",
        ///             "phoneNumber": "123456789",
        ///             "notificationEmail": "email@email.email",
        ///             "firstName": "Jose",
        ///             "lastName": "Test",
        ///             "active": true,
        ///             "description": "this a test user",
        ///             "dateCreated": "2020-07-08T19:57:23.5669785",
        ///             "roles": "Admin"
        ///         }
        ///     ]
        /// 
        /// Sample Response (short):
        /// 
        ///     [
        ///         {
        ///             "id": 1,
        ///             "username": "user",
        ///             "email": "email@email.email",
        ///             "name": "Jose Test",
        ///             "active": true,
        ///             "description": "this a test user"
        ///         }
        ///     ]
        /// 
        /// 
        /// </remarks>
        /// <response code="200">Returns the list of users</response>
        /// <response code="400">Invalid request - parameters are not valid</response>
        /// <response code="404">Specified Id not found</response>
        [ProducesCustom("application/json",
            "application/xml",
            "application/problem+json", "application/problem+xml"
            )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestHeaderMatchesMediaType("Accept", "application/json", "application/xml")]
        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<IEnumerable<UserDetailsResponse>>> GetUsers([FromQuery] UsersParameters parameters)
        {
            return await GetUsersGeneric<UserDetailsResponse>(parameters);
        }

        [ProducesCustom(
            AdminApiConstants.MediaTypeShortUserDetails, 
            "application/problem+json", "application/problem+xml"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestHeaderMatchesMediaType("Accept", AdminApiConstants.MediaTypeShortUserDetails)]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<UserDetailsShortResponse>>> GetUsersShort([FromQuery] UsersParameters parameters)

        {
            return await GetUsersGeneric<UserDetailsShortResponse>(parameters);
        }


        private async Task<ActionResult<IEnumerable<T>>> GetUsersGeneric<T>(UsersParameters parameters)
        {
            bool isValidMapping = _mappingService.ValidMappingExistsFor<T, ApplicationUser>(parameters.OrderBy);

            parameters.SetSortingMappingSourceTypeName(typeof(T).Name);

            if (!isValidMapping)
            {
                return this.GetGenericProblem(HttpStatusCode.BadRequest,
                    $"Sorting using '{parameters.OrderBy}' is not possible");
            }

            parameters.SetSortingMappingDestinationTypeName(nameof(ApplicationUser));

            var result = await _identityService.GetUsersFromRole(parameters);


            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            this.SetPaginationInfoHeader(result.Object);

            return Ok(_mapper.Map<IEnumerable<T>>(result.Object));
        }


        ///  <summary>
        ///  Get user details for one user. A custom accept header can be used for returning a short version of user details.
        ///  </summary>
        ///  <param name="userId">user Id to get details</param>
        ///  <returns>Returns 'UserDetailsResponse' or 'UserDetailsShortResponse' object (according to the accept header used on request) </returns>
        /// <remarks>
        ///  Sample Response:
        ///  
        ///      {
        ///          "id": 1,
        ///          "username": "user",
        ///          "email": "email@email.email",
        ///          "phoneNumber": "123456789",
        ///          "notificationEmail": "email@email.email",
        ///          "firstName": "Jose",
        ///          "lastName": "Test",
        ///          "active": true,
        ///          "description": "this a test user",
        ///          "dateCreated": "2020-07-08T19:57:23.5669785",
        ///          "roles": "Admin"
        ///      }
        /// 
        ///  Sample Response (short):
        /// 
        ///      {
        ///          "id": 1,
        ///          "username": "user",
        ///          "email": "email@email.email",
        ///          "name": "Jose Test",
        ///          "active": true,
        ///          "description": "this a test user"
        ///      }
        ///  
        ///  
        ///  </remarks>
        ///  <response code="200">Returns the user details</response>
        ///  <response code="404">Specified Id not found</response>
        [ProducesCustom("application/json",
            "application/xml",
            "application/problem+json",
            "application/problem+xml"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept", "application/json", "application/xml")]
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<ActionResult<UserDetailsResponse>> GetUser(string userId)
        {
            return await GetUserGeneric<UserDetailsResponse>(userId);
        }


        [ProducesCustom(
            AdminApiConstants.MediaTypeShortUserDetails,
            "application/problem+json"
            //, "application/problem+xml"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept", AdminApiConstants.MediaTypeShortUserDetails)]
        [HttpGet("{userId}")]
        [ApiExplorerSettings (IgnoreApi = true) ]
        public async Task<ActionResult<UserDetailsShortResponse>> GetUserShort(string userId)
        {
            return await GetUserGeneric<UserDetailsShortResponse>(userId);
        }

        private async Task<ActionResult<T>> GetUserGeneric<T>(string userId)
        {
            var result = await _identityService.GetUserByUsername(userId);
            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            return typeof(T) == typeof(UserDetailsShortResponse)
                ? Ok(GetUserToReturnShort(result.Object))
                : Ok(GetUserToReturn(result.Object));
        }


        private UserDetailsResponse GetUserToReturn(UserWithRoles userWithRoles)
        {
            var userToReturn = _mapper.Map<UserDetailsResponse>(userWithRoles.User);
            userToReturn.Roles = string.Join(';', userWithRoles.Roles);

            return userToReturn;
        }

        private UserDetailsShortResponse GetUserToReturnShort(UserWithRoles userWithRoles)
        {
            var userToReturn = _mapper.Map<UserDetailsShortResponse>(userWithRoles.User);

            return userToReturn;
        }


        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="request">Details to create new user</param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="422">Validation problems</response>
        /// <response code="400">Invalid request</response>
        [ProducesCustom("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        public async Task<ActionResult<UserDetailsResponse>> RegisterUser(UserRequestCreate request)
        {
            var user = _mapper.Map<ApplicationUser>(request);

            var roles = ParametersHelper.GetRoles(request.Roles);

            var result = await _identityService.RegisterUserAsync(user, request.Password, roles);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            var resultGetUser = await _identityService.GetUserByUsername(user.UserName);
            var userToReturn = GetUserToReturn(resultGetUser.Object);

            return CreatedAtRoute("GetUser",
                new { userId = user.UserName },
                userToReturn);
        }

        /// <summary>
        /// Delete an existing user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="204">The user has been removed, as well all API keys for that account.</response>
        /// <response code="404">User not found.</response>
        [ProducesCustom("application/json",
            "application/xml","application/problem+json", "application/problem+xml")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var resultDelete = await _identityService.DeleteUserAsync(userId);

            if (!resultDelete.Success)
            {
                return this.GetGenericProblem(resultDelete);
            }

            return NoContent();
        }

        /// <summary>
        /// Update an user - full update.
        /// </summary>
        /// <param name="userId">User Id to update</param>
        /// <param name="request">User details to update</param>
        /// <returns></returns>
        /// <response code="204">User details updated.</response>
        /// <response code="404">User not found.</response>
        /// <response code="400">Invalid request.</response>
        [ProducesCustom("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UserRequestUpdate request)
        {

            var resultGet = await _identityService.GetUserByUsername(userId);
            if (!resultGet.Success)
            {
                return this.GetProblem(resultGet);
            }

            var user = _mapper.Map(request, resultGet.Object.User);

            var roles = ParametersHelper.GetRoles(request.Roles);

            var result = await _identityService.UpdateUserAsync(user, request.Password, roles);

            if (!result.Success)
            {
                return this.GetValidationProblem(result);
            }

            return NoContent();
        }

        /// <summary>
        /// Update an user - partial update. Must use 'application/json-patch+json' content type in request header an the body must be a json patch document.
        /// </summary>
        /// <param name="userId">User Id to update</param>
        /// <param name="patchDocument">Document with patch instructions</param>
        /// <returns></returns>
        /// <response code="204">User details updated.</response>
        /// <response code="404">User not found.</response>
        /// <response code="422">Validation problems</response>
        /// <response code="400">Invalid request.</response>
        [ProducesCustom("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUserPartial(string userId, JsonPatchDocument<UserRequestUpdate> patchDocument)
        {

            var resultGet = await _identityService.GetUserByUsername(userId);
            if (!resultGet.Success)
            {
                return this.GetGenericProblem(resultGet);
            }

            var userFromRepo = resultGet.Object.User;

            var userDetailsToPatch = _mapper.Map<UserRequestUpdate>(userFromRepo);

            //must add package Microsoft.AspNetCore.Mvc.NewtonsoftJson and add it in startup
            try
            {
                patchDocument.ApplyTo(userDetailsToPatch, ModelState);
            }
            catch (Exception e)
            {
                return this.GetGenericProblem(HttpStatusCode.BadRequest, e.Message);
            }
            

            //validate if model is valid after applying patch 
            if (!TryValidateModel(userDetailsToPatch))
            {
                // ValidationProblem is overriden because the base method doesn't use api behaviour and doesn't use http 422 as we have configured
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userDetailsToPatch, userFromRepo);

            var roles = ParametersHelper.GetRoles(userDetailsToPatch.Roles);

            var result = await _identityService.UpdateUserAsync(userFromRepo, userDetailsToPatch.Password, roles);

            if (!result.Success)
            {
                return this.GetValidationProblem(result);
            }


            return NoContent();
        }


        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            // force to use our api config
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }



    }

}
