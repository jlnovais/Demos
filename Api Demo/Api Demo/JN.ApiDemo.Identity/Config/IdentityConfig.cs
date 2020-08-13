namespace JN.ApiDemo.Identity.Config
{
    public class IdentityConfig
    {
        public string ConStringConfigName { get; set; }
        public long DefaultLockoutTimeMinutes { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }
}
