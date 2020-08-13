namespace JN.ApiDemo.Contracts.V1.Admin.Requests
{
    /// <summary>
    /// Request for a new key 
    /// </summary>
    public class UserApiKeyRequest
    {
        /// <summary>
        /// Key to create - leave empty to create automatically
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Activate the key (ready for use)
        /// </summary>
        public bool Active { get; set; }
    }
}