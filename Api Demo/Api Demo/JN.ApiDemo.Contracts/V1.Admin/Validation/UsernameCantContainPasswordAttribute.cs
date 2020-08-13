using System.ComponentModel.DataAnnotations;
using JN.ApiDemo.Contracts.V1.Admin.Requests;

namespace JN.ApiDemo.Contracts.V1.Admin.Validation
{
    public class UsernameCantContainPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = (UserRequest)validationContext.ObjectInstance;

            if(string.IsNullOrEmpty(user.Username))
                return ValidationResult.Success;

            if (!string.IsNullOrEmpty(user.Password) && user.Username.ToUpper().Contains(user.Password.ToUpper()))
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(UserRequest) });
            }

            return ValidationResult.Success;
        }
    }
}
