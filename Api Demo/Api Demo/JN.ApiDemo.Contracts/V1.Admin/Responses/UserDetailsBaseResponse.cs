namespace JN.ApiDemo.Contracts.V1.Admin.Responses
{
    public abstract class UserDetailsBaseResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

    }
}