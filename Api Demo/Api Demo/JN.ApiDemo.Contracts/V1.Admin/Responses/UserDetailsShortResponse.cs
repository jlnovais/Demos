namespace JN.ApiDemo.Contracts.V1.Admin.Responses
{
    public class UserDetailsShortResponse: UserDetailsBaseResponse, IUserDetails
    {
        public string Name { get; set; } //FirstName + LastName
    }
}