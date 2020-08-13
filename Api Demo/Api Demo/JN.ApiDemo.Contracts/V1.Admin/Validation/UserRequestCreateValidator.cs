using FluentValidation;
using JN.ApiDemo.Contracts.V1.Admin.Requests;

namespace JN.ApiDemo.Contracts.V1.Admin.Validation
{
    public class UserRequestCreateValidator : AbstractValidator<UserRequestCreate>
    {
        public UserRequestCreateValidator()
        {
            Include(new UserRequestValidator());

            RuleFor(p => p.Password).NotEmpty().MinimumLength(5);
        }
    }
}