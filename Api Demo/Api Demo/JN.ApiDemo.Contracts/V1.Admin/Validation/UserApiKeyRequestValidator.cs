using FluentValidation;
using JN.ApiDemo.Contracts.V1.Admin.Requests;

namespace JN.ApiDemo.Contracts.V1.Admin.Validation
{
    public class UserApiKeyRequestValidator : AbstractValidator<UserApiKeyRequest>
    {
        public UserApiKeyRequestValidator()
        {
            RuleFor(p => p.Key).MaximumLength(255);

        }
    }
}