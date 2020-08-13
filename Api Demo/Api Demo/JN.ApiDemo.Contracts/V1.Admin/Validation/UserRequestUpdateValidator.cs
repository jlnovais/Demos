using FluentValidation;
using JN.ApiDemo.Contracts.V1.Admin.Requests;

namespace JN.ApiDemo.Contracts.V1.Admin.Validation
{
    public class UserRequestUpdateValidator : AbstractValidator<UserRequestUpdate>
    {
        public UserRequestUpdateValidator()
        {
            Include(new UserRequestValidator());
        }
    }
}