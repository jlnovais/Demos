namespace JN.ApiDemo.Contracts.V1.Admin.Requests
{
    /// <summary>
    /// Key update request
    /// </summary>
    public class UserApiKeyUpdateRequest
    {
        /// <summary>
        /// Key is active
        /// </summary>
        public bool Active { get; set; }
    }
}