using FluentValidation;
using JN.ApiDemo.Contracts.V1.Admin.Requests;

namespace JN.ApiDemo.Contracts.V1.Admin.Validation
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(p => p.Username).NotEmpty().MaximumLength(255);
            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("Invalid email address. Specify a unique address")
                .NotEmpty();
            RuleFor(p => p.NotificationEmail)
                .EmailAddress().WithMessage("Invalid email address.");
            RuleFor(p => p.FirstName).NotEmpty().Length(2, 255);

        }
    }
}
