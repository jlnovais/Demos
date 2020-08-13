using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using JN.ApiDemo.Identity.Services;
using JN.Authentication.HelperClasses;
using JN.Authentication.Interfaces;

namespace JN.ApiDemo.AdminAPI.Services
{

    public static class ConstantsAuthentication
    {
        public enum AuthResult
        {
            Ok,
            NotFound = -1,
            InvalidUserOrPass = -2,
            NotAllowed = -3
        }
    }

    public class BasicValidationService : IBasicValidationService
    {
        private readonly IIdentityService _identityService;

        public BasicValidationService(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        public async Task<ValidationResult> ValidateUser(string username, string password, string resourceName)
        {

            var resUser = await _identityService.GetUserByUsername(username, password, true);

            var details = resUser.Object;

            if (!resUser.Success)
            {
                return new ValidationResult()
                {
                    Success = false,
                    ErrorDescription = resUser.Errors?.Aggregate((i, j) => i + ";" + j),
                    ErrorCode = (int)ConstantsAuthentication.AuthResult.InvalidUserOrPass
                };
            }

            if (!details.User.Active)
            {
                return new ValidationResult()
                {
                    Success = false,
                    ErrorDescription = "User not active", 
                    ErrorCode = (int)ConstantsAuthentication.AuthResult.NotAllowed
                };
            }


            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, details.User.Email),
            };

            if(details.Roles != null)
            {
                claims.AddRange(details.Roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            }

            var res = new ValidationResult()
            {
                Success = true,
                ErrorDescription = "Ok",
                ErrorCode = (int)ConstantsAuthentication.AuthResult.Ok,
                Claims = claims
            };

            return res;
        }



        public static Task<ChallengeResult> ChallengeResponse(Exception ex)
        {
            var res = new ChallengeResult();

            if (ex == null)
            {
                return Task.FromResult(res);
            }

            if (ex is CustomAuthException exception)
            {

                // throw exceptions to let them be processed by CustomExceptionHandler (in order to produce a standard exception - with ProblemDetails object)
                switch (exception.ErrorCode)
                {
                    case (int)ConstantsAuthentication.AuthResult.InvalidUserOrPass:
                    case (int)AuthenticationError.AuthenticationFailed:
                        exception.ErrorCode = (int)HttpStatusCode.Unauthorized;
                        throw exception;

                    case (int)ConstantsAuthentication.AuthResult.NotAllowed:
                        exception.ErrorCode = (int)HttpStatusCode.Forbidden;
                        throw exception;

                    case (int)AuthenticationError.MethodNotAllowed:
                        exception.ErrorCode = (int)HttpStatusCode.MethodNotAllowed;
                        throw exception;

                    case (int)AuthenticationError.OtherError:
                        throw exception;

                    default:
                        res.textToWriteOutput = exception.Message;
                        break;
                }
            }
            else
            {
                res.textToWriteOutput = ex.Message;
            }

            return Task.FromResult(res);
        }


    }
}
