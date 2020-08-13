namespace JN.ApiDemo.Contracts.V1.Admin.Responses
{
    public class UserApiKeyResponse
    {
        public string Key { get; set; }
        public string CreationDate { get; set; }
        public bool Active { get; set; }
        public string UserId { get; set; }
    }
}