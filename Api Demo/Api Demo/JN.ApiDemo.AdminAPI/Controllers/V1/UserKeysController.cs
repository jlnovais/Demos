using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using JN.ApiDemo.AdminAPI.Helpers;
using JN.ApiDemo.Contracts.V1.Admin.Requests;
using JN.ApiDemo.Contracts.V1.Admin.Responses;
using JN.ApiDemo.Identity.Domain;
using JN.ApiDemo.Identity.Parameters;
using JN.ApiDemo.Identity.Services;
using JN.ApiDemo.Utils.Parameters;
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
    [Route("api/v1/Users/{userId}/Keys")]
    [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme, Policy = "IsAdminPolicy")]
    [ApiController]
    public class UserKeysController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ISortingPropertyMappingService _mappingService;

        public UserKeysController(IIdentityService identityService, IMapper mapper, ISortingPropertyMappingService sortingPropertyMappingService)
        {
            _identityService = identityService;
            _mapper = mapper;
            _mappingService = sortingPropertyMappingService;
        }

        /// <summary>
        /// Create a new key for the given user
        /// </summary>
        /// <param name="userId">User Id for the new key</param>
        /// <param name="request">Key details - leaves 'key' property empty to create automatically</param>
        /// <returns></returns>
        /// <response code="201">Key created successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Specified user Id not found</response>
        /// <response code="422">Validation errors</response>
        [ProducesCustom("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [Consumes("application/json", "application/xml")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<UserApiKeyResponse>> RegisterUserKey(string userId, [FromBody] UserApiKeyRequest request)
        {
            var apiKey = _mapper.Map<ApiKey>(request);

            var key = !string.IsNullOrWhiteSpace(apiKey.Key) ? apiKey.Key : Guid.NewGuid().ToString();
            apiKey.Key = key;

            var result = await _identityService.RegisterUserApiKeyAsync(apiKey, userId);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            var resultKey = result.Object; //await _identityService.GetUserApiKeyAsync(key, userId);

            var apiKeyToReturn = _mapper.Map<UserApiKeyResponse>(resultKey);

            return CreatedAtRoute("GetKey",
                new { userId = userId, apiKey = key},
                apiKeyToReturn);
        }

        /// <summary>
        /// Get a key for the user
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="apiKey">key to find</param>
        /// <returns></returns>
        /// <response code="200">Key was found</response>
        /// <response code="404">Specified user Id or key not found</response>
        [ProducesCustom("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{apiKey}", Name = "GetKey")]
        public async Task<ActionResult<UserApiKeyResponse>> GetUserKey(string userId, string apiKey)
        {
            var result = await _identityService.GetUserApiKeyAsync(apiKey, userId);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            var keyResponse = _mapper.Map<UserApiKeyResponse>(result.Object);

            return Ok(keyResponse);
        }


        /// <summary>
        /// Get a list of user keys.
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="parameters">Search parameters</param>
        /// <returns></returns>
        /// <response code="200">Operation completed successful</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Specified user Id not found</response>
        [Produces("application/json",
            "application/xml", "application/problem+json", "application/problem+xml")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserApiKeyResponse>>> GetUserKeys(string userId, [FromQuery] PaginationAndSortingParameters parameters)
        {
            var param = _mapper.Map<UserKeysParameters>(parameters);
            param.UserId = userId;

            if (!_mappingService.ValidMappingExistsFor<UserApiKeyResponse, ApiKey>(param.OrderBy))
            {
                return this.GetGenericProblem(HttpStatusCode.BadRequest, $"Sorting using '{param.OrderBy}' is not possible");
            }

            param.SetSortingMappingSourceTypeName(nameof(UserApiKeyResponse));
            param.SetSortingMappingDestinationTypeName(nameof(ApiKey));


            var result = await _identityService.GetUserApiKeysAsync(param);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            this.SetPaginationInfoHeader(result.Object);

            return Ok(_mapper.Map<IEnumerable<UserApiKeyResponse>>(result.Object));
        }

        /// <summary>
        /// Delete a key for the given user.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="apiKey">Key to delete</param>
        /// <returns></returns>
        /// <response code="204">Operation completed successful</response>
        /// <response code="404">Specified user Id or key not found</response>
        [Produces("application/problem+json", "application/problem+xml")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{apiKey}")]
        public async Task<ActionResult<UserApiKeyResponse>> DeleteUserKey(string userId, string apiKey)
        {
            var result = await _identityService.DeleteUserApiKeyAsync(apiKey, userId);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
            }

            return NoContent();
        }


        /// <summary>
        /// Update an user key - partial update. Must use 'application/json-patch+json' content type in request header an the body must be a json patch document.
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <param name="apiKey">key to update</param>
        /// <param name="patchDocument">document with changes</param>
        /// <response code="204">Operation completed successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Specified user Id ok key not found</response>
        /// <response code="422">Validation errors</response>
        [ProducesCustom("application/problem+json", "application/problem+xml")]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{apiKey}")]
        public async Task<IActionResult> UpdateKeyPartial(string userId, string apiKey, JsonPatchDocument<UserApiKeyUpdateRequest> patchDocument)
        {

            var resultGet = await _identityService.GetUserByUsername(userId);
            if (!resultGet.Success)
            {
                return this.GetGenericProblem(resultGet);
            }

            var resultKey = await _identityService.GetUserApiKeyAsync(apiKey, userId);

            if (!resultKey.Success)
            {
                return this.GetGenericProblem(resultKey);
            }

            var apiKeyFromRepo = resultKey.Object;

            var apiKeyDetailsToPatch = _mapper.Map<UserApiKeyUpdateRequest>(apiKeyFromRepo);

            try
            {
                patchDocument.ApplyTo(apiKeyDetailsToPatch, ModelState);
            }
            catch (Exception e)
            {
                return this.GetGenericProblem(HttpStatusCode.BadRequest, e.Message);
            }


            if (!TryValidateModel(apiKeyDetailsToPatch))
            {
                // ValidationProblem is overriden because the base method doesn't use api behaviour and doesn't use http 422 as we have configured
                return ValidationProblem(ModelState);
            }

            _mapper.Map(apiKeyDetailsToPatch, apiKeyFromRepo);

            var result = await _identityService.UpdateApiKeyAsync(apiKeyFromRepo);

            if (!result.Success)
            {
                return this.GetGenericProblem(result);
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
