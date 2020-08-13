using Microsoft.AspNetCore.Identity;

namespace JN.ApiDemo.Identity.Domain
{

    // extending default IdentityUser class for AspNetCore.Identity
    // IdentityRole --> normal table with string key;  IdentityRole<int> --> for using a table with int key
    // Important: Replace IdentityRole by ApplicationRole everywhere

    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        {
        }
        public ApplicationRole(string roleName):base(roleName)
        {
        }
    }
}