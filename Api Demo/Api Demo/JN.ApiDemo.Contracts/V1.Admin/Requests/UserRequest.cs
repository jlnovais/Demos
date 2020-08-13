using JN.ApiDemo.Contracts.V1.Admin.Validation;

namespace JN.ApiDemo.Contracts.V1.Admin.Requests
{

    public abstract class UserRequest
    {
        [UsernameCantContainPassword(ErrorMessage = "Username name can't contain password")]
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string NotificationEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; }

        public string Roles { get; set; }
    }
}